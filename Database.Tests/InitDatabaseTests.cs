using GingerMintSoft.Earth.Location.Solar;
using GingerMintSoft.Earth.Location.Solar.Generator;
using JsonFlatFileDataStore;

namespace Database.Tests;

[TestClass]
public sealed class InitDatabaseTests
{
    [TestMethod]
    public async Task TestInitDatabase()
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

        // east orientation roof generator configuration
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
        var store = new DataStore("data.json");
        var collection = store.GetCollection<PowerPlant>();

        powerPlant.Calculate = null;
        await collection.InsertOneAsync(powerPlant);
    }
}