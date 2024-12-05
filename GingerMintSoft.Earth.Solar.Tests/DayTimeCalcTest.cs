using GingerMintSoft.Earth.Solar.Calculation;

namespace GingerMintSoft.Earth.Solar.Tests
{
    [TestClass]
    public class DayTimeCalcYearTest
    {
        [TestMethod]
        public void TestSunriseSunSetTimeForOneYear()
        {
            var home = new Coordinate(48.10507778308992, 7.9085683992118);

            // iterate through whole year 
            var actDate = new DateTime(DateTime.Now.Year, 1, 1);
            var nextYear = DateTime.Now.Year + 1;   

            while(actDate.Year < nextYear)
            {
                // Parameters : year - month - day - lat - long
                var actDay = new CalcDayTime().SunriseSunset(actDate, home);
            
                Console.WriteLine($"{actDate.Day:00}.{actDate.Month:00}.{actDate.Year} Sunrise: {actDay.SunRise}");
                Console.WriteLine($"           Sunset:  {actDay.SunSet}");
                Console.WriteLine($"        DayLength:  {actDay.DayLength.Hours:00}:{actDay.DayLength.Minutes:00}:{actDay.DayLength.Seconds:00}");
                Console.WriteLine("---------------------------------------");

                actDate = actDate.AddDays(1);
            }

            Assert.IsTrue(new DateTime(DateTime.Now.Year + 1, 1, 1) == actDate);
        }
    }
}

