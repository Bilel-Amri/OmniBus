using AutoMapper;
using OmniBus.Domain.Entities;
using OmniBus.Application.DTOs;

namespace OmniBus.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserResponseDto>().ReverseMap();
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, UserProfileDto>().ReverseMap();
            
            CreateMap<Bus, BusDto>().ReverseMap();
            CreateMap<Bus, BusResponseDto>().ReverseMap();
            CreateMap<Bus, CreateBusDto>().ReverseMap();
            CreateMap<Bus, UpdateBusDto>().ReverseMap();
            
            CreateMap<Route, RouteDto>().ReverseMap();
            CreateMap<Route, RouteResponseDto>().ReverseMap();
            CreateMap<Route, CreateRouteDto>().ReverseMap();
            CreateMap<Route, UpdateRouteDto>().ReverseMap();
            CreateMap<RouteStop, RouteStopDto>().ReverseMap();
            CreateMap<RouteStop, CreateRouteStopDto>().ReverseMap();
            
            CreateMap<Schedule, ScheduleDto>().ReverseMap();
            CreateMap<Schedule, ScheduleResponseDto>().ReverseMap();
            CreateMap<Schedule, CreateScheduleDto>().ReverseMap();
            
            CreateMap<Ticket, TicketDto>().ReverseMap();
            CreateMap<Ticket, TicketResponseDto>().ReverseMap();
            CreateMap<Ticket, CreateTicketDto>().ReverseMap();
            
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<Payment, PaymentResponseDto>().ReverseMap();
            CreateMap<Payment, CreatePaymentDto>().ReverseMap();
            
            CreateMap<SeatLock, SeatLockDto>().ReverseMap();
        }
    }
}