using Luminus.API.DAL;
using Luminus.API.Services;
using Luminus.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Luminus.API.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ClientDbContext _context;
        private readonly ClientService _clientService;

        public ChatController(ClientDbContext context, ClientService clientService)
        {
            _context = context;
            _clientService = clientService;
        }
        [HttpPost("save-message")]
        public async Task<IActionResult> SaveMessage(Message message)
        {
            if (message != null && message.User != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(f => f.Id == message.User.Id);
                message.User = user;
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("get-messages")]
        public async Task<IActionResult> GetMessages(User user)
        {
            var result = await _context.Users.FirstOrDefaultAsync(f => f.Name  == user.Name && f.Password == user.Password);
            if (result != null)
            {
                var messages = await _context.Messages.Include(m => m.File).Include(m => m.User).ToListAsync();
                messages.ForEach(f => f.User.Password = "");
                return Ok(messages);
            }
            return Unauthorized();
        }
        [Route("echo")]
        public async Task Echo()
        {
            var httpWebSocket = HttpContext.WebSockets;
            if (httpWebSocket.IsWebSocketRequest)
            {
                using var webSocket = await httpWebSocket.AcceptWebSocketAsync();
                await _clientService.Echo(webSocket);
                await webSocket
                    .CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Close", CancellationToken.None);
            }
        }
    }
}
