using DataBase.Contract;
using GingerMintSoft.Earth.Location.Solar;
using GingerMintSoft.Earth.Location.Solar.Generator;
using JsonFlatFileDataStore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Database.Tests;

[TestClass]
public sealed class InitDatabaseTests
{
    [TestMethod]
    public async Task TestInitDatabase()
    {
        // Solar power plant location
        var powerPlant = new PowerPlant(
            "",
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
        var collection = store.GetCollection("PowerPlants");

        powerPlant.Name = "Test Power Plant 0";

        var json = JsonConvert.SerializeObject(
            powerPlant, 
            Formatting.Indented, 
            new JsonSerializerSettings
            {
                ContractResolver = new DynamicContractResolver("Radiation", "EarningData", "Calculate")
            });

        await collection.InsertOneAsync(JToken.Parse(json));

        powerPlant.Name = "Test Power Plant 1";

        json = JsonConvert.SerializeObject(
            powerPlant, 
            Formatting.Indented, 
            new JsonSerializerSettings
            {
                ContractResolver = new DynamicContractResolver("Radiation", "EarningData", "Calculate")
            });

        await collection.InsertOneAsync(JToken.Parse(json));

        Assert.AreEqual(collection.Count, 2);
    }

    [TestMethod]
    public void TestReadDatabase()
    {
        var store = new DataStore("data.json");
        var collection = store.GetCollection("PowerPlants");

        Assert.AreEqual(collection.Count, 2);
    }

    //[TestMethod]
    //public async Task TestDeleteDatabase()
    //{
    //    var store = new DataStore("data.json");
    //    var collection = store.GetCollection("PowerPlants");
    //    await collection.DeleteOneAsync(p => p.Id == 1);
       
    //    Assert.AreEqual(collection.Count, 1);

    //    await collection.DeleteOneAsync(p => p.Id == 0);

    //    Assert.AreEqual(collection.Count, 0);
    //}
}