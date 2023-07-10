using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRTestProject.API.DTOs;
using SignalRTestProject.API.Models;
using SignalRTestProject.API.Services;

namespace SignalRTestProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _service;

        public MessagesController(MessageService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(PlayerDto playerDto)
        {
            await _service.AddMessage(playerDto);

            return Ok(await _service.GetMessages().ToListAsync() );
        }

        [HttpGet]
        public async Task<IActionResult> InitializeMessages()
        {
           

            return Ok(await _service.GetMessages().ToListAsync());
        }
    }
}
