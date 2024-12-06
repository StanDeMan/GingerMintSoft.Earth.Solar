namespace GingerMintSoft.Earth.Solar.Calculation;

public class Calculate
{
    [Flags]
    public enum EnmRadiation
    {
        None = 0b0000,
        ForDay = 0b0001,
        FromSunRiseTillSunSet = 0b0010
    }

    /// <summary>
    /// Init Location on Greenwich Meridian
    /// Altitude above sea level in meters
    /// </summary>
    public Location? Location { get; set; }

    /// <summary>
    /// Berechnung der Solarstrahlung in W/m² für einen Tag
    /// </summary>
    /// <param name="date">Strahlung für einen Tag von Sonnenauf bis Untergang</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Strahlung für einen Tag von Sonnenauf bis Untergang</returns>
    public Dictionary<DateTime, double> Radiation(DateTime date)
    {
        if (Location == null) throw new ArgumentNullException(nameof(Location));

        var locationCoordinate = new Coordinate(Location.Latitude, Location.Longitude);
        var actDay = new CalcDayTime().SunriseSunset(date, locationCoordinate);

        var solarDailyRadiation = Location.Calculate.DailyRadiation(date);

        return Location.Calculate.RadiationSunriseToSunset(
            solarDailyRadiation, 
            actDay.SunRise, 
            actDay.SunSet);
    }

    /// <summary>
    /// Berechnung der Solarstrahlung in W/m² für einen Tag jede Minute
    /// </summary>
    /// <returns>Strahlung für einen Tag</returns>
    private Dictionary<DateTime, double> DailyRadiation(DateTime date)
    {
        if (Location == null) throw new ArgumentNullException(nameof(Location));

        var solarRadiationDaily = new Dictionary<DateTime, double>();

        for (var minute = 0; minute < TimeSpan.FromDays(1).TotalMinutes; minute++)
        {
            var currentDateTime = date.AddMinutes(minute);
            var radiation = CalculateRadiation(Location.Latitude, Location.Longitude, Location.Altitude, currentDateTime);
            solarRadiationDaily.Add(currentDateTime.ToLocalTime(), radiation);
        }

        return solarRadiationDaily;
    }

    /// <summary>
    /// Berechnung der Solarstrahlung in W/m² für einen Tag von Sonnenaufgang bis Sonnenuntergang per Minute
    /// </summary>
    /// <param name="solarRadiationDaily"></param>
    /// <param name="sunRise"></param>
    /// <param name="sunSet"></param>
    /// <returns></returns>
    private Dictionary<DateTime, double> RadiationSunriseToSunset(
        Dictionary<DateTime, double> solarRadiationDaily,
        DateTime sunRise,
        DateTime sunSet)
    {
        return solarRadiationDaily.Where(solarRadiationInRange =>
            IsBetween(solarRadiationInRange.Key, sunRise.TimeOfDay, sunSet.TimeOfDay))
            .ToDictionary(solarRadiationByMinute =>
                solarRadiationByMinute.Key, solarRadiationByMinute =>
                solarRadiationByMinute.Value);
    }

    /// <summary>
    /// Prüfen ob ein Wert zwischen den zwei Werten liegt
    /// </summary>
    /// <param name="actual">Actuller Wert</param>
    /// <param name="start">Von hier</param>
    /// <param name="end">Bis hier</param>
    /// <returns>Werte im Bereich</returns>
    private static bool IsBetween(DateTime actual, TimeSpan start, TimeSpan end)
    {
        var time = actual.TimeOfDay;

        return start <= end
            ? time >= start && time <= end  // Scenario 1: If the start time and the end time are in the same day.
            : time >= start || time <= end; // Scenario 2: The start time and end time is on different days.
    }

    /// <summary>
    /// Berechnung der Solarstrahlung in W/m²
    /// </summary>
    /// <param name="latitude">Längengrad</param>
    /// <param name="longitude">Breitengrad</param>
    /// <param name="altitude">Höhe des Messpunktes</param>
    /// <param name="dateTime">Zu dieser Zeit</param>
    /// <returns>Solarstrahlung in W/m²</returns>
    public double CalculateRadiation(
        double latitude,
        double longitude,
        double altitude,
        DateTime dateTime)
    {
        double solarConstant = 1361;    // Solarkonstante in W/m²
        double opticalDepth = 0.2;      // Typischer Wert für saubere Luft

        // Sonnenhöhe berechnen
        double solarElevation = Elevation(latitude, longitude, dateTime);

        // Keine direkte Solarstrahlung bei negativer Sonnenhöhe
        if (solarElevation <= 0) return 0;

        // Luftmassenindex basierend auf der Höhe und Sonnenstand
        double airMass = 1 / Math.Sin(solarElevation * Math.PI / 180); // Näherung für flache Winkel
        airMass *= Math.Exp(-altitude / 8500); // Höhenkorrektur

        // Solarstrahlung unter Berücksichtigung der Atmosphäre
        return solarConstant * Math.Exp(-opticalDepth * airMass);
    }

    /// <summary>
    /// Berechnung der Sonnenhöhe in Grad
    /// </summary>
    /// <param name="latitude">Längengrad</param>
    /// <param name="longitude">Breitengrad</param>
    /// <param name="dateTime">Zu dieser Zeit</param>
    /// <returns>Sonnenhöhe in Grad</returns>
    private double Elevation(
        double latitude,
        double longitude,
        DateTime dateTime)
    {
        // Berechnung der Sonnenhöhe (Elevation) basierend auf astronomischen Gleichungen
        double dayOfYear = dateTime.DayOfYear;
        double declination = 23.45 * Math.Sin(360 / 365.0 * (dayOfYear - 81) * Math.PI / 180);

        double timeInHours = dateTime.Hour + dateTime.Minute / 60.0;
        double solarTime = timeInHours + 4 * longitude / 60.0;  // Solarzeit-Korrektur
        double hourAngle = (solarTime - 12) * 15;               // Stundengrad (positiv am Nachmittag)

        double latitudeRad = latitude * Math.PI / 180;
        double declinationRad = declination * Math.PI / 180;

        double elevationRad = Math.Asin(
            Math.Sin(latitudeRad) * Math.Sin(declinationRad) +
            Math.Cos(latitudeRad) * Math.Cos(declinationRad) * Math.Cos(hourAngle * Math.PI / 180)
        );

        return elevationRad * 180 / Math.PI;                    // Rückgabe der Sonnenhöhe in Grad
    }
}
