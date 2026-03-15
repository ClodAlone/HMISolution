/**
 * DevExtreme (cjs/ui/file_manager/ui.file_manager.notification_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.NotificationManagerStub = exports.NotificationManager = exports.MANAGER_ID_NAME = void 0;
var _guid = _interopRequireDefault(require("../../core/guid"));
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _extend = require("../../core/utils/extend");
var _icon = require("../../core/utils/icon");
var _uiFile_managerNotification = _interopRequireDefault(require("./ui.file_manager.notification.progress_panel"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _defineProperty(obj, key, value) {
    if (key in obj) {
        Object.defineProperty(obj, key, {
            value: value,
            enumerable: true,
            configurable: true,
            writable: true
        })
    } else {
        obj[key] = value
    }
    return obj
}

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

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var FILE_MANAGER_PROGRESS_BOX_CLASS = "dx-filemanager-progress-box";
var FILE_MANAGER_PROGRESS_BOX_ERROR_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-error");
var FILE_MANAGER_PROGRESS_BOX_IMAGE_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-image");
var FILE_MANAGER_PROGRESS_BOX_WRAPPER_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-wrapper");
var FILE_MANAGER_PROGRESS_BOX_COMMON_CLASS = "".concat(FILE_MANAGER_PROGRESS_BOX_CLASS, "-common");
var MANAGER_ID_NAME = "__operationInfoManager";
exports.MANAGER_ID_NAME = MANAGER_ID_NAME;
var ACTION_PROGRESS_STATUS = {
    default: "default",
    progress: "progress",
    error: "error",
    success: "success"
};
var NotificationManagerBase = function() {
    function NotificationManagerBase(_ref) {
        var onActionProgressStatusChanged = _ref.onActionProgressStatusChanged,
            isActual = _ref.isActual;
        this._id = (new _guid.default).toString();
        this._isActual = isActual || false;
        this._actionProgressStatus = ACTION_PROGRESS_STATUS.default;
        this._raiseActionProgress = onActionProgressStatusChanged
    }
    var _proto = NotificationManagerBase.prototype;
    _proto.getId = function() {
        return this._id
    };
    _proto.isActual = function() {
        return this._isActual
    };
    _proto.createErrorDetailsProgressBox = function($container, item, errorText) {
        var detailsItem = this._createDetailsItem($container, item);
        this.renderError(detailsItem.$wrapper, errorText)
    };
    _proto.renderError = function($container, errorText) {
        (0, _renderer.default)("<div>").text(errorText).addClass(FILE_MANAGER_PROGRESS_BOX_ERROR_CLASS).appendTo($container)
    };
    _proto.isActionProgressStatusDefault = function() {
        return this._actionProgressStatus === ACTION_PROGRESS_STATUS.default
    };
    _proto._createDetailsItem = function($container, item) {
        var $detailsItem = (0, _renderer.default)("<div>").appendTo($container);
        return this._createProgressBox($detailsItem, {
            commonText: item.commonText,
            imageUrl: item.imageUrl
        })
    };
    _proto._createProgressBox = function($container, options) {
        $container.addClass(FILE_MANAGER_PROGRESS_BOX_CLASS);
        if (options.imageUrl) {
            (0, _icon.getImageContainer)(options.imageUrl).addClass(FILE_MANAGER_PROGRESS_BOX_IMAGE_CLASS).appendTo($container)
        }
        var $wrapper = (0, _renderer.default)("<div>").addClass(FILE_MANAGER_PROGRESS_BOX_WRAPPER_CLASS).appendTo($container);
        var $commonText = (0, _renderer.default)("<div>").addClass(FILE_MANAGER_PROGRESS_BOX_COMMON_CLASS).text(options.commonText).appendTo($wrapper);
        return {
            $commonText: $commonText,
            $element: $container,
            $wrapper: $wrapper
        }
    };
    return NotificationManagerBase
}();
var NotificationManagerStub = function(_NotificationManagerB) {
    _inheritsLoose(NotificationManagerStub, _NotificationManagerB);

    function NotificationManagerStub() {
        return _NotificationManagerB.apply(this, arguments) || this
    }
    var _proto2 = NotificationManagerStub.prototype;
    _proto2.addOperation = function() {
        return _defineProperty({}, MANAGER_ID_NAME, this._id)
    };
    _proto2.addOperationDetails = function() {};
    _proto2.updateOperationItemProgress = function() {};
    _proto2.completeOperationItem = function() {};
    _proto2.completeOperation = function() {};
    _proto2.completeSingleOperationWithError = function() {};
    _proto2.addOperationDetailsError = function() {};
    _proto2.handleDimensionChanged = function() {
        return false
    };
    _proto2.ensureProgressPanelCreated = function() {};
    _proto2.tryHideActionProgress = function() {
        this._updateActionProgress("", ACTION_PROGRESS_STATUS.default)
    };
    _proto2.updateActionProgressStatus = function() {
        this._updateActionProgress("", ACTION_PROGRESS_STATUS.default)
    };
    _proto2._updateActionProgress = function(message, status) {
        if (status !== ACTION_PROGRESS_STATUS.default && status !== ACTION_PROGRESS_STATUS.progress) {
            return
        }
        this._actionProgressStatus = status;
        this._raiseActionProgress(message, status)
    };
    _proto2.hasNoOperations = function() {
        return true
    };
    _createClass(NotificationManagerStub, [{
        key: "_operationInProgressCount",
        get: function() {
            return 0
        },
        set: function(value) {}
    }, {
        key: "_failedOperationCount",
        get: function() {
            return 0
        },
        set: function(value) {}
    }]);
    return NotificationManagerStub
}(NotificationManagerBase);
exports.NotificationManagerStub = NotificationManagerStub;
var NotificationManager = function(_NotificationManagerB2) {
    _inheritsLoose(NotificationManager, _NotificationManagerB2);

    function NotificationManager(options) {
        var _this;
        _this = _NotificationManagerB2.call(this, options) || this;
        _this._failedOperationCount = 0;
        _this._operationInProgressCount = 0;
        return _this
    }
    var _proto3 = NotificationManager.prototype;
    _proto3.addOperation = function(processingMessage, allowCancel, allowProgressAutoUpdate) {
        this._operationInProgressCount++;
        var operationInfo = this._progressPanel.addOperation(processingMessage, allowCancel, allowProgressAutoUpdate);
        operationInfo[MANAGER_ID_NAME] = this._id;
        this._updateActionProgress(processingMessage, ACTION_PROGRESS_STATUS.progress);
        return operationInfo
    };
    _proto3.addOperationDetails = function(operationInfo, details, showCloseButton) {
        this._progressPanel.addOperationDetails(operationInfo, details, showCloseButton)
    };
    _proto3.updateOperationItemProgress = function(operationInfo, itemIndex, itemProgress, commonProgress) {
        this._progressPanel.updateOperationItemProgress(operationInfo, itemIndex, itemProgress, commonProgress)
    };
    _proto3.completeOperationItem = function(operationInfo, itemIndex, commonProgress) {
        this._progressPanel.completeOperationItem(operationInfo, itemIndex, commonProgress)
    };
    _proto3.completeOperation = function(operationInfo, commonText, isError, statusText) {
        this._operationInProgressCount--;
        if (isError) {
            this._failedOperationCount++
        }
        this._progressPanel.completeOperation(operationInfo, commonText, isError, statusText)
    };
    _proto3.completeSingleOperationWithError = function(operationInfo, errorInfo) {
        this._progressPanel.completeSingleOperationWithError(operationInfo, errorInfo.detailErrorText);
        this._notifyError(errorInfo)
    };
    _proto3.addOperationDetailsError = function(operationInfo, errorInfo) {
        this._progressPanel.addOperationDetailsError(operationInfo, errorInfo.itemIndex, errorInfo.detailErrorText);
        this._notifyError(errorInfo)
    };
    _proto3.handleDimensionChanged = function() {
        if (this._progressPanel) {
            this._progressPanel.$element().detach()
        }
        return true
    };
    _proto3.ensureProgressPanelCreated = function(container, options) {
        var _this2 = this;
        if (!this._progressPanel) {
            var $progressPanelElement = (0, _renderer.default)("<div>").appendTo(container);
            var ProgressPanelClass = this._getProgressPanelComponent();
            this._progressPanel = new ProgressPanelClass($progressPanelElement, (0, _extend.extend)({}, options, {
                onOperationClosed: function(_ref3) {
                    var info = _ref3.info;
                    return _this2._onProgressPanelOperationClosed(info)
                }
            }))
        } else {
            this._progressPanel.$element().appendTo(container)
        }
    };
    _proto3._getProgressPanelComponent = function() {
        return _uiFile_managerNotification.default
    };
    _proto3._onProgressPanelOperationClosed = function(operationInfo) {
        if (operationInfo.hasError) {
            this._failedOperationCount--;
            this.tryHideActionProgress()
        }
    };
    _proto3.tryHideActionProgress = function() {
        if (this.hasNoOperations()) {
            this._updateActionProgress("", ACTION_PROGRESS_STATUS.default)
        }
    };
    _proto3.updateActionProgressStatus = function(operationInfo) {
        if (operationInfo) {
            var status = 0 === this._failedOperationCount ? ACTION_PROGRESS_STATUS.success : ACTION_PROGRESS_STATUS.error;
            this._updateActionProgress("", status)
        }
    };
    _proto3._notifyError = function(errorInfo) {
        var status = this.hasNoOperations() ? ACTION_PROGRESS_STATUS.default : ACTION_PROGRESS_STATUS.error;
        this._updateActionProgress(errorInfo.commonErrorText, status)
    };
    _proto3._updateActionProgress = function(message, status) {
        this._actionProgressStatus = status;
        this._raiseActionProgress(message, status)
    };
    _proto3.hasNoOperations = function() {
        return 0 === this._operationInProgressCount && 0 === this._failedOperationCount
    };
    _createClass(NotificationManager, [{
        key: "_operationInProgressCount",
        get: function() {
            return this._operationInProgressCountInternal
        },
        set: function(value) {
            this._operationInProgressCountInternal = value
        }
    }, {
        key: "_failedOperationCount",
        get: function() {
            return this._failedOperationCountInternal
        },
        set: function(value) {
            this._failedOperationCountInternal = value
        }
    }]);
    return NotificationManager
}(NotificationManagerBase);
exports.NotificationManager = NotificationManager;
