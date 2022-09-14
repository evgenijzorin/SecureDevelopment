using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;

namespace CardStorageService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Создать конфигурацию маппинга. То есть автоматическое создание объектов из других объектов, сопоставляя их поля.
            CreateMap<Card, CardDto>();
            CreateMap<Client, ClientDto>();
            CreateMap<CreateCardRequest, Card>();
            CreateMap<CreateClientRequest, Client>();
        }
            
    }
}
