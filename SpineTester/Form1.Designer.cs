namespace SpineTester
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.AnimationName = new System.Windows.Forms.ComboBox();
            this.PlayButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.FileName = new System.Windows.Forms.TextBox();
            this.Loop = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ScaleMinus = new System.Windows.Forms.Button();
            this.ScalePlus = new System.Windows.Forms.Button();
            this.Scale = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AnimationName
            // 
            resources.ApplyResources(this.AnimationName, "AnimationName");
            this.AnimationName.FormattingEnabled = true;
            this.AnimationName.Name = "AnimationName";
            this.AnimationName.SelectedIndexChanged += new System.EventHandler(this.AnimationName_SelectedIndexChanged);
            // 
            // PlayButton
            // 
            resources.ApplyResources(this.PlayButton, "PlayButton");
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.ShowButton_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FileName
            // 
            resources.ApplyResources(this.FileName, "FileName");
            this.FileName.Name = "FileName";
            // 
            // Loop
            // 
            resources.ApplyResources(this.Loop, "Loop");
            this.Loop.Name = "Loop";
            this.Loop.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // ScaleMinus
            // 
            resources.ApplyResources(this.ScaleMinus, "ScaleMinus");
            this.ScaleMinus.Name = "ScaleMinus";
            this.ScaleMinus.UseVisualStyleBackColor = true;
            this.ScaleMinus.Click += new System.EventHandler(this.ScaleMinus_Click);
            // 
            // ScalePlus
            // 
            resources.ApplyResources(this.ScalePlus, "ScalePlus");
            this.ScalePlus.Name = "ScalePlus";
            this.ScalePlus.UseVisualStyleBackColor = true;
            this.ScalePlus.Click += new System.EventHandler(this.ScalePlus_Click);
            // 
            // Scale
            // 
            resources.ApplyResources(this.Scale, "Scale");
            this.Scale.Name = "Scale";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Scale);
            this.Controls.Add(this.ScalePlus);
            this.Controls.Add(this.ScaleMinus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Loop);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AnimationName);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FileName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox AnimationName;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FileName;
        private System.Windows.Forms.CheckBox Loop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ScaleMinus;
        private System.Windows.Forms.Button ScalePlus;
        private System.Windows.Forms.Label Scale;
    }
}