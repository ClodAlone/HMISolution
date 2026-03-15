/**
 * DevExtreme (cjs/file_management/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.pathCombine = exports.getPathParts = exports.getParentPath = exports.getName = exports.getFileExtension = exports.getEscapedFileName = exports.PATH_SEPARATOR = void 0;
var _iterator = require("../core/utils/iterator");
var PATH_SEPARATOR = "/";
exports.PATH_SEPARATOR = PATH_SEPARATOR;
var getFileExtension = function(path) {
    var index = path.lastIndexOf(".");
    return -1 !== index ? path.substr(index) : ""
};
exports.getFileExtension = getFileExtension;
var getName = function(path) {
    var index = path.lastIndexOf(PATH_SEPARATOR);
    return -1 !== index ? path.substr(index + PATH_SEPARATOR.length) : path
};
exports.getName = getName;
var getParentPath = function(path) {
    var index = path.lastIndexOf(PATH_SEPARATOR);
    return -1 !== index ? path.substr(0, index) : ""
};
exports.getParentPath = getParentPath;
var getPathParts = function(path, includeFullPath) {
    if (!path || "/" === path) {
        return []
    }
    var result = [];
    var pathPart = "";
    for (var i = 0; i < path.length; i++) {
        var char = path.charAt(i);
        if (char === PATH_SEPARATOR) {
            var nextChar = path.charAt(i + 1);
            if (nextChar !== PATH_SEPARATOR) {
                if (pathPart) {
                    result.push(pathPart);
                    pathPart = ""
                }
                char = nextChar
            }
            i++
        }
        pathPart += char
    }
    if (pathPart) {
        result.push(pathPart)
    }
    if (includeFullPath) {
        for (var _i = 0; _i < result.length; _i++) {
            result[_i] = pathCombine(0 === _i ? "" : result[_i - 1], getEscapedFileName(result[_i]))
        }
    }
    return result
};
exports.getPathParts = getPathParts;
var getEscapedFileName = function(fileName) {
    return fileName.replace(/\//g, "//")
};
exports.getEscapedFileName = getEscapedFileName;
var pathCombine = function() {
    var result = "";
    (0, _iterator.each)(arguments, (function(_, arg) {
        if (arg) {
            if (result) {
                result += PATH_SEPARATOR
            }
            result += arg
        }
    }));
    return result
};
exports.pathCombine = pathCombine;
