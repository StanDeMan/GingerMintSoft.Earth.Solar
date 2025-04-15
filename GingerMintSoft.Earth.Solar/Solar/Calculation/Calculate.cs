using Newtonsoft.Json;

namespace GingerMintSoft.Earth.Location.Solar.Calculation;

public class Calculate
{
    private const double DaysPerYear = 365.0;                       // Tage pro Jahr als Näherung
    private const double EarthAxisTilt = 23.44;                     // Neigung der Erdachse in Grad
    private const double SolarConstant = 1361;                      // Solarkonstante in W/m²
    private const double OpticalDepth = 0.2;                        // Typischer Wert für saubere Luft
    private const double AirScaleHeight = 8500.0;                   // Für Berechnung der atmosphärische Dichte mit zunehmender Höhe
    private const double AirAltAdjustmentFactor = -0.0001184;       // Luftdichte nimmt mit zunehmender Höhe ab

    [JsonIgnore]
    public PowerPlant? Location { get; set; }

    public void InitLocation(PowerPlant location)
    {
        Location = location;
    }

    /// <summary>
    /// Berechnung der Solarstrahlung auf geneigte Flächen
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public List<Roof> RadiationOnTiltedPanel(DateTime date)
    {
        if (Location == null) throw new ArgumentOutOfRangeException (nameof(Location));
        if(!Location.Roofs.Any()) throw new ArgumentOutOfRangeException (nameof(Roof));

        foreach (var roof in Location.Roofs)
        {
            var solarRadiationDailyTilted = new Dictionary<DateTime, double>();

            for (var minute = 0; minute < TimeSpan.FromDays(1).TotalMinutes; minute++)
            {
                var currentDateTime = date.AddMinutes(minute);

                // Einstrahlung auf geneigte Fläche berechnen
                var radiation = RadiationOnTiltedSurface(currentDateTime, roof.Tilt, roof.Azimuth + roof.AzimuthDeviation);

                solarRadiationDailyTilted.Add(currentDateTime.ToLocalTime(), radiation);
            }

            var actDay = new CalcDayTime().SunriseSunset(date, new Coordinate(Location.Latitude, Location.Longitude));

            roof.Radiation = Location.Calculate!.RadiationSunriseToSunset(
                solarRadiationDailyTilted, 
                actDay.SunRise, 
                actDay.SunSet);

            roof.EarningData = roof.Earning();
        }

        return Location.Roofs;
    }

    /// <summary>
    /// Berechnung der Solarstrahlung in W/m² für einen Tag
    /// </summary>
    /// <param name="date">Strahlung für einen Tag von Sonnenauf bis Untergang</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Strahlung für einen Tag von Sonnenauf bis Untergang</returns>
    public Dictionary<DateTime, double> Radiation(DateTime date)
    {
        if (Location == null) throw new ArgumentOutOfRangeException (nameof(Location));

        var actDay = new CalcDayTime().SunriseSunset(date, new Coordinate(Location.Latitude, Location.Longitude));

        var solarDailyRadiation = Location.Calculate!.DailyRadiation(date);

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
        if (Location == null) throw new ArgumentOutOfRangeException (nameof(Location));

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
    /// see cref="https://de.wikipedia.org/wiki/Sonnenstrahlung"
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
        airMass *= Math.Exp(-altitude / AirScaleHeight); // Höhenkorrektur

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
    /// see cref="https://solarsena.com/solar-elevation-angle-altitude/"
    private double Elevation(
        double latitude,
        double longitude,
        DateTime dateTime)
    {
        // Berechnung der Sonnenhöhe (Elevation) basierend auf astronomischen Gleichungen
        double dayOfYear = dateTime.DayOfYear;
        double declination = EarthAxisTilt * Math.Sin(360 / DaysPerYear * (dayOfYear - 81) * Math.PI / 180);

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

        double declination = EarthAxisTilt * Math.Sin(2 * Math.PI / DaysPerYear * (dayOfYear - 81));
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

        if (hourAngle > 0) solarAzimuth = 2 * Math.PI - solarAzimuth;

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
    /// see cref="https://www.pveducation.org/pvcdrom/properties-of-sunlight/air-mass"
    private double AtmosphericTransmission(double airMass, double altitude)
    {
        double altitudeFactor = Math.Exp(AirAltAdjustmentFactor * altitude); // Höhenanpassung
        
        return Math.Pow(0.7, airMass * altitudeFactor); // Abschwächung durch die Atmosphäre
    }

    /// <summary>
    /// Berechnet die solare Einstrahlung auf eine geneigte Fläche
    /// </summary>
    /// <param name="currentDateTime"></param>
    /// <param name="roofTilt"></param>
    /// <param name="roofAzimuth"></param>
    /// <returns></returns>
    /// see cref="https://de.mathworks.com/matlabcentral/fileexchange/19791-solar-radiation"
    private double RadiationOnTiltedSurface(DateTime currentDateTime, double roofTilt, double roofAzimuth)
    {
        if (Location == null) throw new ArgumentNullException(nameof(Location));

        // Schritt 1: Sonnenstand berechnen
        (double solarAltitude, double solarAzimuth) = SolarPosition(currentDateTime, Location.Latitude, Location.Longitude);

        // Schritt 2: Atmosphärische Dämpfung berechnen
        double airMass = AirMass(solarAltitude);
        double atmosphericTransmission = AtmosphericTransmission(airMass, Location.Altitude);

        atmosphericTransmission *= SolarConstant;

        if (solarAltitude <= 0) return 0; // Keine Einstrahlung bei negativer Sonnenhöhe

        double incidenceAngle = Math.Acos(
            Math.Sin(DegreeToRadian(solarAltitude)) * 
            Math.Cos(DegreeToRadian(roofTilt)) +
            Math.Cos(DegreeToRadian(solarAltitude)) * 
            Math.Sin(DegreeToRadian(roofTilt)) * 
            Math.Cos(DegreeToRadian((solarAzimuth - roofAzimuth + 360) % 360)));

        atmosphericTransmission *= Math.Cos(incidenceAngle);

        return atmosphericTransmission <= 0 ? 0 : atmosphericTransmission;
    }

    // Hilfsfunktionen für Grad- und Bogenmaß-Umrechnung
    private static double DegreeToRadian(double angle) => angle * Math.PI / 180;
    private static double RadianToDegree(double angle) => angle * 180 / Math.PI;
}
