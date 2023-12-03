using AutoMapper;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Extensions;
using Donace_BE_Project.Models.Calendar;
using Donace_BE_Project.Models.CalendarParticipation;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Models.EventParticipation;
using Donace_BE_Project.Models.User;
using Donace_BE_Project.Models.VNPay;

namespace Application.ProfileAutoMapper
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            #region User
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<User, UpdateUserModel>().ReverseMap();
            CreateMap<User, GetListUserInCalendarModel>().ReverseMap();
            #endregion

            #region Event
            CreateMap<EventCreateInput, Event>();
            CreateMap<SectionCreateInput, Section>();

            CreateMap<Event, EventFullOutput>();
            CreateMap<Event, EventOutput>();

            CreateMap<EventUpdateInput, Event>()
                .Ignore(des => des.Sections);

            CreateMap<Event, EventDetailModel>()
                .Ignore(x => x.IsAppro)
                .Ignore(x => x.IsSub)
                .ReverseMap();
            #endregion

            #region Section
            CreateMap<Section, SectionOutput>();
            #endregion

            #region Calendar
            CreateMap<Calendar, CalendarModel>().ReverseMap();
            CreateMap<Calendar, CalendarUpdateModel>().ReverseMap();
            CreateMap<Calendar, GetListCalendarModel>()
                .ReverseMap();

            CreateMap<Calendar, CalendarResponseModel>().ReverseMap();
            #endregion

            #region CalendarParticipation
            CreateMap<CalendarParticipation, CalendarParticipationModel>().ReverseMap();
            CreateMap<CalendarParticipation, CalendarParticipationGetByCalendarIdModel>().ReverseMap();
            #endregion

            #region EventParticipation
            CreateMap<EventParticipation, EventParticipationModel>().ReverseMap();
            #endregion

            #region Payment
            CreateMap<ConnectPayment, ConnectVnPayModel>()
                .ForMember(a => a.TmnCode, c => c.MapFrom(x => x.Key))
                .ForMember(a => a.HashSecret, c => c.MapFrom(x => x.SecretKey))
                .ReverseMap();
            #endregion
        }
    }
}
