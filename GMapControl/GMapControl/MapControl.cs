using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Windows.Forms;

namespace GMapControl
{
    public partial class MapControl : UserControl
    {
        private double clickedLat;
        private double clickedLng;

        private readonly GMapOverlay markerOverlay = new GMapOverlay("Markers");

        public MapControl()
        {
            InitializeComponent();

            GMaps.Instance.Mode = AccessMode.ServerOnly;

            // Default map
            gMapControl1.MapProvider = GoogleMapProvider.Instance;

            // Default location (Oulu)
            gMapControl1.Position = new PointLatLng(65.0121, 25.4651);

            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 13;

            gMapControl1.DragButton = MouseButtons.Left;

            gMapControl1.Overlays.Add(markerOverlay);

            gMapControl1.MouseDown += GMapControl1_MouseDown;
            gMapControl1.OnMarkerClick += GMapControl1_OnMarkerClick;
        }

        //----------------------------------------------------
        // User clicked somewhere on the map
        //----------------------------------------------------
        private void GMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            clickedLat = point.Lat;
            clickedLng = point.Lng;

            // No marker is added here.
            // LabVIEW will read the coordinates and decide what to display.
        }

        //----------------------------------------------------
        // Right-click marker -> remove it
        //----------------------------------------------------
        private void GMapControl1_OnMarkerClick(GMapMarker marker, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                markerOverlay.Markers.Remove(marker);
                gMapControl1.Refresh();
            }
        }

        //----------------------------------------------------
        // Read selected coordinates
        //----------------------------------------------------
        public double GetLatitude()
        {
            return clickedLat;
        }

        public double GetLongitude()
        {
            return clickedLng;
        }

        //----------------------------------------------------
        // Center map
        //----------------------------------------------------
        public void CenterMap(double lat, double lon)
        {
            gMapControl1.Position = new PointLatLng(lat, lon);
        }

        //----------------------------------------------------
        // Zoom
        //----------------------------------------------------
        public void SetZoom(double zoom)
        {
            gMapControl1.Zoom = zoom;
        }

        //----------------------------------------------------
        // 0 = Road
        // 1 = Satellite
        // 2 = Hybrid
        //----------------------------------------------------
        public void SetMapType(int type)
        {
            switch (type)
            {
                case 0:
                    gMapControl1.MapProvider = GoogleMapProvider.Instance;
                    break;

                case 1:
                    gMapControl1.MapProvider = GoogleSatelliteMapProvider.Instance;
                    break;

                case 2:
                    gMapControl1.MapProvider = GoogleHybridMapProvider.Instance;
                    break;
            }

            gMapControl1.ReloadMap();
        }

        //----------------------------------------------------
        // Add Air Quality Marker
        //----------------------------------------------------
        public void AddAirQualityMarker(
            double lat,
            double lon,
            string stationName,
            double pm25,
            double pm10,
            double no2,
            double o3,
            string lastUpdate)
        {
            PointLatLng point = new PointLatLng(lat, lon);

            GMarkerGoogle marker =
                new GMarkerGoogle(point, GMarkerGoogleType.red_dot);

            marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

            marker.ToolTipText = stationName;

            marker.Tag =
                $"Station: {stationName}\n\n" +
                $"PM2.5 : {pm25} µg/m³\n" +
                $"PM10  : {pm10} µg/m³\n" +
                $"NO₂   : {no2} µg/m³\n" +
                $"O₃    : {o3} µg/m³\n\n" +
                $"Updated:\n{lastUpdate}";

            markerOverlay.Markers.Add(marker);

            gMapControl1.Refresh();
        }

        //----------------------------------------------------
        // Remove all markers
        //----------------------------------------------------
        public void ClearMarkers()
        {
            markerOverlay.Markers.Clear();
            gMapControl1.Refresh();
        }
    }
}
