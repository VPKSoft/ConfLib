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
    // just a form..
    public partial class FormMain : Form
    {
        // the settings..
        private readonly Settings settings;

        // the constructor..
        public FormMain()
        {
            InitializeComponent();

            // create the settings class..
            settings = new Settings(new Conflib()); // the Conflib will create a folder and a SQLite database automatically to %LOCALAPPDATA%\YourApp

            // subscribe the event to allow the Settings class to request a TypeConverter for more complex types..
            settings.RequestTypeConverter += Settings_RequestTypeConverter;

            // load the settings..
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

        // get a type converter "from-to-from" string conversion for more complex types..
        private void Settings_RequestTypeConverter(object sender, TypeConverterEventArgs e)
        {
            // just a color so we can assume that a type converter will be found..
            var converter = TypeDescriptor.GetConverter(e.TypeOfConverter);
            e.Converter = converter; // return the TypeConverter to the class instance via the event arguments..
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
