/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.file_uploader.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getInnerHeight,
    setHeight,
    setWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    Deferred
} from "../../core/utils/deferred";
import {
    hasWindow
} from "../../core/utils/window";
import Guid from "../../core/guid";
import Widget from "../widget/ui.widget";
import FileUploader from "../file_uploader";
import {
    whenSome
} from "./ui.file_manager.common";
var FILE_MANAGER_FILE_UPLOADER_CLASS = "dx-filemanager-fileuploader";
var FILE_MANAGER_FILE_UPLOADER_DROPZONE_PLACEHOLER_CLASS = "dx-filemanager-fileuploader-dropzone-placeholder";
class FileManagerFileUploader extends Widget {
    _initMarkup() {
        this._initActions();
        this.$element().addClass(FILE_MANAGER_FILE_UPLOADER_CLASS);
        this._uploaderInfos = [];
        this._createInternalFileUploader();
        this._createDropZonePlaceholder();
        this._setDropZonePlaceholderVisible(false);
        super._initMarkup()
    }
    _createInternalFileUploader() {
        var chunkSize = this._getController().chunkSize;
        var $fileUploader = $("<div>").appendTo(this.$element());
        var fileUploader = this._createComponent($fileUploader, FileUploader, {
            name: "file",
            multiple: true,
            showFileList: false,
            activeStateEnabled: false,
            focusStateEnabled: false,
            hoverStateEnabled: false,
            labelText: "",
            readyToUploadMessage: "",
            accept: "*",
            chunkSize: chunkSize,
            dropZone: this.option("dropZone"),
            onValueChanged: e => this._onFileUploaderValueChanged(e),
            onProgress: e => this._onFileUploaderProgress(e),
            onUploaded: e => this._onFileUploaderUploaded(e),
            onUploadAborted: e => this._onFileUploaderUploadAborted(e),
            onUploadError: e => this._onFileUploaderUploadError(e),
            onDropZoneEnter: () => this._setDropZonePlaceholderVisible(true),
            onDropZoneLeave: () => this._setDropZonePlaceholderVisible(false)
        });
        fileUploader.option({
            uploadChunk: (file, chunksData) => this._fileUploaderUploadChunk(fileUploader, file, chunksData),
            abortUpload: (file, chunksData) => this._fileUploaderAbortUpload(fileUploader, file, chunksData)
        });
        fileUploader._shouldRaiseDragLeaveBase = fileUploader._shouldRaiseDragLeave;
        fileUploader._shouldRaiseDragLeave = e => this._shouldRaiseDragLeave(e, fileUploader);
        var uploaderInfo = {
            fileUploader: fileUploader
        };
        this._uploaderInfos.push(uploaderInfo)
    }
    tryUpload() {
        var info = this._findAndUpdateAvailableUploaderInfo();
        if (info) {
            info.fileUploader._selectButtonClickHandler()
        }
    }
    cancelUpload(sessionId) {
        this._cancelUpload(sessionId)
    }
    cancelFileUpload(sessionId, fileIndex) {
        this._cancelUpload(sessionId, fileIndex)
    }
    _cancelUpload(sessionId, fileIndex) {
        var {
            fileUploader: fileUploader
        } = this._findUploaderInfoBySessionId(sessionId);
        fileUploader.abortUpload(fileIndex)
    }
    _fileUploaderUploadChunk(fileUploader, file, chunksInfo) {
        var {
            session: session,
            fileIndex: fileIndex
        } = this._findSessionByFile(fileUploader, file);
        var controller = session.controller;
        chunksInfo.fileIndex = fileIndex;
        return controller.uploadFileChunk(file, chunksInfo)
    }
    _fileUploaderAbortUpload(fileUploader, file, chunksInfo) {
        var {
            session: session,
            fileIndex: fileIndex
        } = this._findSessionByFile(fileUploader, file);
        var controller = session.controller;
        chunksInfo.fileIndex = fileIndex;
        return controller.abortFileUpload(file, chunksInfo)
    }
    _onFileUploaderValueChanged(_ref) {
        var {
            component: component,
            value: value
        } = _ref;
        if (0 === value.length) {
            return
        }
        var files = value.slice();
        var uploaderInfo = this._findUploaderInfo(component);
        this._uploadFiles(uploaderInfo, files);
        setTimeout(() => {
            if (!this._findAndUpdateAvailableUploaderInfo()) {
                this._createInternalFileUploader()
            }
        })
    }
    _onFileUploaderProgress(_ref2) {
        var {
            component: component,
            file: file,
            bytesLoaded: bytesLoaded,
            bytesTotal: bytesTotal
        } = _ref2;
        var {
            session: session,
            fileIndex: fileIndex
        } = this._findSessionByFile(component, file);
        var fileValue = 0 !== bytesTotal ? bytesLoaded / bytesTotal : 1;
        var commonValue = component.option("progress") / 100;
        var args = {
            sessionId: session.id,
            fileIndex: fileIndex,
            commonValue: commonValue,
            fileValue: fileValue
        };
        this._raiseUploadProgress(args)
    }
    _onFileUploaderUploaded(_ref3) {
        var {
            component: component,
            file: file
        } = _ref3;
        var deferred = this._getDeferredForFile(component, file);
        deferred.resolve()
    }
    _onFileUploaderUploadAborted(_ref4) {
        var {
            component: component,
            file: file
        } = _ref4;
        var deferred = this._getDeferredForFile(component, file);
        deferred.resolve({
            canceled: true
        })
    }
    _onFileUploaderUploadError(_ref5) {
        var {
            component: component,
            file: file,
            error: error
        } = _ref5;
        var deferred = this._getDeferredForFile(component, file);
        deferred.reject(error)
    }
    _createDropZonePlaceholder() {
        this._$dropZonePlaceholder = $("<div>").addClass(FILE_MANAGER_FILE_UPLOADER_DROPZONE_PLACEHOLER_CLASS).appendTo(this.option("dropZonePlaceholderContainer"))
    }
    _adjustDropZonePlaceholder() {
        var $dropZoneTarget = this.option("dropZone");
        if (!hasWindow() || 0 === $dropZoneTarget.length) {
            return
        }
        var placeholderBorderTopWidth = parseFloat(this._$dropZonePlaceholder.css("borderTopWidth"));
        var placeholderBorderLeftWidth = parseFloat(this._$dropZonePlaceholder.css("borderLeftWidth"));
        var $placeholderContainer = this.option("dropZonePlaceholderContainer");
        var containerBorderBottomWidth = parseFloat($placeholderContainer.css("borderBottomWidth"));
        var containerBorderLeftWidth = parseFloat($placeholderContainer.css("borderLeftWidth"));
        var containerHeight = getInnerHeight($placeholderContainer);
        var containerOffset = $placeholderContainer.offset();
        var dropZoneOffset = $dropZoneTarget.offset();
        this._$dropZonePlaceholder.css({
            top: dropZoneOffset.top - containerOffset.top - containerHeight - containerBorderBottomWidth,
            left: dropZoneOffset.left - containerOffset.left - containerBorderLeftWidth
        });
        setHeight(this._$dropZonePlaceholder, $dropZoneTarget.get(0).offsetHeight - 2 * placeholderBorderTopWidth);
        setWidth(this._$dropZonePlaceholder, $dropZoneTarget.get(0).offsetWidth - 2 * placeholderBorderLeftWidth)
    }
    _setDropZonePlaceholderVisible(visible) {
        if (visible) {
            this._adjustDropZonePlaceholder();
            this._$dropZonePlaceholder.css("display", "")
        } else {
            this._$dropZonePlaceholder.css("display", "none")
        }
    }
    _shouldRaiseDragLeave(e, uploaderInstance) {
        return uploaderInstance.isMouseOverElement(e, this.option("splitterElement")) || uploaderInstance._shouldRaiseDragLeaveBase(e, true)
    }
    _uploadFiles(uploaderInfo, files) {
        this._setDropZonePlaceholderVisible(false);
        var sessionId = (new Guid).toString();
        var controller = this._getController();
        var deferreds = files.map(() => new Deferred);
        var session = {
            id: sessionId,
            controller: controller,
            files: files,
            deferreds: deferreds
        };
        uploaderInfo.session = session;
        var sessionInfo = {
            sessionId: sessionId,
            deferreds: deferreds,
            files: files
        };
        this._raiseUploadSessionStarted(sessionInfo);
        return whenSome(deferreds).always(() => setTimeout(() => {
            uploaderInfo.fileUploader.reset();
            uploaderInfo.session = null
        }))
    }
    _getDeferredForFile(fileUploader, file) {
        var {
            session: session,
            fileIndex: fileIndex
        } = this._findSessionByFile(fileUploader, file);
        return session.deferreds[fileIndex]
    }
    _findSessionByFile(fileUploader, file) {
        var uploaderInfo = this._findUploaderInfo(fileUploader);
        var session = uploaderInfo.session;
        var fileIndex = session.files.indexOf(file);
        return {
            session: session,
            fileIndex: fileIndex
        }
    }
    _findUploaderInfoBySessionId(sessionId) {
        for (var i = 0; i < this._uploaderInfos.length; i++) {
            var uploaderInfo = this._uploaderInfos[i];
            var session = uploaderInfo.session;
            if (session && session.id === sessionId) {
                return uploaderInfo
            }
        }
        return null
    }
    _findAndUpdateAvailableUploaderInfo() {
        var _info;
        var info = null;
        for (var i = 0; i < this._uploaderInfos.length; i++) {
            var currentInfo = this._uploaderInfos[i];
            currentInfo.fileUploader.option("dropZone", "");
            if (!info && !currentInfo.session) {
                info = currentInfo
            }
        }
        null === (_info = info) || void 0 === _info ? void 0 : _info.fileUploader.option("dropZone", this.option("dropZone"));
        return info
    }
    _findUploaderInfo(fileUploader) {
        for (var i = 0; i < this._uploaderInfos.length; i++) {
            var info = this._uploaderInfos[i];
            if (info.fileUploader === fileUploader) {
                return info
            }
        }
        return null
    }
    _getController() {
        var controllerGetter = this.option("getController");
        return controllerGetter()
    }
    _raiseUploadSessionStarted(sessionInfo) {
        this._actions.onUploadSessionStarted({
            sessionInfo: sessionInfo
        })
    }
    _raiseUploadProgress(args) {
        this._actions.onUploadProgress(args)
    }
    _initActions() {
        this._actions = {
            onUploadSessionStarted: this._createActionByOption("onUploadSessionStarted"),
            onUploadProgress: this._createActionByOption("onUploadProgress")
        }
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            getController: null,
            onUploadSessionStarted: null,
            onUploadProgress: null,
            splitterElement: null
        })
    }
    _optionChanged(args) {
        var name = args.name;
        switch (name) {
            case "getController":
                this.repaint();
                break;
            case "onUploadSessionStarted":
            case "onUploadProgress":
                this._actions[name] = this._createActionByOption(name);
                break;
            case "dropZone":
                this._findAndUpdateAvailableUploaderInfo();
                this._adjustDropZonePlaceholder();
                break;
            case "dropZonePlaceholderContainer":
                this._$dropZonePlaceholder.detach();
                this._$dropZonePlaceholder.appendTo(args.value);
                break;
            case "splitterElement":
                break;
            default:
                super._optionChanged(args)
        }
    }
}
export default FileManagerFileUploader;
