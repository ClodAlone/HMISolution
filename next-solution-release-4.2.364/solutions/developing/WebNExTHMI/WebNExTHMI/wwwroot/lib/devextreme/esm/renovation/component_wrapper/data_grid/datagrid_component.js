/**
 * DevExtreme (esm/renovation/component_wrapper/data_grid/datagrid_component.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import DataGridBase from "../../../ui/data_grid/ui.data_grid.base";
var DATA_GRID_NAME = "dxDataGrid";
export class DataGridComponent extends DataGridBase {
    constructor(element, options) {
        super(element, options);
        this.NAME = DATA_GRID_NAME;
        this._deprecatedOptions = {}
    }
    _setDeprecatedOptions() {}
    _initTemplates() {}
    _updateDOMComponent() {}
    _isDimensionChangeSupported() {
        return false
    }
}
