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

namespace VPKSoft.ConfLib
{
    /// <summary>
    /// Event arguments for requesting a type converter for a given type.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TypeConverterEventArgs: EventArgs
    {
        /// <summary>
        /// Gets or sets the type converter.
        /// </summary>
        public TypeConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets the type for the type converter.
        /// </summary>
        public Type TypeOfConverter { get; set; }
    }
}
