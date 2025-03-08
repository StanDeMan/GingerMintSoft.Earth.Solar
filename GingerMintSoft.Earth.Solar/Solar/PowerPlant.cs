using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using GingerMintSoft.Earth.Location.Solar.Calculation;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace GingerMintSoft.Earth.Location.Solar;

public class PowerPlant()
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct Power
    {
        public const double W = 1.0;
        public const double kW = W / 1000;
        public const double MW = kW / 1000;
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public struct Energy
    {
        public const double Wh = 60.0 / 1000;
        // ReSharper disable once InconsistentNaming
        public const double kWh = Wh / 1000;
        public const double MWh = kWh / 1000;
    }

    public string? Name { get; set; } = string.Empty;
    public int Altitude { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeSpan TimeZoneOffset { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Init Location on Greenwich Meridian
    /// Altitude above sea level in meters
    /// </summary>
    [JsonIgnore]
    public Calculate? Calculate { get; set; } = new Calculate();
    public List<Roof> Roofs { get; set; } = [];

    [JsonIgnore]
    public Dictionary<DateTime,double>? PowerEarning { get; set; }

    [JsonIgnore]
    public Dictionary<DateTime, double>? EnergyEarning { get; set; }

    public PowerPlant Initialize()
    {
        Calculate!.InitLocation(this);

        return this;
    }

    public PowerPlant(string? name, int altitude, double latitude, double longitude) : this()
    {
        Name = name;
        Altitude = altitude;
        Latitude = latitude;
        Longitude = longitude;

        TimeZoneOffset = TimeSpan.Zero;
        Calculate.InitLocation(this);
    }

    /// <summary>
    /// Berechnet den Gesamtertrag in Watt über einen Tag
    /// der gesamten Analage (alle Generatoren)
    /// </summary>
    /// <returns></returns>
    public Dictionary<DateTime, double> PowerOverDay(double factor = Power.W)
    {
        Dictionary<DateTime, double> maxTotalPower = [];

        foreach (var power in Roofs.SelectMany(roof => roof.Earning()))
        {
            if (maxTotalPower.ContainsKey(power.Key))
            {
                maxTotalPower[power.Key] += power.Value * factor;
            }
            else
            {
                maxTotalPower.Add(power.Key, power.Value * factor);
            }
        }
        
        return maxTotalPower;
    }

    /// <summary>
    /// Berechnet die Energy in kWh über einen Tag
    /// der gesamten Analage (alle Generatoren)
    /// </summary>
    /// <returns></returns>
    public Dictionary<DateTime, double> EnergyOverDay(double factor = Energy.kWh)
    {
        var totalEnergy = 0.0;
        var maxPower = PowerOverDay();
        var maxTotalEnergy = new Dictionary<DateTime, double>();

        foreach (var power in maxPower)
        {
            totalEnergy += power.Value * factor;
            maxTotalEnergy.Add(power.Key, totalEnergy);
        }

        return maxTotalEnergy;
    }
}
