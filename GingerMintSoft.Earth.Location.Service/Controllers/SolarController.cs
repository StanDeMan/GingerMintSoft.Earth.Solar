using GingerMintSoft.Earth.Location.Solar;
using GingerMintSoft.Earth.Location.Solar.Generator;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GingerMintSoft.Earth.Location.Service.Controllers
{
    [Route("Solar")]
    public class SolarController(ILogger<SolarController> logger) : Controller
    {
        private readonly ILogger<SolarController> _logger = logger;

        //[HttpPost]
        //[Route("Powerplant/Data/ForDay/{day}")]
        //public async Task<ActionResult> Post([FromBody] PowerplantData powerPlantData)
        //{
        //}

        [HttpGet]
        [Route("Powerplant/Yield/ForDay")]
        public async Task<ActionResult> GetPowerplantData([FromQuery] DateTime? day)
        {
            try
            {
                // Solar power plant location
                var powerPlant = new PowerPlant(
                    "Eichstädt PV Anlage",
                    232,                    // Höhe über NN in Metern
                    48.1051268096319,       // Breitengrad in Dezimalgrad
                    7.9085366169705145      // Längengrad in Dezimalgrad
                );

                //var json = JsonConvert.SerializeObject(powerPlant, Formatting.Indented, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

                // multiple roof locations with different modules possible
                // east orientation roof generator configuration
                powerPlant.Roofs.Add(new Roof()
                {
                    Name = "Ostdach",
                    Azimuth = Roof.CompassPoint.East,
                    AzimuthDeviation = 15.0,
                    Tilt = 43.0,
                    Panels = new Panels()
                    {
                        Panel =
                        [
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            },
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            },
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            },
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            }
                        ],
                    }
                });

                // west orientation roof generator configuration
                powerPlant.Roofs.Add(new Roof()
                {
                    Name = "Westdach",
                    Azimuth = Roof.CompassPoint.West,
                    AzimuthDeviation = 15.0,
                    Tilt = 43.0,
                    Panels = new Panels()
                    {
                        Panel =
                        [
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            },
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            },
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            },
                            new Panel()
                            {
                                Name = "Ying Ping 420Wp",
                                Area = 1.78,
                                Efficiency = 0.21
                            }
                        ],
                    }
                });

                var utcNow = DateTime.Now.ToUniversalTime();
                
                var dateTime = day.HasValue ? 
                    new DateTime(day.Value.Year, day.Value.Month, day.Value.Day, 0, 0, 0, DateTimeKind.Utc) : 
                    new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
                
                await Task.Run(() => powerPlant.Calculate.RadiationOnTiltedPanel(dateTime));
                
                //var plantData = await task;

                //powerPlant.EnergyEarning = powerPlant.MaximumEnergy();
                powerPlant.PowerEarning = powerPlant.MaximumPower();

                return new CreatedResult($"/Powerplant/Data", JsonConvert.SerializeObject(powerPlant.PowerEarning));
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
