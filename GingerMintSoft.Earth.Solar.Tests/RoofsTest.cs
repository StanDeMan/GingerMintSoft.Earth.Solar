using GingerMintSoft.Earth.Location.Solar;
using GingerMintSoft.Earth.Location.Solar.Generator;

namespace GingerMintSoft.Earth.Location.Tests;

[TestClass]
public class RoofsTest
{
    private Roofs _roofs = new Roofs();

    [TestInitialize]
    public void Setup()
    {
        _roofs = new Roofs();
        Assert.IsNotNull(_roofs);
        Assert.IsNotNull(_roofs.Read);
    }

    [TestMethod]
    public void AddRoofsTest()
    {
        _roofs.Add(new Roof()
        {
            Name = "Test Roof 1",
            Azimuth = Roof.CompassPoint.East,
            AzimuthDeviation = 10.0,
        });

        _roofs.Add(new Roof()
        {
            Name = "Test Roof 2",
            Azimuth = Roof.CompassPoint.West,
            AzimuthDeviation = 5.0,
        });

        _roofs.Add(new Roof()
        {
            Name = "Test Roof 3",
            Azimuth = Roof.CompassPoint.South
        });

        Assert.AreEqual(3, _roofs.Read!.Count);
    }

    [TestMethod]
    public void AddRoofsGeneratorsTest()
    {
        _roofs.Add(new Roof()
        {
            Name = "Test Roof 1",
            Azimuth = Roof.CompassPoint.East,
            AzimuthDeviation = 10.0,
            Panels = new Panels()
            {
                Read =
                [
                    new Panel()
                    {
                        Name = "Panel 1",
                        Area = 2.0,
                        Efficiency = 0.21
                    }
                ]
            }
        });

        _roofs.Add(new Roof()
        {
            Name = "Test Roof 2",
            Azimuth = Roof.CompassPoint.West,
            AzimuthDeviation = 5.0,
            Panels = new Panels()
            {
                Read =
                [
                    new Panel()
                    {
                        Name = "Panel 2.1",
                        Area = 3.0,
                        Efficiency = 0.21
                    },
                    new Panel()
                    {
                        Name = "Panel 2.2",
                        Area = 3.0,
                        Efficiency = 0.21
                    }
                ],
            }
        });
        _roofs.Add(new Roof()
        {
            Name = "Test Roof 3",
            Azimuth = Roof.CompassPoint.South,
            Panels = new Panels()
            {
                Read =
                [
                    new Panel()
                    {
                        Name = "Panel 3.1",
                        Area = 4.5,
                        Efficiency = 0.22
                    },
                    new Panel()
                    {
                        Name = "Panel 3.2",
                        Area = 5.0,
                        Efficiency = 0.21
                    },
                    new Panel()
                    {
                        Name = "Panel 3.3",
                        Area = 5.5,
                        Efficiency = 0.22
                    }
                ]
            }
        });

        Assert.AreEqual(3, _roofs.Read!.Count);

        foreach (var roof in _roofs.Read!)
        {
            Assert.IsNotNull(roof);
            if(roof.Name != "Test Roof 3") continue;

            var generatorData = 0.0;

            foreach (var panel in roof.Panels.Read)
            {
                Assert.IsNotNull(panel);
                generatorData += panel.Area * panel.Efficiency;
            }

            Assert.AreEqual(3.25, generatorData);
        }
    }
}
