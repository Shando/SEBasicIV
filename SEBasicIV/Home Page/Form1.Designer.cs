using System.Windows.Forms;
using MiniTimer_Theme;

namespace SEBasicIV
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.miniTimer_Label2 = new MiniTimer_Theme.MiniTimer_Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.miniTimer_Label4 = new MiniTimer_Theme.MiniTimer_Label();
            this.miniTimer_Label1 = new MiniTimer_Theme.MiniTimer_Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.close = new System.Windows.Forms.PictureBox();
            this.max = new System.Windows.Forms.PictureBox();
            this.min = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.miniTimer_LinkLabel2 = new MiniTimer_Theme.MiniTimer_LinkLabel();
            this.miniTimer_Label5 = new MiniTimer_Theme.MiniTimer_Label();
            this.userControl11 = new SEBasicIV.UserControl1();
            this.miniTimer_Label3 = new MiniTimer_Theme.MiniTimer_Label();
            this.status = new MiniTimer_Theme.MiniTimer_Label();
            this.miniTimer_LinkLabel1 = new MiniTimer_Theme.MiniTimer_LinkLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.close)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.miniTimer_Label2);
            this.panel1.Controls.Add(this.pictureBox5);
            this.panel1.Controls.Add(this.miniTimer_Label4);
            this.panel1.Controls.Add(this.miniTimer_Label1);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.pictureBox4);
            this.panel1.Location = new System.Drawing.Point(33, 78);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1076, 187);
            this.panel1.TabIndex = 24;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // miniTimer_Label2
            // 
            this.miniTimer_Label2.AutoSize = true;
            this.miniTimer_Label2.BackColor = System.Drawing.Color.Transparent;
            this.miniTimer_Label2.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.miniTimer_Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.miniTimer_Label2.Location = new System.Drawing.Point(454, 159);
            this.miniTimer_Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.miniTimer_Label2.Name = "miniTimer_Label2";
            this.miniTimer_Label2.Size = new System.Drawing.Size(170, 21);
            this.miniTimer_Label2.TabIndex = 32;
            this.miniTimer_Label2.Text = "Z80 Assembler Editor";
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = global::SEBasicIV.Properties.Resources.tile_frame;
            this.pictureBox5.Image = global::SEBasicIV.Properties.Resources.writenew;
            this.pictureBox5.ImageLocation = "";
            this.pictureBox5.Location = new System.Drawing.Point(430, 31);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(216, 124);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox5.TabIndex = 31;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Click += new System.EventHandler(this.pictureBox5_Click_1);
            // 
            // miniTimer_Label4
            // 
            this.miniTimer_Label4.AutoSize = true;
            this.miniTimer_Label4.BackColor = System.Drawing.Color.Transparent;
            this.miniTimer_Label4.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.miniTimer_Label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.miniTimer_Label4.Location = new System.Drawing.Point(795, 156);
            this.miniTimer_Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.miniTimer_Label4.Name = "miniTimer_Label4";
            this.miniTimer_Label4.Size = new System.Drawing.Size(219, 21);
            this.miniTimer_Label4.TabIndex = 30;
            this.miniTimer_Label4.Text = "SE BASIC IV Documentation";
            this.miniTimer_Label4.Click += new System.EventHandler(this.miniTimer_Label4_Click);
            // 
            // miniTimer_Label1
            // 
            this.miniTimer_Label1.AutoSize = true;
            this.miniTimer_Label1.BackColor = System.Drawing.Color.Transparent;
            this.miniTimer_Label1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.miniTimer_Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.miniTimer_Label1.Location = new System.Drawing.Point(120, 156);
            this.miniTimer_Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.miniTimer_Label1.Name = "miniTimer_Label1";
            this.miniTimer_Label1.Size = new System.Drawing.Size(105, 21);
            this.miniTimer_Label1.TabIndex = 27;
            this.miniTimer_Label1.Text = "BASIC Editor";
            this.miniTimer_Label1.Click += new System.EventHandler(this.miniTimer_Label1_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImage = global::SEBasicIV.Properties.Resources.tile_frame;
            this.pictureBox3.Image = global::SEBasicIV.Properties.Resources.writenew;
            this.pictureBox3.ImageLocation = "";
            this.pictureBox3.Location = new System.Drawing.Point(65, 28);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(216, 124);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            this.pictureBox3.MouseLeave += new System.EventHandler(this.pictureBox3_MouseLeave);
            this.pictureBox3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseMove);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImage = global::SEBasicIV.Properties.Resources.tile_frame;
            this.pictureBox4.Image = global::SEBasicIV.Properties.Resources.open;
            this.pictureBox4.ImageLocation = "";
            this.pictureBox4.Location = new System.Drawing.Point(796, 28);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(216, 124);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox4.TabIndex = 26;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            this.pictureBox4.MouseLeave += new System.EventHandler(this.pictureBox4_MouseLeave);
            this.pictureBox4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox4_MouseMove);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.pictureBox1.Location = new System.Drawing.Point(-1, 64);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1160, 1);
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.close.Image = global::SEBasicIV.Properties.Resources.CtrlBtn;
            this.close.Location = new System.Drawing.Point(1117, 28);
            this.close.Margin = new System.Windows.Forms.Padding(4);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(19, 17);
            this.close.TabIndex = 21;
            this.close.TabStop = false;
            this.close.Click += new System.EventHandler(this.close_Click);
            this.close.MouseLeave += new System.EventHandler(this.close_MouseLeave);
            this.close.MouseMove += new System.Windows.Forms.MouseEventHandler(this.close_MouseMove);
            // 
            // max
            // 
            this.max.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.max.Image = global::SEBasicIV.Properties.Resources.CtrlBtn;
            this.max.Location = new System.Drawing.Point(1091, 28);
            this.max.Margin = new System.Windows.Forms.Padding(4);
            this.max.Name = "max";
            this.max.Size = new System.Drawing.Size(19, 17);
            this.max.TabIndex = 20;
            this.max.TabStop = false;
            this.max.Click += new System.EventHandler(this.max_Click);
            this.max.MouseLeave += new System.EventHandler(this.max_MouseLeave);
            this.max.MouseMove += new System.Windows.Forms.MouseEventHandler(this.max_MouseMove);
            // 
            // min
            // 
            this.min.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.min.Image = global::SEBasicIV.Properties.Resources.CtrlBtn;
            this.min.Location = new System.Drawing.Point(1063, 28);
            this.min.Margin = new System.Windows.Forms.Padding(4);
            this.min.Name = "min";
            this.min.Size = new System.Drawing.Size(19, 17);
            this.min.TabIndex = 19;
            this.min.TabStop = false;
            this.min.Click += new System.EventHandler(this.min_Click);
            this.min.MouseLeave += new System.EventHandler(this.min_MouseLeave);
            this.min.MouseMove += new System.Windows.Forms.MouseEventHandler(this.min_MouseMove);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = global::SEBasicIV.Properties.Resources.footer;
            this.pictureBox2.Location = new System.Drawing.Point(4, 622);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1148, 53);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 23;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox9.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox9.Image = global::SEBasicIV.Properties.Resources.info;
            this.pictureBox9.Location = new System.Drawing.Point(1089, 635);
            this.pictureBox9.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(27, 25);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox9.TabIndex = 63;
            this.pictureBox9.TabStop = false;
            this.pictureBox9.Click += new System.EventHandler(this.pictureBox9_Click);
            this.pictureBox9.MouseLeave += new System.EventHandler(this.pictureBox9_MouseLeave);
            this.pictureBox9.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox9_MouseMove);
            // 
            // miniTimer_LinkLabel2
            // 
            this.miniTimer_LinkLabel2.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(134)))), ((int)(((byte)(158)))));
            this.miniTimer_LinkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.miniTimer_LinkLabel2.AutoSize = true;
            this.miniTimer_LinkLabel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.miniTimer_LinkLabel2.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.miniTimer_LinkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.miniTimer_LinkLabel2.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(151)))), ((int)(((byte)(172)))));
            this.miniTimer_LinkLabel2.Location = new System.Drawing.Point(689, 639);
            this.miniTimer_LinkLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.miniTimer_LinkLabel2.Name = "miniTimer_LinkLabel2";
            this.miniTimer_LinkLabel2.Size = new System.Drawing.Size(356, 21);
            this.miniTimer_LinkLabel2.TabIndex = 65;
            this.miniTimer_LinkLabel2.TabStop = true;
            this.miniTimer_LinkLabel2.Text = "https://github.com/Shando/SEBasicIV";
            this.miniTimer_LinkLabel2.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(151)))), ((int)(((byte)(172)))));
            this.miniTimer_LinkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.miniTimer_LinkLabel2_LinkClicked);
            // 
            // miniTimer_Label5
            // 
            this.miniTimer_Label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.miniTimer_Label5.AutoSize = true;
            this.miniTimer_Label5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.miniTimer_Label5.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.miniTimer_Label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.miniTimer_Label5.Location = new System.Drawing.Point(348, 639);
            this.miniTimer_Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.miniTimer_Label5.Name = "miniTimer_Label5";
            this.miniTimer_Label5.Size = new System.Drawing.Size(326, 21);
            this.miniTimer_Label5.TabIndex = 64;
            this.miniTimer_Label5.Text = "In case of any bugs, please contact me at:";
            this.miniTimer_Label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.miniTimer_Label5.Click += new System.EventHandler(this.miniTimer_Label5_Click);
            // 
            // userControl11
            // 
            this.userControl11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userControl11.AutoScroll = true;
            this.userControl11.Location = new System.Drawing.Point(35, 295);
            this.userControl11.Margin = new System.Windows.Forms.Padding(5);
            this.userControl11.Name = "userControl11";
            this.userControl11.Size = new System.Drawing.Size(1077, 306);
            this.userControl11.TabIndex = 62;
            this.userControl11.Load += new System.EventHandler(this.userControl11_Load);
            // 
            // miniTimer_Label3
            // 
            this.miniTimer_Label3.BackColor = System.Drawing.Color.Transparent;
            this.miniTimer_Label3.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.miniTimer_Label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.miniTimer_Label3.Location = new System.Drawing.Point(0, 0);
            this.miniTimer_Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.miniTimer_Label3.Name = "miniTimer_Label3";
            this.miniTimer_Label3.Size = new System.Drawing.Size(133, 28);
            this.miniTimer_Label3.TabIndex = 0;
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.status.AutoSize = true;
            this.status.BackColor = System.Drawing.Color.WhiteSmoke;
            this.status.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.status.Location = new System.Drawing.Point(29, 639);
            this.status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(141, 21);
            this.status.TabIndex = 22;
            this.status.Text = "miniTimer_Label1";
            // 
            // miniTimer_LinkLabel1
            // 
            this.miniTimer_LinkLabel1.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(134)))), ((int)(((byte)(158)))));
            this.miniTimer_LinkLabel1.AutoSize = true;
            this.miniTimer_LinkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.miniTimer_LinkLabel1.Font = new System.Drawing.Font("Tahoma", 9.75F,System.Drawing.FontStyle.Bold);
            this.miniTimer_LinkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.miniTimer_LinkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(151)))), ((int)(((byte)(172)))));
            this.miniTimer_LinkLabel1.Location = new System.Drawing.Point(31, 26);
            this.miniTimer_LinkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.miniTimer_LinkLabel1.Name = "miniTimer_LinkLabel1";
            this.miniTimer_LinkLabel1.Size = new System.Drawing.Size(243, 21);
            this.miniTimer_LinkLabel1.TabIndex = 0;
            this.miniTimer_LinkLabel1.TabStop = true;
            this.miniTimer_LinkLabel1.Text = "SE BASIC IV 4.2 Cordelia Editor";
            this.miniTimer_LinkLabel1.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(151)))), ((int)(((byte)(172)))));
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1156, 678);
            this.Controls.Add(this.miniTimer_LinkLabel2);
            this.Controls.Add(this.miniTimer_Label5);
            this.Controls.Add(this.pictureBox9);
            this.Controls.Add(this.userControl11);
            this.Controls.Add(this.miniTimer_Label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.status);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.miniTimer_LinkLabel1);
            this.Controls.Add(this.close);
            this.Controls.Add(this.max);
            this.Controls.Add(this.min);
            this.Controls.Add(this.pictureBox2);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1101, 605);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SEBasicIV";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.close)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MiniTimer_LinkLabel miniTimer_LinkLabel1;
        private PictureBox pictureBox1;
        private PictureBox min;
        private PictureBox max;
        private PictureBox close;
        private MiniTimer_Label status;
        private PictureBox pictureBox2;
        private Panel panel1;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private MiniTimer_Label miniTimer_Label1;
        private MiniTimer_Label miniTimer_Label3;
        private MiniTimer_Label miniTimer_Label4;
        private UserControl1 userControl11;
        private PictureBox pictureBox9;
        private MiniTimer_Label miniTimer_Label5;
        private MiniTimer_LinkLabel miniTimer_LinkLabel2;
        private MiniTimer_Label miniTimer_Label2;
        private PictureBox pictureBox5;
    }
}