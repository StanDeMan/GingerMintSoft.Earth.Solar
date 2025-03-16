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
    private static IDocumentCollection<dynamic>? _collection;
    private static DataStore? _store;
    private const string DataBaseFile = "data.json";

    private static void CleanUpDataBaseFile(string fileName = DataBaseFile)
    {
        var file = @$"{Environment.CurrentDirectory}\{fileName}";

        if (File.Exists(file)) File.Delete(file);
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        CleanUpDataBaseFile();                      // remove old database file
        _store = new DataStore(DataBaseFile);       // create new database file
        _collection = _store.GetCollection("PowerPlants");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _store!.Dispose();                          // dispose database file
        CleanUpDataBaseFile();                      // remove database file
    }

    [TestMethod]
    public void TestInitDatabase()
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

        powerPlant.Name = "Test Power Plant 0";

        var json = JsonConvert.SerializeObject(
            powerPlant,
            Formatting.Indented,
            new JsonSerializerSettings
            {
                ContractResolver = new DynamicContractResolver("Radiation", "EarningData", "Calculate")
            });

        _collection!.InsertOne(JToken.Parse(json));

        powerPlant.Name = "Test Power Plant 1";

        json = JsonConvert.SerializeObject(
            powerPlant,
            Formatting.Indented,
            new JsonSerializerSettings
            {
                ContractResolver = new DynamicContractResolver("Radiation", "EarningData", "Calculate")
            });

        _collection.InsertOne(JToken.Parse(json));

        Assert.AreEqual(2, _collection.Count);
    }

    [TestMethod]
    public async Task TestDeleteDatabase()
    {
        // Solar power plant - typed collection
        var collection = _store!.GetCollection<PowerPlant>("PowerPlants");
        Thread.Sleep(200);

        await collection!.DeleteOneAsync(p => p.Name == "Test Power Plant 1");

        Assert.AreEqual(1, collection.Count);

        await collection.DeleteOneAsync(p => p.Name == "Test Power Plant 0");

        Assert.AreEqual(0, collection.Count);
    }
}