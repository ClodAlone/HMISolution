/**
* DevExtreme (ui/diagram.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DataSource, { DataSourceLike } from '../data/data_source';

import {
    DxElement,
} from '../core/element';

import {
    template,
} from '../core/templates/template';

import {
  EventInfo,
  InitializedEventInfo,
  ChangedOptionInfo,
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ContentReadyEvent = EventInfo<dxDiagram>;

export type CustomCommandEvent = {
    readonly component: dxDiagram;
    readonly element: DxElement;
    readonly name: string;
};

export type DisposingEvent = EventInfo<dxDiagram>;

export type InitializedEvent = InitializedEventInfo<dxDiagram>;

export type ItemClickEvent = EventInfo<dxDiagram> & {
    readonly item: Item;
};

export type ItemDblClickEvent = EventInfo<dxDiagram> & {
    readonly item: Item;
};

export type OptionChangedEvent = EventInfo<dxDiagram> & ChangedOptionInfo;

export type RequestEditOperationEvent = EventInfo<dxDiagram> & {
    readonly operation: 'addShape' | 'addShapeFromToolbox' | 'deleteShape' | 'deleteConnector' | 'changeConnection' | 'changeConnectorPoints';
    readonly args: dxDiagramAddShapeArgs | dxDiagramAddShapeFromToolboxArgs | dxDiagramDeleteShapeArgs | dxDiagramDeleteConnectorArgs | dxDiagramChangeConnectionArgs | dxDiagramChangeConnectorPointsArgs | dxDiagramBeforeChangeShapeTextArgs | dxDiagramChangeShapeTextArgs | dxDiagramBeforeChangeConnectorTextArgs | dxDiagramChangeConnectorTextArgs | dxDiagramResizeShapeArgs | dxDiagramMoveShapeArgs;
    readonly reason: 'checkUIElementAvailability' | 'modelModification';
    allowed?: boolean;
};

export type RequestLayoutUpdateEvent = EventInfo<dxDiagram> & {
    readonly changes: any[];
    allowed?: boolean;
};

export type SelectionChangedEvent = EventInfo<dxDiagram> & {
    readonly items: Array<Item>;
};

export type CustomShapeTemplateData = {
    readonly item: dxDiagramShape;
};

export type CustomShapeToolboxTemplateData = {
    readonly item: dxDiagramShape;
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramOptions extends WidgetOptions<dxDiagram> {
    /**
     * Specifies how the Diagram UI component automatically zooms the work area.
     */
    autoZoomMode?: 'fitContent' | 'fitWidth' | 'disabled';
    /**
     * Configures the context menu&apos;s settings.
     */
    contextMenu?: {
      /**
       * Lists commands in the context menu.
       */
      commands?: Array<'separator' | 'exportSvg' | 'exportPng' | 'exportJpg' | 'undo' | 'redo' | 'cut' | 'copy' | 'paste' | 'selectAll' | 'delete' | 'fontName' | 'fontSize' | 'bold' | 'italic' | 'underline' | 'lineStyle' | 'lineWidth' | 'textAlignLeft' | 'textAlignCenter' | 'textAlignRight' | 'lock' | 'unlock' | 'sendToBack' | 'bringToFront' | 'insertShapeImage' | 'editShapeImage' | 'deleteShapeImage' | 'connectorLineType' | 'connectorLineStart' | 'connectorLineEnd' | 'layoutTreeTopToBottom' | 'layoutTreeBottomToTop' | 'layoutTreeLeftToRight' | 'layoutTreeRightToLeft' | 'layoutLayeredTopToBottom' | 'layoutLayeredBottomToTop' | 'layoutLayeredLeftToRight' | 'layoutLayeredRightToLeft' | 'fullScreen' | 'zoomLevel' | 'showGrid' | 'snapToGrid' | 'gridSize' | 'units' | 'pageSize' | 'pageOrientation' | 'pageColor' | 'simpleView' | 'toolbox'>;
      /**
       * Specifies whether the context menu is enabled.
       */
      enabled?: boolean;
    };
    /**
     * Configures the context toolbox&apos;s settings.
     */
    contextToolbox?: {
      /**
       * Specifies the category of shapes that are displayed in the context toolbox.
       */
      category?: 'general' | 'flowchart' | 'orgChart' | 'containers' | 'custom' | string;
      /**
       * Specifies how shapes are displayed in the context toolbox.
       */
      displayMode?: 'icons' | 'texts';
      /**
       * Specifies the context toolbox&apos;s availability.
       */
      enabled?: boolean;
      /**
       * Specifies the number of shape icons in a row.
       */
      shapeIconsPerRow?: number;
      /**
       * Lists the shapes that are displayed in the context toolbox. The built-in shape types are shown in the Shape Types section.
       */
      shapes?: Array<'text' | 'rectangle' | 'ellipse' | 'cross' | 'triangle' | 'diamond' | 'heart' | 'pentagon' | 'hexagon' | 'octagon' | 'star' | 'arrowLeft' | 'arrowTop' | 'arrowRight' | 'arrowBottom' | 'arrowNorthSouth' | 'arrowEastWest' | 'process' | 'decision' | 'terminator' | 'predefinedProcess' | 'document' | 'multipleDocuments' | 'manualInput' | 'preparation' | 'data' | 'database' | 'hardDisk' | 'internalStorage' | 'paperTape' | 'manualOperation' | 'delay' | 'storedData' | 'display' | 'merge' | 'connector' | 'or' | 'summingJunction' | 'verticalContainer' | 'horizontalContainer' | 'cardWithImageOnLeft' | 'cardWithImageOnTop' | 'cardWithImageOnRight'> | Array<string>;
      /**
       * Specifies the context toolbox&apos;s width in pixels.
       */
      width?: number;
    };
    /**
     * A function that is executed after a custom command item was clicked and allows you to implement the custom command&apos;s logic.
     */
    onCustomCommand?: ((e: CustomCommandEvent) => void);
    /**
     * Specifies a custom template for shapes.
     */
    customShapeTemplate?: template | ((container: DxElement<SVGElement>, data: CustomShapeTemplateData) => any);
    /**
     * Specifies a custom template for shapes in the toolbox.
     */
    customShapeToolboxTemplate?: template | ((container: DxElement<SVGElement>, data: CustomShapeToolboxTemplateData) => any);
    /**
     * Provide access to an array of custom shapes.
     */
    customShapes?: Array<{
      /**
       * Specifies whether a card shape&apos;s image can be edited.
       */
      allowEditImage?: boolean;
      /**
       * Specifies whether the shape&apos;s text can be edited.
       */
      allowEditText?: boolean;
      /**
       * Specifies whether the shape can be resized.
       */
      allowResize?: boolean;
      /**
       * Specifies the shape background image&apos;s fractional height.
       */
      backgroundImageHeight?: number;
      /**
       * Specifies the shape background image&apos;s left offset.
       */
      backgroundImageLeft?: number;
      /**
       * Specifies the shape background image&apos;s top offset.
       */
      backgroundImageTop?: number;
      /**
       * Specifies the shape background image&apos;s URL.
       */
      backgroundImageUrl?: string;
      /**
       * Specifies the shape image displayed in the toolbox.
       */
      backgroundImageToolboxUrl?: string;
      /**
       * Specifies the shape background image&apos;s fractional width.
       */
      backgroundImageWidth?: number;
      /**
       * Specifies the base shape type for the custom shape. The built-in shape types are shown in the Shape Types section.
       */
      baseType?: 'text' | 'rectangle' | 'ellipse' | 'cross' | 'triangle' | 'diamond' | 'heart' | 'pentagon' | 'hexagon' | 'octagon' | 'star' | 'arrowLeft' | 'arrowTop' | 'arrowRight' | 'arrowBottom' | 'arrowNorthSouth' | 'arrowEastWest' | 'process' | 'decision' | 'terminator' | 'predefinedProcess' | 'document' | 'multipleDocuments' | 'manualInput' | 'preparation' | 'data' | 'database' | 'hardDisk' | 'internalStorage' | 'paperTape' | 'manualOperation' | 'delay' | 'storedData' | 'display' | 'merge' | 'connector' | 'or' | 'summingJunction' | 'verticalContainer' | 'horizontalContainer' | 'cardWithImageOnLeft' | 'cardWithImageOnTop' | 'cardWithImageOnRight' | string;
      /**
       * Specifies a category to which the custom shape belongs.
       */
      category?: string;
      /**
       * An array of the shape&apos;s connection points.
       */
      connectionPoints?: Array<{
        /**
         * Specifies the horizontal offset.
         */
        x?: number;
        /**
         * Specifies the vertical offset.
         */
        y?: number;
      }>;
      /**
       * Specifies the initial height of the shape.
       */
      defaultHeight?: number;
      /**
       * Specifies the URL of an image displayed in a card shape.
       */
      defaultImageUrl?: string;
      /**
       * Specifies the initial text of the shape.
       */
      defaultText?: string;
      /**
       * Specifies the initial width of the shape.
       */
      defaultWidth?: number;
      /**
       * Specifies the shape image&apos;s fractional height.
       */
      imageHeight?: number;
      /**
       * Specifies the shape image&apos;s left offset.
       */
      imageLeft?: number;
      /**
       * Specifies the shape image&apos;s top offset.
       */
      imageTop?: number;
      /**
       * Specifies the shape image&apos;s fractional width.
       */
      imageWidth?: number;
      /**
       * Specifies whether the shape maintains its width-to-height ratio on auto resize.
       */
      keepRatioOnAutoSize?: boolean;
      /**
       * Specifies the maximum height of the shape.
       */
      maxHeight?: number;
      /**
       * Specifies the maximum width of the shape.
       */
      maxWidth?: number;
      /**
       * Specifies the maximum height of the shape.
       */
      minHeight?: number;
      /**
       * Specifies the minimum width of the shape.
       */
      minWidth?: number;
      /**
       * Specifies a custom template for the shape.
       */
      template?: template | ((container: DxElement<SVGElement>, data: CustomShapeTemplateData) => any);
      /**
       * Specifies the shape template&apos;s fractional height.
       */
      templateHeight?: number;
      /**
       * Specifies the shape template&apos;s left offset.
       */
      templateLeft?: number;
      /**
       * Specifies the shape template&apos;s top offset.
       */
      templateTop?: number;
      /**
       * Specifies the shape template&apos;s fractional width.
       */
      templateWidth?: number;
      /**
       * Specifies the shape text container&apos;s height.
       */
      textHeight?: number;
      /**
       * Specifies the shape text&apos;s left offset.
       */
      textLeft?: number;
      /**
       * Specifies the shape text&apos;s top offset.
       */
      textTop?: number;
      /**
       * Specifies the shape text container&apos;s width.
       */
      textWidth?: number;
      /**
       * Specifies the shape&apos;s tooltip in the toolbox panel.
       */
      title?: string;
      /**
       * Specifies a custom template for the shape in the toolbox.
       */
      toolboxTemplate?: template | ((container: DxElement<SVGElement>, data: CustomShapeToolboxTemplateData) => any);
      /**
       * Specifies the aspect ratio of the shape in the toolbox.
       */
      toolboxWidthToHeightRatio?: number;
      /**
       * Specifies the custom shape&apos;s identifier.
       */
      type?: string;
    }>;
    /**
     * Configures default item properties.
     */
    defaultItemProperties?: {
      /**
       * Specifies a default item style.
       */
      style?: Object;
      /**
       * Specifies an item&apos;s default text style.
       */
      textStyle?: Object;
      /**
       * Specifies the default type of a connector.
       */
      connectorLineType?: 'straight' | 'orthogonal';
      /**
       * Specifies the default tip of a connector&apos;s start point.
       */
      connectorLineStart?: 'none' | 'arrow' | 'outlinedTriangle' | 'filledTriangle';
      /**
       * Specifies the default tip of a connector&apos;s end point.
       */
      connectorLineEnd?: 'none' | 'arrow' | 'outlinedTriangle' | 'filledTriangle';
      /**
       * Specifies the default minimum width of a shape.
       */
      shapeMinWidth?: number;
      /**
       * Specifies the default maximum width of a shape.
       */
      shapeMaxWidth?: number;
      /**
       * Specifies the default minimum height of a shape.
       */
      shapeMinHeight?: number;
      /**
       * Specifies the default maximum height of a shape.
       */
      shapeMaxHeight?: number;
    };
    /**
     * Specifies which editing operations a user can perform.
     */
    editing?: {
      /**
       * Specifies whether a user can add a shape.
       */
      allowAddShape?: boolean;
      /**
       * Specifies whether a user can delete a shape.
       */
      allowDeleteShape?: boolean;
      /**
       * Specifies whether a user can delete a connector.
       */
      allowDeleteConnector?: boolean;
      /**
       * Specifies whether a user can change a connection.
       */
      allowChangeConnection?: boolean;
      /**
       * Specifies whether a user can change connector points.
       */
      allowChangeConnectorPoints?: boolean;
      /**
       * Specifies whether a user can change a connector&apos;s text.
       */
      allowChangeConnectorText?: boolean;
      /**
       * Specifies whether a user can change a shape&apos;s text.
       */
      allowChangeShapeText?: boolean;
      /**
       * Specifies whether a user can resize a shape.
       */
      allowResizeShape?: boolean;
      /**
       * Specifies whether a user can move a shape.
       */
      allowMoveShape?: boolean;
    };
    /**
     * Allows you to bind the collection of diagram edges to a data source. For more information, see the Data Binding section.
     */
    edges?: {
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s custom data.
       */
      customDataExpr?: string | ((data: any, value?: any) => any);
      /**
       * Binds the edges collection to the specified data. Specify this property if you use node and edge data sources.
       */
      dataSource?: DataSourceLike<any>;
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s start node key.
       */
      fromExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s line start tip.
       */
      fromLineEndExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an index of a shape connection point where an edge starts.
       */
      fromPointIndexExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s key.
       */
      keyExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s line type.
       */
      lineTypeExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression whose Boolean value indicates whether an edge is locked.
       */
      lockedExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s key points.
       */
      pointsExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge style.
       */
      styleExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns edge text.
       */
      textExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s text style.
       */
      textStyleExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s end node key.
       */
      toExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s line end tip.
       */
      toLineEndExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an index of a shape connection point where an edge ends.
       */
      toPointIndexExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an edge&apos;s z-index.
       */
      zIndexExpr?: string | ((data: any, value?: any) => any);
    };
    /**
     * Configures export settings.
     */
    export?: {
      /**
       * Specifies the name of the file to which the diagram is exported.
       */
      fileName?: string;
      /**
       * Specifies the URL of the server-side proxy that streams the resulting file and enables export in Safari.
       * @deprecated Since v10, Safari browser supports API for saving files, and this property is no longer required.
       */
      proxyUrl?: string;
    };
    /**
     * Specifies whether or not to display the UI component in full-screen mode.
     */
    fullScreen?: boolean;
    /**
     * Specifies the grid pitch.
     */
    gridSize?: number | {
      /**
       * An array that specifies the _Grid Size_ combobox items on &apos;Properties&apos; panel.
       */
      items?: Array<number>;
      /**
       * Specifies the grid&apos;s pitch.
       */
      value?: number;
    };
    /**
     * Allows you to bind the collection of diagram nodes to a data source. For more information, see the Data Binding section.
     */
    nodes?: {
      /**
       * Specifies an auto-layout algorithm that the UI component uses to build a diagram.
       */
      autoLayout?: 'off' | 'tree' | 'layered' | {
        /**
         * Specifies the diagram layout orientation.
         */
        orientation?: 'vertical' | 'horizontal';
        /**
         * Specifies an auto-layout algorithm that is used to automatically arrange shapes.
         */
        type?: 'off' | 'tree' | 'layered';
      };
      /**
       * Specifies whether or not a shape size is automatically changed to fit the text when the UI component is bound to a data source.
       */
      autoSizeEnabled?: boolean;
      /**
       * Specifies the name of a data source field or an expression that returns a key of a node&apos;s parent container node.
       */
      containerKeyExpr?: string | ((data: any, value?: any) => any);
      /**
        * Specifies the name of a data source field or an expression that returns a container&apos;s nested items.
        */
       containerChildrenExpr?: string | ((data: any, value?: any) => any);
       /**
       * Specifies the name of a data source field or an expression that returns a node&apos;s custom data.
       */
      customDataExpr?: string | ((data: any, value?: any) => any);
      /**
       * Binds the nodes collection to the specified data. For more information, see the Data Binding section.
       */
      dataSource?: DataSourceLike<any>;
      /**
       * Specifies the name of a data source field or an expression that returns a node&apos;s height.
       */
      heightExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns an image URL or Base64 encoded image for a node.
       */
      imageUrlExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns a node&apos;s child items.
       */
      itemsExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns node keys.
       */
      keyExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns the x-coordinate of a node&apos;s left border.
       */
      leftExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression whose Boolean value indicates whether a node is locked.
       */
      lockedExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns a parent node key for a node.
       */
      parentKeyExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns a node style.
       */
      styleExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns node texts.
       */
      textExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns a node&apos;s text style.
       */
      textStyleExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns the y-coordinate of a node&apos;s top border.
       */
      topExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns the shape type for a node.
       */
      typeExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns a node&apos;s width.
       */
      widthExpr?: string | ((data: any, value?: any) => any);
      /**
       * Specifies the name of a data source field or an expression that returns a node&apos;s z-index.
       */
      zIndexExpr?: string | ((data: any, value?: any) => any);
    };
    /**
     * Indicates whether diagram content has unsaved changes.
     */
    hasChanges?: boolean;
    /**
     * A function that is executed after a shape or connector is clicked.
     */
    onItemClick?: ((e: ItemClickEvent) => void);
    /**
     * A function that is executed after a shape or connector is double-clicked.
     */
    onItemDblClick?: ((e: ItemDblClickEvent) => void);
    /**
     * A function that is executed after the selection is changed in the Diagram.
     */
    onSelectionChanged?: ((e: SelectionChangedEvent) => void);
    /**
     * A function that allows you to prohibit an edit operation at run time.
     */
    onRequestEditOperation?: ((e: RequestEditOperationEvent) => void);
    /**
     * A function that is executed after diagram data is reloaded and allows you to specify whether or not the UI component should update the diagram layout.
     */
    onRequestLayoutUpdate?: ((e: RequestLayoutUpdateEvent) => void);
    /**
     * Specifies the color of a diagram page.
     */
    pageColor?: string;
    /**
     * Specifies the page orientation.
     */
    pageOrientation?: 'portrait' | 'landscape';
    /**
     * Specifies a size of pages.
     */
    pageSize?: {
      /**
       * Specifies the page height.
       */
      height?: number;
      /**
       * An array that specifies the page size items in the _Page Size_ combobox on _Properties Panel_.
       */
      items?: Array<{
        /**
         * Specifies the page height.
         */
        height?: number;
        /**
         * Specifies the display text.
         */
        text?: string;
        /**
         * Specifies the page width.
         */
        width?: number;
      }>;
      /**
       * Specifies the page width.
       */
      width?: number;
    };
    /**
     * Configures the Properties panel settings.
     */
    propertiesPanel?: {
      /**
       * Contains an array of tabs in the Properties panel.
       */
      tabs?: Array<{
        /**
         * Lists commands in a tab.
         */
        commands?: Array<'separator' | 'exportSvg' | 'exportPng' | 'exportJpg' | 'undo' | 'redo' | 'cut' | 'copy' | 'paste' | 'selectAll' | 'delete' | 'fontName' | 'fontSize' | 'bold' | 'italic' | 'underline' | 'fontColor' | 'lineStyle' | 'lineWidth' | 'lineColor' | 'fillColor' | 'textAlignLeft' | 'textAlignCenter' | 'textAlignRight' | 'lock' | 'unlock' | 'sendToBack' | 'bringToFront' | 'insertShapeImage' | 'editShapeImage' | 'deleteShapeImage' | 'connectorLineType' | 'connectorLineStart' | 'connectorLineEnd' | 'layoutTreeTopToBottom' | 'layoutTreeBottomToTop' | 'layoutTreeLeftToRight' | 'layoutTreeRightToLeft' | 'layoutLayeredTopToBottom' | 'layoutLayeredBottomToTop' | 'layoutLayeredLeftToRight' | 'layoutLayeredRightToLeft' | 'fullScreen' | 'zoomLevel' | 'showGrid' | 'snapToGrid' | 'gridSize' | 'units' | 'pageSize' | 'pageOrientation' | 'pageColor' | 'simpleView' | 'toolbox'>;
        /**
         * Contains an array of command groups in the tab.
         */
        groups?: Array<{
          /**
           * Lists commands in a group.
           */
          commands?: Array<'separator' | 'exportSvg' | 'exportPng' | 'exportJpg' | 'undo' | 'redo' | 'cut' | 'copy' | 'paste' | 'selectAll' | 'delete' | 'fontName' | 'fontSize' | 'bold' | 'italic' | 'underline' | 'fontColor' | 'lineStyle' | 'lineWidth' | 'lineColor' | 'fillColor' | 'textAlignLeft' | 'textAlignCenter' | 'textAlignRight' | 'lock' | 'unlock' | 'sendToBack' | 'bringToFront' | 'insertShapeImage' | 'editShapeImage' | 'deleteShapeImage' | 'connectorLineType' | 'connectorLineStart' | 'connectorLineEnd' | 'layoutTreeTopToBottom' | 'layoutTreeBottomToTop' | 'layoutTreeLeftToRight' | 'layoutTreeRightToLeft' | 'layoutLayeredTopToBottom' | 'layoutLayeredBottomToTop' | 'layoutLayeredLeftToRight' | 'layoutLayeredRightToLeft' | 'fullScreen' | 'zoomLevel' | 'showGrid' | 'snapToGrid' | 'gridSize' | 'units' | 'pageSize' | 'pageOrientation' | 'pageColor' | 'simpleView' | 'toolbox'>;
          /**
           * Specifies a title of the group.
           */
          title?: string;
        }>;
        /**
         * Specifies the tab&apos;s title.
         */
        title?: string;
      }>;
      /**
       * Specifies the panel&apos;s visibility.
       */
      visibility?: 'auto' | 'visible' | 'collapsed' | 'disabled';
    };
    /**
     * Specifies whether the diagram is read-only.
     */
    readOnly?: boolean;
    /**
     * Specifies whether grid lines are visible.
     */
    showGrid?: boolean;
    /**
     * Switch the Diagram UI component to simple view mode.
     */
    simpleView?: boolean;
    /**
      * Specifies whether or not the UI component uses native scrolling.
      */
     useNativeScrolling?: boolean;
    /**
     * Specifies whether diagram elements should snap to grid lines.
     */
    snapToGrid?: boolean;
    /**
     * Configures the main toolbar settings.
     */
    mainToolbar?: {
      /**
       * Lists commands in the toolbar.
       */
      commands?: Array<'separator' | 'exportSvg' | 'exportPng' | 'exportJpg' | 'undo' | 'redo' | 'cut' | 'copy' | 'paste' | 'selectAll' | 'delete' | 'fontName' | 'fontSize' | 'bold' | 'italic' | 'underline' | 'fontColor' | 'lineStyle' | 'lineWidth' | 'lineColor' | 'fillColor' | 'textAlignLeft' | 'textAlignCenter' | 'textAlignRight' | 'lock' | 'unlock' | 'sendToBack' | 'bringToFront' | 'insertShapeImage' | 'editShapeImage' | 'deleteShapeImage' | 'connectorLineType' | 'connectorLineStart' | 'connectorLineEnd' | 'layoutTreeTopToBottom' | 'layoutTreeBottomToTop' | 'layoutTreeLeftToRight' | 'layoutTreeRightToLeft' | 'layoutLayeredTopToBottom' | 'layoutLayeredBottomToTop' | 'layoutLayeredLeftToRight' | 'layoutLayeredRightToLeft' | 'fullScreen' | 'zoomLevel' | 'showGrid' | 'snapToGrid' | 'gridSize' | 'units' | 'pageSize' | 'pageOrientation' | 'pageColor' | 'simpleView' | 'toolbox'>;
      /**
       * Specifies the toolbar&apos;s visibility.
       */
      visible?: boolean;
    };
    /**
     * Configures the history toolbar&apos;s settings.
     */
    historyToolbar?: {
      /**
       * Lists commands in the history toolbar.
       */
      commands?: Array<'separator' | 'exportSvg' | 'exportPng' | 'exportJpg' | 'undo' | 'redo' | 'cut' | 'copy' | 'paste' | 'selectAll' | 'delete' | 'fontName' | 'fontSize' | 'bold' | 'italic' | 'underline' | 'fontColor' | 'lineStyle' | 'lineWidth' | 'lineColor' | 'fillColor' | 'textAlignLeft' | 'textAlignCenter' | 'textAlignRight' | 'lock' | 'unlock' | 'sendToBack' | 'bringToFront' | 'insertShapeImage' | 'editShapeImage' | 'deleteShapeImage' | 'connectorLineType' | 'connectorLineStart' | 'connectorLineEnd' | 'layoutTreeTopToBottom' | 'layoutTreeBottomToTop' | 'layoutTreeLeftToRight' | 'layoutTreeRightToLeft' | 'layoutLayeredTopToBottom' | 'layoutLayeredBottomToTop' | 'layoutLayeredLeftToRight' | 'layoutLayeredRightToLeft' | 'fullScreen' | 'zoomLevel' | 'showGrid' | 'snapToGrid' | 'gridSize' | 'units' | 'pageSize' | 'pageOrientation' | 'pageColor' | 'simpleView' | 'toolbox'>;
      /**
       * Specifies the history toolbar&apos;s visibility.
       */
      visible?: boolean;
    };
    /**
     * Configures the view toolbar settings.
     */
    viewToolbar?: {
      /**
       * Lists commands in the toolbar.
       */
      commands?: Array<'separator' | 'exportSvg' | 'exportPng' | 'exportJpg' | 'undo' | 'redo' | 'cut' | 'copy' | 'paste' | 'selectAll' | 'delete' | 'fontName' | 'fontSize' | 'bold' | 'italic' | 'underline' | 'fontColor' | 'lineStyle' | 'lineWidth' | 'lineColor' | 'fillColor' | 'textAlignLeft' | 'textAlignCenter' | 'textAlignRight' | 'lock' | 'unlock' | 'sendToBack' | 'bringToFront' | 'insertShapeImage' | 'editShapeImage' | 'deleteShapeImage' | 'connectorLineType' | 'connectorLineStart' | 'connectorLineEnd' | 'layoutTreeTopToBottom' | 'layoutTreeBottomToTop' | 'layoutTreeLeftToRight' | 'layoutTreeRightToLeft' | 'layoutLayeredTopToBottom' | 'layoutLayeredBottomToTop' | 'layoutLayeredLeftToRight' | 'layoutLayeredRightToLeft' | 'fullScreen' | 'zoomLevel' | 'showGrid' | 'snapToGrid' | 'gridSize' | 'units' | 'pageSize' | 'pageOrientation' | 'pageColor' | 'simpleView' | 'toolbox'>;
      /**
       * Specifies the view toolbar&apos;s visibility.
       */
      visible?: boolean;
    };
    /**
     * Configures the toolbox settings.
     */
    toolbox?: {
      /**
       * Lists toolbox groups.
       */
      groups?: Array<{
        /**
         * Specifies the category of shapes that are displayed in the group.
         */
        category?: 'general' | 'flowchart' | 'orgChart' | 'containers' | 'custom' | string;
        /**
         * Specifies how shapes are displayed in the toolbox.
         */
        displayMode?: 'icons' | 'texts';
        /**
         * Specifies whether the group is expanded.
         */
        expanded?: boolean;
        /**
         * Lists the shapes in the group. The built-in shape types are shown in the Shape Types section.
         */
        shapes?: Array<'text' | 'rectangle' | 'ellipse' | 'cross' | 'triangle' | 'diamond' | 'heart' | 'pentagon' | 'hexagon' | 'octagon' | 'star' | 'arrowLeft' | 'arrowTop' | 'arrowRight' | 'arrowBottom' | 'arrowNorthSouth' | 'arrowEastWest' | 'process' | 'decision' | 'terminator' | 'predefinedProcess' | 'document' | 'multipleDocuments' | 'manualInput' | 'preparation' | 'data' | 'database' | 'hardDisk' | 'internalStorage' | 'paperTape' | 'manualOperation' | 'delay' | 'storedData' | 'display' | 'merge' | 'connector' | 'or' | 'summingJunction' | 'verticalContainer' | 'horizontalContainer' | 'cardWithImageOnLeft' | 'cardWithImageOnTop' | 'cardWithImageOnRight'> | Array<string>;
        /**
         * Specifies the group title in the toolbox.
         */
        title?: string;
      }> | Array<'general' | 'flowchart' | 'orgChart' | 'containers' | 'custom'>;
      /**
       * Specifies the number of shape icons in a row.
       */
      shapeIconsPerRow?: number;
      /**
       * Specifies whether the search box is visible.
       */
      showSearch?: boolean;
      /**
       * Specifies the toolbar&apos;s visibility.
       */
      visibility?: 'auto' | 'visible' | 'collapsed' | 'disabled';
      /**
       * Specifies the toolbox&apos;s width in pixels.
       */
      width?: number;
    };
    /**
     * Specifies the measurement unit for size properties.
     */
    units?: 'in' | 'cm' | 'px';
    /**
     * Specifies the measurement unit that is displayed in user interface elements.
     */
    viewUnits?: 'in' | 'cm' | 'px';
    /**
     * Specifies the zoom level.
     */
    zoomLevel?: number | {
      /**
       * An array that specifies the zoom level items in the _Zoom Level_ combobox on &apos;Properties&apos; panel.
       */
      items?: Array<number>;
      /**
       * Specifies the zoom level.
       */
      value?: number;
    };
}
/**
 * The Diagram UI component provides a visual interface to help you design new and modify existing diagrams.
 */
