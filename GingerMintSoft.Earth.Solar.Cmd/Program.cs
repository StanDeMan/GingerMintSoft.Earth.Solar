﻿namespace GingerMintSoft.Earth.Solar.Cmd
{
    public static class Program
    {
        private static void Main()
        {
            // Solar location
            var location = new Location(
                232,                    // Höhe über NN in Metern
                48.1051268096319,       // Breitengrad in Dezimalgrad
                7.9085366169705145      // Längengrad in Dezimalgrad
            );

            var date = new DateTime(2024, 6, 21, 0, 0, 0, DateTimeKind.Utc);

            // Calculate solar radiation from sunrise to sunset
            Console.WriteLine($"Stündliche Solarstrahlung für {date.ToShortDateString()}");
            Console.WriteLine("Stunde:Minute\tStrahlung [W/m²]");

            var solarDailyRadiationFromSunRiseTillSunSet = location.Calculate.Radiation(date);

            foreach (var solarMinutelyRadiation in solarDailyRadiationFromSunRiseTillSunSet)
            {
                Console.WriteLine($"{solarMinutelyRadiation.Key:HH:mm}\t{solarMinutelyRadiation.Value:F2}");
            }

            Console.ReadKey();
        }
    }
}
