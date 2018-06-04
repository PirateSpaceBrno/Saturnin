using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saturnin.Helpers;
using TechTalk.SpecFlow;
using NUnit.Framework;
using static Saturnin.Helpers.GeoLocationHelper;

namespace Saturnin.Tests.Steps
{
    [Binding]
    public class GeoLocationSteps
    {
        [Then("Point '(.*)''(.*)' is '(inside|outside)' circle with center '(.*)''(.*)' and radius '(.*)'")]
        public void TestPointInRadiusMethod(string lat1, string lon1, string insideoutside, string lat2, string lon2, string radius)
        {
            var checkedPoint = new GeoCoordinations()
            {
                latitude = Convert.ToDouble(lat1),
                longitude = Convert.ToDouble(lon1)
            };

            var centerPoint = new GeoCoordinations()
            {
                latitude = Convert.ToDouble(lat2),
                longitude = Convert.ToDouble(lon2)
            };

            var result = centerPoint.Contains(checkedPoint, Convert.ToDouble(radius));

            Assert.AreEqual((insideoutside == "inside"), result);
        }
    }
}