export default class dxDiagram extends Widget<dxDiagramOptions> {
    /**
     * Gets the DataSource instance.
     */
    getNodeDataSource(): DataSource;
    /**
     * Returns the DataSource instance.
     */
    getEdgeDataSource(): DataSource;
    /**
     * Returns a shape or connector object specified by its key.
     */
    getItemByKey(key: Object): Item;
    /**
     * Returns a shape or connector object specified by its internal identifier.
     */
    getItemById(id: string): Item;
    /**
      * Returns an array of diagram items.
      */
     getItems(): Array<Item>;
    /**
      * Returns an array of selected diagram items.
      */
     getSelectedItems(): Array<Item>;
    /**
      * Selects the specified items.
      */
     setSelectedItems(items: Array<Item>): void;
    /**
      * Scrolls the view area to the specified item.
      */
     scrollToItem(item: Item): void;
    /**
     * Exports the diagram data to a JSON object.
     */
    export(): string;
    /**
     * Exports the diagram to an image format.
     */
    exportTo(format: 'svg' | 'png' | 'jpg', callback: Function): void;
    /**
     * Imports the diagram data.
     */
    import(data: string, updateExistingItemsOnly?: boolean): void;
    /**
     * Updates the diagram toolboxes.
     */
    updateToolbox(): void;
    /**
      * Fits the diagram content into the work area. The maximum scale is 100%.
      */
     fitToContent(): void;
    /**
      * Fits the diagram content&apos;s width into the work area width. The maximum scale is 100%.
      */
     fitToWidth(): void;
}

