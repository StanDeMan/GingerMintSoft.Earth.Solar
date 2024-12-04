using GingerMintSoft.Earth.Solar.Calculation;
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
            DateTime date = new DateTime(2023, 10, 1);

            // Act
            var result = _calculate!.Radiation(date);

            // Assert
            Assert.AreEqual(1440, result.Count); // 1440 minutes in a day
            foreach (var kvp in result)
            {
                Assert.IsTrue(kvp.Value >= 0);
            }
        }

        [TestMethod]
        public void TestRadiationSunriseToSunset_ReturnsCorrectDictionary()
        {
            // Arrange
            DateTime date = new DateTime(2023, 10, 1);
            var solarRadiationDaily = _calculate!.Radiation(date);
            DateTime sunRise = date.AddHours(6); // Assuming sunrise at 6 AM
            DateTime sunSet = date.AddHours(18); // Assuming sunset at 6 PM

            // Act
            var result = _calculate.RadiationSunriseToSunset(solarRadiationDaily, sunRise, sunSet);

            // Assert
            Assert.IsTrue(result.All(kvp => kvp.Key.TimeOfDay >= sunRise.TimeOfDay && kvp.Key.TimeOfDay <= sunSet.TimeOfDay));
        }

        [TestMethod]
        public void TestIsBetween_ReturnsCorrectBoolean()
        {
            // Arrange
            DateTime actual = new DateTime(2023, 10, 1, 12, 0, 0); // 12 PM
            TimeSpan start = new TimeSpan(6, 0, 0); // 6 AM
            TimeSpan end = new TimeSpan(18, 0, 0); // 6 PM

            // Act
            bool result = InvokeIsBetween(actual, start, end);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCalculateRadiation_ReturnsCorrectValue()
        {
            // Arrange
            double latitude = 45.0;
            double longitude = 90.0;
            double altitude = 100;
            DateTime dateTime = new DateTime(2023, 10, 1, 12, 0, 0); // 12 PM

            // Act
            double result = _calculate!.CalculateRadiation(latitude, longitude, altitude, dateTime);

            // Assert
            Assert.IsTrue(result >= 0);
        }

        [TestMethod]
        public void TestElevation_ReturnsCorrectValue()
        {
            // Arrange
            double latitude = 48.093;
            double longitude = 7.896;
            DateTime dateTime = new DateTime(2024, 6, 22, 12, 0, 0); // 12 PM

            // Act
            double result = InvokeElevation(latitude, longitude, dateTime);

            // Assert
            Assert.IsTrue(result >= 64);
        }

        private double InvokeElevation(double latitude, double longitude, DateTime dateTime)
        {
            var methodInfo = typeof(Calculate).GetMethod("Elevation", BindingFlags.NonPublic | BindingFlags.Instance);
            return (double)methodInfo!.Invoke(_calculate, new object[] { latitude, longitude, dateTime })!;
        }

        private bool InvokeIsBetween(DateTime actual, TimeSpan start, TimeSpan end)
        {
            var methodInfo = typeof(Calculate).GetMethod("IsBetween", BindingFlags.NonPublic | BindingFlags.Static);
            return (bool)methodInfo!.Invoke(null, new object[] { actual, start, end })!;
        }
    }
}
