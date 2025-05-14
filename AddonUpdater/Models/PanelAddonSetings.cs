using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddonUpdater.Models
{
    public class PanelAddonSetings
    {
        public int Id { get; set; }
        public Label Name { get; set; } = null!;
        public Label Author { get; set; } = null!;
        public Label Category { get; set; } = null!;
        public Button Version { get; set; } = null!;
        public Button Delete { get; set; } = null!;
        public ProgressBar ProgressBar { get; set; } = null!;
        public PictureBox Track { get; set; } = null!;
        public int Width { get; set; }
    }
}
