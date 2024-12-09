using GingerMintSoft.Earth.Location.Solar.Generator;

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
        public string Name { get; set; } = string.Empty;

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

        /// <summary>
        /// Solarmodule
        /// </summary>
        public Panels Panels { get; set; } = new Panels();

        /// <summary>
        /// Ertrag in Watt pro m² abhängig vom Wirkungsgrad und der Fläche des Solarmoduls
        /// </summary>
        public Dictionary<DateTime, double>? Radiation { get; set; }

        /// <summary>
        /// Fläche der Solarmodule multipliziert mit dem Wirkungsgrad
        /// </summary>
        /// <returns></returns>
        public double GeneratorData()
        {
            if(!Panels.Panel.Any()) throw new ArgumentOutOfRangeException (nameof(Panels));

            return Panels.Panel.Sum(panel => panel.Area * panel.Efficiency);
        }

        /// <summary>
        /// Errechnete Energiedaten für einen Solar-Generator
        /// </summary>
        /// <returns></returns>
        public Dictionary<DateTime, double> Earning()
        {
            var generatorData = GeneratorData();

            return Radiation!.ToDictionary(
                dataPoint => dataPoint.Key,
                dataPoint => dataPoint.Value * generatorData);

        }
    }
}
