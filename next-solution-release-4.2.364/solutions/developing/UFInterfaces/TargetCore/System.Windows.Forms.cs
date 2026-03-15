/// <summary>
/// Redefines enum in System.Windows.Forms.dll
/// </summary>

#if NET_STANDARD
namespace System.Windows.Forms
{
    //
    // Summary:
    //     Defines a set of standardized icons that can be associated with a ToolTip.
    public enum ToolTipIcon
    {
        //
        // Summary:
        //     Not a standard icon.
        None = 0,
        //
        // Summary:
        //     An information icon.
        Info = 1,
        //
        // Summary:
        //     A warning icon.
        Warning = 2,
        //
        // Summary:
        //     An error icon
        Error = 3
    }
}
#endif