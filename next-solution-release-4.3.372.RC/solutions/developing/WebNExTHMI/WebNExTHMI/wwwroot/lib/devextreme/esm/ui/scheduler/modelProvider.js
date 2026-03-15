/**
 * DevExtreme (esm/ui/scheduler/modelProvider.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getGroupCount
} from "./resources/utils";
import {
    isObject
} from "../../core/utils/type";
import {
    getCurrentView
} from "../../renovation/ui/scheduler/model/views";
export class ModelProvider {
    constructor(model) {
        this.model = model;
        this.currentView = null
    }
    get key() {
        return this.model.key
    }
    get loadedResources() {
        return this.model.loadedResources
    }
    get startDayHour() {
        return this.model.startDayHour
    }
    get endDayHour() {
        return this.model.endDayHour
    }
    get adaptivityEnabled() {
        return this.model.adaptivityEnabled
    }
    get rtlEnabled() {
        return this.model.rtlEnabled
    }
    get maxAppointmentsPerCell() {
        return this.getCurrentViewOption("maxAppointmentsPerCell")
    }
    get currentViewOptions() {
        return this.currentView
    }
    get currentViewType() {
        return isObject(this.currentView) ? this.currentView.type : this.currentView
    }
    get agendaDuration() {
        return this.model.agendaDuration
    }
    get currentDate() {
        return this.model.currentDate
    }
    get timeZone() {
        return this.model.timeZone
    }
    get isRenovatedAppointments() {
        return this.model.isRenovatedAppointments
    }
    supportAllDayResizing() {
        return "day" !== this.currentViewType || this.currentView.intervalCount > 1
    }
    updateCurrentView() {
        var views = this.model.views;
        var currentView = this.model.currentView;
        this.currentView = getCurrentView(currentView, views)
    }
    isGroupedByDate() {
        return this.model.groupByDate && this._isHorizontalGroupedWorkSpace() && getGroupCount(this.loadedResources) > 0
    }
    _isHorizontalGroupedWorkSpace() {
        return !!this.loadedResources.length && "horizontal" === this.model.groupOrientation
    }
    getCurrentViewOption(optionName) {
        if (this.currentView && void 0 !== this.currentView[optionName]) {
            return this.currentView[optionName]
        }
        return this.model[optionName]
    }
}
