/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.editing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    Deferred
} from "../../core/utils/deferred";
import {
    each
} from "../../core/utils/iterator";
import {
    format
} from "../../core/utils/string";
import messageLocalization from "../../localization/message";
import Widget from "../widget/ui.widget";
import FileManagerDialogManager from "./ui.file_manager.dialog_manager";
import FileManagerFileUploader from "./ui.file_manager.file_uploader";
import {
    FileManagerMessages
} from "./ui.file_manager.messages";
class FileManagerEditingControl extends Widget {
    _initMarkup() {
        super._initMarkup();
        this._initActions();
        this._controller = this.option("controller");
        this._controller.on("EditActionStarting", this._onEditActionStarting.bind(this));
        this._controller.on("EditActionResultAcquired", this._onEditActionResultAcquired.bind(this));
        this._controller.on("EditActionItemError", this._onEditActionItemError.bind(this));
        this._controller.on("EditActionError", this._onEditActionError.bind(this));
        this._controller.on("CompleteEditActionItem", this._onCompleteEditActionItem.bind(this));
        this._controller.on("CompleteEditAction", this._onCompleteEditAction.bind(this));
        this._model = this.option("model");
        this._uploadOperationInfoMap = {};
        this._dialogManager = new FileManagerDialogManager(this.$element(), {
            chooseDirectoryDialog: {
                provider: this._controller._fileProvider,
                getDirectories: this._controller.getDirectories.bind(this._controller),
                getCurrentDirectory: this._controller.getCurrentDirectory.bind(this._controller)
            },
            rtlEnabled: this.option("rtlEnabled"),
            onDialogClosed: this._onDialogClosed.bind(this)
        });
        this._fileUploader = this._createFileUploader();
        var notificationControl = this.option("notificationControl");
        if (notificationControl) {
            this._initNotificationControl(notificationControl)
        }
        this._createMetadataMap()
    }
    _initNotificationControl(notificationControl) {
        this._notificationControl = notificationControl;
        this._notificationControl.option({
            onOperationCanceled: _ref => {
                var {
                    info: info
                } = _ref;
                return this._onCancelUploadSession(info)
            },
            onOperationItemCanceled: _ref2 => {
                var {
                    item: item,
                    itemIndex: itemIndex
                } = _ref2;
                return this._onCancelFileUpload(item, itemIndex)
            }
        })
    }
    _getFileUploaderComponent() {
        return FileManagerFileUploader
    }
    _createFileUploader() {
        var $fileUploader = $("<div>").appendTo(this.$element());
        return this._createComponent($fileUploader, this._getFileUploaderComponent(), {
            getController: this._getFileUploaderController.bind(this),
            dropZonePlaceholderContainer: this.option("uploadDropZonePlaceholderContainer"),
            onUploadSessionStarted: e => this._onUploadSessionStarted(e),
            onUploadProgress: e => this._onUploadProgress(e)
        })
    }
    setUploaderDropZone($element) {
        this._fileUploader.option("dropZone", $element)
    }
    setUploaderSplitterElement(element) {
        this._fileUploader.option("splitterElement", element)
    }
    _getFileUploaderController() {
        var uploadDirectory = this.uploadDirectoryInfo.fileItem;
        return {
            chunkSize: this._controller.getFileUploadChunkSize(),
            uploadFileChunk: (fileData, chunksInfo) => this._controller.uploadFileChunk(fileData, chunksInfo, uploadDirectory),
            abortFileUpload: (fileData, chunksInfo) => this._controller.abortFileUpload(fileData, chunksInfo, uploadDirectory)
        }
    }
    _createMetadataMap() {
        this._metadataMap = {
            create: {
                action: arg => this._tryCreate(arg),
                affectsAllItems: true,
                singleItemProcessingMessage: messageLocalization.format("dxFileManager-editingCreateSingleItemProcessingMessage"),
                singleItemSuccessMessage: messageLocalization.format("dxFileManager-editingCreateSingleItemSuccessMessage"),
                singleItemErrorMessage: messageLocalization.format("dxFileManager-editingCreateSingleItemErrorMessage"),
                commonErrorMessage: messageLocalization.format("dxFileManager-editingCreateCommonErrorMessage")
            },
            rename: {
                action: arg => this._tryRename(arg),
                singleItemProcessingMessage: messageLocalization.format("dxFileManager-editingRenameSingleItemProcessingMessage"),
                singleItemSuccessMessage: messageLocalization.format("dxFileManager-editingRenameSingleItemSuccessMessage"),
                singleItemErrorMessage: messageLocalization.format("dxFileManager-editingRenameSingleItemErrorMessage"),
                commonErrorMessage: messageLocalization.format("dxFileManager-editingRenameCommonErrorMessage")
            },
            delete: {
                action: arg => this._tryDelete(arg),
                singleItemProcessingMessage: messageLocalization.format("dxFileManager-editingDeleteSingleItemProcessingMessage"),
                multipleItemsProcessingMessage: messageLocalization.format("dxFileManager-editingDeleteMultipleItemsProcessingMessage"),
                singleItemSuccessMessage: messageLocalization.format("dxFileManager-editingDeleteSingleItemSuccessMessage"),
                multipleItemsSuccessMessage: messageLocalization.format("dxFileManager-editingDeleteMultipleItemsSuccessMessage"),
                singleItemErrorMessage: messageLocalization.format("dxFileManager-editingDeleteSingleItemErrorMessage"),
                multipleItemsErrorMessage: messageLocalization.format("dxFileManager-editingDeleteMultipleItemsErrorMessage"),
                commonErrorMessage: messageLocalization.format("dxFileManager-editingDeleteCommonErrorMessage")
            },
            move: {
                action: arg => this._tryMove(arg),
                singleItemProcessingMessage: messageLocalization.format("dxFileManager-editingMoveSingleItemProcessingMessage"),
                multipleItemsProcessingMessage: messageLocalization.format("dxFileManager-editingMoveMultipleItemsProcessingMessage"),
                singleItemSuccessMessage: messageLocalization.format("dxFileManager-editingMoveSingleItemSuccessMessage"),
                multipleItemsSuccessMessage: messageLocalization.format("dxFileManager-editingMoveMultipleItemsSuccessMessage"),
                singleItemErrorMessage: messageLocalization.format("dxFileManager-editingMoveSingleItemErrorMessage"),
                multipleItemsErrorMessage: messageLocalization.format("dxFileManager-editingMoveMultipleItemsErrorMessage"),
                commonErrorMessage: messageLocalization.format("dxFileManager-editingMoveCommonErrorMessage")
            },
            copy: {
                action: arg => this._tryCopy(arg),
                singleItemProcessingMessage: messageLocalization.format("dxFileManager-editingCopySingleItemProcessingMessage"),
                multipleItemsProcessingMessage: messageLocalization.format("dxFileManager-editingCopyMultipleItemsProcessingMessage"),
                singleItemSuccessMessage: messageLocalization.format("dxFileManager-editingCopySingleItemSuccessMessage"),
                multipleItemsSuccessMessage: messageLocalization.format("dxFileManager-editingCopyMultipleItemsSuccessMessage"),
                singleItemErrorMessage: messageLocalization.format("dxFileManager-editingCopySingleItemErrorMessage"),
                multipleItemsErrorMessage: messageLocalization.format("dxFileManager-editingCopyMultipleItemsErrorMessage"),
                commonErrorMessage: messageLocalization.format("dxFileManager-editingCopyCommonErrorMessage")
            },
            upload: {
                action: arg => this._tryUpload(arg),
                allowCancel: true,
                allowItemProgress: true,
                singleItemProcessingMessage: messageLocalization.format("dxFileManager-editingUploadSingleItemProcessingMessage"),
                multipleItemsProcessingMessage: messageLocalization.format("dxFileManager-editingUploadMultipleItemsProcessingMessage"),
                singleItemSuccessMessage: messageLocalization.format("dxFileManager-editingUploadSingleItemSuccessMessage"),
                multipleItemsSuccessMessage: messageLocalization.format("dxFileManager-editingUploadMultipleItemsSuccessMessage"),
                singleItemErrorMessage: messageLocalization.format("dxFileManager-editingUploadSingleItemErrorMessage"),
                multipleItemsErrorMessage: messageLocalization.format("dxFileManager-editingUploadMultipleItemsErrorMessage"),
                canceledMessage: messageLocalization.format("dxFileManager-editingUploadCanceledMessage")
            },
            download: {
                action: arg => this._download(arg)
            },
            getItemContent: {
                action: arg => this._getItemContent(arg)
            },
            getItems: {
                singleItemProcessingMessage: "",
                singleItemErrorMessage: messageLocalization.format("dxFileManager-errorDirectoryOpenFailed"),
                commonErrorMessage: messageLocalization.format("dxFileManager-errorDirectoryOpenFailed")
            }
        }
    }
    getCommandActions() {
        var result = {};
        each(this._metadataMap, name => {
            if (Object.prototype.hasOwnProperty.call(this._metadataMap, name)) {
                result[name] = arg => this._executeAction(name, arg)
            }
        });
        return result
    }
    _executeAction(actionName, arg) {
        var actionMetadata = this._metadataMap[actionName];
        return actionMetadata ? actionMetadata.action(arg) : null
    }
    _onCancelUploadSession(info) {
        this._fileUploader.cancelUpload(info.uploadSessionId)
    }
    _onCancelFileUpload(item, itemIndex) {
        this._fileUploader.cancelFileUpload(item.info.uploadSessionId, itemIndex)
    }
    _onUploadProgress(_ref3) {
        var {
            sessionId: sessionId,
            fileIndex: fileIndex,
            commonValue: commonValue,
            fileValue: fileValue
        } = _ref3;
        var operationInfo = this._uploadOperationInfoMap[sessionId];
        this._notificationControl.updateOperationItemProgress(operationInfo, fileIndex, 100 * fileValue, 100 * commonValue)
    }
    _onUploadSessionStarted(_ref4) {
        var {
            sessionInfo: sessionInfo
        } = _ref4;
        this._controller.processUploadSession(sessionInfo, this.uploadDirectoryInfo)
    }
    _onEditActionStarting(actionInfo) {
        var actionMetadata = this._metadataMap[actionInfo.name];
        var context = new FileManagerActionContext(actionMetadata, actionInfo.itemInfos, actionInfo.directory);
        var operationInfo = this._notificationControl.addOperation(context.processingMessage, actionMetadata.allowCancel, !actionMetadata.allowItemProgress);
        extend(actionInfo.customData, {
            context: context,
            operationInfo: operationInfo
        });
        switch (actionInfo.name) {
            case "upload":
                var sessionId = actionInfo.customData.sessionInfo.sessionId;
                operationInfo.uploadSessionId = sessionId;
                this._uploadOperationInfoMap[sessionId] = operationInfo;
                break;
            case "rename":
                actionInfo.customData.context.itemNewName = actionInfo.customData.itemNewName
        }
    }
    _onEditActionResultAcquired(actionInfo) {
        var {
            context: context,
            operationInfo: operationInfo
        } = actionInfo.customData;
        context.singleRequest = actionInfo.singleRequest;
        var details = context.itemInfos.map(itemInfo => this._getItemProgressDisplayInfo(itemInfo));
        this._notificationControl.addOperationDetails(operationInfo, details, context.actionMetadata.allowCancel)
    }
    _onEditActionError(actionInfo, errorInfo) {
        var {
            context: context,
            operationInfo: operationInfo
        } = actionInfo.customData;
        context.singleRequest = actionInfo.singleRequest;
        this._handleActionError(operationInfo, context, errorInfo);
        this._completeAction(operationInfo, context)
    }
    _onEditActionItemError(actionInfo, errorInfo) {
        var {
            context: context,
            operationInfo: operationInfo
        } = actionInfo.customData;
        this._handleActionError(operationInfo, context, errorInfo)
    }
    _onCompleteEditActionItem(actionInfo, info) {
        var {
            context: context,
            operationInfo: operationInfo
        } = actionInfo.customData;
        if (!info.result || !info.result.canceled) {
            context.completeOperationItem(info.index);
            this._notificationControl.completeOperationItem(operationInfo, info.index, context.commonProgress)
        }
    }
    _onCompleteEditAction(actionInfo) {
        var {
            context: context,
            operationInfo: operationInfo
        } = actionInfo.customData;
        this._completeAction(operationInfo, context);
        if ("upload" === actionInfo.name) {
            delete this._uploadOperationInfoMap[actionInfo.customData.sessionInfo.sessionId]
        }
    }
    _tryCreate(parentDirectories) {
        var parentDirectoryInfo = parentDirectories && parentDirectories[0] || this._getCurrentDirectory();
        var newDirName = messageLocalization.format("dxFileManager-newDirectoryName");
        return this._showDialog(this._dialogManager.getCreateItemDialog(), newDirName).then(_ref5 => {
            var {
                name: name
            } = _ref5;
            return this._controller.createDirectory(parentDirectoryInfo, name)
        })
    }
    _tryRename(itemInfos) {
        var itemInfo = itemInfos && itemInfos[0] || this._model.getMultipleSelectedItems()[0];
        if (!itemInfo) {
            return (new Deferred).reject().promise()
        }
        return this._showDialog(this._dialogManager.getRenameItemDialog(), itemInfo.fileItem.name).then(_ref6 => {
            var {
                name: name
            } = _ref6;
            return this._controller.renameItem(itemInfo, name)
        })
    }
    _tryDelete(itemInfos) {
        itemInfos = itemInfos || this._model.getMultipleSelectedItems();
        if (0 === itemInfos.length) {
            return (new Deferred).reject().promise()
        }
        var itemName = itemInfos[0].fileItem.name;
        var itemCount = itemInfos.length;
        return this._showDialog(this._dialogManager.getDeleteItemDialog(), {
            itemName: itemName,
            itemCount: itemCount
        }).then(() => this._controller.deleteItems(itemInfos))
    }
    _tryMove(itemInfos) {
        itemInfos = itemInfos || this._model.getMultipleSelectedItems();
        if (0 === itemInfos.length) {
            return (new Deferred).reject().promise()
        }
        return this._showDialog(this._dialogManager.getMoveDialog(itemInfos)).then(_ref7 => {
            var {
                folder: folder
            } = _ref7;
            return this._controller.moveItems(itemInfos, folder)
        })
    }
    _tryCopy(itemInfos) {
        itemInfos = itemInfos || this._model.getMultipleSelectedItems();
        if (0 === itemInfos.length) {
            return (new Deferred).reject().promise()
        }
        return this._showDialog(this._dialogManager.getCopyDialog(itemInfos)).then(_ref8 => {
            var {
                folder: folder
            } = _ref8;
            return this._controller.copyItems(itemInfos, folder)
        })
    }
    _tryUpload(destinationFolder) {
        this._uploadDirectoryInfo = null === destinationFolder || void 0 === destinationFolder ? void 0 : destinationFolder[0];
        this._fileUploader.tryUpload()
    }
    _download(itemInfos) {
        itemInfos = itemInfos || this._model.getMultipleSelectedItems();
        if (0 === itemInfos.length) {
            return (new Deferred).reject().promise()
        }
        return this._controller.downloadItems(itemInfos)
    }
    _getItemContent(itemInfos) {
        itemInfos = itemInfos || this._model.getMultipleSelectedItems();
        return this._controller.getItemContent(itemInfos)
    }
    _completeAction(operationInfo, context) {
        this._notificationControl.completeOperation(operationInfo, context.completionMessage, !context.success, context.statusText);
        if (context.hasModifiedItems()) {
            this._raiseOnSuccess(context.onlyFiles)
        }
    }
    _handleActionError(operationInfo, context, errorInfo) {
        operationInfo.hasError = true;
        if (context.singleRequest) {
            this._handleSingleRequestActionError(operationInfo, context, errorInfo)
        } else {
            this._handleMultipleRequestActionError(operationInfo, context, errorInfo)
        }
    }
    _handleSingleRequestActionError(operationInfo, context, errorInfo) {
        var itemInfo = context.getItemForSingleRequestError();
        var itemName = context.itemNewName;
        var errorText = this._getErrorText(errorInfo, itemInfo, itemName);
        context.processSingleRequestError(errorText);
        var operationErrorInfo = this._getOperationErrorInfo(context);
        this._notificationControl.completeSingleOperationWithError(operationInfo, operationErrorInfo);
        if (context.multipleItems) {
            this._raiseOnSuccess(context.onlyFiles)
        }
    }
    _handleMultipleRequestActionError(operationInfo, context, errorInfo) {
        var itemInfo = context.getItemForMultipleRequestError(errorInfo.index);
        var errorText = this._getErrorText(errorInfo, itemInfo);
        context.processMultipleRequestError(errorInfo.index, errorText);
        var operationErrorInfo = this._getOperationErrorInfo(context);
        this._notificationControl.addOperationDetailsError(operationInfo, operationErrorInfo)
    }
    _getOperationErrorInfo(context) {
        var detailError = context.errorState.currentDetailError;
        return {
            commonErrorText: context.errorState.commonErrorText,
            item: detailError.itemInfo ? this._getItemProgressDisplayInfo(detailError.itemInfo) : null,
            itemIndex: detailError.itemIndex,
            detailErrorText: detailError.errorText
        }
    }
    _getErrorText(errorInfo, itemInfo, itemName) {
        itemName = itemName || (null === itemInfo || void 0 === itemInfo ? void 0 : itemInfo.fileItem.name);
        var errorText = errorInfo.errorText || FileManagerMessages.get(errorInfo.errorCode, itemName);
        var errorArgs = {
            fileSystemItem: null === itemInfo || void 0 === itemInfo ? void 0 : itemInfo.fileItem,
            errorCode: errorInfo.errorCode,
            errorText: errorText
        };
        this._raiseOnError(errorArgs);
        return errorArgs.errorText
    }
    _getItemProgressDisplayInfo(itemInfo) {
        return {
            commonText: itemInfo.fileItem.name,
            imageUrl: this._getItemThumbnail(itemInfo)
        }
    }
    _showDialog(dialog, dialogArgument) {
        this._dialogDeferred = new Deferred;
        dialog.show(dialogArgument);
        return this._dialogDeferred.promise()
    }
    _onDialogClosed(e) {
        var result = e.dialogResult;
        if (result) {
            this._dialogDeferred.resolve(result)
        } else {
            this._dialogDeferred.reject()
        }
    }
    updateDialogRtl(value) {
        this._dialogManager.updateDialogRtl(value)
    }
    _getItemThumbnail(item) {
        var itemThumbnailGetter = this.option("getItemThumbnail");
        if (!itemThumbnailGetter) {
            return null
        }
        var info = itemThumbnailGetter(item);
        return info ? info.thumbnail : null
    }
    _initActions() {
        this._actions = {
            onSuccess: this._createActionByOption("onSuccess"),
            onError: this._createActionByOption("onError")
        }
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            model: {
                getMultipleSelectedItems: null
            },
            notificationControl: null,
            getItemThumbnail: null,
            onSuccess: null,
            onError: null
        })
    }
    _optionChanged(args) {
        var name = args.name;
        switch (name) {
            case "model":
                this.repaint();
                break;
            case "notificationControl":
                this._initNotificationControl(args.value);
                break;
            case "getItemThumbnail":
                break;
            case "uploadDropZonePlaceholderContainer":
                this._fileUploader.option("dropZonePlaceholderContainer", args.value);
                break;
            case "onSuccess":
            case "onError":
                this._actions[name] = this._createActionByOption(name);
                break;
            default:
                super._optionChanged(args)
        }
    }
    _raiseOnSuccess(updatedOnlyFiles) {
        this._actions.onSuccess({
            updatedOnlyFiles: updatedOnlyFiles
        })
    }
    _raiseOnError(args) {
        this._actions.onError(args)
    }
    _getCurrentDirectory() {
        return this._controller.getCurrentDirectory()
    }
    get uploadDirectoryInfo() {
        return this._uploadDirectoryInfo || this._getCurrentDirectory()
    }
}
class FileManagerActionContext {
    constructor(actionMetadata, itemInfos, directoryInfo) {
        this._actionMetadata = actionMetadata;
        this._itemInfos = itemInfos;
        this._onlyFiles = !this._actionMetadata.affectsAllItems && this._itemInfos.every(info => !info.fileItem.isDirectory);
        this._items = this._itemInfos.map(itemInfo => itemInfo.fileItem);
        this._multipleItems = this._items.length > 1;
        this._location = directoryInfo.getDisplayName();
        this._singleRequest = true;
        this._completedItems = [];
        this._commonProgress = 0;
        this._errorState = {
            failedCount: 0
        };
        this._itemNewName = ""
    }
    completeOperationItem(itemIndex) {
        if (this._singleRequest) {
            this._completedItems = [...this._items]
        } else {
            var item = this._items[itemIndex];
            this._completedItems.push(item)
        }
        if (!this._actionMetadata.allowItemProgress) {
            this._commonProgress = this._completedItems.length / this._items.length * 100
        }
    }
    processSingleRequestError(errorText) {
        this._errorState.failedCount = 1;
        this._errorState.commonErrorText = this._multipleItems ? this._actionMetadata.commonErrorMessage : this._actionMetadata.singleItemErrorMessage;
        var itemIndex = this._multipleItems ? -1 : 1;
        var itemInfo = this.getItemForSingleRequestError();
        this._setCurrentDetailError(itemIndex, itemInfo, errorText)
    }
    processMultipleRequestError(itemIndex, errorText) {
        this._errorState.failedCount++;
        this._errorState.commonErrorText = this._errorState.failedCount > 1 ? format(this._actionMetadata.multipleItemsErrorMessage, this._errorState.failedCount) : this._actionMetadata.singleItemErrorMessage;
        var itemInfo = this.getItemForMultipleRequestError(itemIndex);
        this._setCurrentDetailError(itemIndex, itemInfo, errorText)
    }
    hasModifiedItems() {
        return this._hasCompletedItems() || this._singleRequest && !this.success && this._multipleItems
    }
    getItemForSingleRequestError() {
        return this._multipleItems ? null : this._itemInfos[0]
    }
    getItemForMultipleRequestError(itemIndex) {
        return this._itemInfos[itemIndex]
    }
    _setCurrentDetailError(itemIndex, itemInfo, errorText) {
        this._errorState.currentDetailError = {
            itemIndex: itemIndex,
            itemInfo: itemInfo,
            errorText: errorText
        }
    }
    _hasCompletedItems() {
        return this._completedItems.length > 0
    }
    get actionMetadata() {
        return this._actionMetadata
    }
    get itemInfos() {
        return this._itemInfos
    }
    get itemNewName() {
        return this._itemNewName
    }
    set itemNewName(value) {
        this._itemNewName = value
    }
    get errorState() {
        return this._errorState
    }
    get singleRequest() {
        return this._singleRequest
    }
    set singleRequest(value) {
        this._singleRequest = value
    }
    get multipleItems() {
        return this._multipleItems
    }
    get onlyFiles() {
        return this._onlyFiles
    }
    get processingMessage() {
        return this._multipleItems ? format(this._actionMetadata.multipleItemsProcessingMessage, this._items.length, this._location) : format(this._actionMetadata.singleItemProcessingMessage, this._location)
    }
    get successMessage() {
        if (this._hasCompletedItems()) {
            return this._multipleItems ? format(this._actionMetadata.multipleItemsSuccessMessage, this._completedItems.length, this._location) : format(this._actionMetadata.singleItemSuccessMessage, this._location)
        } else {
            return this._multipleItems ? format(this._actionMetadata.multipleItemsErrorMessage, this._items.length) : this._actionMetadata.singleItemErrorMessage
        }
    }
    get completionMessage() {
        return this.success ? this.successMessage : this.errorState.commonErrorText
    }
    get statusText() {
        return this.success && !this._hasCompletedItems() ? this._actionMetadata.canceledMessage : void 0
    }
    get commonProgress() {
        return this._commonProgress
    }
    get success() {
        return !this._errorState.failedCount
    }
}
export default FileManagerEditingControl;
