using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Domain.Entities;

namespace TekTrov.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetAsync(int userId, int productId)
    {
        return await _context.Carts
            .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.ProductId == productId);
    }

    public async Task AddAsync(Cart cart)
    {
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cart cart)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Cart cart)   
    {
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Cart>> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task RemoveRangeAsync(List<Cart> carts)
    {
        _context.Carts.RemoveRange(carts);
        await _context.SaveChangesAsync();
    }

}
