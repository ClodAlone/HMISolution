/**
 * DevExtreme (esm/ui/color_box/color_box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import Color from "../../color";
import ColorView from "./color_view";
import {
    extend
} from "../../core/utils/extend";
import registerComponent from "../../core/component_registrator";
import DropDownEditor from "../drop_down_editor/ui.drop_down_editor";
var COLOR_BOX_CLASS = "dx-colorbox";
var COLOR_BOX_INPUT_CLASS = COLOR_BOX_CLASS + "-input";
var COLOR_BOX_INPUT_CONTAINER_CLASS = COLOR_BOX_INPUT_CLASS + "-container";
var COLOR_BOX_COLOR_RESULT_PREVIEW_CLASS = COLOR_BOX_CLASS + "-color-result-preview";
var COLOR_BOX_COLOR_IS_NOT_DEFINED = COLOR_BOX_CLASS + "-color-is-not-defined";
var COLOR_BOX_OVERLAY_CLASS = COLOR_BOX_CLASS + "-overlay";
var COLOR_BOX_CONTAINER_CELL_CLASS = "dx-colorview-container-cell";
var COLOR_BOX_BUTTON_CELL_CLASS = "dx-colorview-button-cell";
var COLOR_BOX_BUTTONS_CONTAINER_CLASS = "dx-colorview-buttons-container";
var COLOR_BOX_APPLY_BUTTON_CLASS = "dx-colorview-apply-button";
var COLOR_BOX_CANCEL_BUTTON_CLASS = "dx-colorview-cancel-button";
var colorEditorPrototype = ColorView.prototype;
var colorUtils = {
    makeTransparentBackground: colorEditorPrototype._makeTransparentBackground.bind(colorEditorPrototype),
    makeRgba: colorEditorPrototype._makeRgba.bind(colorEditorPrototype)
};
var ColorBox = DropDownEditor.inherit({
    _supportedKeys: function() {
        var arrowHandler = function(e) {
            e.stopPropagation();
            if (this.option("opened")) {
                e.preventDefault();
                return true
            }
        };
        return extend(this.callBase(), {
            tab: function(e) {
                if (this.option("opened")) {
                    e.preventDefault();
                    this._colorView._rgbInputs[0].focus()
                }
            },
            enter: this._enterKeyHandler,
            leftArrow: arrowHandler,
            rightArrow: arrowHandler,
            upArrow: function(e) {
                if (!this.option("opened")) {
                    e.preventDefault();
                    return false
                }
                if (e.altKey) {
                    this.close();
                    return false
                }
                return true
            },
            downArrow: function(e) {
                if (!this.option("opened") && !e.altKey) {
                    e.preventDefault();
                    return false
                }
                if (!this.option("opened") && e.altKey) {
                    this._validatedOpening();
                    return false
                }
                return true
            }
        })
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            editAlphaChannel: false,
            applyValueMode: "useButtons",
            keyStep: 1,
            fieldTemplate: null,
            buttonsLocation: "bottom after"
        })
    },
    _popupHidingHandler: function() {
        this.callBase();
        if ("useButtons" === this.option("applyValueMode")) {
            this._updateColorViewValue(this.option("value"))
        }
    },
    _popupConfig: function() {
        return extend(this.callBase(), {
            width: ""
        })
    },
    _contentReadyHandler: function() {
        this._createColorView();
        this._addPopupBottomClasses()
    },
    _addPopupBottomClasses: function() {
        var $popupBottom = this._popup.bottomToolbar();
        if ($popupBottom) {
            $popupBottom.addClass(COLOR_BOX_CONTAINER_CELL_CLASS).addClass(COLOR_BOX_BUTTON_CELL_CLASS).find(".dx-toolbar-items-container").addClass(COLOR_BOX_BUTTONS_CONTAINER_CLASS);
            $popupBottom.find(".dx-popup-done").addClass(COLOR_BOX_APPLY_BUTTON_CLASS);
            $popupBottom.find(".dx-popup-cancel").addClass(COLOR_BOX_CANCEL_BUTTON_CLASS)
        }
    },
    _createColorView: function() {
        this._popup.$overlayContent().addClass(COLOR_BOX_OVERLAY_CLASS);
        var $colorView = $("<div>").appendTo(this._popup.$content());
        this._colorView = this._createComponent($colorView, ColorView, this._colorViewConfig());
        this._colorView.registerKeyHandler("escape", this._escapeHandler.bind(this));
        eventsEngine.on($colorView, "focus", function() {
            this.focus()
        }.bind(this))
    },
    _escapeHandler: function() {
        this.close();
        this.focus()
    },
    _applyNewColor: function(value) {
        this.option("value", value);
        if (value) {
            colorUtils.makeTransparentBackground(this._$colorResultPreview, value)
        }
        if (this._colorViewEnterKeyPressed) {
            this.close();
            this._colorViewEnterKeyPressed = false
        }
    },
    _colorViewConfig: function() {
        var that = this;
        return {
            value: that.option("value"),
            matchValue: that.option("value"),
            editAlphaChannel: that.option("editAlphaChannel"),
            applyValueMode: that.option("applyValueMode"),
            focusStateEnabled: that.option("focusStateEnabled"),
            stylingMode: this.option("stylingMode"),
            onEnterKeyPressed: function(_ref) {
                var {
                    event: event
                } = _ref;
                that._colorViewEnterKeyPressed = true;
                if (that._colorView.option("value") !== that.option("value")) {
                    that._saveValueChangeEvent(event);
                    that._applyNewColor(that._colorView.option("value"));
                    that.close()
                }
            },
            onValueChanged: function(_ref2) {
                var {
                    event: event,
                    value: value,
                    previousValue: previousValue
                } = _ref2;
                var instantlyMode = "instantly" === that.option("applyValueMode");
                var isOldValue = colorUtils.makeRgba(value) === previousValue;
                var changesApplied = instantlyMode || that._colorViewEnterKeyPressed;
                var valueCleared = that._shouldSaveEmptyValue;
                if (isOldValue || !changesApplied || valueCleared) {
                    return
                }
                if (event) {
                    that._saveValueChangeEvent(event)
                }
                that._applyNewColor(value)
            }
        }
    },
    _enterKeyHandler: function(e) {
        var newValue = this._input().val();
        var value = this.option("value");
        var oldValue = this.option("editAlphaChannel") ? colorUtils.makeRgba(value) : value;
        if (!newValue) {
            return false
        }
        var color = new Color(newValue);
        if (color.colorIsInvalid) {
            this._input().val(oldValue);
            return
        }
        if (newValue !== oldValue) {
            this._applyColorFromInput(newValue);
            this._saveValueChangeEvent(e);
            this.option("value", this.option("editAlphaChannel") ? colorUtils.makeRgba(newValue) : newValue)
        }
        if (this._colorView) {
            var colorViewValue = this._colorView.option("value");
            if (value !== colorViewValue) {
                this._saveValueChangeEvent(e);
                this.option("value", colorViewValue)
            }
        }
        this.close();
        return false
    },
    _applyButtonHandler: function(e) {
        this._saveValueChangeEvent(e.event);
        this._applyNewColor(this._colorView.option("value"));
        this.callBase()
    },
    _cancelButtonHandler: function() {
        this._resetInputValue();
        this.callBase()
    },
    _getKeyboardListeners() {
        return this.callBase().concat([this._colorView])
    },
    _init: function() {
        this.callBase()
    },
    _initMarkup: function() {
        this.$element().addClass(COLOR_BOX_CLASS);
        this.callBase()
    },
    _renderInput: function() {
        this.callBase();
        this._input().addClass(COLOR_BOX_INPUT_CLASS);
        this._renderColorPreview()
    },
    _renderColorPreview: function() {
        this.$element().wrapInner($("<div>").addClass(COLOR_BOX_INPUT_CONTAINER_CLASS));
        this._$colorBoxInputContainer = this.$element().children().eq(0);
        this._$colorResultPreview = $("<div>").addClass(COLOR_BOX_COLOR_RESULT_PREVIEW_CLASS).appendTo(this._$textEditorInputContainer);
        if (!this.option("value")) {
            this._$colorBoxInputContainer.addClass(COLOR_BOX_COLOR_IS_NOT_DEFINED)
        } else {
            colorUtils.makeTransparentBackground(this._$colorResultPreview, this.option("value"))
        }
    },
    _renderValue: function() {
        var value = this.option("value");
        var convertToColor = null !== value && this.option("editAlphaChannel");
        var text = convertToColor ? colorUtils.makeRgba(value) : value;
        this.option("text", text);
        return this.callBase()
    },
    _resetInputValue: function() {
        var $input = this._input();
        var value = this.option("value");
        $input.val(value);
        this._updateColorViewValue(value)
    },
    _updateColorViewValue: function(value) {
        if (this._colorView) {
            this._colorView.option({
                value: value,
                matchValue: value
            })
        }
    },
    _valueChangeEventHandler: function(e) {
        var value = this._input().val();
        if (value) {
            value = this._applyColorFromInput(value);
            this._updateColorViewValue(value)
        }
        this.callBase(e, value)
    },
    _applyColorFromInput: function(value) {
        var newColor = new Color(value);
        if (newColor.colorIsInvalid) {
            this._resetInputValue();
            value = this.option("value")
        }
        return value
    },
    _clean: function() {
        this.callBase();
        delete this._shouldSaveEmptyValue
    },
    _optionChanged: function(args) {
        var value = args.value;
        var name = args.name;
        switch (name) {
            case "value":
                this._$colorBoxInputContainer.toggleClass(COLOR_BOX_COLOR_IS_NOT_DEFINED, !value);
                if (value) {
                    colorUtils.makeTransparentBackground(this._$colorResultPreview, value)
                } else {
                    this._$colorResultPreview.removeAttr("style")
                }
                if (null === value) {
                    this._shouldSaveEmptyValue = true
                }
                this._updateColorViewValue(value);
                this._shouldSaveEmptyValue = false;
                this.callBase(args);
                break;
            case "applyButtonText":
            case "cancelButtonText":
                this.callBase(args);
                this._popup && this._addPopupBottomClasses();
                break;
            case "editAlphaChannel":
            case "keyStep":
                if (this._colorView) {
                    this._colorView.option(name, value)
                }
                break;
            default:
                this.callBase(args)
        }
    }
});
registerComponent("dxColorBox", ColorBox);
export default ColorBox;
