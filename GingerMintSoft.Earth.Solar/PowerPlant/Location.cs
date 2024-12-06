﻿using GingerMintSoft.Earth.Solar.Calculation;

namespace GingerMintSoft.Earth.Solar.PowerPlant;

public class Location()
{
    public int Altitude { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeSpan TimeZoneOffset { get; set; } = TimeSpan.Zero;

    public Calculate Calculate { get; set; } = new Calculate();
    public List<Roof> Roofs { get; set; } = [];

    public Location(int altitude, double latitude, double longitude) : this()
    {
        Altitude = altitude;
        Latitude = latitude;
        Longitude = longitude;

        TimeZoneOffset = TimeSpan.Zero;

        Calculate.Location = this;
    }
}
