﻿using GingerMintSoft.Earth.Solar.Calculation;

namespace GingerMintSoft.Earth.Solar;

public class Location()
{
    public int Altitude { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeSpan TimeZoneOffset { get; set; }

    public Calculate Calculate { get; set; } = new Calculate();

    public Location(int altitude, double latitude, double longitude) : this()
    {
        Altitude = altitude;
        Latitude = latitude;
        Longitude = longitude;

        TimeZoneOffset = DateTimeOffset.Now.Offset;

        Calculate.Location = this;
    }
}
