using System;
using System.Windows.Data;
using UFRecipeSettings.UFRecipeModel;
using UFUAModel.Extensions;
using Utilities;
using Utilities.Converters;

namespace RecipeViewerControl.ViewModel
{
    internal class RecipeValueViewModel : Observable, IDisposable
    {
        #region Declarations
        readonly UFDataValueEntity dataValue;
        readonly int arrayIndex;
        readonly RecipeGridViewModel parent;

        UFUAModel.Helpers.EngineeringUnitData engineeringUnitData;
        #endregion

        #region Constructors
        public RecipeValueViewModel(UFDataValueEntity dataValue, RecipeGridViewModel parent)
            : this(dataValue, parent, -1)
        { }

        public RecipeValueViewModel(UFDataValueEntity dataValue, RecipeGridViewModel parent, int arrayIndex)
        {
            this.dataValue = dataValue;
            this.parent = parent;
            this.arrayIndex = arrayIndex;
            if (this.dataValue.DataType.IsMinMaxType() && !String.IsNullOrEmpty(dataValue.EngineeringUnit))
            {
                var euXml = parent.UFUAEditorService?.GetEngineeringUnit(parent.recipeDocument, dataValue.EngineeringUnit, inExecution: true);
                if (!String.IsNullOrEmpty(euXml))
                {
                    try
                    {
                        engineeringUnitData = euXml.FromXml<UFUAModel.Helpers.EngineeringUnitData>();
                    }
                    catch
                    { }
                }
            }
        }
        #endregion

        #region Properties
        public String GroupName
        {
            get
            {
                if (dataValue.UFGroupAss != null)
                    return dataValue.UFGroupAss.GroupName;
                else
                    return String.Empty;
            }
        }

        public String Name
        {
            get
            {
                if (ArrayIndex >= 0)
                    return String.Format("{0}({1})", dataValue.DataValueName, ArrayIndex);
                else
                    return dataValue.DataValueName;
            }
        }

        public string Description
        {
            get
            {
                return dataValue.Description;
            }
        }

        //object value;
        //public object Value
        //{
        //    get
        //    {
        //        return value;
        //    }
        //    set
        //    {
        //        Set(ref this.value, value, "Value");
        //    }
        //}

        public UFDataValueEntity DataValue
        {
            get
            {
                return dataValue;
            }
        }

        public int DecimalDigits
        {
            get
            {
                if (dataValue.DataType.IsDecimalType())
                    return dataValue.DecimalDigits;
                else if (engineeringUnitData != null)
                    return (int)Math.Ceiling(Math.Abs(Math.Log10(MultiplierFactor)));
                else
                    return 0;
            }

        }

        public double MultiplierFactor
        {
            get
            {
                if (engineeringUnitData != null &&
                    engineeringUnitData.EURange.Magnitude > 0 &&
                    engineeringUnitData.InstrumentRange.Magnitude > 0)
                {

                    return engineeringUnitData.EURange.Magnitude / engineeringUnitData.InstrumentRange.Magnitude;
                }
                else
                    return 1;
            }

        }

        public int ArrayIndex
        {
            get
            {
                if (dataValue.ArrayDimension > 0)
                    return arrayIndex;
                else
                    return -1;
            }

        }

        public UFUAModel.DataType DataType
        {
            get
            {
                if (MultiplierFactor != 1)
                    return UFUAModel.DataType.Float;
                else
                    return dataValue.DataType;
            }

        }

        public String UnitName
        {
            get
            {
                if (engineeringUnitData != null)
                    return engineeringUnitData.UnitName;
                else
                    return dataValue.UnitName;
            }
        }

        public double MinValue
        {
            get
            {
                if (engineeringUnitData != null)
                    return engineeringUnitData.EURange.Low;
                else
                    return dataValue.MinValue;
            }
        }

        public double MaxValue
        {
            get
            {
                if (engineeringUnitData != null)
                    return engineeringUnitData.EURange.High;
                else
                    return dataValue.MaxValue;
            }
        }

        IValueConverter valueConverter;
        public IValueConverter ValueConverter
        {
            get
            {
                if (valueConverter == null)
                {
                    if (ArrayIndex >= 0)
                    {
                        var formula = ExpressionValueConverter.GetArrayFormula(ArrayIndex);
                        var converter = new ExpressionValueConverter() { Formula = formula, ReverseFormula = formula, AdjustOutOfRangeArrayIndex = true };
                        converter.ParseFormula();
                        valueConverter = converter;
                    }
                    if (MultiplierFactor != 1)
                    {
                        var converter = new Converters.EngineeringUnitValueConverter(MultiplierFactor);
                        if (valueConverter != null)
                            valueConverter = new CombiningConverter() { Converter1 = converter, Converter2 = valueConverter };
                        else
                            valueConverter = converter;
                    }
                }

                return valueConverter;
            }
        }

        public RecipeGridViewModel Parent
        {
            get
            {
                return parent;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (valueConverter is IDisposable)
            {
                (valueConverter as IDisposable).Dispose();
                valueConverter = null;
            }
        }
        #endregion
    }
}
