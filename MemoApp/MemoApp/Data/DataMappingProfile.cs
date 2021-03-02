using AutoMapper;
using MemoApp.Models;
using MemoApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace MemoApp.Data
{
    public class DataMappingProfile : Profile
    {
        public DataMappingProfile()
        {
            CreateMap<Memo, MemoViewModel>().ReverseMap();
            CreateMap<Status, StatusViewModel>().ReverseMap();
            CreateMap<Tag, TagViewModel>().ReverseMap();
            CreateMap<IdentityRole, RoleViewModel>();
            CreateMap<PersonSettingsModel,Setting>();
        }
    }
}
