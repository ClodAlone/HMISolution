using Opc.Ua.Utilities;
using RedundancyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyUnitTests.ActiveServerManager
{
    public class ChangedTagsFiller
    {
        #region Declarations
        readonly List<ChangedTags> changedTags;
        readonly int tagsCounter;
        readonly int valuesCounter;
        #endregion

        #region Constructors
        ChangedTagsFiller()
        {
            changedTags = new List<ChangedTags>(tagsCounter);
        }

        public ChangedTagsFiller(int tagsCounter, int valuesCounter) : 
            this()
        {
            this.tagsCounter = tagsCounter;
            this.valuesCounter = valuesCounter;

            FillChangedTags();
        }
        #endregion

        #region Methods
        void FillChangedTags()
        {
            for (int ii = 0; ii < tagsCounter; ii++)
            {
                var values = new WrappedDataValueCollection(valuesCounter);
                for (int cc = 0; cc < valuesCounter; cc++)
                    values.Add(new Opc.Ua.DataValue(new Opc.Ua.Variant(ii * cc)));
                changedTags.Add(new ChangedTags() { NodeId = new Opc.Ua.NodeId(Guid.NewGuid()).ToString(), DataValues = values });
            }
        }
        #endregion

        #region Properties
        public List<ChangedTags> ChangedTags
        {
            get
            {
                return changedTags;
            }
        }

        public int MaxTags
        {
            get
            {
                return tagsCounter;
            }
        }

        public int MaxValues
        {
            get
            {
                return valuesCounter;
            }
        }
        #endregion
    }
}
