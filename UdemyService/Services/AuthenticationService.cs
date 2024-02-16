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

            var token = await _tokenService.CreateTokenAsync(user);

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

        public async Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _client.SingleOrDefault(x => x.ClientId == clientLoginDto.ClientId && x.ClientSecret == clientLoginDto.ClientSecret);

            if (client is null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", StatusCodes.Status404NotFound, true);
            }

            var token = _tokenService.CreateTokenByClient(client);

            return Response<ClientTokenDto>.Success(token, StatusCodes.Status200OK);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            var refreshTokenFromDb = await _userRefreshTokenRepository.Where(x => x.Token == refreshToken).SingleOrDefaultAsync();

            if (refreshTokenFromDb == null)
            {
                return Response<TokenDto>.Fail("Refresh token not found", StatusCodes.Status404NotFound, true);
            }

            var user = await _userManager.FindByIdAsync(refreshTokenFromDb.UserId);

            if (user == null)
            {
                return Response<TokenDto>.Fail("User id not found", StatusCodes.Status404NotFound, true);
            }

            var tokenDto = await _tokenService.CreateTokenAsync(user);

            refreshTokenFromDb.Token = tokenDto.RefreshToken;
            refreshTokenFromDb.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(tokenDto, StatusCodes.Status200OK);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var refreshTokenFromDb = await _userRefreshTokenRepository.Where(x => x.Token == refreshToken).SingleOrDefaultAsync();

            if (refreshTokenFromDb is null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found", StatusCodes.Status404NotFound, true);
            }

            _userRefreshTokenRepository.Remove(refreshTokenFromDb);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(StatusCodes.Status200OK);
        }
    }
}
