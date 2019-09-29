# ConfLib
A library to store application configuration/settings into a SQLite database.

## Using auto-properties
To ease up saving and loading settings is to use auto-properties and TypeConverter class for more complex types such as System.Drawing.Color. To get the auto-properties to be notified of a change in a value the library uses [PropertyChanged.Fody](https://github.com/Fody/PropertyChanged) library, which injects the auto-properties to raise [PropertyChanged](docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged) event - but this is just the internal logic.

## Constructing a settings class
To implement the settings class to be as simple as possible we use the SettingsBase class to inherit the settings class from.
```C#
using System.Drawing; // for the Color class..
using PropertyChanged; // the previously mentioned PropertyChanged library..
using VPKSoft.ConfLib; // the library in question..

namespace YourApp
{
    /// <summary>
    /// A class for settings within the YourApp application.
    /// </summary>
    [AddINotifyPropertyChangedInterface] // use from a separate application requires this attribute from the PropertyChanged library..
    internal class Settings: SettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="confLib">An instance to a ConfLib class.</param>
        public Settings(Conflib confLib) : base(confLib)
        {
        }
    
        // a setting for a user name..
        [SettingAttribute("indentity/userName", typeof(string), true)] // the SettingAttribute must be set for a setting value..
        internal string UserName { get; set; } = string.Empty;    

        // a color value for some use..
        [SettingAttribute("profile/color", typeof(Color))] // the SettingAttribute must be set for a setting value..
        internal Color ProfileColor { get; set; } = Color.DarkCyan;
        
        // this is required for the base class event to be available for subscription..
        public override event OnRequestTypeConverter RequestTypeConverter
        {
            add => base.RequestTypeConverter += value;
            remove => base.RequestTypeConverter -= value;
        }        
```
So the Settings class now ready, lets take it into use:
```C#
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
```
No for the type converter:
```C#
        // get a type converter "form-to-from" string conversion for more complex types..
        private void Settings_RequestTypeConverter(object sender, TypeConverterEventArgs e)
        {
            // just a color so we can assume that a type converter will be found..
            var converter = TypeDescriptor.GetConverter(e.TypeOfConverter);
            e.Converter = converter; // return the TypeConverter to the class instance via the event arguments..
        }
```
Do note that the library can cache TypeConverter instances if allowed: `settings.CacheTypeConverters = true;`

Now just use the class by setting or getting the values from the properties marked with the SettingAttribute attribute class.

### Security
The ConfLib can encrypt data using the [ProtectedData](docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.protecteddata) class.
```C#
        [SettingAttribute("indentity/password", typeof(string), true)] // just set the Secure value to true..
        internal string Password { get; set; } = string.Empty;
```

### The generated database
The software genrates tables and adds rows to the tables automatically:

_Database from the [ConfLibTest sample application](https://github.com/VPKSoft/ConfLib/tree/master/ConfLibTest):_
![image](https://user-images.githubusercontent.com/40712699/65828441-a0fd1d00-e2a3-11e9-9457-1941b6fdd8b1.png)

##### Thanks to
* [JetBrains](http://www.jetbrains.com) for their open source license(s).

![JetBrains](http://www.vpksoft.net/site/External/JetBrains/jetbrains.svg)
