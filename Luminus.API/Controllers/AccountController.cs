using Luminus.API.DAL;
using Luminus.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Luminus.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly ClientDbContext _context;

        public AccountController(ClientDbContext context)
        {
            _context = context;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Authorize(User user)
        {
            if(user != null && user.Password != null & user.Name != null)
            {
                var result = await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Name == user.Name && f.Password == user.Password);
                if (result == null) return NotFound();
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (user != null && user.Password != null & user.Name != null)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                var result = await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Name == user.Name && f.Password == user.Password);
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
