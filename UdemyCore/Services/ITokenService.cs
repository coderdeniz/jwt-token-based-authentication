using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyCore.Configuration;
using UdemyCore.DTOs;
using UdemyCore.Models;

namespace UdemyCore.Services
{
    public interface ITokenService
    {
        Task<TokenDto> CreateTokenAsync(UserApp userApp);
        ClientTokenDto CreateTokenByClient(Client client);
    }
}
