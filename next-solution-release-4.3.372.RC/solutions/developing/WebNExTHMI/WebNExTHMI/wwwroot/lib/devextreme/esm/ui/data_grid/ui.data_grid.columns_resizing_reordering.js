/**
 * DevExtreme (esm/ui/data_grid/ui.data_grid.columns_resizing_reordering.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import gridCore from "./ui.data_grid.core";
import {
    columnsResizingReorderingModule
} from "../grid_core/ui.grid_core.columns_resizing_reordering";
export var DraggingHeaderView = columnsResizingReorderingModule.views.draggingHeaderView;
export var DraggingHeaderViewController = columnsResizingReorderingModule.controllers.draggingHeader;
export var ColumnsSeparatorView = columnsResizingReorderingModule.views.columnsSeparatorView;
export var TablePositionViewController = columnsResizingReorderingModule.controllers.tablePosition;
export var ColumnsResizerViewController = columnsResizingReorderingModule.controllers.columnsResizer;
export var TrackerView = columnsResizingReorderingModule.views.trackerView;
gridCore.registerModule("columnsResizingReordering", columnsResizingReorderingModule);
