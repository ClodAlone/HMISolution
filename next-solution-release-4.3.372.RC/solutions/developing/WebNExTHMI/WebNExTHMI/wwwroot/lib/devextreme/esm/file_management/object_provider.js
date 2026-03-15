/**
 * DevExtreme (esm/file_management/object_provider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    find
} from "../core/utils/array";
import {
    ensureDefined
} from "../core/utils/common";
import {
    compileGetter,
    compileSetter
} from "../core/utils/data";
import Guid from "../core/guid";
import {
    isFunction
} from "../core/utils/type";
import {
    errors
} from "../data/errors";
import {
    Deferred
} from "../core/utils/deferred";
import {
    getWindow
} from "../core/utils/window";
import {
    fileSaver
} from "../exporter/file_saver";
import Errors from "../ui/widget/ui.errors";
import JSZip from "jszip";
import FileSystemProviderBase from "./provider_base";
import FileSystemError from "./error";
import ErrorCode from "./error_codes";
import {
    pathCombine
} from "./utils";
var window = getWindow();
class ObjectFileSystemProvider extends FileSystemProviderBase {
    constructor(options) {
        options = ensureDefined(options, {});
        super(options);
        var initialArray = options.data;
        if (initialArray && !Array.isArray(initialArray)) {
            throw errors.Error("E4006")
        }
        var itemsExpr = options.itemsExpr || "items";
        this._subFileItemsGetter = compileGetter(itemsExpr);
        this._subFileItemsSetter = this._getSetter(itemsExpr);
        var contentExpr = options.contentExpr || "content";
        this._contentGetter = compileGetter(contentExpr);
        this._contentSetter = this._getSetter(contentExpr);
        var nameExpr = this._getNameExpr(options);
        this._nameSetter = this._getSetter(nameExpr);
        var isDirExpr = this._getIsDirExpr(options);
        this._getIsDirSetter = this._getSetter(isDirExpr);
        var keyExpr = this._getKeyExpr(options);
        this._keySetter = this._getSetter(keyExpr);
        var sizeExpr = this._getSizeExpr(options);
        this._sizeSetter = this._getSetter(sizeExpr);
        var dateModifiedExpr = this._getDateModifiedExpr(options);
        this._dateModifiedSetter = this._getSetter(dateModifiedExpr);
        this._data = initialArray || []
    }
    getItems(parentDir) {
        return this._executeActionAsDeferred(() => this._getItems(parentDir), true)
    }
    renameItem(item, name) {
        return this._executeActionAsDeferred(() => this._renameItemCore(item, name))
    }
    _renameItemCore(item, name) {
        if (!item) {
            return
        }
        var dataItem = this._findDataObject(item);
        this._nameSetter(dataItem, name);
        item.name = name;
        item.key = this._ensureDataObjectKey(dataItem)
    }
    createDirectory(parentDir, name) {
        return this._executeActionAsDeferred(() => {
            this._validateDirectoryExists(parentDir);
            this._createDataObject(parentDir, name, true)
        })
    }
    deleteItems(items) {
        return items.map(item => this._executeActionAsDeferred(() => this._deleteItem(item)))
    }
    moveItems(items, destinationDir) {
        var destinationDataItem = this._findDataObject(destinationDir);
        var array = this._getDirectoryDataItems(destinationDataItem);
        var deferreds = items.map(item => this._executeActionAsDeferred(() => {
            this._checkAbilityToMoveOrCopyItem(item, destinationDir);
            var dataItem = this._findDataObject(item);
            this._deleteItem(item);
            array.push(dataItem)
        }));
        return deferreds
    }
    copyItems(items, destinationDir) {
        var destinationDataItem = this._findDataObject(destinationDir);
        var array = this._getDirectoryDataItems(destinationDataItem);
        var deferreds = items.map(item => this._executeActionAsDeferred(() => {
            this._checkAbilityToMoveOrCopyItem(item, destinationDir);
            var dataItem = this._findDataObject(item);
            var copiedItem = this._createCopy(dataItem);
            array.push(copiedItem)
        }));
        return deferreds
    }
    uploadFileChunk(fileData, chunksInfo, destinationDirectory) {
        if (chunksInfo.chunkIndex > 0) {
            return chunksInfo.customData.deferred
        }
        this._validateDirectoryExists(destinationDirectory);
        var deferred = chunksInfo.customData.deferred = new Deferred;
        var reader = this._createFileReader();
        reader.readAsDataURL(fileData);
        reader.onload = () => {
            var content = reader.result.split(",")[1];
            var dataObj = this._createDataObject(destinationDirectory, fileData.name, false);
            this._sizeSetter(dataObj, fileData.size);
            this._dateModifiedSetter(dataObj, fileData.lastModifiedDate);
            this._contentSetter(dataObj, content);
            deferred.resolve()
        };
        reader.onerror = error => deferred.reject(error);
        return deferred
    }
    downloadItems(items) {
        if (1 === items.length) {
            this._downloadSingleFile(items[0])
        } else {
            this._downloadMultipleFiles(items)
        }
    }
    _downloadSingleFile(file) {
        var content = this._getFileContent(file);
        var byteString = window.atob(content);
        var arrayBuffer = new ArrayBuffer(byteString.length);
        var array = new Uint8Array(arrayBuffer);
        for (var i = 0; i < byteString.length; i++) {
            array[i] = byteString.charCodeAt(i)
        }
        var blob = new window.Blob([arrayBuffer], {
            type: "application/octet-stream"
        });
        fileSaver.saveAs(file.name, null, blob)
    }
    _downloadMultipleFiles(files) {
        var jsZip = getJSZip();
        var zip = new jsZip;
        files.forEach(file => zip.file(file.name, this._getFileContent(file), {
            base64: true
        }));
        var options = {
            type: "blob",
            compression: "DEFLATE",
            mimeType: "application/zip"
        };
        var deferred = new Deferred;
        if (zip.generateAsync) {
            zip.generateAsync(options).then(deferred.resolve)
        } else {
            deferred.resolve(zip.generate(options))
        }
        deferred.done(blob => fileSaver.saveAs("files.zip", null, blob))
    }
    _getFileContent(file) {
        var dataItem = this._findDataObject(file);
        return this._contentGetter(dataItem) || ""
    }
    _validateDirectoryExists(directoryInfo) {
        if (!this._isFileItemExists(directoryInfo) || this._isDirGetter(directoryInfo.fileItem)) {
            throw new FileSystemError(ErrorCode.DirectoryNotFound, directoryInfo)
        }
    }
    _checkAbilityToMoveOrCopyItem(item, destinationDir) {
        var dataItem = this._findDataObject(item);
        var itemKey = this._getKeyFromDataObject(dataItem, item.parentPath);
        var pathInfo = destinationDir.getFullPathInfo();
        var currentPath = "";
        pathInfo.forEach(info => {
            currentPath = pathCombine(currentPath, info.name);
            var pathKey = this._getDataObjectKey(info.key, currentPath);
            if (pathKey === itemKey) {
                throw new FileSystemError(ErrorCode.Other, item)
            }
        })
    }
    _createDataObject(parentDir, name, isDirectory) {
        var dataObj = {};
        this._nameSetter(dataObj, name);
        this._getIsDirSetter(dataObj, isDirectory);
        this._keySetter(dataObj, String(new Guid));
        var parentDataItem = this._findDataObject(parentDir);
        var array = this._getDirectoryDataItems(parentDataItem);
        array.push(dataObj);
        return dataObj
    }
    _createCopy(dataObj) {
        var copyObj = {};
        this._nameSetter(copyObj, this._nameGetter(dataObj));
        this._getIsDirSetter(copyObj, this._isDirGetter(dataObj));
        var items = this._subFileItemsGetter(dataObj);
        if (Array.isArray(items)) {
            var itemsCopy = [];
            items.forEach(childItem => {
                var childCopy = this._createCopy(childItem);
                itemsCopy.push(childCopy)
            });
            this._subFileItemsSetter(copyObj, itemsCopy)
        }
        return copyObj
    }
    _deleteItem(fileItem) {
        var dataItem = this._findDataObject(fileItem);
        var parentDirDataObj = this._findFileItemObj(fileItem.pathInfo);
        var array = this._getDirectoryDataItems(parentDirDataObj);
        var index = array.indexOf(dataItem);
        array.splice(index, 1)
    }
    _getDirectoryDataItems(directoryDataObj) {
        if (!directoryDataObj) {
            return this._data
        }
        var dataItems = this._subFileItemsGetter(directoryDataObj);
        if (!Array.isArray(dataItems)) {
            dataItems = [];
            this._subFileItemsSetter(directoryDataObj, dataItems)
        }
        return dataItems
    }
    _getItems(parentDir) {
        this._validateDirectoryExists(parentDir);
        var pathInfo = parentDir.getFullPathInfo();
        var parentDirKey = pathInfo && pathInfo.length > 0 ? pathInfo[pathInfo.length - 1].key : null;
        var dirFileObjects = this._data;
        if (parentDirKey) {
            var directoryEntry = this._findFileItemObj(pathInfo);
            dirFileObjects = directoryEntry && this._subFileItemsGetter(directoryEntry) || []
        }
        this._ensureKeysForDuplicateNameItems(dirFileObjects);
        return this._convertDataObjectsToFileItems(dirFileObjects, pathInfo)
    }
    _ensureKeysForDuplicateNameItems(dataObjects) {
        var names = {};
        dataObjects.forEach(obj => {
            var name = this._nameGetter(obj);
            if (names[name]) {
                this._ensureDataObjectKey(obj)
            } else {
                names[name] = true
            }
        })
    }
    _findDataObject(item) {
        if (item.isRoot()) {
            return null
        }
        var result = this._findFileItemObj(item.getFullPathInfo());
        if (!result) {
            var errorCode = item.isDirectory ? ErrorCode.DirectoryNotFound : ErrorCode.FileNotFound;
            throw new FileSystemError(errorCode, item)
        }
        return result
    }
    _findFileItemObj(pathInfo) {
        var _this = this;
        if (!Array.isArray(pathInfo)) {
            pathInfo = []
        }
        var currentPath = "";
        var fileItemObj = null;
        var fileItemObjects = this._data;
        var _loop = function(i) {
            fileItemObj = find(fileItemObjects, item => {
                var hasCorrectFileItemType = _this._isDirGetter(item) || i === pathInfo.length - 1;
                return _this._getKeyFromDataObject(item, currentPath) === pathInfo[i].key && _this._nameGetter(item) === pathInfo[i].name && hasCorrectFileItemType
            });
            if (fileItemObj) {
                currentPath = pathCombine(currentPath, _this._nameGetter(fileItemObj));
                fileItemObjects = _this._subFileItemsGetter(fileItemObj)
            }
        };
        for (var i = 0; i < pathInfo.length && (0 === i || fileItemObj); i++) {
            _loop(i)
        }
        return fileItemObj
    }
    _getKeyFromDataObject(dataObj, defaultKeyPrefix) {
        var key = this._keyGetter(dataObj);
        var relativeName = pathCombine(defaultKeyPrefix, this._nameGetter(dataObj));
        return this._getDataObjectKey(key, relativeName)
    }
    _getDataObjectKey(key, relativeName) {
        return key ? key : relativeName
    }
    _ensureDataObjectKey(dataObj) {
        var key = this._keyGetter(dataObj);
        if (!key) {
            key = String(new Guid);
            this._keySetter(dataObj, key)
        }
        return key
    }
    _hasSubDirs(dataObj) {
        var subItems = ensureDefined(this._subFileItemsGetter(dataObj), []);
        if (!Array.isArray(subItems)) {
            return true
        }
        for (var i = 0; i < subItems.length; i++) {
            if (true === this._isDirGetter(subItems[i])) {
                return true
            }
        }
        return false
    }
    _getSetter(expr) {
        return isFunction(expr) ? expr : compileSetter(expr)
    }
    _isFileItemExists(fileItem) {
        return fileItem.isDirectory && fileItem.isRoot() || !!this._findFileItemObj(fileItem.getFullPathInfo())
    }
    _createFileReader() {
        return new window.FileReader
    }
}

function getJSZip() {
    if (!JSZip) {
        throw Errors.Error("E1041", "JSZip")
    }
    return JSZip
}
export default ObjectFileSystemProvider;
