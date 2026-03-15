/**
 * DevExtreme (esm/ui/html_editor/modules/resizing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import eventsEngine from "../../../events/core/events_engine";
import {
    name as ClickEvent
} from "../../../events/click";
import {
    addNamespace,
    normalizeKeyName
} from "../../../events/utils/index";
import {
    move
} from "../../../animation/translator";
import devices from "../../../core/devices";
import Resizable from "../../resizable";
import {
    getBoundingRect
} from "../../../core/utils/position";
import Quill from "devextreme-quill";
import BaseModule from "./base";
import {
    getHeight,
    getOuterHeight,
    getOuterWidth,
    getWidth
} from "../../../core/utils/size";
var DX_RESIZE_FRAME_CLASS = "dx-resize-frame";
var DX_TOUCH_DEVICE_CLASS = "dx-touch-device";
var MODULE_NAMESPACE = "dxHtmlResizingModule";
var KEYDOWN_EVENT = addNamespace("keydown", MODULE_NAMESPACE);
var SCROLL_EVENT = addNamespace("scroll", MODULE_NAMESPACE);
var MOUSEDOWN_EVENT = addNamespace("mousedown", MODULE_NAMESPACE);
var FRAME_PADDING = 1;
export default class ResizingModule extends BaseModule {
    constructor(quill, options) {
        super(quill, options);
        this.allowedTargets = options.allowedTargets || ["image"];
        this.enabled = !!options.enabled;
        this._hideFrameWithContext = this.hideFrame.bind(this);
        this._framePositionChangedHandler = this._prepareFramePositionChangedHandler();
        if (this.enabled) {
            this._attachEvents();
            this._createResizeFrame()
        }
    }
    _attachEvents() {
        eventsEngine.on(this.quill.root, addNamespace(ClickEvent, MODULE_NAMESPACE), this._clickHandler.bind(this));
        eventsEngine.on(this.quill.root, SCROLL_EVENT, this._framePositionChangedHandler);
        this.editorInstance.on("focusOut", this._hideFrameWithContext);
        this.quill.on("text-change", this._framePositionChangedHandler)
    }
    _detachEvents() {
        eventsEngine.off(this.quill.root, MODULE_NAMESPACE);
        this.editorInstance.off("focusOut", this._hideFrameWithContext);
        this.quill.off("text-change", this._framePositionChangedHandler)
    }
    _clickHandler(e) {
        if (this._isAllowedTarget(e.target)) {
            if (this._$target === e.target) {
                return
            }
            this._$target = e.target;
            var $target = $(this._$target);
            var minWidth = Math.max(getOuterWidth($target) - getWidth($target), this.resizable.option("minWidth"));
            var minHeight = Math.max(getOuterHeight($target) - getHeight($target), this.resizable.option("minHeight"));
            this.resizable.option({
                minWidth: minWidth,
                minHeight: minHeight
            });
            this.updateFramePosition();
            this.showFrame();
            this._adjustSelection()
        } else if (this._$target) {
            this.hideFrame()
        }
    }
    _prepareFramePositionChangedHandler(e) {
        return () => {
            if (this._$target) {
                this.updateFramePosition()
            }
        }
    }
    _adjustSelection() {
        if (!this.quill.getSelection()) {
            this.quill.setSelection(0, 0)
        }
    }
    _isAllowedTarget(targetElement) {
        return this._isImage(targetElement)
    }
    _isImage(targetElement) {
        return -1 !== this.allowedTargets.indexOf("image") && "IMG" === targetElement.tagName.toUpperCase()
    }
    showFrame() {
        this._$resizeFrame.show();
        eventsEngine.on(this.quill.root, KEYDOWN_EVENT, this._handleFrameKeyDown.bind(this))
    }
    _handleFrameKeyDown(e) {
        var keyName = normalizeKeyName(e);
        if ("del" === keyName || "backspace" === keyName) {
            this._deleteImage()
        }
        this.hideFrame()
    }
    hideFrame() {
        this._$target = null;
        this._$resizeFrame.hide();
        eventsEngine.off(this.quill.root, KEYDOWN_EVENT)
    }
    updateFramePosition() {
        var {
            height: height,
            width: width,
            top: targetTop,
            left: targetLeft
        } = getBoundingRect(this._$target);
        var {
            top: containerTop,
            left: containerLeft
        } = getBoundingRect(this.quill.root);
        var borderWidth = this._getBorderWidth();
        this._$resizeFrame.css({
            height: height,
            width: width,
            padding: FRAME_PADDING,
            top: targetTop - containerTop - borderWidth - FRAME_PADDING,
            left: targetLeft - containerLeft - borderWidth - FRAME_PADDING
        });
        move(this._$resizeFrame, {
            left: 0,
            top: 0
        })
    }
    _getBorderWidth() {
        return parseInt(this._$resizeFrame.css("borderTopWidth"))
    }
    _createResizeFrame() {
        if (this._$resizeFrame) {
            return
        }
        var {
            deviceType: deviceType
        } = devices.current();
        this._$resizeFrame = $("<div>").addClass(DX_RESIZE_FRAME_CLASS).toggleClass(DX_TOUCH_DEVICE_CLASS, "desktop" !== deviceType).appendTo(this.editorInstance._getQuillContainer()).hide();
        eventsEngine.on(this._$resizeFrame, MOUSEDOWN_EVENT, e => {
            e.preventDefault()
        });
        this.resizable = this.editorInstance._createComponent(this._$resizeFrame, Resizable, {
            _keepAspectRatio: true,
            onResize: e => {
                if (!this._$target) {
                    return
                }
                $(this._$target).attr({
                    height: e.height,
                    width: e.width
                });
                this.updateFramePosition()
            }
        })
    }
    _deleteImage() {
        if (this._isAllowedTarget(this._$target)) {
            var _Quill$find;
            null === (_Quill$find = Quill.find(this._$target)) || void 0 === _Quill$find ? void 0 : _Quill$find.deleteAt(0)
        }
    }
    option(option, value) {
        if ("mediaResizing" === option) {
            this.handleOptionChangeValue(value);
            return
        }
        if ("enabled" === option) {
            this.enabled = value;
            value ? this._attachEvents() : this._detachEvents()
        } else if ("allowedTargets" === option && Array.isArray(value)) {
            this.allowedTargets = value
        }
    }
    clean() {
        this._detachEvents();
        this._$resizeFrame.remove();
        this._$resizeFrame = void 0
    }
}
