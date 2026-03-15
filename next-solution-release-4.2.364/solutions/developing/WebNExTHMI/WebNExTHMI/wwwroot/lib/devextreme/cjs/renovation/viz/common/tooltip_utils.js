/**
 * DevExtreme (cjs/renovation/viz/common/tooltip_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getCanvas = getCanvas;
exports.getCloudAngle = getCloudAngle;
exports.getCloudPoints = getCloudPoints;
exports.isTextEmpty = isTextEmpty;
exports.prepareData = prepareData;
exports.recalculateCoordinates = recalculateCoordinates;
var _type = require("../../../core/utils/type");
var _dom_adapter = _interopRequireDefault(require("../../../core/dom_adapter"));
var _window = require("../../../core/utils/window");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var PI = Math.PI,
    asin = Math.asin,
    ceil = Math.ceil,
    cos = Math.cos,
    floor = Math.floor,
    max = Math.max,
    min = Math.min,
    round = Math.round,
    sin = Math.sin;
var buildPath = function() {
    for (var _len = arguments.length, points = new Array(_len), _key = 0; _key < _len; _key++) {
        points[_key] = arguments[_key]
    }
    return points.join("")
};

function getArc(cornerRadius, xDirection, yDirection) {
    return "a ".concat(cornerRadius, " ").concat(cornerRadius, " 0 0 1 ").concat(xDirection * cornerRadius, " ").concat(yDirection * cornerRadius)
}

function getAbsoluteArc(cornerRadius, x, y) {
    return "A ".concat(cornerRadius, " ").concat(cornerRadius, " 0 0 1 ").concat(x, " ").concat(y)
}

function rotateSize(_ref, angle) {
    var height = _ref.height,
        width = _ref.width;
    if (angle % 90 === 0 && angle % 180 !== 0) {
        return {
            width: height,
            height: width
        }
    }
    return {
        width: width,
        height: height
    }
}

function rotateX(_ref2, angle) {
    var anchorX = _ref2.anchorX,
        anchorY = _ref2.anchorY,
        x = _ref2.x,
        y = _ref2.y;
    return (anchorX - x) * round(cos(angle)) + (anchorY - y) * round(sin(angle)) + x
}

function rotateY(_ref3, angle) {
    var anchorX = _ref3.anchorX,
        anchorY = _ref3.anchorY,
        x = _ref3.x,
        y = _ref3.y;
    return -(anchorX - x) * round(sin(angle)) + (anchorY - y) * round(cos(angle)) + y
}

function getCloudPoints(size, coordinates, rotationAngle, options, bounded) {
    var x = coordinates.x,
        y = coordinates.y;
    var radRotationAngle = rotationAngle * PI / 180;
    var _rotateSize = rotateSize(size, rotationAngle),
        height = _rotateSize.height,
        width = _rotateSize.width;
    var anchorX = rotateX(coordinates, radRotationAngle);
    var anchorY = rotateY(coordinates, radRotationAngle);
    var halfArrowWidth = options.arrowWidth / 2;
    var halfWidth = width / 2;
    var halfHeight = height / 2;
    var xr = Math.ceil(x + halfWidth);
    var xl = Math.floor(x - halfWidth);
    var yt = Math.floor(y - halfHeight);
    var yb = Math.ceil(y + halfHeight);
    var leftTopCorner = [xl, yt];
    var rightTopCorner = [xr, yt];
    var rightBottomCorner = [xr, yb];
    var leftBottomCorner = [xl, yb];
    var getCoordinate = function(cur, side1, side2) {
        if (cur <= side1) {
            return side1
        }
        if (cur >= side2) {
            return side2
        }
        return cur
    };
    var arrowX = getCoordinate(anchorX, xl, xr);
    var arrowY = getCoordinate(anchorY, yt, yb);
    var arrowBaseBottom = min(arrowY + halfArrowWidth, yb);
    var arrowBaseTop = max(arrowY - halfArrowWidth, yt);
    var arrowBaseLeft = max(arrowX - halfArrowWidth, xl);
    var cornerRadius = Math.min(halfWidth, halfHeight, options.cornerRadius);
    var points = "";
    var arrowArc = "";
    leftTopCorner[1] += cornerRadius;
    rightTopCorner[0] -= cornerRadius;
    rightBottomCorner[1] -= cornerRadius;
    leftBottomCorner[0] += cornerRadius;
    if (!bounded || xl <= anchorX && anchorX <= xr && yt <= anchorY && anchorY <= yb) {
        points = buildPath(leftTopCorner, getArc(cornerRadius, 1, -1), "L", rightTopCorner, getArc(cornerRadius, 1, 1), "L", rightBottomCorner, getArc(cornerRadius, -1, 1), "L", leftBottomCorner, getArc(cornerRadius, -1, -1))
    } else if (anchorX > xr && anchorY < yt) {
        var arrowAngle = options.arrowWidth / cornerRadius || 0;
        var angle = PI / 4 + arrowAngle / 2;
        var endAngle = PI / 4 - arrowAngle / 2;
        var arrowEndPointX = rightTopCorner[0] + cos(endAngle) * cornerRadius;
        var arrowEndPointY = rightTopCorner[1] + (1 - sin(endAngle)) * cornerRadius;
        if (Math.abs(angle) > PI / 2) {
            arrowArc = buildPath("L", [arrowBaseLeft, yt, anchorX, anchorY, xr, arrowBaseBottom])
        } else {
            arrowArc = buildPath("L", rightTopCorner, getArc(cornerRadius, cos(angle), 1 - sin(angle)), "L", [anchorX, anchorY, arrowEndPointX, arrowEndPointY], getAbsoluteArc(cornerRadius, rightTopCorner[0] + cornerRadius, rightTopCorner[1] + cornerRadius))
        }
        points = buildPath(leftTopCorner, getArc(cornerRadius, 1, -1), arrowArc, "L", rightBottomCorner, getArc(cornerRadius, -1, 1), "L", leftBottomCorner, getArc(cornerRadius, -1, -1))
    } else if (anchorX > xr && anchorY >= yt && anchorY <= yb) {
        if (arrowBaseTop >= rightTopCorner[1] + cornerRadius && arrowBaseBottom <= rightBottomCorner[1]) {
            arrowArc = buildPath(getArc(cornerRadius, 1, 1), "L", [xr, arrowBaseTop, anchorX, anchorY, xr, arrowBaseBottom], "L", rightBottomCorner, getArc(cornerRadius, -1, 1))
        } else if (arrowBaseTop < rightTopCorner[1] + cornerRadius && arrowBaseBottom >= rightTopCorner[1] + cornerRadius && arrowBaseBottom <= rightBottomCorner[1]) {
            var arrowWidthRest = rightTopCorner[1] + cornerRadius - arrowBaseTop;
            var _angle = arrowWidthRest / cornerRadius;
            var arrowBaseTopX = rightTopCorner[0] + cos(_angle) * cornerRadius;
            var arrowBaseTopY = rightTopCorner[1] + (1 - sin(_angle)) * cornerRadius;
            arrowArc = buildPath(getArc(cornerRadius, cos(_angle), 1 - sin(_angle)), "L", [arrowBaseTopX, arrowBaseTopY, anchorX, anchorY, xr, arrowBaseBottom], "L", rightBottomCorner, getArc(cornerRadius, -1, 1))
        } else if (arrowBaseTop < rightTopCorner[1] + cornerRadius && arrowBaseBottom < rightTopCorner[1] + cornerRadius) {
            var _arrowWidthRest = rightTopCorner[1] + cornerRadius - arrowBaseTop;
            var _arrowAngle = _arrowWidthRest / cornerRadius;
            var _angle2 = _arrowAngle;
            var _arrowBaseTopX = rightTopCorner[0] + cos(_angle2) * cornerRadius;
            var _arrowBaseTopY = rightTopCorner[1] + (1 - sin(_angle2)) * cornerRadius;
            var bottomAngle = Math.sin((rightTopCorner[1] + cornerRadius - arrowBaseBottom) / cornerRadius);
            var arrowBaseBottomX = rightTopCorner[0] + cornerRadius * cos(bottomAngle);
            var arrowBaseBottomY = rightTopCorner[1] + cornerRadius * (1 - sin(bottomAngle));
            arrowArc = buildPath(getArc(cornerRadius, cos(_angle2), 1 - sin(_angle2)), "L", [_arrowBaseTopX, _arrowBaseTopY, anchorX, anchorY, arrowBaseBottomX, arrowBaseBottomY], getAbsoluteArc(cornerRadius, rightTopCorner[0] + cornerRadius, rightTopCorner[1] + cornerRadius), "L", rightBottomCorner, getArc(cornerRadius, -1, 1))
        } else if (arrowBaseTop <= rightTopCorner[1] + cornerRadius && arrowBaseBottom >= rightBottomCorner[1]) {
            var topAngle = asin((rightTopCorner[1] + cornerRadius - arrowBaseTop) / cornerRadius);
            var _arrowBaseTopX2 = rightTopCorner[0] + cornerRadius * cos(topAngle);
            var _arrowBaseTopY2 = rightTopCorner[1] + cornerRadius * (1 - sin(topAngle));
            var _bottomAngle = asin((arrowBaseBottom - rightBottomCorner[1]) / cornerRadius);
            var _arrowBaseBottomX = rightBottomCorner[0] + cornerRadius * (cos(_bottomAngle) - 1);
            var _arrowBaseBottomY = rightBottomCorner[1] + cornerRadius * sin(_bottomAngle);
            arrowArc = buildPath(getArc(cornerRadius, cos(topAngle), 1 - sin(topAngle)), "L", [_arrowBaseTopX2, _arrowBaseTopY2, anchorX, anchorY, _arrowBaseBottomX, _arrowBaseBottomY], getAbsoluteArc(cornerRadius, rightBottomCorner[0] - cornerRadius, rightBottomCorner[1] + cornerRadius))
        } else if (arrowBaseTop > rightTopCorner[1] + cornerRadius && arrowBaseTop <= rightBottomCorner[1] && arrowBaseBottom > rightBottomCorner[1]) {
            var _bottomAngle2 = asin((arrowBaseBottom - rightBottomCorner[1]) / cornerRadius);
            var _arrowBaseBottomX2 = rightBottomCorner[0] + cornerRadius * (cos(_bottomAngle2) - 1);
            var _arrowBaseBottomY2 = rightBottomCorner[1] + cornerRadius * sin(_bottomAngle2);
            arrowArc = buildPath(getArc(cornerRadius, 1, 1), "L", [xr, arrowBaseTop, anchorX, anchorY, _arrowBaseBottomX2, _arrowBaseBottomY2], getAbsoluteArc(cornerRadius, rightBottomCorner[0] - cornerRadius, rightBottomCorner[1] + cornerRadius))
        } else if (arrowBaseTop > rightTopCorner[1] + cornerRadius && arrowBaseBottom > rightBottomCorner[1]) {
            var _bottomAngle3 = asin((arrowBaseBottom - rightBottomCorner[1]) / cornerRadius);
            var _arrowBaseBottomX3 = rightBottomCorner[0] + cornerRadius * (cos(_bottomAngle3) - 1);
            var _arrowBaseBottomY3 = rightBottomCorner[1] + cornerRadius * sin(_bottomAngle3);
            var _topAngle = asin((arrowBaseTop - rightBottomCorner[1]) / cornerRadius);
            var _arrowBaseTopX3 = rightBottomCorner[0] + cornerRadius * (cos(_topAngle) - 1);
            var _arrowBaseTopY3 = rightBottomCorner[1] + cornerRadius * sin(_topAngle);
            arrowArc = buildPath(getArc(cornerRadius, 1, 1), "L", rightBottomCorner, getArc(cornerRadius, cos(_topAngle) - 1, sin(_topAngle)), "L", [_arrowBaseTopX3, _arrowBaseTopY3, anchorX, anchorY, _arrowBaseBottomX3, _arrowBaseBottomY3], getAbsoluteArc(cornerRadius, rightBottomCorner[0] - cornerRadius, rightBottomCorner[1] + cornerRadius))
        }
        points = buildPath(leftTopCorner, getArc(cornerRadius, 1, -1), "L", rightTopCorner, arrowArc, "L", leftBottomCorner, getArc(cornerRadius, -1, -1))
    }
    return buildPath("M", points, "Z")
}

function getCanvas(container) {
    var _ref4, _getWindow, _ref5, _getWindow2;
    var containerBox = container.getBoundingClientRect();
    var html = _dom_adapter.default.getDocumentElement();
    var body = _dom_adapter.default.getBody();
    var left = null !== (_ref4 = Number(null === (_getWindow = (0, _window.getWindow)()) || void 0 === _getWindow ? void 0 : _getWindow.pageXOffset) || html.scrollLeft) && void 0 !== _ref4 ? _ref4 : 0;
    var top = null !== (_ref5 = Number(null === (_getWindow2 = (0, _window.getWindow)()) || void 0 === _getWindow2 ? void 0 : _getWindow2.pageYOffset) || html.scrollTop) && void 0 !== _ref5 ? _ref5 : 0;
    var box = {
        left: left,
        top: top,
        width: max(body.clientWidth, html.clientWidth) + left,
        height: max(body.scrollHeight, html.scrollHeight, body.offsetHeight, html.offsetHeight, body.clientHeight, html.clientHeight),
        right: 0,
        bottom: 0
    };
    if (container !== _dom_adapter.default.getBody()) {
        left = max(box.left, box.left + containerBox.left);
        top = max(box.top, box.top + containerBox.top);
        box.width = min(containerBox.width, box.width) + left + box.left;
        box.height = min(containerBox.height, box.height) + top + box.top;
        box.left = left;
        box.top = top
    }
    return box
}

function recalculateCoordinates(_ref6) {
    var anchorX = _ref6.anchorX,
        anchorY = _ref6.anchorY,
        arrowLength = _ref6.arrowLength,
        canvas = _ref6.canvas,
        offset = _ref6.offset,
        size = _ref6.size;
    var bounds_xl = canvas.left,
        bounds_xr = canvas.width - canvas.right,
        bounds_width = canvas.width - canvas.right - canvas.left,
        bounds_yt = canvas.top,
        bounds_yb = canvas.height - canvas.bottom,
        bounds_height = canvas.height - canvas.bottom - canvas.top;
    if (anchorX < bounds_xl || bounds_xr < anchorX || anchorY < bounds_yt || bounds_yb < anchorY) {
        return false
    }
    var x = Number.NaN;
    var y = Number.NaN;
    var correctedAnchorY = anchorY;
    if (bounds_width < size.width) {
        x = round(bounds_xl + bounds_width / 2)
    } else {
        x = min(max(anchorX, ceil(bounds_xl + size.width / 2)), floor(bounds_xr - size.width / 2))
    }
    var halfHeightWithArrow = arrowLength + size.height / 2 + offset;
    var yTop = anchorY - halfHeightWithArrow;
    var yBottom = anchorY + halfHeightWithArrow;
    if (bounds_height < size.height + arrowLength) {
        y = round(bounds_yt + size.height / 2)
    } else if (yTop - size.height / 2 < bounds_yt) {
        if (yBottom + size.height / 2 < bounds_yb) {
            y = yBottom;
            correctedAnchorY += offset
        } else {
            y = round(bounds_yt + size.height / 2)
        }
    } else {
        y = yTop;
        correctedAnchorY -= offset
    }
    return {
        x: x,
        y: y,
        anchorX: anchorX,
        anchorY: correctedAnchorY
    }
}

function getCloudAngle(_ref7, _ref8) {
    var height = _ref7.height,
        width = _ref7.width;
    var anchorX = _ref8.anchorX,
        anchorY = _ref8.anchorY,
        x = _ref8.x,
        y = _ref8.y;
    var halfWidth = width / 2;
    var halfHeight = height / 2;
    var xr = Math.ceil(x + halfWidth);
    var xl = Math.floor(x - halfWidth);
    var yt = Math.floor(y - halfHeight);
    var yb = Math.ceil(y + halfHeight);
    var angle = 0;
    if (anchorX < xl && anchorY < yt || anchorX >= xl && anchorX <= xr && anchorY < yt) {
        angle = 270
    }
    if (anchorX > xr && anchorY > yb || anchorX >= xl && anchorX <= xr && anchorY > yb) {
        angle = 90
    }
    if (anchorX < xl && anchorY > yb || anchorX < xl && anchorY >= yt && anchorY <= yb) {
        angle = 180
    }
    return angle
}

function prepareData(data, color, border, font, customizeTooltip) {
    var _customize$color, _customize$borderColo, _customize$fontColor;
    var customize = {};
    if ((0, _type.isFunction)(customizeTooltip)) {
        customize = customizeTooltip.call(data, data);
        customize = (0, _type.isPlainObject)(customize) ? customize : {};
        if ("text" in customize) {
            customize.text = (0, _type.isDefined)(customize.text) ? String(customize.text) : ""
        }
        if ("html" in customize) {
            customize.html = (0, _type.isDefined)(customize.html) ? String(customize.html) : ""
        }
    }
    if (!("text" in customize) && !("html" in customize)) {
        var _data$valueText, _data$description;
        customize.text = (null !== (_data$valueText = data.valueText) && void 0 !== _data$valueText ? _data$valueText : "") || (null !== (_data$description = data.description) && void 0 !== _data$description ? _data$description : "")
    }
    customize.color = (null !== (_customize$color = customize.color) && void 0 !== _customize$color ? _customize$color : "") || color;
    customize.borderColor = (null !== (_customize$borderColo = customize.borderColor) && void 0 !== _customize$borderColo ? _customize$borderColo : "") || border.color;
    customize.fontColor = (null !== (_customize$fontColor = customize.fontColor) && void 0 !== _customize$fontColor ? _customize$fontColor : "") || font.color;
    return customize
}

function isTextEmpty(_ref9) {
    var html = _ref9.html,
        text = _ref9.text;
    return null === text || "" === text || "" === html || null === html
}
