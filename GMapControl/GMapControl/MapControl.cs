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
        public enum MapAction
        {
            None = 0,

            SearchNearby = 1,

            StationInfo = 2,

            RefreshStation = 3,

            RemoveStation = 4
        }
        private MapAction lastAction = MapAction.None;

        private double searchRadiusMeters = 5000;
        private bool markerWasClicked = false;

        private readonly GMapOverlay markerOverlay = new GMapOverlay("Markers");
        private readonly GMapOverlay searchOverlay = new GMapOverlay("Search");
        // Context menu
        private ContextMenuStrip mapMenu = new ContextMenuStrip();
        private ToolStripMenuItem searchNearbyItem =
            new ToolStripMenuItem("Search nearby stations");

        // Marker menu
        private ContextMenuStrip markerMenu = new ContextMenuStrip();

        private ToolStripMenuItem infoItem =
            new ToolStripMenuItem("Station information");

        private ToolStripMenuItem refreshItem =
            new ToolStripMenuItem("Refresh station");

        private ToolStripMenuItem removeItem =
            new ToolStripMenuItem("Remove station");

        private GMapMarker selectedMarker = null;

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

            gMapControl1.Overlays.Add(searchOverlay);
            gMapControl1.Overlays.Add(markerOverlay);
            gMapControl1.MouseUp += GMapControl1_MouseUp;
            gMapControl1.MouseDown += GMapControl1_MouseDown;

            gMapControl1.OnMarkerClick += GMapControl1_OnMarkerClick;
            searchNearbyItem.Click += SearchNearbyItem_Click;
            mapMenu.Items.Add(searchNearbyItem);
            // Marker context menu
            infoItem.Click += InfoItem_Click;
            refreshItem.Click += RefreshItem_Click;
            removeItem.Click += RemoveItem_Click;

            markerMenu.Items.Add(infoItem);
            markerMenu.Items.Add(refreshItem);
            markerMenu.Items.Add(removeItem);
        }

        //----------------------------------------------------
        // User clicked somewhere on the map
        //----------------------------------------------------
        private void GMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            clickedLat = point.Lat;
            clickedLng = point.Lng;
        }
        private void GMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (markerWasClicked)
                {
                    markerWasClicked = false;
                    return;
                }

                mapMenu.Show(gMapControl1, e.Location);
            }
        }
        private void SearchNearbyItem_Click(object sender, EventArgs e)
        {
            lastAction = MapAction.SearchNearby;
        }

        private void InfoItem_Click(object sender, EventArgs e)
        {
            lastAction = MapAction.StationInfo;
        }

        private void RefreshItem_Click(object sender, EventArgs e)
        {
            lastAction = MapAction.RefreshStation;
        }

        private void RemoveItem_Click(object sender, EventArgs e)
        {
            lastAction = MapAction.RemoveStation;
        }

        //----------------------------------------------------
        // Right-click marker -> remove it
        //----------------------------------------------------
        private void GMapControl1_OnMarkerClick(GMapMarker marker, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            markerWasClicked = true;
            selectedMarker = marker;

            markerMenu.Show(gMapControl1, e.Location);
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
        public int GetLastAction()
        {
            int action = (int)lastAction;
            lastAction = MapAction.None;
            return action;
        }
        public string GetSelectedStationId()
        {
            if (selectedMarker == null)
                return "";

            StationInfo info = (StationInfo)selectedMarker.Tag;

            return info.StationId;
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
        public void RemoveMarker(string stationId)
        {
            GMapMarker markerToRemove = null;

            foreach (GMapMarker marker in markerOverlay.Markers)
            {
                StationInfo info = marker.Tag as StationInfo;

                if (info != null && info.StationId == stationId)
                {
                    markerToRemove = marker;
                    break;
                }
            }

            if (markerToRemove != null)
            {
                markerOverlay.Markers.Remove(markerToRemove);
                gMapControl1.Refresh();
            }
        }


        //----------------------------------------------------
        // Clear search graphics (LabVIEW calls this)
        //----------------------------------------------------
        public void ClearSearchArea()
        {
            searchOverlay.Markers.Clear();
            searchOverlay.Polygons.Clear();

            gMapControl1.Refresh();
        }
        //----------------------------------------------------
        // Draw Search Area
        //----------------------------------------------------
        public void DrawSearchArea(double radiusMeters)
        {
            // Store the radius
            searchRadiusMeters = radiusMeters;

            // Remove previous search graphics
            searchOverlay.Markers.Clear();
            searchOverlay.Polygons.Clear();

            // Search center
            PointLatLng center = new PointLatLng(clickedLat, clickedLng);

            // Draw center marker
            GMarkerGoogle centerMarker =
                new GMarkerGoogle(center, GMarkerGoogleType.blue_dot);

            searchOverlay.Markers.Add(centerMarker);

            // Draw search circle
            GMapPolygon circle =
                new GMapPolygon(
                    GeoCircle.CreateCircle(center, searchRadiusMeters),
                    "SearchArea");

            circle.Stroke = new System.Drawing.Pen(
                System.Drawing.Color.Blue, 2);

            circle.Fill =
                new System.Drawing.SolidBrush(
                    System.Drawing.Color.FromArgb(40, System.Drawing.Color.Blue));

            searchOverlay.Polygons.Add(circle);

            gMapControl1.Refresh();
        }
        //----------------------------------------------------
        // Add Air Quality Marker
        //----------------------------------------------------
        public void AddAirQualityMarker(
            string stationId,
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

            marker.Tag = new StationInfo
            {
                StationId = stationId,
                StationName = stationName,
                PM25 = pm25,
                PM10 = pm10,
                NO2 = no2,
                O3 = o3,
                LastUpdate = lastUpdate
            };

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
