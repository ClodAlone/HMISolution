/**
* DevExtreme (viz/tree_map.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DataSource, { DataSourceLike } from '../data/data_source';

import {
    UserDefinedElement,
    DxElement,
} from '../core/element';

import {
    PaletteType,
    PaletteExtensionModeType,
} from './palette';

import {
    template,
} from '../core/templates/template';

import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import BaseWidget, {
    BaseWidgetMargin,
    BaseWidgetOptions,
    BaseWidgetTooltip,
    Font,
    WordWrapType,
    VizTextOverflowType,
    FileSavingEventInfo,
    ExportInfo,
    IncidentInfo,
} from './core/base_widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface InteractionInfo {
  readonly node: dxTreeMapNode;
}

export type ClickEvent = NativeEventInfo<dxTreeMap, MouseEvent | PointerEvent> & {
  readonly node: dxTreeMapNode;
};

export type DisposingEvent = EventInfo<dxTreeMap>;

export type DrawnEvent = EventInfo<dxTreeMap>;

export type DrillEvent = EventInfo<dxTreeMap> & {
  readonly node: dxTreeMapNode;
};

export type ExportedEvent = EventInfo<dxTreeMap>;

export type ExportingEvent = EventInfo<dxTreeMap> & ExportInfo;

export type FileSavingEvent = FileSavingEventInfo<dxTreeMap>;

export type HoverChangedEvent = EventInfo<dxTreeMap> & InteractionInfo;

export type IncidentOccurredEvent = EventInfo<dxTreeMap> & IncidentInfo;

export type InitializedEvent = InitializedEventInfo<dxTreeMap>;

export type NodesInitializedEvent = EventInfo<dxTreeMap> & {
    readonly root: dxTreeMapNode;
};

export type NodesRenderingEvent = EventInfo<dxTreeMap> & {
    readonly node: dxTreeMapNode;
};

export type OptionChangedEvent = EventInfo<dxTreeMap> & ChangedOptionInfo;

export type SelectionChangedEvent = EventInfo<dxTreeMap> & InteractionInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxTreeMapOptions extends BaseWidgetOptions<dxTreeMap> {
    /**
     * Specifies the name of the data source field that provides nested items for a group. Applies to hierarchical data sources only.
     */
    childrenField?: string;
    /**
     * Specifies the name of the data source field that provides colors for tiles.
     */
    colorField?: string;
    /**
     * Manages the color settings.
     */
    colorizer?: {
      /**
       * Specifies the name of the data source field whose values define the color of a tile. Applies only if the type property is &apos;gradient&apos; or &apos;range&apos;.
       */
      colorCodeField?: string;
      /**
       * Specifies whether or not all tiles in a group must be colored uniformly. Applies only if the type property is &apos;discrete&apos;.
       */
      colorizeGroups?: boolean;
      /**
       * Sets the palette to be used to colorize tiles.
       */
      palette?: Array<string> | PaletteType;
      /**
       * Specifies what to do with colors in the palette when their number is less than the number of treemap tiles.
       */
      paletteExtensionMode?: PaletteExtensionModeType;
      /**
       * Allows you to paint tiles with similar values uniformly. Applies only if the type property is &apos;gradient&apos; or &apos;range&apos;.
       */
      range?: Array<number>;
      /**
       * Specifies the colorizing algorithm.
       */
      type?: 'discrete' | 'gradient' | 'none' | 'range';
    };
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<any>;
    /**
     * Configures groups.
     */
    group?: {
      /**
       * Configures the group borders.
       */
      border?: {
        /**
         * Colors the group borders.
         */
        color?: string;
        /**
         * Specifies the width of the group borders in pixels.
         */
        width?: number;
      };
      /**
       * Colors the group headers.
       */
      color?: string;
      /**
       * Specifies the distance in pixels between group borders and content.
       */
      padding?: number;
      /**
       * Specifies the height of the group headers in pixels.
       */
      headerHeight?: number;
      /**
       * Specifies whether groups change their style when a user pauses on them.
       */
      hoverEnabled?: boolean;
      /**
       * Specifies the appearance of groups in the hover state.
       */
      hoverStyle?: {
        /**
         * Configures the appearance of the group borders in the hover state.
         */
        border?: {
          /**
           * Colors the group borders in the hover state.
           */
          color?: string;
          /**
           * Specifies the width of the group borders in pixels. Applies to a group in the hover state.
           */
          width?: number;
        };
        /**
         * Colors the group headers in the hover state.
         */
        color?: string;
      };
      /**
       * Configures the group labels.
       */
      label?: {
        /**
         * Specifies the font settings of the group labels.
         */
        font?: Font;
        /**
         * Specifies what to do with labels that overflow their group headers: hide, truncated them with ellipsis, or leave them as they are.
         */
        textOverflow?: VizTextOverflowType;
        /**
         * Changes the visibility of the group labels.
         */
        visible?: boolean;
      };
      /**
       * Specifies the appearance of groups in the selected state.
       */
      selectionStyle?: {
        /**
         * Configures the appearance of the group borders in the selected state.
         */
        border?: {
          /**
           * Colors the group borders in the selected state.
           */
          color?: string;
          /**
           * Specifies the width of the group borders in pixels. Applies to a group in the selected state.
           */
          width?: number;
        };
        /**
         * Colors the group headers in the selected state.
         */
        color?: string;
      };
    };
    /**
     * Specifies whether tiles and groups change their style when a user pauses on them.
     */
    hoverEnabled?: boolean;
    /**
     * Specifies the name of the data source field that provides IDs for items. Applies to plain data sources only.
     */
    idField?: string;
    /**
     * Specifies whether the user will interact with a single tile or its group.
     */
    interactWithGroup?: boolean;
    /**
     * Specifies the name of the data source field that provides texts for tile and group labels.
     */
    labelField?: string;
    /**
     * Specifies the layout algorithm.
     */
    layoutAlgorithm?: 'sliceanddice' | 'squarified' | 'strip' | ((e: { rect?: Array<number>; sum?: number; items?: Array<any> }) => any);
    /**
     * Specifies the direction in which the items will be laid out.
     */
    layoutDirection?: 'leftBottomRightTop' | 'leftTopRightBottom' | 'rightBottomLeftTop' | 'rightTopLeftBottom';
    /**
     * Generates space around the UI component.
     */
    margin?: BaseWidgetMargin;
    /**
     * Specifies how many hierarchical levels must be visualized.
     */
    maxDepth?: number;
    /**
     * A function that is executed when a node is clicked or tapped.
     */
    onClick?: ((e: ClickEvent) => void) | string;
    /**
     * A function that is executed when a user drills up or down.
     */
    onDrill?: ((e: DrillEvent) => void);
    /**
     * A function that is executed after the pointer enters or leaves a node.
     */
    onHoverChanged?: ((e: HoverChangedEvent) => void);
    /**
     * A function that is executed only once, after the nodes are initialized.
     */
    onNodesInitialized?: ((e: NodesInitializedEvent) => void);
    /**
     * A function that is executed before the nodes are displayed and each time the collection of active nodes is changed.
     */
    onNodesRendering?: ((e: NodesRenderingEvent) => void);
    /**
     * A function that is executed when a node is selected or selection is canceled.
     */
    onSelectionChanged?: ((e: SelectionChangedEvent) => void);
    /**
     * Specifies the name of the data source field that provides parent IDs for items. Applies to plain data sources only.
     */
    parentField?: string;
    /**
     * Specifies whether a single or multiple nodes can be in the selected state simultaneously.
     */
    selectionMode?: 'multiple' | 'none' | 'single';
    /**
     * Configures tiles.
     */
    tile?: {
      /**
       * Configures the tile borders.
       */
      border?: {
        /**
         * Colors the tile borders.
         */
        color?: string;
        /**
         * Specifies the width of the tile borders in pixels.
         */
        width?: number;
      };
      /**
       * Specifies a single color for all tiles.
       */
      color?: string;
      /**
       * Specifies the appearance of tiles in the hover state.
       */
      hoverStyle?: {
        /**
         * Configures the appearance of the tile borders in the hover state.
         */
        border?: {
          /**
           * Colors the tile borders in the hover state.
           */
          color?: string;
          /**
           * Specifies the width of the tile borders in pixels. Applies to a tile in the hover state.
           */
          width?: number;
        };
        /**
         * Colors tiles in the hover state.
         */
        color?: string;
      };
      /**
       * Configures the tile labels.
       */
      label?: {
        /**
         * Specifies the font settings of the tile labels.
         */
        font?: Font;
        /**
         * Specifies what to do with labels that overflow their tiles after applying wordWrap: hide, truncate them and display an ellipsis, or do nothing.
         */
        textOverflow?: VizTextOverflowType;
        /**
         * Changes the visibility of the tile labels.
         */
        visible?: boolean;
        /**
         * Specifies how to wrap texts that do not fit into a single line.
         */
        wordWrap?: WordWrapType;
      };
      /**
       * Specifies the appearance of tiles in the selected state.
       */
      selectionStyle?: {
        /**
         * Configures the appearance of the tile borders in the selected state.
         */
        border?: {
          /**
           * Colors the tile borders in the selected state.
           */
          color?: string;
          /**
           * Specifies the width of the tile borders in pixels. Applies to a tile in the selected state.
           */
          width?: number;
        };
        /**
         * Colors tiles in the selected state.
         */
        color?: string;
      };
    };
    /**
     * Configures tooltips - small pop-up rectangles that display information about a data-visualizing UI component element being pressed or hovered over with the mouse pointer.
     */
    tooltip?: dxTreeMapTooltip;
    /**
     * Specifies the name of the data source field that provides values for tiles.
     */
    valueField?: string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxTreeMapTooltip extends BaseWidgetTooltip {
    /**
     * Specifies a custom template for a tooltip.
     */
    contentTemplate?: template | ((info: { value?: number; valueText?: string; node?: dxTreeMapNode }, element: DxElement) => string | UserDefinedElement);
    /**
     * Allows you to change tooltip appearance.
     */
    customizeTooltip?: ((info: { value?: number; valueText?: string; node?: dxTreeMapNode }) => any);
}
/**
 * The TreeMap is a UI component that displays hierarchical data by using nested rectangles.
 */
