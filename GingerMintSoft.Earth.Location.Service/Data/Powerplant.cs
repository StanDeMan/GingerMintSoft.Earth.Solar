namespace GingerMintSoft.Earth.Location.Service.Data;

public class Powerplant
{
    public string? Name { get; set; }
    public int Altitude { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public Roofs? Roofs { get; set; }
}

public class Roofs
{
    public Roof[]? Roof { get; set; }
}

public class Roof
{
    public string? Name { get; set; }
    public float Tilt { get; set; }
    public float Azimuth { get; set; }
    public float AzimuthDeviation { get; set; }
    public Panels? Panels { get; set; }
}

public class Panels
{
    public int Count { get; set; }
    public Panel[]? Panel { get; set; }
}

public class Panel
{
    public string? Name { get; set; }
    public float Area { get; set; }
    public float Efficiency { get; set; }
}
