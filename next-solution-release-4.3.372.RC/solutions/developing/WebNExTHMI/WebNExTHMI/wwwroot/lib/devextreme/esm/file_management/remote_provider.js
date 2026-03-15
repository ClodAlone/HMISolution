/**
 * DevExtreme (esm/file_management/remote_provider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import ajax from "../core/utils/ajax";
import {
    ensureDefined,
    noop
} from "../core/utils/common";
import Guid from "../core/guid";
import {
    getWindow
} from "../core/utils/window";
import {
    each
} from "../core/utils/iterator";
import {
    Deferred
} from "../core/utils/deferred";
import eventsEngine from "../events/core/events_engine";
import FileSystemProviderBase from "./provider_base";
import {
    compileGetter
} from "../core/utils/data";
import {
    isDefined,
    isEmptyObject,
    isFunction
} from "../core/utils/type";
var window = getWindow();
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
class RemoteFileSystemProvider extends FileSystemProviderBase {
    constructor(options) {
        options = ensureDefined(options, {});
        super(options);
        this._endpointUrl = options.endpointUrl;
        this._beforeAjaxSend = options.beforeAjaxSend;
        this._beforeSubmit = options.beforeSubmit;
        this._requestHeaders = options.requestHeaders;
        this._hasSubDirsGetter = compileGetter(options.hasSubDirectoriesExpr || "hasSubDirectories")
    }
    getItems(parentDir) {
        var pathInfo = parentDir.getFullPathInfo();
        return this._executeRequest(FILE_SYSTEM_COMMNAD.GET_DIR_CONTENTS, {
            pathInfo: pathInfo
        }).then(result => this._convertDataObjectsToFileItems(result.result, pathInfo))
    }
    renameItem(item, name) {
        return this._executeRequest(FILE_SYSTEM_COMMNAD.RENAME, {
            pathInfo: item.getFullPathInfo(),
            isDirectory: item.isDirectory,
            name: name
        })
    }
    createDirectory(parentDir, name) {
        return this._executeRequest(FILE_SYSTEM_COMMNAD.CREATE_DIR, {
            pathInfo: parentDir.getFullPathInfo(),
            name: name
        })
    }
    deleteItems(items) {
        return items.map(item => this._executeRequest(FILE_SYSTEM_COMMNAD.REMOVE, {
            pathInfo: item.getFullPathInfo(),
            isDirectory: item.isDirectory
        }))
    }
    moveItems(items, destinationDirectory) {
        return items.map(item => this._executeRequest(FILE_SYSTEM_COMMNAD.MOVE, {
            sourcePathInfo: item.getFullPathInfo(),
            sourceIsDirectory: item.isDirectory,
            destinationPathInfo: destinationDirectory.getFullPathInfo()
        }))
    }
    copyItems(items, destinationFolder) {
        return items.map(item => this._executeRequest(FILE_SYSTEM_COMMNAD.COPY, {
            sourcePathInfo: item.getFullPathInfo(),
            sourceIsDirectory: item.isDirectory,
            destinationPathInfo: destinationFolder.getFullPathInfo()
        }))
    }
    uploadFileChunk(fileData, chunksInfo, destinationDirectory) {
        if (0 === chunksInfo.chunkIndex) {
            chunksInfo.customData.uploadId = new Guid
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
            data: {
                [FILE_CHUNK_BLOB_NAME]: chunksInfo.chunkBlob,
                arguments: JSON.stringify(args),
                command: FILE_SYSTEM_COMMNAD.UPLOAD_CHUNK
            },
            upload: {
                onprogress: noop,
                onloadstart: noop,
                onabort: noop
            },
            xhrFields: {},
            cache: false
        };
        var deferred = new Deferred;
        this._beforeSendInternal(ajaxSettings);
        ajax.sendRequest(ajaxSettings).done(result => {
            !result.success && deferred.reject(result) || deferred.resolve()
        }).fail(deferred.reject);
        return deferred.promise()
    }
    abortFileUpload(fileData, chunksInfo, destinationDirectory) {
        return this._executeRequest(FILE_SYSTEM_COMMNAD.ABORT_UPLOAD, {
            uploadId: chunksInfo.customData.uploadId
        })
    }
    downloadItems(items) {
        var args = this._getDownloadArgs(items);
        var $form = $("<form>").css({
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
        eventsEngine.trigger($form, "submit");
        setTimeout(() => $form.remove())
    }
    getItemsContent(items) {
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
                onprogress: noop,
                onloadstart: noop,
                onabort: noop
            },
            xhrFields: {},
            cache: false
        };
        this._beforeSendInternal(ajaxSettings);
        return ajax.sendRequest(ajaxSettings)
    }
    _getDownloadArgs(items) {
        var pathInfoList = items.map(item => item.getFullPathInfo());
        var args = {
            pathInfoList: pathInfoList
        };
        var argsStr = JSON.stringify(args);
        return {
            url: this._endpointUrl,
            arguments: argsStr,
            command: FILE_SYSTEM_COMMNAD.DOWLOAD
        }
    }
    _getItemsIds(items) {
        return items.map(it => it.relativeName)
    }
    _executeRequest(command, args) {
        var method = command === FILE_SYSTEM_COMMNAD.GET_DIR_CONTENTS ? REQUEST_METHOD.GET : REQUEST_METHOD.POST;
        var deferred = new Deferred;
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
        ajax.sendRequest(ajaxSettings).then(result => {
            !result.success && deferred.reject(result) || deferred.resolve(result)
        }, e => deferred.reject(e));
        return deferred.promise()
    }
    _beforeSubmitInternal(formDataEntries) {
        if (isFunction(this._beforeSubmit)) {
            this._beforeSubmit({
                formData: formDataEntries
            })
        }
    }
    _beforeSendInternal(ajaxSettings) {
        if (isFunction(this._beforeAjaxSend)) {
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
        if (isEmptyObject(ajaxSettings.data)) {
            delete ajaxSettings.data
        } else if (ajaxSettings.responseType || ajaxSettings.upload) {
            ajaxSettings.data = this._createFormData(ajaxSettings.data)
        }
    }
    _createFormData(formDataEntries) {
        var formData = new window.FormData;
        for (var entryName in formDataEntries) {
            if (Object.prototype.hasOwnProperty.call(formDataEntries, entryName) && isDefined(formDataEntries[entryName])) {
                formData.append(entryName, formDataEntries[entryName])
            }
        }
        return formData
    }
    _appendFormDataInputsToForm(formDataEntries, formElement) {
        for (var entryName in formDataEntries) {
            if (Object.prototype.hasOwnProperty.call(formDataEntries, entryName) && isDefined(formDataEntries[entryName])) {
                $("<input>").attr({
                    type: "hidden",
                    name: entryName,
                    value: formDataEntries[entryName]
                }).appendTo(formElement)
            }
        }
    }
    _getEndpointUrl(command, args) {
        var queryString = this._getQueryString({
            command: command,
            arguments: JSON.stringify(args)
        });
        var separator = this._endpointUrl && this._endpointUrl.indexOf("?") > 0 ? "&" : "?";
        return this._endpointUrl + separator + queryString
    }
    _getQueryString(params) {
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
    }
    _processQueryStringArrayParam(key, array, pairs) {
        each(array, (_, item) => {
            var pair = this._getQueryStringPair(key, item);
            pairs.push(pair)
        })
    }
    _getQueryStringPair(key, value) {
        return encodeURIComponent(key) + "=" + encodeURIComponent(value)
    }
    _hasSubDirs(dataObj) {
        var hasSubDirs = this._hasSubDirsGetter(dataObj);
        return "boolean" === typeof hasSubDirs ? hasSubDirs : true
    }
    _getKeyExpr(options) {
        return options.keyExpr || "key"
    }
}
export default RemoteFileSystemProvider;
