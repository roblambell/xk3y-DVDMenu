namespace xk3yDVDMenu
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
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cmbDrive = new System.Windows.Forms.ComboBox();
            this.btScan = new System.Windows.Forms.Button();
            this.btDVDStyle = new System.Windows.Forms.Button();
            this.btCopy = new System.Windows.Forms.Button();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Log = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkArtwork = new System.Windows.Forms.CheckBox();
            this.chkTraillers = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Arial", 8.25F);
            label1.Location = new System.Drawing.Point(5, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(35, 14);
            label1.TabIndex = 5;
            label1.Text = "Drive:";
            // 
            // cmbDrive
            // 
            this.cmbDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrive.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbDrive.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDrive.FormattingEnabled = true;
            this.cmbDrive.Location = new System.Drawing.Point(53, 12);
            this.cmbDrive.Name = "cmbDrive";
            this.cmbDrive.Size = new System.Drawing.Size(179, 22);
            this.cmbDrive.TabIndex = 6;
            this.cmbDrive.DropDown += new System.EventHandler(this.cmbDrive_DropDown);
            // 
            // btScan
            // 
            this.btScan.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btScan.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btScan.Location = new System.Drawing.Point(8, 117);
            this.btScan.Name = "btScan";
            this.btScan.Size = new System.Drawing.Size(109, 37);
            this.btScan.TabIndex = 7;
            this.btScan.Text = "Prepare Games Library";
            this.btScan.UseVisualStyleBackColor = false;
            this.btScan.Click += new System.EventHandler(this.Button1Click);
            // 
            // btDVDStyle
            // 
            this.btDVDStyle.Enabled = false;
            this.btDVDStyle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btDVDStyle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDVDStyle.Location = new System.Drawing.Point(123, 117);
            this.btDVDStyle.Name = "btDVDStyle";
            this.btDVDStyle.Size = new System.Drawing.Size(109, 37);
            this.btDVDStyle.TabIndex = 8;
            this.btDVDStyle.Text = "Generate\r\nDVDMenu";
            this.btDVDStyle.UseVisualStyleBackColor = false;
            this.btDVDStyle.Click += new System.EventHandler(this.Button2Click);
            // 
            // btCopy
            // 
            this.btCopy.Enabled = false;
            this.btCopy.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btCopy.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCopy.Location = new System.Drawing.Point(238, 117);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(109, 37);
            this.btCopy.TabIndex = 9;
            this.btCopy.Text = "Copy DVDMenu\r\nto drive";
            this.btCopy.UseVisualStyleBackColor = false;
            this.btCopy.Click += new System.EventHandler(this.Button3Click);
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbTheme.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(53, 39);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(179, 22);
            this.cmbTheme.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 14);
            this.label3.TabIndex = 15;
            this.label3.Text = "Theme:";
            // 
            // Log
            // 
            this.Log.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Log.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Log.Location = new System.Drawing.Point(8, 189);
            this.Log.Multiline = true;
            this.Log.Name = "Log";
            this.Log.ReadOnly = true;
            this.Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Log.Size = new System.Drawing.Size(339, 145);
            this.Log.TabIndex = 16;
            this.Log.TextChanged += new System.EventHandler(this.Log_TextChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(8, 160);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(339, 23);
            this.progressBar1.TabIndex = 17;
            // 
            // chkArtwork
            // 
            this.chkArtwork.AutoSize = true;
            this.chkArtwork.Checked = true;
            this.chkArtwork.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArtwork.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkArtwork.Location = new System.Drawing.Point(10, 19);
            this.chkArtwork.Name = "chkArtwork";
            this.chkArtwork.Size = new System.Drawing.Size(66, 18);
            this.chkArtwork.TabIndex = 18;
            this.chkArtwork.Text = "Artwork";
            this.chkArtwork.UseVisualStyleBackColor = true;
            // 
            // chkTraillers
            // 
            this.chkTraillers.AutoSize = true;
            this.chkTraillers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTraillers.Location = new System.Drawing.Point(82, 19);
            this.chkTraillers.Name = "chkTraillers";
            this.chkTraillers.Size = new System.Drawing.Size(64, 18);
            this.chkTraillers.TabIndex = 19;
            this.chkTraillers.Text = "Traillers";
            this.chkTraillers.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkArtwork);
            this.groupBox1.Controls.Add(this.chkTraillers);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(8, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 43);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download from Xbox.com";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(25)))), ((int)(((byte)(23)))));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(241, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(5);
            this.pictureBox1.Size = new System.Drawing.Size(106, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 341);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbTheme);
            this.Controls.Add(this.btCopy);
            this.Controls.Add(this.btDVDStyle);
            this.Controls.Add(this.btScan);
            this.Controls.Add(this.cmbDrive);
            this.Controls.Add(label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "xk3y DVDMenu Tool";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.Load += new System.EventHandler(this.Form1Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbDrive;
        private System.Windows.Forms.Button btScan;
        private System.Windows.Forms.Button btDVDStyle;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Log;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkArtwork;
        private System.Windows.Forms.CheckBox chkTraillers;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

