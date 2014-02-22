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
            this.comboBoxDriveList = new System.Windows.Forms.ComboBox();
            this.buttonPrepareXML = new System.Windows.Forms.Button();
            this.buttonGenerateDVDMenu = new System.Windows.Forms.Button();
            this.buttonCopyToDrive = new System.Windows.Forms.Button();
            this.comboBoxThemeList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Log = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkArtwork = new System.Windows.Forms.CheckBox();
            this.chkTraillers = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
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
            // comboBoxDriveList
            // 
            this.comboBoxDriveList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDriveList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxDriveList.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxDriveList.FormattingEnabled = true;
            this.comboBoxDriveList.Location = new System.Drawing.Point(53, 12);
            this.comboBoxDriveList.Name = "comboBoxDriveList";
            this.comboBoxDriveList.Size = new System.Drawing.Size(179, 22);
            this.comboBoxDriveList.TabIndex = 0;
            this.comboBoxDriveList.DropDown += new System.EventHandler(this.comboBoxDriveList_DropDown);
            // 
            // buttonPrepareXML
            // 
            this.buttonPrepareXML.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonPrepareXML.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPrepareXML.Location = new System.Drawing.Point(8, 117);
            this.buttonPrepareXML.Name = "buttonPrepareXML";
            this.buttonPrepareXML.Size = new System.Drawing.Size(109, 37);
            this.buttonPrepareXML.TabIndex = 25;
            this.buttonPrepareXML.Text = "Prepare Games Library && Project";
            this.buttonPrepareXML.UseVisualStyleBackColor = false;
            this.buttonPrepareXML.Click += new System.EventHandler(this.buttonPrepareXML_Click);
            // 
            // buttonGenerateDVDMenu
            // 
            this.buttonGenerateDVDMenu.Enabled = false;
            this.buttonGenerateDVDMenu.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonGenerateDVDMenu.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGenerateDVDMenu.Location = new System.Drawing.Point(123, 117);
            this.buttonGenerateDVDMenu.Name = "buttonGenerateDVDMenu";
            this.buttonGenerateDVDMenu.Size = new System.Drawing.Size(109, 37);
            this.buttonGenerateDVDMenu.TabIndex = 30;
            this.buttonGenerateDVDMenu.Text = "Generate\r\nDVDMenu";
            this.buttonGenerateDVDMenu.UseVisualStyleBackColor = false;
            this.buttonGenerateDVDMenu.Click += new System.EventHandler(this.buttonGenerateDVDMenu_Click);
            // 
            // buttonCopyToDrive
            // 
            this.buttonCopyToDrive.Enabled = false;
            this.buttonCopyToDrive.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCopyToDrive.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCopyToDrive.Location = new System.Drawing.Point(238, 117);
            this.buttonCopyToDrive.Name = "buttonCopyToDrive";
            this.buttonCopyToDrive.Size = new System.Drawing.Size(109, 37);
            this.buttonCopyToDrive.TabIndex = 35;
            this.buttonCopyToDrive.Text = "Copy DVDMenu\r\nto drive";
            this.buttonCopyToDrive.UseVisualStyleBackColor = false;
            this.buttonCopyToDrive.Click += new System.EventHandler(this.buttonCopyToDrive_Click);
            // 
            // comboBoxThemeList
            // 
            this.comboBoxThemeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxThemeList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxThemeList.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxThemeList.FormattingEnabled = true;
            this.comboBoxThemeList.Location = new System.Drawing.Point(53, 39);
            this.comboBoxThemeList.Name = "comboBoxThemeList";
            this.comboBoxThemeList.Size = new System.Drawing.Size(179, 22);
            this.comboBoxThemeList.TabIndex = 5;
            this.comboBoxThemeList.DropDown += new System.EventHandler(this.comboBoxThemeList_DropDown);
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
            this.Log.TabIndex = 40;
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
            this.chkArtwork.TabIndex = 15;
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
            this.chkTraillers.TabIndex = 20;
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
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download from Xbox.com";
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(25)))), ((int)(((byte)(23)))));
            this.pictureBoxLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(241, 12);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Padding = new System.Windows.Forms.Padding(5);
            this.pictureBoxLogo.Size = new System.Drawing.Size(106, 50);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 21;
            this.pictureBoxLogo.TabStop = false;
            this.pictureBoxLogo.Click += new System.EventHandler(this.pictureBoxLogo_Click);
            this.pictureBoxLogo.MouseHover += new System.EventHandler(this.pictureBoxLogo_MouseHover);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 341);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxThemeList);
            this.Controls.Add(this.buttonCopyToDrive);
            this.Controls.Add(this.buttonGenerateDVDMenu);
            this.Controls.Add(this.buttonPrepareXML);
            this.Controls.Add(this.comboBoxDriveList);
            this.Controls.Add(label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xk3y DVDMenu Tool";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.Load += new System.EventHandler(this.Form1Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDriveList;
        private System.Windows.Forms.Button buttonPrepareXML;
        private System.Windows.Forms.Button buttonGenerateDVDMenu;
        private System.Windows.Forms.Button buttonCopyToDrive;
        private System.Windows.Forms.ComboBox comboBoxThemeList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Log;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkArtwork;
        private System.Windows.Forms.CheckBox chkTraillers;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
    }
}

