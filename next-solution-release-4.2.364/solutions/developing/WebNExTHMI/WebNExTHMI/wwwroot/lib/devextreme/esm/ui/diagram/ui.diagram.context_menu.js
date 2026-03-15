/**
 * DevExtreme (esm/ui/diagram/ui.diagram.context_menu.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Widget from "../widget/ui.widget";
import ContextMenu from "../context_menu";
import DiagramCommandsManager from "./diagram.commands_manager";
import DiagramMenuHelper from "./ui.diagram.menu_helper";
import DiagramBar from "./diagram.bar";
import {
    getDiagram
} from "./diagram.importer";
import {
    hasWindow,
    getWindow
} from "../../core/utils/window";
var DIAGRAM_TOUCHBAR_CLASS = "dx-diagram-touchbar";
var DIAGRAM_TOUCHBAR_OVERLAY_CLASS = "dx-diagram-touchbar-overlay";
var DIAGRAM_TOUCHBAR_TARGET_CLASS = "dx-diagram-touchbar-target";
var DIAGRAM_TOUCHBAR_MIN_UNWRAPPED_WIDTH = 800;
var DIAGRAM_TOUCHBAR_Y_OFFSET = 32;
class DiagramContextMenuWrapper extends Widget {
    _init() {
        super._init();
        this._createOnVisibilityChangingAction();
        this._createOnInternalCommand();
        this._createOnCustomCommand();
        this._createOnItemClickAction();
        this._tempState = void 0;
        this._commands = [];
        this._commandToIndexMap = {};
        this.bar = new DiagramContextMenuBar(this)
    }
    _initMarkup() {
        super._initMarkup();
        this._commands = this._getCommands();
        this._commandToIndexMap = {};
        this._fillCommandToIndexMap(this._commands, []);
        this._$contextMenuTargetElement = $("<div>").addClass(DIAGRAM_TOUCHBAR_TARGET_CLASS).appendTo(this.$element());
        var $contextMenu = $("<div>").appendTo(this.$element());
        this._contextMenuInstance = this._createComponent($contextMenu, DiagramContextMenu, {
            isTouchBarMode: this._isTouchBarMode(),
            cssClass: this._isTouchBarMode() ? DIAGRAM_TOUCHBAR_CLASS : DiagramMenuHelper.getContextMenuCssClass(),
            closeOnOutsideClick: false,
            showEvent: "",
            focusStateEnabled: false,
            items: this._commands,
            position: this._isTouchBarMode() ? {
                my: {
                    x: "center",
                    y: "bottom"
                },
                at: {
                    x: "center",
                    y: "top"
                },
                of: this._$contextMenuTargetElement
            } : {},
            itemTemplate: function(itemData, itemIndex, itemElement) {
                DiagramMenuHelper.getContextMenuItemTemplate(this, itemData, itemIndex, itemElement)
            },
            onItemClick: _ref => {
                var {
                    itemData: itemData
                } = _ref;
                return this._onItemClick(itemData)
            },
            onShowing: e => {
                if (true === this._inOnShowing) {
                    return
                }
                this._inOnShowing = true;
                this._onVisibilityChangingAction({
                    visible: true,
                    component: this
                });
                e.component.option("items", e.component.option("items"));
                delete this._inOnShowing
            }
        })
    }
    _show(x, y, selection) {
        this._contextMenuInstance.hide();
        if (this._isTouchBarMode()) {
            this._$contextMenuTargetElement.show();
            if (!selection) {
                selection = {
                    x: x,
                    y: y,
                    width: 0,
                    height: 0
                }
            }
            var widthCorrection = selection.width > DIAGRAM_TOUCHBAR_MIN_UNWRAPPED_WIDTH ? 0 : (DIAGRAM_TOUCHBAR_MIN_UNWRAPPED_WIDTH - selection.width) / 2;
            this._$contextMenuTargetElement.css({
                left: selection.x - widthCorrection,
                top: selection.y - DIAGRAM_TOUCHBAR_Y_OFFSET,
                width: selection.width + 2 * widthCorrection,
                height: selection.height + 2 * DIAGRAM_TOUCHBAR_Y_OFFSET
            });
            this._contextMenuInstance.show()
        } else {
            this._contextMenuInstance.option("position", {
                offset: x + " " + y
            });
            this._contextMenuInstance.show()
        }
    }
    _hide() {
        this._$contextMenuTargetElement.hide();
        this._contextMenuInstance.hide();
        delete this._isTouchMode
    }
    _isTouchBarMode() {
        if (void 0 !== this._isTouchMode) {
            return this._isTouchMode
        }
        var {
            Browser: Browser
        } = getDiagram();
        if (Browser.TouchUI) {
            return true
        }
        if (!hasWindow()) {
            return false
        }
        var window = getWindow();
        return window.navigator && window.navigator.maxTouchPoints > 0
    }
    _onItemClick(itemData) {
        var processed = false;
        if (this._onItemClickAction) {
            processed = this._onItemClickAction(itemData)
        }
        if (!processed) {
            DiagramMenuHelper.onContextMenuItemClick(this, itemData, this._executeCommand.bind(this));
            this._contextMenuInstance.hide()
        }
    }
    _executeCommand(command, name, value) {
        if ("number" === typeof command) {
            this.bar.raiseBarCommandExecuted(command, value)
        } else if ("string" === typeof command) {
            this._onInternalCommandAction({
                command: command
            })
        }
        if (void 0 !== name) {
            this._onCustomCommandAction({
                name: name
            })
        }
    }
    _createOnInternalCommand() {
        this._onInternalCommandAction = this._createActionByOption("onInternalCommand")
    }
    _createOnCustomCommand() {
        this._onCustomCommandAction = this._createActionByOption("onCustomCommand")
    }
    _getCommands() {
        return DiagramCommandsManager.getContextMenuCommands(this.option("commands"))
    }
    _fillCommandToIndexMap(commands, indexPath) {
        commands.forEach((command, index) => {
            var commandIndexPath = indexPath.concat([index]);
            if (void 0 !== command.command) {
                this._commandToIndexMap[command.command] = commandIndexPath
            }
            if (Array.isArray(command.items)) {
                this._fillCommandToIndexMap(command.items, commandIndexPath)
            }
        })
    }
    _setItemEnabled(key, enabled) {
        this._setItemVisible(key, enabled)
    }
    _setItemVisible(key, visible) {
        var itemOptionText = DiagramMenuHelper.getItemOptionText(this._contextMenuInstance, this._commandToIndexMap[key]);
        DiagramMenuHelper.updateContextMenuItemVisible(this._contextMenuInstance, itemOptionText, visible)
    }
    _setItemValue(key, value) {
        var itemOptionText = DiagramMenuHelper.getItemOptionText(this._contextMenuInstance, this._commandToIndexMap[key]);
        DiagramMenuHelper.updateContextMenuItemValue(this._contextMenuInstance, itemOptionText, key, value)
    }
    _setItemSubItems(key, items) {
        var itemOptionText = DiagramMenuHelper.getItemOptionText(this._contextMenuInstance, this._commandToIndexMap[key]);
        DiagramMenuHelper.updateContextMenuItems(this._contextMenuInstance, itemOptionText, key, items)
    }
    _setEnabled(enabled) {
        this._contextMenuInstance.option("disabled", !enabled)
    }
    isVisible() {
        return this._inOnShowing
    }
    _createOnVisibilityChangingAction() {
        this._onVisibilityChangingAction = this._createActionByOption("onVisibilityChanging")
    }
    _createOnItemClickAction() {
        this._onItemClickAction = this._createActionByOption("onItemClick")
    }
    _optionChanged(args) {
        switch (args.name) {
            case "onVisibilityChanging":
                this._createOnVisibilityChangingAction();
                break;
            case "onInternalCommand":
                this._createOnInternalCommand();
                break;
            case "onCustomCommand":
                this._createOnCustomCommand();
                break;
            case "onItemClick":
                this._createOnItemClickAction();
                break;
            case "commands":
                this._invalidate();
                break;
            case "export":
                break;
            default:
                super._optionChanged(args)
        }
    }
}
class DiagramContextMenu extends ContextMenu {
    _renderContextMenuOverlay() {
        super._renderContextMenuOverlay();
        if (this._overlay && this.option("isTouchBarMode")) {
            this._overlay && this._overlay.option("onShown", () => {
                var $content = $(this._overlay.$content());
                $content.parent().addClass(DIAGRAM_TOUCHBAR_OVERLAY_CLASS)
            })
        }
    }
}
class DiagramContextMenuBar extends DiagramBar {
    constructor(owner) {
        super(owner)
    }
    getCommandKeys() {
        return this._getKeys(this._owner._commands)
    }
    setItemValue(key, value) {
        this._owner._setItemValue(key, value)
    }
    setItemEnabled(key, enabled) {
        this._owner._setItemEnabled(key, enabled)
    }
    setItemVisible(key, visible) {
        this._owner._setItemVisible(key, visible)
    }
    setItemSubItems(key, items) {
        this._owner._setItemSubItems(key, items)
    }
    setEnabled(enabled) {
        this._owner._setEnabled(enabled)
    }
    isVisible() {
        return this._owner.isVisible()
    }
}
export default {
    DiagramContextMenuWrapper: DiagramContextMenuWrapper,
    DiagramContextMenu: DiagramContextMenu
};
