using GingerMintSoft.Earth.Location.Solar.Calculation;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GingerMintSoft.Earth.Location.Solar;

public class PowerPlant()
{
    public string Name { get; set; } = string.Empty;
    public int Altitude { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeSpan TimeZoneOffset { get; set; } = TimeSpan.Zero;

    public Calculate Calculate { get; set; } = new Calculate();
    public List<Roof> Roofs { get; set; } = [];

    public PowerPlant(string name, int altitude, double latitude, double longitude) : this()
    {
        Name = name;
        Altitude = altitude;
        Latitude = latitude;
        Longitude = longitude;

        TimeZoneOffset = TimeSpan.Zero;

        Calculate.Location = this;
    }

    /// <summary>
    /// Berechnet den maximalen Gesamtertrag in Watt
    /// </summary>
    /// <returns></returns>
    public Dictionary<DateTime, double> MaximumTotalPower()
    {
        Dictionary<DateTime, double> maxTotalPower = [];

        foreach (var roof in Roofs)
        {
            foreach (var power in roof.EarningData!)
            {
                if (maxTotalPower.ContainsKey(power.Key))
                {
                    maxTotalPower[power.Key] += power.Value;
                }
                else
                {
                    maxTotalPower.Add(power.Key, power.Value);
                }
            }
        }
        
        return maxTotalPower;
    }

    /// <summary>
    /// Berechnet den maximale Energy in kWh
    /// </summary>
    /// <returns></returns>
    public Dictionary<DateTime, double> MaximumTotalEnergy()
    {
        var totalEnergy = 0.0;
        var maxPower = MaximumTotalPower();
        var maxTotalEnergy = new Dictionary<DateTime, double>();

        foreach (var power in maxPower)
        {
            totalEnergy += power.Value * 0.06 / 1000;
            maxTotalEnergy.Add(power.Key, totalEnergy);
        }

        return maxTotalEnergy;
    }
}
