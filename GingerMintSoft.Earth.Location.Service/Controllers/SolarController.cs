using GingerMintSoft.Earth.Location.Service.Data;
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

        [HttpPost]
        [Route("Powerplant/Data/ForDate/{date}")]
        public async Task<ActionResult> Post([FromQuery] DateOnly? date, [FromBody] PowerplantData powerPlantData)
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

                var utcNow = DateTime.UtcNow;

                // Converting DateOnly to DateTime by providing Time Info
                var dateTime = date?.ToDateTime(TimeOnly.Parse("00:00 AM")) 
                               ?? new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
                
                await Task.Run(() => powerPlant.Calculate.RadiationOnTiltedPanel(dateTime.ToUniversalTime()));
                
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
