# Entity Framework Core Migration Guide

Scaffolding/Reverse Engineering Guide
Source: [Scaffolding (Reverse Engineering)
](https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli)

> This guide will help you scaffold your database, manage migrations, and update your database using Entity Framework Core in your .NET project.

<br>

1. **Scaffold the Database**
   Use the following command to scaffold your database and generate models from an existing SQL Server database:

```sh
dotnet ef dbcontext scaffold "Server=localhost;Database=<database>;User Id=<username>;Password=<password>;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c AppDbContext --force
```

**Instructions:**

- Replace `username` and `password` with your actual SQL Server credentials.
- This command will:
  - Connect to your SQL Server database named `database`.
  - Generate entity classes in the `Models` folder.
  - Use `AppDbContext` as the context class.
  - Overwrite existing files in the Models folder (`--force`).

2. **List Available Migrations & Create Migration**

```sh
dotnet ef migrations list
dotnet ef migrations add <MigrationFileName>
```

**Instructions:**

- This command will display a list of all migrations in your project.
- Use this to verify which migrations are available before applying or reverting them.

3. **Apply Migrations to the Database**

```sh
dotnet ef database update
```

**Instructions:**

- This command applies all pending migrations to your database.
- Make sure your connection string in `appsettings.json` or `appsettings.Development.json` is correct.
