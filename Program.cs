using System.Text;
using System.Text.RegularExpressions;
using GenericApi.Models;
using GenericApi.Seed;
using GenericApi.Services.Auth;
using GenericApi.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

var builder = WebApplication.CreateBuilder(args);

// Configure AppDbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin() // For development only! Use .WithOrigins("https://your-frontend.com") in production
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings["Key"]
                        ?? throw new InvalidOperationException("JWT Key is not configured.")
                )
            ),
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("accessToken"))
                {
                    context.Token = context.Request.Cookies["accessToken"];
                }
                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission", policy => policy.Requirements.Add(new PermissionRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UsersService>();

builder.Services.AddHostedService<RefreshTokenCleanupService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Generic API", Version = "v1" }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ! Uncomment the next line to enable HTTPS redirection
// app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (args.Contains("--seed"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var keyCategorySeeder = new KeyCategoriesSeed(dbContext);
        var accountSeeder = new AccountsSeed(dbContext, app.Configuration);
        var modulesSeeder = new ModulesSeeder(dbContext);

        keyCategorySeeder.Seed();
        accountSeeder.Seed();

        modulesSeeder.Seed();
    }
    return;
}
if (args.Contains("--export"))
{
    using var scope = app.Services.CreateScope();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // Use the Name= syntax to read the connection string from configuration
    string connectionString = configuration.GetConnectionString("DefaultConnection")!;
    var sqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "DbScripts");
    string tablesPath = Path.Combine(outputPath, "Tables");
    string spsPath = Path.Combine(outputPath, "StoredProcedures");
    string viewsPath = Path.Combine(outputPath, "Views");
    if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);
    Directory.CreateDirectory(tablesPath);
    Directory.CreateDirectory(spsPath);
    Directory.CreateDirectory(viewsPath);

    ServerConnection serverConnection = sqlBuilder.IntegratedSecurity
        ? new ServerConnection(sqlBuilder.DataSource)
        : new ServerConnection(
            sqlBuilder.DataSource,
            sqlBuilder.UserID
                ?? throw new InvalidOperationException("SQL authentication requires UserID."),
            sqlBuilder.Password
                ?? throw new InvalidOperationException("SQL authentication requires Password.")
        );

    serverConnection.LoginSecure = sqlBuilder.IntegratedSecurity;
    var server = new Server(serverConnection);
    var database = server.Databases[sqlBuilder.InitialCatalog];

    foreach (Table table in database.Tables)
    {
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
        if (!table.IsSystemObject)
        {
            // Generate and clean script
            var scriptCollection = table.Script(scriptingOptions);
            string rawScript = string.Join(Environment.NewLine, scriptCollection.Cast<string>());
            string cleanedScript = Regex.Replace(
                rawScript,
                @"\s+COLLATE\s+\w+",
                "",
                RegexOptions.IgnoreCase
            );

            // Write main table script
            File.WriteAllText(Path.Combine(tablesPath, $"{table.Name}.sql"), cleanedScript);
            // Enumerate foreign key references for each column
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

                            // Save reference info
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
    }

    foreach (StoredProcedure sp in database.StoredProcedures)
    {
        if (!sp.IsSystemObject)
        {
            var options = new ScriptingOptions
            {
                ScriptDrops = false,
                IncludeHeaders = true,
                SchemaQualify = true,
                WithDependencies = false,
                DriAll = true, // Includes PKs, FKs, etc.
            };

            var scriptCollection = sp.Script(options);
            string script = string.Join(Environment.NewLine, scriptCollection.Cast<string>());

            // Append GO for proper batching
            string finalScript = $"{script}\nGO";

            File.WriteAllText(Path.Combine(spsPath, $"{sp.Schema}.{sp.Name}.sql"), finalScript);
        }
    }

    foreach (View view in database.Views)
    {
        if (!view.IsSystemObject)
        {
            var scriptCollection = view.Script();
            string script = string.Join(Environment.NewLine, scriptCollection.Cast<string>());
            File.WriteAllText(Path.Combine(viewsPath, $"{view.Name}.sql"), script);
        }
    }

    Console.WriteLine("Database objects exported successfully to DbScripts folder.");
    return;
}

if (args.Contains("--runscripts"))
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

    using var scope = app.Services.CreateScope();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")!;

    foreach (var file in sqlFiles)
    {
        Console.WriteLine($"Running script: {file}");
        var script = await File.ReadAllTextAsync(file);

        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
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

    return;
}
app.Run();
