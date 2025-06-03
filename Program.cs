using System.Text;
using GenericApi.Models;
using GenericApi.Seed;
using GenericApi.Services.Auth;
using GenericApi.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load appsettings.Development.json if --dev argument is provided
var useDevSettings = args.Contains("--dev");
if (useDevSettings)
{
    builder.Configuration.AddJsonFile(
        "appsettings.Development.json",
        optional: true,
        reloadOnChange: true
    );
}

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
    await DbScriptManager.ExportScriptsAsync(app.Services);
    return;
}

if (args.Contains("--runscripts"))
{
    await DbScriptManager.RunScriptsAsync(app.Services);
    return;
}
app.Run();
