using GingerMintSoft.Earth.Solar.PowerPlant;

namespace GingerMintSoft.Earth.Solar.Calculation;

public class Calculate
{
    private const double SolarConstant = 1361; // Solarkonstante in W/m²
    private const double OpticalDepth = 0.2;    // Typischer Wert für saubere Luft

    /// <summary>
    /// Init Location on Greenwich Meridian
    /// Altitude above sea level in meters
    /// </summary>
    public Location? Location { get; set; }

    /// <summary>
    /// Berechnung der Solarstrahlung auf geneigte Flächen
    /// </summary>
    /// <param name="date"></param>
    /// <param name="roofIndex"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Dictionary<DateTime, double> RadiationOnTiltedPanel(DateTime date, int roofIndex = 0)
    {
        if (Location == null) throw new ArgumentNullException(nameof(Location));

        var solarRadiationDailyTilted = new Dictionary<DateTime, double>();

        for (var minute = 0; minute < TimeSpan.FromDays(1).TotalMinutes; minute++)
        {
            var currentDateTime = date.AddMinutes(minute);

            // Schritt 1: Sonnenstand berechnen
            (double solarAltitude, double solarAzimuth) = SolarPosition(currentDateTime, Location.Latitude, Location.Longitude);

            // Schritt 2: Atmosphärische Dämpfung berechnen
            double airMass = AirMass(solarAltitude);
            double atmosphericTransmission = AtmosphericTransmission(airMass, Location.Altitude);

            if(!Location.Roofs.Any()) throw new ArgumentNullException(nameof(Roof));
            var roof = Location.Roofs[roofIndex];

            // Schritt 3: Einstrahlung auf geneigte Fläche berechnen
            double radiation = RadiationOnTiltedSurface(atmosphericTransmission, solarAltitude, solarAzimuth, roof.Tilt, roof.Azimuth);

            solarRadiationDailyTilted.Add(currentDateTime.ToLocalTime(), radiation);
        }

        var actDay = new CalcDayTime().SunriseSunset(date, new Coordinate(Location.Latitude, Location.Longitude));

        return Location.Calculate.RadiationSunriseToSunset(
            solarRadiationDailyTilted, 
            actDay.SunRise, 
            actDay.SunSet);
    }

