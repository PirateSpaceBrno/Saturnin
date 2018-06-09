using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Saturnin.Saturnin;
using Saturnin.Models;

namespace Saturnin.Helpers
{
    public static class GeoLocationHelper
    {
        private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = ToRadians(lat2 - lat1);  // deg2rad below
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km

            d = d * 1000; //convert km to meters
            return d;
        }

        private static double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        /// <summary>
        /// Checks if specified point is in specified radius within this point.
        /// </summary>
        /// <param name="centerPoint">This point (center of radius)</param>
        /// <param name="checkedPoint">Checked point (discover is point is inside or outside radius)</param>
        /// <param name="radius">Radius in meters</param>
        /// <returns></returns>
        public static bool Contains(this GeoCoordinations centerPoint, GeoCoordinations checkedPoint, double radius)
        {
            return (GetDistance(centerPoint.latitude, centerPoint.longitude, checkedPoint.latitude, checkedPoint.longitude) < radius);
        }

        public struct GeoCoordinations
        {
            public double latitude;
            public double longitude;
        }

        /// <summary>
        /// Zjistí, zda se nějaký autobus DPMB zadané linky nachází v okruhu zadaného místa
        /// </summary>
        /// <param name="dpmbSubscriber"></param>
        /// <param name="buses"></param>
        /// <returns></returns>
        public static async Task<List<DpmbRisObject>> WatchDpmbLine(DpmbSubscriber dpmbSubscriber, List<DpmbRisObject> buses)
        {
            List<DpmbRisObject> result = new List<DpmbRisObject>();

            foreach(DpmbRisObject bus in buses)
            {
                var busLocation = new GeoCoordinations()
                {
                    latitude = bus.latitude,
                    longitude = bus.longitude
                };
                if (dpmbSubscriber.centerPoint.Contains(busLocation, dpmbSubscriber.radius))
                    result.Add(bus);
            }

            return result;
        }
    }
}
