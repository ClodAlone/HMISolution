#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Drawing
{
    internal static class Helper
    {
        internal static Dictionary<string, int> columnAttributes;
        internal static string GetPlacementType(PlacementType placementType)
        {
            switch (placementType)
            {
                case PlacementType.FreeFloating:
                    return "absolute";

                case PlacementType.Move:
                    return "oneCell";

                case PlacementType.MoveAndSize:
                    return "twoCell";
            }
            throw new ArgumentException("Invalid PlacementType val");
        }
        internal static PlacementType GetPlacementType(string placementString)
        {
            switch (placementString)
            {
                case "absolute":
                    return PlacementType.FreeFloating;

                case "oneCell":
                    return PlacementType.Move;

                case "twoCell":
                    return PlacementType.MoveAndSize;
            }
            return PlacementType.MoveAndSize;
        }


        internal static string GetVerticalFlowType(TextVertOverflowType textVertOverflowType)
        {
            switch (textVertOverflowType)
            {
                case TextVertOverflowType.Clip:
                    return "clip";

                case TextVertOverflowType.Ellipsis:
                    return "ellipsis";
            }
            return "overflow";
        }
        internal static string GetHorizontalFlowType(TextHorzOverflowType textHorzOverflowType)
        {
            switch (textHorzOverflowType)
            {
                case TextHorzOverflowType.Clip:
                    return "clip";
            }
            return "overflow";
        }
        internal static TextVertOverflowType GetVerticalFlowType(string value)
        {
            switch (value)
            {
                case "clip":
                    return TextVertOverflowType.Clip;

                case "ellipsis":
                    return TextVertOverflowType.Ellipsis;
            }
            return TextVertOverflowType.OverFlow;
        }
        internal static TextHorzOverflowType GetHorizontalFlowType(string value)
        {
            switch (value)
            {
                case "clip":
                    return TextHorzOverflowType.Clip;
            }
            return TextHorzOverflowType.OverFlow;
        }
        internal static TextDirection SetTextDirection(string textVerticalType)
        {
            switch (textVerticalType)
            {
                case "horz":
                    return TextDirection.Horizontal;
                case "vert":
                    return TextDirection.RotateAllText90;
                case "vert270":
                    return TextDirection.RotateAllText270;
                case "wordArtVert":
                    return TextDirection.StackedLeftToRight;
                case "wordArtVertRtl":
                    return TextDirection.StackedRightToLeft;
            }
            return TextDirection.Horizontal;

        }

        internal static void SetAnchorPosition(TextFrame txtFrame, string anchorType, bool anchorCtrl)
        {
            switch (txtFrame.TextDirection)
            {
                case TextDirection.Horizontal:
                    {
                        switch (anchorType)
                        {
                            case "t":
                                if (anchorCtrl)
                                    txtFrame.VerticalAlignment = ExcelVerticalAlignment.TopCentered;
                                else
                                    txtFrame.VerticalAlignment = ExcelVerticalAlignment.Top;
                                break;
                            case "ctr":
                                if (anchorCtrl)
                                    txtFrame.VerticalAlignment = ExcelVerticalAlignment.MiddleCentered;
                                else
                                    txtFrame.VerticalAlignment = ExcelVerticalAlignment.Middle;
                                break;
                            case "b":
                                if (anchorCtrl)
                                    txtFrame.VerticalAlignment = ExcelVerticalAlignment.BottomCentered;
                                else
                                    txtFrame.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                break;
                        }
                        break;
                    }
                case TextDirection.RotateAllText90:
                case TextDirection.StackedRightToLeft:
                    {
                        switch (anchorType)
                        {
                            case "t":
                                if (anchorCtrl)
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.RightMiddle;
                                else
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                break;
                            case "ctr":
                                if (anchorCtrl)
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.CenterMiddle;
                                else
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                break;
                            case "b":
                                if (anchorCtrl)
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.LeftMiddle;
                                else
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                break;
                        }
                        break;
                    }
                case TextDirection.RotateAllText270:
                case TextDirection.StackedLeftToRight:
                    {
                        switch (anchorType)
                        {
                            case "t":
                                if (anchorCtrl)
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.LeftMiddle;
                                else
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                break;
                            case "ctr":
                                if (anchorCtrl)
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.CenterMiddle;
                                else
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                break;
                            case "b":
                                if (anchorCtrl)
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.RightMiddle;
                                else
                                    txtFrame.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                break;
                        }
                        break;
                    }
            }
        }

        internal static double ParseDouble(string value)
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }
        internal static int ParseInt(string value)
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }
        internal static string ToString(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToString(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        internal static bool ParseBoolen(string value)
        {
            string str;
            if (value.Length != 1)
            {
                return (string.Compare(value, "true") == 0);
            }
            if (((str = value) == null) || ((!(str == "1") && !(str == "t")) && !(str == "T")))
            {
                return false;
            }
            return true;
        }
        internal static short ParseShort(string value)
        {
            return short.Parse(value, CultureInfo.InvariantCulture);
        }
        internal static int ConvertEmuToOffset(int emuValue, int resolution)
        {
            return (int)((((((double)emuValue) / 12700.0) / 72.0) * resolution) + 0.5);
        }
        internal static int ConvertOffsetToEMU(int offsetValue, int resolution)
        {
            return (int)((((offsetValue * 72.0) / ((double)resolution)) * 12700.0) + 0.5);
        }
        internal static AnchorType GetAnchorType(string anchorType)
        {
            switch (anchorType)
            {
                case "oneCellAnchor":
                    return AnchorType.OneCell;
                case "absoluteAnchor":
                    return AnchorType.Absolute;
                case "relSizeAnchor":
                    return AnchorType.RelSize;
                default:
                    return AnchorType.TwoCell;
            }
        }
        internal static string GetAnchorTypeString(AnchorType anchorType)
        {
            switch (anchorType)
            {
                case AnchorType.Absolute:
                    return "absoluteAnchor";
                case AnchorType.OneCell:
                    return "oneCellAnchor";
                case AnchorType.RelSize:
                    return "relSizeAnchor";
                default:
                    return "twoCellAnchor";
            }
        }
        
    }
}

