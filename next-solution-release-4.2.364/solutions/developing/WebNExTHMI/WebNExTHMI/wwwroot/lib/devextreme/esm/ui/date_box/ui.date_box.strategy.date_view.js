/**
 * DevExtreme (esm/ui/date_box/ui.date_box.strategy.date_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    getWindow
} from "../../core/utils/window";
var window = getWindow();
import DateView from "./ui.date_view";
import DateBoxStrategy from "./ui.date_box.strategy";
import {
    inputType
} from "../../core/utils/support";
import {
    extend
} from "../../core/utils/extend";
import dateUtils from "./ui.date_utils";
import messageLocalization from "../../localization/message";
var DateViewStrategy = DateBoxStrategy.inherit({
    NAME: "DateView",
    getDefaultOptions: function() {
        return extend(this.callBase(), {
            openOnFieldClick: true,
            applyButtonText: messageLocalization.format("OK"),
            "dropDownOptions.showTitle": true
        })
    },
    getDisplayFormat: function(displayFormat) {
        return displayFormat || dateUtils.FORMATS_MAP[this.dateBox.option("type")]
    },
    popupConfig: function(config) {
        return {
            toolbarItems: this.dateBox._popupToolbarItemsConfig(),
            onInitialized: config.onInitialized,
            defaultOptionsRules: [{
                device: {
                    platform: "android"
                },
                options: {
                    width: 333,
                    height: 331
                }
            }, {
                device: function(_device) {
                    var platform = _device.platform;
                    return "generic" === platform || "ios" === platform
                },
                options: {
                    width: "auto",
                    height: "auto"
                }
            }, {
                device: function(_device2) {
                    var platform = _device2.platform;
                    var phone = _device2.phone;
                    return "generic" === platform && phone
                },
                options: {
                    width: 333,
                    maxWidth: "100%",
                    maxHeight: "100%",
                    height: "auto",
                    position: {
                        collision: "flipfit flip"
                    }
                }
            }, {
                device: {
                    platform: "ios",
                    phone: true
                },
                options: {
                    width: "100%",
                    position: {
                        my: "bottom",
                        at: "bottom",
                        of: window
                    }
                }
            }]
        }
    },
    _renderWidget: function() {
        if (inputType(this.dateBox.option("mode")) && this.dateBox._isNativeType() || this.dateBox.option("readOnly")) {
            if (this._widget) {
                this._widget.$element().remove();
                this._widget = null
            }
            return
        }
        var popup = this._getPopup();
        if (this._widget) {
            this._widget.option(this._getWidgetOptions())
        } else {
            var element = $("<div>").appendTo(popup.$content());
            this._widget = this._createWidget(element)
        }
        this._widget.$element().appendTo(this._getWidgetContainer())
    },
    _getWidgetName: function() {
        return DateView
    },
    renderOpenedState: function() {
        this.callBase();
        if (this._widget) {
            this._widget.option("value", this._widget._getCurrentDate())
        }
    },
    _getWidgetOptions: function() {
        return {
            value: this.dateBoxValue() || new Date,
            type: this.dateBox.option("type"),
            minDate: this.dateBox.dateOption("min") || new Date(1900, 0, 1),
            maxDate: this.dateBox.dateOption("max") || new Date(Date.now() + 50 * dateUtils.ONE_YEAR),
            onDisposing: function() {
                this._widget = null
            }.bind(this)
        }
    }
});
export default DateViewStrategy;
