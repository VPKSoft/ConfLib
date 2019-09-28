namespace ConfLibTest
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.lbFirstName = new System.Windows.Forms.Label();
            this.tbFirstName = new System.Windows.Forms.TextBox();
            this.tbLastName = new System.Windows.Forms.TextBox();
            this.lbLastName = new System.Windows.Forms.Label();
            this.cbNerd = new System.Windows.Forms.CheckBox();
            this.cbCoding = new System.Windows.Forms.CheckBox();
            this.gbGender = new System.Windows.Forms.GroupBox();
            this.rbOther = new System.Windows.Forms.RadioButton();
            this.rbFemale = new System.Windows.Forms.RadioButton();
            this.rbMale = new System.Windows.Forms.RadioButton();
            this.btFavoriteColor = new System.Windows.Forms.Button();
            this.cdFavouriteColor = new System.Windows.Forms.ColorDialog();
            this.gbGender.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbFirstName
            // 
            this.lbFirstName.AutoSize = true;
            this.lbFirstName.Location = new System.Drawing.Point(12, 9);
            this.lbFirstName.Name = "lbFirstName";
            this.lbFirstName.Size = new System.Drawing.Size(58, 13);
            this.lbFirstName.TabIndex = 0;
            this.lbFirstName.Text = "First name:";
            // 
            // tbFirstName
            // 
            this.tbFirstName.Location = new System.Drawing.Point(76, 6);
            this.tbFirstName.Name = "tbFirstName";
            this.tbFirstName.Size = new System.Drawing.Size(298, 20);
            this.tbFirstName.TabIndex = 1;
            // 
            // tbLastName
            // 
            this.tbLastName.Location = new System.Drawing.Point(76, 32);
            this.tbLastName.Name = "tbLastName";
            this.tbLastName.Size = new System.Drawing.Size(298, 20);
            this.tbLastName.TabIndex = 3;
            // 
            // lbLastName
            // 
            this.lbLastName.AutoSize = true;
            this.lbLastName.Location = new System.Drawing.Point(12, 35);
            this.lbLastName.Name = "lbLastName";
            this.lbLastName.Size = new System.Drawing.Size(59, 13);
            this.lbLastName.TabIndex = 2;
            this.lbLastName.Text = "Last name:";
            // 
            // cbNerd
            // 
            this.cbNerd.AutoSize = true;
            this.cbNerd.Location = new System.Drawing.Point(12, 58);
            this.cbNerd.Name = "cbNerd";
            this.cbNerd.Size = new System.Drawing.Size(73, 17);
            this.cbNerd.TabIndex = 4;
            this.cbNerd.Text = "I am nerd!";
            this.cbNerd.UseVisualStyleBackColor = true;
            // 
            // cbCoding
            // 
            this.cbCoding.AutoSize = true;
            this.cbCoding.Location = new System.Drawing.Point(103, 58);
            this.cbCoding.Name = "cbCoding";
            this.cbCoding.Size = new System.Drawing.Size(271, 17);
            this.cbCoding.TabIndex = 5;
            this.cbCoding.Text = "I can code like no one else (meens good/bad/ugly)!";
            this.cbCoding.UseVisualStyleBackColor = true;
            // 
            // gbGender
            // 
            this.gbGender.Controls.Add(this.rbOther);
            this.gbGender.Controls.Add(this.rbFemale);
            this.gbGender.Controls.Add(this.rbMale);
            this.gbGender.Location = new System.Drawing.Point(12, 81);
            this.gbGender.Name = "gbGender";
            this.gbGender.Size = new System.Drawing.Size(362, 50);
            this.gbGender.TabIndex = 6;
            this.gbGender.TabStop = false;
            this.gbGender.Text = "Gender";
            // 
            // rbOther
            // 
            this.rbOther.AutoSize = true;
            this.rbOther.Location = new System.Drawing.Point(263, 19);
            this.rbOther.Name = "rbOther";
            this.rbOther.Size = new System.Drawing.Size(51, 17);
            this.rbOther.TabIndex = 2;
            this.rbOther.TabStop = true;
            this.rbOther.Tag = "2";
            this.rbOther.Text = "Other";
            this.rbOther.UseVisualStyleBackColor = true;
            // 
            // rbFemale
            // 
            this.rbFemale.AutoSize = true;
            this.rbFemale.Location = new System.Drawing.Point(138, 19);
            this.rbFemale.Name = "rbFemale";
            this.rbFemale.Size = new System.Drawing.Size(59, 17);
            this.rbFemale.TabIndex = 1;
            this.rbFemale.TabStop = true;
            this.rbFemale.Tag = "1";
            this.rbFemale.Text = "Female";
            this.rbFemale.UseVisualStyleBackColor = true;
            // 
            // rbMale
            // 
            this.rbMale.AutoSize = true;
            this.rbMale.Location = new System.Drawing.Point(16, 19);
            this.rbMale.Name = "rbMale";
            this.rbMale.Size = new System.Drawing.Size(48, 17);
            this.rbMale.TabIndex = 0;
            this.rbMale.TabStop = true;
            this.rbMale.Tag = "0";
            this.rbMale.Text = "Male";
            this.rbMale.UseVisualStyleBackColor = true;
            // 
            // btFavoriteColor
            // 
            this.btFavoriteColor.Location = new System.Drawing.Point(12, 137);
            this.btFavoriteColor.Name = "btFavoriteColor";
            this.btFavoriteColor.Size = new System.Drawing.Size(362, 23);
            this.btFavoriteColor.TabIndex = 7;
            this.btFavoriteColor.Text = "Favorite Color";
            this.btFavoriteColor.UseVisualStyleBackColor = true;
            this.btFavoriteColor.Click += new System.EventHandler(this.BtFavoriteColor_Click);
            // 
            // cdFavouriteColor
            // 
            this.cdFavouriteColor.AnyColor = true;
            this.cdFavouriteColor.FullOpen = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 172);
            this.Controls.Add(this.btFavoriteColor);
            this.Controls.Add(this.gbGender);
            this.Controls.Add(this.cbCoding);
            this.Controls.Add(this.cbNerd);
            this.Controls.Add(this.tbLastName);
            this.Controls.Add(this.lbLastName);
            this.Controls.Add(this.tbFirstName);
            this.Controls.Add(this.lbFirstName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.gbGender.ResumeLayout(false);
            this.gbGender.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbFirstName;
        private System.Windows.Forms.TextBox tbFirstName;
        private System.Windows.Forms.TextBox tbLastName;
        private System.Windows.Forms.Label lbLastName;
        private System.Windows.Forms.CheckBox cbNerd;
        private System.Windows.Forms.CheckBox cbCoding;
        private System.Windows.Forms.GroupBox gbGender;
        private System.Windows.Forms.RadioButton rbOther;
        private System.Windows.Forms.RadioButton rbFemale;
        private System.Windows.Forms.RadioButton rbMale;
        private System.Windows.Forms.Button btFavoriteColor;
        private System.Windows.Forms.ColorDialog cdFavouriteColor;
    }
}

