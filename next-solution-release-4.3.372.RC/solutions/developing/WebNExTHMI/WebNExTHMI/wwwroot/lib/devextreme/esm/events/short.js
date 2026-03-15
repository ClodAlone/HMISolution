/**
 * DevExtreme (esm/events/short.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "./core/events_engine";
import KeyboardProcessor from "./core/keyboard_processor";
import {
    addNamespace as pureAddNamespace
} from "./utils/index";

function addNamespace(event, namespace) {
    return namespace ? pureAddNamespace(event, namespace) : event
}

function executeAction(action, args) {
    return "function" === typeof action ? action(args) : action.execute(args)
}
export var active = {
    on: ($el, active, inactive, opts) => {
        var {
            selector: selector,
            showTimeout: showTimeout,
            hideTimeout: hideTimeout,
            namespace: namespace
        } = opts;
        eventsEngine.on($el, addNamespace("dxactive", namespace), selector, {
            timeout: showTimeout
        }, event => executeAction(active, {
            event: event,
            element: event.currentTarget
        }));
        eventsEngine.on($el, addNamespace("dxinactive", namespace), selector, {
            timeout: hideTimeout
        }, event => executeAction(inactive, {
            event: event,
            element: event.currentTarget
        }))
    },
    off: ($el, _ref) => {
        var {
            namespace: namespace,
            selector: selector
        } = _ref;
        eventsEngine.off($el, addNamespace("dxactive", namespace), selector);
        eventsEngine.off($el, addNamespace("dxinactive", namespace), selector)
    }
};
export var resize = {
    on: function($el, resize) {
        var {
            namespace: namespace
        } = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : {};
        eventsEngine.on($el, addNamespace("dxresize", namespace), resize)
    },
    off: function($el) {
        var {
            namespace: namespace
        } = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {};
        eventsEngine.off($el, addNamespace("dxresize", namespace))
    }
};
export var hover = {
    on: ($el, start, end, _ref2) => {
        var {
            selector: selector,
            namespace: namespace
        } = _ref2;
        eventsEngine.on($el, addNamespace("dxhoverend", namespace), selector, event => end(event));
        eventsEngine.on($el, addNamespace("dxhoverstart", namespace), selector, event => executeAction(start, {
            element: event.target,
            event: event
        }))
    },
    off: ($el, _ref3) => {
        var {
            selector: selector,
            namespace: namespace
        } = _ref3;
        eventsEngine.off($el, addNamespace("dxhoverstart", namespace), selector);
        eventsEngine.off($el, addNamespace("dxhoverend", namespace), selector)
    }
};
export var visibility = {
    on: ($el, shown, hiding, _ref4) => {
        var {
            namespace: namespace
        } = _ref4;
        eventsEngine.on($el, addNamespace("dxhiding", namespace), hiding);
        eventsEngine.on($el, addNamespace("dxshown", namespace), shown)
    },
    off: ($el, _ref5) => {
        var {
            namespace: namespace
        } = _ref5;
        eventsEngine.off($el, addNamespace("dxhiding", namespace));
        eventsEngine.off($el, addNamespace("dxshown", namespace))
    }
};
export var focus = {
    on: ($el, focusIn, focusOut, _ref6) => {
        var {
            namespace: namespace
        } = _ref6;
        eventsEngine.on($el, addNamespace("focusin", namespace), focusIn);
        eventsEngine.on($el, addNamespace("focusout", namespace), focusOut)
    },
    off: ($el, _ref7) => {
        var {
            namespace: namespace
        } = _ref7;
        eventsEngine.off($el, addNamespace("focusin", namespace));
        eventsEngine.off($el, addNamespace("focusout", namespace))
    },
    trigger: $el => eventsEngine.trigger($el, "focus")
};
export var dxClick = {
    on: function($el, click) {
        var {
            namespace: namespace
        } = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : {};
        eventsEngine.on($el, addNamespace("dxclick", namespace), click)
    },
    off: function($el) {
        var {
            namespace: namespace
        } = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {};
        eventsEngine.off($el, addNamespace("dxclick", namespace))
    }
};
export var click = {
    on: function($el, click) {
        var {
            namespace: namespace
        } = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : {};
        eventsEngine.on($el, addNamespace("click", namespace), click)
    },
    off: function($el) {
        var {
            namespace: namespace
        } = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {};
        eventsEngine.off($el, addNamespace("click", namespace))
    }
};
var index = 0;
var keyboardProcessors = {};
var generateListenerId = () => "keyboardProcessorId".concat(index++);
export var keyboard = {
    on: (element, focusTarget, handler) => {
        var listenerId = generateListenerId();
        keyboardProcessors[listenerId] = new KeyboardProcessor({
            element: element,
            focusTarget: focusTarget,
            handler: handler
        });
        return listenerId
    },
    off: listenerId => {
        if (listenerId && keyboardProcessors[listenerId]) {
            keyboardProcessors[listenerId].dispose();
            delete keyboardProcessors[listenerId]
        }
    },
    _getProcessor: listenerId => keyboardProcessors[listenerId]
};
