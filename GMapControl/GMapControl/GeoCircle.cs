using System;
using System.Collections.Generic;
using GMap.NET;

namespace GMapControl
{
    internal static class GeoCircle
    {
        private const double EarthRadius = 6378137.0;

        public static List<PointLatLng> CreateCircle(
            PointLatLng center,
            double radiusMeters,
            int segments = 72)
        {
            List<PointLatLng> points = new List<PointLatLng>();

            double lat = DegreesToRadians(center.Lat);
            double lon = DegreesToRadians(center.Lng);

            double angularDistance = radiusMeters / EarthRadius;

            for (int i = 0; i <= segments; i++)
            {
                double bearing = 2.0 * Math.PI * i / segments;

                double lat2 =
                    Math.Asin(
                        Math.Sin(lat) * Math.Cos(angularDistance) +
                        Math.Cos(lat) * Math.Sin(angularDistance) * Math.Cos(bearing));

                double lon2 =
                    lon +
                    Math.Atan2(
                        Math.Sin(bearing) *
                        Math.Sin(angularDistance) *
                        Math.Cos(lat),

                        Math.Cos(angularDistance) -
                        Math.Sin(lat) * Math.Sin(lat2));

                points.Add(
                    new PointLatLng(
                        RadiansToDegrees(lat2),
                        RadiansToDegrees(lon2)));
            }

            return points;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
    }
}