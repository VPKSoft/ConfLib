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

using System.Drawing;
using PropertyChanged;
using VPKSoft.ConfLib;

namespace ConfLibTest
{
    /// <summary>
    /// A class for settings within the test application.
    /// </summary>

    // ReSharper disable once CommentTypo
    // this needs to be added since the inherited class within a different project doesn't get injected via
    // the PropertyChanged.Fody (https://github.com/Fody/PropertyChanged) library..
    [AddINotifyPropertyChangedInterface]
    internal class Settings: SettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="confLib">An instance to a ConfLib class.</param>
        public Settings(Conflib confLib) : base(confLib)
        {
        }

        [SettingAttribute("text", typeof(string))]
        internal string Text { get; set; } = "ConfLibTest  © VPKSoft";

        [SettingAttribute("Year", typeof(string))]
        internal string Year { get; set; } = "2019";

        [SettingAttribute("texts/tbFirstName", typeof(string), true)]
        internal string FirstName { get; set; } = string.Empty;

        [SettingAttribute("texts/tbLastName", typeof(string))]
        internal string LastName { get; set; } = string.Empty;

        [SettingAttribute("bools/cbCoding", typeof(bool))]
        internal bool MasterCoder { get; set; }

        [SettingAttribute("bools/cbNerd", typeof(bool))]
        internal bool UserIsANerd { get; set; }

        [SettingAttribute("groupBoxes/gbGender", typeof(int))]
        internal int GenderIndex { get; set; } = -1;

        [SettingAttribute("nonPrimitiveType/favoriteColor", typeof(Color))]
        internal Color FavoriteColor { get; set; } = Color.DarkCyan;

        // this is required for the base class event to be available for subscription..
        public override event OnRequestTypeConverter RequestTypeConverter
        {
            add => base.RequestTypeConverter += value;
            remove => base.RequestTypeConverter -= value;
        }
    }
}
