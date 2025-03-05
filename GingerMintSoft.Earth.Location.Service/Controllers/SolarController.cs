using DataBase;
using GingerMintSoft.Earth.Location.Solar;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GingerMintSoft.Earth.Location.Service.Controllers
{
    [Route("Solar")]
    public class SolarController(IFileStore plantStore, ILogger<SolarController> logger) : Controller
    {
        private readonly IFileStore _plantStore = plantStore;
        private readonly ILogger<SolarController> _logger = logger;

        private PowerPlant? _powerPlant;

        [HttpGet]
        [Route("Powerplants/{plantId}")]
        public async Task<ActionResult> PostPowerplant(string plantId)
        {
            try
            {
                return new CreatedResult(@$"/Powerplants/{plantId}", 
                    await _plantStore.ReadByPlantIdAsync(plantId));
            }
            catch (Exception e)
            {
                var error = $"Cannot read power plant: {e}";
                _logger.LogError(error);

                return Conflict(error);
            }
        }

        [HttpGet]
        [Route("Powerplants/{plantId}/Energy/ForDay")]
        public async Task<ActionResult> GetEnergyForDay(string plantId, [FromQuery] DateTime? day)
        {
            try
            {
                _powerPlant = JsonConvert
                    .DeserializeObject<PowerPlant>(JsonConvert
                        .SerializeObject(await _plantStore.ReadByPlantIdAsync(plantId)));

                _powerPlant.ExecCalculation();

                await Task.Run(() => _powerPlant.Calculate.RadiationOnTiltedPanel(ProcessDate(day)));
                _powerPlant.EnergyEarning = _powerPlant.EnergyOverDay();

                return new CreatedResult($@"Powerplants/{plantId}/Energy/ForDay?day={day}", 
                    JsonConvert.SerializeObject(_powerPlant.EnergyEarning));
            }
            catch (Exception e)
            {
                var error = $"Cannot calculate energy for a day: {e}";
                _logger.LogError(error);

                return Conflict(error);
            }
        }

        [HttpGet]
        [Route("Powerplants/{plantId}/Power/ForDay")]
        public async Task<ActionResult> GetPowerForDay(string plantId, [FromQuery] DateTime? day)
        {
            try
            {
                _powerPlant = JsonConvert
                    .DeserializeObject<PowerPlant>(JsonConvert
                        .SerializeObject(await _plantStore.ReadByPlantIdAsync(plantId)));

                _powerPlant.ExecCalculation();

                var utcNow = DateTime.Now.ToUniversalTime();
                
                var dateTime = day.HasValue ? 
                    new DateTime(day.Value.Year, day.Value.Month, day.Value.Day, 0, 0, 0, DateTimeKind.Utc) : 
                    new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
                
                await Task.Run(() => _powerPlant.Calculate.RadiationOnTiltedPanel(dateTime));
                _powerPlant.PowerEarning = _powerPlant.PowerOverDay();

                return new CreatedResult(@$"/Powerplants/{plantId}/Power/ForDay?day={day}", 
                    JsonConvert.SerializeObject(_powerPlant.PowerEarning));
            }
            catch (Exception e)
            {
                var error = $"Cannot calculate power for a day: {e}";
                _logger.LogError(error);

                return Conflict(error);
            }
        }

        /// <summary>
        /// Process day date - use UTC midnight
        /// </summary>
        /// <param name="day">for this day</param>
        /// <returns>day at utc midnight</returns>
        private DateTime ProcessDate(DateTime? day)
        {
            var utcNow = DateTime.Now.ToUniversalTime();
                
            return day.HasValue ? 
                new DateTime(day.Value.Year, day.Value.Month, day.Value.Day, 0, 0, 0, DateTimeKind.Utc) : 
                new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
