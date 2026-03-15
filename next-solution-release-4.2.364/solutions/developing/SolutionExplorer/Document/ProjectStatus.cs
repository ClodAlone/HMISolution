using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFProjectManager
{
    public class ProjectStatus
    {
        #region Declarations
        bool starting;
        bool running;
        bool terminating;
        UFProjectDocument parent;

        #region Constructors
        public ProjectStatus(UFProjectDocument p)
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
        public bool Starting
        {
            get
            {
                return starting;
            }
            set
            {
                starting = value;
            }
        }
        public bool Terminating
        {
            get
            {
                return terminating;
            }
            set
            {
                terminating = value;
            }
        }
        #endregion
    }
}
