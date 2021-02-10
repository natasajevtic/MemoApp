using AutoMapper;
using MemoApp.ViewModels;

namespace MemoApp.Data
{
    public class DataMappingProfile : Profile
    {
        public DataMappingProfile()
        {
            CreateMap<Memo, MemoViewModel>().ReverseMap();
            CreateMap<Status, StatusViewModel>().ReverseMap();
            CreateMap<Tag, TagViewModel>().ReverseMap();
        }
    }
}
