param(
    [Parameter(Mandatory = $true)]
    [string]$DatabaseName,

    [switch]$DropExisting
)

$ServerName = "DESKTOP-OL6L16D"

$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path

$CreateTablesScript = Join-Path $ScriptDirectory "01-CreateTables.sql"
$CreateConstraintsScript = Join-Path $ScriptDirectory "02-CreateConstraints.sql"
$SeedDataScript = Join-Path $ScriptDirectory "03-SeedTestData.sql"

Write-Host "Database: $DatabaseName"
Write-Host "Server:   $ServerName"
Write-Host ""

if ($DropExisting) {
    Write-Host "Dropping existing database '$DatabaseName'..."

    sqlcmd `
        -S $ServerName `
        -E `
        -C `
        -Q "IF DB_ID(N'$DatabaseName') IS NOT NULL DROP DATABASE [$DatabaseName];"

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to drop database '$DatabaseName'."
    }
}

Write-Host "Creating database '$DatabaseName' if it does not exist..."

sqlcmd `
    -S $ServerName `
    -E `
    -C `
    -Q "IF DB_ID(N'$DatabaseName') IS NULL CREATE DATABASE [$DatabaseName];"

if ($LASTEXITCODE -ne 0) {
    throw "Failed to create database '$DatabaseName'."
}

Write-Host "Creating tables..."

sqlcmd `
    -S $ServerName `
    -E `
    -C `
    -d $DatabaseName `
    -i $CreateTablesScript

if ($LASTEXITCODE -ne 0) {
    throw "Failed to create tables."
}

Write-Host "Creating constraints..."

sqlcmd `
    -S $ServerName `
    -E `
    -C `
    -d $DatabaseName `
    -i $CreateConstraintsScript

if ($LASTEXITCODE -ne 0) {
    throw "Failed to create constraints."
}

Write-Host "Seeding test data..."

sqlcmd `
    -S $ServerName `
    -E `
    -C `
    -d $DatabaseName `
    -i $SeedDataScript

if ($LASTEXITCODE -ne 0) {
    throw "Failed to seed test data."
}

Write-Host ""
Write-Host "Database '$DatabaseName' setup completed successfully."