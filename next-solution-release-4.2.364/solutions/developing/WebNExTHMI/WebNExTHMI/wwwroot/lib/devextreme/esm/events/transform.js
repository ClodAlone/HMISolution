/**
 * DevExtreme (esm/events/transform.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    sign as mathSign,
    fitIntoRange
} from "../core/utils/math";
import * as iteratorUtils from "../core/utils/iterator";
import {
    hasTouches
} from "./utils/index";
import Emitter from "./core/emitter";
import registerEmitter from "./core/emitter_registrator";
var DX_PREFIX = "dx";
var TRANSFORM = "transform";
var TRANSLATE = "translate";
var PINCH = "pinch";
var ROTATE = "rotate";
var START_POSTFIX = "start";
var UPDATE_POSTFIX = "";
var END_POSTFIX = "end";
var eventAliases = [];
var addAlias = function(eventName, eventArgs) {
    eventAliases.push({
        name: eventName,
        args: eventArgs
    })
};
addAlias(TRANSFORM, {
    scale: true,
    deltaScale: true,
    rotation: true,
    deltaRotation: true,
    translation: true,
    deltaTranslation: true
});
addAlias(TRANSLATE, {
    translation: true,
    deltaTranslation: true
});
addAlias(PINCH, {
    scale: true,
    deltaScale: true
});
addAlias(ROTATE, {
    rotation: true,
    deltaRotation: true
});
var getVector = function(first, second) {
    return {
        x: second.pageX - first.pageX,
        y: -second.pageY + first.pageY,
        centerX: .5 * (second.pageX + first.pageX),
        centerY: .5 * (second.pageY + first.pageY)
    }
};
var getEventVector = function(e) {
    var pointers = e.pointers;
    return getVector(pointers[0], pointers[1])
};
var getDistance = function(vector) {
    return Math.sqrt(vector.x * vector.x + vector.y * vector.y)
};
var getScale = function(firstVector, secondVector) {
    return getDistance(firstVector) / getDistance(secondVector)
};
var getRotation = function(firstVector, secondVector) {
    var scalarProduct = firstVector.x * secondVector.x + firstVector.y * secondVector.y;
    var distanceProduct = getDistance(firstVector) * getDistance(secondVector);
    if (0 === distanceProduct) {
        return 0
    }
    var sign = mathSign(firstVector.x * secondVector.y - secondVector.x * firstVector.y);
    var angle = Math.acos(fitIntoRange(scalarProduct / distanceProduct, -1, 1));
    return sign * angle
};
var getTranslation = function(firstVector, secondVector) {
    return {
        x: firstVector.centerX - secondVector.centerX,
        y: firstVector.centerY - secondVector.centerY
    }
};
var TransformEmitter = Emitter.inherit({
    validatePointers: function(e) {
        return hasTouches(e) > 1
    },
    start: function(e) {
        this._accept(e);
        var startVector = getEventVector(e);
        this._startVector = startVector;
        this._prevVector = startVector;
        this._fireEventAliases(START_POSTFIX, e)
    },
    move: function(e) {
        var currentVector = getEventVector(e);
        var eventArgs = this._getEventArgs(currentVector);
        this._fireEventAliases(UPDATE_POSTFIX, e, eventArgs);
        this._prevVector = currentVector
    },
    end: function(e) {
        var eventArgs = this._getEventArgs(this._prevVector);
        this._fireEventAliases(END_POSTFIX, e, eventArgs)
    },
    _getEventArgs: function(vector) {
        return {
            scale: getScale(vector, this._startVector),
            deltaScale: getScale(vector, this._prevVector),
            rotation: getRotation(vector, this._startVector),
            deltaRotation: getRotation(vector, this._prevVector),
            translation: getTranslation(vector, this._startVector),
            deltaTranslation: getTranslation(vector, this._prevVector)
        }
    },
    _fireEventAliases: function(eventPostfix, originalEvent, eventArgs) {
        eventArgs = eventArgs || {};
        iteratorUtils.each(eventAliases, function(_, eventAlias) {
            var args = {};
            iteratorUtils.each(eventAlias.args, (function(name) {
                if (name in eventArgs) {
                    args[name] = eventArgs[name]
                }
            }));
            this._fireEvent(DX_PREFIX + eventAlias.name + eventPostfix, originalEvent, args)
        }.bind(this))
    }
});
var eventNames = eventAliases.reduce((result, eventAlias) => {
    [START_POSTFIX, UPDATE_POSTFIX, END_POSTFIX].forEach(eventPostfix => {
        result.push(DX_PREFIX + eventAlias.name + eventPostfix)
    });
    return result
}, []);
registerEmitter({
    emitter: TransformEmitter,
    events: eventNames
});
var exportNames = {};
iteratorUtils.each(eventNames, (function(_, eventName) {
    exportNames[eventName.substring(DX_PREFIX.length)] = eventName
}));
export var {
    transformstart: transformstart,
    transform: transform,
    transformend: transformend,
    translatestart: translatestart,
    translate: translate,
    translateend: translateend,
    zoomstart: zoomstart,
    zoom: zoom,
    zoomend: zoomend,
    pinchstart: pinchstart,
    pinch: pinch,
    pinchend: pinchend,
    rotatestart: rotatestart,
    rotate: rotate,
    rotateend: rotateend
} = exportNames;
