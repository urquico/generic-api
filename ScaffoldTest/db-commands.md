# Database Commands for Migration

```sh
# For dropping the database
dotnet ef database drop --force
```

```sh
# For creating the database
dotnet ef database update
```

```sh
# For dropping and creating the database
dotnet ef database drop --force && dotnet ef database update
```
