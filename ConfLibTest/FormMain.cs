#region License
/*
ConfLib

A library for storing application setting into a SQLite database.
Copyright (C) 2019 VPKSoft, Petteri Kautonen

Contact: vpksoft@vpksoft.net

This file is part of ConfLib.

ConfLib is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ConfLib is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with ConfLib.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using VPKSoft.ConfLib;

namespace ConfLibTest
{
    public partial class FormMain : Form
    {
        private readonly Settings settings;

        public FormMain()
        {
            InitializeComponent();
            var cl = new Conflib();

            settings = new Settings(cl);

            settings.RequestTypeConverter += Settings_RequestTypeConverter;

            settings.LoadSettings();


            // ReSharper disable once VirtualMemberCallInConstructor
            Text = settings.Text + @" " + settings.Year;

            tbFirstName.Text = settings.FirstName;
            tbLastName.Text = settings.LastName;

            cbCoding.Checked = settings.MasterCoder;
            cbNerd.Checked = settings.UserIsANerd;

            if (settings.GenderIndex != -1)
            {                
                foreach (Control c in gbGender.Controls)
                {
                    if ((c is RadioButton radioButton) && (string)radioButton.Tag == settings.GenderIndex.ToString())
                    {
                        radioButton.Checked = true;
                        break;
                    }
                }
            }

            btFavoriteColor.BackColor = settings.FavoriteColor;
        }

        private void Settings_RequestTypeConverter(object sender, TypeConverterEventArgs e)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Color));
            e.Converter = converter;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            using (settings)
            {
                settings.FirstName = tbFirstName.Text;
                settings.LastName = tbLastName.Text;
                settings.MasterCoder = cbCoding.Checked;
                settings.UserIsANerd = cbNerd.Checked;
                foreach (Control c in gbGender.Controls)
                {
                    if ((c is RadioButton radioButton) && radioButton.Checked)
                    {
                        settings.GenderIndex = Convert.ToInt32(radioButton.Tag);
                        break;
                    }
                }

                settings.FavoriteColor = btFavoriteColor.BackColor;
            }
        }

        private void BtFavoriteColor_Click(object sender, EventArgs e)
        {
            cdFavouriteColor.Color = btFavoriteColor.BackColor;
            if (cdFavouriteColor.ShowDialog() == DialogResult.OK)
            {
                btFavoriteColor.BackColor = cdFavouriteColor.Color;
            }
        }
    }
}
