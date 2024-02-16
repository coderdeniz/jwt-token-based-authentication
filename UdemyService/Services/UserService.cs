using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyCore.DTOs;
using UdemyCore.Models;
using UdemyCore.Services;
using UdemyShared.Dtos;

namespace UdemyService.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp
            {
                Id = Guid.NewGuid().ToString(),
                UserName = createUserDto.UserName,
                Email = createUserDto.Email
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), StatusCodes.Status400BadRequest);
            }

            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), StatusCodes.Status200OK);
        }

        public async Task<Response<NoContent>> CreateUserRoles(string userName)
        {
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                await _roleManager.CreateAsync(new() { Name = "admin" });
                await _roleManager.CreateAsync(new() { Name = "manager" });
            }

            UserApp user = await _userManager.FindByNameAsync(userName);
            List<string> roles = _roleManager.Roles.Select(x => x.Name).ToList();

            await _userManager.AddToRoleAsync(user, "admin");
            await _userManager.AddToRoleAsync(user!, "manager");

            return Response<NoContent>.Success(StatusCodes.Status201Created);
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return Response<UserAppDto>.Fail("UserName not found", StatusCodes.Status404NotFound, true);
            }

            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), StatusCodes.Status200OK);
        }
    }
}
