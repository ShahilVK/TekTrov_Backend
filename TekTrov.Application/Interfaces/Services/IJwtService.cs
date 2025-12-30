using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(int userId, string email, string role);
        string GenerateRefreshToken();
    }
}
