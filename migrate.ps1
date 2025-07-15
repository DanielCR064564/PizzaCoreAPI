# PizzaCoreAPI Database Migration Script (PowerShell)
# This script helps implement the reverse engineering changes

Write-Host "=== PizzaCoreAPI Database Migration Script ===" -ForegroundColor Green
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "PizzaCoreAPI.csproj")) {
    Write-Host "Error: Run this script from the PizzaCoreAPI root directory" -ForegroundColor Red
    exit 1
}

Write-Host "1. Backing up current models..." -ForegroundColor Yellow
$backupDir = "backup\$(Get-Date -Format 'yyyyMMdd_HHmmss')"
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
Copy-Item -Path "Models" -Destination "$backupDir\Models_backup" -Recurse -Force
Copy-Item -Path "Data" -Destination "$backupDir\Data_backup" -Recurse -Force

Write-Host "2. Implementing model updates..." -ForegroundColor Yellow

# Update Cliente model
Write-Host "   - Updating Cliente model..."
if (Test-Path "Models\ClienteUpdated.cs") {
    Copy-Item "Models\ClienteUpdated.cs" "Models\Cliente.cs" -Force
    Write-Host "     ✓ Cliente model updated" -ForegroundColor Green
} else {
    Write-Host "     ✗ ClienteUpdated.cs not found" -ForegroundColor Red
}

# Update Empleado model
Write-Host "   - Updating Empleado model..."
if (Test-Path "Models\EmpleadoUpdated.cs") {
    Copy-Item "Models\EmpleadoUpdated.cs" "Models\Empleado.cs" -Force
    Write-Host "     ✓ Empleado model updated" -ForegroundColor Green
} else {
    Write-Host "     ✗ EmpleadoUpdated.cs not found" -ForegroundColor Red
}

# Update Venta models
Write-Host "   - Updating Venta models..."
if (Test-Path "Models\VentaUpdated.cs") {
    Copy-Item "Models\VentaUpdated.cs" "Models\Venta.cs" -Force
    Write-Host "     ✓ Venta models updated" -ForegroundColor Green
} else {
    Write-Host "     ✗ VentaUpdated.cs not found" -ForegroundColor Red
}

# Update DbContext
Write-Host "   - Updating DbContext..."
if (Test-Path "Data\UpdatedPizzaDbContext.cs") {
    Copy-Item "Data\UpdatedPizzaDbContext.cs" "Data\PizzaDbContext.cs" -Force
    Write-Host "     ✓ DbContext updated" -ForegroundColor Green
} else {
    Write-Host "     ✗ UpdatedPizzaDbContext.cs not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "3. Building project..." -ForegroundColor Yellow
$buildResult = dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "     ✓ Build successful" -ForegroundColor Green
} else {
    Write-Host "     ✗ Build failed - check errors above" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "4. Creating migration for schema alignment..." -ForegroundColor Yellow
$migrationResult = dotnet ef migrations add AlignWithDatabaseSchema
if ($LASTEXITCODE -eq 0) {
    Write-Host "     ✓ Migration created" -ForegroundColor Green
} else {
    Write-Host "     ✗ Migration failed - check errors above" -ForegroundColor Red
}

Write-Host ""
Write-Host "5. Applying migration..." -ForegroundColor Yellow
$updateResult = dotnet ef database update
if ($LASTEXITCODE -eq 0) {
    Write-Host "     ✓ Database updated" -ForegroundColor Green
} else {
    Write-Host "     ✗ Database update failed - check errors above" -ForegroundColor Red
}

Write-Host ""
Write-Host "6. Testing application..." -ForegroundColor Yellow
$appProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --no-build" -PassThru -NoNewWindow

Start-Sleep -Seconds 10

# Test if application started
if (-not $appProcess.HasExited) {
    Write-Host "     ✓ Application started successfully" -ForegroundColor Green
    $appProcess.Kill()
} else {
    Write-Host "     ✗ Application failed to start" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Migration Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Review the updated models in the Models/ directory"
Write-Host "2. Update DTOs to match new model structures"
Write-Host "3. Update services to handle new field mappings"
Write-Host "4. Test all functionality thoroughly"
Write-Host "5. Update API documentation"
Write-Host ""
Write-Host "Backup location: $backupDir" -ForegroundColor Cyan
