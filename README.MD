# Change to your Scripts folder (update path as needed)

cd "C:\Path\To\Your\Scripts"

## Execute all .sql files recursively, in order

Get-ChildItem -Recurse -Filter \*.sql | Sort-Object FullName | ForEach-Object {
Write-Host "Running script: $($\_.FullName)..."

    # Run sqlcmd with placeholders - update these before running
    sqlcmd -S "<SERVER_NAME>" -d "<DATABASE_NAME>" -E -i $_.FullName

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to run $($_.Name)" -ForegroundColor Red
        exit $LASTEXITCODE
    } else {
        Write-Host "Finished $($_.Name)" -ForegroundColor Green
    }

}

<SERVER_NAME> → Your SQL Server instance

<DATABASE_NAME> → Your target database

The path in cd "C:\Path\To\Your\Scripts" → Your scripts folder path.