    /// <summary>
    /// Berechnung der Solarstrahlung in W/m² für einen Tag
    /// </summary>
    /// <param name="date">Strahlung für einen Tag von Sonnenauf bis Untergang</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Strahlung für einen Tag von Sonnenauf bis Untergang</returns>
    public Dictionary<DateTime, double> Radiation(DateTime date)
    {
        if (Location == null) throw new ArgumentNullException(nameof(Location));

        var actDay = new CalcDayTime().SunriseSunset(date, new Coordinate(Location.Latitude, Location.Longitude));

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
            var radiation = Irradiation(Location.Latitude, Location.Longitude, Location.Altitude, currentDateTime);
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
    public double Irradiation(
        double latitude,
        double longitude,
        double altitude,
        DateTime dateTime)
    {
        // Sonnenhöhe berechnen
        double solarElevation = Elevation(latitude, longitude, dateTime);

        // Keine direkte Solarstrahlung bei negativer Sonnenhöhe
        if (solarElevation <= 0) return 0;

        // Luftmassenindex basierend auf der Höhe und Sonnenstand
        double airMass = 1 / Math.Sin(solarElevation * Math.PI / 180); // Näherung für flache Winkel
        airMass *= Math.Exp(-altitude / 8500); // Höhenkorrektur

        // Solarstrahlung unter Berücksichtigung der Atmosphäre
        return SolarConstant * Math.Exp(-OpticalDepth * airMass);
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

    /// <summary>
    /// Berechnung der Solarposition
    /// </summary>
    /// <param name="time"></param>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    private (double solarAltitude, double solarAzimuth) SolarPosition(DateTime time, double latitude, double longitude)
    {
        if (Location == null) throw new ArgumentNullException(nameof(Location));

        double timezoneOffset = Location.TimeZoneOffset.Hours;
        double dayOfYear = time.DayOfYear;
        double hourOfDay = time.Hour + time.Minute / 60.0 - timezoneOffset;

        double declination = 23.45 * Math.Sin(2 * Math.PI / 365 * (dayOfYear - 81));
        double solarTime = hourOfDay + (4 * (longitude - 15 * timezoneOffset)) / 60.0;
        double hourAngle = 15 * (solarTime - 12);

        double solarAltitude = Math.Asin(
            Math.Sin(DegreeToRadian(latitude)) * Math.Sin(DegreeToRadian(declination)) +
            Math.Cos(DegreeToRadian(latitude)) * Math.Cos(DegreeToRadian(declination)) * Math.Cos(DegreeToRadian(hourAngle))
        );

        double solarAzimuth = Math.Acos(
            (Math.Sin(DegreeToRadian(declination)) - Math.Sin(DegreeToRadian(solarAltitude)) * Math.Sin(DegreeToRadian(latitude))) /
            (Math.Cos(DegreeToRadian(solarAltitude)) * Math.Cos(DegreeToRadian(latitude)))
        );

        if (hourAngle > 0) solarAzimuth = 360 - RadianToDegree(solarAzimuth);

        return (RadianToDegree(solarAltitude), RadianToDegree(solarAzimuth));
    }

    /// <summary>
    /// Berechnung der Luftmasse
    /// </summary>
    /// <param name="solarAltitude"></param>
    /// <returns></returns>
    private double AirMass(double solarAltitude)
    {
        if (solarAltitude <= 0) return double.MaxValue; // Keine Einstrahlung bei negativer Sonnenhöhe
        
        return 1 / Math.Sin(DegreeToRadian(solarAltitude));
    }

    /// <summary>
    /// Berechnet die atmosphärische Transmission basierend auf der Höhe
    /// </summary>
    /// <param name="airMass"></param>
    /// <param name="altitude"></param>
    /// <returns></returns>
    private double AtmosphericTransmission(double airMass, double altitude)
    {
        double altitudeFactor = Math.Exp(-0.0001184 * altitude); // Höhenanpassung
        
        return Math.Pow(0.7, airMass * altitudeFactor); // Abschwächung durch die Atmosphäre
    }

    /// <summary>
    /// Berechnet die solare Einstrahlung auf eine geneigte Fläche
    /// </summary>
    /// <param name="solarIrradiance"></param>
    /// <param name="solarAltitude"></param>
    /// <param name="solarAzimuth"></param>
    /// <param name="roofTilt"></param>
    /// <param name="roofAzimuth"></param>
    /// <returns></returns>
    private double RadiationOnTiltedSurface(double solarIrradiance, double solarAltitude, double solarAzimuth, double roofTilt, double roofAzimuth)
    {
        solarIrradiance *= SolarConstant;

        if (solarAltitude <= 0) return 0; // Keine Einstrahlung bei negativer Sonnenhöhe

        double incidenceAngle = Math.Acos(
            Math.Sin(DegreeToRadian(solarAltitude)) * Math.Cos(DegreeToRadian(roofTilt)) +
            Math.Cos(DegreeToRadian(solarAltitude)) * Math.Sin(DegreeToRadian(roofTilt)) * Math.Cos(DegreeToRadian(solarAzimuth - roofAzimuth)));

        solarIrradiance = solarIrradiance * Math.Cos(incidenceAngle);

        return solarIrradiance <= 0 ? 0 : solarIrradiance;
    }

    // Hilfsfunktionen für Grad- und Bogenmaß-Umrechnung
    private static double DegreeToRadian(double angle) => angle * Math.PI / 180;
    private static double RadianToDegree(double angle) => angle * 180 / Math.PI;
}
