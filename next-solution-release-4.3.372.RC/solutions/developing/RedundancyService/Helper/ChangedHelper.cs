using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedundancyService.Helper
{
    public abstract class ChangedHelper<T>
    {
        #region Declarations
        int elementsDivision;
        int valuesDivision;
        
        protected int skipElements;
        protected int skipValues;
        protected int takeElements;
        protected int takeValues;
        #endregion

        #region Constructors
        protected ChangedHelper()
        {
            elementsDivision = valuesDivision = 1;
        }
        #endregion

        #region Abstract Methods
        protected abstract int GetElementsCount();
        protected abstract int GetValuesCount();
        public abstract List<T> GetChangedElementsBlock();
        #endregion

        #region Methods
        public void GoToNextChangedElementsBlock()
        {
            if (ProcessingElementsCounter == 1 && 
                ReadyValuesCounter > ProcessingValuesCounter)
            {
                skipValues += takeValues;
            }
            else
            {
                skipElements += takeElements;
                takeElements = takeValues = skipValues = 0;
            }
        }

        public bool MergeChangedElementsBlock()
        {
            var oldElementsDivision = elementsDivision;
            var oldValuesDivision = valuesDivision;

            if (elementsDivision > 1)
                elementsDivision /= 2;
            else if (valuesDivision > 1)
                valuesDivision /= 2;

            return oldElementsDivision != elementsDivision || oldValuesDivision != valuesDivision;
        }

        public bool SplitChangedElementsBlock()
        {
            var oldElementsDivision = elementsDivision;
            var oldValuesDivision = valuesDivision;

            if (GetElementsCount() > 0)
            {
                takeElements = GetCurrentElementsCounter();
                takeValues = GetCurrentValuesCounter();

                if (ProcessingElementsCounter > 1)
                    elementsDivision *= 2;
                else if (ProcessingValuesCounter > 1)
                    valuesDivision *= 2;
            }

            return oldElementsDivision != elementsDivision || oldValuesDivision != valuesDivision;
        }

        protected void UpdateCounters()
        {
            takeElements = GetCurrentElementsCounter();
            takeValues = GetCurrentValuesCounter();
        }

        int GetCurrentElementsCounter()
        {
            return Math.Max(1, GetElementsCount() / elementsDivision);
        }

        int GetCurrentValuesCounter()
        {
            if (skipElements < GetElementsCount())
                return Math.Max(1, GetValuesCount() / valuesDivision);

            return 0;
        }

        #endregion

        #region Properties
        public int ReadyElementsCounter
        {
            get
            {
                return Math.Max(0, GetElementsCount() - skipElements);
            }
        }

        public int ReadyValuesCounter
        {
            get
            {
                if (skipElements < GetElementsCount())
                    return Math.Max(0, GetValuesCount() - skipValues);

                return 0;
            }
        }

        public int ProcessedElementsCounter
        {
            get
            {
                return Math.Min(skipElements, GetElementsCount());
            }
        }

        public int ProcessingElementsCounter
        {
            get
            {
                return Math.Min(takeElements, ReadyElementsCounter);
            }
        }

        public int ProcessingValuesCounter
        {
            get
            {
                return Math.Min(takeValues, ReadyValuesCounter);
            }
        }

        public bool IsTerminated
        {
            get
            {
                return ProcessedElementsCounter >= GetElementsCount();
            }
        }
        #endregion
    }
}
