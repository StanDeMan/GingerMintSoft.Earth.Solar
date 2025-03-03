using GingerMintSoft.Earth.Location.Service.Data;
using GingerMintSoft.Earth.Location.Solar.Generator;
using GingerMintSoft.Earth.Location.Solar;
using Microsoft.AspNetCore.Mvc;

namespace GingerMintSoft.Earth.Location.Service.Controllers
{
    [Route("Solar")]
    public class SolarController(ILogger<SolarController> logger) : Controller
    {
        private readonly ILogger<SolarController> _logger = logger;

        [HttpPost]
        [Route("Powerplant/Data")]
        public async Task<ActionResult> Post([FromBody] PowerplantData powerPlantData)
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

                var date = new DateTime(2025, 03, 02, 0, 0, 0, DateTimeKind.Utc);

                var task = Task.Run(() => powerPlant.Calculate.Radiation(date));
                var solarRadiationFromSunRiseTillSunSet = await task;

                foreach (var solarMinutelyRadiation in solarRadiationFromSunRiseTillSunSet)
                {
                    Console.WriteLine($"{solarMinutelyRadiation.Key:HH:mm}\t{solarMinutelyRadiation.Value:F2}");
                }

                return new CreatedResult($"/Powerplant/Data", powerPlantData);
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
