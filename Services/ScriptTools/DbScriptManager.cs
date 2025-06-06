using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

public static class DbScriptManager
{
    private static readonly string[] TableDefaultOrder =
    [
        "mwss.key_categories.sql",
        "fmis.security_questions.sql",
        "fmis.modules.sql",
        "fmis.module_permissions.sql",
        "fmis.roles.sql",
        "fmis.role_module_permissions.sql",
        "fmis.users.sql",
        "fmis.user_roles.sql",
        "fmis.user.security_questions.sql",
        "fmis.user_special_permissions.sql",
        "fmis.refresh_tokens.sql",
    ];

    //<summary>
    /// Exports database objects (tables, stored procedures, views) to SQL scripts.
    public static void ExportScripts(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        string connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var sqlBuilder = new SqlConnectionStringBuilder(connectionString);

        Console.WriteLine($"Connecting to server: {sqlBuilder.DataSource}");
        Console.WriteLine($"Target database: {sqlBuilder.InitialCatalog}");

        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "DbScripts");
        string tablesPath = Path.Combine(outputPath, "Tables");
        string spsPath = Path.Combine(outputPath, "StoredProcedures");
        string viewsPath = Path.Combine(outputPath, "Views");

        Directory.CreateDirectory(outputPath);
        Directory.CreateDirectory(tablesPath);
        Directory.CreateDirectory(spsPath);
        Directory.CreateDirectory(viewsPath);

        var serverConnection = sqlBuilder.IntegratedSecurity
            ? new ServerConnection(sqlBuilder.DataSource)
            : new ServerConnection(sqlBuilder.DataSource, sqlBuilder.UserID!, sqlBuilder.Password!)
            {
                LoginSecure = sqlBuilder.IntegratedSecurity,
            };

        var server = new Server(serverConnection);

        string dbName = sqlBuilder.InitialCatalog;

        // Debug: list available databases
        Console.WriteLine("Available databases:");
        foreach (Database db in server.Databases)
        {
            if (db.Name == dbName)
                Console.WriteLine($" - {db.Name} (selected)");
            else
                Console.WriteLine($" - {db.Name}");
        }

        var database = server.Databases[dbName];

        if (database == null)
        {
            Console.WriteLine(
                $"Error: Database '{dbName}' not found on server '{sqlBuilder.DataSource}'."
            );
            return;
        }

        // === Script Tables ===
        foreach (Table table in database.Tables)
        {
            if (table.IsSystemObject)
                continue;

            try
            {
                var now = DateTime.Now;
                var scriptDate = now.ToString("M/d/yyyy h:mm:ss tt");
                var tableFile = Path.Combine(tablesPath, $"{table.Schema}.{table.Name}.sql");
                var sb = new System.Text.StringBuilder();

                sb.AppendLine(
                    $"/****** Object:  Table [{table.Schema}].[{table.Name}]    Script Date: {scriptDate} ******/"
                );
                sb.AppendLine("SET ANSI_NULLS ON");
                sb.AppendLine("GO");
                sb.AppendLine();
                sb.AppendLine("SET QUOTED_IDENTIFIER ON");
                sb.AppendLine("GO");
                sb.AppendLine();

                // Script CREATE TABLE with PK (but no FKs, no defaults, no checks, no uniques, no indexes)
                var so = new ScriptingOptions
                {
                    ScriptDrops = false,
                    WithDependencies = false,
                    IncludeHeaders = false,
                    SchemaQualify = true,
                    Indexes = false,
                    DriAll = false,
                    Triggers = false,
                    ScriptDataCompression = true,
                    ScriptOwner = false,
                    NoCollation = false,
                    DriPrimaryKey = true, // include PK in CREATE TABLE
                    DriClustered = true,
                    DriNonClustered = true,
                };
                var createTableScript = table.Script(so);
                if (createTableScript != null)
                {
                    foreach (var line in createTableScript)
                    {
                        sb.AppendLine(line);
                    }
                }
                sb.AppendLine("GO");
                sb.AppendLine();

                // Script defaults (DriDefaults)
                var soDefaults = new ScriptingOptions
                {
                    DriDefaults = true,
                    SchemaQualify = true,
                    IncludeHeaders = false,
                };
                var defaultsScript = table.Script(soDefaults);
                if (defaultsScript != null)
                {
                    foreach (var line in defaultsScript)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var trimmed = line.Trim();
                            if (
                                trimmed.StartsWith(
                                    "ALTER TABLE",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            {
                                sb.AppendLine(line);
                                sb.AppendLine("GO");
                                sb.AppendLine();
                            }
                        }
                    }
                }

                // Script foreign keys (DriForeignKeys)
                var soFKs = new ScriptingOptions
                {
                    DriForeignKeys = true,
                    SchemaQualify = true,
                    IncludeHeaders = false,
                };
                var fkScript = table.Script(soFKs);
                if (fkScript != null)
                {
                    foreach (var line in fkScript)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var trimmed = line.Trim();
                            if (
                                trimmed.StartsWith(
                                    "ALTER TABLE",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            {
                                sb.AppendLine(line);
                                sb.AppendLine("GO");
                                sb.AppendLine();
                            }
                        }
                    }
                }

                // Script check constraints (DriChecks)
                var soChecks = new ScriptingOptions
                {
                    DriChecks = true,
                    SchemaQualify = true,
                    IncludeHeaders = false,
                };
                var checkScript = table.Script(soChecks);
                if (checkScript != null)
                {
                    foreach (var line in checkScript)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var trimmed = line.Trim();
                            if (
                                trimmed.StartsWith(
                                    "ALTER TABLE",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            {
                                sb.AppendLine(line);
                                sb.AppendLine("GO");
                                sb.AppendLine();
                            }
                        }
                    }
                }

                // Script unique constraints (DriUniqueKeys)
                var soUniques = new ScriptingOptions
                {
                    DriUniqueKeys = true,
                    SchemaQualify = true,
                    IncludeHeaders = false,
                };
                var uniqueScript = table.Script(soUniques);
                if (uniqueScript != null)
                {
                    foreach (var line in uniqueScript)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var trimmed = line.Trim();
                            if (
                                trimmed.StartsWith(
                                    "ALTER TABLE",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            {
                                sb.AppendLine(line);
                                sb.AppendLine("GO");
                                sb.AppendLine();
                            }
                        }
                    }
                }

                // Script indexes (non-clustered, etc.)
                var soIndexes = new ScriptingOptions
                {
                    Indexes = true,
                    SchemaQualify = true,
                    IncludeHeaders = false,
                };
                var indexScript = table.Script(soIndexes);
                if (indexScript != null)
                {
                    foreach (var line in indexScript)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var trimmed = line.Trim();
                            // Only append CREATE INDEX or CREATE UNIQUE INDEX, never CREATE TABLE
                            if (
                                trimmed.StartsWith(
                                    "CREATE INDEX",
                                    StringComparison.OrdinalIgnoreCase
                                )
                                || trimmed.StartsWith(
                                    "CREATE UNIQUE INDEX",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            {
                                sb.AppendLine(line);
                                sb.AppendLine("GO");
                                sb.AppendLine();
                            }
                        }
                    }
                }

                // Always overwrite the file, do not append
                File.WriteAllText(tableFile, sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Warning: Could not script table {table.Schema}.{table.Name}: {ex.Message}"
                );
            }
        }

