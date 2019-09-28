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
using System.Security.Cryptography;

namespace VPKSoft.ConfLib
{
    /// <summary>
    /// An attribute class for describing a setting name and it's type.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)] // target a property only..
    public class SettingAttribute: Attribute
    {
        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        /// Gets or sets the type of the setting.
        /// </summary>
        public Type SettingType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this data should be encrypted / decrypted. See <see cref="ProtectedData"/> class with the current user scope.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAttribute"/> class.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        /// <param name="secure">A value indicating whether this data should be encrypted / decrypted. See <see cref="ProtectedData"/> class with the current user scope.</param>
        public SettingAttribute(string settingName, Type type, bool secure)
        {
            SettingName = settingName; // save the given values..
            SettingType = type;
            Secure = secure;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAttribute"/> class.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        public SettingAttribute(string settingName, Type type)
        {
            SettingName = settingName; // save the given values..
            SettingType = type;
        }
    }
}
