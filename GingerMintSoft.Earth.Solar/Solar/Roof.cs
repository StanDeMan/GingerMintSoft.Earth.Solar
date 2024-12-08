namespace GingerMintSoft.Earth.Location.Solar
{
    public class Roof
    {
        public struct CompassPoint
        {
            public const double North = 0.0;
            public const double NorthNorthEast = 22.5;
            public const double NorthEast = 45.0;
            public const double EastNorthEast= 67.5;
            public const double East = 90.0;
            public const double EastSouthEast = 112.5;
            public const double SouthEast = 135.0;
            public const double SouthSouthEast = 157.5;
            public const double South = 180.0;
            public const double SouthSouthWest = 202.5;
            public const double SouthWest = 225.0;
            public const double WestSouthWest = 247.5;
            public const double West = 270.0;
            public const double WestNorthWest = 292.5;
            public const double NorthWest = 315.0;
            public const double NorthNorthWest = 337.5;
        }

        /// <summary>
        /// Name des Dachs
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Neigungswinkel des Dachs in Grad
        /// </summary>
        public double Tilt { get; init; }

        /// <summary>
        /// PV Generator Ausrichtung in Grad (0° = Nord, 90° = Ost, 180° = Süd, 270° = West)
        /// </summary>
        public double Azimuth { get; set; } = CompassPoint.South;

        /// <summary>
        /// Abweichung zum Azimuth in Grad
        /// </summary>
        public double AzimuthDeviation { get; init; }

        public Dictionary<DateTime, double>? Earning { get; set; }
    }
}
