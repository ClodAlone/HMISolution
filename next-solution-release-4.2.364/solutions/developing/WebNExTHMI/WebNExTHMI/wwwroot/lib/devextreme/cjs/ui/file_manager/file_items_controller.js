/**
 * DevExtreme (cjs/ui/file_manager/file_items_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _provider_base = _interopRequireDefault(require("../../file_management/provider_base"));
var _file_system_item = _interopRequireDefault(require("../../file_management/file_system_item"));
var _object_provider = _interopRequireDefault(require("../../file_management/object_provider"));
var _remote_provider = _interopRequireDefault(require("../../file_management/remote_provider"));
var _custom_provider = _interopRequireDefault(require("../../file_management/custom_provider"));
var _error = _interopRequireDefault(require("../../file_management/error"));
var _error_codes = _interopRequireDefault(require("../../file_management/error_codes"));
var _utils = require("../../file_management/utils");
var _uiFile_manager = require("./ui.file_manager.common");
var _deferred = require("../../core/utils/deferred");
var _array = require("../../core/utils/array");
var _extend = require("../../core/utils/extend");
var _common = require("../../core/utils/common");
var _type = require("../../core/utils/type");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
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
var DEFAULT_ROOT_FILE_SYSTEM_ITEM_NAME = "Files";
var FileItemsController = function() {
    function FileItemsController(options) {
        options = options || {};
        this._options = (0, _extend.extend)({}, options);
        this._isInitialized = false;
        this._dataLoading = false;
        this._dataLoadingDeferred = null;
        this._rootDirectoryInfo = this._createRootDirectoryInfo(options.rootText);
        this._currentDirectoryInfo = this._rootDirectoryInfo;
        this._defaultIconMap = this._createDefaultIconMap();
        this._setSecurityController();
        this._setProvider(options.fileProvider);
        this._initialize()
    }
    var _proto = FileItemsController.prototype;
    _proto._initialize = function() {
        var _this = this;
        var result = this._options.currentPathKeys && this._options.currentPathKeys.length ? this.setCurrentPathByKeys(this._options.currentPathKeys) : this.setCurrentPath(this._options.currentPath);
        var completeInitialization = function() {
            _this._isInitialized = true;
            _this._raiseInitialized()
        };
        if (result) {
            (0, _deferred.when)(result).always(completeInitialization)
        } else {
            completeInitialization()
        }
    };
    _proto._setSecurityController = function() {
        this._securityController = new FileSecurityController({
            allowedFileExtensions: this._options.allowedFileExtensions,
            maxFileSize: this._options.uploadMaxFileSize
        });
        this._resetState()
    };
    _proto.setAllowedFileExtensions = function(allowedFileExtensions) {
        if ((0, _type.isDefined)(allowedFileExtensions)) {
            this._options.allowedFileExtensions = allowedFileExtensions
        }
        this._setSecurityController();
        this.refresh()
    };
    _proto.setUploadOptions = function(_ref) {
        var maxFileSize = _ref.maxFileSize,
            chunkSize = _ref.chunkSize;
        if ((0, _type.isDefined)(chunkSize)) {
            this._options.uploadChunkSize = chunkSize
        }
        if ((0, _type.isDefined)(maxFileSize)) {
            this._options.uploadMaxFileSize = maxFileSize;
            this._setSecurityController();
            this.refresh()
        }
    };
    _proto._setProvider = function(fileProvider) {
        this._fileProvider = this._createFileProvider(fileProvider);
        this._resetState()
    };
    _proto.updateProvider = function(fileProvider, currentPathKeys) {
        var _this2 = this;
        if (!(0, _type.isDefined)(currentPathKeys)) {
            return this._updateProviderOnly(fileProvider)
        }
        return (0, _deferred.when)(this._getDirectoryByPathParts(this._rootDirectoryInfo, currentPathKeys, true)).then((function(newDirectory) {
            if (newDirectory !== _this2._rootDirectoryInfo) {
                _this2._resetCurrentDirectory()
            }
            _this2._setProvider(fileProvider)
        })).then((function() {
            return _this2.setCurrentPathByKeys(currentPathKeys)
        }))
    };
    _proto._updateProviderOnly = function(fileProvider) {
        this._resetCurrentDirectory();
        this._setProvider(fileProvider);
        return (0, _deferred.when)(this.refresh())
    };
    _proto._createFileProvider = function(fileProvider) {
        if (!fileProvider) {
            fileProvider = []
        }
        if (Array.isArray(fileProvider)) {
            return new _object_provider.default({
                data: fileProvider
            })
        }
        if (fileProvider instanceof _provider_base.default) {
            return fileProvider
        }
        switch (fileProvider.type) {
            case "remote":
                return new _remote_provider.default(fileProvider);
            case "custom":
                return new _custom_provider.default(fileProvider)
        }
        return new _object_provider.default(fileProvider)
    };
    _proto.setCurrentPath = function(path) {
        var pathParts = (0, _utils.getPathParts)(path);
        var rawPath = _utils.pathCombine.apply(void 0, _toConsumableArray(pathParts));
        if (this.getCurrentDirectory().fileItem.relativeName === rawPath) {
            return (new _deferred.Deferred).resolve().promise()
        }
        return this._setCurrentDirectoryByPathParts(pathParts)
    };
    _proto.setCurrentPathByKeys = function(pathKeys) {
        if ((0, _common.equalByValue)(this.getCurrentDirectory().fileItem.pathKeys, pathKeys, 0, true)) {
            return (new _deferred.Deferred).resolve().promise()
        }
        return this._setCurrentDirectoryByPathParts(pathKeys, true)
    };
    _proto.getCurrentPath = function() {
        var currentPath = "";
        var directory = this.getCurrentDirectory();
        while (directory && !directory.fileItem.isRoot()) {
            var escapedName = (0, _utils.getEscapedFileName)(directory.fileItem.name);
            currentPath = (0, _utils.pathCombine)(escapedName, currentPath);
            directory = directory.parentDirectory
        }
        return currentPath
    };
    _proto.getCurrentPathKeys = function() {
        return this.getCurrentDirectory().fileItem.pathKeys
    };
    _proto.getCurrentDirectory = function() {
        return this._currentDirectoryInfo
    };
    _proto.setCurrentDirectory = function(directoryInfo, checkActuality) {
        if (!directoryInfo) {
            return
        }
        if (checkActuality) {
            directoryInfo = this._getActualDirectoryInfo(directoryInfo)
        }
        if (this._currentDirectoryInfo && this._currentDirectoryInfo === directoryInfo) {
            this._raisePathPotentiallyChanged();
            return
        }
        var requireRaiseSelectedDirectory = this._currentDirectoryInfo.fileItem.key !== directoryInfo.fileItem.key;
        this._currentDirectoryInfo = directoryInfo;
        if (requireRaiseSelectedDirectory && this._isInitialized) {
            if (!this._dataLoading) {
                this._raiseDataLoading("navigation")
            }
            this._raiseSelectedDirectoryChanged(directoryInfo)
        }
    };
    _proto._resetCurrentDirectory = function() {
        this._currentDirectoryInfo = this._rootDirectoryInfo
    };
    _proto.getCurrentItems = function(onlyFiles) {
        var _this3 = this;
        return this._dataLoadingDeferred ? this._dataLoadingDeferred.then((function() {
            return _this3._getCurrentItemsInternal(onlyFiles)
        })) : this._getCurrentItemsInternal(onlyFiles)
    };
    _proto._getCurrentItemsInternal = function(onlyFiles) {
        var _this4 = this;
        var currentDirectory = this.getCurrentDirectory();
        var getItemsPromise = this.getDirectoryContents(currentDirectory);
        return getItemsPromise.then((function(items) {
            var separatedItems = _this4._separateItemsByType(items);
            currentDirectory.fileItem.hasSubDirectories = !!separatedItems.folders.length;
            return onlyFiles ? separatedItems.files : items
        }))
    };
    _proto.getDirectories = function(parentDirectoryInfo, skipNavigationOnError) {
        return this.getDirectoryContents(parentDirectoryInfo, skipNavigationOnError).then((function(itemInfos) {
            return itemInfos.filter((function(info) {
                return info.fileItem.isDirectory
            }))
        }))
    };
    _proto._separateItemsByType = function(itemInfos) {
        var folders = [];
        var files = [];
        itemInfos.forEach((function(info) {
            return info.fileItem.isDirectory ? folders.push(info) : files.push(info)
        }));
        return {
            folders: folders,
            files: files
        }
    };
    _proto.getDirectoryContents = function(parentDirectoryInfo, skipNavigationOnError) {
        var _this5 = this;
        if (!parentDirectoryInfo) {
            return (new _deferred.Deferred).resolve([this._rootDirectoryInfo]).promise()
        }
        if (parentDirectoryInfo.itemsLoaded) {
            return (new _deferred.Deferred).resolve(parentDirectoryInfo.items).promise()
        }
        var dirKey = parentDirectoryInfo.getInternalKey();
        var loadItemsDeferred = this._loadedItems[dirKey];
        if (loadItemsDeferred) {
            return loadItemsDeferred
        }
        loadItemsDeferred = this._getFileItems(parentDirectoryInfo, skipNavigationOnError).then((function(fileItems) {
            fileItems = fileItems || [];
            parentDirectoryInfo.items = fileItems.map((function(fileItem) {
                return fileItem.isDirectory && _this5._createDirectoryInfo(fileItem, parentDirectoryInfo) || _this5._createFileInfo(fileItem, parentDirectoryInfo)
            }));
            parentDirectoryInfo.itemsLoaded = true;
            return parentDirectoryInfo.items
        }));
        this._loadedItems[dirKey] = loadItemsDeferred;
        loadItemsDeferred.always((function() {
            delete _this5._loadedItems[dirKey]
        }));
        return loadItemsDeferred
    };
    _proto._getFileItems = function(parentDirectoryInfo, skipNavigationOnError) {
        var _this6 = this;
        var loadItemsDeferred = null;
        try {
            loadItemsDeferred = this._fileProvider.getItems(parentDirectoryInfo.fileItem)
        } catch (error) {
            return this._handleItemLoadError(parentDirectoryInfo, error, skipNavigationOnError)
        }
        return (0, _deferred.when)(loadItemsDeferred).then((function(fileItems) {
            return _this6._securityController.getAllowedItems(fileItems)
        }), (function(errorInfo) {
            return _this6._handleItemLoadError(parentDirectoryInfo, errorInfo, skipNavigationOnError)
        }))
    };
    _proto.createDirectory = function(parentDirectoryInfo, name) {
        var _this7 = this;
        var parentDirItem = parentDirectoryInfo.fileItem;
        var tempDirInfo = this._createDirInfoByName(name, parentDirectoryInfo);
        var actionInfo = this._createEditActionInfo("create", tempDirInfo, parentDirectoryInfo);
        return this._processEditAction(actionInfo, (function(args) {
            args.parentDirectory = parentDirItem;
            args.name = name;
            _this7._editingEvents.onDirectoryCreating(args)
        }), (function() {
            return _this7._fileProvider.createDirectory(parentDirItem, name).done((function(info) {
                if (!parentDirItem.isRoot()) {
                    parentDirItem.hasSubDirectories = true
                }
                return info
            }))
        }), (function() {
            var args = {
                parentDirectory: parentDirItem,
                name: name
            };
            _this7._editingEvents.onDirectoryCreated(args)
        }), (function() {
            return _this7._resetDirectoryState(parentDirectoryInfo, true)
        }))
    };
    _proto.renameItem = function(fileItemInfo, name) {
        var _this8 = this;
        var sourceItem = fileItemInfo.fileItem.createClone();
        var actionInfo = this._createEditActionInfo("rename", fileItemInfo, fileItemInfo.parentDirectory, {
            itemNewName: name
        });
        return this._processEditAction(actionInfo, (function(args, itemInfo) {
            if (!itemInfo.fileItem.isDirectory) {
                _this8._securityController.validateExtension(name)
            }
            args.item = sourceItem;
            args.newName = name;
            _this8._editingEvents.onItemRenaming(args)
        }), (function(item) {
            return _this8._fileProvider.renameItem(item, name)
        }), (function() {
            var args = {
                sourceItem: sourceItem,
                itemName: name
            };
            _this8._editingEvents.onItemRenamed(args)
        }), (function() {
            var parentDirectory = _this8._getActualDirectoryInfo(fileItemInfo.parentDirectory);
            _this8._resetDirectoryState(parentDirectory);
            _this8.setCurrentDirectory(parentDirectory)
        }))
    };
    _proto.moveItems = function(itemInfos, destinationDirectory) {
        var _this9 = this;
        var actionInfo = this._createEditActionInfo("move", itemInfos, destinationDirectory);
        return this._processEditAction(actionInfo, (function(args, itemInfo) {
            args.item = itemInfo.fileItem;
            args.destinationDirectory = destinationDirectory.fileItem;
            _this9._editingEvents.onItemMoving(args)
        }), (function(item) {
            return _this9._fileProvider.moveItems([item], destinationDirectory.fileItem)
        }), (function(itemInfo) {
            var args = {
                sourceItem: itemInfo.fileItem,
                parentDirectory: destinationDirectory.fileItem,
                itemName: itemInfo.fileItem.name,
                itemPath: (0, _utils.pathCombine)(destinationDirectory.fileItem.path, itemInfo.fileItem.name)
            };
            _this9._editingEvents.onItemMoved(args)
        }), (function() {
            destinationDirectory = _this9._getActualDirectoryInfo(destinationDirectory);
            itemInfos.forEach((function(itemInfo) {
                return _this9._resetDirectoryState(itemInfo.parentDirectory, true)
            }));
            _this9._resetDirectoryState(destinationDirectory);
            _this9.setCurrentDirectory(destinationDirectory);
            destinationDirectory.expanded = true
        }))
    };
    _proto.copyItems = function(itemInfos, destinationDirectory) {
        var _this10 = this;
        var actionInfo = this._createEditActionInfo("copy", itemInfos, destinationDirectory);
        return this._processEditAction(actionInfo, (function(args, itemInfo) {
            args.item = itemInfo.fileItem;
            args.destinationDirectory = destinationDirectory.fileItem;
            _this10._editingEvents.onItemCopying(args)
        }), (function(item) {
            return _this10._fileProvider.copyItems([item], destinationDirectory.fileItem)
        }), (function(itemInfo) {
            var args = {
                sourceItem: itemInfo.fileItem,
                parentDirectory: destinationDirectory.fileItem,
                itemName: itemInfo.fileItem.name,
                itemPath: (0, _utils.pathCombine)(destinationDirectory.fileItem.path, itemInfo.fileItem.name)
            };
            _this10._editingEvents.onItemCopied(args)
        }), (function() {
            destinationDirectory = _this10._getActualDirectoryInfo(destinationDirectory);
            _this10._resetDirectoryState(destinationDirectory);
            _this10.setCurrentDirectory(destinationDirectory);
            destinationDirectory.expanded = true
        }))
    };
    _proto.deleteItems = function(itemInfos) {
        var _this11 = this;
        var directory = itemInfos.length > 0 ? itemInfos[0].parentDirectory : null;
        var actionInfo = this._createEditActionInfo("delete", itemInfos, directory);
        return this._processEditAction(actionInfo, (function(args, itemInfo) {
            args.item = itemInfo.fileItem;
            _this11._editingEvents.onItemDeleting(args)
        }), (function(item) {
            return _this11._fileProvider.deleteItems([item])
        }), (function(itemInfo) {
            return _this11._editingEvents.onItemDeleted({
                item: itemInfo.fileItem
            })
        }), (function() {
            itemInfos.forEach((function(itemInfo) {
                var parentDir = _this11._getActualDirectoryInfo(itemInfo.parentDirectory);
                _this11._resetDirectoryState(parentDir);
                _this11.setCurrentDirectory(parentDir)
            }))
        }))
    };
    _proto.processUploadSession = function(sessionInfo, uploadDirectoryInfo) {
        var _this12 = this;
        var itemInfos = this._getItemInfosForUploaderFiles(sessionInfo.files, uploadDirectoryInfo);
        var actionInfo = this._createEditActionInfo("upload", itemInfos, uploadDirectoryInfo, {
            sessionInfo: sessionInfo
        });
        return this._processEditAction(actionInfo, (function() {}), (function(_, index) {
            return sessionInfo.deferreds[index]
        }), (function() {}), (function() {
            return _this12._resetDirectoryState(uploadDirectoryInfo, true)
        }))
    };
    _proto.uploadFileChunk = function(fileData, chunksInfo, destinationDirectory) {
        var _this13 = this;
        var startDeferred = null;
        if (0 === chunksInfo.chunkIndex) {
            this._securityController.validateMaxFileSize(fileData.size);
            this._securityController.validateExtension(fileData.name);
            startDeferred = this._processBeforeItemEditAction((function(args) {
                args.fileData = fileData;
                args.destinationDirectory = destinationDirectory;
                _this13._editingEvents.onFileUploading(args)
            }))
        } else {
            startDeferred = (new _deferred.Deferred).resolve().promise()
        }
        var result = startDeferred.then((function() {
            return _this13._fileProvider.uploadFileChunk(fileData, chunksInfo, destinationDirectory)
        }));
        if (chunksInfo.chunkIndex === chunksInfo.chunkCount - 1) {
            result = result.done((function() {
                var args = {
                    fileData: fileData,
                    parentDirectory: destinationDirectory
                };
                _this13._editingEvents.onFileUploaded(args)
            }))
        }
        return result
    };
    _proto.abortFileUpload = function(fileData, chunksInfo, destinationDirectory) {
        return (0, _deferred.when)(this._fileProvider.abortFileUpload(fileData, chunksInfo, destinationDirectory))
    };
    _proto.getFileUploadChunkSize = function() {
        var chunkSize = this._options.uploadChunkSize;
        if (chunkSize && chunkSize > 0) {
            return chunkSize
        }
        return this._fileProvider.getFileUploadChunkSize()
    };
    _proto.downloadItems = function(itemInfos) {
        var _this14 = this;
        var canceled = false;
        var deferreds = itemInfos.map((function(itemInfo) {
            return _this14._processBeforeItemEditAction((function(args) {
                args.item = itemInfo.fileItem;
                _this14._editingEvents.onItemDownloading(args)
            }), itemInfo)
        }));
        (0, _uiFile_manager.whenSome)(deferreds, null, (function() {
            canceled = true
        })).then((function() {
            if (!canceled) {
                var items = itemInfos.map((function(i) {
                    return i.fileItem
                }));
                _this14._fileProvider.downloadItems(items)
            }
        }))
    };
    _proto.getItemContent = function(itemInfos) {
        var items = itemInfos.map((function(i) {
            return i.fileItem
        }));
        return (0, _deferred.when)(this._fileProvider.getItemsContent(items))
    };
    _proto._handleItemLoadError = function(parentDirectoryInfo, errorInfo, skipNavigationOnError) {
        parentDirectoryInfo = this._getActualDirectoryInfo(parentDirectoryInfo);
        var actionInfo = this._createEditActionInfo("getItems", parentDirectoryInfo, parentDirectoryInfo);
        this._raiseEditActionStarting(actionInfo);
        this._raiseEditActionResultAcquired(actionInfo);
        this._raiseEditActionError(actionInfo, {
            errorCode: errorInfo.errorCode,
            errorText: errorInfo.errorText,
            fileItem: parentDirectoryInfo.fileItem,
            index: 0
        });
        this._resetDirectoryState(parentDirectoryInfo);
        parentDirectoryInfo.expanded = false;
        if (!skipNavigationOnError) {
            this.setCurrentDirectory(parentDirectoryInfo.parentDirectory)
        }
        return (new _deferred.Deferred).reject().promise()
    };
    _proto._processEditAction = function(actionInfo, beforeAction, action, afterAction, completeAction) {
        var _this15 = this;
        this._raiseEditActionStarting(actionInfo);
        var actionResult = actionInfo.itemInfos.map((function(itemInfo, itemIndex) {
            return _this15._processBeforeItemEditAction(beforeAction, itemInfo).then((function() {
                var itemActionResult = action(itemInfo.fileItem, itemIndex);
                if (Array.isArray(itemActionResult)) {
                    itemActionResult = itemActionResult[0]
                }
                return itemActionResult.done((function() {
                    return afterAction(itemInfo)
                }))
            }))
        }));
        actionInfo.singleRequest = 1 === actionResult.length;
        this._raiseEditActionResultAcquired(actionInfo);
        return (0, _uiFile_manager.whenSome)(actionResult, (function(info) {
            return _this15._raiseCompleteEditActionItem(actionInfo, info)
        }), (function(errorInfo) {
            return _this15._raiseEditActionItemError(actionInfo, errorInfo)
        })).then((function() {
            completeAction();
            _this15._raiseCompleteEditAction(actionInfo)
        }))
    };
    _proto._createEditActionInfo = function(name, targetItemInfos, directory, customData) {
        targetItemInfos = Array.isArray(targetItemInfos) ? targetItemInfos : [targetItemInfos];
        customData = customData || {};
        var items = targetItemInfos.map((function(itemInfo) {
            return itemInfo.fileItem
        }));
        return {
            name: name,
            itemInfos: targetItemInfos,
            items: items,
            directory: directory,
            customData: customData,
            singleRequest: true
        }
    };
    _proto._processBeforeItemEditAction = function(action, itemInfo) {
        var deferred = new _deferred.Deferred;
        var args = this._createBeforeActionArgs();
        try {
            action(args, itemInfo)
        } catch (errorInfo) {
            return deferred.reject(errorInfo).promise()
        }
        if (!args.cancel) {
            deferred.resolve()
        } else if (true === args.cancel) {
            return deferred.reject({
                errorText: args.errorText,
                errorCode: args.errorCode
            })
        } else if ((0, _type.isPromise)(args.cancel)) {
            (0, _deferred.when)(args.cancel).then((function(res) {
                if (true === res) {
                    deferred.reject()
                } else if ((0, _type.isObject)(res) && true === res.cancel) {
                    deferred.reject({
                        errorText: res.errorText,
                        errorCode: res.errorCode
                    })
                }
                deferred.resolve()
            }), deferred.resolve)
        }
        return deferred.promise()
    };
    _proto._createBeforeActionArgs = function() {
        return {
            errorCode: void 0,
            errorText: "",
            cancel: false
        }
    };
    _proto._getItemInfosForUploaderFiles = function(files, parentDirectoryInfo) {
        var pathInfo = this._getPathInfo(parentDirectoryInfo);
        var result = [];
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var item = new _file_system_item.default(pathInfo, file.name, false);
            var itemInfo = this._createFileInfo(item, parentDirectoryInfo);
            result.push(itemInfo)
        }
        return result
    };
    _proto.refresh = function() {
        var _this16 = this;
        if (this._lockRefresh) {
            return this._refreshDeferred
        }
        this._lockRefresh = true;
        return this._executeDataLoad((function() {
            return _this16._refreshDeferred = _this16._refreshInternal()
        }), "refresh")
    };
    _proto._refreshInternal = function() {
        var _this17 = this;
        var cachedRootInfo = {
            items: this._rootDirectoryInfo.items
        };
        var selectedKeyParts = this._getDirectoryPathKeyParts(this.getCurrentDirectory());
        this._resetDirectoryState(this._rootDirectoryInfo);
        return this._loadItemsRecursive(this._rootDirectoryInfo, cachedRootInfo).then((function() {
            var dirInfo = _this17._findDirectoryByPathKeyParts(selectedKeyParts);
            _this17.setCurrentDirectory(dirInfo);
            delete _this17._lockRefresh
        }))
    };
    _proto._loadItemsRecursive = function(directoryInfo, cachedDirectoryInfo) {
        var _this18 = this;
        return this.getDirectories(directoryInfo).then((function(dirInfos) {
            var itemDeferreds = [];
            var _loop = function(i) {
                var cachedItem = (0, _array.find)(cachedDirectoryInfo.items, (function(cache) {
                    return dirInfos[i].fileItem.key === cache.fileItem.key
                }));
                if (!cachedItem) {
                    return "continue"
                }
                dirInfos[i].expanded = cachedItem.expanded;
                if (dirInfos[i].expanded) {
                    itemDeferreds.push(_this18._loadItemsRecursive(dirInfos[i], cachedItem))
                }
            };
            for (var i = 0; i < dirInfos.length; i++) {
                var _ret = _loop(i);
                if ("continue" === _ret) {
                    continue
                }
            }
            return (0, _uiFile_manager.whenSome)(itemDeferreds)
        }), (function() {
            return null
        }))
    };
    _proto._setCurrentDirectoryByPathParts = function(pathParts, useKeys) {
        var _this19 = this;
        return this._executeDataLoad((function() {
            return _this19._setCurrentDirectoryByPathPartsInternal(pathParts, useKeys)
        }), "navigation")
    };
    _proto._setCurrentDirectoryByPathPartsInternal = function(pathParts, useKeys) {
        var _this20 = this;
        return this._getDirectoryByPathParts(this._rootDirectoryInfo, pathParts, useKeys).then((function(directoryInfo) {
            for (var info = directoryInfo.parentDirectory; info; info = info.parentDirectory) {
                info.expanded = true
            }
            _this20.setCurrentDirectory(directoryInfo)
        }), (function() {
            _this20._raisePathPotentiallyChanged()
        }))
    };
    _proto._executeDataLoad = function(action, operation) {
        var _this21 = this;
        if (this._dataLoadingDeferred) {
            return this._dataLoadingDeferred.then((function() {
                return _this21._executeDataLoad(action, operation)
            }))
        }
        this._dataLoading = true;
        this._dataLoadingDeferred = new _deferred.Deferred;
        if (this._isInitialized) {
            this._raiseDataLoading(operation)
        }
        return action().always((function() {
            var tempDeferred = _this21._dataLoadingDeferred;
            _this21._dataLoadingDeferred = null;
            _this21._dataLoading = false;
            tempDeferred.resolve()
        }))
    };
    _proto._getDirectoryByPathParts = function(parentDirectoryInfo, pathParts, useKeys) {
        var _this22 = this;
        if (pathParts.length < 1) {
            return (new _deferred.Deferred).resolve(parentDirectoryInfo).promise()
        }
        var fieldName = useKeys ? "key" : "name";
        return this.getDirectories(parentDirectoryInfo).then((function(dirInfos) {
            var subDirInfo = (0, _array.find)(dirInfos, (function(d) {
                return d.fileItem[fieldName] === pathParts[0]
            }));
            if (!subDirInfo) {
                return (new _deferred.Deferred).reject().promise()
            }
            var restPathParts = _toConsumableArray(pathParts).splice(1);
            return _this22._getDirectoryByPathParts(subDirInfo, restPathParts, useKeys)
        }))
    };
    _proto._getDirectoryPathKeyParts = function(directoryInfo) {
        var pathParts = [];
        while (directoryInfo && directoryInfo.parentDirectory) {
            pathParts.unshift(directoryInfo.fileItem.key);
            directoryInfo = directoryInfo.parentDirectory
        }
        return pathParts
    };
    _proto._findDirectoryByPathKeyParts = function(keyParts) {
        var selectedDirInfo = this._rootDirectoryInfo;
        if (0 === keyParts.length) {
            return selectedDirInfo
        }
        var i = 0;
        var newSelectedDir = selectedDirInfo;
        while (newSelectedDir && i < keyParts.length) {
            newSelectedDir = (0, _array.find)(selectedDirInfo.items, (function(info) {
                return info.fileItem.key === keyParts[i]
            }));
            if (newSelectedDir) {
                selectedDirInfo = newSelectedDir
            }
            i++
        }
        return selectedDirInfo
    };
    _proto._getActualDirectoryInfo = function(directoryInfo) {
        var keys = this._getDirectoryPathKeyParts(directoryInfo);
        return this._findDirectoryByPathKeyParts(keys)
    };
    _proto._createDirInfoByName = function(name, parentDirectoryInfo) {
        var dirPathInfo = this._getPathInfo(parentDirectoryInfo);
        var fileItem = new _file_system_item.default(dirPathInfo, name, true);
        return this._createDirectoryInfo(fileItem, parentDirectoryInfo)
    };
    _proto._createDirectoryInfo = function(fileItem, parentDirectoryInfo) {
        return (0, _extend.extend)(this._createFileInfo(fileItem, parentDirectoryInfo), {
            icon: "folder",
            expanded: fileItem.isRoot(),
            items: []
        })
    };
    _proto._createFileInfo = function(fileItem, parentDirectoryInfo) {
        return {
            fileItem: fileItem,
            parentDirectory: parentDirectoryInfo,
            icon: this._getFileItemDefaultIcon(fileItem),
            getInternalKey: function() {
                return "FIK_".concat(this.fileItem.key)
            },
            getDisplayName: function() {
                return this.displayName || this.fileItem.name
            }
        }
    };
    _proto._resetDirectoryState = function(directoryInfo, isActualDirectoryRequired) {
        if (isActualDirectoryRequired) {
            directoryInfo = this._getActualDirectoryInfo(directoryInfo)
        }
        directoryInfo.itemsLoaded = false;
        directoryInfo.items = []
    };
    _proto._getFileItemDefaultIcon = function(fileItem) {
        if (fileItem.isDirectory) {
            return "folder"
        }
        var extension = fileItem.getFileExtension();
        var icon = this._defaultIconMap[extension];
        return icon || "doc"
    };
    _proto._createDefaultIconMap = function() {
        var result = {
            ".txt": "txtfile",
            ".rtf": "rtffile",
            ".doc": "docfile",
            ".docx": "docxfile",
            ".xls": "xlsfile",
            ".xlsx": "xlsxfile",
            ".ppt": "pptfile",
            ".pptx": "pptxfile",
            ".pdf": "pdffile"
        };
        [".png", ".gif", ".jpg", ".jpeg", ".ico", ".bmp"].forEach((function(extension) {
            result[extension] = "image"
        }));
        return result
    };
    _proto._createRootDirectoryInfo = function(text) {
        var rootDirectory = new _file_system_item.default(null, "", true);
        var result = this._createDirectoryInfo(rootDirectory, null);
        result.displayName = text || DEFAULT_ROOT_FILE_SYSTEM_ITEM_NAME;
        return result
    };
    _proto.setRootText = function(rootText) {
        this._rootDirectoryInfo.displayName = rootText || DEFAULT_ROOT_FILE_SYSTEM_ITEM_NAME
    };
    _proto._raiseInitialized = function() {
        this._tryCallAction("onInitialized", {
            controller: this
        })
    };
    _proto._raiseDataLoading = function(operation) {
        this._tryCallAction("onDataLoading", {
            operation: operation
        })
    };
    _proto._raiseSelectedDirectoryChanged = function(directoryInfo) {
        this._tryCallAction("onSelectedDirectoryChanged", {
            selectedDirectoryInfo: directoryInfo
        })
    };
    _proto._raiseEditActionStarting = function(actionInfo) {
        this._tryCallAction("onEditActionStarting", actionInfo)
    };
    _proto._raiseEditActionResultAcquired = function(actionInfo) {
        this._tryCallAction("onEditActionResultAcquired", actionInfo)
    };
    _proto._raiseEditActionError = function(actionInfo, errorInfo) {
        this._tryCallAction("onEditActionError", actionInfo, errorInfo)
    };
    _proto._raiseEditActionItemError = function(actionInfo, errorInfo) {
        this._tryCallAction("onEditActionItemError", actionInfo, errorInfo)
    };
    _proto._raiseCompleteEditActionItem = function(actionInfo, info) {
        this._tryCallAction("onCompleteEditActionItem", actionInfo, info)
    };
    _proto._raiseCompleteEditAction = function(actionInfo) {
        this._tryCallAction("onCompleteEditAction", actionInfo)
    };
    _proto._raisePathPotentiallyChanged = function() {
        this._tryCallAction("onPathPotentiallyChanged")
    };
    _proto._tryCallAction = function(actionName) {
        var args = Array.prototype.slice.call(arguments, 1);
        if (this._options[actionName]) {
            var _this$_options;
            (_this$_options = this._options)[actionName].apply(_this$_options, _toConsumableArray(args))
        }
    };
    _proto._resetState = function() {
        this._selectedDirectory = null;
        this._rootDirectoryInfo.items = [];
        this._rootDirectoryInfo.itemsLoaded = false;
        this._loadedItems = {}
    };
    _proto._getPathInfo = function(directoryInfo) {
        var pathInfo = [];
        for (var dirInfo = directoryInfo; dirInfo && !dirInfo.fileItem.isRoot(); dirInfo = dirInfo.parentDirectory) {
            pathInfo.unshift({
                key: dirInfo.fileItem.key,
                name: dirInfo.fileItem.name
            })
        }
        return pathInfo
    };
    _proto.on = function(eventName, eventHandler) {
        var finalEventName = "on".concat(eventName);
        this._options[finalEventName] = eventHandler
    };
    _createClass(FileItemsController, [{
        key: "_editingEvents",
        get: function() {
            return this._options.editingEvents
        }
    }]);
    return FileItemsController
}();
exports.default = FileItemsController;
var FileSecurityController = function() {
    function FileSecurityController(options) {
        var _this23 = this;
        this._options = (0, _extend.extend)({
            allowedFileExtensions: [],
            maxFileSize: 0
        }, options);
        this._extensionsMap = {};
        this._allowedFileExtensions.forEach((function(extension) {
            _this23._extensionsMap[extension.toUpperCase()] = true
        }))
    }
    var _proto2 = FileSecurityController.prototype;
    _proto2.getAllowedItems = function(items) {
        var _this24 = this;
        if (0 === this._allowedFileExtensions.length) {
            return items
        }
        return items.filter((function(item) {
            return item.isDirectory || _this24._isValidExtension(item.name)
        }))
    };
    _proto2.validateExtension = function(name) {
        if (!this._isValidExtension(name)) {
            throw new _error.default(_error_codes.default.WrongFileExtension, null)
        }
    };
    _proto2.validateMaxFileSize = function(size) {
        if (this._maxFileSize && size > this._maxFileSize) {
            throw new _error.default(_error_codes.default.MaxFileSizeExceeded, null)
        }
    };
    _proto2._isValidExtension = function(name) {
        if (0 === this._allowedFileExtensions.length) {
            return true
        }
        var extension = (0, _utils.getFileExtension)(name).toUpperCase();
        return this._extensionsMap[extension]
    };
    _createClass(FileSecurityController, [{
        key: "_allowedFileExtensions",
        get: function() {
            return this._options.allowedFileExtensions
        }
    }, {
        key: "_maxFileSize",
        get: function() {
            return this._options.maxFileSize
        }
    }]);
    return FileSecurityController
}();
module.exports = exports.default;
module.exports.default = exports.default;
