namespace GingerMintSoft.Earth.Solar.Tests
{
    [TestClass]
    public sealed class LocationTests
    {
        [TestMethod]
        public void TestLocation_Constructor_InitializesProperties()
        {
            // Arrange
            int altitude = 100;
            double latitude = 45.0;
            double longitude = 90.0;

            // Act
            var location = new Location(altitude, latitude, longitude);

            // Assert
            Assert.AreEqual(altitude, location.Altitude);
            Assert.AreEqual(latitude, location.Latitude);
            Assert.AreEqual(longitude, location.Longitude);
            Assert.IsNotNull(location.Calculate);
            Assert.AreEqual(location, location.Calculate.Location);
        }

        [TestMethod]
        public void TestLocation_SetAltitude_UpdatesProperty()
        {
            // Arrange
            var location = new Location(100, 45.0, 90.0);
            int newAltitude = 200;

            // Act
            location.Altitude = newAltitude;

            // Assert
            Assert.AreEqual(newAltitude, location.Altitude);
        }

        [TestMethod]
        public void TestLocation_SetLatitude_UpdatesProperty()
        {
            // Arrange
            var location = new Location(100, 45.0, 90.0);
            double newLatitude = 50.0;

            // Act
            location.Latitude = newLatitude;

            // Assert
            Assert.AreEqual(newLatitude, location.Latitude);
        }

        [TestMethod]
        public void TestLocation_SetLongitude_UpdatesProperty()
        {
            // Arrange
            var location = new Location(100, 45.0, 90.0);
            double newLongitude = 95.0;

            // Act
            location.Longitude = newLongitude;

            // Assert
            Assert.AreEqual(newLongitude, location.Longitude);
        }
    }
}
