using Luminus.API.DAL;
using Luminus.API.Services;
using Luminus.Domain;
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
        private readonly ClientService _clientService;

        public AccountController(ClientDbContext context, ClientService clientService)
        {
            _context = context;
            _clientService = clientService;
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

        [HttpGet("get-active-users")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var result = await _context.Users.Select(s => new UserInfo
            {
                Name = s.Name,
                Id = s.Id
            }).ToListAsync();
            if (result == null) return BadRequest();
            return Ok(result);
        }
    }
}
