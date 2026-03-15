/**
 * DevExtreme (esm/file_management/file_system_item.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isString
} from "../core/utils/type";
import {
    pathCombine,
    getFileExtension,
    getPathParts,
    getName,
    getEscapedFileName,
    PATH_SEPARATOR
} from "./utils";
class FileSystemItem {
    constructor() {
        var ctor = isString(arguments[0]) ? this._publicCtor : this._internalCtor;
        ctor.apply(this, arguments)
    }
    _internalCtor(pathInfo, name, isDirectory, key) {
        this.name = name || "";
        this.pathInfo = pathInfo && [...pathInfo] || [];
        this.parentPath = this._getPathByPathInfo(this.pathInfo);
        this.relativeName = pathCombine(this.parentPath, name);
        this.key = key || this._getPathByPathInfo(this.getFullPathInfo(), true);
        this.path = pathCombine(this.parentPath, name);
        this.pathKeys = this.pathInfo.map(_ref => {
            var {
                key: key
            } = _ref;
            return key
        });
        if (!this.isRoot()) {
            this.pathKeys.push(this.key)
        }
        this._initialize(isDirectory)
    }
    _publicCtor(path, isDirectory, pathKeys) {
        this.path = path || "";
        this.pathKeys = pathKeys || [];
        var pathInfo = [];
        var parts = getPathParts(path, true);
        for (var i = 0; i < parts.length - 1; i++) {
            var part = parts[i];
            var pathInfoPart = {
                key: this.pathKeys[i] || part,
                name: getName(part)
            };
            pathInfo.push(pathInfoPart)
        }
        this.pathInfo = pathInfo;
        this.relativeName = path;
        this.name = getName(path);
        this.key = this.pathKeys.length ? this.pathKeys[this.pathKeys.length - 1] : path;
        this.parentPath = parts.length > 1 ? parts[parts.length - 2] : "";
        this._initialize(isDirectory)
    }
    _initialize(isDirectory) {
        this.isDirectory = !!isDirectory;
        this.size = 0;
        this.dateModified = new Date;
        this.thumbnail = "";
        this.tooltipText = ""
    }
    getFullPathInfo() {
        var pathInfo = [...this.pathInfo];
        if (!this.isRoot()) {
            pathInfo.push({
                key: this.key,
                name: this.name
            })
        }
        return pathInfo
    }
    isRoot() {
        return "" === this.path
    }
    getFileExtension() {
        return this.isDirectory ? "" : getFileExtension(this.name)
    }
    equals(item) {
        return item && this.key === item.key
    }
    createClone() {
        var result = new FileSystemItem(this.pathInfo, this.name, this.isDirectory, this.key);
        result.key = this.key;
        result.size = this.size;
        result.dateModified = this.dateModified;
        result.thumbnail = this.thumbnail;
        result.tooltipText = this.tooltipText;
        result.hasSubDirectories = this.hasSubDirectories;
        result.dataItem = this.dataItem;
        return result
    }
    _getPathByPathInfo(pathInfo, escape) {
        return pathInfo.map(info => escape ? getEscapedFileName(info.name) : info.name).join(PATH_SEPARATOR)
    }
}
export default FileSystemItem;
