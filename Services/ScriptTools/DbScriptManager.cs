using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

public static class DbScriptManager
{
    //<summary>
    /// Exports database objects (tables, stored procedures, views) to SQL scripts.
    public static Task ExportScriptsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        string connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var sqlBuilder = new SqlConnectionStringBuilder(connectionString);

        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "DbScripts");
        string tablesPath = Path.Combine(outputPath, "Tables");
        string spsPath = Path.Combine(outputPath, "StoredProcedures");
        string viewsPath = Path.Combine(outputPath, "Views");

        Directory.CreateDirectory(tablesPath);
        Directory.CreateDirectory(spsPath);
        Directory.CreateDirectory(viewsPath);

        var serverConnection = sqlBuilder.IntegratedSecurity
            ? new ServerConnection(sqlBuilder.DataSource)
            : new ServerConnection(sqlBuilder.DataSource, sqlBuilder.UserID!, sqlBuilder.Password!);

        serverConnection.LoginSecure = sqlBuilder.IntegratedSecurity;
        var server = new Server(serverConnection);
        var database = server.Databases[sqlBuilder.InitialCatalog];

        foreach (Table table in database.Tables)
        {
            if (table.IsSystemObject)
                continue;

            var scriptingOptions = new ScriptingOptions
            {
                ScriptDrops = false,
                WithDependencies = true,
                IncludeHeaders = true,
                SchemaQualify = true,
                Indexes = true,
                DriPrimaryKey = true,
                DriForeignKeys = true,
                DriAll = true,
                Triggers = true,
                DriAllConstraints = true,
                DriIndexes = true,
                ScriptDataCompression = true,
                ScriptOwner = false,
                NoCollation = false,
                IncludeIfNotExists = false,
            };

            var scriptCollection = table.Script(scriptingOptions);
            string rawScript = string.Join(Environment.NewLine, scriptCollection.Cast<string>());
            string cleanedScript = Regex.Replace(
                rawScript,
                @"\s+COLLATE\s+\w+",
                "",
                RegexOptions.IgnoreCase
            );

            File.WriteAllText(Path.Combine(tablesPath, $"{table.Name}.sql"), cleanedScript);

            foreach (Column col in table.Columns)
            {
                foreach (ForeignKey fk in table.ForeignKeys)
                {
                    foreach (ForeignKeyColumn fkCol in fk.Columns)
                    {
                        if (fkCol.Name == col.Name)
                        {
                            string refDetails =
                                $@"Column '{col.Name}' in table '{table.Schema}.{table.Name}' references 
'{fk.ReferencedTableSchema}.{fk.ReferencedTable}'({fkCol.ReferencedColumn}) 
via foreign key '{fk.Name}'.";

                            File.AppendAllText(
                                Path.Combine(
                                    tablesPath,
                                    $"FK_Refs_{table.Schema}_{table.Name}.txt"
                                ),
                                refDetails + Environment.NewLine
                            );
                        }
                    }
                }
            }
        }

        foreach (StoredProcedure sp in database.StoredProcedures)
        {
            if (sp.IsSystemObject)
                continue;

            var options = new ScriptingOptions
            {
                ScriptDrops = false,
                IncludeHeaders = true,
                SchemaQualify = true,
                WithDependencies = false,
                DriAll = true,
            };

            var scriptCollection = sp.Script(options);
            string script =
                string.Join(Environment.NewLine, scriptCollection.Cast<string>()) + "\nGO";
            File.WriteAllText(Path.Combine(spsPath, $"{sp.Name}.sql"), script);
        }

        foreach (View view in database.Views)
        {
            if (view.IsSystemObject)
                continue;

            var scriptCollection = view.Script();
            string script = string.Join(Environment.NewLine, scriptCollection.Cast<string>());
            File.WriteAllText(Path.Combine(viewsPath, $"{view.Name}.sql"), script);
        }

        Console.WriteLine("Database objects exported successfully to DbScripts folder.");
        return Task.CompletedTask;
    }

    // Runs all SQL scripts in the DbScripts directory against the database.
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
                //comment
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
