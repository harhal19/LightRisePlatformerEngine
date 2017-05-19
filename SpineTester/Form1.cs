using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spine;
using LightRise.WinUtilsLib;

namespace SpineTester
{
    public partial class Form1 : Form
    {
        Action<string> LoadSpine;
        Action<string, bool> PlayAnimation;
        Action<float> SetScale;
        Func<string[]> GetAnimationArray;
        float scale = 1f;

        public Form1(Action<string> LoadSpine, Action<string, bool> PlayAnimation, Action<float> SetScale, Func<string[]> GetAnimationArray, SpineObject spineObj)
        {
            this.LoadSpine = LoadSpine;
            this.PlayAnimation = PlayAnimation;
            this.SetScale = SetScale;
            this.GetAnimationArray = GetAnimationArray;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Json|*.json";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string file = dialog.FileName;
                    FileName.Text = file;
                    LoadSpine(file);
                    AnimationName.Items.Clear();
                    AnimationName.Items.AddRange(GetAnimationArray());
                    AnimationName.Enabled = true;
                    ScaleMinus.Enabled = true;
                    ScalePlus.Enabled = true;
                    Loop.Enabled = true;
                }
                catch (Exception ex)
                {
                    FileName.Text = ex.Message;
                }
            }
        }

        private void AnimationName_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlayButton.Enabled = true;
        }

        private void ShowButton_Click(object sender, EventArgs e)
        {
            PlayAnimation(AnimationName.SelectedItem as string, Loop.Checked);
        }

        private void ScaleMinus_Click(object sender, EventArgs e)
        {
            scale *= 0.9f;
            Scale.Text = scale.ToString();
            SetScale(scale);
        }

        private void ScalePlus_Click(object sender, EventArgs e)
        {
            scale *= 1.1f;
            Scale.Text = scale.ToString();
            SetScale(scale);
        }
    }
}