/**
 * An object that provides information about a connector in the Diagram UI component.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramConnector extends Item {
    /**
     * Specifies the connector&apos;s start node key.
     */
    fromKey?: any;
    /**
     * Gets the connector&apos;s start node identifier.
     */
    fromId?: string;
    /**
     * The index of a shape connection point where the connector starts.
     */
    fromPointIndex?: number;
    /**
     * Gets the connector&apos;s key points.
     */
    points?: Array<{
      /**
       * A horizontal position of the point.
       */
      x?: number;
      /**
       * A vertical position of the point.
       */
      y?: number;
    }>;

    /**
     * Specifies the connector&apos;s text.
     */
    texts?: Array<string>;
    /**
     * Specifies the connector&apos;s end node key.
     */
    toKey?: any;
    /**
     * Gets the connector&apos;s end node identifier.
     */
    toId?: string;
    /**
     * The index of the shape connection point where the connector ends.
     */
    toPointIndex?: number;
}

export type Item = dxDiagramItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramItem {
    /**
     * Returns the data item that is bound to the diagram item.
     */
    dataItem?: any;
    /**
     * Specifies the item&apos;s internal identifier.
     */
    id?: string;
    /**
     * Gets the item&apos;s key from a data source.
     */
    key?: Object;
    /**
     * Returns the type of the item.
     */
    itemType?: 'shape' | 'connector';
}

