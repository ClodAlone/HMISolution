/**
* DevExtreme (ui/data_grid.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DataSource, { DataSourceLike, Options as DataSourceOptions } from '../data/data_source';
import {
  UserDefinedElement,
  DxElement,
  UserDefinedElementsArray,
} from '../core/element';

import {
    DxPromise,
} from '../core/utils/deferred';

import {
    template,
} from '../core/templates/template';

import {
    DeepPartial,
    Skip,
} from '../core/index';

import {
    DxEvent,
    Cancelable,
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import {
    DataGridCell as ExcelCell,
} from '../excel_exporter';

import {
    ExcelFont,
} from '../exporter/excel/excel.doc_comments';

import dxDraggable from './draggable';

import {
    dxFilterBuilderOptions,
    FilterLookupDataSource,
} from './filter_builder';

import {
    dxFormOptions,
    dxFormSimpleItem,
} from './form';

import {
    Properties as PopupProperties,
} from './popup';

import dxScrollable from './scroll_view/ui.scrollable';

import dxSortable from './sortable';

import {
    dxToolbarOptions, dxToolbarItem,
} from './toolbar';

import {
    AsyncRule,
    CompareRule,
    CustomRule,
    EmailRule,
    NumericRule,
    PatternRule,
    RangeRule,
    RequiredRule,
    StringLengthRule,
} from './validation_rules';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

import {
  Format,
} from '../localization';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface AdaptiveDetailRowPreparingInfo {
  readonly formOptions: any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DataErrorOccurredInfo {
  readonly error?: Error;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DataChangeInfo<TRowData = any, TKey = any> {
  readonly changes: Array<DataChange<TRowData, TKey>>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface NewRowInfo<TRowData = any> {
  data: TRowData;
  promise?: PromiseLike<void>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface KeyDownInfo {
  handled: boolean;
}

type GroupKey = any[];

export type GroupData<TRowData> = {
  key: any;
  items: Array<TRowData> | Array<GroupData<TRowData>> | null;
  /**
   * @deprecated Attention! This property is for internal purposes only.
   */
  collapsedItems?: Array<TRowData> | Array<GroupData<TRowData>>;
  /**
   * @deprecated Attention! This property is for internal purposes only.
   */
  aggregates?: Array<any>;
  /**
   * @deprecated Attention! This property is for internal purposes only.
   */
  summary?: Array<any>;
  /**
   * @deprecated Attention! This property is for internal purposes only.
   */
  isContinuation?: boolean;
  /**
   * @deprecated Attention! This property is for internal purposes only.
   */
  isContinuationOnNextPage?: boolean;
};

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowKeyInfo<TKey = any> {
  readonly key: TKey;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowInsertedInfo<TRowData = any, TKey = any> {
  readonly data: TRowData;
  readonly key: TKey;
  readonly error: Error;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowInsertingInfo<TRowData = any> {
  data: TRowData;
  cancel: boolean | PromiseLike<void>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowRemovedInfo<TRowData = any, TKey = any> {
  readonly data: TRowData;
  readonly key: TKey;
  readonly error: Error;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowRemovingInfo<TRowData = any, TKey = any> {
  readonly data: TRowData;
  readonly key: TKey;
  cancel: boolean | PromiseLike<void>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowUpdatedInfo<TRowData = any, TKey = any> {
  readonly data: TRowData;
  readonly key: TKey;
  readonly error: Error;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowUpdatingInfo<TRowData = any, TKey = any> {
  readonly oldData: TRowData;
  newData: DeepPartial<TRowData>;
  readonly key: TKey;
  cancel: boolean | PromiseLike<void>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowValidatingInfo<TRowData = any, TKey = any> {
  readonly brokenRules: Array<RequiredRule | NumericRule | RangeRule | StringLengthRule | CustomRule | CompareRule | PatternRule | EmailRule | AsyncRule>;
  isValid: boolean;
  readonly key: TKey;
  readonly newData: DeepPartial<TRowData>;
  readonly oldData: TRowData;
  errorText: string;
  promise?: PromiseLike<void>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface SavingInfo<TRowData = any, TKey = any> {
  changes: Array<DataChange<TRowData, TKey>>;
  promise?: PromiseLike<void>;
  cancel: boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface SelectionChangedInfo<TRowData = any, TKey = any> {
  readonly currentSelectedRowKeys: Array<TKey>;
  readonly currentDeselectedRowKeys: Array<TKey>;
  readonly selectedRowKeys: Array<TKey>;
  readonly selectedRowsData: Array<TRowData>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ToolbarPreparingInfo {
  toolbarOptions: dxToolbarOptions;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowDraggingEventInfo<T extends GridBase<TRowData, TKey>, TRowData = any, TKey = any> {
  readonly component: T;
  readonly event: DxEvent<PointerEvent | MouseEvent | TouchEvent>;
  readonly itemData?: TRowData;
  readonly itemElement: DxElement;
  readonly fromIndex: number;
  readonly toIndex: number;
  readonly fromComponent: dxSortable | dxDraggable;
  readonly toComponent: dxSortable | dxDraggable;
  readonly fromData?: any;
  readonly toData?: any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DragStartEventInfo<T extends GridBase<TRowData, TKey>, TRowData = any, TKey = any> {
  readonly component: T;
  readonly event: DxEvent<PointerEvent | MouseEvent | TouchEvent>;
  itemData?: TRowData;
  readonly itemElement: DxElement;
  readonly fromIndex: number;
  readonly fromData?: any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DragDropInfo {
  readonly dropInsideItem: boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DragReorderInfo {
  readonly dropInsideItem: boolean;
  promise?: PromiseLike<void>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowDraggingTemplateDataModel<TRowData = any> {
  readonly itemData: TRowData;
  readonly itemElement: DxElement;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface FilterPanelCustomizeTextArg<T> {
  readonly component: T;
  readonly filterValue: any;
  readonly text: string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface FilterPanel<T extends GridBase<TRowData, TKey>, TRowData = any, TKey = any> {
  /**
   * Customizes the filter expression&apos;s text representation.
   */
  customizeText?: ((e: FilterPanelCustomizeTextArg<T>) => string);
  /**
   * Specifies whether the filter expression is applied.
   */
  filterEnabled?: boolean;
  /**
   * Specifies texts for the filter panel&apos;s elements.
   */
  texts?: FilterPanelTexts;
  /**
   * Specifies whether the filter panel is visible.
   */
  visible?: boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RowDragging<T extends GridBase<TRowData, TKey>, TRowData = any, TKey = any> {
    /**
     * Allows users to drop a row inside another row.
     */
    allowDropInsideItem?: boolean;
    /**
     * Allows row reordering using drag and drop gestures.
     */
    allowReordering?: boolean;
    /**
     * Enables automatic scrolling while dragging a row beyond the viewport.
     */
    autoScroll?: boolean;
    /**
     * Specifies a DOM element that limits the dragging area.
     */
    boundary?: string | UserDefinedElement;
    /**
     * Specifies a custom container in which the draggable row should be rendered.
     */
    container?: string | UserDefinedElement;
    /**
     * Specifies the cursor offset from the dragged row.
     */
    cursorOffset?: string | {
      /**
       * Specifies the horizontal cursor offset from the dragged row in pixels.
       */
      x?: number;
      /**
       * Specifies the vertical cursor offset from the dragged row in pixels.
       */
      y?: number;
    };
    /**
     * A container for custom data.
     */
    data?: any;
    /**
     * Specifies the directions in which a row can be dragged.
     */
    dragDirection?: 'both' | 'horizontal' | 'vertical';
    /**
     * Specifies custom markup to be shown instead of the item being dragged.
     */
    dragTemplate?: template | ((dragInfo: RowDraggingTemplateData<TRowData>, containerElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies how to highlight the row&apos;s drop position.
     */
    dropFeedbackMode?: 'push' | 'indicate';
    /**
     * Specifies a CSS selector for draggable rows.
     */
    filter?: string;
    /**
     * Allows you to group several UI components so that users can drag and drop rows between them.
     */
    group?: string;
    /**
     * Specifies a CSS selector (ID or class) for the element(s) that should act as the drag handle(s).
     */
    handle?: string;
    /**
     * A function that is called when a new row is added.
     */
    onAdd?: ((e: RowDraggingEventInfo<T, TRowData, TKey> & DragDropInfo) => void);
    /**
     * A function that is called when the dragged row&apos;s position is changed.
     */
    onDragChange?: ((e: Cancelable & RowDraggingEventInfo<T, TRowData, TKey> & DragDropInfo) => void);
    /**
     * A function that is called when the drag gesture is finished.
     */
    onDragEnd?: ((e: Cancelable & RowDraggingEventInfo<T, TRowData, TKey> & DragDropInfo) => void);
    /**
     * A function that is called every time a draggable row is moved.
     */
    onDragMove?: ((e: Cancelable & RowDraggingEventInfo<T, TRowData, TKey> & DragDropInfo) => void);
    /**
     * A function that is called when the drag gesture is initialized.
     */
    onDragStart?: ((e: Cancelable & DragStartEventInfo<T, TRowData, TKey>) => void);
    /**
     * A function that is called when a draggable row is removed.
     */
    onRemove?: ((e: RowDraggingEventInfo<T, TRowData, TKey>) => void);
    /**
     * A function that is called when the draggable rows are reordered.
     */
    onReorder?: ((e: RowDraggingEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey> & DragReorderInfo) => void);
    /**
     * Specifies the distance in pixels from the edge of viewport at which scrolling should start. Applies only if autoScroll is true.
     */
    scrollSensitivity?: number;
    /**
     * Specifies the scrolling speed when dragging a row beyond the viewport. Applies only if autoScroll is true.
     */
    scrollSpeed?: number;
    /**
     * Shows or hides row dragging icons.
     */
    showDragIcons?: boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface GridBaseOptions<TComponent extends GridBase<TRowData, TKey>, TRowData = any, TKey = any> extends WidgetOptions<TComponent> {
    /**
     * Specifies whether a user can reorder columns.
     */
    allowColumnReordering?: boolean;
    /**
     * Specifies whether a user can resize columns.
     */
    allowColumnResizing?: boolean;
    /**
     * Automatically scrolls to the focused row when the focusedRowKey is changed.
     */
    autoNavigateToFocusedRow?: boolean;
    /**
     * Specifies whether data should be cached.
     */
    cacheEnabled?: boolean;
    /**
     * Enables a hint that appears when a user hovers the mouse pointer over a cell with truncated content.
     */
    cellHintEnabled?: boolean;
    /**
     * Specifies whether columns should adjust their widths to the content.
     */
    columnAutoWidth?: boolean;
    /**
     * Configures the column chooser.
     */
    columnChooser?: ColumnChooser;
    /**
     * Configures column fixing.
     */
    columnFixing?: ColumnFixing;
    /**
     * Specifies whether the UI component should hide columns to adapt to the screen or container size. Ignored if allowColumnResizing is true and columnResizingMode is &apos;widget&apos;.
     */
    columnHidingEnabled?: boolean;
    /**
     * Specifies the minimum width of columns.
     */
    columnMinWidth?: number;
    /**
     * Specifies how the UI component resizes columns. Applies only if allowColumnResizing is true.
     */
    columnResizingMode?: 'nextColumn' | 'widget';
    /**
     * Specifies the width for all data columns. Has a lower priority than the column.width property.
     */
    columnWidth?: number | 'auto';
    /**
     * Overridden.
     */
    columns?: Array<ColumnBase<TRowData> | string>;
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TRowData, TKey>;
    /**
     * Specifies the format in which date-time values should be sent to the server.
     */
    dateSerializationFormat?: string;
    /**
     * Overriden.
     */
    editing?: EditingBase<TRowData, TKey>;
    /**
     * Indicates whether to show the error row.
     */
    errorRowEnabled?: boolean;
    /**
     * Configures the integrated filter builder.
     */
    filterBuilder?: dxFilterBuilderOptions;
    /**
     * Configures the popup in which the integrated filter builder is shown.
     */
    filterBuilderPopup?: PopupProperties;
    /**
     * Configures the filter panel.
     */
    filterPanel?: FilterPanel<TComponent, TRowData, TKey>;
    /**
     * Configures the filter row.
     */
    filterRow?: FilterRow;
    /**
     * Specifies whether to synchronize the filter row, header filter, and filter builder. The synchronized filter expression is stored in the filterValue property.
     */
    filterSyncEnabled?: boolean | 'auto';
    /**
     * Specifies a filter expression.
     */
    filterValue?: string | Array<any> | Function;
    /**
     * The index of the column that contains the focused data cell. This index is taken from the columns array.
     */
    focusedColumnIndex?: number;
    /**
     * Specifies whether the focused row feature is enabled.
     */
    focusedRowEnabled?: boolean;
    /**
     * Specifies or indicates the focused data row&apos;s index.
     */
    focusedRowIndex?: number;
    /**
     * Specifies initially or currently focused grid row&apos;s key.
     */
    focusedRowKey?: TKey;
    /**
     * Configures the header filter feature.
     */
    headerFilter?: HeaderFilter;
    /**
     * Specifies whether to highlight rows and cells with edited data. repaintChangesOnly should be true.
     */
    highlightChanges?: boolean;
    /**
     * Configures keyboard navigation.
     */
    keyboardNavigation?: KeyboardNavigation;
    /**
     * Configures the load panel.
     */
    loadPanel?: LoadPanel;
    /**
     * Specifies a text string shown when the widget does not display any data.
     */
    noDataText?: string;
    /**
     * A function that is executed before an adaptive detail row is rendered.
     */
    onAdaptiveDetailRowPreparing?: ((e: EventInfo<TComponent> & AdaptiveDetailRowPreparingInfo) => void);
    /**
     * A function that is executed when an error occurs in the data source.
     */
    onDataErrorOccurred?: ((e: EventInfo<TComponent> & DataErrorOccurredInfo) => void);
    /**
     * A function that is executed after row changes are discarded.
     */
    onEditCanceled?: ((e: EventInfo<TComponent> & DataChangeInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed when the edit operation is canceled, but row changes are not yet discarded.
     */
    onEditCanceling?: ((e: Cancelable & EventInfo<TComponent> & DataChangeInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed before a new row is added to the UI component.
     */
    onInitNewRow?: ((e: EventInfo<TComponent> & NewRowInfo<TRowData>) => void);
    /**
     * A function that is executed when the UI component is in focus and a key has been pressed down.
     */
    onKeyDown?: ((e: NativeEventInfo<TComponent, KeyboardEvent> & KeyDownInfo) => void);
    /**
     * A function that is executed after a row is collapsed.
     */
    onRowCollapsed?: ((e: EventInfo<TComponent> & RowKeyInfo<TKey>) => void);
    /**
     * A function that is executed before a row is collapsed.
     */
    onRowCollapsing?: ((e: Cancelable & EventInfo<TComponent> & RowKeyInfo<TKey>) => void);
    /**
     * A function that is executed after a row is expanded.
     */
    onRowExpanded?: ((e: EventInfo<TComponent> & RowKeyInfo<TKey>) => void);
    /**
     * A function that is executed before a row is expanded.
     */
    onRowExpanding?: ((e: Cancelable & EventInfo<TComponent> & RowKeyInfo<TKey>) => void);
    /**
     * A function that is executed after a new row has been inserted into the data source.
     */
    onRowInserted?: ((e: EventInfo<TComponent> & RowInsertedInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed before a new row is inserted into the data source.
     */
    onRowInserting?: ((e: EventInfo<TComponent> & RowInsertingInfo<TRowData>) => void);
    /**
     * A function that is executed after a row has been removed from the data source.
     */
    onRowRemoved?: ((e: EventInfo<TComponent> & RowRemovedInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed before a row is removed from the data source.
     */
    onRowRemoving?: ((e: EventInfo<TComponent> & RowRemovingInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed after a row has been updated in the data source.
     */
    onRowUpdated?: ((e: EventInfo<TComponent> & RowUpdatedInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed before a row is updated in the data source.
     */
    onRowUpdating?: ((e: EventInfo<TComponent> & RowUpdatingInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed after cells in a row are validated against validation rules.
     */
    onRowValidating?: ((e: EventInfo<TComponent> & RowValidatingInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed after row changes are saved.
     */
    onSaved?: ((e: EventInfo<TComponent> & DataChangeInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed before pending row changes are saved.
     */
    onSaving?: ((e: EventInfo<TComponent> & SavingInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed after selecting a row or clearing its selection.
     */
    onSelectionChanged?: ((e: EventInfo<TComponent> & SelectionChangedInfo<TRowData, TKey>) => void);
    /**
     * A function that is executed before the toolbar is created.
     * @deprecated The onToolbarPreparing property is deprecated. Use the toolbar property instead.
     */
    onToolbarPreparing?: ((e: EventInfo<TComponent> & ToolbarPreparingInfo) => void);
    /**
     * Configures the pager.
     */
    pager?: Pager;
    /**
     * Configures paging.
     */
    paging?: PagingBase;
    /**
     * Specifies whether to render the filter row, command columns, and columns with showEditorAlways set to true after other elements.
     */
    renderAsync?: boolean;
    /**
     * Specifies whether to repaint only those cells whose data changed.
     */
    repaintChangesOnly?: boolean;
    /**
     * Specifies whether rows should be shaded differently.
     */
    rowAlternationEnabled?: boolean;
    /**
     * Configures row reordering using drag and drop gestures.
     */
    rowDragging?: RowDragging<TComponent, TRowData, TKey>;
    /**
     * 
     */
    scrolling?: ScrollingBase;
    /**
     * Configures the search panel.
     */
    searchPanel?: SearchPanel;
    /**
     * Allows you to select rows or determine which rows are selected.
     */
    selectedRowKeys?: Array<TKey>;
    /**
     * 
     */
    selection?: SelectionBase;
    /**
     * Specifies whether the outer borders of the UI component are visible.
     */
    showBorders?: boolean;
    /**
     * Specifies whether column headers are visible.
     */
    showColumnHeaders?: boolean;
    /**
     * Specifies whether vertical lines that separate one column from another are visible.
     */
    showColumnLines?: boolean;
    /**
     * Specifies whether horizontal lines that separate one row from another are visible.
     */
    showRowLines?: boolean;
    /**
     * Configures runtime sorting.
     */
    sorting?: Sorting;
    /**
     * Configures state storing.
     */
    stateStoring?: StateStoring;
    /**
     * Specifies whether to enable two-way data binding.
     */
    twoWayBindingEnabled?: boolean;
    /**
     * Specifies whether text that does not fit into a column should be wrapped.
     */
    wordWrapEnabled?: boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnChooser {
    /**
     * Specifies whether searching is enabled in the column chooser.
     */
    allowSearch?: boolean;
    /**
     * Specifies text displayed by the column chooser when it is empty.
     */
    emptyPanelText?: string;
    /**
     * Specifies whether a user can open the column chooser.
     */
    enabled?: boolean;
    /**
     * Specifies the height of the column chooser.
     */
    height?: number;
    /**
     * Specifies how a user manages columns using the column chooser.
     */
    mode?: 'dragAndDrop' | 'select';
    /**
     * Specifies a delay in milliseconds between when a user finishes typing in the column chooser&apos;s search panel, and when the search is executed.
     */
    searchTimeout?: number;
    /**
     * Specifies the title of the column chooser.
     */
    title?: string;
    /**
     * Specifies the width of the column chooser.
     */
    width?: number;
    /**
     * Specifies the sort order of column headers.
     */
    sortOrder?: 'asc' | 'desc';
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnFixing {
    /**
     * Enables column fixing.
     */
    enabled?: boolean;
    /**
     * Contains properties that specify texts for column fixing commands in the context menu of a column header.
     */
    texts?: ColumnFixingTexts;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnFixingTexts {
    /**
     * Specifies text for the context menu item that fixes a column.
     */
    fix?: string;
    /**
     * Specifies text for the context menu subitem that fixes a column to the left edge of the UI component.
     */
    leftPosition?: string;
    /**
     * Specifies text for the context menu subitem that fixes a column to the right edge of the UI component.
     */
    rightPosition?: string;
    /**
     * Specifies text for the context menu item that unfixes a column.
     */
    unfix?: string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface FilterPanelTexts {
    /**
     * The text of the &apos;Clear&apos; link.
     */
    clearFilter?: string;
    /**
     * The text of the &apos;Create Filter&apos; link.
     */
    createFilter?: string;
    /**
     * The hint of the checkbox that applies the filter.
     */
    filterEnabledHint?: string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface FilterRow {
    /**
     * Specifies when to apply a filter.
     */
    applyFilter?: 'auto' | 'onClick';
    /**
     * Specifies text for a hint that appears when a user pauses on a button that applies the filter.
     */
    applyFilterText?: string;
    /**
     * Specifies a placeholder for the editor that specifies the end of a range when a user selects the &apos;between&apos; filter operation.
     */
    betweenEndText?: string;
    /**
     * Specifies a placeholder for the editor that specifies the start of a range when a user selects the &apos;between&apos; filter operation.
     */
    betweenStartText?: string;
    /**
     * Specifies descriptions for filter operations on the filter list.
     */
    operationDescriptions?: FilterRowOperationDescriptions;
    /**
     * Specifies text for the reset operation on the filter list.
     */
    resetOperationText?: string;
    /**
     * Specifies text for the item that clears the applied filter. Used only when a cell of the filter row contains a select box.
     */
    showAllText?: string;
    /**
     * Specifies whether icons that open the filter lists are visible.
     */
    showOperationChooser?: boolean;
    /**
     * Specifies whether the filter row is visible.
     */
    visible?: boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface FilterRowOperationDescriptions {
    /**
     * A description for the &apos;between&apos; operation.
     */
    between?: string;
    /**
     * A description for the &apos;contains&apos; operation.
     */
    contains?: string;
    /**
     * A description for the &apos;endswith&apos; operation.
     */
    endsWith?: string;
    /**
     * A description for the &apos;=&apos; operation.
     */
    equal?: string;
    /**
     * A description for the &apos;&gt;&apos; operation.
     */
    greaterThan?: string;
    /**
     * A description for the &apos;&gt;=&apos; operation.
     */
    greaterThanOrEqual?: string;
    /**
     * A description for the &apos;&lt;&apos; operation.
     */
    lessThan?: string;
    /**
     * A description for the &apos;&lt;=&apos; operation.
     */
    lessThanOrEqual?: string;
    /**
     * A description for the &apos;notcontains&apos; operation.
     */
    notContains?: string;
    /**
     * A description for the &apos;&lt;&gt;&apos; operation.
     */
    notEqual?: string;
    /**
     * A description for the &apos;startswith&apos; operation.
     */
    startsWith?: string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface HeaderFilter {
    /**
     * Specifies whether searching is enabled in the header filter.
     */
    allowSearch?: boolean;
    /**
     * Specifies the height of the popup menu that contains values for filtering.
     */
    height?: number;
    /**
     * Specifies a delay in milliseconds between when a user finishes typing in the header filter&apos;s search panel, and when the search is executed.
     */
    searchTimeout?: number;
    /**
     * Contains properties that specify text for various elements of the popup menu.
     */
    texts?: HeaderFilterTexts;
    /**
     * Specifies whether header filter icons are visible.
     */
    visible?: boolean;
    /**
     * Specifies the width of the popup menu that contains values for filtering.
     */
    width?: number;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface HeaderFilterTexts {
    /**
     * Specifies text for the button that closes the popup menu without applying a filter.
     */
    cancel?: string;
    /**
     * Specifies a name for the item that represents empty values in the popup menu.
     */
    emptyValue?: string;
    /**
     * Specifies text for the button that applies the specified filter.
     */
    ok?: string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface KeyboardNavigation {
    /**
     * Specifies whether users can enter a new cell value on a key press. Applies if editing.mode is &apos;cell&apos; or &apos;batch&apos;.
     */
    editOnKeyPress?: boolean;
    /**
     * Enables keyboard navigation.
     */
    enabled?: boolean;
    /**
     * Specifies whether the Enter key switches the cell or row to the edit state or moves focus in the enterKeyDirection. Applies for all edit modes, except &apos;popup&apos;.
     */
    enterKeyAction?: 'startEdit' | 'moveFocus';
    /**
     * Specifies the direction in which to move focus when a user presses Enter. Applies if editing.mode is &apos;cell&apos; or &apos;batch&apos;.
     */
    enterKeyDirection?: 'none' | 'column' | 'row';
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface LoadPanel {
    /**
     * Enables displaying the load panel automatically.
     */
    enabled?: boolean | 'auto';
    /**
     * Specifies the height of the load panel in pixels.
     */
    height?: number;
    /**
     * Specifies a URL pointing to an image to be used as a loading indicator.
     */
    indicatorSrc?: string;
    /**
     * Specifies whether to shade the UI component when the load panel is shown.
     */
    shading?: boolean;
    /**
     * Specifies the shading color. Applies only if shading is true.
     */
    shadingColor?: string;
    /**
     * Specifies whether to show the loading indicator.
     */
    showIndicator?: boolean;
    /**
     * Specifies whether to show the pane of the load panel.
     */
    showPane?: boolean;
    /**
     * Specifies text displayed on the load panel.
     */
    text?: string;
    /**
     * Specifies the width of the load panel in pixels.
     */
    width?: number;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface Pager {
    /**
     * Specifies the available page sizes in the page size selector.
     */
    allowedPageSizes?: Array<(number | 'all')> | 'auto';
    /**
     * Specifies the pager&apos;s display mode.
     */
    displayMode?: 'adaptive' | 'compact' | 'full';
    /**
     * Specifies the page information text.
     */
    infoText?: string;
    /**
     * Specifies whether to show the page information.
     */
    showInfo?: boolean;
    /**
     * Specifies whether to show navigation buttons.
     */
    showNavigationButtons?: boolean;
    /**
     * Specifies whether to show the page size selector.
     */
    showPageSizeSelector?: boolean;
    /**
     * Specifies whether the pager is visible.
     */
    visible?: boolean | 'auto';
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface SearchPanel {
    /**
     * Notifies the UI component whether search is case-sensitive to ensure that search results are highlighted correctly. Applies only if highlightSearchText is true.
     */
    highlightCaseSensitive?: boolean;
    /**
     * Specifies whether found substrings should be highlighted.
     */
    highlightSearchText?: boolean;
    /**
     * Specifies a placeholder for the search panel.
     */
    placeholder?: string;
    /**
     * Specifies whether the UI component should search against all columns or only visible ones.
     */
    searchVisibleColumnsOnly?: boolean;
    /**
     * Sets a search string for the search panel.
     */
    text?: string;
    /**
     * Specifies whether the search panel is visible or not.
     */
    visible?: boolean;
    /**
     * Specifies the width of the search panel in pixels.
     */
    width?: number;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface Sorting {
    /**
     * Specifies text for the context menu item that sets an ascending sort order in a column.
     */
    ascendingText?: string;
    /**
     * Specifies text for the context menu item that clears sorting settings for a column.
     */
    clearText?: string;
    /**
     * Specifies text for the context menu item that sets a descending sort order in a column.
     */
    descendingText?: string;
    /**
     * Specifies the sorting mode.
     */
    mode?: 'multiple' | 'none' | 'single';
    /**
     * Specifies whether to display sort indexes in column headers. Applies only when sorting.mode is &apos;multiple&apos; and data is sorted by two or more columns.
     */
    showSortIndexes?: boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface StateStoring {
    /**
     * Specifies a function that is executed on state loading. Applies only if the type is &apos;custom&apos;.
     */
    customLoad?: (() => PromiseLike<any>);
    /**
     * Specifies a function that is executed on state change. Applies only if the type is &apos;custom&apos;.
     */
    customSave?: ((gridState: any) => any);
    /**
     * Enables state storing.
     */
    enabled?: boolean;
    /**
     * Specifies the delay in milliseconds between when a user makes a change and when this change is saved.
     */
    savingTimeout?: number;
    /**
     * Specifies the key for storing the UI component state.
     */
    storageKey?: string;
    /**
     * Specifies the type of storage where the state is saved.
     */
    type?: 'custom' | 'localStorage' | 'sessionStorage';
}

/**
 * @deprecated 
 */
export type GridBaseEditing<TRowData = any, TKey = any> = EditingBase<TRowData, TKey>;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface EditingBase<TRowData = any, TKey = any> {
    /**
     * Specifies if confirmation is required when a user deletes a row.
     */
    confirmDelete?: boolean;
    /**
     * An array of pending row changes.
     */
    changes?: Array<DataChange<TRowData, TKey>>;
    /**
     * The name of a column being edited. Applies only if editing.mode is &apos;cell&apos; or &apos;batch&apos;.
     */
    editColumnName?: string;
    /**
     * The key(s) of a row being edited.
     */
    editRowKey?: TKey;
    /**
     * Configures the form. Used only if editing.mode is &apos;form&apos; or &apos;popup&apos;.
     */
    form?: dxFormOptions;
    /**
     * Specifies how a user edits data.
     */
    mode?: 'batch' | 'cell' | 'row' | 'form' | 'popup';
    /**
     * Configures the popup. Used only if editing.mode is &apos;popup&apos;.
     */
    popup?: PopupProperties;
    /**
     * Specifies operations that are performed after saving changes.
     */
    refreshMode?: 'full' | 'reshape' | 'repaint';
    /**
     * Specifies whether to select text in a cell when a user starts editing.
     */
    selectTextOnEditStart?: boolean;
    /**
     * Specifies whether a single or double click should switch a cell to the editing state. Applies if editing.mode is &apos;cell&apos; or &apos;batch&apos;.
     */
    startEditAction?: 'click' | 'dblClick';
    /**
     * Contains properties that specify texts for editing-related UI elements.
     */
    texts?: EditingTextsBase;
    /**
     * Specifies whether the edit column uses icons instead of links.
     */
    useIcons?: boolean;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DataChange<TRowData = any, TKey = any> {
    /**
     * The key of the row being updated or removed.
     */
    key: TKey;
    /**
     * Data change type.
     */
    type: 'insert' | 'update' | 'remove';
    /**
     * An object with updated row fields.
     */
    data: DeepPartial<TRowData>;
    /**
     * A key that identifies a record after which a new record should be inserted. Applies only if the type is &apos;insert&apos;.
     */
    insertAfterKey?: TKey;
    /**
     * A key that identifies the record before which a new record should be inserted. Applies only if the type is &apos;insert&apos;.
     */
    insertBeforeKey?: TKey;
}

/**
 * @deprecated 
 */
export type GridBaseEditingTexts = EditingTextsBase;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface EditingTextsBase {
    /**
     * Specifies text for a hint that appears when a user pauses on the global &apos;Add&apos; button. Applies only if editing.allowAdding is true.
     */
    addRow?: string;
    /**
     * Specifies text for a hint that appears when a user pauses on the &apos;Discard&apos; button. Applies only if editing.mode is &apos;batch&apos;.
     */
    cancelAllChanges?: string;
    /**
     * Specifies text for a button that cancels changes in a row. Applies only if editing.allowUpdating is true and editing.mode is &apos;row&apos;.
     */
    cancelRowChanges?: string;
    /**
     * Specifies a message that prompts a user to confirm deletion.
     */
    confirmDeleteMessage?: string;
    /**
     * Specifies a title for the window that asks a user to confirm deletion.
     */
    confirmDeleteTitle?: string;
    /**
     * Specifies text for buttons that delete rows. Applies only if allowDeleting is true.
     */
    deleteRow?: string;
    /**
     * Specifies text for buttons that switch rows into the editing state. Applies only if allowUpdating is true.
     */
    editRow?: string;
    /**
     * Specifies text for a hint that appears when a user pauses on the global &apos;Save&apos; button. Applies only if editing.mode is &apos;batch&apos;.
     */
    saveAllChanges?: string;
    /**
     * Specifies text for a button that saves changes made in a row. Applies only if allowUpdating is true.
     */
    saveRowChanges?: string;
    /**
     * Specifies text for buttons that recover deleted rows. Applies only if allowDeleting is true and editing.mode is &apos;batch&apos;.
     */
    undeleteRow?: string;
    /**
     * Specifies text for a hint appearing when a user pauses on the button that cancels changes in a cell. Applies only if editing.mode is &apos;cell&apos; and data validation is enabled.
     */
    validationCancelChanges?: string;
}

/**
 * @deprecated 
 */
export type GridBasePaging = PagingBase;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface PagingBase {
    /**
     * Enables paging.
     */
    enabled?: boolean;
    /**
     * Specifies the page to be displayed using a zero-based index.
     */
    pageIndex?: number;
    /**
     * Specifies the page size.
     */
    pageSize?: number;
}

/**
 * @deprecated 
 */
export type GridBaseScrolling = ScrollingBase;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ScrollingBase {
    /**
     * Specifies the rendering mode for columns. Applies when columns are left outside the viewport. Requires the columnWidth, columnAutoWidth, or width (for all columns) property specified.
     */
    columnRenderingMode?: 'standard' | 'virtual';
    /**
     * Specifies whether the UI component should load adjacent pages. Applies only if scrolling.mode is &apos;virtual&apos; or &apos;infinite&apos;.
     */
    preloadEnabled?: boolean;
    /**
     * Specifies the rendering mode for loaded rows.
     */
    rowRenderingMode?: 'standard' | 'virtual';
    /**
     * Specifies whether a user can scroll the content with a swipe gesture. Applies only if useNative is false.
     */
    scrollByContent?: boolean;
    /**
     * Specifies whether a user can scroll the content with the scrollbar. Applies only if useNative is false.
     */
    scrollByThumb?: boolean;
    /**
     * Specifies when to show scrollbars. Applies only if useNative is false.
     */
    showScrollbar?: 'always' | 'never' | 'onHover' | 'onScroll';
    /**
     * Specifies whether the widget should use native or simulated scrolling.
     */
    useNative?: boolean | 'auto';
    /**
     * Specifies whether to render rows after a user stops scrolling or at the same time as the user scrolls the widget.
     */
    renderAsync?: boolean;
}

/**
 * @deprecated 
 */
export type GridBaseSelection = SelectionBase;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface SelectionBase {
    /**
     * Allows users to simultaneously select all or current page rows (depending on the selectAllMode).
     */
    allowSelectAll?: boolean;
    /**
     * Specifies the selection mode.
     */
    mode?: 'multiple' | 'none' | 'single';
}

/**
 * The base class for UI components.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface GridBase<TRowData = any, TKey = any> {
    /**
     * Shows the load panel.
     */
    beginCustomLoading(messageText: string): void;
    /**
     * Gets a data object with a specific key.
     */
    byKey(key: TKey): DxPromise<TRowData>;
    /**
     * Discards changes that a user made to data.
     */
    cancelEditData(): void;
    /**
     * Gets the value of a cell with a specific row index and a data field, column caption or name.
     */
    cellValue(rowIndex: number, dataField: string): any;
    /**
     * Sets a new value to a cell with a specific row index and a data field, column caption or name.
     */
    cellValue(rowIndex: number, dataField: string, value: any): void;
    /**
     * Gets the value of a cell with specific row and column indexes.
     */
    cellValue(rowIndex: number, visibleColumnIndex: number): any;
    /**
     * Sets a new value to a cell with specific row and column indexes.
     */
    cellValue(rowIndex: number, visibleColumnIndex: number, value: any): void;
    /**
     * Clears all filters applied to UI component rows.
     */
    clearFilter(): void;
    /**
     * Clears all row filters of a specific type.
     */
    clearFilter(filterName: string): void;
    /**
     * Clears selection of all rows on all pages.
     */
    clearSelection(): void;
    /**
     * Clears sorting settings of all columns at once.
     */
    clearSorting(): void;
    /**
     * Switches the cell being edited back to the normal state. Takes effect only if editing.mode is batch or cell and showEditorAlways is false.
     */
    closeEditCell(): void;
    /**
     * Collapses the currently expanded adaptive detail row (if there is one).
     */
    collapseAdaptiveDetailRow(): void;
    /**
     * Gets the data column count. Includes visible and hidden columns, excludes command columns.
     */
    columnCount(): number;
    /**
     * Gets all properties of a column with a specific identifier.
     */
    columnOption(id: number | string): any;
    /**
     * Gets the value of a single column property.
     */
    columnOption(id: number | string, optionName: string): any;
    /**
     * Updates the value of a single column property.
     */
    columnOption(id: number | string, optionName: string, optionValue: any): void;
    /**
     * Updates the values of several column properties.
     */
    columnOption(id: number | string, options: any): void;
    /**
     * Removes a column.
     */
    deleteColumn(id: number | string): void;
    /**
     * Removes a row with a specific index.
     */
    deleteRow(rowIndex: number): void;
    /**
     * Clears the selection of all rows on all pages or the currently rendered page only.
     */
    deselectAll(): DxPromise<void>;
    /**
     * Cancels the selection of rows with specific keys.
     */
    deselectRows(keys: Array<any>): DxPromise<any>;
    /**
     * Switches a cell with a specific row index and a data field to the editing state. Takes effect only if the editing mode is &apos;batch&apos; or &apos;cell&apos;.
     */
    editCell(rowIndex: number, dataField: string): void;
    /**
     * Switches a cell with specific row and column indexes to the editing state. Takes effect only if the editing mode is &apos;batch&apos; or &apos;cell&apos;.
     */
    editCell(rowIndex: number, visibleColumnIndex: number): void;
    /**
     * Switches a row with a specific index to the editing state. Takes effect only if the editing mode is &apos;row&apos;, &apos;popup&apos; or &apos;form&apos;.
     */
    editRow(rowIndex: number): void;
    /**
     * Hides the load panel.
     */
    endCustomLoading(): void;
    /**
     * Expands an adaptive detail row.
     */
    expandAdaptiveDetailRow(key: TKey): void;
    /**
     * Gets a filter expression applied to the UI component&apos;s data source using the filter(filterExpr) method and the DataSource&apos;s filter property.
     */
    filter(): any;
    /**
     * Applies a filter to the dataSource.
     */
    filter(filterExpr: any): void;
    focus(): void;
    /**
     * Sets focus on a specific cell.
     */
    focus(element: UserDefinedElement): void;
    /**
     * Gets a cell with a specific row index and a data field, column caption or name.
     */
    getCellElement(rowIndex: number, dataField: string): DxElement | undefined;
    /**
     * Gets a cell with specific row and column indexes.
     */
    getCellElement(rowIndex: number, visibleColumnIndex: number): DxElement | undefined;
    /**
     * Gets the total filter that combines all the filters applied.
     */
    getCombinedFilter(): any;
    /**
     * Gets the total filter that combines all the filters applied.
     */
    getCombinedFilter(returnDataField: boolean): any;
    getDataSource(): DataSource<TRowData, TKey>;
    /**
     * Gets the key of a row with a specific index.
     */
    getKeyByRowIndex(rowIndex: number): TKey | undefined;
    /**
     * Gets the container of a row with a specific index.
     */
    getRowElement(rowIndex: number): UserDefinedElementsArray | undefined;
    /**
     * Gets the index of a row with a specific key.
     */
    getRowIndexByKey(key: TKey): number;
    /**
     * Gets the instance of the UI component&apos;s scrollable part.
     */
    getScrollable(): Scrollable;
    /**
     * Gets the index of a visible column.
     */
    getVisibleColumnIndex(id: number | string): number;
    /**
     * Checks whether the UI component has unsaved changes.
     */
    hasEditData(): boolean;
    /**
     * Hides the column chooser.
     */
    hideColumnChooser(): void;
    /**
     * Checks whether an adaptive detail row is expanded or collapsed.
     */
    isAdaptiveDetailRowExpanded(key: TKey): boolean;
    /**
     * Checks whether a row with a specific key is focused.
     */
    isRowFocused(key: TKey): boolean;
    /**
     * Checks whether a row with a specific key is selected.
     */
    isRowSelected(key: TKey): boolean;
    /**
     * Gets a data object&apos;s key.
     */
    keyOf(obj: TRowData): TKey;
    /**
     * Navigates to a row with the specified key.
     */
    navigateToRow(key: TKey): DxPromise<void>;
    /**
     * Gets the total page count.
     */
    pageCount(): number;
    /**
     * Gets the current page index.
     */
    pageIndex(): number;
    /**
     * Switches the UI component to a specific page using a zero-based index.
     */
    pageIndex(newIndex: number): DxPromise<void>;
    /**
     * Gets the current page size.
     */
    pageSize(): number;
    /**
     * Sets the page size.
     */
    pageSize(value: number): void;
    /**
     * Reloads data and repaints data rows.
     */
    refresh(): DxPromise<void>;
    /**
     * Reloads data and repaints all or only updated data rows.
     */
    refresh(changesOnly: boolean): DxPromise<void>;
    /**
     * Repaints specific rows.
     */
    repaintRows(rowIndexes: Array<number>): void;
    /**
     * Saves changes that a user made to data.
     */
    saveEditData(): DxPromise<void>;
    /**
     * Seeks a search string in the columns whose allowSearch property is true.
     */
    searchByText(text: string): void;
    /**
     * Selects all rows.
     */
    selectAll(): DxPromise<void>;
    /**
     * Selects rows with specific keys.
     */
    selectRows(keys: Array<TKey>, preserve: boolean): DxPromise<Array<TRowData>>;
    /**
     * Selects rows with specific indexes.
     */
    selectRowsByIndexes(indexes: Array<number>): DxPromise<Array<TRowData>>;
    /**
     * Shows the column chooser.
     */
    showColumnChooser(): void;
    /**
     * Gets the current UI component state.
     */
    state(): any;
    /**
     * Sets the UI component state.
     */
    state(state: any): void;
    /**
     * Recovers a row deleted in batch editing mode.
     */
    undeleteRow(rowIndex: number): void;
    /**
     * Updates the UI component&apos;s content after resizing.
     */
    updateDimensions(): void;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnCustomizeTextArg {
  value?: string | number | Date;
  valueText?: string;
  target?: string;
  groupInterval?: string | number;
}

/**
 * @deprecated 
 */
export type GridBaseColumn<TRowData = any> = ColumnBase<TRowData>;
/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnBase<TRowData = any> {
    /**
     * Aligns the content of the column.
     */
    alignment?: 'center' | 'left' | 'right';
    /**
     * Specifies whether a user can edit values in the column at runtime. By default, inherits the value of the editing.allowUpdating property.
     */
    allowEditing?: boolean;
    /**
     * Specifies whether data can be filtered by this column. Applies only if filterRow.visible is true.
     */
    allowFiltering?: boolean;
    /**
     * Specifies whether a user can fix the column at runtime. Applies only if columnFixing.enabled is true.
     */
    allowFixing?: boolean;
    /**
     * Specifies whether the header filter can be used to filter data by this column. Applies only if headerFilter.visible is true. By default, inherits the value of the allowFiltering property.
     */
    allowHeaderFiltering?: boolean;
    /**
     * Specifies whether a user can hide the column using the column chooser at runtime. Applies only if columnChooser.enabled is true.
     */
    allowHiding?: boolean;
    /**
     * Specifies whether this column can be used in column reordering at runtime. Applies only if allowColumnReordering is true.
     */
    allowReordering?: boolean;
    /**
     * Specifies whether a user can resize the column at runtime. Applies only if allowColumnResizing is true.
     */
    allowResizing?: boolean;
    /**
     * Specifies whether this column can be searched. Applies only if searchPanel.visible is true. Inherits the value of the allowFiltering property by default.
     */
    allowSearch?: boolean;
    /**
     * Specifies whether a user can sort rows by this column at runtime. Applies only if sorting.mode differs from &apos;none&apos;.
     */
    allowSorting?: boolean;
    /**
     * Calculates custom cell values. Use this function to create an unbound data column.
     */
    calculateCellValue?: ((rowData: TRowData) => any);
    /**
     * Calculates custom display values for column cells. Requires specifying the dataField or calculateCellValue property. Used in lookup optimization.
     */
    calculateDisplayValue?: string | ((rowData: TRowData) => any);
    /**
     * Specifies the column&apos;s custom rules to filter data.
     */
    calculateFilterExpression?: ((filterValue: any, selectedFilterOperation: string, target: string) => string | Array<any> | Function);
    /**
     * Calculates custom values used to sort this column.
     */
    calculateSortValue?: string | ((rowData: TRowData) => any);
    /**
     * Specifies a caption for the column.
     */
    caption?: string;
    /**
     * Specifies a CSS class to be applied to the column.
     */
    cssClass?: string;
    /**
     * Customizes the text displayed in column cells.
     */
    customizeText?: ((cellInfo: ColumnCustomizeTextArg) => string);
    /**
     * Binds the column to a field of the dataSource.
     */
    dataField?: string;
    /**
     * Casts column values to a specific data type.
     */
    dataType?: 'string' | 'number' | 'date' | 'boolean' | 'object' | 'datetime';
    /**
     * Configures the default UI component used for editing and filtering in the filter row.
     */
    editorOptions?: any;
    /**
     * Specifies whether HTML tags are displayed as plain text or applied to the values of the column.
     */
    encodeHtml?: boolean;
    /**
     * In a boolean column, replaces all false items with a specified text. Applies only if showEditorAlways property is false.
     */
    falseText?: string;
    /**
     * Specifies available filter operations. Applies if allowFiltering is true and the filterRow and/or filterPanel are visible.
     */
    filterOperations?: Array<'=' | '<>' | '<' | '<=' | '>' | '>=' | 'contains' | 'endswith' | 'isblank' | 'isnotblank' | 'notcontains' | 'startswith' | 'between' | 'anyof' | 'noneof'>;
    /**
     * Specifies whether a user changes the current filter by including (selecting) or excluding (clearing the selection of) values. Applies only if headerFilter.visible and allowHeaderFiltering are true.
     */
    filterType?: 'exclude' | 'include';
    /**
     * Specifies the column&apos;s filter value displayed in the filter row.
     */
    filterValue?: any;
    /**
     * Specifies values selected in the column&apos;s header filter.
     */
    filterValues?: Array<any>;
    /**
     * Fixes the column.
     */
    fixed?: boolean;
    /**
     * Specifies the UI component&apos;s edge to which the column is fixed. Applies only if columns[].fixed is true.
     */
    fixedPosition?: 'left' | 'right';
    /**
     * Configures the form item that the column produces in the editing state. Applies only if editing.mode is &apos;form&apos; or &apos;popup&apos;.
     */
    formItem?: dxFormSimpleItem;
    /**
     * Formats a value before it is displayed in a column cell.
     */
    format?: Format;
    /**
     * Specifies data settings for the header filter.
     */
    headerFilter?: ColumnHeaderFilter;
    /**
     * Specifies the order in which columns are hidden when the UI component adapts to the screen or container size. Ignored if allowColumnResizing is true and columnResizingMode is &apos;widget&apos;.
     */
    hidingPriority?: number;
    /**
     * Specifies whether the column organizes other columns into bands.
     */
    isBand?: boolean;
    /**
     * Specifies properties of a lookup column.
     */
    lookup?: ColumnLookup;
    /**
     * Specifies the minimum width of the column.
     */
    minWidth?: number;
    /**
     * Specifies the column&apos;s unique identifier. If not set in code, this value is inherited from the dataField.
     */
    name?: string;
    /**
     * Specifies the band column that owns the current column. Accepts the index of the band column in the columns array.
     */
    ownerBand?: number;
    /**
     * Specifies whether to render the column after other columns and elements. Use if column cells have a complex template. Requires the width property specified.
     */
    renderAsync?: boolean;
    /**
     * Specifies a filter operation that applies when users use the filter row to filter the column.
     */
    selectedFilterOperation?: '<' | '<=' | '<>' | '=' | '>' | '>=' | 'between' | 'contains' | 'endswith' | 'notcontains' | 'startswith';
    /**
     * Specifies a function to be invoked after the user has edited a cell value, but before it will be saved in the data source.
     */
    setCellValue?: ((newData: DeepPartial<TRowData>, value: any, currentRowData: TRowData) => void | PromiseLike<void>);
    /**
     * Specifies whether the column displays its values using editors.
     */
    showEditorAlways?: boolean;
    /**
     * Specifies whether the column chooser can contain the column header.
     */
    showInColumnChooser?: boolean;
    /**
     * Specifies the index according to which columns participate in sorting.
     */
    sortIndex?: number;
    /**
     * Specifies the sort order of column values.
     */
    sortOrder?: 'asc' | 'desc';
    /**
     * Specifies a custom comparison function for sorting. Applies only when sorting is performed on the client.
     */
    sortingMethod?: ((value1: any, value2: any) => number);
    /**
     * In a boolean column, replaces all true items with a specified text. Applies only if showEditorAlways property is false.
     */
    trueText?: string;
    /**
     * Specifies validation rules to be checked when cell values are updated.
     */
    validationRules?: Array<RequiredRule | NumericRule | RangeRule | StringLengthRule | CustomRule | CompareRule | PatternRule | EmailRule | AsyncRule>;
    /**
     * Specifies whether the column is visible, that is, occupies space in the table.
     */
    visible?: boolean;
    /**
     * Specifies the position of the column regarding other columns in the resulting UI component.
     */
    visibleIndex?: number;
    /**
     * Specifies the column&apos;s width in pixels or as a percentage. Ignored if it is less than minWidth.
     */
    width?: number | string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnHeaderFilter {
  /**
   * Specifies whether searching is enabled in the header filter.
   */
  allowSearch?: boolean;
  /**
   * Specifies the header filter&apos;s data source.
   */
  dataSource?: FilterLookupDataSource<any> | ((options: { component?: any; dataSource?: DataSourceOptions }) => void);
  /**
   * Specifies how the header filter combines values into groups. Does not apply if you specify a custom header filter data source.
   */
  groupInterval?: 'day' | 'hour' | 'minute' | 'month' | 'quarter' | 'second' | 'year' | number;
  /**
   * Specifies the height of the popup menu containing filtering values.
   */
  height?: number;
  /**
   * Specifies a comparison operation used to search header filter values.
   */
  searchMode?: 'contains' | 'startswith' | 'equals';
  /**
   * Specifies the width of the popup menu containing filtering values.
   */
  width?: number;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnLookup {
  /**
   * Specifies whether to display the Clear button in lookup column cells while they are being edited.
   */
  allowClearing?: boolean;
  /**
   * Specifies the data source for the lookup column.
   */
  dataSource?: FilterLookupDataSource<any> | ((options: { data?: any; key?: any }) => FilterLookupDataSource<any>);
  /**
   * Specifies the data source field whose values must be displayed.
   */
  displayExpr?: string | ((data: any) => string);
  /**
   * Specifies the data field whose values should be replaced with values from the displayExpr field.
   */
  valueExpr?: string;
  /**
   * 
   */
  calculateCellValue?: ((rowData: any) => any);
}

/**
 * @deprecated 
 */
export type GridBaseColumnButton = ColumnButtonBase;
/**
 * Allows you to customize buttons in the edit column or create a custom command column. Applies only if the column&apos;s type is &apos;buttons&apos;.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ColumnButtonBase {
    /**
     * Specifies a CSS class to be applied to the button.
     */
    cssClass?: string;
    /**
     * Specifies the text for the hint that appears when the button is hovered over or long-pressed.
     */
    hint?: string;
    /**
     * Specifies the button&apos;s icon.
     */
    icon?: string;
    /**
     * Specifies the button&apos;s text. Applies only if the button&apos;s icon is not specified.
     */
    text?: string;
}

export type Scrollable = Skip<dxScrollable, '_templateManager' | '_cancelOptionChange' | '_getTemplate' | '_invalidate' | '_refresh' | '_notifyOptionChanged' | '_createElement'>;

export type AdaptiveDetailRowPreparingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & AdaptiveDetailRowPreparingInfo;

export type CellClickEvent<TRowData = any, TKey = any> = NativeEventInfo<dxDataGrid<TRowData, TKey>, PointerEvent | MouseEvent> & {
  readonly data: TRowData;
  readonly key: TKey;
  readonly value?: any;
  readonly displayValue?: any;
  readonly text: string;
  readonly columnIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly rowIndex: number;
  readonly rowType: string;
  readonly cellElement: DxElement;
  readonly row: Row<TRowData, TKey>;
};

export type CellDblClickEvent<TRowData = any, TKey = any> = NativeEventInfo<dxDataGrid<TRowData, TKey>, PointerEvent | MouseEvent> & {
  readonly data: TRowData;
  readonly key: TKey;
  readonly value?: any;
  readonly displayValue?: any;
  readonly text: string;
  readonly columnIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly rowIndex: number;
  readonly rowType: string;
  readonly cellElement: DxElement;
  readonly row: Row<TRowData, TKey>;
};

export type CellHoverChangedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly eventType: string;
  readonly data: TRowData;
  readonly key: TKey;
  readonly value?: any;
  readonly text: string;
  readonly displayValue?: any;
  readonly columnIndex: number;
  readonly rowIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly rowType: string;
  readonly cellElement: DxElement;
  readonly row: Row<TRowData, TKey>;
};

export type CellPreparedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly data: TRowData;
  readonly key: TKey;
  readonly value?: any;
  readonly displayValue?: any;
  readonly text: string;
  readonly columnIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly rowIndex: number;
  readonly rowType: string;
  readonly row: Row<TRowData, TKey>;
  readonly isSelected?: boolean;
  readonly isExpanded?: boolean;
  readonly isNewRow?: boolean;
  readonly cellElement: DxElement;
  readonly watch?: Function;
  readonly oldValue?: any;
};

export type ContentReadyEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>>;

export type ContextMenuPreparingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  items?: Array<any>;
  readonly target: string;
  readonly targetElement: DxElement;
  readonly columnIndex: number;
  readonly column?: Column<TRowData, TKey>;
  readonly rowIndex: number;
  readonly row?: Row<TRowData, TKey>;
};

export type DataErrorOccurredEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & DataErrorOccurredInfo;

export type DisposingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>>;

export type EditCanceledEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & DataChangeInfo<TRowData, TKey>;

export type EditCancelingEvent<TRowData = any, TKey = any> = Cancelable & EventInfo<dxDataGrid<TRowData, TKey>> & DataChangeInfo<TRowData, TKey>;

export type EditingStartEvent<TRowData = any, TKey = any> = Cancelable & EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly data: TRowData;
  readonly key: TKey;
  readonly column?: Column<TRowData, TKey>;
};

export type EditorPreparedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly parentType: string;
  readonly value?: any;
  readonly setValue?: any;
  readonly updateValueTimeout?: number;
  readonly width?: number;
  readonly disabled: boolean;
  readonly rtlEnabled: boolean;
  readonly editorElement: DxElement;
  readonly readOnly: boolean;
  readonly dataField?: string;
  readonly row?: Row<TRowData, TKey>;
};

export type EditorPreparingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly parentType: string;
  readonly value?: any;
  readonly setValue?: any;
  readonly updateValueTimeout?: number;
  readonly width?: number;
  readonly disabled: boolean;
  readonly rtlEnabled: boolean;
  cancel: boolean;
  readonly editorElement: DxElement;
  readonly readOnly: boolean;
  editorName: string;
  editorOptions: any;
  readonly dataField?: string;
  readonly row?: Row<TRowData, TKey>;
};

export type ExportedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>>;

export type ExportingEvent<TRowData = any, TKey = any> = Cancelable & EventInfo<dxDataGrid<TRowData, TKey>> & {
  fileName?: string;
};

export type FileSavingEvent<TRowData = any, TKey = any> = Cancelable & {
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly element: DxElement;
  fileName?: string;
  format?: string;
  readonly data: Blob;
};

export type FocusedCellChangedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly cellElement: DxElement;
  readonly columnIndex: number;
  readonly rowIndex: number;
  readonly row?: Row<TRowData, TKey>;
  readonly column?: Column<TRowData, TKey>;
};

export type FocusedCellChangingEvent<TRowData = any, TKey = any> = Cancelable & NativeEventInfo<dxDataGrid<TRowData, TKey>, KeyboardEvent | PointerEvent | MouseEvent | TouchEvent> & {
  readonly cellElement: DxElement;
  readonly prevColumnIndex: number;
  readonly prevRowIndex: number;
  newColumnIndex: number;
  newRowIndex: number;
  readonly rows: Array<Row<TRowData, TKey>>;
  readonly columns: Array<Column<TRowData, TKey>>;
  isHighlighted: boolean;
};

export type FocusedRowChangedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly rowElement: DxElement;
  readonly rowIndex: number;
  readonly row?: Row<TRowData, TKey>;
};

export type FocusedRowChangingEvent<TRowData = any, TKey = any> = Cancelable & NativeEventInfo<dxDataGrid<TRowData, TKey>, KeyboardEvent | PointerEvent | MouseEvent | TouchEvent> & {
  readonly rowElement: DxElement;
  readonly prevRowIndex: number;
  newRowIndex: number;
  readonly rows: Array<Row<TRowData, TKey>>;
};

export type InitializedEvent<TRowData = any, TKey = any> = InitializedEventInfo<dxDataGrid<TRowData, TKey>>;

export type InitNewRowEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & NewRowInfo<TRowData>;

export type KeyDownEvent<TRowData = any, TKey = any> = NativeEventInfo<dxDataGrid<TRowData, TKey>, KeyboardEvent> & KeyDownInfo;

export type OptionChangedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & ChangedOptionInfo;

export type RowClickEvent<TRowData = any, TKey = any> = NativeEventInfo<dxDataGrid<TRowData, TKey>, PointerEvent | MouseEvent> & {
  readonly data: TRowData;
  readonly key: TKey;
  readonly values: Array<any>;
  readonly columns: Array<Column<TRowData, TKey>>;
  readonly rowIndex: number;
  readonly rowType: string;
  readonly isSelected?: boolean;
  readonly isExpanded?: boolean;
  readonly isNewRow?: boolean;
  readonly groupIndex?: number;
  readonly rowElement: DxElement;
  readonly handled: boolean;
};

export type RowCollapsedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowKeyInfo<TKey>;

export type RowCollapsingEvent<TRowData = any, TKey = any> = Cancelable & EventInfo<dxDataGrid<TRowData, TKey>> & RowKeyInfo<TKey>;

export type RowDblClickEvent<TRowData = any, TKey = any> = NativeEventInfo<dxDataGrid<TRowData, TKey>, PointerEvent | MouseEvent> & {
  readonly data: TRowData;
  readonly key: TKey;
  readonly values: Array<any>;
  readonly columns: Array<Column<TRowData, TKey>>;
  readonly rowIndex: number;
  readonly rowType: string;
  readonly isSelected?: boolean;
  readonly isExpanded?: boolean;
  readonly isNewRow?: boolean;
  readonly groupIndex?: number;
  readonly rowElement: DxElement;
};

export type RowExpandedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowKeyInfo<TKey>;

export type RowExpandingEvent<TRowData = any, TKey = any> = Cancelable & EventInfo<dxDataGrid<TRowData, TKey>> & RowKeyInfo<TKey>;

export type RowInsertedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowInsertedInfo<TRowData, TKey>;

export type RowInsertingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowInsertingInfo<TRowData>;

export type RowPreparedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & {
  readonly data: TRowData;
  readonly key: TKey;
  readonly values: Array<any>;
  readonly columns: Array<Column<TRowData, TKey>>;
  readonly rowIndex: number;
  readonly rowType: string;
  readonly groupIndex?: number;
  readonly isSelected?: boolean;
  readonly isExpanded?: boolean;
  readonly isNewRow?: boolean;
  readonly rowElement: DxElement;
};

export type RowRemovedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowRemovedInfo<TRowData, TKey>;

export type RowRemovingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowRemovingInfo<TRowData, TKey>;

export type RowUpdatedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowUpdatedInfo<TRowData, TKey>;

export type RowUpdatingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowUpdatingInfo<TRowData, TKey>;

export type RowValidatingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & RowValidatingInfo<TRowData, TKey>;

export type SavedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & DataChangeInfo<TRowData, TKey>;

export type SavingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & SavingInfo<TRowData, TKey>;

export type SelectionChangedEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & SelectionChangedInfo<TRowData, TKey>;

export type ToolbarPreparingEvent<TRowData = any, TKey = any> = EventInfo<dxDataGrid<TRowData, TKey>> & ToolbarPreparingInfo;

export type RowDraggingAddEvent<TRowData = any, TKey = any> = RowDraggingEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey> & DragDropInfo;

export type RowDraggingChangeEvent<TRowData = any, TKey = any> = Cancelable & RowDraggingEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey> & DragDropInfo;

export type RowDraggingEndEvent<TRowData = any, TKey = any> = Cancelable & RowDraggingEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey> & DragDropInfo;

export type RowDraggingMoveEvent<TRowData = any, TKey = any> = Cancelable & RowDraggingEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey> & DragDropInfo;

export type RowDraggingStartEvent<TRowData = any, TKey = any> = Cancelable & DragStartEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey>;

export type RowDraggingRemoveEvent<TRowData = any, TKey = any> = RowDraggingEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey>;

export type RowDraggingReorderEvent<TRowData = any, TKey = any> = RowDraggingEventInfo<dxDataGrid<TRowData, TKey>, TRowData, TKey> & DragReorderInfo;

export type ColumnButtonClickEvent<TRowData = any, TKey = any> = NativeEventInfo<dxDataGrid<TRowData, TKey>, PointerEvent | MouseEvent> & {
  row?: Row<TRowData, TKey>;
  column?: Column<TRowData, TKey>;
};

export type ColumnButtonTemplateData<TRowData = any, TKey = any> = {
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly data?: TRowData;
  readonly key?: TKey;
  readonly columnIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly rowIndex: number;
  readonly rowType: string;
  readonly row: Row<TRowData, TKey>;
};

export type ColumnCellTemplateData<TRowData = any, TKey = any> = {
  readonly data?: TRowData;
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly value?: any;
  readonly oldValue?: any;
  readonly displayValue?: any;
  readonly text: string;
  readonly columnIndex: number;
  readonly rowIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly row: Row<TRowData, TKey>;
  readonly rowType: string;
  readonly watch?: Function;
};

export type ColumnEditCellTemplateData<TRowData = any, TKey = any> = {
  readonly setValue?: any;
  readonly data?: TRowData;
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly value?: any;
  readonly displayValue?: any;
  readonly text: string;
  readonly columnIndex: number;
  readonly rowIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly row: Row<TRowData, TKey>;
  readonly rowType: string;
  readonly watch?: Function;
};

export type ColumnGroupCellTemplateData<TRowData = any, TKey = any> = {
  readonly data?: GroupData<TRowData>;
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly value?: any;
  readonly text: string;
  readonly displayValue?: any;
  readonly columnIndex: number;
  readonly rowIndex: number;
  readonly column: Column<TRowData, TKey>;
  readonly row: Row<GroupData<TRowData>, GroupKey>;
  readonly summaryItems: Array<any>;
  readonly groupContinuesMessage?: string;
  readonly groupContinuedMessage?: string;
};

export type ColumnHeaderCellTemplateData<TRowData = any, TKey = any> = {
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly columnIndex: number;
  readonly column: Column<TRowData, TKey>;
};

export type MasterDetailTemplateData<TRowData = any, TKey = any> = {
  readonly key: TKey;
  readonly data: TRowData;
  readonly watch?: Function;
};

export type RowDraggingTemplateData<TRowData = any> = RowDraggingTemplateDataModel<TRowData>;

export type RowTemplateData<TRowData = any, TKey = any> = {
  readonly key: TKey;
  readonly data: TRowData;
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly values: Array<any>;
  readonly rowIndex: number;
  readonly columns: Array<Column<TRowData, TKey>>;
  readonly isSelected?: boolean;
  readonly rowType: string;
  readonly groupIndex?: number;
  readonly isExpanded?: boolean;
};

export type DataRowTemplateData<TRowData = any, TKey = any> = {
  readonly key: TKey;
  readonly data: TRowData;
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly values: Array<any>;
  readonly rowIndex: number;
  readonly columns: Array<Column<TRowData, TKey>>;
  readonly isSelected?: boolean;
  readonly isExpanded?: boolean;
};

/**
 * @deprecated use Properties instead
 */
export interface dxDataGridOptions<TRowData = any, TKey = any> extends GridBaseOptions<dxDataGrid<TRowData, TKey>, TRowData, TKey> {
    /**
     * An array of grid columns.
     */
    columns?: Array<Column<TRowData, TKey> | string>;
    /**
     * Customizes columns after they are created.
     */
    customizeColumns?: ((columns: Array<Column<TRowData, TKey>>) => void);
    /**
     * Customizes data before export.
     * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
     */
    customizeExportData?: ((columns: Array<Column<TRowData, TKey>>, rows: Array<Row<TRowData, TKey>>) => void);
    /**
     * Configures editing.
     */
    editing?: Editing<TRowData, TKey>;
    /**
     * Configures client-side exporting.
     */
    export?: Export<TRowData, TKey>;
    /**
     * Configures the group panel.
     */
    groupPanel?: GroupPanel;
    /**
     * Configures grouping.
     */
    grouping?: Grouping;
    /**
     * Specifies the key property (or properties) that provide(s) key values to access data items. Each key value must be unique. This property applies only if data is a simple array.
     */
    keyExpr?: string | Array<string>;
    /**
     * Allows you to build a master-detail interface in the grid.
     */
    masterDetail?: MasterDetail<TRowData, TKey>;
    /**
     * A function that is executed when a cell is clicked or tapped. Executed before onRowClick.
     */
    onCellClick?: ((e: CellClickEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed when a cell is double-clicked or double-tapped. Executed before onRowDblClick.
     */
    onCellDblClick?: ((e: CellDblClickEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed after the pointer enters or leaves a cell.
     */
    onCellHoverChanged?: ((e: CellHoverChangedEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed after a grid cell is created.
     */
    onCellPrepared?: ((e: CellPreparedEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed before the context menu is rendered.
     */
    onContextMenuPreparing?: ((e: ContextMenuPreparingEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed before a cell or row switches to the editing state.
     */
    onEditingStart?: ((e: EditingStartEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed after an editor is created. Not executed for cells with an editCellTemplate.
     */
    onEditorPrepared?: ((options: EditorPreparedEvent<TRowData, TKey>) => void);
    /**
     * A function used to customize a cell&apos;s editor. Not executed for cells with an editCellTemplate.
     */
    onEditorPreparing?: ((e: EditorPreparingEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed after data is exported.
     * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
     */
    onExported?: ((e: ExportedEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed before data is exported.
     */
    onExporting?: ((e: ExportingEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed before a file with exported data is saved to the user&apos;s local storage.
     * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
     */
    onFileSaving?: ((e: FileSavingEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed after the focused cell changes. Applies only to cells in data or group rows.
     */
    onFocusedCellChanged?: ((e: FocusedCellChangedEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed before the focused cell changes. Applies only to cells in data or group rows.
     */
    onFocusedCellChanging?: ((e: FocusedCellChangingEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed after the focused row changes. Applies only to data or group rows. focusedRowEnabled should be true.
     */
    onFocusedRowChanged?: ((e: FocusedRowChangedEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed before the focused row changes. Applies only to data or group rows. focusedRowEnabled should be true.
     */
    onFocusedRowChanging?: ((e: FocusedRowChangingEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed when a row is clicked or tapped.
     */
    onRowClick?: ((e: RowClickEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed when a row is double-clicked or double-tapped. Executed after onCellDblClick.
     */
    onRowDblClick?: ((e: RowDblClickEvent<TRowData, TKey>) => void);
    /**
     * A function that is executed after a row is created.
     */
    onRowPrepared?: ((e: RowPreparedEvent<TRowData, TKey>) => void);
    /**
     * Notifies the DataGrid of the server&apos;s data processing operations.
     */
    remoteOperations?: boolean | {
      /**
       * Specifies whether or not filtering must be performed on the server side.
       */
      filtering?: boolean;
      /**
       * Specifies whether paging by groups should be performed on the server side.
       */
      groupPaging?: boolean;
      /**
       * Specifies whether or not grouping must be performed on the server side.
       */
      grouping?: boolean;
      /**
       * Specifies whether or not paging must be performed on the server side.
       */
      paging?: boolean;
      /**
       * Specifies whether or not sorting must be performed on the server side.
       */
      sorting?: boolean;
      /**
       * Specifies whether or not summaries summaries are calculated on the server-side.
       */
      summary?: boolean;
    } | 'auto';
    /**
     * Specifies a custom template for rows.
     * @deprecated Use the dataRowTemplate option instead.
     */
    rowTemplate?: template | ((rowElement: DxElement, rowInfo: RowTemplateData<TRowData, TKey>) => any);
        /**
     * Specifies a custom template for data rows.
     */
    dataRowTemplate?: template | ((rowElement: DxElement, rowInfo: DataRowTemplateData<TRowData, TKey>) => any);
    /**
     * Configures scrolling.
     */
    scrolling?: Scrolling;
    /**
     * Configures runtime selection.
     */
    selection?: Selection;
    /**
     * Specifies filters for the rows that must be selected initially. Applies only if selection.deferred is true.
     */
    selectionFilter?: string | Array<any> | Function;
    /**
     * Allows you to sort groups according to the values of group summary items.
     */
    sortByGroupSummaryInfo?: Array<dxDataGridSortByGroupSummaryInfoItem>;
    /**
     * Specifies the properties of the grid summary.
     */
    summary?: Summary<TRowData, TKey>;
    /**
     * Configures the toolbar.
     */
    toolbar?: Toolbar;
}

export type ExcelCellInfo<TRowData = any, TKey = any> = {
  readonly component: dxDataGrid<TRowData, TKey>;
  horizontalAlignment?: 'center' | 'centerContinuous' | 'distributed' | 'fill' | 'general' | 'justify' | 'left' | 'right';
  verticalAlignment?: 'bottom' | 'center' | 'distributed' | 'justify' | 'top';
  wrapTextEnabled?: boolean;
  backgroundColor?: string;
  fillPatternType?: 'darkDown' | 'darkGray' | 'darkGrid' | 'darkHorizontal' | 'darkTrellis' | 'darkUp' | 'darkVertical' | 'gray0625' | 'gray125' | 'lightDown' | 'lightGray' | 'lightGrid' | 'lightHorizontal' | 'lightTrellis' | 'lightUp' | 'lightVertical' | 'mediumGray' | 'none' | 'solid';
  fillPatternColor?: string;
  font?: ExcelFont;
  readonly value?: string | number | Date;
  numberFormat?: string;
  gridCell?: ExcelCell;
};

export type Export<TRowData = any, TKey = any> = {
  /**
   * Allows users to export selected rows only.
   */
  allowExportSelectedData?: boolean;
  /**
   * Customizes an Excel cell after it is created.
   * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
   */
  customizeExcelCell?: ((options: ExcelCellInfo<TRowData, TKey>) => void);
  /**
   * Adds the Export button to the DataGrid&apos;s toolbar.
   */
  enabled?: boolean;
  /**
   * Specifies whether to enable Excel filtering for the exported data in the resulting XLSX file.
   * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
   */
  excelFilterEnabled?: boolean;
  /**
   * Specifies whether to enable word wrapping for exported data in the resulting XLSX file.
   * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
   */
  excelWrapTextEnabled?: boolean;
  /**
   * Specifies a default name for the file to which grid data is exported.
   * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
   */
  fileName?: string;
  /**
   * Specifies whether Excel should hide warnings if there are errors in the exported document.
   * @deprecated Since v20.1, we recommend ExcelJS-based export which does not use this property.
   */
  ignoreExcelErrors?: boolean;
  /**
   * Specifies the URL of the server-side proxy that streams the resulting file to the end user to enable exporting in the Safari browser.
   * @deprecated Since v10, Safari browser supports API for saving files, and this property is no longer required.
   */
  proxyUrl?: string;
  /**
   * Configures the texts of export commands, buttons, and hints.
   */
  texts?: ExportTexts;
};

export type ExportTexts = {
  /**
   * The text or hint of the command that exports all data.
   */
  exportAll?: string;
  /**
   * The text of the command that exports selected rows. Applies when allowExportSelectedData property is true.
   */
  exportSelectedRows?: string;
  /**
   * The hint of the Export button when the allowExportSelectedData property is true.
   */
  exportTo?: string;
};

export type GroupPanel = {
  /**
   * Specifies whether columns can be dragged onto or from the group panel.
   */
  allowColumnDragging?: boolean;
  /**
   * Specifies text displayed by the group panel when it does not contain any columns.
   */
  emptyPanelText?: string;
  /**
   * Specifies whether the group panel is visible or not.
   */
  visible?: boolean | 'auto';
};

export type Grouping = {
  /**
   * Specifies whether the user can collapse grouped records in a grid or not.
   */
  allowCollapsing?: boolean;
  /**
   * Specifies whether groups appear expanded or not.
   */
  autoExpandAll?: boolean;
  /**
   * Enables the user to group data using the context menu.
   */
  contextMenuEnabled?: boolean;
  /**
   * Specifies the event on which a group will be expanded/collapsed.
   */
  expandMode?: 'buttonClick' | 'rowClick';
  /**
   * Defines the texts of grouping-related visual elements.
   */
  texts?: GroupingTexts;
};

export type GroupingTexts = {
  /**
   * Specifies the text of the context menu item that groups data by a specific column.
   */
  groupByThisColumn?: string;
  /**
   * Specifies the message displayed in a group row when the corresponding group is continued from the previous page.
   */
  groupContinuedMessage?: string;
  /**
   * Specifies the message displayed in a group row when the corresponding group continues on the next page.
   */
  groupContinuesMessage?: string;
  /**
   * Specifies the text of the context menu item that clears grouping settings of a specific column.
   */
  ungroup?: string;
  /**
   * Specifies the text of the context menu item that clears grouping settings of all columns.
   */
  ungroupAll?: string;
};

export type MasterDetail<TRowData = any, TKey = any> = {
  /**
   * Specifies whether detail sections appear expanded or collapsed.
   */
  autoExpandAll?: boolean;
  /**
   * Enables an end-user to expand/collapse detail sections.
   */
  enabled?: boolean;
  /**
   * Specifies a custom template for detail sections.
   */
  template?: template | ((detailElement: DxElement, detailInfo: MasterDetailTemplateData<TRowData, TKey>) => any);
};

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDataGridSortByGroupSummaryInfoItem {
    /**
     * Specifies the identifier of the column that must be used in grouping so that sorting by group summary item values be applied.
     */
    groupColumn?: string;
    /**
     * Specifies the sort order of group summary item values.
     */
    sortOrder?: 'asc' | 'desc';
    /**
     * Specifies the group summary item whose values must be used to sort groups.
     */
    summaryItem?: string | number;
}

export type CustomSummaryInfo<TRowData = any, TKey = any> = {
  readonly component: dxDataGrid<TRowData, TKey>;
  readonly name?: string;
  readonly summaryProcess: string;
  readonly value?: any;
  totalValue?: any;
  readonly groupIndex?: number;
};

export type Summary<TRowData = any, TKey = any> = {
  /**
   * Specifies a custom aggregate function. This function is called for summary items whose summaryType is &apos;custom&apos;.
   */
  calculateCustomSummary?: ((options: CustomSummaryInfo<TRowData, TKey>) => void);
  /**
   * Specifies items of the group summary.
   */
  groupItems?: Array<SummaryGroupItem>;
  /**
   * Specifies whether to recalculate summaries while a user edits data.
   */
  recalculateWhileEditing?: boolean;
  /**
   * Specifies whether to skip empty strings, null and undefined values when calculating a summary. Does not apply when you use a remote data source.
   */
  skipEmptyValues?: boolean;
  /**
   * Contains properties that specify text patterns for summary items.
   */
  texts?: SummaryTexts;
  /**
   * Specifies items of the total summary.
   */
  totalItems?: Array<SummaryTotalItem>;
};

export type SummaryItemTextInfo = {
  readonly value?: string | number | Date;
  readonly valueText: string;
};

export type SummaryGroupItem = {
    /**
     * Indicates whether to display group summary items in parentheses after the group row header or to align them by the corresponding columns within the group row.
     */
    alignByColumn?: boolean;
    /**
     * Specifies the column that provides data for a group summary item.
     */
    column?: string;
    /**
     * Customizes the text to be displayed in the summary item.
     */
    customizeText?: ((itemInfo: SummaryItemTextInfo) => string);
    /**
     * Specifies the summary item&apos;s text.
     */
    displayFormat?: string;
    /**
     * Specifies the group summary item&apos;s identifier.
     */
    name?: string;
    /**
     * Specifies the column that must hold the summary item when this item is displayed in the group footer or aligned by a column in the group row.
     */
    showInColumn?: string;
    /**
     * Specifies whether or not a summary item must be displayed in the group footer.
     */
    showInGroupFooter?: boolean;
    /**
     * Specifies whether to skip empty strings, null, and undefined values when calculating a summary. Does not apply when you use a remote data source.
     */
    skipEmptyValues?: boolean;
    /**
     * Specifies how to aggregate data for the group summary item.
     */
    summaryType?: 'avg' | 'count' | 'custom' | 'max' | 'min' | 'sum' | string;
    /**
     * Specifies a summary item value&apos;s display format.
     */
    valueFormat?: Format;
};

export type SummaryTotalItem = {
  /**
   * Specifies the alignment of a summary item.
   */
  alignment?: 'center' | 'left' | 'right';
  /**
   * Specifies the column that provides data for a summary item.
   */
  column?: string;
  /**
   * Specifies a CSS class to be applied to a summary item.
   */
  cssClass?: string;
  /**
   * Customizes the text to be displayed in the summary item.
   */
  customizeText?: ((itemInfo: SummaryItemTextInfo) => string);
  /**
   * Specifies the summary item&apos;s text.
   */
  displayFormat?: string;
  /**
   * Specifies the total summary item&apos;s identifier.
   */
  name?: string;
  /**
   * Specifies the column that must hold the summary item.
   */
  showInColumn?: string;
  /**
   * Specifies whether to skip empty strings, null, and undefined values when calculating a summary. Does not apply when you use a remote data source.
   */
  skipEmptyValues?: boolean;
  /**
   * Specifies how to aggregate data for the total summary item.
   */
  summaryType?: 'avg' | 'count' | 'custom' | 'max' | 'min' | 'sum' | string;
  /**
   * Specifies a summary item value&apos;s display format.
   */
  valueFormat?: Format;
};

export type SummaryTexts = {
    /**
     * Specifies a pattern for the &apos;avg&apos; summary items when they are displayed in the parent column.
     */
    avg?: string;
    /**
     * Specifies a pattern for the &apos;avg&apos; summary items displayed in a group row or in any other column rather than the parent one.
     */
    avgOtherColumn?: string;
    /**
     * Specifies a pattern for the &apos;count&apos; summary items.
     */
    count?: string;
    /**
     * Specifies a pattern for the &apos;max&apos; summary items when they are displayed in the parent column.
     */
    max?: string;
    /**
     * Specifies a pattern for the &apos;max&apos; summary items displayed in a group row or in any other column rather than the parent one.
     */
    maxOtherColumn?: string;
    /**
     * Specifies a pattern for the &apos;min&apos; summary items when they are displayed in the parent column.
     */
    min?: string;
    /**
     * Specifies a pattern for the &apos;min&apos; summary items displayed in a group row or in any other column rather than the parent one.
     */
    minOtherColumn?: string;
    /**
     * Specifies a pattern for the &apos;sum&apos; summary items when they are displayed in the parent column.
     */
    sum?: string;
    /**
     * Specifies a pattern for the &apos;sum&apos; summary items displayed in a group row or in any other column rather than the parent one.
     */
    sumOtherColumn?: string;
};

type DefaultToolbarItemName = 'addRowButton' | 'applyFilterButton' | 'columnChooserButton' | 'exportButton' | 'groupPanel' | 'revertButton' | 'saveButton' | 'searchPanel';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type dxDataGridToolbar = Toolbar;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type dxDataGridToolbarItem = ToolbarItem;

/**
 * Configures toolbar items.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ToolbarItem extends dxToolbarItem {
  /**
   * A name used to identify the toolbar item.
   */
  name?: DefaultToolbarItemName | string;
  /**
   * Specifies a location for the item on the toolbar.
   */
  location?: 'after' | 'before' | 'center';
}

/**
 * Configures the toolbar.
 */
export type Toolbar = {
  /**
   * Configures toolbar items.
   */
  items?: Array<DefaultToolbarItemName | ToolbarItem>;
  /**
   * Specifies whether the toolbar is visible.
   */
  visible?: boolean;
  /**
   * Specifies whether the toolbar responds to user interaction.
   */
  disabled?: boolean;
};

/**
 * @deprecated Use Editing instead
 */
export type dxDataGridEditing<TRowData, TKey = any> = Editing<TRowData, TKey>;

export type Editing<TRowData = any, TKey = any> = EditingBase<TRowData, TKey> & {
    /**
     * Specifies whether a user can add new rows.
     */
    allowAdding?: boolean;
    /**
     * Specifies whether a user can delete rows. It is called for each data row when defined as a function.
     */
    allowDeleting?: boolean | ((options: { component?: dxDataGrid<TRowData, TKey>; row?: Row<TRowData, TKey> }) => boolean);
    /**
     * Specifies whether a user can update rows. It is called for each data row when defined as a function.
     */
    allowUpdating?: boolean | ((options: { component?: dxDataGrid<TRowData, TKey>; row?: Row<TRowData, TKey> }) => boolean);
    /**
     * Contains properties that specify texts for editing-related UI elements.
     */
    texts?: any;
    /**
     * Specifies a position for a new row.
     */
    newRowPosition?: 'first' | 'last' | 'pageBottom' | 'pageTop' | 'viewportBottom' | 'viewportTop';
};

/**
 * @deprecated Use Scrolling instead
 */
export type dxDataGridScrolling = Scrolling;

export type Scrolling = ScrollingBase & {
    /**
     * Specifies the scrolling mode.
     */
    mode?: 'infinite' | 'standard' | 'virtual';
};

/**
 * @deprecated Use Selection instead
 */
export type dxDataGridSelection = Selection;

export type Selection = SelectionBase & {
    /**
     * Makes selection deferred.
     */
    deferred?: boolean;
    /**
     * Specifies the mode in which all the records are selected. Applies only if selection.allowSelectAll is true.
     */
    selectAllMode?: 'allPages' | 'page';
    /**
     * Specifies when to display the selection column and row selection checkboxes. Applies only if selection.mode is &apos;multiple&apos;.
     */
    showCheckBoxesMode?: 'always' | 'none' | 'onClick' | 'onLongTap';
};
/**
 * The DataGrid is a UI component that represents data from a local or remote source in the form of a grid. This UI component offers such basic features as sorting, grouping, filtering, as well as more advanced capabilities, like state storing, client-side exporting, master-detail interface, and many others.
 */
declare class dxDataGrid<TRowData = any, TKey = any> extends Widget<dxDataGridOptions<TRowData, TKey>> implements GridBase<TRowData, TKey> {
    /**
     * Adds a new column.
     */
    addColumn(columnOptions: Column<TRowData, TKey> | string): void;
    /**
     * Adds an empty data row and switches it to the editing state.
     */
    addRow(): DxPromise<void>;
    /**
     * Ungroups grid records.
     */
    clearGrouping(): void;
    /**
     * Collapses master rows or groups of a specific level.
     */
    collapseAll(groupIndex?: number): void;
    /**
     * Collapses a group or a master row with a specific key.
     */
    collapseRow(key: TKey): DxPromise<void>;
    /**
     * Expands master rows or groups of a specific level. Does not apply if data is remote.
     */
    expandAll(groupIndex?: number): void;
    /**
     * Expands a group or a master row with a specific key.
     */
    expandRow(key: TKey): DxPromise<void>;
    /**
     * Exports grid data to Excel.
     * @deprecated Use exportDataGrid instead.
     */
    exportToExcel(selectionOnly: boolean): void;
    /**
     * Gets the currently selected rows&apos; keys.
     */
    getSelectedRowKeys(): Array<TKey> & DxPromise<Array<TKey>>;
    /**
     * Gets the selected rows&apos; data objects.
     */
    getSelectedRowsData(): Array<TRowData> & DxPromise<Array<TRowData>>;
    /**
     * Gets the value of a total summary item.
     */
    getTotalSummaryValue(summaryItemName: string): any;
    /**
     * Gets all visible columns.
     */
    getVisibleColumns(): Array<Column<TRowData, TKey>>;
    /**
     * Gets all visible columns at a specific hierarchical level of column headers. Use it to access banded columns.
     */
    getVisibleColumns(headerLevel: number): Array<Column<TRowData, TKey>>;
    /**
     * Gets currently rendered rows.
     */
    getVisibleRows(): Array<Row<TRowData, TKey>>;
    /**
     * Checks whether a specific group or master row is expanded or collapsed.
     */
    isRowExpanded(key: TKey): boolean;
    /**
     * Checks whether a row found using its data object is selected. Takes effect only if selection.deferred is true.
     */
    isRowSelected(data: TRowData): boolean;
    isRowSelected(key: TKey): boolean;
    /**
     * Gets the total row count.
     */
    totalCount(): number;

    beginCustomLoading(messageText: string): void;
    byKey(key: TKey): DxPromise<TRowData>;
    cancelEditData(): void;
    cellValue(rowIndex: number, dataField: string): any;
    cellValue(rowIndex: number, dataField: string, value: any): void;
    cellValue(rowIndex: number, visibleColumnIndex: number): any;
    cellValue(rowIndex: number, visibleColumnIndex: number, value: any): void;
    clearFilter(): void;
    clearFilter(filterName: string): void;
    clearSelection(): void;
    clearSorting(): void;
    closeEditCell(): void;
    collapseAdaptiveDetailRow(): void;
    columnCount(): number;
    columnOption(id: number | string): any;
    columnOption(id: number | string, optionName: string): any;
    columnOption(id: number | string, optionName: string, optionValue: any): void;
    columnOption(id: number | string, options: any): void;
    deleteColumn(id: number | string): void;
    deleteRow(rowIndex: number): void;
    deselectAll(): DxPromise<void>;
    deselectRows(keys: Array<TKey>): DxPromise<Array<TRowData>>;
    editCell(rowIndex: number, dataField: string): void;
    editCell(rowIndex: number, visibleColumnIndex: number): void;
    editRow(rowIndex: number): void;
    endCustomLoading(): void;
    expandAdaptiveDetailRow(key: TKey): void;
    filter(): any;
    filter(filterExpr: any): void;
    focus(): void;
    focus(element: UserDefinedElement): void;
    getCellElement(rowIndex: number, dataField: string): DxElement | undefined;
    getCellElement(rowIndex: number, visibleColumnIndex: number): DxElement | undefined;
    getCombinedFilter(): any;
    getCombinedFilter(returnDataField: boolean): any;
    getDataSource(): DataSource<TRowData, TKey>;
    getKeyByRowIndex(rowIndex: number): TKey | undefined;
    getRowElement(rowIndex: number): UserDefinedElementsArray | undefined;
    getRowIndexByKey(key: TKey): number;
    getScrollable(): Scrollable;
    getVisibleColumnIndex(id: number | string): number;
    hasEditData(): boolean;
    hideColumnChooser(): void;
    isAdaptiveDetailRowExpanded(key: TKey): boolean;
    isRowFocused(key: TKey): boolean;
    keyOf(obj: TRowData): TKey;
    navigateToRow(key: TKey): DxPromise<void>;
    pageCount(): number;
    pageIndex(): number;
    pageIndex(newIndex: number): DxPromise<void>;
    pageSize(): number;
    pageSize(value: number): void;
    refresh(): DxPromise<void>;
    refresh(changesOnly: boolean): DxPromise<void>;
    repaintRows(rowIndexes: Array<number>): void;
    saveEditData(): DxPromise<void>;
    searchByText(text: string): void;
    selectAll(): DxPromise<void>;
    selectRows(keys: Array<TKey>, preserve: boolean): DxPromise<Array<TRowData>>;
    selectRowsByIndexes(indexes: Array<number>): DxPromise<Array<TRowData>>;
    showColumnChooser(): void;
    state(): any;
    state(state: any): void;
    undeleteRow(rowIndex: number): void;
    updateDimensions(): void;
}

export type Column<TRowData = any, TKey = any> = dxDataGridColumn<TRowData, TKey>;

/**
 * @deprecated Use the Column type instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDataGridColumn<TRowData = any, TKey = any> extends ColumnBase<TRowData> {
    /**
     * Specifies whether data from this column should be exported. Applies only if the column is visible.
     */
    allowExporting?: boolean;
    /**
     * Specifies whether the user can group data by values of this column. Applies only when grouping is enabled.
     */
    allowGrouping?: boolean;
    /**
     * Specifies whether groups appear expanded or not when records are grouped by a specific column. Setting this property makes sense only when grouping is allowed for this column.
     */
    autoExpandGroup?: boolean;
    /**
     * Allows you to customize buttons in the edit column or create a custom command column. Applies only if the column&apos;s type is &apos;buttons&apos;.
     */
    buttons?: Array<'cancel' | 'delete' | 'edit' | 'save' | 'undelete' | ColumnButton<TRowData, TKey>>;
    /**
     * Sets custom column values used to group grid records.
     */
    calculateGroupValue?: string | ((rowData: TRowData) => any);
    /**
     * Specifies a custom template for data cells.
     */
    cellTemplate?: template | ((cellElement: DxElement, cellInfo: ColumnCellTemplateData<TRowData, TKey>) => any);
    /**
     * An array of grid columns.
     */
    columns?: Array<Column<TRowData, TKey> | string>;
    /**
     * Specifies a custom template for data cells in editing state.
     */
    editCellTemplate?: template | ((cellElement: DxElement, cellInfo: ColumnEditCellTemplateData<TRowData, TKey>) => any);
    /**
     * Specifies a custom template for group cells (group rows).
     */
    groupCellTemplate?: template | ((cellElement: DxElement, cellInfo: ColumnGroupCellTemplateData<TRowData, TKey>) => any);
    /**
     * Specifies the index of a column when grid records are grouped by the values of this column.
     */
    groupIndex?: number;
    /**
     * Specifies a custom template for column headers.
     */
    headerCellTemplate?: template | ((columnHeader: DxElement, headerInfo: ColumnHeaderCellTemplateData<TRowData, TKey>) => any);
    /**
     * Specifies whether or not to display the column when grid records are grouped by it.
     */
    showWhenGrouped?: boolean;
    /**
     * Specifies the command column that this object customizes.
     */
    type?: 'adaptive' | 'buttons' | 'detailExpand' | 'groupExpand' | 'selection' | 'drag';
}

export type ColumnButton<TRowData = any, TKey = any> = dxDataGridColumnButton<TRowData, TKey>;
/**
 * @deprecated Use the DataGrid's ColumnButton type instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDataGridColumnButton<TRowData = any, TKey = any> extends ColumnButtonBase {
    /**
     * The name used to identify a built-in button.
     */
    name?: 'cancel' | 'delete' | 'edit' | 'save' | 'undelete' | string;
    /**
     * A function that is executed when the button is clicked or tapped.
     */
    onClick?: ((e: ColumnButtonClickEvent<TRowData, TKey>) => void);
    /**
     * Specifies a custom button template.
     */
    template?: template | ((cellElement: DxElement, cellInfo: ColumnButtonTemplateData<TRowData, TKey>) => string | UserDefinedElement);
    /**
     * Specifies the button&apos;s visibility.
     */
    visible?: boolean | ((options: { component?: dxDataGrid<TRowData, TKey>; row?: Row<TRowData, TKey>; column?: Column<TRowData, TKey> }) => boolean);
    /**
     * Specifies whether the button is disabled.
     */
    disabled?: boolean | ((options: { component?: dxDataGrid<TRowData, TKey>; row?: Row<TRowData, TKey>; column?: Column<TRowData, TKey> }) => boolean);
}

/**
 * @deprecated Use Row instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type dxDataGridRowObject<TRowData = any, TKey = any> = Row<TRowData, TKey>;

/**
 * A grid row.
 */
export type Row<TRowData = any, TKey = any> = {
    /**
     * The data object represented by the row.
     */
    readonly data: TRowData;
    /**
     * The group index of the row. Available when the rowType is &apos;group&apos;.
     */
    readonly groupIndex?: number;
    /**
     * Indicates whether the row is in the editing state.
     */
    readonly isEditing?: boolean;
    /**
     * Indicates whether the row is expanded or collapsed. Available if rowType is &apos;data&apos; or &apos;group&apos;.
     */
    readonly isExpanded?: boolean;
    /**
     * Indicates that the row is added, but not yet saved. Available if rowType is &apos;data&apos;.
     */
    readonly isNewRow?: boolean;
    /**
     * Indicates whether the row is selected. Available if rowType is &apos;data&apos;.
     */
    readonly isSelected?: boolean;
    /**
     * The key of the data object represented by the row.
     */
    readonly key: TKey;
    /**
     * The visible index of the row.
     */
    readonly rowIndex: number;
    /**
     * The row&apos;s type.
     */
    readonly rowType: string;
    /**
     * Values of the row as they exist in the data source.
     */
    readonly values: Array<any>;
};

export type ExplicitTypes<TRowData, TKey> = {
  AdaptiveDetailRowPreparingEvent: AdaptiveDetailRowPreparingEvent<TRowData, TKey>;
  CellClickEvent: CellClickEvent<TRowData, TKey>;
  CellDblClickEvent: CellDblClickEvent<TRowData, TKey>;
  CellHoverChangedEvent: CellHoverChangedEvent<TRowData, TKey>;
  CellPreparedEvent: CellPreparedEvent<TRowData, TKey>;
  Column: Column<TRowData, TKey>;
  ColumnButton: ColumnButton<TRowData, TKey>;
  ColumnButtonClickEvent: ColumnButtonClickEvent<TRowData, TKey>;
  ColumnButtonTemplateData: ColumnButtonTemplateData<TRowData, TKey>;
  ColumnCellTemplateData: ColumnCellTemplateData<TRowData, TKey>;
  ColumnEditCellTemplateData: ColumnEditCellTemplateData<TRowData, TKey>;
  ColumnGroupCellTemplateData: ColumnGroupCellTemplateData<TRowData, TKey>;
  ColumnHeaderCellTemplateData: ColumnHeaderCellTemplateData<TRowData, TKey>;
  ContentReadyEvent: ContentReadyEvent<TRowData, TKey>;
  ContextMenuPreparingEvent: ContextMenuPreparingEvent<TRowData, TKey>;
  CustomSummaryInfo: CustomSummaryInfo<TRowData, TKey>;
  DataErrorOccurredEvent: DataErrorOccurredEvent<TRowData, TKey>;
  DataRowTemplateData: DataRowTemplateData<TRowData, TKey>;
  DisposingEvent: DisposingEvent<TRowData, TKey>;
  EditCanceledEvent: EditCanceledEvent<TRowData, TKey>;
  EditCancelingEvent: EditCancelingEvent<TRowData, TKey>;
  Editing: Editing<TRowData, TKey>;
  EditingStartEvent: EditingStartEvent<TRowData, TKey>;
  EditorPreparedEvent: EditorPreparedEvent<TRowData, TKey>;
  EditorPreparingEvent: EditorPreparingEvent<TRowData, TKey>;
  ExcelCellInfo: ExcelCellInfo<TRowData, TKey>;
  Export: Export<TRowData, TKey>;
  ExportedEvent: ExportedEvent<TRowData, TKey>;
  ExportingEvent: ExportingEvent<TRowData, TKey>;
  ExportTexts: ExportTexts;
  FileSavingEvent: FileSavingEvent<TRowData, TKey>;
  FocusedCellChangedEvent: FocusedCellChangedEvent<TRowData, TKey>;
  FocusedCellChangingEvent: FocusedCellChangingEvent<TRowData, TKey>;
  FocusedRowChangedEvent: FocusedRowChangedEvent<TRowData, TKey>;
  FocusedRowChangingEvent: FocusedRowChangingEvent<TRowData, TKey>;
  GroupData: GroupData<TRowData>;
  Grouping: Grouping;
  GroupingTexts: GroupingTexts;
  GroupPanel: GroupPanel;
  InitializedEvent: InitializedEvent<TRowData, TKey>;
  InitNewRowEvent: InitNewRowEvent<TRowData, TKey>;
  KeyDownEvent: KeyDownEvent<TRowData, TKey>;
  MasterDetail: MasterDetail<TRowData, TKey>;
  MasterDetailTemplateData: MasterDetailTemplateData<TRowData, TKey>;
  OptionChangedEvent: OptionChangedEvent<TRowData, TKey>;
  Properties: Properties<TRowData, TKey>;
  Row: Row<TRowData, TKey>;
  RowClickEvent: RowClickEvent<TRowData, TKey>;
  RowCollapsedEvent: RowCollapsedEvent<TRowData, TKey>;
  RowCollapsingEvent: RowCollapsingEvent<TRowData, TKey>;
  RowDblClickEvent: RowDblClickEvent<TRowData, TKey>;
  RowDraggingAddEvent: RowDraggingAddEvent<TRowData, TKey>;
  RowDraggingChangeEvent: RowDraggingChangeEvent<TRowData, TKey>;
  RowDraggingEndEvent: RowDraggingEndEvent<TRowData, TKey>;
  RowDraggingMoveEvent: RowDraggingMoveEvent<TRowData, TKey>;
  RowDraggingRemoveEvent: RowDraggingRemoveEvent<TRowData, TKey>;
  RowDraggingReorderEvent: RowDraggingReorderEvent<TRowData, TKey>;
  RowDraggingStartEvent: RowDraggingStartEvent<TRowData, TKey>;
  RowDraggingTemplateData: RowDraggingTemplateData<TRowData>;
  RowExpandedEvent: RowExpandedEvent<TRowData, TKey>;
  RowExpandingEvent: RowExpandingEvent<TRowData, TKey>;
  RowInsertedEvent: RowInsertedEvent<TRowData, TKey>;
  RowInsertingEvent: RowInsertingEvent<TRowData, TKey>;
  RowPreparedEvent: RowPreparedEvent<TRowData, TKey>;
  RowRemovedEvent: RowRemovedEvent<TRowData, TKey>;
  RowRemovingEvent: RowRemovingEvent<TRowData, TKey>;
  RowTemplateData: RowTemplateData<TRowData, TKey>;
  RowUpdatedEvent: RowUpdatedEvent<TRowData, TKey>;
  RowUpdatingEvent: RowUpdatingEvent<TRowData, TKey>;
  RowValidatingEvent: RowValidatingEvent<TRowData, TKey>;
  SavedEvent: SavedEvent<TRowData, TKey>;
  SavingEvent: SavingEvent<TRowData, TKey>;
  Scrolling: Scrolling;
  Selection: Selection;
  SelectionChangedEvent: SelectionChangedEvent<TRowData, TKey>;
  Summary: Summary<TRowData, TKey>;
  SummaryGroupItem: SummaryGroupItem;
  SummaryItemTextInfo: SummaryItemTextInfo;
  SummaryTexts: SummaryTexts;
  SummaryTotalItem: SummaryTotalItem;
  Toolbar: Toolbar;
  ToolbarItem: ToolbarItem;
  ToolbarPreparingEvent: ToolbarPreparingEvent<TRowData, TKey>;
};

export type Properties<TRowData = any, TKey = any> = dxDataGridOptions<TRowData, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<TRowData = any, TKey = any> = dxDataGridOptions<TRowData, TKey>;

export default dxDataGrid;
