using AutoMapper;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using System.Data;

namespace ChattingAppAPI.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MemberDto, AppUser>().ReverseMap()
             .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                    src.Photos.FirstOrDefault(x => x.IsMain)!.Url))
                .ForMember(dest => dest.Age, opt =>
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>().ReverseMap();
        CreateMap<AppUser, MemberUpdateDto>().ReverseMap();
        CreateMap<AppUser, RegisterDto>().ReverseMap();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderPhotoUrl,
            o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(p => p.IsMain)!.Url))
              .ForMember(d => d.RecipientPhotoUrl,
            o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(p => p.IsMain)!.Url)).ReverseMap();
        CreateMap<DateTime, DateTime>()
            .ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ?
        DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
