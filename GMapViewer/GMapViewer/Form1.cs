using System.Windows.Forms;
using GMap.NET.WindowsForms;
using GMapControl;

namespace GMapViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MapControl map = new MapControl();
            map.Dock = DockStyle.Fill;

            this.Controls.Add(map);
        }
    }
}
