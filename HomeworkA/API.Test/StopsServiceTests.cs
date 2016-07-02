using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using API.Services;

namespace API.Test
{
    [TestClass]
    public class StopsServiceTests
    {
        private StopsService stopsService;

        [TestInitialize]
        public void TestInitialize()
        {
            stopsService = new StopsService();
        }

        [TestMethod]
        public void StopsService_GetClosestArrivalTimes_PositiveTest()
        {
            // Arrange
            var timeSpan = new TimeSpan(15, 1, 0);
            var stopNumber = 1;

            // Act
            var arrivalTimes = stopsService.GetClosestArrivalTimes(stopNumber, timeSpan).ToList();

            // Assert
            Assert.IsNotNull(arrivalTimes);
            Assert.AreEqual(3, arrivalTimes.Count);

            Assert.AreEqual("Route 1", arrivalTimes[0].RouteName);
            Assert.AreEqual("Route 2", arrivalTimes[1].RouteName);
            Assert.AreEqual("Route 3", arrivalTimes[2].RouteName);

            Assert.AreEqual(new TimeSpan(15, 15, 0), arrivalTimes[0].ArrivalTimes.ToList()[0]);
            Assert.AreEqual(new TimeSpan(15, 30, 0), arrivalTimes[0].ArrivalTimes.ToList()[1]);

            Assert.AreEqual(new TimeSpan(15, 2, 0), arrivalTimes[1].ArrivalTimes.ToList()[0]);
            Assert.AreEqual(new TimeSpan(15, 17, 0), arrivalTimes[1].ArrivalTimes.ToList()[1]);

            Assert.AreEqual(new TimeSpan(15, 4, 0), arrivalTimes[2].ArrivalTimes.ToList()[0]);
            Assert.AreEqual(new TimeSpan(15, 19, 0), arrivalTimes[2].ArrivalTimes.ToList()[1]);
        }
    }
}
