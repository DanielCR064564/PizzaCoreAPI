#!/bin/bash

# PizzaCoreAPI Database Migration Script
# This script helps implement the reverse engineering changes

echo "=== PizzaCoreAPI Database Migration Script ==="
echo ""

# Check if we're in the right directory
if [ ! -f "PizzaCoreAPI.csproj" ]; then
    echo "Error: Run this script from the PizzaCoreAPI root directory"
    exit 1
fi

echo "1. Backing up current models..."
mkdir -p backup/$(date +%Y%m%d_%H%M%S)
cp -r Models/ backup/$(date +%Y%m%d_%H%M%S)/Models_backup/
cp -r Data/ backup/$(date +%Y%m%d_%H%M%S)/Data_backup/

echo "2. Implementing model updates..."

# Update Cliente model
echo "   - Updating Cliente model..."
if [ -f "Models/ClienteUpdated.cs" ]; then
    cp Models/ClienteUpdated.cs Models/Cliente.cs
    echo "     ✓ Cliente model updated"
else
    echo "     ✗ ClienteUpdated.cs not found"
fi

# Update Empleado model
echo "   - Updating Empleado model..."
if [ -f "Models/EmpleadoUpdated.cs" ]; then
    cp Models/EmpleadoUpdated.cs Models/Empleado.cs
    echo "     ✓ Empleado model updated"
else
    echo "     ✗ EmpleadoUpdated.cs not found"
fi

# Update Venta models
echo "   - Updating Venta models..."
if [ -f "Models/VentaUpdated.cs" ]; then
    cp Models/VentaUpdated.cs Models/Venta.cs
    echo "     ✓ Venta models updated"
else
    echo "     ✗ VentaUpdated.cs not found"
fi

# Update DbContext
echo "   - Updating DbContext..."
if [ -f "Data/UpdatedPizzaDbContext.cs" ]; then
    cp Data/UpdatedPizzaDbContext.cs Data/PizzaDbContext.cs
    echo "     ✓ DbContext updated"
else
    echo "     ✗ UpdatedPizzaDbContext.cs not found"
fi

echo ""
echo "3. Building project..."
dotnet build

if [ $? -eq 0 ]; then
    echo "     ✓ Build successful"
else
    echo "     ✗ Build failed - check errors above"
    exit 1
fi

echo ""
echo "4. Creating migration for schema alignment..."
dotnet ef migrations add AlignWithDatabaseSchema

if [ $? -eq 0 ]; then
    echo "     ✓ Migration created"
else
    echo "     ✗ Migration failed - check errors above"
fi

echo ""
echo "5. Applying migration..."
dotnet ef database update

if [ $? -eq 0 ]; then
    echo "     ✓ Database updated"
else
    echo "     ✗ Database update failed - check errors above"
fi

echo ""
echo "6. Testing application..."
dotnet run --no-build &
APP_PID=$!

sleep 10

# Test if application started
if kill -0 $APP_PID 2>/dev/null; then
    echo "     ✓ Application started successfully"
    kill $APP_PID
else
    echo "     ✗ Application failed to start"
fi

echo ""
echo "=== Migration Complete ==="
echo ""
echo "Next steps:"
echo "1. Review the updated models in the Models/ directory"
echo "2. Update DTOs to match new model structures"
echo "3. Update services to handle new field mappings"
echo "4. Test all functionality thoroughly"
echo "5. Update API documentation"
echo ""
echo "Backup location: backup/$(date +%Y%m%d_%H%M%S)/"
