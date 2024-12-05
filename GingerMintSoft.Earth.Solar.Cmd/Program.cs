using GingerMintSoft.Earth.Solar.Calculation;

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

            var date = new DateTime(2024, 6, 21, 0, 0, 0, DateTimeKind.Utc);

            // Solarstrahlung berechnen
            Console.WriteLine($"Stündliche Solarstrahlung für {date.ToShortDateString()}");
            Console.WriteLine("Stunde:Minute\tStrahlung [W/m²]");

            var solarDailyRadiation = location.Calculate.Radiation(date);

            foreach (var solarMinutelyRadiation in solarDailyRadiation)
            {
                Console.WriteLine($"{solarMinutelyRadiation.Key:HH:mm}\t{solarMinutelyRadiation.Value:F2}");
            }

            Console.WriteLine("--------------");
            var locationCoordinate = new Coordinate(location.Latitude, location.Longitude);
            var actDay = new CalcDayTime().SunriseSunset(date, locationCoordinate);

            var solarDailyRadiationFromSunRiseTillSunSet = location.Calculate.RadiationSunriseToSunset(
                solarDailyRadiation, 
                actDay.SunRise, 
                actDay.SunSet);

            foreach (var solarMinutelyRadiation in solarDailyRadiationFromSunRiseTillSunSet)
            {
                Console.WriteLine($"{solarMinutelyRadiation.Key:HH:mm}\t{solarMinutelyRadiation.Value:F2}");
            }

            Console.ReadKey();
        }
    }
}