/**
 * An object that provides information about a shape in the Diagram UI component.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramShape extends Item {
    /**
     * Specifies the shape&apos;s text.
     */
    text?: string;
    /**
     * Specifies the shape type. The built-in shape types are shown in the Shape Types section.
     */
    type?: 'text' | 'rectangle' | 'ellipse' | 'cross' | 'triangle' | 'diamond' | 'heart' | 'pentagon' | 'hexagon' | 'octagon' | 'star' | 'arrowLeft' | 'arrowTop' | 'arrowRight' | 'arrowBottom' | 'arrowNorthSouth' | 'arrowEastWest' | 'process' | 'decision' | 'terminator' | 'predefinedProcess' | 'document' | 'multipleDocuments' | 'manualInput' | 'preparation' | 'data' | 'database' | 'hardDisk' | 'internalStorage' | 'paperTape' | 'manualOperation' | 'delay' | 'storedData' | 'display' | 'merge' | 'connector' | 'or' | 'summingJunction' | 'verticalContainer' | 'horizontalContainer' | 'cardWithImageOnLeft' | 'cardWithImageOnTop' | 'cardWithImageOnRight' | string;
    /**
     * Specifies the position of the top left shape corner relative to the top left corner of the diagram work area.
     */
    position?: {
      /**
       * The horizontal shape position specified in units.
       */
      x?: number;
      /**
       * The vertical shape position specified in units.
       */
      y?: number;
    };

    /**
     * Specifies the shape size.
     */
    size?: {
      /**
       * The shape height specified in units.
       */
      height?: number;
      /**
       * The shape width specified in units.
       */
      width?: number;
    };
    /**
     * Gets an array of attached connector identifiers.
     */
    attachedConnectorIds?: Array<String>;
    /**
     * Gets the identifier of the container that stores the shape.
     */
    containerId?: string;
    /**
     * Gets identifiers of shapes stored in the container.
     */
    containerChildItemIds?: Array<String>;
    /**
     * Gets whether the container is expanded.
     */
    containerExpanded?: boolean;
}

