using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CITRUS
{
    public partial class CIT_00_1_FillingParameterLevelProgressBarForm : Form
    {
        
        public CIT_00_1_FillingParameterLevelProgressBarForm(int max)
        {
            InitializeComponent();
            progressBar_pb.Minimum = 0;
            progressBar_pb.Maximum = max;
            progressBar_pb.Step = 1;
            label_LevelName.Text = "";
        }
    }
}
