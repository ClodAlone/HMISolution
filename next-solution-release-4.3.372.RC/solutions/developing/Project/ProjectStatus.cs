using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFProject
{
    public class ProjectStatus
    {
        #region Declarations
        bool running;
        UFProject parent;

        #region Constructors
        public ProjectStatus(UFProject p)
        {
            parent = p;
        }
        #endregion
                       
        #endregion

        #region Properties
        public bool Running
        {
            get
            {
                return running;
            }
            set
            {
                running = value;
            }
        }
        #endregion
    }
}
