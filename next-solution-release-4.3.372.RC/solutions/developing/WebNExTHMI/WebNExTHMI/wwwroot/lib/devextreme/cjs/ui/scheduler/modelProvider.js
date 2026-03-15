/**
 * DevExtreme (cjs/ui/scheduler/modelProvider.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ModelProvider = void 0;
var _utils = require("./resources/utils");
var _type = require("../../core/utils/type");
var _views = require("../../renovation/ui/scheduler/model/views");

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}
var ModelProvider = function() {
    function ModelProvider(model) {
        this.model = model;
        this.currentView = null
    }
    var _proto = ModelProvider.prototype;
    _proto.supportAllDayResizing = function() {
        return "day" !== this.currentViewType || this.currentView.intervalCount > 1
    };
    _proto.updateCurrentView = function() {
        var views = this.model.views;
        var currentView = this.model.currentView;
        this.currentView = (0, _views.getCurrentView)(currentView, views)
    };
    _proto.isGroupedByDate = function() {
        return this.model.groupByDate && this._isHorizontalGroupedWorkSpace() && (0, _utils.getGroupCount)(this.loadedResources) > 0
    };
    _proto._isHorizontalGroupedWorkSpace = function() {
        return !!this.loadedResources.length && "horizontal" === this.model.groupOrientation
    };
    _proto.getCurrentViewOption = function(optionName) {
        if (this.currentView && void 0 !== this.currentView[optionName]) {
            return this.currentView[optionName]
        }
        return this.model[optionName]
    };
    _createClass(ModelProvider, [{
        key: "key",
        get: function() {
            return this.model.key
        }
    }, {
        key: "loadedResources",
        get: function() {
            return this.model.loadedResources
        }
    }, {
        key: "startDayHour",
        get: function() {
            return this.model.startDayHour
        }
    }, {
        key: "endDayHour",
        get: function() {
            return this.model.endDayHour
        }
    }, {
        key: "adaptivityEnabled",
        get: function() {
            return this.model.adaptivityEnabled
        }
    }, {
        key: "rtlEnabled",
        get: function() {
            return this.model.rtlEnabled
        }
    }, {
        key: "maxAppointmentsPerCell",
        get: function() {
            return this.getCurrentViewOption("maxAppointmentsPerCell")
        }
    }, {
        key: "currentViewOptions",
        get: function() {
            return this.currentView
        }
    }, {
        key: "currentViewType",
        get: function() {
            return (0, _type.isObject)(this.currentView) ? this.currentView.type : this.currentView
        }
    }, {
        key: "agendaDuration",
        get: function() {
            return this.model.agendaDuration
        }
    }, {
        key: "currentDate",
        get: function() {
            return this.model.currentDate
        }
    }, {
        key: "timeZone",
        get: function() {
            return this.model.timeZone
        }
    }, {
        key: "isRenovatedAppointments",
        get: function() {
            return this.model.isRenovatedAppointments
        }
    }]);
    return ModelProvider
}();
exports.ModelProvider = ModelProvider;
