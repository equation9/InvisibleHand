using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invisiblehand
{
    public partial class Invisible_hand : Form
    {
        public Invisible_hand()
        {
            InitializeComponent();
        }

      

        private void rectangleShape1_Click(object sender, EventArgs e)
        {
            CalibrationForm dlg = new CalibrationForm();
            dlg.ShowDialog();
        }

        private void rectangleShape2_Click_1(object sender, EventArgs e)
        {
            VideoCaptureControl dlg = new VideoCaptureControl();
            dlg.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            CalibrationForm dlg = new CalibrationForm();
            dlg.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            VideoCaptureControl dlg = new VideoCaptureControl();
            dlg.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            CalibrationForm dlg = new CalibrationForm();
            dlg.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            VideoCaptureControl dlg = new VideoCaptureControl();
            dlg.ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
