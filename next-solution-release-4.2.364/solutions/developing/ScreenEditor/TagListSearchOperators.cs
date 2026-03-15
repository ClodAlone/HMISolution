using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager
{
    public class TagListSearchOperatorCaseSensitive : TagListSearchOperator
    {
        #region Static
        readonly static string internalName = "TagListSearchOperatorCaseSensitive";

        static TagListSearchOperatorCaseSensitive()
        {
            var instance = new TagListSearchOperatorCaseSensitive();
            if (CriteriaOperator.GetCustomFunction(internalName) == null)
            {
                CriteriaOperator.RegisterCustomFunction(instance);
            }
        }

        public static TagListSearchOperatorCaseSensitive Register() 
        {
            return CriteriaOperator.GetCustomFunction(internalName) as TagListSearchOperatorCaseSensitive;
        }
        #endregion

        #region Overrides
        protected override bool IsCaseSensitive
        {
            get
            {
                return true;
            }
        }

        public override string Name 
        { 
            get 
            {
                return internalName;
            } 
        }
        #endregion
    }

    public class TagListSearchOperatorCaseInsensitive : TagListSearchOperator
    {
        #region Static
        readonly static string internalName = "TagListSearchOperatorCaseInsensitive";

        static TagListSearchOperatorCaseInsensitive()
        {
            var instance = new TagListSearchOperatorCaseInsensitive();
            if (CriteriaOperator.GetCustomFunction(internalName) == null)
            {
                CriteriaOperator.RegisterCustomFunction(instance);
            }
        }

        public static TagListSearchOperatorCaseInsensitive Register()
        {
            return CriteriaOperator.GetCustomFunction(internalName) as TagListSearchOperatorCaseInsensitive;
        }
        #endregion

        #region Overrides
        protected override bool IsCaseSensitive
        {
            get
            {
                return false;
            }
        }

        public override string Name
        {
            get
            {
                return internalName;
            }
        }
        #endregion
    }

    public abstract class TagListSearchOperator : ICustomFunctionOperator
    {
        protected abstract bool IsCaseSensitive { get; }

        protected OperandProperty[] operandProperties;

        #region ICustomFunctionOperator
        public abstract string Name { get; }

        public string SearchString { get; set; }

        public CriteriaOperator FilterCriteria { get; private set; }

        public object Evaluate(params object[] operands)
        {
            if (String.IsNullOrEmpty(SearchString) || operands == null || operands.Length == 0)
                return true;

            var searchString = IsCaseSensitive ? SearchString : SearchString.ToLower();
            foreach (var operand in operands)
            {
                if (operand == null)
                    continue;

                var valueString = IsCaseSensitive ? operand.ToString() : operand.ToString().ToLower();
                if (SearchString.Contains(Properties.Settings.Default.MultipleCharPlaceHolder))
                {
                    if (ContainsCombinedSearchString(searchString, valueString, Properties.Settings.Default.MultipleCharPlaceHolder))
                        return true;
                }
                else if (SearchString.Contains(Properties.Settings.Default.OneCharPlaceHolder))
                {
                    if (ContainsCombinedSearchString(searchString, valueString, Properties.Settings.Default.OneCharPlaceHolder, checkOneChar: true))
                        return true;
                }
                else if (valueString.Contains(searchString))
                    return true;
            }            

            return false;
        }

        public Type ResultType(params Type[] operands)
        {
            return typeof(bool);
        }
        #endregion

        #region Public Methods
        public void SetCriteriaOperator(params String[] propertyNames)
        {
            operandProperties = new OperandProperty[propertyNames.Length];
            for (int ii = 0; ii < propertyNames.Length; ii++)
                operandProperties[ii] = new OperandProperty(propertyNames[ii]);
            FilterCriteria = new FunctionOperator(Name, operandProperties);
        }
        #endregion

        #region Private Methods
        bool ContainsCombinedSearchString(string searchString, string operand, char splitter, bool checkOneChar = false)
        {
            if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(operand))
                return false;

            if (searchString.IndexOf(splitter) != -1)
            {
                var searchingStrings = searchString.Split(splitter);
                var initString = searchingStrings[0];
                var endString = searchingStrings[1];

                if (string.IsNullOrEmpty(endString))
                {
                    return initString.Contains(operand);
                }
                else if (operand.Contains(initString))
                {
                    if (!checkOneChar &&
                        operand.Length > operand.IndexOf(initString) + initString.Length &&
                        operand.Substring(operand.IndexOf(initString) + initString.Length).Contains(endString))
                        return true;
                    else if (checkOneChar &&
                        operand.Length > operand.IndexOf(initString) + initString.Length + 1 &&
                        operand.Substring(operand.IndexOf(initString) + initString.Length + 1).StartsWith(endString))
                        return true;
                }
            }
            else
                return operand.Contains(searchString);
            
            return false;
        }        
        #endregion
    }
}
