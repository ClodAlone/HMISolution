/**
 * DevExtreme (cjs/file_management/remote_provider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../core/renderer"));
var _ajax = _interopRequireDefault(require("../core/utils/ajax"));
var _common = require("../core/utils/common");
var _guid = _interopRequireDefault(require("../core/guid"));
var _window = require("../core/utils/window");
var _iterator = require("../core/utils/iterator");
var _deferred = require("../core/utils/deferred");
var _events_engine = _interopRequireDefault(require("../events/core/events_engine"));
var _provider_base = _interopRequireDefault(require("./provider_base"));
var _data2 = require("../core/utils/data");
var _type = require("../core/utils/type");

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
var window = (0, _window.getWindow)();
var FILE_CHUNK_BLOB_NAME = "chunk";
var FILE_SYSTEM_COMMNAD = {
    GET_DIR_CONTENTS: "GetDirContents",
    CREATE_DIR: "CreateDir",
    RENAME: "Rename",
    MOVE: "Move",
    COPY: "Copy",
    REMOVE: "Remove",
    UPLOAD_CHUNK: "UploadChunk",
    ABORT_UPLOAD: "AbortUpload",
    DOWLOAD: "Download"
};
var REQUEST_METHOD = {
    GET: "GET",
    POST: "POST"
};
var RemoteFileSystemProvider = function(_FileSystemProviderBa) {
    _inheritsLoose(RemoteFileSystemProvider, _FileSystemProviderBa);

    function RemoteFileSystemProvider(options) {
        var _this;
        options = (0, _common.ensureDefined)(options, {});
        _this = _FileSystemProviderBa.call(this, options) || this;
        _this._endpointUrl = options.endpointUrl;
        _this._beforeAjaxSend = options.beforeAjaxSend;
        _this._beforeSubmit = options.beforeSubmit;
        _this._requestHeaders = options.requestHeaders;
        _this._hasSubDirsGetter = (0, _data2.compileGetter)(options.hasSubDirectoriesExpr || "hasSubDirectories");
        return _this
    }
    var _proto = RemoteFileSystemProvider.prototype;
    _proto.getItems = function(parentDir) {
        var _this2 = this;
        var pathInfo = parentDir.getFullPathInfo();
        return this._executeRequest(FILE_SYSTEM_COMMNAD.GET_DIR_CONTENTS, {
            pathInfo: pathInfo
        }).then((function(result) {
            return _this2._convertDataObjectsToFileItems(result.result, pathInfo)
        }))
    };
    _proto.renameItem = function(item, name) {
        return this._executeRequest(FILE_SYSTEM_COMMNAD.RENAME, {
            pathInfo: item.getFullPathInfo(),
            isDirectory: item.isDirectory,
            name: name
        })
    };
    _proto.createDirectory = function(parentDir, name) {
        return this._executeRequest(FILE_SYSTEM_COMMNAD.CREATE_DIR, {
            pathInfo: parentDir.getFullPathInfo(),
            name: name
        })
    };
    _proto.deleteItems = function(items) {
        var _this3 = this;
        return items.map((function(item) {
            return _this3._executeRequest(FILE_SYSTEM_COMMNAD.REMOVE, {
                pathInfo: item.getFullPathInfo(),
                isDirectory: item.isDirectory
            })
        }))
    };
    _proto.moveItems = function(items, destinationDirectory) {
        var _this4 = this;
        return items.map((function(item) {
            return _this4._executeRequest(FILE_SYSTEM_COMMNAD.MOVE, {
                sourcePathInfo: item.getFullPathInfo(),
                sourceIsDirectory: item.isDirectory,
                destinationPathInfo: destinationDirectory.getFullPathInfo()
            })
        }))
    };
    _proto.copyItems = function(items, destinationFolder) {
        var _this5 = this;
        return items.map((function(item) {
            return _this5._executeRequest(FILE_SYSTEM_COMMNAD.COPY, {
                sourcePathInfo: item.getFullPathInfo(),
                sourceIsDirectory: item.isDirectory,
                destinationPathInfo: destinationFolder.getFullPathInfo()
            })
        }))
    };
    _proto.uploadFileChunk = function(fileData, chunksInfo, destinationDirectory) {
        var _data;
        if (0 === chunksInfo.chunkIndex) {
            chunksInfo.customData.uploadId = new _guid.default
        }
        var args = {
            destinationPathInfo: destinationDirectory.getFullPathInfo(),
            chunkMetadata: JSON.stringify({
                UploadId: chunksInfo.customData.uploadId,
                FileName: fileData.name,
                Index: chunksInfo.chunkIndex,
                TotalCount: chunksInfo.chunkCount,
                FileSize: fileData.size
            })
        };
        var ajaxSettings = {
            url: this._endpointUrl,
            headers: this._requestHeaders || {},
            method: REQUEST_METHOD.POST,
            dataType: "json",
            data: (_data = {}, _defineProperty(_data, FILE_CHUNK_BLOB_NAME, chunksInfo.chunkBlob), _defineProperty(_data, "arguments", JSON.stringify(args)), _defineProperty(_data, "command", FILE_SYSTEM_COMMNAD.UPLOAD_CHUNK), _data),
            upload: {
                onprogress: _common.noop,
                onloadstart: _common.noop,
                onabort: _common.noop
            },
            xhrFields: {},
            cache: false
        };
        var deferred = new _deferred.Deferred;
        this._beforeSendInternal(ajaxSettings);
        _ajax.default.sendRequest(ajaxSettings).done((function(result) {
            !result.success && deferred.reject(result) || deferred.resolve()
        })).fail(deferred.reject);
        return deferred.promise()
    };
    _proto.abortFileUpload = function(fileData, chunksInfo, destinationDirectory) {
        return this._executeRequest(FILE_SYSTEM_COMMNAD.ABORT_UPLOAD, {
            uploadId: chunksInfo.customData.uploadId
        })
    };
    _proto.downloadItems = function(items) {
        var args = this._getDownloadArgs(items);
        var $form = (0, _renderer.default)("<form>").css({
            display: "none"
        }).attr({
            method: REQUEST_METHOD.POST,
            action: args.url
        });
        var formDataEntries = {
            command: args.command,
            arguments: args.arguments
        };
        this._beforeSubmitInternal(formDataEntries);
        this._appendFormDataInputsToForm(formDataEntries, $form);
        $form.appendTo("body");
        _events_engine.default.trigger($form, "submit");
        setTimeout((function() {
            return $form.remove()
        }))
    };
    _proto.getItemsContent = function(items) {
        var args = this._getDownloadArgs(items);
        var ajaxSettings = {
            url: args.url,
            headers: this._requestHeaders || {},
            method: REQUEST_METHOD.POST,
            responseType: "arraybuffer",
            data: {
                command: args.command,
                arguments: args.arguments
            },
            upload: {
                onprogress: _common.noop,
                onloadstart: _common.noop,
                onabort: _common.noop
            },
            xhrFields: {},
            cache: false
        };
        this._beforeSendInternal(ajaxSettings);
        return _ajax.default.sendRequest(ajaxSettings)
    };
    _proto._getDownloadArgs = function(items) {
        var pathInfoList = items.map((function(item) {
            return item.getFullPathInfo()
        }));
        var args = {
            pathInfoList: pathInfoList
        };
        var argsStr = JSON.stringify(args);
        return {
            url: this._endpointUrl,
            arguments: argsStr,
            command: FILE_SYSTEM_COMMNAD.DOWLOAD
        }
    };
    _proto._getItemsIds = function(items) {
        return items.map((function(it) {
            return it.relativeName
        }))
    };
    _proto._executeRequest = function(command, args) {
        var method = command === FILE_SYSTEM_COMMNAD.GET_DIR_CONTENTS ? REQUEST_METHOD.GET : REQUEST_METHOD.POST;
        var deferred = new _deferred.Deferred;
        var ajaxSettings = {
            url: this._getEndpointUrl(command, args),
            headers: this._requestHeaders || {},
            method: method,
            dataType: "json",
            data: {},
            xhrFields: {},
            cache: false
        };
        this._beforeSendInternal(ajaxSettings);
        _ajax.default.sendRequest(ajaxSettings).then((function(result) {
            !result.success && deferred.reject(result) || deferred.resolve(result)
        }), (function(e) {
            return deferred.reject(e)
        }));
        return deferred.promise()
    };
    _proto._beforeSubmitInternal = function(formDataEntries) {
        if ((0, _type.isFunction)(this._beforeSubmit)) {
            this._beforeSubmit({
                formData: formDataEntries
            })
        }
    };
    _proto._beforeSendInternal = function(ajaxSettings) {
        if ((0, _type.isFunction)(this._beforeAjaxSend)) {
            var ajaxArguments = {
                headers: ajaxSettings.headers,
                formData: ajaxSettings.data,
                xhrFields: ajaxSettings.xhrFields
            };
            this._beforeAjaxSend(ajaxArguments);
            ajaxSettings.headers = ajaxArguments.headers;
            ajaxSettings.data = ajaxArguments.formData;
            ajaxSettings.xhrFields = ajaxArguments.xhrFields
        }
        if ((0, _type.isEmptyObject)(ajaxSettings.data)) {
            delete ajaxSettings.data
        } else if (ajaxSettings.responseType || ajaxSettings.upload) {
            ajaxSettings.data = this._createFormData(ajaxSettings.data)
        }
    };
    _proto._createFormData = function(formDataEntries) {
        var formData = new window.FormData;
        for (var entryName in formDataEntries) {
            if (Object.prototype.hasOwnProperty.call(formDataEntries, entryName) && (0, _type.isDefined)(formDataEntries[entryName])) {
                formData.append(entryName, formDataEntries[entryName])
            }
        }
        return formData
    };
    _proto._appendFormDataInputsToForm = function(formDataEntries, formElement) {
        for (var entryName in formDataEntries) {
            if (Object.prototype.hasOwnProperty.call(formDataEntries, entryName) && (0, _type.isDefined)(formDataEntries[entryName])) {
                (0, _renderer.default)("<input>").attr({
                    type: "hidden",
                    name: entryName,
                    value: formDataEntries[entryName]
                }).appendTo(formElement)
            }
        }
    };
    _proto._getEndpointUrl = function(command, args) {
        var queryString = this._getQueryString({
            command: command,
            arguments: JSON.stringify(args)
        });
        var separator = this._endpointUrl && this._endpointUrl.indexOf("?") > 0 ? "&" : "?";
        return this._endpointUrl + separator + queryString
    };
    _proto._getQueryString = function(params) {
        var pairs = [];
        var keys = Object.keys(params);
        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];
            var value = params[key];
            if (void 0 === value) {
                continue
            }
            if (null === value) {
                value = ""
            }
            if (Array.isArray(value)) {
                this._processQueryStringArrayParam(key, value, pairs)
            } else {
                var pair = this._getQueryStringPair(key, value);
                pairs.push(pair)
            }
        }
        return pairs.join("&")
    };
    _proto._processQueryStringArrayParam = function(key, array, pairs) {
        var _this6 = this;
        (0, _iterator.each)(array, (function(_, item) {
            var pair = _this6._getQueryStringPair(key, item);
            pairs.push(pair)
        }))
    };
    _proto._getQueryStringPair = function(key, value) {
        return encodeURIComponent(key) + "=" + encodeURIComponent(value)
    };
    _proto._hasSubDirs = function(dataObj) {
        var hasSubDirs = this._hasSubDirsGetter(dataObj);
        return "boolean" === typeof hasSubDirs ? hasSubDirs : true
    };
    _proto._getKeyExpr = function(options) {
        return options.keyExpr || "key"
    };
    return RemoteFileSystemProvider
}(_provider_base.default);
var _default = RemoteFileSystemProvider;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
