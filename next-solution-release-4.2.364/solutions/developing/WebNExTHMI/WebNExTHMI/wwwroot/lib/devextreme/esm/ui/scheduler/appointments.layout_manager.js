/**
 * DevExtreme (esm/ui/scheduler/appointments.layout_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    equalByValue
} from "../../core/utils/common";
import {
    AppointmentViewModelGenerator
} from "./appointments/viewModelGenerator";
import {
    getGroupCount
} from "./resources/utils";
import {
    getCellWidth,
    getCellHeight,
    getAllDayHeight
} from "./workspaces/helpers/positionHelper";
import {
    getCellDuration
} from "../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    getAppointmentRenderingStrategyName
} from "../../renovation/ui/scheduler/model/appointments";
class AppointmentLayoutManager {
    constructor(instance) {
        this.instance = instance;
        this.appointmentViewModel = new AppointmentViewModelGenerator
    }
    get appointmentRenderingStrategyName() {
        return getAppointmentRenderingStrategyName(this.instance.currentViewType)
    }
    getCellDimensions(options) {
        if (this.instance._workSpace) {
            return {
                width: this.instance._workSpace.getCellWidth(),
                height: this.instance._workSpace.getCellHeight(),
                allDayHeight: this.instance._workSpace.getAllDayHeight()
            }
        }
    }
    _getRenderingStrategyOptions() {
        var workspace = this.instance.getWorkSpace();
        var {
            virtualScrollingDispatcher: virtualScrollingDispatcher
        } = this.instance.getWorkSpace();
        var {
            cellCountInsideLeftVirtualCell: cellCountInsideLeftVirtualCell,
            cellCountInsideTopVirtualRow: cellCountInsideTopVirtualRow
        } = virtualScrollingDispatcher;
        var groupCount = getGroupCount(this.instance.option("loadedResources"));
        var DOMMetaData = workspace.getDOMElementsMetaData();
        var allDayHeight = getAllDayHeight(workspace.option("showAllDayPanel"), workspace._isVerticalGroupedWorkSpace(), DOMMetaData);
        var rowCount = workspace._getRowCount();
        var {
            positionHelper: positionHelper,
            viewDataProvider: viewDataProvider
        } = workspace;
        var visibleDayDuration = viewDataProvider.getVisibleDayDuration(workspace.option("startDayHour"), workspace.option("endDayHour"), workspace.option("hoursInterval"));
        var cellDuration = getCellDuration(workspace.type, workspace.option("startDayHour"), workspace.option("endDayHour"), workspace.option("hoursInterval"));
        return {
            resources: this.instance.option("resources"),
            loadedResources: this.instance.option("loadedResources"),
            getAppointmentColor: this.instance.createGetAppointmentColor(),
            dataAccessors: this.instance._dataAccessors,
            isRenovatedAppointments: this.instance.option("isRenovatedAppointments"),
            appointmentRenderingStrategyName: this.appointmentRenderingStrategyName,
            adaptivityEnabled: this.instance.option("adaptivityEnabled"),
            rtlEnabled: this.instance.option("rtlEnabled"),
            startDayHour: this.instance._getCurrentViewOption("startDayHour"),
            endDayHour: this.instance._getCurrentViewOption("endDayHour"),
            maxAppointmentsPerCell: this.instance._getCurrentViewOption("maxAppointmentsPerCell"),
            currentDate: this.instance.option("currentDate"),
            isVirtualScrolling: this.instance.isVirtualScrolling(),
            leftVirtualCellCount: cellCountInsideLeftVirtualCell,
            topVirtualCellCount: cellCountInsideTopVirtualRow,
            intervalCount: workspace.option("intervalCount"),
            hoursInterval: workspace.option("hoursInterval"),
            showAllDayPanel: workspace.option("showAllDayPanel"),
            isGroupedAllDayPanel: workspace.isGroupedAllDayPanel(),
            modelGroups: this.instance._getCurrentViewOption("groups"),
            groupCount: groupCount,
            rowCount: rowCount,
            appointmentCountPerCell: this.instance.option("_appointmentCountPerCell"),
            appointmentOffset: this.instance.option("_appointmentOffset"),
            allowResizing: this.instance._allowResizing(),
            allowAllDayResizing: this.instance._allowAllDayResizing(),
            startViewDate: workspace.getStartViewDate(),
            groupOrientation: workspace._getRealGroupOrientation(),
            cellWidth: getCellWidth(DOMMetaData),
            cellHeight: getCellHeight(DOMMetaData),
            allDayHeight: allDayHeight,
            resizableStep: positionHelper.getResizableStep(),
            visibleDayDuration: visibleDayDuration,
            timeZoneCalculator: this.instance.timeZoneCalculator,
            timeZone: this.instance.option("timeZone"),
            firstDayOfWeek: this.instance.getFirstDayOfWeek(),
            viewStartDayHour: this.instance._getCurrentViewOption("startDayHour"),
            viewEndDayHour: this.instance._getCurrentViewOption("endDayHour"),
            viewType: workspace.type,
            endViewDate: workspace.getEndViewDate(),
            positionHelper: positionHelper,
            isGroupedByDate: workspace.isGroupedByDate(),
            cellDuration: cellDuration,
            cellDurationInMinutes: workspace.option("cellDuration"),
            viewDataProvider: workspace.viewDataProvider,
            supportAllDayRow: workspace.supportAllDayRow(),
            dateRange: workspace.getDateRange(),
            intervalDuration: workspace.getIntervalDuration(),
            allDayIntervalDuration: workspace.getIntervalDuration(true),
            isVerticalOrientation: workspace.isVerticalOrientation(),
            DOMMetaData: DOMMetaData,
            instance: this.instance,
            agendaDuration: workspace.option("agendaDuration")
        }
    }
    createAppointmentsMap(items) {
        var renderingStrategyOptions = this._getRenderingStrategyOptions();
        var {
            viewModel: viewModel,
            positionMap: positionMap
        } = this.appointmentViewModel.generate(items, renderingStrategyOptions);
        this._positionMap = positionMap;
        return viewModel
    }
    _isDataChanged(data) {
        var appointmentDataProvider = this.instance.appointmentDataProvider;
        var updatedData = appointmentDataProvider.getUpdatedAppointment();
        return updatedData === data || appointmentDataProvider.getUpdatedAppointmentKeys().some(item => data[item.key] === item.value)
    }
    _isAppointmentShouldAppear(currentAppointment, sourceAppointment) {
        return currentAppointment.needRepaint && sourceAppointment.needRemove
    }
    _isSettingChanged(settings, sourceSetting) {
        if (settings.length !== sourceSetting.length) {
            return true
        }
        var createSettingsToCompare = (settings, index) => {
            var currentSetting = settings[index];
            var leftVirtualCellCount = currentSetting.leftVirtualCellCount || 0;
            var topVirtualCellCount = currentSetting.topVirtualCellCount || 0;
            var columnIndex = currentSetting.columnIndex + leftVirtualCellCount;
            var rowIndex = currentSetting.rowIndex + topVirtualCellCount;
            var hMax = currentSetting.reduced ? currentSetting.hMax : void 0;
            var vMax = currentSetting.reduced ? currentSetting.vMax : void 0;
            return _extends({}, currentSetting, {
                columnIndex: columnIndex,
                rowIndex: rowIndex,
                positionByMap: void 0,
                topVirtualCellCount: void 0,
                leftVirtualCellCount: void 0,
                leftVirtualWidth: void 0,
                topVirtualHeight: void 0,
                hMax: hMax,
                vMax: vMax,
                info: {}
            })
        };
        for (var i = 0; i < settings.length; i++) {
            var newSettings = createSettingsToCompare(settings, i);
            var oldSettings = createSettingsToCompare(sourceSetting, i);
            if (oldSettings) {
                oldSettings.sortedIndex = newSettings.sortedIndex
            }
            if (!equalByValue(newSettings, oldSettings)) {
                return true
            }
        }
        return false
    }
    _getAssociatedSourceAppointment(currentAppointment, sourceAppointments) {
        for (var i = 0; i < sourceAppointments.length; i++) {
            var item = sourceAppointments[i];
            if (item.itemData === currentAppointment.itemData) {
                return item
            }
        }
        return null
    }
    _getDeletedAppointments(currentAppointments, sourceAppointments) {
        var result = [];
        for (var i = 0; i < sourceAppointments.length; i++) {
            var sourceAppointment = sourceAppointments[i];
            var currentAppointment = this._getAssociatedSourceAppointment(sourceAppointment, currentAppointments);
            if (!currentAppointment) {
                sourceAppointment.needRemove = true;
                result.push(sourceAppointment)
            }
        }
        return result
    }
    getRepaintedAppointments(currentAppointments, sourceAppointments) {
        if (0 === sourceAppointments.length || "agenda" === this.appointmentRenderingStrategyName) {
            return currentAppointments
        }
        currentAppointments.forEach(appointment => {
            var sourceAppointment = this._getAssociatedSourceAppointment(appointment, sourceAppointments);
            if (sourceAppointment) {
                appointment.needRepaint = this._isDataChanged(appointment.itemData) || this._isSettingChanged(appointment.settings, sourceAppointment.settings) || this._isAppointmentShouldAppear(appointment, sourceAppointment)
            }
        });
        return currentAppointments.concat(this._getDeletedAppointments(currentAppointments, sourceAppointments))
    }
    getRenderingStrategyInstance() {
        var renderingStrategy = this.appointmentViewModel.getRenderingStrategy();
        if (!renderingStrategy) {
            var options = this._getRenderingStrategyOptions();
            this.appointmentViewModel.initRenderingStrategy(options)
        }
        return this.appointmentViewModel.getRenderingStrategy()
    }
}
export default AppointmentLayoutManager;
