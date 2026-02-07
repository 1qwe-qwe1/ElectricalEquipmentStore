using ElectricalEquipmentStore.Data;
using ElectricalEquipmentStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ElectricalEquipmentStore.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Status)
                .Where(p => p.Status.Name == "В наличии" || p.Status.Name == "Доступен")
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Status)
                .Where(p => p.CategoryId == categoryId &&
                           (p.Status.Name == "В наличии" || p.Status.Name == "Доступен"))
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Status)
                .Where(p => p.Name.Contains(searchTerm) ||
                           p.Description.Contains(searchTerm) ||
                           p.Manufacturer.Name.Contains(searchTerm))
                .Where(p => p.Status.Name == "В наличии" || p.Status.Name == "Доступен")
                .ToListAsync();
        }
    }
}
