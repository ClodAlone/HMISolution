using DocumentManager.ComponentService;
using ExternalAuthentication.Model;
using System;
using UFUserEditor.ComponentService;

namespace ExternalAuthentication.Services
{
    public interface IGetExternalAuthenticationSettings
    {
        ExternalIdPUserSettings Execute(IDocument document);
    }

    public class GetExternalAuthenticationSettings : IGetExternalAuthenticationSettings
    {
        private IConvertGeneralSettingsToExternalIdPUserSettings _convertGeneralSettingsToExternalIdPUserSettings;
        private IUFUserEditorManager _userManager;

        public GetExternalAuthenticationSettings(
            IConvertGeneralSettingsToExternalIdPUserSettings convertGeneralSettingsToExternalIdPUserSettings, 
            IUFUserEditorManager userManager)
        {
            _convertGeneralSettingsToExternalIdPUserSettings = convertGeneralSettingsToExternalIdPUserSettings;
            _userManager = userManager;
        }

        public ExternalIdPUserSettings Execute(IDocument document)
        {
            try
            {
                var externalIdPUserSettings = new ExternalIdPUserSettings();
                
                var externalIdPSettingsDictionary = _userManager.GetExternalAuthenticationSettings(document);

                if (externalIdPSettingsDictionary != null && externalIdPSettingsDictionary.Count > 0)
                {
                    externalIdPUserSettings = _convertGeneralSettingsToExternalIdPUserSettings.Execute(externalIdPSettingsDictionary);
                }
                
                return externalIdPUserSettings;
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.GetExternalAuthenticationSettingsError + ex.Message);
            }
        }
    }
}
