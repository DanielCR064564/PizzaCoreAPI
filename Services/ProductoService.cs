using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Services
{
    public class ProductoService : IProductoService
    {
        private readonly PizzaDbContext _context;

        public ProductoService(PizzaDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoDTO>> GetAllProductosAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.Ingredientes)
                .ToListAsync();

            return productos.Select(p => new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Tipo = p.Tipo.ToString(),
                Disponible = p.Disponible,
                Ingredientes = p.Ingredientes.Select(i => new IngredienteDTO
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    PrecioAdicional = i.PrecioAdicional,
                    Tipo = i.Tipo.ToString()
                }).ToList()
            }).ToList();
        }

        public async Task<ProductoDTO> GetProductoByIdAsync(Guid id)
        {
            var producto = await _context.Productos
                .Include(p => p.Ingredientes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return null;

            return new ProductoDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Tipo = producto.Tipo.ToString(),
                Disponible = producto.Disponible,
                Ingredientes = producto.Ingredientes.Select(i => new IngredienteDTO
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    PrecioAdicional = i.PrecioAdicional,
                    Tipo = i.Tipo.ToString()
                }).ToList()
            };
        }

        public async Task<ProductoDTO> CreateProductoAsync(CrearProductoDTO productoDto)
        {
            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                Precio = productoDto.Precio,
                Tipo = Enum.Parse<TipoProducto>(productoDto.Tipo, ignoreCase: true),
                Disponible = productoDto.Disponible
            };

            if (productoDto.IngredientesIds != null && productoDto.IngredientesIds.Any())
            {
                var ingredientes = await _context.Ingredientes
                    .Where(i => productoDto.IngredientesIds.Contains(i.Id))
                    .ToListAsync();

                producto.Ingredientes = ingredientes;
            }

            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            return new ProductoDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Tipo = producto.Tipo.ToString(),
                Disponible = producto.Disponible,
                Ingredientes = producto.Ingredientes.Select(i => new IngredienteDTO
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    PrecioAdicional = i.PrecioAdicional,
                    Tipo = i.Tipo.ToString()
                }).ToList()
            };
        }

        public async Task UpdateProductoAsync(Guid id, CrearProductoDTO productoDto)
        {
            var producto = await _context.Productos
                .Include(p => p.Ingredientes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return;

            producto.Nombre = productoDto.Nombre;
            producto.Descripcion = productoDto.Descripcion;
            producto.Precio = productoDto.Precio;
            producto.Tipo = Enum.Parse<TipoProducto>(productoDto.Tipo, ignoreCase: true);
            producto.Disponible = productoDto.Disponible;

            if (productoDto.IngredientesIds != null)
            {
                var ingredientes = await _context.Ingredientes
                    .Where(i => productoDto.IngredientesIds.Contains(i.Id))
                    .ToListAsync();

                producto.Ingredientes = ingredientes;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductoAsync(Guid id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ProductoDTO>> GetProductosDisponiblesAsync()
        {
            var productos = await _context.Productos
                .Where(p => p.Disponible)
                .Include(p => p.Ingredientes)
                .ToListAsync();

            return productos.Select(p => new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Tipo = p.Tipo.ToString(),
                Disponible = p.Disponible,
                Ingredientes = p.Ingredientes.Select(i => new IngredienteDTO
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    PrecioAdicional = i.PrecioAdicional,
                    Tipo = i.Tipo.ToString()
                }).ToList()
            }).ToList();
        }

        public async Task<decimal> CalcularPrecioConIngredientesAsync(Guid productoId, List<Guid> ingredientesIds)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
                return 0;

            var precioBase = producto.Precio;
            var ingredientes = await _context.Ingredientes
                .Where(i => ingredientesIds.Contains(i.Id))
                .ToListAsync();

            var precioAdicional = ingredientes.Sum(i => i.PrecioAdicional);
            return precioBase + precioAdicional;
        }

        public async Task<bool> VerificarDisponibilidadAsync(Guid productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            return producto != null && producto.Disponible;
        }
    }
}
