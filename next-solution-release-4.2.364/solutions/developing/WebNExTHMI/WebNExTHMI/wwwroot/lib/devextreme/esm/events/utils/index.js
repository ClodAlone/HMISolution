/**
 * DevExtreme (esm/events/utils/index.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import mappedAddNamespace from "./add_namespace";
import eventsEngine from "../core/events_engine";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    focused
} from "../../ui/widget/selectors";
var KEY_MAP = {
    backspace: "backspace",
    tab: "tab",
    enter: "enter",
    escape: "escape",
    pageup: "pageUp",
    pagedown: "pageDown",
    end: "end",
    home: "home",
    arrowleft: "leftArrow",
    arrowup: "upArrow",
    arrowright: "rightArrow",
    arrowdown: "downArrow",
    delete: "del",
    " ": "space",
    f: "F",
    a: "A",
    "*": "asterisk",
    "-": "minus",
    alt: "alt",
    control: "control",
    shift: "shift"
};
var LEGACY_KEY_CODES = {
    8: "backspace",
    9: "tab",
    13: "enter",
    27: "escape",
    33: "pageUp",
    34: "pageDown",
    35: "end",
    36: "home",
    37: "leftArrow",
    38: "upArrow",
    39: "rightArrow",
    40: "downArrow",
    46: "del",
    32: "space",
    70: "F",
    65: "A",
    106: "asterisk",
    109: "minus",
    189: "minus",
    173: "minus",
    16: "shift",
    17: "control",
    18: "alt"
};
var EVENT_SOURCES_REGEX = {
    dx: /^dx/i,
    mouse: /(mouse|wheel)/i,
    touch: /^touch/i,
    keyboard: /^key/i,
    pointer: /^(ms)?pointer/i
};
var fixMethod = e => e;
var copyEvent = originalEvent => fixMethod(eventsEngine.Event(originalEvent, originalEvent), originalEvent);
var isDxEvent = e => "dx" === eventSource(e);
var isNativeMouseEvent = e => "mouse" === eventSource(e);
var isNativeTouchEvent = e => "touch" === eventSource(e);
export var eventSource = _ref => {
    var {
        type: type
    } = _ref;
    var result = "other";
    each(EVENT_SOURCES_REGEX, (function(key) {
        if (this.test(type)) {
            result = key;
            return false
        }
    }));
    return result
};
export var isPointerEvent = e => "pointer" === eventSource(e);
export var isMouseEvent = e => isNativeMouseEvent(e) || (isPointerEvent(e) || isDxEvent(e)) && "mouse" === e.pointerType;
export var isDxMouseWheelEvent = e => e && "dxmousewheel" === e.type;
export var isTouchEvent = e => isNativeTouchEvent(e) || (isPointerEvent(e) || isDxEvent(e)) && "touch" === e.pointerType;
export var isKeyboardEvent = e => "keyboard" === eventSource(e);
export var isFakeClickEvent = _ref2 => {
    var {
        screenX: screenX,
        offsetX: offsetX,
        pageX: pageX
    } = _ref2;
    return 0 === screenX && !offsetX && 0 === pageX
};
export var eventData = _ref3 => {
    var {
        pageX: pageX,
        pageY: pageY,
        timeStamp: timeStamp
    } = _ref3;
    return {
        x: pageX,
        y: pageY,
        time: timeStamp
    }
};
export var eventDelta = (from, to) => ({
    x: to.x - from.x,
    y: to.y - from.y,
    time: to.time - from.time || 1
});
export var hasTouches = e => {
    var {
        originalEvent: originalEvent,
        pointers: pointers
    } = e;
    if (isNativeTouchEvent(e)) {
        return (originalEvent.touches || []).length
    }
    if (isDxEvent(e)) {
        return (pointers || []).length
    }
    return 0
};
var skipEvents = false;
export var forceSkipEvents = () => skipEvents = true;
export var stopEventsSkipping = () => skipEvents = false;
export var needSkipEvent = e => {
    if (skipEvents) {
        return true
    }
    var {
        target: target
    } = e;
    var $target = $(target);
    var isDropDown = $target.is(".dx-dropdownlist-popup-wrapper *, .dx-dropdownlist-popup-wrapper");
    var isContentEditable = (null === target || void 0 === target ? void 0 : target.isContentEditable) || (null === target || void 0 === target ? void 0 : target.hasAttribute("contenteditable"));
    var touchInEditable = $target.is("input, textarea, select") || isContentEditable;
    if ($target.is(".dx-skip-gesture-event *, .dx-skip-gesture-event") && !isDropDown) {
        return true
    }
    if (isDxMouseWheelEvent(e)) {
        var isTextArea = $target.is("textarea") && $target.hasClass("dx-texteditor-input");
        if (isTextArea || isContentEditable) {
            return false
        }
        var isInputFocused = $target.is("input[type='number'], textarea, select") && $target.is(":focus");
        return isInputFocused
    }
    if (isMouseEvent(e)) {
        return touchInEditable || e.which > 1
    }
    if (isTouchEvent(e)) {
        return touchInEditable && focused($target)
    }
};
export var setEventFixMethod = func => fixMethod = func;
export var createEvent = (originalEvent, args) => {
    var event = copyEvent(originalEvent);
    args && extend(event, args);
    return event
};
export var fireEvent = props => {
    var {
        originalEvent: originalEvent,
        delegateTarget: delegateTarget
    } = props;
    var event = createEvent(originalEvent, props);
    eventsEngine.trigger(delegateTarget || event.target, event);
    return event
};
export var normalizeKeyName = _ref4 => {
    var {
        key: key,
        which: which
    } = _ref4;
    var originalKey = key;
    var isKeySupported = !!key;
    if (key || which) {
        if (isKeySupported) {
            key = KEY_MAP[key.toLowerCase()]
        }
        if (!isKeySupported || !key && which) {
            key = LEGACY_KEY_CODES[which] || String.fromCharCode(which)
        }
        return key || originalKey
    }
};
export var getChar = _ref5 => {
    var {
        key: key,
        which: which
    } = _ref5;
    return key || String.fromCharCode(which)
};
export var addNamespace = mappedAddNamespace;
export var isCommandKeyPressed = _ref6 => {
    var {
        ctrlKey: ctrlKey,
        metaKey: metaKey
    } = _ref6;
    return ctrlKey || metaKey
};
