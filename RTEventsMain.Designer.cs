namespace RTEvents
{
    partial class RTEventsMain
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
            this.components = new System.ComponentModel.Container();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lbRTShow = new System.Windows.Forms.ListBox();
            this.rtTimer = new System.Windows.Forms.Timer(this.components);
            this.timerRestart = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbConnected = new System.Windows.Forms.ListBox();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.lbRTShow);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(476, 215);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Device list";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(389, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbRTShow
            // 
            this.lbRTShow.ColumnWidth = 100;
            this.lbRTShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRTShow.FormattingEnabled = true;
            this.lbRTShow.ItemHeight = 16;
            this.lbRTShow.Location = new System.Drawing.Point(3, 18);
            this.lbRTShow.MultiColumn = true;
            this.lbRTShow.Name = "lbRTShow";
            this.lbRTShow.Size = new System.Drawing.Size(470, 194);
            this.lbRTShow.TabIndex = 4;
            // 
            // rtTimer
            // 
            this.rtTimer.Enabled = true;
            this.rtTimer.Interval = 1000;
            this.rtTimer.Tick += new System.EventHandler(this.rtTimer_Tick);
            // 
            // timerRestart
            // 
            this.timerRestart.Enabled = true;
            this.timerRestart.Interval = 120000;
            this.timerRestart.Tick += new System.EventHandler(this.timerRestart_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbConnected);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 215);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 296);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connected";
            // 
            // lbConnected
            // 
            this.lbConnected.ColumnWidth = 100;
            this.lbConnected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbConnected.FormattingEnabled = true;
            this.lbConnected.ItemHeight = 16;
            this.lbConnected.Location = new System.Drawing.Point(3, 18);
            this.lbConnected.MultiColumn = true;
            this.lbConnected.Name = "lbConnected";
            this.lbConnected.Size = new System.Drawing.Size(470, 275);
            this.lbConnected.TabIndex = 4;
            // 
            // RTEventsMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(476, 511);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RTEventsMain";
            this.Text = "PROPASS Logger";
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Timer rtTimer;
        private System.Windows.Forms.ListBox lbRTShow;
        private System.Windows.Forms.Timer timerRestart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbConnected;
        private System.Windows.Forms.Button button1;
    }
}

