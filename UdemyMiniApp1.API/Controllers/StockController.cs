using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UdemyMiniApp1.API.Controllers
{
    [Authorize(Roles = "admin",Policy = "AnkaraPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInvoices()
        {
            var userName = User.Identity.Name;
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            // veritabanında userName ya da userID alanları üzerinden gerekli dataları çek

            return Ok($"Invoice İşlemleri => UserName: {userName} -  UserId: {userId}");
        }
    }
}
