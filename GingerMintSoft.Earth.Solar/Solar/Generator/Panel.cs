namespace GingerMintSoft.Earth.Location.Solar.Generator
{
    public class Panel
    {
        /// <summary>
        /// Name des Solarmodul: z.B. Hersteller und Typ
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Fläche des Solarmoduls in m²
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Wirkungsgrad des Solarmoduls in Prozent
        /// </summary>
        public double Efficiency { get; set; }
    }
}
