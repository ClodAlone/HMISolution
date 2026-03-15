/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.notification_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Guid from "../../core/guid";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    getImageContainer
} from "../../core/utils/icon";
import FileManagerProgressPanel from "./ui.file_manager.notification.progress_panel";
var FILE_MANAGER_PROGRESS_BOX_CLASS = "dx-filemanager-progress-box";
var FILE_MANAGER_PROGRESS_BOX_ERROR_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-error");
var FILE_MANAGER_PROGRESS_BOX_IMAGE_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-image");
var FILE_MANAGER_PROGRESS_BOX_WRAPPER_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-wrapper");
var FILE_MANAGER_PROGRESS_BOX_COMMON_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-common");
var MANAGER_ID_NAME = "__operationInfoManager";
var ACTION_PROGRESS_STATUS = {
    default: "default",
    progress: "progress",
    error: "error",
    success: "success"
};
class NotificationManagerBase {
    constructor(_ref) {
        var {
            onActionProgressStatusChanged: onActionProgressStatusChanged,
            isActual: isActual
        } = _ref;
        this._id = (new Guid).toString();
        this._isActual = isActual || false;
        this._actionProgressStatus = ACTION_PROGRESS_STATUS.default;
        this._raiseActionProgress = onActionProgressStatusChanged
    }
    getId() {
        return this._id
    }
    isActual() {
        return this._isActual
    }
    createErrorDetailsProgressBox($container, item, errorText) {
        var detailsItem = this._createDetailsItem($container, item);
        this.renderError(detailsItem.$wrapper, errorText)
    }
    renderError($container, errorText) {
        $("<div>").text(errorText).addClass(FILE_MANAGER_PROGRESS_BOX_ERROR_CLASS).appendTo($container)
    }
    isActionProgressStatusDefault() {
        return this._actionProgressStatus === ACTION_PROGRESS_STATUS.default
    }
    _createDetailsItem($container, item) {
        var $detailsItem = $("<div>").appendTo($container);
        return this._createProgressBox($detailsItem, {
            commonText: item.commonText,
            imageUrl: item.imageUrl
        })
    }
    _createProgressBox($container, options) {
        $container.addClass(FILE_MANAGER_PROGRESS_BOX_CLASS);
        if (options.imageUrl) {
            getImageContainer(options.imageUrl).addClass(FILE_MANAGER_PROGRESS_BOX_IMAGE_CLASS).appendTo($container)
        }
        var $wrapper = $("<div>").addClass(FILE_MANAGER_PROGRESS_BOX_WRAPPER_CLASS).appendTo($container);
        var $commonText = $("<div>").addClass(FILE_MANAGER_PROGRESS_BOX_COMMON_CLASS).text(options.commonText).appendTo($wrapper);
        return {
            $commonText: $commonText,
            $element: $container,
            $wrapper: $wrapper
        }
    }
}
class NotificationManagerStub extends NotificationManagerBase {
    addOperation() {
        return {
            [MANAGER_ID_NAME]: this._id
        }
    }
    addOperationDetails() {}
    updateOperationItemProgress() {}
    completeOperationItem() {}
    completeOperation() {}
    completeSingleOperationWithError() {}
    addOperationDetailsError() {}
    handleDimensionChanged() {
        return false
    }
    ensureProgressPanelCreated() {}
    tryHideActionProgress() {
        this._updateActionProgress("", ACTION_PROGRESS_STATUS.default)
    }
    updateActionProgressStatus() {
        this._updateActionProgress("", ACTION_PROGRESS_STATUS.default)
    }
    _updateActionProgress(message, status) {
        if (status !== ACTION_PROGRESS_STATUS.default && status !== ACTION_PROGRESS_STATUS.progress) {
            return
        }
        this._actionProgressStatus = status;
        this._raiseActionProgress(message, status)
    }
    hasNoOperations() {
        return true
    }
    get _operationInProgressCount() {
        return 0
    }
    set _operationInProgressCount(value) {}
    get _failedOperationCount() {
        return 0
    }
    set _failedOperationCount(value) {}
}
class NotificationManager extends NotificationManagerBase {
    constructor(options) {
        super(options);
        this._failedOperationCount = 0;
        this._operationInProgressCount = 0
    }
    addOperation(processingMessage, allowCancel, allowProgressAutoUpdate) {
        this._operationInProgressCount++;
        var operationInfo = this._progressPanel.addOperation(processingMessage, allowCancel, allowProgressAutoUpdate);
        operationInfo[MANAGER_ID_NAME] = this._id;
        this._updateActionProgress(processingMessage, ACTION_PROGRESS_STATUS.progress);
        return operationInfo
    }
    addOperationDetails(operationInfo, details, showCloseButton) {
        this._progressPanel.addOperationDetails(operationInfo, details, showCloseButton)
    }
    updateOperationItemProgress(operationInfo, itemIndex, itemProgress, commonProgress) {
        this._progressPanel.updateOperationItemProgress(operationInfo, itemIndex, itemProgress, commonProgress)
    }
    completeOperationItem(operationInfo, itemIndex, commonProgress) {
        this._progressPanel.completeOperationItem(operationInfo, itemIndex, commonProgress)
    }
    completeOperation(operationInfo, commonText, isError, statusText) {
        this._operationInProgressCount--;
        if (isError) {
            this._failedOperationCount++
        }
        this._progressPanel.completeOperation(operationInfo, commonText, isError, statusText)
    }
    completeSingleOperationWithError(operationInfo, errorInfo) {
        this._progressPanel.completeSingleOperationWithError(operationInfo, errorInfo.detailErrorText);
        this._notifyError(errorInfo)
    }
    addOperationDetailsError(operationInfo, errorInfo) {
        this._progressPanel.addOperationDetailsError(operationInfo, errorInfo.itemIndex, errorInfo.detailErrorText);
        this._notifyError(errorInfo)
    }
    handleDimensionChanged() {
        if (this._progressPanel) {
            this._progressPanel.$element().detach()
        }
        return true
    }
    ensureProgressPanelCreated(container, options) {
        if (!this._progressPanel) {
            var $progressPanelElement = $("<div>").appendTo(container);
            var ProgressPanelClass = this._getProgressPanelComponent();
            this._progressPanel = new ProgressPanelClass($progressPanelElement, extend({}, options, {
                onOperationClosed: _ref2 => {
                    var {
                        info: info
                    } = _ref2;
                    return this._onProgressPanelOperationClosed(info)
                }
            }))
        } else {
            this._progressPanel.$element().appendTo(container)
        }
    }
    _getProgressPanelComponent() {
        return FileManagerProgressPanel
    }
    _onProgressPanelOperationClosed(operationInfo) {
        if (operationInfo.hasError) {
            this._failedOperationCount--;
            this.tryHideActionProgress()
        }
    }
    tryHideActionProgress() {
        if (this.hasNoOperations()) {
            this._updateActionProgress("", ACTION_PROGRESS_STATUS.default)
        }
    }
    updateActionProgressStatus(operationInfo) {
        if (operationInfo) {
            var status = 0 === this._failedOperationCount ? ACTION_PROGRESS_STATUS.success : ACTION_PROGRESS_STATUS.error;
            this._updateActionProgress("", status)
        }
    }
    _notifyError(errorInfo) {
        var status = this.hasNoOperations() ? ACTION_PROGRESS_STATUS.default : ACTION_PROGRESS_STATUS.error;
        this._updateActionProgress(errorInfo.commonErrorText, status)
    }
    _updateActionProgress(message, status) {
        this._actionProgressStatus = status;
        this._raiseActionProgress(message, status)
    }
    hasNoOperations() {
        return 0 === this._operationInProgressCount && 0 === this._failedOperationCount
    }
    get _operationInProgressCount() {
        return this._operationInProgressCountInternal
    }
    set _operationInProgressCount(value) {
        this._operationInProgressCountInternal = value
    }
    get _failedOperationCount() {
        return this._failedOperationCountInternal
    }
    set _failedOperationCount(value) {
        this._failedOperationCountInternal = value
    }
}
export {
    NotificationManager,
    NotificationManagerStub,
    MANAGER_ID_NAME
};