export default class dxTreeMap extends BaseWidget<dxTreeMapOptions> {
    /**
     * Deselects all nodes in the UI component.
     */
    clearSelection(): void;
    /**
     * Drills one level up.
     */
    drillUp(): void;
    /**
     * Gets the current node.
     */
    getCurrentNode(): dxTreeMapNode;
    getDataSource(): DataSource;
    /**
     * Gets the root node.
     */
    getRootNode(): dxTreeMapNode;
    /**
     * Hides the tooltip.
     */
    hideTooltip(): void;
    /**
     * Resets the drill down level.
     */
    resetDrillDown(): void;
}

/**
 * This section describes the Node object, which represents a treemap node.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxTreeMapNode {
    /**
     * Customizes the node.
     */
    customize(options: any): void;
    /**
     * The object from the data source visualized by the node.
     */
    data?: any;
    /**
     * Drills down into the node.
     */
    drillDown(): void;
    /**
     * Returns all nodes nested in the current node.
     */
    getAllChildren(): Array<dxTreeMapNode>;
    /**
     * Returns all descendant nodes.
     */
    getAllNodes(): Array<dxTreeMapNode>;
    /**
     * Gets a specific node from a collection of direct descendants.
     */
    getChild(index: number): dxTreeMapNode;
    /**
     * Indicates how many direct descendants the current node has.
     */
    getChildrenCount(): number;
    /**
     * Returns the parent node of the current node.
     */
    getParent(): dxTreeMapNode;
    /**
     * The index of the current node in the array of all nodes on the same level.
     */
    index?: number;
    /**
     * Indicates whether the current node is active.
     */
    isActive(): boolean;
    /**
     * Indicates whether the node is in the hover state or not.
     */
    isHovered(): boolean;
    /**
     * Indicates whether the node is visualized by a tile or a group of tiles.
     */
    isLeaf(): boolean;
    /**
     * Indicates whether the node is selected or not.
     */
    isSelected(): boolean;
    /**
     * Returns the label of the node.
     */
    label(): string;
    /**
     * Sets the label to the node.
     */
    label(label: string): void;
    /**
     * The level that the current node occupies in the hierarchy of nodes.
     */
    level?: number;
    /**
     * Reverts the appearance of the node to the initial state.
     */
    resetCustomization(): void;
    /**
     * Sets the selection state of a node.
     */
    select(state: boolean): void;
    /**
     * Shows the tooltip.
     */
    showTooltip(): void;
    /**
     * Gets the raw value of the node.
     */
    value(): number;
}

export type Properties = dxTreeMapOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxTreeMapOptions;
