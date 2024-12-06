using GingerMintSoft.Earth.Solar.Calculation;
using GingerMintSoft.Earth.Solar.PowerPlant;
using System.Reflection;

namespace GingerMintSoft.Earth.Solar.Tests
{
    [TestClass]
    public sealed class CalculateTests
    {
        private Calculate? _calculate;
        private Location? _location;

        [TestInitialize]
        public void Setup()
        {
            _location = new Location(100, 45.0, 90.0);
            _calculate = new Calculate { Location = _location };
        }

        [TestMethod]
        public void TestRadiation_ReturnsCorrectDictionary()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);

            // Act
            var result = _calculate!.Radiation(date);

            // Assert
            Assert.AreEqual(705, result.Count); // 705 solar minutes for this specific date
            foreach (var kvp in result)
            {
                Assert.IsTrue(kvp.Value >= 0);
            }
        }

        [TestMethod]
        public void TestRadiationSunriseToSunset_ReturnsCorrectDictionary()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);
            var solarRadiationDaily = _calculate!.Radiation(date);

            var sunRise = date.AddHours(6); // Assuming sunrise at 6 AM
            var sunSet = date.AddHours(18); // Assuming sunset at 6 PM

            // Radiation for this specific date
            var result = InvokeRadiationSunriseToSunset(solarRadiationDaily, sunRise, sunSet);

            // Assert
            Assert.IsTrue(result.All(kvp => kvp.Key.TimeOfDay >= sunRise.TimeOfDay && kvp.Key.TimeOfDay <= sunSet.TimeOfDay));
        }

        [TestMethod]
        public void TestIsBetween_ReturnsCorrectBoolean()
        {
            // Arrange
            var actual = new DateTime(2023, 10, 1, 12, 0, 0); // 12 PM
            var start = new TimeSpan(6, 0, 0); // 6 AM
            var end = new TimeSpan(18, 0, 0); // 6 PM

            // Act
            var result = InvokeIsBetween(actual, start, end);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCalculateRadiation_ReturnsCorrectValue()
        {
            // Arrange
            const double latitude = 45.0;
            const double longitude = 90.0;
            const double altitude = 100;
            var dateTime = new DateTime(2023, 10, 1, 12, 0, 0); // 12 PM

            // Act
            var result = _calculate!.Irradiation(latitude, longitude, altitude, dateTime);

            // Assert
            Assert.IsTrue(result >= 0);
        }

        [TestMethod]
        public void TestElevation_ReturnsCorrectValue()
        {
            // Arrange
            const double latitude = 48.093;
            const double longitude = 7.896;
            var dateTime = new DateTime(2024, 6, 22, 12, 0, 0); // 12 PM

            // Act
            var result = InvokeElevation(latitude, longitude, dateTime);

            // Assert
            Assert.IsTrue(result >= 64);
        }

        // Invoke private methods for testing
        private double InvokeElevation(double latitude, double longitude, DateTime dateTime)
        {
            var methodInfo = typeof(Calculate).GetMethod("Elevation", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (methodInfo == null)
            {
                throw new InvalidOperationException("Method 'Elevation' not found.");
            }

            return (double)methodInfo.Invoke(_calculate, [latitude, longitude, dateTime])!;
        }

        private static bool InvokeIsBetween(DateTime actual, TimeSpan start, TimeSpan end)
        {
            var methodInfo = typeof(Calculate).GetMethod("IsBetween", BindingFlags.NonPublic | BindingFlags.Static);

            if (methodInfo == null)
            {
                throw new InvalidOperationException("Method 'IsBetween' not found.");
            }

            return (bool)methodInfo.Invoke(null, [actual, start, end])!;
        }

        private Dictionary<DateTime, double> InvokeRadiationSunriseToSunset(Dictionary<DateTime, double> dailyData, DateTime sunRise, DateTime sunSet)
        {
            var methodInfo = typeof(Calculate).GetMethod("RadiationSunriseToSunset", BindingFlags.NonPublic | BindingFlags.Instance);

            if (methodInfo == null)
            {
                throw new InvalidOperationException("Method 'RadiationSunriseToSunset' not found.");
            }

            // Ensure _calculate is not null
            if (_calculate == null)
            {
                throw new InvalidOperationException("Calculate instance is not initialized.");
            }

            return (Dictionary<DateTime, double>)methodInfo.Invoke(_calculate, [dailyData, sunRise, sunSet])!;
        }
    }
}
