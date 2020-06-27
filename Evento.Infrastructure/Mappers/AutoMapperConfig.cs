using AutoMapper;
using Evento.Core.Domain;
using Evento.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Evento.Infrastructure.Mappers
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
        => new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Event, EventDto>()
                    .ForMember(x => x.AvaliableTicketCount, m => m.MapFrom(p => p.AvaliableTickets.Count()))
                    .ForMember(x => x.PurchasedTicketsCount, m => m.MapFrom(p => p.PurchasdTickets.Count()))
                    .ForMember(x => x.TicketsCount, m => m.MapFrom(p => p.Tickets.Count()));
            cfg.CreateMap<Event, EventDetailsDto>()
                    .ForMember(x => x.AvaliableTicketCount, m => m.MapFrom(p => p.AvaliableTickets.Count()))
                    .ForMember(x => x.PurchasedTicketsCount, m => m.MapFrom(p => p.PurchasdTickets.Count()))
                    .ForMember(x => x.TicketsCount, m => m.MapFrom(p => p.Tickets.Count()));
            cfg.CreateMap<Ticket, TicketDto>();
            cfg.CreateMap<Ticket, TicketDetailsDto>();
            cfg.CreateMap<User, AccountDto>();
        })
            .CreateMapper();
    }
}
