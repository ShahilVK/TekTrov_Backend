using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(int userId, int productId);

        Task<List<Cart>> GetCartAsync(int userId);

        Task UpdateCartAsync(int userId, int productId, int quantity);
    }
}
