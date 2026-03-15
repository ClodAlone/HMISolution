using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Constants;
using ViewModelLib;

namespace UFProjectManager
{
    [DataContract(Name = "ChildProjectData", Namespace = Namespaces.UriProgea)]
    public class UFChildProjectData : ViewModelBase
#if !WINDOWS_UWP
        , ICloneable
#endif
    {
#region Persistance
        [DataMember]
        bool isStartable;
        [DataMember]
        string projectName;
#endregion

#region Ctors
        public UFChildProjectData()
        { }

        internal UFChildProjectData(UFChildProjectData template)
        {
            if (template == null)
                return;

            isStartable = template.isStartable;
            projectName = template.projectName;
        }

#endregion

#region Properties

        public bool IsStartable
        {
            get { return isStartable; }
            set
            {
                Set(ref isStartable, value, "IsStartable");
            }
        }

        public string ProjectName
        {
            get { return projectName; }
            set
            {
                Set(ref projectName, value, "ProjectName");
            }
        }

#endregion

#region PropertyChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        protected void Set<T>(ref T field, T value, string propertyName)
        {
            if (!Object.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

#endregion

#region ICloneable Members

#if !WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Clone()
        {
            return new UFChildProjectData(this);
        }
#endif
#endregion
    }
}
