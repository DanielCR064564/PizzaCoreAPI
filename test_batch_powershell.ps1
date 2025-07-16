# PowerShell script to test batch factura sync endpoints

# Test Health Check first
Write-Host "Testing Health Endpoint..." -ForegroundColor Green
try {
    $healthResponse = Invoke-RestMethod -Uri "http://localhost:5200/api/sync/health" -Method GET -ContentType "application/json"
    Write-Host "Health Response: $($healthResponse | ConvertTo-Json -Depth 3)" -ForegroundColor Yellow
} catch {
    Write-Host "Health Check Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test Single Factura Sync with simpler data
Write-Host "`nTesting Single Factura Sync..." -ForegroundColor Green
$singleFacturaBody = @{
    idFactura = 5001
    fechaFactura = "2025-01-15T12:00:00"
    fkCliente = 1
    tipoEntrega = "Local"
    subtotal = 150.00
    iva = 27.00
    total = 177.00
    estado = "Pendiente"
    empleadoId = 1
    fechaCreacion = "2025-01-15T12:00:00"
    notas = "Test single factura"
    terminalId = "TERMINAL-001"
} | ConvertTo-Json -Depth 3

try {
    $singleResponse = Invoke-RestMethod -Uri "http://localhost:5200/api/sync/facturas" -Method POST -Body $singleFacturaBody -ContentType "application/json"
    Write-Host "Single Factura Response: $($singleResponse | ConvertTo-Json -Depth 3)" -ForegroundColor Yellow
} catch {
    Write-Host "Single Factura Error: $($_.Exception.Message)" -ForegroundColor Red
    $errorResponse = $_.Exception.Response
    if ($errorResponse) {
        $stream = $errorResponse.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Error Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test Batch Factura Sync with simpler data
Write-Host "`nTesting Batch Factura Sync..." -ForegroundColor Green
$batchFacturaBody = @{
    facturas = @(
        @{
            idFactura = 5002
            fechaFactura = "2025-01-15T10:30:00"
            fkCliente = 1
            tipoEntrega = "Local"
            subtotal = 100.00
            iva = 18.00
            total = 118.00
            estado = "Pendiente"
            empleadoId = 1
            fechaCreacion = "2025-01-15T10:30:00"
            notas = "Test batch factura 1"
            terminalId = "TERMINAL-001"
        },
        @{
            idFactura = 5003
            fechaFactura = "2025-01-15T11:00:00"
            fkCliente = 2
            tipoEntrega = "Domicilio"
            subtotal = 200.00
            iva = 36.00
            total = 236.00
            estado = "Pagado"
            empleadoId = 2
            fechaCreacion = "2025-01-15T11:00:00"
            notas = "Test batch factura 2"
            terminalId = "TERMINAL-001"
        },
        @{
            idFactura = 5004
            fechaFactura = "2025-01-15T12:00:00"
            fkCliente = 3
            tipoEntrega = "Para_Llevar"
            subtotal = 300.00
            iva = 54.00
            total = 354.00
            estado = "Pendiente"
            empleadoId = 3
            fechaCreacion = "2025-01-15T12:00:00"
            notas = "Test batch factura 3"
            terminalId = "TERMINAL-001"
        }
    )
    terminalId = "TERMINAL-001"
    syncTimestamp = "2025-01-15T11:30:00"
} | ConvertTo-Json -Depth 3

try {
    $batchResponse = Invoke-RestMethod -Uri "http://localhost:5200/api/sync/facturas/batch" -Method POST -Body $batchFacturaBody -ContentType "application/json"
    Write-Host "Batch Factura Response: $($batchResponse | ConvertTo-Json -Depth 3)" -ForegroundColor Yellow
} catch {
    Write-Host "Batch Factura Error: $($_.Exception.Message)" -ForegroundColor Red
    $errorResponse = $_.Exception.Response
    if ($errorResponse) {
        $stream = $errorResponse.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Error Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test Get Pending Facturas
Write-Host "`nTesting Get Pending Facturas..." -ForegroundColor Green
try {
    $pendingResponse = Invoke-RestMethod -Uri "http://localhost:5200/api/sync/facturas/pending" -Method GET -ContentType "application/json"
    Write-Host "Pending Facturas Response: $($pendingResponse | ConvertTo-Json -Depth 3)" -ForegroundColor Yellow
} catch {
    Write-Host "Pending Facturas Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test Get Sync Status
Write-Host "`nTesting Get Sync Status..." -ForegroundColor Green
try {
    $statusResponse = Invoke-RestMethod -Uri "http://localhost:5200/api/sync/facturas/status" -Method GET -ContentType "application/json"
    Write-Host "Sync Status Response: $($statusResponse | ConvertTo-Json -Depth 3)" -ForegroundColor Yellow
} catch {
    Write-Host "Sync Status Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nAll tests completed!" -ForegroundColor Green
