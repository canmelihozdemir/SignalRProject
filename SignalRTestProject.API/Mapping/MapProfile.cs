using AutoMapper;
using SignalRTestProject.API.DTOs;
using SignalRTestProject.API.Models;

namespace SignalRTestProject.API.Mapping
{
    public class MapProfile:Profile
    {
        public MapProfile()
        {
            CreateMap<Player, PlayerDto>().ReverseMap();
        }
    }
}
