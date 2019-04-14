#region License
/*
ConfLib

A library for storing application setting into a SQLite database.
Copyright (C) 2015 VPKSoft, Petteri Kautonen

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using VPKSoft.ConfLib;

namespace ConfLibTest
{
    public partial class FormMain : Form
    {
        Conflib cl;
        public FormMain()
        {
            InitializeComponent();
            cl = new Conflib();

            // Use auto create
            cl.AutoCreateSettings = true;

            Text = cl["text", "ConfLibTest  © VPKSoft"] + "  " + cl["year", "2015"];

            tbFirstName.Text = cl["texts/" + tbFirstName.Name];
            tbLastName.Text = cl["texts/" + tbLastName.Name];

            cbCoding.Checked = cl["bools/" + cbCoding.Name, false.ToString()] == string.Empty ? false : Convert.ToBoolean(cl["bools/" + cbCoding.Name]);
            cbNerd.Checked = cl["bools/" + cbNerd.Name, false.ToString()] == string.Empty ? false : Convert.ToBoolean(cl["bools/" + cbNerd.Name]);

            if (cl["groupBoxes/" + gbGender.Name, "-1"] != "-1")
            {                
                foreach (Control c in gbGender.Controls)
                {
                    if ((c is RadioButton) && (string)(c as RadioButton).Tag == cl["groupBoxes/" + gbGender.Name])
                    {
                        (c as RadioButton).Checked = true;
                        break;
                    }
                }
            }

        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            cl["texts/" + tbFirstName.Name] = "SECURE:" + tbFirstName.Text;
            cl["texts/" + tbLastName.Name] = tbLastName.Text;

            cl["bools/" + cbCoding.Name] = cbCoding.Checked.ToString();
            cl["bools/" + cbNerd.Name] = cbNerd.Checked.ToString();

            foreach (Control c in gbGender.Controls)
            {
                if ((c is RadioButton) && (c as RadioButton).Checked)
                {
                    cl["groupBoxes/" + gbGender.Name] = (c as RadioButton).Tag.ToString();
                    break;
                }
            }

        }
    }
}
