using DataBase;
using GingerMintSoft.Earth.Location.Solar;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GingerMintSoft.Earth.Location.Service.Controllers
{
    [Route("Solar")]
    public class SolarController(
        IFileStore plantStore, 
        ILogger<SolarController> logger) : Controller
    {
        private PowerPlant? _powerPlant;

        /// <summary>
        /// Request power plant data by plantId
        /// </summary>
        /// <param name="plantId">For this plant</param>
        /// <returns>Power Plant data</returns>
        [HttpGet]
        [Route("Powerplants/{plantId}")]
        public async Task<ActionResult> GetPowerplant(string plantId)
        {
            try
            {
                return new CreatedResult($"/Powerplants/{plantId}", 
                    await plantStore.ReadByPlantIdAsync(plantId));
            }
            catch (Exception e)
            {
                var error = $"Cannot read power plant: {e}";
                logger.LogError(error);

                return Conflict(error);
            }
        }

        /// <summary>
        /// Request power plant energy for a day
        /// </summary>
        /// <param name="plantId">For this plant</param>
        /// <param name="day">For this day</param>
        /// <returns>Energy data per minute from sun rise till sun set</returns>
        [HttpGet]
        [Route("Powerplants/{plantId}/Energy/ForDay")]
        public async Task<ActionResult> GetEnergyForDay(string plantId, [FromQuery] DateTime? day)
        {
            try
            {
                _powerPlant = await InitializePowerPlant(plantId);

                await Task.Run(() => _powerPlant.Calculate.RadiationOnTiltedPanel(ProcessDate(day)));
                _powerPlant.EnergyEarning = _powerPlant.EnergyOverDay();

                return new CreatedResult($"Powerplants/{plantId}/Energy/ForDay?day={day}", 
                    JsonConvert.SerializeObject(_powerPlant.EnergyEarning));
            }
            catch (Exception e)
            {
                var error = $"Cannot calculate energy for a day: {e}";
                logger.LogError(error);

                return Conflict(error);
            }
        }

        /// <summary>
        /// Request power plant power data for a day
        /// </summary>
        /// <param name="plantId">For this plant</param>
        /// <param name="day">For this day</param>
        /// <returns>Power data per minute from sun rise till sun set</returns>
        [HttpGet]
        [Route("Powerplants/{plantId}/Power/ForDay")]
        public async Task<ActionResult> GetPowerForDay(string plantId, [FromQuery] DateTime? day)
        {
            try
            {
                _powerPlant = await InitializePowerPlant(plantId);

                var utcNow = DateTime.Now.ToUniversalTime();
                
                var dateTime = day.HasValue ? 
                    new DateTime(day.Value.Year, day.Value.Month, day.Value.Day, 0, 0, 0, DateTimeKind.Utc) : 
                    new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
                
                await Task.Run(() => _powerPlant.Calculate.RadiationOnTiltedPanel(dateTime));
                _powerPlant.PowerEarning = _powerPlant.PowerOverDay();

                return new CreatedResult($"/Powerplants/{plantId}/Power/ForDay?day={day}", 
                    JsonConvert.SerializeObject(_powerPlant.PowerEarning));
            }
            catch (Exception e)
            {
                var error = $"Cannot calculate power for a day: {e}";
                logger.LogError(error);

                return Conflict(error);
            }
        }

        /// <summary>
        /// Initialize power plant
        /// </summary>
        /// <param name="plantId">For this plant</param>
        /// <returns>Initialized power plant</returns>
        private async Task<PowerPlant> InitializePowerPlant(string plantId)
        {
            var powerPlant = JsonConvert
                .DeserializeObject<PowerPlant>(JsonConvert
                    .SerializeObject(await plantStore.ReadByPlantIdAsync(plantId)));
            
            powerPlant.Initialize();

            return powerPlant;
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