        Console.WriteLine("Finished scripting tables. Moving on to stored procedures...");

        // === Script Stored Procedures ===
        database.StoredProcedures.Refresh();

        int count = 0;
        foreach (StoredProcedure sp in database.StoredProcedures)
        {
            if (sp == null || sp.IsSystemObject || sp.IsEncrypted || sp.Schema != "dbo")
                continue;

            try
            {
                sp.Refresh();
                var options = new ScriptingOptions
                {
                    ScriptDrops = false,
                    IncludeHeaders = true,
                    SchemaQualify = true,
                    WithDependencies = false,
                    DriAll = true,
                };

                var scriptCollection = sp.Script(options);
                string script = string.Join(Environment.NewLine, scriptCollection.Cast<string>());
                string finalScript = $"{script}\nGO";

                File.WriteAllText(Path.Combine(spsPath, $"{sp.Schema}.{sp.Name}.sql"), finalScript);
                Console.WriteLine($"Scripted: {sp.Schema}.{sp.Name}");
                count++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scripting {sp.Schema}.{sp.Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        Console.WriteLine($"Successfully scripted {count} user-defined stored procedures.");

        // === Script Views ===
        foreach (View view in database.Views)
        {
            if (view.IsSystemObject)
                continue;

            try
            {
                var scriptCollection = view.Script();
                string script = string.Join(Environment.NewLine, scriptCollection.Cast<string>());
                File.WriteAllText(Path.Combine(viewsPath, $"{view.Name}.sql"), script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scripting view {view.Schema}.{view.Name}: {ex.Message}");
            }
        }

        Console.WriteLine("Database objects exported successfully to DbScripts folder.");
    }

    public static async Task RunScriptsAsync(IServiceProvider services)
    {
        var scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "DbScripts");

        if (!Directory.Exists(scriptsPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"DbScripts directory not found: {scriptsPath}");
            Console.ResetColor();
            return;
        }

        // Get all .sql files
        var allSqlFiles = Directory
            .GetFiles(scriptsPath, "*.sql", SearchOption.AllDirectories)
            .ToList();

        // Separate table scripts and others
        var tablesPath = Path.Combine(scriptsPath, "Tables");
        var tableScripts = new List<string>();
        var otherScripts = new List<string>();
        foreach (var file in allSqlFiles)
        {
            if (file.StartsWith(tablesPath))
                tableScripts.Add(file);
            else
                otherScripts.Add(file);
        }

        // Order table scripts by TableDefaultOrder
        var orderedTableScripts = new List<string>();
        foreach (var name in TableDefaultOrder)
        {
            var match = tableScripts.FirstOrDefault(f =>
                f.Replace("\\", "/").EndsWith($"Tables/{name}", StringComparison.OrdinalIgnoreCase)
            );
            if (match != null)
                orderedTableScripts.Add(match);
        }
        // Add any remaining table scripts not in the default order
        orderedTableScripts.AddRange(tableScripts.Except(orderedTableScripts));

        // Combine ordered table scripts and other scripts (other scripts after tables)
        var sqlFiles = orderedTableScripts.Concat(otherScripts).ToList();

        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        foreach (var file in sqlFiles)
        {
            Console.WriteLine($"Running script: {file}");
            var script = await File.ReadAllTextAsync(file);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                var batches = Regex.Split(
                    script,
                    @"^\s*GO\s*$",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase
                );

                foreach (var batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch))
                        continue;

                    command.CommandText = batch;
                    await command.ExecuteNonQueryAsync();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Finished {Path.GetFileName(file)}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to run {Path.GetFileName(file)}");
                Console.WriteLine(ex.Message);
                break;
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
