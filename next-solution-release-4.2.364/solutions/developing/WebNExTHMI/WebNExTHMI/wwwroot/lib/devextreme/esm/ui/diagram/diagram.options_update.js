/**
 * DevExtreme (esm/ui/diagram/diagram.options_update.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import DiagramBar from "./diagram.bar";
import {
    getDiagram
} from "./diagram.importer";
class DiagramOptionsUpdateBar extends DiagramBar {
    constructor(owner) {
        super(owner);
        var {
            DiagramCommand: DiagramCommand
        } = getDiagram();
        this.commandOptions = {};
        this.commandOptions[DiagramCommand.Fullscreen] = "fullScreen";
        this.commandOptions[DiagramCommand.ZoomLevel] = function(value) {
            if ("object" === typeof this._getOption("zoomLevel")) {
                this._setOption("zoomLevel.value", value)
            } else {
                this._setOption("zoomLevel", value)
            }
        };
        this.commandOptions[DiagramCommand.SwitchAutoZoom] = function(value) {
            var {
                AutoZoomMode: AutoZoomMode
            } = getDiagram();
            switch (value) {
                case AutoZoomMode.FitContent:
                    this._setOption("autoZoomMode", "fitContent");
                    break;
                case AutoZoomMode.FitToWidth:
                    this._setOption("autoZoomMode", "fitWidth");
                    break;
                case AutoZoomMode.Disabled:
                    this._setOption("autoZoomMode", "disabled")
            }
        };
        this.commandOptions[DiagramCommand.ToggleSimpleView] = "simpleView";
        this.commandOptions[DiagramCommand.ShowGrid] = "showGrid";
        this.commandOptions[DiagramCommand.SnapToGrid] = "snapToGrid";
        this.commandOptions[DiagramCommand.GridSize] = function(value) {
            if ("object" === typeof this._getOption("gridSize")) {
                this._setOption("gridSize.value", value)
            } else {
                this._setOption("gridSize", value)
            }
        };
        this.commandOptions[DiagramCommand.ViewUnits] = "viewUnits";
        this.commandOptions[DiagramCommand.PageSize] = function(value) {
            var pageSize = this._getOption("pageSize");
            if (void 0 === pageSize || pageSize.width !== value.width || pageSize.height !== value.height) {
                this._setOption("pageSize", value)
            }
        };
        this.commandOptions[DiagramCommand.PageLandscape] = function(value) {
            this._setOption("pageOrientation", value ? "landscape" : "portrait")
        };
        this.commandOptions[DiagramCommand.ViewUnits] = function(value) {
            var {
                DiagramUnit: DiagramUnit
            } = getDiagram();
            switch (value) {
                case DiagramUnit.In:
                    this._setOption("viewUnits", "in");
                    break;
                case DiagramUnit.Cm:
                    this._setOption("viewUnits", "cm");
                    break;
                case DiagramUnit.Px:
                    this._setOption("viewUnits", "px")
            }
        };
        this.commandOptions[DiagramCommand.PageColor] = "pageColor";
        this._updateLock = 0
    }
    getCommandKeys() {
        return Object.keys(this.commandOptions).map((function(key) {
            return parseInt(key)
        }))
    }
    setItemValue(key, value) {
        if (this.isUpdateLocked()) {
            return
        }
        this.beginUpdate();
        try {
            if ("function" === typeof this.commandOptions[key]) {
                this.commandOptions[key].call(this, value)
            } else {
                this._setOption(this.commandOptions[key], value)
            }
        } finally {
            this.endUpdate()
        }
    }
    beginUpdate() {
        this._updateLock++
    }
    endUpdate() {
        this._updateLock--
    }
    isUpdateLocked() {
        return this._updateLock > 0
    }
    _getOption(name) {
        return this._owner.option(name)
    }
    _setOption(name, value) {
        this._owner.option(name, value)
    }
}
export default DiagramOptionsUpdateBar;
