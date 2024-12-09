using GingerMintSoft.Earth.Location.Solar.Calculation;
using GingerMintSoft.Earth.Location.Solar.Generator;

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
}
