using AutoMapper;
using challenge1.Application.Bll.Weather.Interfaces;
using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Common.Util.Common;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.Application.Bll.Weather
{
    public class WeatherAlertBll : IWeatherAlertBll
    {
        private readonly IMapper _mapper;
        private readonly ILogging _logging;
        private readonly IWeatherAlertRepository _alertRepo;
        private readonly IWeatherLocationRepository _locationRepo;

        public WeatherAlertBll(
            IMapper mapper,
            ILogging logging,
            IWeatherAlertRepository alertRepo,
            IWeatherLocationRepository locationRepo)
        {
            _mapper = mapper;
            _logging = logging;
            _alertRepo = alertRepo;
            _locationRepo = locationRepo;
        }

        public async Task<List<GetWeatherAlertsResponseDTO>?> GetAlertsAsync(GetWeatherAlertsFilter request)
        {
            try
            {
                var alerts = await _alertRepo.GetAlertsAsync(request);
                if (alerts == null) return null;

                var dtos = _mapper.Map<List<GetWeatherAlertsResponseDTO>>(alerts);

                foreach (var dto in dtos)
                {
                    var loc = await _locationRepo.GetByIdAsync(dto.LocationId);
                    dto.LocationName = loc?.LocationName ?? "";
                }

                return dtos;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<Guid?> SubscribeAsync(SubscribeWeatherAlertRequestDTO request)
        {
            try
            {
                var location = await _locationRepo.GetByIdAsync(request.LocationId);
                if (location == null) return null;

                var alert = _mapper.Map<WeatherAlert>(request);
                alert.AlertId = Guid.NewGuid();
                alert.IsActive = true;
                alert.CreatedDate = DateTime.Now;

                alert = await _alertRepo.CreateAsync(alert);
                return alert?.AlertId;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<bool> UnsubscribeAsync(UnsubscribeWeatherAlertRequestDTO request)
        {
            try
            {
                var alert = await _alertRepo.GetByIdAsync(request.AlertId);
                if (alert == null) return false;

                alert.IsActive = false;
                alert.IsDeleted = true;
                alert.UpdatedById = request.UpdatedById;
                alert.UpdatedByName = request.UpdatedByName;
                alert.UpdatedDate = DateTime.Now;

                return await _alertRepo.UpdateAsync(alert);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return false;
            }
        }
    }
}
