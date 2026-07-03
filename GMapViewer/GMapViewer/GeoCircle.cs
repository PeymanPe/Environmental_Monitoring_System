using System;
using System.Collections.Generic;
using GMap.NET;

namespace GMapViewer
{
    public static class GeoCircle
    {
        public static List<PointLatLng> CreateCircle(
            PointLatLng center,
            double radiusMeters,
            int segments = 72)
        {
            List<PointLatLng> points = new List<PointLatLng>();

            const double EarthRadius = 6378137.0;

            double lat = center.Lat * Math.PI / 180.0;
            double lng = center.Lng * Math.PI / 180.0;

            double angularDistance = radiusMeters / EarthRadius;

            for (int i = 0; i <= segments; i++)
            {
                double bearing = i * 2.0 * Math.PI / segments;

                double lat2 = Math.Asin(
                    Math.Sin(lat) * Math.Cos(angularDistance) +
                    Math.Cos(lat) * Math.Sin(angularDistance) * Math.Cos(bearing));

                double lng2 = lng + Math.Atan2(
                    Math.Sin(bearing) * Math.Sin(angularDistance) * Math.Cos(lat),
                    Math.Cos(angularDistance) - Math.Sin(lat) * Math.Sin(lat2));

                points.Add(new PointLatLng(
                    lat2 * 180.0 / Math.PI,
                    lng2 * 180.0 / Math.PI));
            }

            return points;
        }
    }
}