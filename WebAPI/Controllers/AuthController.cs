using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("GetUsers")]
        public ActionResult GetUsers()
        {
            var user = _userRepository.GetUsers();
            return Ok(user);
        }

        [HttpPost("Login")]
        public ActionResult Login(User user)
        {
            bool userExist = _userRepository.IsExistUserByEmail(user.Email);
            if (!userExist)
            {
                return Ok("User Bulunamadı");
            }
            
            bool passCheck = _userRepository.Login(user.Email, user.Password);
            if (!passCheck)
            {
                return Ok("Kullanıcı adı veya şifre hatalı");
            }



            return Ok();
        }
    }
}
