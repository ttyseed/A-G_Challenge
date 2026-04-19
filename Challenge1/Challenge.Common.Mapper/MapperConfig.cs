using AutoMapper;
using challenge1.Application.DTO.Common;
using challenge1.Application.DTO.Weather;
using challenge1.Application.ViewModel.Common.Intranet;
using challenge1.Database.Models;
using challenge1.Database.Models.Weather;

namespace challenge1.Common.Mapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<AddAuditRequestDTO, Audit>();
            CreateMap<BackendUser, GetBackendUserByLoginIdResponseDTO>();
            CreateMap<BackendUser, GetBackendUsersResponseDTO>();
            CreateMap<CreateBackendUserRequestDTO, BackendUser>();
            CreateMap<LoginUser, GetLoginUserByLoginIdResponseDTO>();
            CreateMap<LoginUser, GetLoginUsersResponseDTO>();
            CreateMap<Role, GetRolesResponseDTO>();
            CreateMap<Role, GetRoleByIdResponseDTO>();
            CreateMap<CreateRoleRequestDTO, Role>();
            CreateMap<GetBackendUsersRolesResponseDTO, GetBackendUsersRolesVM>();

            // Weather
            CreateMap<WeatherLocation, GetWeatherLocationsResponseDTO>();
            CreateMap<CreateWeatherLocationRequestDTO, WeatherLocation>();
            CreateMap<WeatherRecord, GetCurrentWeatherResponseDTO>()
                .ForMember(d => d.LocationName, opt => opt.Ignore());
            CreateMap<WeatherRecord, GetHistoricalWeatherResponseDTO>()
                .ForMember(d => d.LocationName, opt => opt.Ignore());
            CreateMap<WeatherForecast, GetWeatherForecastResponseDTO>()
                .ForMember(d => d.LocationName, opt => opt.Ignore());
            CreateMap<WeatherAlert, GetWeatherAlertsResponseDTO>()
                .ForMember(d => d.LocationName, opt => opt.Ignore());
            CreateMap<SubscribeWeatherAlertRequestDTO, WeatherAlert>();
        }
    }
}
