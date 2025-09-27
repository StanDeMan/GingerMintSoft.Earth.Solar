using System.Text.Json.Serialization;
using GingerMintSoft.Earth.Location.Solar.Calculation.Astro;

namespace GingerMintSoft.Earth.Location.Solar.Calculation;

public class Calculate
{
    private const double DaysPerYear = 365.0;                       // Tage pro Jahr als Näherung
    private const double EarthAxisTilt = 23.44;                     // Neigung der Erdachse in Grad
    private const double SolarConstant = 1361;                      // Solarkonstante in W/m²
    private const double OpticalDepth = 0.2;                        // Typischer Wert für saubere Luft
    private const double AirScaleHeight = 8500.0;                   // Für Berechnung der atmosphärischen Dichte mit zunehmender Höhe
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
    /// Berechnung der Solarstrahlung in W/m² für einen Tag von Sonnenaufgang bis Sonnenuntergang pro Minute
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
        if (solarAltitude <= 0) return double.NegativeZero; // Keine Einstrahlung bei negativer Sonnenhöhe
        
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
            Math.Cos(DegreeToRadian(roofTilt)) -
            Math.Cos(DegreeToRadian(solarAltitude)) * 
            Math.Sin(DegreeToRadian(roofTilt)) * 
            Math.Cos(DegreeToRadian((solarAzimuth + roofAzimuth + 360) % 360)));

        atmosphericTransmission *= Math.Cos(incidenceAngle);

        return atmosphericTransmission <= 0 ? 0 : atmosphericTransmission;
    }

    static double DegreeToRadian(double angle) => angle * Constants.RadPerDeg;
    static double RadianToDegree(double angle) => angle * Constants.DegPerRad;

    /// <summary>
    /// sun related calculations
    /// </summary>
    public class Sun
    {
        public int Altitude { get; init; }
        public double Longitude { get; init; }
        public double Latitude { get; init; }

        /// <summary>
        /// Berechnet Sonnen-Azimut und scheinbare Höhe (inkl. Refraktion)
        /// </summary>
        /// <param name="dateTime">For this date and time</param>
        /// <param name="temperature"></param>
        /// <returns></returns>
        public (double solarAltitude, double solarAzimuth) Position(DateTime dateTime, double temperature = 15.0)
        {
            var calculate = new Calculate();
            calculate.InitLocation(new PowerPlant("Location", Altitude, Latitude, Longitude));

            DateTime utc = dateTime.ToUniversalTime();
            double jd = JulianDay(utc);
            double t  = (jd - Constants.J2000) / Constants.DaysPerCentury;

            // astronomische Berechnungen
            double m = Constants.MeanAnomaly +
                       t * (Constants.MeanAnomalyRate - Constants.MeanAnomalyRateCorr * t);

            double l = Normalize(Constants.MeanLongSun +
                                  t * (Constants.MeanLongSunRate + Constants.MeanLongSunRateCorr * t));

            double c = (Constants.C1 - t * (Constants.C1Rate1 + Constants.C1Rate2 * t)) * Math.Sin(DegreeToRadian(m))
                     + (Constants.C2 - Constants.C2Rate * t) * Math.Sin(DegreeToRadian(2 * m))
                     + Constants.C3 * Math.Sin(DegreeToRadian(3 * m));

            double trueLong = l + c;

            double omega  = Constants.Omega - Constants.OmegaRate * t;
            double lambda = trueLong - Constants.ApparentLongCorr1
                            - Constants.ApparentLongCorr2 * Math.Sin(DegreeToRadian(omega));

            double eps0 = 23 + (26 + ((21.448 - t * (Constants.ObliquityRate1 +
                          t * (Constants.ObliquityRate2 - Constants.ObliquityRate3 * t))) / 60)) / 60;
            double eps  = eps0 + Constants.ObliquityCorr * Math.Cos(DegreeToRadian(omega));

            double alpha = RadianToDegree(Math.Atan2(Math.Cos(DegreeToRadian(eps)) * Math.Sin(DegreeToRadian(lambda)),
                                              Math.Cos(DegreeToRadian(lambda))));
            double delta = RadianToDegree(Math.Asin(Math.Sin(DegreeToRadian(eps)) * Math.Sin(DegreeToRadian(lambda))));
            alpha = Normalize(alpha);

            double gmst = Constants.Gmst + Constants.GmstRate * (jd - Constants.J2000)
                          + t * t * (Constants.GmstCoeff1 - t / Constants.GmstCoeff2);
            gmst = Normalize(gmst);

            double lst = Normalize(gmst + Longitude);
            double h   = Normalize(lst - alpha);

            // Geometrische Höhe
            double altitude = RadianToDegree(Math.Asin(Math.Sin(DegreeToRadian(Latitude)) * Math.Sin(DegreeToRadian(delta))
                                 + Math.Cos(DegreeToRadian(Latitude)) * Math.Cos(DegreeToRadian(delta)) * Math.Cos(DegreeToRadian(h))));

            // Azimut (0° = Nord, 90° = Ost)
            double sunAzimuth = RadianToDegree(Math.Atan2(-Math.Sin(DegreeToRadian(h)),
                                    Math.Tan(DegreeToRadian(delta)) * Math.Cos(DegreeToRadian(Latitude))
                                  - Math.Sin(DegreeToRadian(Latitude)) * Math.Cos(DegreeToRadian(h))));
            sunAzimuth = Normalize(sunAzimuth);

            // ---- Refraktionskorrektur ----
            double pressure = PressureFromElevation(Altitude); // Luftdruck in hPa   
            double sunAltitude = altitude + RefractionCorrection(altitude, temperature, pressure);

            return (TruncateToDecimalPlace(sunAzimuth), TruncateToDecimalPlace(sunAltitude));
        }

        /// <summary>
        /// Luftdruck nach barometrischer Höhenformel (vereinfachte ISA-Standardatmosphäre)
        /// </summary>
        /// <param name="altitude"></param>
        /// <returns></returns>
        private static double PressureFromElevation(double altitude)
        {
            // Druck in hPa
            return Constants.SeaLevelPressure *
                   Math.Pow(1.0 - (Constants.LapseRate * altitude) /
                       Constants.SeaLevelTempK,
                       Constants.GravityTimesMolar / (Constants.GasConstant * Constants.LapseRate));
        }

        /// <summary>
        ///  Näherungsformel für atmosphärische Refraktion (Grad)
        /// </summary>
        /// <param name="altitudeDeg"></param>
        /// <param name="tempC"></param>
        /// <param name="pressureHPa"></param>
        /// <returns></returns>
        private static double RefractionCorrection(double altitudeDeg, double tempC, double pressureHPa)
        {
            if (altitudeDeg < -1) return 0.0; // unterhalb des Horizonts keine Korrektur
            double kelvin = tempC + 273.15;
            // Standard-Formel (Bennett 1982)
            double refraction = (pressureHPa / 1010.0) 
                * (283.0 / kelvin) 
                * 1.02 / (60.0 * Math.Tan(DegreeToRadian(altitudeDeg + 10.3 / (altitudeDeg + 5.11))));
            
            return refraction; // in Grad
        }

        /// <summary>
        /// Julianisches Datum aus Datum/Zeit (UTC)
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        private static double JulianDay(DateTime utc)
        {
            int year = utc.Year;
            int month = utc.Month;
            double d = utc.Day + (utc.Hour + (utc.Minute + utc.Second / 60.0) / 60.0) / 24.0;

            if (month <= 2) { year -= 1; month += 12; }

            int a = year / 100;
            int b = 2 - a + a / 4;

            return Math.Floor(Constants.JulianYearFactor * (year + 4716))
                 + Math.Floor(Constants.JulianMonthFactor * (month + 1))
                 + d + b - Constants.JulianOffset;
        }

        /// <summary>
        /// Truncates a decimal number to the specified number of decimal places without rounding.
        /// </summary>
        /// <remarks>This method does not perform rounding; it simply removes any digits beyond the
        /// specified decimal places.</remarks>
        /// <param name="numberToTruncate">The decimal number to truncate.</param>
        /// <param name="decimalPlaces">The number of decimal places to retain. Must be a non-negative integer.</param>
        /// <returns>A decimal number truncated to the specified number of decimal places. If <paramref name="decimalPlaces"/> is
        /// 0,  the method returns the integer part of <paramref name="numberToTruncate"/>.</returns>
        private static double TruncateToDecimalPlace(double numberToTruncate, int decimalPlaces = 2)
        {
            var power = (decimal)(Math.Pow(10.0, decimalPlaces));

            return (double)(Math.Truncate((power * (decimal)numberToTruncate)) / power);
        }

        static double Normalize(double deg) => (deg % 360.0 + 360.0) % 360.0;
    }
}
