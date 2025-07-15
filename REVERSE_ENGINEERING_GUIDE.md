# Entity Framework Reverse Engineering Update Guide

## Overview
This guide will help you update your PizzaCoreAPI codebase to match the current database structure after making changes in MySQL Workbench.

## Step 1: Review Current Database vs Code Differences

### Key Changes Found:
1. **Cliente Table**: Uses separate name fields (Nombres, ApPaterno, ApMaterno) instead of combined NombreCompleto
2. **Venta Table**: Uses string IDs instead of GUIDs in some places
3. **Empleado Table**: New table structure with proper fields
4. **Metodosdepago Table**: New table structure

## Step 2: Implementation Options

### Option A: Gradual Migration (Recommended)
Update existing models to match database structure while maintaining backward compatibility.

### Option B: Complete Replace
Replace existing models with scaffolded versions (more disruptive but cleaner).

## Step 3: Gradual Migration Implementation

### 3.1 Update Cliente Model
Replace the existing `Models/Cliente.cs` with `Models/ClienteUpdated.cs`:

```bash
# Backup current model
cp Models/Cliente.cs Models/Cliente.cs.backup

# Replace with updated model
cp Models/ClienteUpdated.cs Models/Cliente.cs
```

### 3.2 Update Empleado Model
Create/update the Empleado model:

```bash
# Create new Empleado model
cp Models/EmpleadoUpdated.cs Models/Empleado.cs
```

### 3.3 Update Venta Models
Update Venta models to match database structure:

```bash
# Backup current models
cp Models/Venta.cs Models/Venta.cs.backup

# Replace with updated models
cp Models/VentaUpdated.cs Models/Venta.cs
```

### 3.4 Update DbContext
Update the main DbContext to use the new model structures:

```bash
# Backup current context
cp Data/PizzaDbContext.cs Data/PizzaDbContext.cs.backup

# Use updated context
cp Data/UpdatedPizzaDbContext.cs Data/PizzaDbContext.cs
```

### 3.5 Update DTOs
Update DTOs to match new model structures:

```csharp
// Update ClienteDTO.cs
public class ClienteDTO
{
    public int IdCliente { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string ApPaterno { get; set; } = string.Empty;
    public string ApMaterno { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    // ... other properties
    
    // Computed property for backward compatibility
    public string NombreCompleto => $"{Nombres} {ApPaterno} {ApMaterno}".Trim();
}
```

### 3.6 Update Services
Update services to handle new field mappings:

```csharp
// Example: Update ClienteService.cs
public async Task<ClienteDTO> CreateClienteAsync(RegisterClienteDTO registerDto)
{
    var nameParts = registerDto.NombreCompleto.Split(' ', 3);
    var cliente = new Cliente
    {
        Nombres = nameParts.Length > 0 ? nameParts[0] : "",
        ApPaterno = nameParts.Length > 1 ? nameParts[1] : "",
        ApMaterno = nameParts.Length > 2 ? nameParts[2] : "",
        Email = registerDto.Email,
        Telefono = registerDto.Telefono,
        // ... other mappings
    };
    
    // ... rest of method
}
```

## Step 4: Update Database Connection

### 4.1 Register Updated DbContext
Update `Program.cs` to use the updated context:

```csharp
// Update Program.cs
builder.Services.AddDbContext<PizzaDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));
```

### 4.2 Update Service Registrations
Ensure all services are properly registered with the new model types.

## Step 5: Testing and Validation

### 5.1 Build and Test
```bash
# Build the project
dotnet build

# Run tests
dotnet test

# Run the application
dotnet run
```

### 5.2 Test Key Functionality
1. Test client creation and retrieval
2. Test sales synchronization
3. Test employee management
4. Test product synchronization

## Step 6: Migration Strategy

### 6.1 Create Migration for Any Missing Changes
```bash
# Create migration for any remaining schema differences
dotnet ef migrations add AlignWithDatabaseSchema

# Apply migration
dotnet ef database update
```

### 6.2 Verify Schema Alignment
```bash
# Check if schema matches database
mysql -u root -p -e "USE pizzacoredb; DESCRIBE cliente;"
mysql -u root -p -e "USE pizzacoredb; DESCRIBE empleado;"
mysql -u root -p -e "USE pizzacoredb; DESCRIBE ventas;"
```

## Step 7: Update Controllers

### 7.1 Update Controller Actions
Update controller actions to handle new model structures:

```csharp
// Example: Update ClientesController.cs
[HttpPost]
public async Task<ActionResult<ClienteDTO>> CreateCliente(RegisterClienteDTO registerDto)
{
    try
    {
        var cliente = await _clienteService.CreateClienteAsync(registerDto);
        return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, cliente);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error creating client", error = ex.Message });
    }
}
```

## Step 8: Update Sync Logic

### 8.1 Update SyncController
Update sync logic to handle new model types:

```csharp
// Update SyncController.cs
[HttpPost("ventas")]
public async Task<ActionResult<SyncVentaDTO>> SyncVenta(SyncVentaDTO syncDto)
{
    try
    {
        // Handle string ID conversion
        var ventaId = Guid.NewGuid().ToString();
        
        var venta = new VentaUpdated
        {
            Id = ventaId,
            NumeroOrden = syncDto.NumeroOrden,
            ClienteId = syncDto.ClienteId?.ToString(), // Handle int to string conversion
            // ... other mappings
        };
        
        // ... rest of method
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error syncing sale", error = ex.Message });
    }
}
```

## Step 9: Data Migration (if needed)

### 9.1 Create Data Migration Scripts
If you need to migrate existing data to new format:

```sql
-- Example: Split NombreCompleto into separate fields
UPDATE cliente 
SET 
    Nombres = SUBSTRING_INDEX(NombreCompleto, ' ', 1),
    ApPaterno = SUBSTRING_INDEX(SUBSTRING_INDEX(NombreCompleto, ' ', 2), ' ', -1),
    ApMaterno = SUBSTRING_INDEX(NombreCompleto, ' ', -1)
WHERE NombreCompleto IS NOT NULL;
```

## Step 10: Documentation and Cleanup

### 10.1 Update API Documentation
Update Swagger/OpenAPI documentation to reflect new model structures.

### 10.2 Clean Up
Remove backup files and temporary scaffolded files once everything is working.

## Troubleshooting

### Common Issues:
1. **Type Conversion Errors**: Ensure proper mapping between old and new types
2. **Foreign Key Constraints**: Update foreign key references to match new types
3. **Migration Conflicts**: Resolve any migration conflicts that arise

### Debug Commands:
```bash
# Check EF model state
dotnet ef dbcontext info

# Generate SQL for pending migrations
dotnet ef migrations script

# Check database connection
dotnet ef database update --verbose
```

## Final Notes

- Always backup your database before making changes
- Test thoroughly in a development environment first
- Consider creating a staging environment for testing
- Update documentation and API contracts as needed
- Monitor logs for any runtime errors after deployment

This approach allows you to gradually update your codebase while maintaining functionality and backward compatibility where possible.
