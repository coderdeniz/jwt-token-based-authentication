using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyCore.Configuration;
using UdemyCore.DTOs;
using UdemyCore.Models;
using UdemyCore.Repositories;
using UdemyCore.Services;
using UdemyShared.Dtos;

namespace UdemyService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _client;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenRepository;

        public AuthenticationService(
            IOptions<List<Client>> optionsClient,
            IGenericRepository<UserRefreshToken> userRefreshTokenRepository,
            UserManager<UserApp> userManager,
            IUnitOfWork unitOfWork,
            ITokenService tokenService)
        {
            _userRefreshTokenRepository = userRefreshTokenRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _client = optionsClient.Value;
            _tokenService = tokenService;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", StatusCodes.Status400BadRequest, true);
            }

            bool check = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!check)
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", StatusCodes.Status400BadRequest, true);
            }

            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshTokenRepository.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshTokenRepository.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Token = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Token = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(token, StatusCodes.Status200OK);
        }

        public Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            throw new NotImplementedException();
        }

        public Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
