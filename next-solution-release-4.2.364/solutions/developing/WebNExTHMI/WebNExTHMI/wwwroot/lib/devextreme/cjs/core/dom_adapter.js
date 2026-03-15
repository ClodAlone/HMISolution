/**
 * DevExtreme (cjs/core/dom_adapter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _dependency_injector = _interopRequireDefault(require("./utils/dependency_injector"));
var _common = require("./utils/common");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _typeof(obj) {
    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(obj) {
        return typeof obj
    } : function(obj) {
        return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj
    }, _typeof(obj)
}
var ELEMENT_NODE = 1;
var TEXT_NODE = 3;
var DOCUMENT_NODE = 9;
var nativeDOMAdapterStrategy = {
    querySelectorAll: function(element, selector) {
        return element.querySelectorAll(selector)
    },
    elementMatches: function(element, selector) {
        var _this = this;
        var matches = element.matches || element.matchesSelector || element.mozMatchesSelector || element.msMatchesSelector || element.oMatchesSelector || element.webkitMatchesSelector || function(selector) {
            var doc = element.document || element.ownerDocument;
            if (!doc) {
                return false
            }
            var items = _this.querySelectorAll(doc, selector);
            for (var i = 0; i < items.length; i++) {
                if (items[i] === element) {
                    return true
                }
            }
        };
        return matches.call(element, selector)
    },
    createElement: function(tagName, context) {
        context = context || this._document;
        return context.createElement(tagName)
    },
    createElementNS: function(ns, tagName, context) {
        context = context || this._document;
        return context.createElementNS(ns, tagName)
    },
    createTextNode: function(text, context) {
        context = context || this._document;
        return context.createTextNode(text)
    },
    isNode: function(element) {
        return element && "object" === _typeof(element) && "nodeType" in element && "nodeName" in element
    },
    isElementNode: function(element) {
        return element && element.nodeType === ELEMENT_NODE
    },
    isTextNode: function(element) {
        return element && element.nodeType === TEXT_NODE
    },
    isDocument: function(element) {
        return element && element.nodeType === DOCUMENT_NODE
    },
    removeElement: function(element) {
        var parentNode = element && element.parentNode;
        if (parentNode) {
            parentNode.removeChild(element)
        }
    },
    insertElement: function(parentElement, newElement, nextSiblingElement) {
        if (parentElement && newElement && parentElement !== newElement) {
            if (nextSiblingElement) {
                parentElement.insertBefore(newElement, nextSiblingElement)
            } else {
                parentElement.appendChild(newElement)
            }
        }
    },
    getAttribute: function(element, name) {
        return element.getAttribute(name)
    },
    setAttribute: function(element, name, value) {
        element.setAttribute(name, value)
    },
    removeAttribute: function(element, name) {
        element.removeAttribute(name)
    },
    setProperty: function(element, name, value) {
        element[name] = value
    },
    setText: function(element, text) {
        if (element) {
            element.textContent = text
        }
    },
    setClass: function(element, className, isAdd) {
        if (1 === element.nodeType && className) {
            isAdd ? element.classList.add(className) : element.classList.remove(className)
        }
    },
    setStyle: function(element, name, value) {
        element.style[name] = value || ""
    },
    _document: "undefined" === typeof document ? void 0 : document,
    getDocument: function() {
        return this._document
    },
    getActiveElement: function() {
        return this._document.activeElement
    },
    getBody: function() {
        return this._document.body
    },
    createDocumentFragment: function() {
        return this._document.createDocumentFragment()
    },
    getDocumentElement: function() {
        return this._document.documentElement
    },
    getLocation: function() {
        return this._document.location
    },
    getSelection: function() {
        return this._document.selection
    },
    getReadyState: function() {
        return this._document.readyState
    },
    getHead: function() {
        return this._document.head
    },
    hasDocumentProperty: function(property) {
        return property in this._document
    },
    listen: function(element, event, callback, options) {
        if (!element || !("addEventListener" in element)) {
            return _common.noop
        }
        element.addEventListener(event, callback, options);
        return function() {
            element.removeEventListener(event, callback)
        }
    },
    elementsFromPoint: function(x, y) {
        return this._document.elementsFromPoint(x, y)
    }
};
var _default = (0, _dependency_injector.default)(nativeDOMAdapterStrategy);
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
