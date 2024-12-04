namespace GingerMintSoft.Earth.Solar.Cmd
{
    internal class Program
    {
        static void Main()
        {
            var location = new Location(
                232,                    // Höhe über NN in Metern
                48.1051268096319,       // Breitengrad in Dezimalgrad
                7.9085366169705145      // Längengrad in Dezimalgrad
            );

            var date = new DateTime(2024, 12, 21, 0, 0, 0, DateTimeKind.Utc);

            // Solarstrahlung berechnen
            Console.WriteLine($"Stündliche Solarstrahlung für {date.ToShortDateString()}");
            Console.WriteLine("Stunde:Minute\tStrahlung [W/m²]");

            var solarDailyRadiation = location.Calculate.Radiation(date);

            foreach (var solarMinutelyRadiation in solarDailyRadiation)
            {
                Console.WriteLine($"{solarMinutelyRadiation.Key:HH:mm}\t{solarMinutelyRadiation.Value:F2}");
            }

            Console.WriteLine("--------------");

            var solarDailyRadiationFromSunRiseTillSunSet = location.Calculate.RadiationSunriseToSunset(
                solarDailyRadiation, 
                new DateTime(2024, 12, 21, 8, 30, 0), 
                new DateTime(2024, 12, 21, 16, 27, 0));

            foreach (var solarMinutelyRadiation in solarDailyRadiationFromSunRiseTillSunSet)
            {
                Console.WriteLine($"{solarMinutelyRadiation.Key:HH:mm}\t{solarMinutelyRadiation.Value:F2}");
            }

            Console.ReadKey();
        }    }
}
