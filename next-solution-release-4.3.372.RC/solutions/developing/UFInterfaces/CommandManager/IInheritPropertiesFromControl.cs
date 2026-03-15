using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;

namespace CommandManagerService.CommandManager
{
    /// <summary>
    /// This interface provides access to some control's properties in order to allow other components, such as commands, to inherit their values.
    /// </summary>
    public interface IInheritPropertiesFromControl
    {
        /// <summary>
        /// Used to check the minimum allowed value property as configured on the implementing control.
        /// </summary>
        /// <returns>A decimal representing the control's minimum acceptable value</returns>
        decimal GetMinValue();
        /// <summary>
        /// Used to check the maximum allowed value property as configured on the implementing control.
        /// </summary>
        /// <returns>A decimal representing the control's maximum acceptable value</returns>
        decimal GetMaxValue();
        /// <summary>
        /// Used to check the number of precision digits configured on the implementing control.
        /// </summary>
        /// <returns>An integer representing the # of precision digits</returns>
        int GetPrecisionDigit();
        /// <summary>
        /// Used to check whether the implementing control is configured to use an Engineering Unit or not.
        /// </summary>
        /// <returns>A boolean indicating if the control is expected to make use of an Engineering Unit</returns>
        bool GetUseEngineeringUnit();
        /// <summary>
        /// Provides access to the Unit Converter ID used by the control implementing the interface.
        /// </summary>
        /// <returns>The Unit Converter ID (null if not found).</returns>
        string GetUnitConverterFromControl(object control);
        /// <summary>
        /// Provides access to the XML serialization of the minimum tag's reference used by the control implementing the interface.
        /// </summary>
        /// <returns>The XML serialization of the minimum tag's OPCUAEntityReference.</returns>
        string GetTagMinValue();
        /// <summary>
        /// Provides access to the XML serialization of the maximum tag's reference used by the control implementing the interface.
        /// </summary>
        /// <returns>The XML serialization of the maximum tag's OPCUAEntityReference.</returns>
        string GetTagMaxValue();
    }
}
