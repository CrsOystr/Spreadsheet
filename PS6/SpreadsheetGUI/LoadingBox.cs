using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class LoadingBox : Form
    {
        SpreadsheetGUIForm parent;

        public LoadingBox(SpreadsheetGUIForm parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void LoadingBox_Load(object sender, EventArgs e)
        {

        }


    }
}
