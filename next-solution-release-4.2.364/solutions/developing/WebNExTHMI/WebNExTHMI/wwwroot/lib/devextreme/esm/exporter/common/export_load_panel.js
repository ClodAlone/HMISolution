/**
 * DevExtreme (esm/exporter/common/export_load_panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import messageLocalization from "../../localization/message";
import {
    isDefined
} from "../../core/utils/type";
import LoadPanel from "../../ui/load_panel";
import gridUtils from "../../ui/grid_core/ui.grid_core.utils";
var EXPORT_LOAD_PANEL_CLASS = "dx-export-loadpanel";
class ExportLoadPanel {
    constructor(component, $targetElement, $container, options) {
        this._$targetElement = $targetElement;
        this._$container = $container;
        this._loadPanel = component._createComponent($("<div>").addClass(EXPORT_LOAD_PANEL_CLASS).appendTo(this._$container), LoadPanel, this.getOptions(options))
    }
    getDefaultOptions() {
        return {
            animation: null,
            shading: false,
            height: 90,
            width: 200,
            container: this._$container
        }
    }
    getOptions(options) {
        if (isDefined(options.text)) {
            options.message = options.text
        } else {
            options.message = messageLocalization.format("dxDataGrid-exporting")
        }
        return extend(this.getDefaultOptions(), options)
    }
    show() {
        this._loadPanel.option("position", gridUtils.calculateLoadPanelPosition(this._$targetElement));
        this._loadPanel.show()
    }
    dispose() {
        $(this._loadPanel.element()).remove();
        delete this._loadPanel
    }
}
export {
    ExportLoadPanel
};
