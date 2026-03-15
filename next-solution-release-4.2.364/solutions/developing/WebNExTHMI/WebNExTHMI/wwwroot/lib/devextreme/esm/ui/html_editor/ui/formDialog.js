/**
 * DevExtreme (esm/ui/html_editor/ui/formDialog.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import {
    extend
} from "../../../core/utils/extend";
import Popup from "../../popup";
import Form from "../../form";
import {
    Deferred
} from "../../../core/utils/deferred";
import localizationMessage from "../../../localization/message";
import {
    getCurrentScreenFactor,
    hasWindow
} from "../../../core/utils/window";
import devices from "../../../core/devices";
var DIALOG_CLASS = "dx-formdialog";
var FORM_CLASS = "dx-formdialog-form";
class FormDialog {
    constructor(editorInstance, popupConfig) {
        this._editorInstance = editorInstance;
        this._popupUserConfig = popupConfig;
        this._renderPopup();
        this._attachOptionChangedHandler()
    }
    _renderPopup() {
        var editorInstance = this._editorInstance;
        var $container = $("<div>").addClass(DIALOG_CLASS).appendTo(editorInstance.$element());
        var popupConfig = this._getPopupConfig();
        return editorInstance._createComponent($container, Popup, popupConfig)
    }
    _attachOptionChangedHandler() {
        var _this$_popup;
        null === (_this$_popup = this._popup) || void 0 === _this$_popup ? void 0 : _this$_popup.on("optionChanged", _ref => {
            var {
                name: name,
                value: value
            } = _ref;
            if ("title" === name) {
                this._updateFormLabel(value)
            }
        })
    }
    _escKeyHandler() {
        this._popup.hide()
    }
    _addEscapeHandler(e) {
        e.component.registerKeyHandler("escape", this._escKeyHandler.bind(this))
    }
    _isSmallScreen() {
        var screenFactor = hasWindow() ? getCurrentScreenFactor() : null;
        return "phone" === devices.real().deviceType || "xs" === screenFactor
    }
    _getPopupConfig() {
        return extend({
            onInitialized: e => {
                this._popup = e.component;
                this._popup.on("hiding", () => {
                    this.deferred.reject()
                });
                this._popup.on("shown", () => {
                    this._form.focus()
                })
            },
            deferRendering: false,
            focusStateEnabled: false,
            showCloseButton: false,
            wrapperAttr: {
                class: "dx-formdialog"
            },
            fullScreen: this._isSmallScreen(),
            contentTemplate: contentElem => {
                var $formContainer = $("<div>").appendTo(contentElem);
                this._renderForm($formContainer, {
                    onEditorEnterKey: _ref2 => {
                        var {
                            component: component,
                            dataField: dataField,
                            event: event
                        } = _ref2;
                        this.hide(component.option("formData"), event)
                    },
                    customizeItem: item => {
                        if ("simple" === item.itemType) {
                            item.editorOptions = extend(true, {}, item.editorOptions, {
                                onInitialized: this._addEscapeHandler.bind(this)
                            })
                        }
                    }
                })
            },
            toolbarItems: [{
                toolbar: "bottom",
                location: "after",
                widget: "dxButton",
                options: {
                    onInitialized: this._addEscapeHandler.bind(this),
                    text: localizationMessage.format("OK"),
                    onClick: _ref3 => {
                        var {
                            event: event
                        } = _ref3;
                        this.hide(this._form.option("formData"), event)
                    }
                }
            }, {
                toolbar: "bottom",
                location: "after",
                widget: "dxButton",
                options: {
                    onInitialized: this._addEscapeHandler.bind(this),
                    text: localizationMessage.format("Cancel"),
                    onClick: () => {
                        this._popup.hide()
                    }
                }
            }]
        }, this._popupUserConfig)
    }
    _renderForm($container, options) {
        $container.addClass(FORM_CLASS);
        this._form = this._editorInstance._createComponent($container, Form, options);
        this._updateFormLabel()
    }
    _updateFormLabel(text) {
        var _this$_form;
        var label = null !== text && void 0 !== text ? text : this.popupOption("title");
        null === (_this$_form = this._form) || void 0 === _this$_form ? void 0 : _this$_form.$element().attr("aria-label", label)
    }
    show(formUserConfig) {
        if (this._popup.option("visible")) {
            return
        }
        this.deferred = new Deferred;
        var formConfig = extend({}, formUserConfig);
        this._form.option(formConfig);
        this._popup.show();
        return this.deferred.promise()
    }
    hide(formData, event) {
        this.deferred.resolve(formData, event);
        this._popup.hide()
    }
    popupOption(optionName, optionValue) {
        return this._popup.option.apply(this._popup, arguments)
    }
}
export default FormDialog;
