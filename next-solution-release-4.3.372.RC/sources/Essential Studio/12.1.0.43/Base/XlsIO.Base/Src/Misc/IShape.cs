#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;

using Excel.Collections;

namespace Excel.Interfaces
{
  /// <summary>
  ///
  /// </summary>
  public interface IShape
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
    MsoAutoShapeType AutoShapeType { get; set; }
    MsoBlackWhiteMode BlackWhiteMode { get; set; }
    MsoTriState Child { get; }
    MsoTriState Connector { get; }
    XlCreator Creator { get; }
    MsoTriState HasDiagram { get; }
    MsoTriState HasDiagramNode { get; }
    MsoTriState HorizontalFlip { get; }
    MsoTriState LockAspectRatio { get; set; }
#endif
    #endregion


    Adjustments Adjustments { get; }
    string AlternativeText { get; set; }
    Application Application { get; }
    Range BottomRightCell { get; }
    CalloutFormat Callout { get; }
    CanvasShapes CanvasItems { get; }
    int ConnectionSiteCount { get; }
    ConnectorFormat ConnectorFormat { get; }
    ControlFormat ControlFormat { get; }
    Diagram Diagram { get; }
    DiagramNode DiagramNode { get; }
    object DrawingObject { get; }
    FillFormat Fill { get; }
    XlFormControl FormControlType { get; }
    GroupShapes GroupItems { get; }
    float Height { get; set; }
    Hyperlink Hyperlink { get; }
    int ID { get; }
    float Left { get; set; }
    LineFormat Line { get; }
    LinkFormat LinkFormat { get; }
    bool Locked { get; set; }
    string Name { get; set; }
    ShapeNodes Nodes { get; }
    OLEFormat OLEFormat { get; }
    string OnAction { get; set; }
    object Parent { get; }
    Shape ParentGroup { get; }
    PictureFormat PictureFormat { get; }
    XlPlacement Placement { get; set; }
    float Rotation { get; set; }
    Script Script { get; }
    ShadowFormat Shadow { get; }
    TextEffectFormat TextEffect { get; }
    TextFrame TextFrame { get; }
    ThreeDFormat ThreeD { get; }
    float Top { get; set; }
    Range TopLeftCell { get; }
    MsoShapeType Type { get; }
    MsoTriState VerticalFlip { get; }
    object Vertices { get; }
    MsoTriState Visible { get; set; }
    float Width { get; set; }
    int ZOrderPosition { get; }

    // Methods
    void Apply();
    void CanvasCropBottom(float Increment);
    void CanvasCropLeft(float Increment);
    void CanvasCropRight(float Increment);
    void CanvasCropTop(float Increment);
    void Copy();
    void CopyPicture(object Appearance, object Format);
    void Cut();
    void Delete();
    Excel.Shape Duplicate();
    void Flip(Microsoft.Office.Core.MsoFlipCmd FlipCmd);
    void IncrementLeft(float Increment);
    void IncrementRotation(float Increment);
    void IncrementTop(float Increment);
    void PickUp();
    void RerouteConnections();
    void ScaleHeight(float Factor, Microsoft.Office.Core.MsoTriState RelativeToOriginalSize, object Scale);
    void ScaleWidth(float Factor, Microsoft.Office.Core.MsoTriState RelativeToOriginalSize, object Scale);
    void Select(object Replace);
    void SetShapesDefaultProperties();
    Excel.ShapeRange Ungroup();
    void ZOrder(Microsoft.Office.Core.MsoZOrderCmd ZOrderCmd);
  }
}
