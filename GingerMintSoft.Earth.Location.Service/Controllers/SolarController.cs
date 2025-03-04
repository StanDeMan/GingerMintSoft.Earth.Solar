using DataBase;
using GingerMintSoft.Earth.Location.Service.Data;
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

        [HttpPost]
        [Route("Powerplant/")]
        public async Task<ActionResult> PostPowerplant([FromBody] Installations? installations)
        {
            try
            {
                if (installations == null) return BadRequest("No data provided");
                if (installations.Powerplants == null) return BadRequest("No power plants provided");

                var plants = installations.Powerplants;

                foreach (var plant in plants)
                {
                    var powerPlant = new PowerPlant(
                        plant.Name,
                        plant.Altitude,             // Höhe über NN in Metern
                        plant.Latitude,             // Breitengrad in Dezimalgrad
                        plant.Longitude             // Längengrad in Dezimalgrad
                    );
                }

                return new CreatedResult($"/Powerplant", installations);
            }
            catch (Exception e)
            {
                var error = $"Cannot create power plant: {e}";
                _logger.LogError(error);

                return Conflict(error);
            }
        }

        [HttpPost]
        [Route("Powerplant/Roofs")]
        public async Task<ActionResult> PostRoofs([FromBody] Service.Data.Roofs? roofs)
        {
            try
            {
                if (roofs == null) return BadRequest("No data provided");

                return new CreatedResult($"/Powerplant/Roofs", roofs);
            }
            catch (Exception e)
            {
                var error = $"Cannot create power plant roof(s): {e}";
                _logger.LogError(error);

                return Conflict(error);
            }
        }

        [HttpGet]
        [Route("Powerplant/Yield/ForDay")]
        public async Task<ActionResult> GetPowerplantData([FromQuery] DateTime? day)
        {
            try
            {
                _powerPlant = JsonConvert.DeserializeObject<PowerPlant>(JsonConvert.SerializeObject(await _plantStore.ReadByPlantIdAsync("WxHCombjzTaw")));
                _powerPlant.ExecCalculation();

                var utcNow = DateTime.Now.ToUniversalTime();
                
                var dateTime = day.HasValue ? 
                    new DateTime(day.Value.Year, day.Value.Month, day.Value.Day, 0, 0, 0, DateTimeKind.Utc) : 
                    new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
                
                await Task.Run(() => _powerPlant.Calculate.RadiationOnTiltedPanel(dateTime));
                
                //powerPlant.EnergyEarning = powerPlant.MaximumEnergy();
                _powerPlant.PowerEarning = _powerPlant.PowerOverDay();

                return new CreatedResult($"/Powerplant/Data", JsonConvert.SerializeObject(_powerPlant.PowerEarning));
            }
            catch (Exception e)
            {
                var error = $"Cannot calculate power plant data: {e}";
                _logger.LogError(error);

                return Conflict(error);
            }
        }
    }
}
