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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PropertyChanged;
using VPKSoft.ConfLib.Annotations;

namespace VPKSoft.ConfLib
{
    // ReSharper disable once CommentTypo
    /// <summary>
    /// A base class for settings with dependency injection via PropertyChanged.Fody library.
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" /></summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class SettingsBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        protected SettingsBase(Conflib conflib)
        {
            conflib.AutoCreateSettings = true; // this must be enabled in order for things to work..
            Conflib = conflib;

            PropertyChanged += SettingsBase_PropertyChanged;
        }

        /// <summary>
        /// An instance to a Conflib class.
        /// </summary>
        public Conflib Conflib { get; set; }

        private void SettingsBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try // just try from the beginning..
            {
                PropertyInfo propertyInfo = // first get the property info for the property..
                    GetType().GetProperty(e.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // get the property value..
                object value = propertyInfo?.GetValue(this);

                // get the setting attribute value of the property.. 
                if (propertyInfo != null)
                {
                    SettingAttribute settingAttribute =
                        (SettingAttribute) propertyInfo.GetCustomAttribute(typeof(SettingAttribute));

                    if (value != null && settingAttribute != null)
                    {
                        if (value.GetType().IsPrimitive || value is string)
                        {
                            Conflib[settingAttribute.SettingName] = (settingAttribute.Secure ? "SECURE:" : string.Empty) + value;
                        }
                        else // a type converted is needed for non-primitive types..
                        {
                            // try to get a type converter for the type..
                            var converter = GetMeATypeConverter(value.GetType());

                            // set the value if the type converter was successfully gotten..
                            if (converter != null)
                            {
                                Conflib[settingAttribute.SettingName] = (settingAttribute.Secure ? "SECURE:" : string.Empty) + converter.ConvertToString(value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // inform of the exception..
                ReportExceptionAction?.Invoke(ex);
            }
        }

        /// <summary>
        /// Gets me a type converter.
        /// </summary>
        /// <param name="type">The type to get the <see cref="TypeConverter"/> for.</param>
        private TypeConverter GetMeATypeConverter(Type type)
        {
            // try to find a type converter from the cache..
            var converter = typeConverterCache.FirstOrDefault(f => f.Key == type).Value;

            TypeConverterEventArgs args = new TypeConverterEventArgs()
                {TypeOfConverter = type};

            // if the type isn't cached, raise the event..
            if (converter == default(TypeConverter))
            {
                RequestTypeConverter?.Invoke(this, args);

                // if the caching is set, just get the converter..
                if (args.Converter != null && CacheTypeConverters)
                {
                    typeConverterCache.Add(
                        new KeyValuePair<Type, TypeConverter>(type, args.Converter));
                }
            }
            else // a converter was already found, so leave it as it is..
            {
                args.Converter = converter;
            }

            return args.Converter;
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public void LoadSettings()
        {
            // get all public instance properties of this class..
            PropertyInfo[] propertyInfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // loop through the properties..
            foreach (var propertyInfo in propertyInfos)
            {
                try // avoid crashes..
                {
                    // get the SettingAttribute class instance of the property..
                    var settingAttribute = (SettingAttribute) propertyInfo.GetCustomAttribute(typeof(SettingAttribute));

                    // only properties marked with the SettingAttribute will be handled..
                    if (settingAttribute == null)
                        continue;

                    // get the default value for the property..
                    object currentValue = propertyInfo.GetValue(this);

                    // null values aren't taken into count..
                    if (currentValue == null)
                    {
                        continue;
                    }

                    // primitive types only need a string conversion..
                    if (settingAttribute.SettingType.IsPrimitive || settingAttribute.SettingType == typeof(string))
                    {
                        propertyInfo.SetValue(this, Convert.ChangeType(Conflib[settingAttribute.SettingName, currentValue.ToString()], settingAttribute.SettingType));
                    }
                    else // try to set the value via a type converter..
                    {
                        var converter = GetMeATypeConverter(settingAttribute.SettingType);
                        if (converter != null)
                        {
                            propertyInfo.SetValue(this,
                                converter.ConvertFromString(Conflib[settingAttribute.SettingName, currentValue.ToString()]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // inform of the exception..
                    ReportExceptionAction?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// The type converter cache where the converters are collected if the <see cref="CacheTypeConverters"/> is set to true.
        /// </summary>
        private readonly List<KeyValuePair<Type, TypeConverter>> typeConverterCache =
            new List<KeyValuePair<Type, TypeConverter>>();

        /// <summary>
        /// Gets or sets a value indicating whether the class should cache the type converters gotten via <see cref="RequestTypeConverter"/> event.
        /// </summary>
        public bool CacheTypeConverters { get; set; }

        /// <summary>
        /// A delegate for the <see cref="RequestTypeConverter"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="TypeConverterEventArgs"/> instance containing the event data.</param>
        public delegate void OnRequestTypeConverter(object sender, TypeConverterEventArgs e);

        /// <summary>
        /// Occurs when a type converter is required for a value conversion to or from a string value.
        /// </summary>
        public virtual event OnRequestTypeConverter RequestTypeConverter;

        /// <summary>
        /// An action which is called when an exception occurs within the class.
        /// </summary>
        public Action<Exception> ReportExceptionAction { get; set; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property value has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            PropertyChanged -= SettingsBase_PropertyChanged;
        }
    }
}
