namespace GingerMintSoft.Earth.Location.Solar.Calculation.Astro
{
    internal static class Constants
    {
        // Umrechnungen
        public const double DegPerRad  = 180.0 / Math.PI;
        public const double RadPerDeg  = Math.PI / 180.0;

        // Julianisches Datum
        public const double J2000             = 2451545.0;   // 1.1.2000 12:00 UT
        public const double DaysPerCentury    = 36525.0;
        public const double JulianYearFactor  = 365.25;
        public const double JulianMonthFactor = 30.6001;
        public const double JulianOffset      = 1524.5;

        // Sonnenbahn
        public const double MeanLongSun         = 280.46646;
        public const double MeanLongSunRate     = 36000.76983;
        public const double MeanLongSunRateCorr = 0.0003032;
        public const double MeanAnomaly         = 357.52911;
        public const double MeanAnomalyRate     = 35999.05029;
        public const double MeanAnomalyRateCorr = 0.0001537;

        // Exzentrizität
        public const double Eccentricity      = 0.016708634;
        public const double EccentricityRate1 = 0.000042037;
        public const double EccentricityRate2 = 0.0000001267;

        // Gleichung des Zentrums
        public const double C1      = 1.914602;
        public const double C1Rate1 = 0.004817;
        public const double C1Rate2 = 0.000014;
        public const double C2      = 0.019993;
        public const double C2Rate  = 0.000101;
        public const double C3      = 0.000289;

        // Nutation & Schiefe
        public const double Omega          = 125.04;
        public const double OmegaRate      = 1934.136;
        public const double ObliquityRate1 = 46.815;
        public const double ObliquityRate2 = 0.00059;
        public const double ObliquityRate3 = 0.001813;
        public const double ObliquityCorr  = 0.00256;

        // Scheinbare Länge Korrektur
        public const double ApparentLongCorr1 = 0.00569;
        public const double ApparentLongCorr2 = 0.00478;

        // Sternzeit
        public const double Gmst       = 280.46061837;
        public const double GmstRate   = 360.98564736629;
        public const double GmstCoeff1 = 0.000387933;
        public const double GmstCoeff2 = 38710000.0;

        // Atmosphärische Parameter
        public const double SeaLevelPressure  = 1013.25; // hPa
        public const double SeaLevelTempK     = 288.15;  // Kelvin (15 °C)
        public const double LapseRate         = 0.0065;  // K/m
        public const double GravityTimesMolar = 0.034163; // g*M/R (dimensionless)
        public const double GasConstant       = 8.3144598; // J/(mol*K) (nur für Exponent)

        public const double DaysPerYear = 365.0;                       // Tage pro Jahr als Näherung
        public const double EarthAxisTilt = 23.44;                     // Neigung der Erdachse in Grad
        public const double SolarIrradiation = 1361;                   // Solarkonstante in W/m²
        public const double OpticalDepth = 0.2;                        // Typischer Wert für saubere Luft
        public const double AirScaleHeight = 8500.0;                   // Für Berechnung der atmosphärischen Dichte mit zunehmender Höhe
        public const double AirAltAdjustmentFactor = -0.0001184;       // Luftdichte nimmt mit zunehmender Höhe ab
    }
}
