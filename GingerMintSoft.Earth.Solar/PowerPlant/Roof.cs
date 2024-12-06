namespace GingerMintSoft.Earth.Solar.PowerPlant
{
    public class Roof
    {
        public enum EnmAzimuth
        {
            North,
            East,
            South,
            West
        }

        /// <summary>
        /// Neigungswinkel des Dachs in Grad
        /// </summary>
        public double Tilt { get; init; }

        /// <summary>
        /// Dachausrichtung in Grad (0° = Nord, 90° = Ost, 180° = Süd, 270° = West)
        /// </summary>
        public double Azimuth { get; init; }

        public static double CardinalDirection(EnmAzimuth azimuth)
        {
            return azimuth switch
            {
                EnmAzimuth.North => 0.0,
                EnmAzimuth.East => 90.0,
                EnmAzimuth.South => 180.0,
                EnmAzimuth.West => 270.0,
                _ => 180.0
            };
        }
    }
}
