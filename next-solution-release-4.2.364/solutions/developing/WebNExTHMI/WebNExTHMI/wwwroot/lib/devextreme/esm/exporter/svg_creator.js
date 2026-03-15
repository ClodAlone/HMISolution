/**
 * DevExtreme (esm/exporter/svg_creator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import ajax from "../core/utils/ajax";
import {
    getWindow
} from "../core/utils/window";
var window = getWindow();
import {
    isFunction
} from "../core/utils/type";
import {
    each
} from "../core/utils/iterator";
import {
    getSvgElement,
    getSvgMarkup,
    HIDDEN_FOR_EXPORT
} from "../core/utils/svg";
import {
    when,
    Deferred
} from "../core/utils/deferred";
export var svgCreator = {
    _markup: "",
    _imageArray: {},
    _imageDeferreds: [],
    _getBinaryFile: function(src, callback) {
        ajax.sendRequest({
            url: src,
            method: "GET",
            responseType: "arraybuffer"
        }).done(callback).fail((function() {
            callback(false)
        }))
    },
    _loadImages: function() {
        var that = this;
        each(that._imageArray, (function(src) {
            var deferred = new Deferred;
            that._imageDeferreds.push(deferred);
            that._getBinaryFile(src, (function(response) {
                if (!response) {
                    delete that._imageArray[src];
                    deferred.resolve();
                    return
                }
                var i;
                var binary = "";
                var bytes = new Uint8Array(response);
                var length = bytes.byteLength;
                for (i = 0; i < length; i++) {
                    binary += String.fromCharCode(bytes[i])
                }
                that._imageArray[src] = "data:image/png;base64," + window.btoa(binary);
                deferred.resolve()
            }))
        }))
    },
    _parseImages: function(element) {
        var href;
        var that = this;
        if ("image" === element.tagName) {
            href = $(element).attr("href") || $(element).attr("xlink:href");
            if (!that._imageArray[href]) {
                that._imageArray[href] = ""
            }
        }
        each(element.childNodes, (function(_, element) {
            that._parseImages(element)
        }))
    },
    _prepareImages: function(svgElem) {
        this._parseImages(svgElem);
        this._loadImages();
        return when.apply($, this._imageDeferreds)
    },
    getData: function(data, options) {
        var markup;
        var that = this;
        var svgElem = getSvgElement(data);
        var $svgObject = $(svgElem);
        $svgObject.find("[".concat(HIDDEN_FOR_EXPORT, "]")).remove();
        markup = '<?xml version="1.0" encoding="UTF-8" standalone="yes" ?>' + getSvgMarkup($svgObject.get(0), options.backgroundColor);
        return that._prepareImages(svgElem).then(() => {
            each(that._imageArray, (function(href, dataURI) {
                var regexpString = "href=['|\"]".concat(href, "['|\"]");
                markup = markup.replace(new RegExp(regexpString, "gi"), 'href="'.concat(dataURI, '"'))
            }));
            return isFunction(window.Blob) ? that._getBlob(markup) : that._getBase64(markup)
        })
    },
    _getBlob: function(markup) {
        return new window.Blob([markup], {
            type: "image/svg+xml"
        })
    },
    _getBase64: function(markup) {
        return window.btoa(markup)
    }
};
export function getData(data, options) {
    return svgCreator.getData(data, options)
}
