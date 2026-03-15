namespace UFWebClient
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
#if !WINDOWS_PHONE
    using System.Windows.Browser;
#endif

    /// <summary>
    /// Wraps access to the strongly typed resource classes so that you can bind
    /// control properties to resource strings in XAML
    /// </summary>
    public sealed class ResourceWrapper
    {
        private static ApplicationStrings applicationStrings = new ApplicationStrings();
        private static SecurityQuestions securityQuestions = new SecurityQuestions();

        /// <summary>
        /// Gets the <see cref="ApplicationStrings"/>.
        /// </summary>
        public ApplicationStrings ApplicationStrings
        {
            get { return applicationStrings; }
        }

        /// <summary>
        /// Gets the <see cref="SecurityQuestions"/>.
        /// </summary>
        public SecurityQuestions SecurityQuestions
        {
            get { return securityQuestions; }
        }
    }
}
