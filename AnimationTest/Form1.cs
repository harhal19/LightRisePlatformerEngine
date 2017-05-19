using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnimationTest
{
    public partial class Form1 : Form
    {
        public static MainThread MainThread { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileName.Text = dialog.FileName;
                ShowButton.Enabled = true;
            }
        }

        [STAThread]
        private void ShowButton_Click(object sender, EventArgs e)
        {
            using (MainThread = new MainThread())
                MainThread.Run();
        }
    }
}
