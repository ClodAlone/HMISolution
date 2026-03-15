using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using StartupWelcome.Model;

namespace StartupWelcome.View_Model
{
    public class ListViewModel : RecentRepository
    {
         #region Properties
        
        private readonly DelegateCommand<RecentInfo> _addProjectPath;
        private readonly DelegateCommand<RecentInfo> _deleteProjectPath;
        private RecentInfo _selectedProjectPath;

        /// <summary>
        /// Gets or sets the selected ProjectPath.
        /// </summary>
        /// <value>The selected ProjectPath.</value>
        public RecentInfo SelectedProjectPath
        {
            get { return _selectedProjectPath; }
            set { _selectedProjectPath = value; }
        }

        /// <summary>
        /// Gets the add ProjectPath.
        /// </summary>
        /// <value>The add ProjectPath.</value>
        public DelegateCommand<RecentInfo> AddProjectPath
        {
            get { return _addProjectPath; }
        }


        /// <summary>
        /// Gets the delete ProjectPath.
        /// </summary>
        /// <value>The delete ProjectPath.</value>
        public DelegateCommand<RecentInfo> DeleteProjectPath
        {
            get { return _deleteProjectPath; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewModel"/> class.
        /// </summary>
        public ListViewModel()
        {
            _addProjectPath = new DelegateCommand<RecentInfo>(AddProjectPathHandler, CanAddProjectPath);
            _deleteProjectPath = new DelegateCommand<RecentInfo>(DeleteProjectPathHandler, CanDeleteProjectPath);
        }

        #endregion

        #region Command Handler

        /// <summary>
        /// Determines whether this instance [can add ProjectPath] the specified new ProjectPath.
        /// </summary>
        /// <param name="newProjectPath">The new ProjectPath.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can add ProjectPath] the specified new ProjectPath; otherwise, <c>false</c>.
        /// </returns>
        bool CanAddProjectPath(RecentInfo newProjectPath)
        {
            return true;
        }

        /// <summary>
        /// Adds the ProjectPath handler.
        /// </summary>
        /// <param name="newProjectPath">The new ProjectPath.</param>
        public void AddProjectPathHandler(RecentInfo newProjectPath)
        {
            if (newProjectPath == null)
            {
                return;
            }

            this.RecentList.Add(newProjectPath);
        }

        /// <summary>
        /// Determines whether this instance [can update zip code] the specified new zip code.
        /// </summary>
        /// <param name="newProjectPath">The new zip code.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can update zip code] the specified new zip code; otherwise, <c>false</c>.
        /// </returns>
        bool CanUpdateZipCode(RecentInfo newProjectPath)
        {
            return this.SelectedProjectPath != null;
        }

        /// <summary>
        /// Determines whether this instance [can delete ProjectPath] the specified new ProjectPath.
        /// </summary>
        /// <param name="newProjectPath">The new ProjectPath.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can delete ProjectPath] the specified new ProjectPath; otherwise, <c>false</c>.
        /// </returns>
        bool CanDeleteProjectPath(RecentInfo newProjectPath)
        {
            return this.SelectedProjectPath != null;
        }

        /// <summary>
        /// Updates the zip code handler.
        /// </summary>
        /// <param name="newProjectPath">The new ProjectPath.</param>
        public void UpdateProjectPathHandler(RecentInfo newProjectPath)
        {
            if (newProjectPath == null)
                return;

            _selectedProjectPath.ProjectPath = newProjectPath.ProjectPath;
        }

        /// <summary>
        /// Deletes the zip code handler.
        /// </summary>
        /// <param name="zipCode">The zip code.</param>
        public void DeleteProjectPathHandler(RecentInfo projectPath)
        {
            if (projectPath == null)
                return;

            this.RecentList.Remove(projectPath);
        }

        #endregion
   }
}