/**
 * An object that provides information about a custom command in the Diagram UI component.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramCustomCommand {
    /**
     * Specifies the custom command&apos;s identifier.
     */
    name?: string;
    /**
     * Specifies the custom command&apos;s text and tooltip text.
     */
    text?: string;
    /**
     * Specifies the custom command&apos;s icon.
     */
    icon?: string;
    /**
     * Lists command sub items.
     */
    items?: Array<dxDiagramCustomCommand>;
    /**
     * Specifies a location for the command or separator on the main toolbar.
     */
    location?: 'after' | 'before' | 'center';
}

/**
 * Contains information about the processed shape.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramAddShapeArgs {
    /**
     * The processed shape.
     */
    shape?: dxDiagramShape;
    /**
     * A position where the shape is being added.
     */
    position?: {
      /**
       * A horizontal position where the shape is being added.
       */
      x?: number;
      /**
       * A vertical position where the shape is being added.
       */
      y?: number;
    };
}

/**
 * Contains information about the processed shape.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramAddShapeFromToolboxArgs {
  /**
   * The type of the processed shape.
   */
  shapeType?: 'text' | 'rectangle' | 'ellipse' | 'cross' | 'triangle' | 'diamond' | 'heart' | 'pentagon' | 'hexagon' | 'octagon' | 'star' | 'arrowLeft' | 'arrowTop' | 'arrowRight' | 'arrowBottom' | 'arrowNorthSouth' | 'arrowEastWest' | 'process' | 'decision' | 'terminator' | 'predefinedProcess' | 'document' | 'multipleDocuments' | 'manualInput' | 'preparation' | 'data' | 'database' | 'hardDisk' | 'internalStorage' | 'paperTape' | 'manualOperation' | 'delay' | 'storedData' | 'display' | 'merge' | 'connector' | 'or' | 'summingJunction' | 'verticalContainer' | 'horizontalContainer' | 'cardWithImageOnLeft' | 'cardWithImageOnTop' | 'cardWithImageOnRight' | string;
}

