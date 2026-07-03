using System;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GMapViewer
{
    public partial class Form1 : Form
    {
        private GMap.NET.WindowsForms.GMapControl map;

        private ContextMenuStrip menu;
        private ToolStripMenuItem searchNearbyItem;

        private PointLatLng clickedLocation;

        private readonly GMapOverlay searchOverlay =
            new GMapOverlay("Search");

        private const double SearchRadiusMeters = 5000;

        public Form1()
        {
            InitializeComponent();

            InitializeMap();
            InitializeMenu();
        }

        private void InitializeMap()
        {
            map = new GMap.NET.WindowsForms.GMapControl();

            map.Dock = DockStyle.Fill;

            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            map.MapProvider = GMapProviders.GoogleMap;

            map.Position = new PointLatLng(65.0121, 25.4651);

            map.MinZoom = 2;
            map.MaxZoom = 20;
            map.Zoom = 11;

            map.DragButton = MouseButtons.Left;

            map.Overlays.Add(searchOverlay);

            map.MouseDown += Map_MouseDown;

            Controls.Add(map);
        }

        private void InitializeMenu()
        {
            menu = new ContextMenuStrip();

            searchNearbyItem =
                new ToolStripMenuItem("Search nearby stations");

            searchNearbyItem.Click += SearchNearbyItem_Click;

            menu.Items.Add(searchNearbyItem);
        }

        private void Map_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            clickedLocation = map.FromLocalToLatLng(e.X, e.Y);

            menu.Show(map, e.Location);
        }

        private void SearchNearbyItem_Click(object sender, EventArgs e)
        {
            DrawSearchArea(clickedLocation);

            MessageBox.Show(
                $"Latitude : {clickedLocation.Lat:F6}\n" +
                $"Longitude: {clickedLocation.Lng:F6}");
        }

        private void DrawSearchArea(PointLatLng center)
        {
            searchOverlay.Markers.Clear();
            searchOverlay.Polygons.Clear();

            var marker = new GMarkerGoogle(
                center,
                GMarkerGoogleType.red_dot);

            searchOverlay.Markers.Add(marker);

            var circle =
                new GMapPolygon(
                    GeoCircle.CreateCircle(
                        center,
                        SearchRadiusMeters),
                    "SearchRadius");

            circle.Stroke = new Pen(Color.Blue, 2);

            circle.Fill =
                new SolidBrush(
                    Color.FromArgb(
                        40,
                        Color.Blue));

            searchOverlay.Polygons.Add(circle);

            map.Refresh();
        }
    }
}