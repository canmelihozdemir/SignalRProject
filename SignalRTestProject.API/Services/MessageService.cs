using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRTestProject.API.DTOs;
using SignalRTestProject.API.Hubs;
using SignalRTestProject.API.Models;

namespace SignalRTestProject.API.Services
{
    public class MessageService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IMapper _mapper;

        public MessageService(AppDbContext context, IHubContext<MessageHub> hubContext, IMapper mapper)
        {
            _context = context;
            _hubContext = hubContext;
            _mapper = mapper;
        }


        public IQueryable<Player> GetMessages()
        {
            return _context.Players.AsQueryable();
        }

        public async Task AddMessage(PlayerDto playerDto)
        {
            var player=_mapper.Map<Player>(playerDto);
            player.DateTime = DateTime.Now.ToString("dd/MM/yyyy H:mm");
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
        }
    }
}