/**
 * Contains information about the processed shape.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramDeleteShapeArgs {
  /**
   * The processed shape.
   */
  shape?: dxDiagramShape;
}

/**
 * Contains information about the processed connector.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramDeleteConnectorArgs {
  /**
   * The processed connector.
   */
  connector?: dxDiagramConnector;
}

/**
 * Contains information about the processed connection.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramChangeConnectionArgs {
  /**
   * The new connected shape.
   */
  newShape?: dxDiagramShape;
  /**
   * The previous connected shape.
   */
  oldShape?: dxDiagramShape;
  /**
   * The processed connector.
   */
  connector?: dxDiagramConnector;
  /**
   * The index of the processed point in the shape&apos;s connection point collection.
   */
  connectionPointIndex?: number;
  /**
   * The position of the connector in the processed point.
   */
  connectorPosition?: 'start' | 'end';
}

/**
 * Contains information about the processed connector.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramChangeConnectorPointsArgs {
  /**
   * The processed connector.
   */
  connector?: dxDiagramConnector;
  /**
   * The array of new connection points.
   */
  newPoints?: Array<{
    /**
     * A horizontal position of the point.
     */
    x?: number;
    /**
     * A vertical position of the point.
     */
    y?: number;
  }>;
  /**
   * The array of previous connection points.
   */
  oldPoints?: Array<{
    /**
     * A horizontal position of the point.
     */
    x?: number;
    /**
     * A vertical position of the point.
     */
    y?: number;
  }>;
}

