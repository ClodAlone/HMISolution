using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Utilities;
using System.IO;
using UFUAServiceLibrary;
using DevExpress.Xpf.Core;
using WPFUtilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using Ookii.Dialogs.Wpf;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace UFUAInstallDeployServerService
{
    public class PasswordValidatorEx : PasswordValidator
    {
        //
        // Summary:
        //     Minimum number of unique chars required in the password
        public int RequiredUniqueChars { get; set; }

        public override Task<IdentityResult> ValidateAsync(string item)
        {
            var task = base.ValidateAsync(item);
            task.Wait();
            if (!task.Result.Succeeded)
                return task;
            
            List<string> list = new List<string>();
            if (RequiredUniqueChars > 0 && new String(item.Distinct().ToArray()).Length < RequiredUniqueChars)
            {
                list.Add(String.Format(Properties.Resources.PasswordRequiredUniqueCharsFormat, RequiredUniqueChars));
            }
            if (list.Count == 0)
                return Task.FromResult(IdentityResult.Success);

            return Task.FromResult(IdentityResult.Failed(string.Join(" ", list)));
        }
    }

    public class ServiceSettings : Observable, IDataErrorInfo
    {
        static PasswordValidatorEx passwordValidator;

        #region  Constructors
        public ServiceSettings()
        { }

        static ServiceSettings()
        {
            passwordValidator = new PasswordValidatorEx();
            passwordValidator.RequiredLength = Properties.Settings.Default.PasswordRequiredLenght;
            passwordValidator.RequireDigit = Properties.Settings.Default.PasswordRequiredDigit;
            passwordValidator.RequireLowercase = Properties.Settings.Default.PasswordRequiredLowercase;
            passwordValidator.RequireUppercase = Properties.Settings.Default.PasswordRequiredUppercase;
            passwordValidator.RequireNonLetterOrDigit = Properties.Settings.Default.PasswordRequiredNonLetterOrDigit;
            passwordValidator.RequiredUniqueChars = Properties.Settings.Default.PasswordRequiredUniqueChars;
        }
        #endregion

        #region Methods
        public void ConfigurePasswordValidator(int requiredLength, bool requireDigit, bool requireLowercase, bool requireUppercase, bool requireNonLetterOrDigit, int requiredUniqueChars)
        {
            passwordValidator.RequiredLength = requiredLength;
            passwordValidator.RequireDigit = requireDigit;
            passwordValidator.RequireLowercase = requireLowercase;
            passwordValidator.RequireUppercase = requireUppercase;
            passwordValidator.RequireNonLetterOrDigit = requireNonLetterOrDigit;
            passwordValidator.RequiredUniqueChars = requiredUniqueChars;
        }

        public bool IsValidSettings()
        {
            String error;
            return !string.IsNullOrEmpty(ProjectPath) && Directory.Exists(ProjectPath) &&
                !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) &&
                Password == ConfirmPassword &&
                IsValidPassword(Password, out error);
        }
        #endregion

        #region Properties
        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                Set(ref _UserName, value, "UserName");
            }
        }
        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                if (Set(ref _Password, value, "Password"))
                    OnPropertyChanged("ConfirmPassword");
            }
        }
        private string _ConfirmPassword;
        public string ConfirmPassword
        {
            get { return _ConfirmPassword; }
            set
            {
                Set(ref _ConfirmPassword, value, "ConfirmPassword");
            }
        }
        private string _ProjectPath;
        public string ProjectPath
        {
            get { return _ProjectPath; }
            set
            {
                Set(ref _ProjectPath, value, "ProjectPath");
            }
        }
        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "UserName")
            {
                if (string.IsNullOrEmpty(UserName))
                    return Properties.Resources.UserNameInvalid;
            }
            if (propertyName == "Password")
            {
                string error;
                if (!IsValidPassword(Password, out error))
                {
                    return String.Format("{0}{1}{2}", 
                        Properties.Resources.PasswordInvalid,
                        Environment.NewLine, error);
                }
            }
            if (propertyName == "ConfirmPassword")
            {
                if (Password != ConfirmPassword)
                    return Properties.Resources.PasswordMismatch;
            }
            if (propertyName == "ProjectPath")
            {
                String path = ProjectPath;
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    return Properties.Resources.ProjectPathInvalid;
            }

            return null;
        }

        bool IsValidPassword(string password, out string error)
        {
            error = String.Empty;
            var task = passwordValidator.ValidateAsync(password);
            task.Wait();
            if (task.Result.Errors != null)
            {
                var builder = new StringBuilder();
                foreach (var e in task.Result.Errors)
                    builder.AppendLine(e);
                error = builder.ToString();
            }
            return task.Result.Succeeded;
        }
        #endregion
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {
        #region Settings
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("Settings", typeof(ServiceSettings), typeof(MainWindow), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSettingsChanged), new CoerceValueCallback(OnCoerceSettings)));

        private static object OnCoerceSettings(DependencyObject o, object value)
        {
            MainWindow control = o as MainWindow;
            if (control != null)
                return control.OnCoerceSettings((ServiceSettings)value);
            else
                return value;
        }

        private static void OnSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MainWindow control = o as MainWindow;
            if (control != null)
                control.OnSettingsChanged((ServiceSettings)e.OldValue, (ServiceSettings)e.NewValue);
        }

        protected virtual ServiceSettings OnCoerceSettings(ServiceSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSettingsChanged(ServiceSettings oldValue, ServiceSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public ServiceSettings Settings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ServiceSettings)GetValue(SettingsProperty);
            }
            set
            {
                SetValue(SettingsProperty, value);
            }
        }

        #endregion
      
        #region Declarations
        internal CommadLineOptions cmdOptions;
        string settingsFilePath;
        bool isValidSettings;

        bool bLoaded;
        #endregion
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            ShowIcon = false;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    ApplicationPropertiesHelper.SetProperty("CurrentSkin", cmdOptions.CurrentSkin);
                    ThemeHelper.SetTheme(this);

                    if (!string.IsNullOrEmpty(cmdOptions.Title))
                        Title = cmdOptions.Title;

                    serviceControl.cmdOptions = cmdOptions;
                    serviceControl.EnableInstall = false;
                    settingsFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(cmdOptions.FilePath), Properties.Settings.Default.SettingsFileName);

                    ExecuteReloadSettings();
                }
            };
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand SaveSettings = new RoutedCommand();
        public static readonly RoutedCommand ReloadSettings = new RoutedCommand();
        #endregion

        #region Command Handlers
        private void OnSaveSettings(object sender, ExecutedRoutedEventArgs e)
        {
            WriteSettings();
        }

        private void WriteSettings(bool onInitFile = false)
        {
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(settingsFilePath)))
                return;

            JObject data = JObject.Parse(File.ReadAllText(settingsFilePath));

            var loginToken = data.SelectToken(Properties.Settings.Default.LoginJSonKey);
            var settingsToken = data.SelectToken(Properties.Settings.Default.DeployServerSettingsJSonKey);

            if (!string.IsNullOrEmpty(Settings.UserName) &&
                !string.IsNullOrEmpty(Settings.Password) &&
                !string.IsNullOrEmpty(Settings.ProjectPath) && Directory.Exists(Settings.ProjectPath) &&
                settingsToken != null && loginToken != null &&
                loginToken.HasValues && settingsToken.HasValues)
            {

                loginToken[Properties.Settings.Default.UserJSonKey] = Settings.UserName;
                loginToken[Properties.Settings.Default.PasswordJSonKey] = Settings.Password;
                settingsToken[Properties.Settings.Default.PathJSonKey] = Settings.ProjectPath;

                string jsonText = data.ToString();
                File.WriteAllText(settingsFilePath, jsonText);

                if (!onInitFile)
                    MessageBox.Show(this, Properties.Resources.RestartService, this.Title);
            }
        }
        private void CanSaveSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isValidSettings;
        }

        private void OnReloadSettings(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteReloadSettings();
        }

        void ExecuteReloadSettings()
        {
            Settings = ReadSettings();
            if (Settings != null)
            {
                ValidateSettings();
                Settings.PropertyChanged += (s, e) =>
                {
                    ValidateSettings();
                };
            }
        }

        void ValidateSettings()
        {
            bool isValid = Settings != null && Settings.IsValidSettings();
            if (isValidSettings != isValid)
            {
                isValidSettings = isValid;
                serviceControl.EnableInstall = isValid;
            }
        }

        public ServiceSettings ReadSettings()
        {
            ServiceSettings serviceSettings = new ServiceSettings();

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(settingsFilePath)))
                return serviceSettings;
            
            JObject data = JObject.Parse(File.ReadAllText(settingsFilePath));
            serviceSettings.UserName = data.SelectToken($"{Properties.Settings.Default.LoginJSonKey}.{Properties.Settings.Default.UserJSonKey}")?.ToString();
            serviceSettings.Password = data.SelectToken($"{Properties.Settings.Default.LoginJSonKey}.{Properties.Settings.Default.PasswordJSonKey}")?.ToString();
            serviceSettings.ConfirmPassword = serviceSettings.Password;
            serviceSettings.ProjectPath = data.SelectToken($"{Properties.Settings.Default.DeployServerSettingsJSonKey}.{Properties.Settings.Default.PathJSonKey}")?.ToString();

            var customPasswordComplexityRules = data.SelectToken($"{Properties.Settings.Default.PasswordComplexityRulesJsonKey}")?.ToString();
            if (!String.IsNullOrEmpty(customPasswordComplexityRules))
            {
                int requiredLength = Properties.Settings.Default.PasswordRequiredLenght;
                if (int.TryParse(data.SelectToken($"{Properties.Settings.Default.PasswordComplexityRulesJsonKey}.RequiredLength")?.ToString(), out int rLength))
                    requiredLength = rLength;

                bool requiredDigit = Properties.Settings.Default.PasswordRequiredDigit;
                if (bool.TryParse(data.SelectToken($"{Properties.Settings.Default.PasswordComplexityRulesJsonKey}.RequireDigit")?.ToString(), out bool rDigit))
                    requiredDigit = rDigit;

                bool requireLowercase = Properties.Settings.Default.PasswordRequiredLowercase;
                if (bool.TryParse(data.SelectToken($"{Properties.Settings.Default.PasswordComplexityRulesJsonKey}.RequireLowercase")?.ToString(), out bool rLowercase))
                    requireLowercase = rLowercase;

                bool requireUppercase = Properties.Settings.Default.PasswordRequiredUppercase;
                if (bool.TryParse(data.SelectToken($"{Properties.Settings.Default.PasswordComplexityRulesJsonKey}.RequireUppercase")?.ToString(), out bool rUppercase))
                    requireUppercase = rUppercase;

                bool requireNonalphanum = Properties.Settings.Default.PasswordRequiredNonLetterOrDigit;
                if (bool.TryParse(data.SelectToken($"{Properties.Settings.Default.PasswordComplexityRulesJsonKey}.RequireNonAlphanumeric")?.ToString(), out bool rNonalphanum))
                    requireNonalphanum = rNonalphanum;

                int requiredUniqueChars = Properties.Settings.Default.PasswordRequiredUniqueChars;
                if (int.TryParse(data.SelectToken($"{Properties.Settings.Default.PasswordComplexityRulesJsonKey}.RequiredUniqueChars")?.ToString(), out int rUniqueChars))
                    requiredUniqueChars = rUniqueChars;

                serviceSettings.ConfigurePasswordValidator(requiredLength, requiredDigit, requireLowercase, requireUppercase, requireNonalphanum, requiredUniqueChars);
            }

            return serviceSettings;
        }

        private void CanReloadSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            //dialog.SelectedPath = selectedpath;
            if (dialog.ShowDialog() != true)
                return;
            projectPath.Text = dialog.SelectedPath;
        }
        #endregion
    }
}
