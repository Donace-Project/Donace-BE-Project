using AutoMapper;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Extensions;
using Donace_BE_Project.Models.Calendar;
using Donace_BE_Project.Models.CalendarParticipation;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Models.User;

namespace Application.ProfileAutoMapper
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            #region User
            CreateMap<User, UserModel>();
            #endregion

            #region Event
            CreateMap<EventCreateInput, Event>();
            CreateMap<SectionCreateInput, Section>();

            CreateMap<Event, EventFullOutput>();
            CreateMap<Event, EventOutput>();

            CreateMap<EventUpdateInput, Event>()
                .Ignore(des => des.Sections);
            #endregion

            #region Section
            CreateMap<Section, SectionOutput>();
            #endregion

            #region Calendar
            CreateMap<Calendar, CalendarModel>().ReverseMap();
            CreateMap<Calendar, CalendarUpdateModel>().ReverseMap();
            #endregion

            #region CalendarParticipation
            CreateMap<CalendarParticipation, CalendarParticipationModel>().ReverseMap();
            CreateMap<CalendarParticipation, CalendarParticipationGetByCalendarIdModel>().ReverseMap();
            #endregion
        }
    }
}
