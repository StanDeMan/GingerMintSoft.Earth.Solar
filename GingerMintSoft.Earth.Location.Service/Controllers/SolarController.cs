using GingerMintSoft.Earth.Location.Service.Data;
using GingerMintSoft.Earth.Location.Solar;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Panel = GingerMintSoft.Earth.Location.Solar.Generator.Panel;
using Panels = GingerMintSoft.Earth.Location.Solar.Generator.Panels;
using Roof = GingerMintSoft.Earth.Location.Solar.Roof;

namespace GingerMintSoft.Earth.Location.Service.Controllers
{
    [Route("Solar")]
    public class SolarController(ILogger<SolarController> logger) : Controller
    {
        private readonly ILogger<SolarController> _logger = logger;

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

                //var powerPlant!.Roofs = roofs.Roof!.Select(roof => new Roof()
                //{
                //    Name = roof.Name,
                //    Azimuth = roof.Azimuth,
                //    AzimuthDeviation = roof.AzimuthDeviation,
                //    Tilt = roof.Tilt,
                //    Panels = new Panels()
                //    {
                //        Panel =
                //        [
                //            ..from panel in roof.Panels!.Panel
                //            select new Panel()
                //            {
                //                Name = panel.Name,
                //                Area = panel.Area,
                //                Efficiency = panel.Efficiency
                //            }
                //        ]
                //    }
                //}).ToList();

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
