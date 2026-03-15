/**
 * DevExtreme (esm/viz/vector_map/tracker.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "../../events/core/events_engine";
import {
    getNavigator,
    hasProperty
} from "../../core/utils/window";
import domAdapter from "../../core/dom_adapter";
import {
    makeEventEmitter
} from "./event_emitter";
import {
    addNamespace
} from "../../events/utils/index";
import {
    name as wheelEventName
} from "../../events/core/wheel";
import {
    parseScalar
} from "../core/utils";
var navigator = getNavigator();
var _math = Math;
var _abs = _math.abs;
var _sqrt = _math.sqrt;
var _round = _math.round;
var _addNamespace = addNamespace;
var _NAME = "dxVectorMap";
var EVENT_START = "start";
var EVENT_MOVE = "move";
var EVENT_END = "end";
var EVENT_ZOOM = "zoom";
var EVENT_HOVER_ON = "hover-on";
var EVENT_HOVER_OFF = "hover-off";
var EVENT_CLICK = "click";
var EVENT_FOCUS_ON = "focus-on";
var EVENT_FOCUS_MOVE = "focus-move";
var EVENT_FOCUS_OFF = "focus-off";
var CLICK_TIME_THRESHOLD = 500;
var CLICK_COORD_THRESHOLD_MOUSE = 5;
var CLICK_COORD_THRESHOLD_TOUCH = 20;
var DRAG_COORD_THRESHOLD_MOUSE = 5;
var DRAG_COORD_THRESHOLD_TOUCH = 10;
var FOCUS_OFF_DELAY = 100;
var WHEEL_COOLDOWN = 50;
var WHEEL_DIRECTION_COOLDOWN = 300;
var EVENTS;
var Focus;
setupEvents();
export function Tracker(parameters) {
    var that = this;
    that._root = parameters.root;
    that._createEventHandlers(parameters.dataKey);
    that._createProjectionHandlers(parameters.projection);
    that._initEvents();
    that._focus = new Focus((function(name, arg) {
        that._fire(name, arg)
    }));
    that._attachHandlers()
}
Tracker.prototype = {
    constructor: Tracker,
    dispose: function() {
        this._detachHandlers();
        this._disposeEvents();
        this._focus.dispose();
        this._root = this._focus = this._docHandlers = this._rootHandlers = null
    },
    _eventNames: [EVENT_START, EVENT_MOVE, EVENT_END, EVENT_ZOOM, EVENT_CLICK, EVENT_HOVER_ON, EVENT_HOVER_OFF, EVENT_FOCUS_ON, EVENT_FOCUS_OFF, EVENT_FOCUS_MOVE],
    _startClick: function(event, data) {
        if (!data) {
            return
        }
        var coords = getEventCoords(event);
        this._clickState = {
            x: coords.x,
            y: coords.y,
            threshold: isTouchEvent(event) ? CLICK_COORD_THRESHOLD_TOUCH : CLICK_COORD_THRESHOLD_MOUSE,
            time: Date.now()
        }
    },
    _endClick: function(event, data) {
        var state = this._clickState;
        var threshold;
        var coords;
        if (!state) {
            return
        }
        if (data && Date.now() - state.time <= CLICK_TIME_THRESHOLD) {
            threshold = state.threshold;
            coords = getEventCoords(event);
            if (_abs(coords.x - state.x) <= threshold && _abs(coords.y - state.y) <= threshold) {
                this._fire(EVENT_CLICK, {
                    data: data,
                    x: coords.x,
                    y: coords.y,
                    $event: event
                })
            }
        }
        this._clickState = null
    },
    _startDrag: function(event, data) {
        if (!data) {
            return
        }
        var coords = getEventCoords(event);
        var state = this._dragState = {
            x: coords.x,
            y: coords.y,
            data: data
        };
        this._fire(EVENT_START, {
            x: state.x,
            y: state.y,
            data: state.data
        })
    },
    _moveDrag: function(event, data) {
        var state = this._dragState;
        if (!state) {
            return
        }
        var coords = getEventCoords(event);
        var threshold = isTouchEvent(event) ? DRAG_COORD_THRESHOLD_TOUCH : DRAG_COORD_THRESHOLD_MOUSE;
        if (state.active || _abs(coords.x - state.x) > threshold || _abs(coords.y - state.y) > threshold) {
            state.x = coords.x;
            state.y = coords.y;
            state.active = true;
            state.data = data || {};
            this._fire(EVENT_MOVE, {
                x: state.x,
                y: state.y,
                data: state.data
            })
        }
    },
    _endDrag: function() {
        var state = this._dragState;
        if (!state) {
            return
        }
        this._dragState = null;
        this._fire(EVENT_END, {
            x: state.x,
            y: state.y,
            data: state.data
        })
    },
    _wheelZoom: function(event, data) {
        if (!data) {
            return
        }
        var lock = this._wheelLock;
        var time = Date.now();
        if (time - lock.time <= WHEEL_COOLDOWN) {
            return
        }
        if (time - lock.dirTime > WHEEL_DIRECTION_COOLDOWN) {
            lock.dir = 0
        }
        var delta = adjustWheelDelta(event.delta / 120 || 0, lock);
        if (0 === delta) {
            return
        }
        var coords = getEventCoords(event);
        this._fire(EVENT_ZOOM, {
            delta: delta,
            x: coords.x,
            y: coords.y
        });
        lock.time = lock.dirTime = time
    },
    _startZoom: function(event, data) {
        if (!isTouchEvent(event) || !data) {
            return
        }
        var state = this._zoomState = this._zoomState || {};
        var coords;
        var pointer2;
        if (state.pointer1 && state.pointer2) {
            return
        }
        if (void 0 === state.pointer1) {
            state.pointer1 = getPointerId(event) || 0;
            coords = getMultitouchEventCoords(event, state.pointer1);
            state.x1 = state.x1_0 = coords.x;
            state.y1 = state.y1_0 = coords.y
        }
        if (void 0 === state.pointer2) {
            pointer2 = getPointerId(event) || 1;
            if (pointer2 !== state.pointer1) {
                coords = getMultitouchEventCoords(event, pointer2);
                if (coords) {
                    state.x2 = state.x2_0 = coords.x;
                    state.y2 = state.y2_0 = coords.y;
                    state.pointer2 = pointer2;
                    state.ready = true;
                    this._endDrag()
                }
            }
        }
    },
    _moveZoom: function(event) {
        var state = this._zoomState;
        var coords;
        if (!state || !isTouchEvent(event)) {
            return
        }
        if (void 0 !== state.pointer1) {
            coords = getMultitouchEventCoords(event, state.pointer1);
            if (coords) {
                state.x1 = coords.x;
                state.y1 = coords.y
            }
        }
        if (void 0 !== state.pointer2) {
            coords = getMultitouchEventCoords(event, state.pointer2);
            if (coords) {
                state.x2 = coords.x;
                state.y2 = coords.y
            }
        }
    },
    _endZoom: function(event) {
        var state = this._zoomState;
        var startDistance;
        var currentDistance;
        if (!state || !isTouchEvent(event)) {
            return
        }
        if (state.ready) {
            startDistance = getDistance(state.x1_0, state.y1_0, state.x2_0, state.y2_0);
            currentDistance = getDistance(state.x1, state.y1, state.x2, state.y2);
            this._fire(EVENT_ZOOM, {
                ratio: currentDistance / startDistance,
                x: (state.x1_0 + state.x2_0) / 2,
                y: (state.y1_0 + state.y2_0) / 2
            })
        }
        this._zoomState = null
    },
    _startHover: function(event, data) {
        this._doHover(event, data, true)
    },
    _moveHover: function(event, data) {
        this._doHover(event, data, false)
    },
    _doHover: function(event, data, isTouch) {
        if (this._dragState && this._dragState.active || this._zoomState && this._zoomState.ready) {
            this._cancelHover();
            return
        }
        if (isTouchEvent(event) !== isTouch || this._hoverTarget === event.target || this._hoverState && this._hoverState.data === data) {
            return
        }
        this._cancelHover();
        if (data) {
            this._hoverState = {
                data: data
            };
            this._fire(EVENT_HOVER_ON, {
                data: data
            })
        }
        this._hoverTarget = event.target
    },
    _cancelHover: function() {
        var state = this._hoverState;
        this._hoverState = this._hoverTarget = null;
        if (state) {
            this._fire(EVENT_HOVER_OFF, {
                data: state.data
            })
        }
    },
    _startFocus: function(event, data) {
        this._doFocus(event, data, true)
    },
    _moveFocus: function(event, data) {
        this._doFocus(event, data, false)
    },
    _doFocus: function(event, data, isTouch) {
        if (this._dragState && this._dragState.active || this._zoomState && this._zoomState.ready) {
            this._cancelFocus();
            return
        }
        if (isTouchEvent(event) !== isTouch) {
            return
        }
        this._focus.turnOff();
        data && this._focus.turnOn(data, getEventCoords(event))
    },
    _cancelFocus: function() {
        this._focus.cancel()
    },
    _createEventHandlers: function(DATA_KEY) {
        var that = this;
        that._docHandlers = {};
        that._rootHandlers = {};
        that._rootHandlers[EVENTS.start] = that._docHandlers[EVENTS.start] = function(event) {
            var isTouch = isTouchEvent(event);
            var data = getData(event);
            if (isTouch && !that._isTouchEnabled) {
                return
            }
            if (data) {
                event.preventDefault();
                event.stopPropagation()
            }
            that._startClick(event, data);
            that._startDrag(event, data);
            that._startZoom(event, data);
            that._startHover(event, data);
            that._startFocus(event, data)
        };
        that._docHandlers[EVENTS.move] = function(event) {
            var isTouch = isTouchEvent(event);
            var data = getData(event);
            if (isTouch && !that._isTouchEnabled) {
                return
            }
            that._moveDrag(event, data);
            that._moveZoom(event, data);
            that._moveHover(event, data);
            that._moveFocus(event, data)
        };
        that._docHandlers[EVENTS.end] = function(event) {
            var isTouch = isTouchEvent(event);
            var data = getData(event);
            if (isTouch && !that._isTouchEnabled) {
                return
            }
            that._endClick(event, data);
            that._endDrag(event, data);
            that._endZoom(event, data)
        };
        that._rootHandlers[EVENTS.wheel] = function(event) {
            that._cancelFocus();
            if (!that._isWheelEnabled) {
                return
            }
            var data = getData(event);
            if (data) {
                event.preventDefault();
                event.stopPropagation();
                that._wheelZoom(event, data)
            }
        };
        that._wheelLock = {
            dir: 0
        };

        function getData(event) {
            var target = event.target;
            return ("tspan" === target.tagName ? target.parentNode : target)[DATA_KEY]
        }
    },
    _createProjectionHandlers: function(projection) {
        var that = this;
        projection.on({
            center: handler,
            zoom: handler
        });

        function handler() {
            that._cancelFocus()
        }
    },
    reset: function() {
        this._clickState = null;
        this._endDrag();
        this._cancelHover();
        this._cancelFocus()
    },
    setOptions: function(options) {
        this.reset();
        this._detachHandlers();
        this._isTouchEnabled = !!parseScalar(options.touchEnabled, true);
        this._isWheelEnabled = !!parseScalar(options.wheelEnabled, true);
        this._attachHandlers()
    },
    _detachHandlers: function() {
        if (this._isTouchEnabled) {
            this._root.css({
                "touch-action": "",
                "-webkit-user-select": ""
            }).off(_addNamespace("MSHoldVisual", _NAME)).off(_addNamespace("contextmenu", _NAME))
        }
        eventsEngine.off(domAdapter.getDocument(), this._docHandlers);
        this._root.off(this._rootHandlers)
    },
    _attachHandlers: function() {
        if (this._isTouchEnabled) {
            this._root.css({
                "touch-action": "none",
                "-webkit-user-select": "none"
            }).on(_addNamespace("MSHoldVisual", _NAME), (function(event) {
                event.preventDefault()
            })).on(_addNamespace("contextmenu", _NAME), (function(event) {
                isTouchEvent(event) && event.preventDefault()
            }))
        }
        eventsEngine.on(domAdapter.getDocument(), this._docHandlers);
        this._root.on(this._rootHandlers)
    }
};
Focus = function(fire) {
    var that = this;
    var _activeData = null;
    var _data = null;
    var _disabled = false;
    var _offTimer = null;
    var _x;
    var _y;
    that.dispose = function() {
        clearTimeout(_offTimer);
        that.turnOn = that.turnOff = that.cancel = that.dispose = that = fire = _activeData = _data = _offTimer = null
    };
    that.turnOn = function(data, coords) {
        if (data === _data && _disabled) {
            return
        }
        _disabled = false;
        _data = data;
        if (_activeData) {
            _x = coords.x;
            _y = coords.y;
            if (_data === _activeData) {
                fire(EVENT_FOCUS_MOVE, {
                    data: _data,
                    x: _x,
                    y: _y
                });
                onCheck(true)
            } else {
                fire(EVENT_FOCUS_ON, {
                    data: _data,
                    x: _x,
                    y: _y,
                    done: onCheck
                })
            }
        } else {
            _x = coords.x;
            _y = coords.y;
            fire(EVENT_FOCUS_ON, {
                data: _data,
                x: _x,
                y: _y,
                done: onCheck
            })
        }

        function onCheck(result) {
            _disabled = !result;
            if (result) {
                _activeData = _data;
                clearTimeout(_offTimer);
                _offTimer = null
            }
        }
    };
    that.turnOff = function() {
        _data = null;
        if (_activeData && !_disabled) {
            _offTimer = _offTimer || setTimeout((function() {
                _offTimer = null;
                fire(EVENT_FOCUS_OFF, {
                    data: _activeData
                });
                _activeData = null
            }), FOCUS_OFF_DELAY)
        }
    };
    that.cancel = function() {
        clearTimeout(_offTimer);
        if (_activeData) {
            fire(EVENT_FOCUS_OFF, {
                data: _activeData
            })
        }
        _activeData = _data = _offTimer = null
    }
};
makeEventEmitter(Tracker);

function getDistance(x1, y1, x2, y2) {
    return _sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2))
}

function isTouchEvent(event) {
    var type = event.originalEvent.type;
    var pointerType = event.originalEvent.pointerType;
    return /^touch/.test(type) || /^MSPointer/.test(type) && 4 !== pointerType || /^pointer/.test(type) && "mouse" !== pointerType
}

function selectItem(flags, items) {
    var i = 0;
    var ii = flags.length;
    var item;
    for (; i < ii; ++i) {
        if (flags[i]) {
            item = items[i];
            break
        }
    }
    return _addNamespace(item || items[i], _NAME)
}

function setupEvents() {
    var flags = [navigator.pointerEnabled, navigator.msPointerEnabled, hasProperty("ontouchstart")];
    EVENTS = {
        start: selectItem(flags, ["pointerdown", "MSPointerDown", "touchstart mousedown", "mousedown"]),
        move: selectItem(flags, ["pointermove", "MSPointerMove", "touchmove mousemove", "mousemove"]),
        end: selectItem(flags, ["pointerup", "MSPointerUp", "touchend mouseup", "mouseup"]),
        wheel: _addNamespace(wheelEventName, _NAME)
    }
}

function getEventCoords(event) {
    var originalEvent = event.originalEvent;
    var touch = originalEvent.touches && originalEvent.touches[0] || {};
    return {
        x: touch.pageX || originalEvent.pageX || event.pageX,
        y: touch.pageY || originalEvent.pageY || event.pageY
    }
}

function getPointerId(event) {
    return event.originalEvent.pointerId
}

function getMultitouchEventCoords(event, pointerId) {
    var originalEvent = event.originalEvent;
    if (void 0 !== originalEvent.pointerId) {
        originalEvent = originalEvent.pointerId === pointerId ? originalEvent : null
    } else {
        originalEvent = originalEvent.touches[pointerId]
    }
    return originalEvent ? {
        x: originalEvent.pageX || event.pageX,
        y: originalEvent.pageY || event.pageY
    } : null
}

function adjustWheelDelta(delta, lock) {
    if (0 === delta) {
        return 0
    }
    var _delta = _abs(delta);
    var sign = _round(delta / _delta);
    if (lock.dir && sign !== lock.dir) {
        return 0
    }
    lock.dir = sign;
    if (_delta < .1) {
        _delta = 0
    } else if (_delta < 1) {
        _delta = 1
    } else if (_delta > 4) {
        _delta = 4
    } else {
        _delta = _round(_delta)
    }
    return sign * _delta
}
