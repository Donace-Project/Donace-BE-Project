using AutoMapper;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Models.User;

namespace Application.ProfileAutoMapper
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<User, UserModel>();
        }
    }
}
