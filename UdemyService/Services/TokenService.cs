using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UdemyCore.Configuration;
using UdemyCore.DTOs;
using UdemyCore.Models;
using UdemyCore.Services;
using UdemyShared.Configuration;

namespace UdemyService.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOptions _customTokenOptions;

        public TokenService(IOptions<CustomTokenOptions> customTokenOptions, UserManager<UserApp> userManager)
        {
            _customTokenOptions = customTokenOptions.Value;
            _userManager = userManager;
        }

        private static string CreateRefreshToken()
        {
            var numberByte = new Byte[32];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(numberByte);
            }

            return Convert.ToBase64String(numberByte);
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            throw new NotImplementedException();
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
