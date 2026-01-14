using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Products;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Domain.Entities;


namespace TekTrov.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> SearchByNameAsync(string query)
        {
            return await _context.Products
                .Where(p =>
                    p.Name.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }

        public async Task<List<Product>> GetSortedAsync(ProductSortType sortType)
        {
            var query = _context.Products.AsQueryable();

            query = sortType switch
            {
                ProductSortType.PriceLow =>
                    query.OrderBy(p => p.Price),

                ProductSortType.PriceHigh =>
                    query.OrderByDescending(p => p.Price),

                ProductSortType.Rating =>
                    query.OrderByDescending(p => p.Rating),

                ProductSortType.Popular =>
                    query.OrderByDescending(p => p.SoldCount),

                _ => // Featured
                    query.OrderByDescending(p => p.CreatedOn)
            };

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<List<Product>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .OrderByDescending(p => p.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }





    }
}
