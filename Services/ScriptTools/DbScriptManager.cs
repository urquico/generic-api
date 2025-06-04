using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

public static class DbScriptManager
{
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

        // Debug: list available databases
        Console.WriteLine("Available databases:");
        foreach (Database db in server.Databases)
        {
            Console.WriteLine($" - {db.Name}");
        }

        string dbName = sqlBuilder.InitialCatalog;
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
                var scriptingOptions = new ScriptingOptions
                {
                    ScriptDrops = false,
                    WithDependencies = false,
                    IncludeHeaders = true,
                    SchemaQualify = true,
                    Indexes = true,
                    DriAll = true,
                    Triggers = true,
                    ScriptDataCompression = true,
                    ScriptOwner = false,
                    NoCollation = false,
                };

                var scriptCollection = table.Script(scriptingOptions);
                string rawScript = string.Join(
                    Environment.NewLine,
                    scriptCollection.Cast<string>()
                );
                string cleanedScript = Regex.Replace(
                    rawScript,
                    @"\s+COLLATE\s+\w+",
                    "",
                    RegexOptions.IgnoreCase
                );

                File.WriteAllText(Path.Combine(tablesPath, $"{table.Name}.sql"), cleanedScript);

                foreach (ForeignKey fk in table.ForeignKeys)
                {
                    foreach (ForeignKeyColumn fkCol in fk.Columns)
                    {
                        string refDetails =
                            $@"Column '{fkCol.Name}' in table '{table.Schema}.{table.Name}' references
        '{fk.ReferencedTableSchema}.{fk.ReferencedTable}'({fkCol.ReferencedColumn})
        via foreign key '{fk.Name}'.";

                        File.AppendAllText(
                            Path.Combine(tablesPath, $"FK_Refs_{table.Schema}_{table.Name}.txt"),
                            refDetails + Environment.NewLine
                        );
                    }
                }
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

        var sqlFiles = Directory
            .GetFiles(scriptsPath, "*.sql", SearchOption.AllDirectories)
            .OrderBy(f => f);

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