/**
 * Contains information about the processed shape.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramBeforeChangeShapeTextArgs {
  /**
   * The processed shape.
   */
  shape?: dxDiagramShape;
}

/**
 * Contains information about the processed shape.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramChangeShapeTextArgs {
  /**
   * The processed shape.
   */
  shape?: dxDiagramShape;
  /**
   * The new shape text.
   */
  text?: string;
}

/**
 * Contains information about the processed connector.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramBeforeChangeConnectorTextArgs {
  /**
   * The processed connector.
   */
  connector?: dxDiagramConnector;
  /**
   * The index of the processed text in the connector&apos;s texts collection.
   */
  index?: number;
}

/**
 * Contains information about the processed connector.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramChangeConnectorTextArgs {
  /**
   * The processed connector.
   */
  connector?: dxDiagramConnector;
  /**
   * The index of the processed text in the connector&apos;s texts collection.
   */
  index?: number;
  /**
   * The new connector text.
   */
  text?: string;
}

/**
 * Contains information about the processed shape.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramResizeShapeArgs {
  /**
   * The processed shape.
   */
  shape?: dxDiagramShape;
  /**
   * The new shape size.
   */
  newSize?: {
    /**
     * The new shape height.
     */
    height?: number;
    /**
     * The new shape width.
     */
    width?: number;
  };
  /**
   * The previous shape size.
   */
  oldSize?: {
    /**
     * The previous shape height.
     */
    height?: number;
    /**
     * The previous shape width.
     */
    width?: number;
  };
}

/**
 * Contains information about the processed shape.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDiagramMoveShapeArgs {
  /**
   * The processed shape.
   */
  shape?: dxDiagramShape;
  /**
   * The new shape position.
   */
  newPosition?: {
      /**
       * The new horizontal shape position specified in units.
       */
      x?: number;
      /**
       * The new vertical shape position specified in units.
       */
      y?: number;
  };
  /**
   * The previous shape position.
   */
  oldPosition?: {
      /**
       * The previous horizontal shape position specified in units.
       */
      x?: number;
      /**
       * The previous vertical shape position specified in units.
       */
      y?: number;
  };
}

export type Properties = dxDiagramOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxDiagramOptions;
