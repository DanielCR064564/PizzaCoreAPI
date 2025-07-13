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
    public class MenuService : IMenuService
    {
        private readonly PizzaDbContext _context;
        private readonly IProductoService _productoService;

        public MenuService(PizzaDbContext context, IProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        public async Task<List<MenuDTO>> GetAllMenusAsync()
        {
            var menus = await _context.Menus
                .Include(m => m.Productos)
                .ToListAsync();

            return menus.Select(m => new MenuDTO
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                PrecioTotal = m.PrecioTotal,
                Activo = m.Activo,
                Productos = m.Productos.Select(p => new ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Tipo = p.Tipo.ToString(),
                    Disponible = p.Disponible
                }).ToList()
            }).ToList();
        }

        public async Task<MenuDTO> GetMenuByIdAsync(Guid id)
        {
            var menu = await _context.Menus
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menu == null)
                return null;

            return new MenuDTO
            {
                Id = menu.Id,
                Nombre = menu.Nombre ?? string.Empty,
                Descripcion = menu.Descripcion ?? string.Empty,
                PrecioTotal = menu.PrecioTotal,
                Activo = menu.Activo,
                Productos = menu.Productos.Select(p => new ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Tipo = p.Tipo.ToString(),
                    Disponible = p.Disponible
                }).ToList()
            };
        }

        public async Task<MenuDTO> CreateMenuAsync(CrearMenuDTO menuDto)
        {
            var menu = new Menu
            {
                Nombre = menuDto.Nombre,
                Descripcion = menuDto.Descripcion,
                Activo = menuDto.Activo
            };

            if (menuDto.ProductosIds != null && menuDto.ProductosIds.Any())
            {
                var productos = await _context.Productos
                    .Where(p => menuDto.ProductosIds.Contains(p.Id))
                    .ToListAsync();
                menu.Productos = productos;

                // Calcular precio total del menú
                menu.PrecioTotal = productos.Sum(p => p.Precio);
            }

            await _context.Menus.AddAsync(menu);
            await _context.SaveChangesAsync();

            return new MenuDTO
            {
                Id = menu.Id,
                Nombre = menu.Nombre ?? string.Empty,
                Descripcion = menu.Descripcion ?? string.Empty,
                PrecioTotal = menu.PrecioTotal,
                Activo = menu.Activo,
                Productos = menu.Productos.Select(p => new ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Tipo = p.Tipo.ToString(),
                    Disponible = p.Disponible
                }).ToList()
            };
        }

        public async Task UpdateMenuAsync(Guid id, CrearMenuDTO menuDto)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return;

            menu.Nombre = menuDto.Nombre;
            menu.Descripcion = menuDto.Descripcion;
            menu.Activo = menuDto.Activo;

            if (menuDto.ProductosIds != null && menuDto.ProductosIds.Any())
            {
                var productos = await _context.Productos
                    .Where(p => menuDto.ProductosIds.Contains(p.Id))
                    .ToListAsync();
                menu.Productos = productos;

                // Recalcular precio total del menú
                menu.PrecioTotal = productos.Sum(p => p.Precio);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMenuAsync(Guid id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MenuDTO>> GetMenusActivosAsync()
        {
            var menus = await _context.Menus
                .Where(m => m.Activo)
                .Include(m => m.Productos)
                .ToListAsync();

            return menus.Select(m => new MenuDTO
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                PrecioTotal = m.PrecioTotal,
                Activo = m.Activo,
                Productos = m.Productos.Select(p => new ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Tipo = p.Tipo.ToString(),
                    Disponible = p.Disponible
                }).ToList()
            }).ToList();
        }

        public async Task<decimal> CalcularPrecioMenuAsync(Guid menuId)
        {
            var menu = await _context.Menus
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == menuId);

            if (menu == null)
                return 0;

            return menu.Productos.Sum(p => p.Precio);
        }

        public async Task<bool> VerificarDisponibilidadMenuAsync(Guid menuId)
        {
            var menu = await _context.Menus
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == menuId);

            if (menu == null)
                return false;

            // Un menú está disponible si está activo y todos sus productos están disponibles
            return menu.Activo && menu.Productos.All(p => p.Disponible);
        }
    }
}
