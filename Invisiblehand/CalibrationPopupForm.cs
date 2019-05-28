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
    public partial class CalibrationPopupForm : Form
    {
        public CalibrationPopupForm()
        {
            InitializeComponent();
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
