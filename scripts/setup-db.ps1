<#
PowerShell helper to prepare database, apply EF Core migrations and start the app so DbSeeder runs.
Usage (PowerShell):
  ./scripts/setup-db.ps1
#>
param(
    [string]$ProjectPath = "MiniCRM"
)

Write-Host "=== MiniCRM setup script (PowerShell) ==="

# Ensure dotnet-ef tool is available
try {
    dotnet ef --version > $null 2>&1
    Write-Host "dotnet-ef already installed"
}
catch {
    Write-Host "dotnet-ef not found — installing globally..."
    dotnet tool install --global dotnet-ef
}

# Apply migrations
Write-Host "Applying EF Core migrations for project: $ProjectPath"
Push-Location $ProjectPath
try {
    dotnet ef database update --context ApplicationDbContext
    Write-Host "Database updated"
}
finally {
    Pop-Location
}

# Start the app so seeding runs (Program.cs executes seeder at startup)
Write-Host "Starting application to run seeders. The process will stay running — stop it when done (Ctrl+C)."
Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project $ProjectPath" -Wait

Write-Host "Script finished. If you started the app in the foreground, stop it with Ctrl+C when seeding is done."
