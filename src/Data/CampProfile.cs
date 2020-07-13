using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(t => t.Venue, s => s.MapFrom(p => p.Location.VenueName))
                .ForMember(t => t.Address1, s => s.MapFrom(p => p.Location.Address1))
                .ForMember(t => t.Address2, s => s.MapFrom(p => p.Location.Address2))
                .ForMember(t => t.Address3, s => s.MapFrom(p => p.Location.Address3))
                .ForMember(t => t.CityTown, s => s.MapFrom(p => p.Location.CityTown))
                .ForMember(t => t.StateProvince, s => s.MapFrom(p => p.Location.StateProvince))
                .ForMember(t => t.PostalCode, s => s.MapFrom(p => p.Location.PostalCode))
                .ForMember(t => t.Country, s => s.MapFrom(p => p.Location.Country));

            CreateMap<CampModel, Location>()
                .ForMember(t => t.VenueName, s => s.MapFrom(p => p.Venue));

            CreateMap<CampModel, Camp>()
                .ForMember(t => t.Location, s => s.MapFrom(p => p));

            CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t => t.Camp, opt => opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());

            CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
        }
    }
}
