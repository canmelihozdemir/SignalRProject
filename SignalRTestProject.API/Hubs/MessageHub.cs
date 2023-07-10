using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRTestProject.API.DTOs;
using SignalRTestProject.API.Models;
using SignalRTestProject.API.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SignalRTestProject.API.Hubs
{
    public class MessageHub:Hub
    {
        private readonly MessageService _service;
        private readonly AppDbContext _context;
        private static List<RoomInfo> _roomInfos { get; set; } = new List<RoomInfo>();

        public MessageHub(MessageService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task GetAllRoomsAsync()
        {
            var rooms = await _context.Rooms.Select(x => new
            {
                roomName = x.Name
            }).ToListAsync();
            await Clients.All.SendAsync("GetAllRooms", rooms);
        }

        public async Task GetAllGroupMessagesAsync(string roomName)
        {
            var team = await _context.Rooms.Where(x => x.Name == roomName).FirstOrDefaultAsync();
            if (team == null) return;


            var players = await _context.Players.Where(x => x.TeamId == team.Id).Select(x => new
            {
                teamId = x.TeamId,
                name = x.Name,
                messageContent = x.MessageContent,
                dateTime = x.DateTime
            }).ToListAsync();
            await Clients.Group(roomName).SendAsync("GetAllGroupMessages", players);
        }
        
        public async Task CreateRoomAsync(string teamName)
        {
            await _context.Rooms.AddAsync(new Room() { Name=teamName,OwnerConnectionId=Context.ConnectionId});
            await _context.SaveChangesAsync();
            await GetAllRoomsAsync();
            _roomInfos.Add(new RoomInfo() {Name=teamName,OwnerConnectionId=Context.ConnectionId,MemberCount=1 });
        }

        public async Task DeleteRoomAsync(string name)
        {
            var room = await _context.Rooms.Where(x => x.Name == name).FirstOrDefaultAsync();

            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                await GetAllRoomsAsync();
            }
        }
        public async Task AddToGroupAsync(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
            _roomInfos.ForEach((room) =>
            {
                if (room.Name==teamName)
                {
                    room.MemberCount++;
                    room.MemberConnectionId.Add(Context.ConnectionId);
                }
            });
        }

        public async Task RemoveToGroupAsync(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
            _roomInfos.ForEach(async (room) =>
            {
                if (room.Name == teamName)
                {
                    room.MemberCount--;
                    if (room.MemberCount <= 0)
                    {
                        _roomInfos.Remove(room);
                        await DeleteRoomAsync(room.Name);
                        room.MemberConnectionId.Remove(Context.ConnectionId);
                    }
                }
            });
        }

        public async Task SendMessageByGroupAsync(string playerName, string playerMessage, string roomName)
        {
            var team = await _context.Rooms.Where(x => x.Name == roomName).FirstOrDefaultAsync();
            string currentDateTime= DateTime.Now.ToString("dd/MM/yyyy H:mm");
            if (team != null)
            {
                await _service.AddMessage(new PlayerDto() { Name = playerName, MessageContent= playerMessage, TeamId=team.Id });
                currentDateTime= DateTime.Now.ToString("dd/MM/yyyy H:mm");
            }
            else
            {
                var newTeam = new Room { Name = roomName };
                newTeam.Players.Add(new Player { Name = playerName });
                _context.Rooms.Add(newTeam);
            }
            await _context.SaveChangesAsync();
            await Clients.Group(roomName).SendAsync("GetNewMessageByGroup", playerName, playerMessage, currentDateTime);
        }
        
        private async Task CheckIfClientHasRoomAsync(string connectionId)
        {
            var room = await _context.Rooms.Where(x => x.OwnerConnectionId == connectionId).FirstOrDefaultAsync();

            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                await GetAllRoomsAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            _roomInfos.ForEach(async (room) =>
            {
                room.MemberConnectionId.ForEach(async (memberConnectionString) =>
                {
                    if (memberConnectionString==Context.ConnectionId)
                    {
                        room.MemberCount--;
                        room.MemberConnectionId.Remove(memberConnectionString);
                        if (room.MemberCount <= 0)
                        {
                            _roomInfos.Remove(room);
                            await DeleteRoomAsync(room.Name);
                        }
                            
                    }
                });
            
            });
        }

        

    }
}
