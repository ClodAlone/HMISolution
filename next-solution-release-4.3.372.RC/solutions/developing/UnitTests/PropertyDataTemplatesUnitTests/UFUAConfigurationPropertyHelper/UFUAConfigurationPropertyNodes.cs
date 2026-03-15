using DevExpress.Xpo;
using Mindscape.WpfElements;
using Mindscape.WpfElements.PropertyEditing;
using UFUAEditor.PropertyDataTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyDataTemplatesUnitTests.UFUAConfigurationPropertyHelper
{
    public class UFUAConfigurationPropertyNodes : IDisposable
    {
        #region Declarations
        readonly UFUAModel.UFUAConfiguration ufuaConfiguration;
        readonly BytesSizePropertyEditor bytesSizePropertyEditor;
        readonly Session session;
        #endregion

        #region Constructors
        public UFUAConfigurationPropertyNodes()
        {
            session = new Session();
            ufuaConfiguration = new UFUAModel.UFUAConfiguration(session);

            var wrapper = CreateMaxHistoryTotalSafelyFilesSizePropertyWrapper();
            bytesSizePropertyEditor = new BytesSizePropertyEditor() { DataContext = wrapper };
        }
        #endregion

        #region Methods
        ObjectWrapper CreateMaxHistoryTotalSafelyFilesSizePropertyWrapper()
        {
            PropertyNode property = new PropertyNode(ufuaConfiguration, typeof(UFUAModel.UFUAConfiguration).GetProperty("MaxHistoryTotalSafelyFilesSize"), null);
            return ObjectWrapperFactory.CreateWrapper(property, true);
        }
        #endregion

        #region Properties
        public UFUAModel.UFUAConfiguration UFUAConfiguration
        {
            get
            {
                return ufuaConfiguration;
            }
        }
        #endregion

        #region Properties Editors
        public BytesSizePropertyEditor BytesSizePropertyEditor
        {
            get
            {
                return bytesSizePropertyEditor;
            }
        }
        #endregion

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            session.Dispose();
        }
    }
}
