#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;


namespace Syncfusion.XlsIO.Silverlight.Implementation.Extensions
{
  public static class PivotSubtotalItems2007Extension
  {
    public static PivotSubtotalItems2007[] GetValues()
    {
      return new PivotSubtotalItems2007[]
      {
        PivotSubtotalItems2007.avg,
        PivotSubtotalItems2007.count,
        PivotSubtotalItems2007.countA,
        PivotSubtotalItems2007.max,
        PivotSubtotalItems2007.min,
        PivotSubtotalItems2007.product,
        PivotSubtotalItems2007.stdDev,
        PivotSubtotalItems2007.stdDevP,
        PivotSubtotalItems2007.sum,
        PivotSubtotalItems2007.var,
        PivotSubtotalItems2007.varP,
      };
    }
  }

  public static class ExcelGradientPresetExtension
  {
    public static ExcelGradientPreset[] GetValues()
    {
      return new ExcelGradientPreset[]
      {
        ExcelGradientPreset.Grad_Early_Sunset,
        ExcelGradientPreset.Grad_Late_Sunset,
        ExcelGradientPreset.Grad_Nightfall,
        ExcelGradientPreset.Grad_Daybreak,
        ExcelGradientPreset.Grad_Horizon,
        ExcelGradientPreset.Grad_Desert,
        ExcelGradientPreset.Grad_Ocean,
        ExcelGradientPreset.Grad_Calm_Water,
        ExcelGradientPreset.Grad_Fire,
        ExcelGradientPreset.Grad_Fog,
        ExcelGradientPreset.Grad_Moss,
        ExcelGradientPreset.Grad_Peacock,
        ExcelGradientPreset.Grad_Wheat,
        ExcelGradientPreset.Grad_Parchment,
        ExcelGradientPreset.Grad_Mahogany,
        ExcelGradientPreset.Grad_Rainbow,
        ExcelGradientPreset.Grad_RainbowII,
        ExcelGradientPreset.Grad_Gold,
        ExcelGradientPreset.Grad_GoldII,
        ExcelGradientPreset.Grad_Brass,
        ExcelGradientPreset.Grad_Chrome,
        ExcelGradientPreset.Grad_ChromeII,
        ExcelGradientPreset.Grad_Silver,
        ExcelGradientPreset.Grad_Sapphire,
      };
    }
  }
}
