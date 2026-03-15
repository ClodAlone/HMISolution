/**
 * DevExtreme (esm/renovation/viz/common/renderers/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isDefined
} from "../../../../core/utils/type";
import domAdapter from "../../../../core/dom_adapter";
import {
    normalizeEnum
} from "../../../../viz/core/utils";
import {
    getWindow
} from "../../../../core/utils/window";
var KEY_FONT_SIZE = "font-size";
var DEFAULT_FONT_SIZE = 12;
var SHARPING_CORRECTION = .5;
var window = getWindow();
export var getNextDefsSvgId = (() => {
    var numDefsSvgElements = 1;
    return () => "DevExpress_".concat(numDefsSvgElements++)
})();
export var getFuncIri = (id, pathModified) => null !== id ? "url(".concat(pathModified ? window.location.href.split("#")[0] : "", "#").concat(id, ")") : id;
export var extend = (target, source) => {
    target = _extends({}, target, source);
    return target
};

function buildSegments(points, buildSimpleSegment, close) {
    var list = [];
    if (Array.isArray(points[0])) {
        for (var i = 0, ii = points.length; i < ii; ++i) {
            buildSimpleSegment(points[i], close, list)
        }
    } else {
        buildSimpleSegment(points, close, list)
    }
    return list
}

function buildSimpleLineSegment(points, close, list) {
    var i = 0;
    var k0 = list.length;
    var k = k0;
    var ii = (points || []).length;
    if (ii) {
        if (void 0 !== points[0].x) {
            var arrPoints = points;
            for (; i < ii;) {
                list[k++] = ["L", arrPoints[i].x, arrPoints[i++].y]
            }
        } else {
            var _arrPoints = points;
            for (; i < ii;) {
                list[k++] = ["L", _arrPoints[i++], _arrPoints[i++]]
            }
        }
        list[k0][0] = "M"
    } else {
        list[k] = ["M", 0, 0]
    }
    close && list.push(["Z"]);
    return list
}

function buildSimpleCurveSegment(points, close, list) {
    var k = list.length;
    var ii = (points || []).length;
    if (ii) {
        if (void 0 !== points[0]) {
            var arrPoints = points;
            list[k++] = ["M", arrPoints[0].x, arrPoints[0].y];
            for (var i = 1; i < ii;) {
                list[k++] = ["C", arrPoints[i].x, arrPoints[i++].y, arrPoints[i].x, arrPoints[i++].y, arrPoints[i].x, arrPoints[i++].y]
            }
        } else {
            var _arrPoints2 = points;
            list[k++] = ["M", _arrPoints2[0], _arrPoints2[1]];
            for (var _i = 2; _i < ii;) {
                list[k++] = ["C", _arrPoints2[_i++], _arrPoints2[_i++], _arrPoints2[_i++], _arrPoints2[_i++], _arrPoints2[_i++], _arrPoints2[_i++]]
            }
        }
    } else {
        list[k] = ["M", 0, 0]
    }
    close && list.push(["Z"]);
    return list
}

function buildLineSegments(points, close) {
    return buildSegments(points, buildSimpleLineSegment, close)
}

function buildCurveSegments(points, close) {
    return buildSegments(points, buildSimpleCurveSegment, close)
}
export var buildPathSegments = (points, type) => {
    var list = [
        ["M", 0, 0]
    ];
    if ("line" === type) {
        list = buildLineSegments(points, false)
    } else if ("area" === type) {
        list = buildLineSegments(points, true)
    } else if ("bezier" === type) {
        list = buildCurveSegments(points, false)
    } else if ("bezierarea" === type) {
        list = buildCurveSegments(points, true)
    }
    return list
};
export var combinePathParam = segments => {
    var d = [];
    var ii = segments.length;
    for (var i = 0; i < ii; ++i) {
        var segment = segments[i];
        for (var j = 0, jj = segment.length; j < jj; ++j) {
            d.push(segment[j])
        }
    }
    return d.join(" ")
};

function prepareConstSegment(constSeg, type) {
    var x = constSeg[constSeg.length - 2];
    var y = constSeg[constSeg.length - 1];
    if ("line" === type || "area" === type) {
        constSeg[0] = "L"
    } else if ("bezier" === type || "bezierarea" === type) {
        constSeg[0] = "C";
        constSeg[1] = x;
        constSeg[3] = x;
        constSeg[5] = x;
        constSeg[2] = y;
        constSeg[4] = y;
        constSeg[6] = y
    }
}

function makeEqualLineSegments(short, long, type) {
    var constSeg = [...short[short.length - 1]];
    var i = short.length;
    prepareConstSegment(constSeg, type);
    for (; i < long.length; i++) {
        short[i] = [...constSeg]
    }
}

function makeEqualAreaSegments(short, long, type) {
    var shortLength = short.length;
    var longLength = long.length;
    if ((shortLength - 1) % 2 === 0 && (longLength - 1) % 2 === 0) {
        var i = (shortLength - 1) / 2 - 1;
        var head = short.slice(0, i + 1);
        var constsSeg1 = [...head[head.length - 1]];
        var constsSeg2 = [...short.slice(i + 1)[0]];
        prepareConstSegment(constsSeg1, type);
        prepareConstSegment(constsSeg2, type);
        for (var j = i; j < (longLength - 1) / 2 - 1; j++) {
            short.splice(j + 1, 0, constsSeg1);
            short.splice(j + 3, 0, constsSeg2)
        }
    }
}
export var compensateSegments = (oldSegments, newSegments, type) => {
    var oldLength = oldSegments.length;
    var newLength = newSegments.length;
    var originalNewSegments = [];
    var makeEqualSegments = type.includes("area") ? makeEqualAreaSegments : makeEqualLineSegments;
    if (0 === oldLength) {
        for (var i = 0; i < newLength; i++) {
            oldSegments.push([...newSegments[i]])
        }
    } else if (oldLength < newLength) {
        makeEqualSegments(oldSegments, newSegments, type)
    } else if (oldLength > newLength) {
        originalNewSegments = [...newSegments];
        makeEqualSegments(newSegments, oldSegments, type)
    }
    return originalNewSegments
};
export var getElementBBox = element => {
    var bBox = new SVGRect(0, 0, 0, 0);
    if (void 0 !== element) {
        bBox = element.getBBox()
    } else if (void 0 !== element) {
        var el = element;
        bBox = new SVGRect(0, 0, el.offsetWidth, el.offsetHeight)
    }
    return bBox
};

function maxLengthFontSize(fontSize1, fontSize2) {
    var height1 = null !== fontSize1 && void 0 !== fontSize1 ? fontSize1 : DEFAULT_FONT_SIZE;
    var height2 = null !== fontSize2 && void 0 !== fontSize2 ? fontSize2 : DEFAULT_FONT_SIZE;
    return height1 > height2 ? height1 : height2
}

function orderHtmlTree(list, line, node, parentStyle, parentClassName) {
    var realStyle = node.style;
    if (isDefined(node.wholeText)) {
        list.push({
            value: node.wholeText,
            style: parentStyle,
            className: parentClassName,
            line: line,
            height: parseFloat(parentStyle.fontSize) || 0
        })
    } else if ("BR" === node.tagName) {
        ++line
    } else if (domAdapter.isElementNode(node)) {
        var style = extend({}, parentStyle);
        switch (node.tagName) {
            case "B":
            case "STRONG":
                style.fontWeight = "bold";
                break;
            case "I":
            case "EM":
                style.fontStyle = "italic";
                break;
            case "U":
                style.textDecoration = "underline"
        }
        realStyle.color && (style.fill = realStyle.color);
        realStyle.fontSize && (style.fontSize = realStyle.fontSize);
        realStyle.fontStyle && (style.fontStyle = realStyle.fontStyle);
        realStyle.fontWeight && (style.fontWeight = realStyle.fontWeight);
        realStyle.textDecoration && (style.textDecoration = realStyle.textDecoration);
        for (var i = 0, nodes = node.childNodes, ii = nodes.length; i < ii; ++i) {
            line = orderHtmlTree(list, line, nodes[i], style, node.className || parentClassName)
        }
    }
    return line
}

function adjustLineHeights(items) {
    var currentItem = items[0];
    for (var i = 1, ii = items.length; i < ii; ++i) {
        var item = items[i];
        if (item.line === currentItem.line) {
            currentItem.height = maxLengthFontSize(currentItem.height, item.height);
            currentItem.inherits = !!currentItem.inherits || 0 === item.height;
            item.height = NaN
        } else {
            currentItem = item
        }
    }
}
export var removeExtraAttrs = html => {
    var findStyleAndClassAttrs = /(style|class)\s*=\s*(["'])(?:(?!\2).)*\2\s?/gi;
    return html.replace(/(?:(<[a-z0-9]+\s*))([\s\S]*?)(>|\/>)/gi, (_, p1, p2, p3) => {
        var _p2$match, _p;
        p2 = (null !== (_p2$match = null === (_p = p2) || void 0 === _p ? void 0 : _p.match(findStyleAndClassAttrs)) && void 0 !== _p2$match ? _p2$match : []).map(str => str).join(" ");
        return p1 + p2 + p3
    })
};
export var parseHTML = text => {
    var items = [];
    var div = domAdapter.createElement("div");
    div.innerHTML = text.replace(/\r/g, "").replace(/\n/g, "<br/>");
    orderHtmlTree(items, 0, div, {}, "");
    adjustLineHeights(items);
    return items
};
export var parseMultiline = text => {
    var texts = text.replace(/\r/g, "").split(/\n/g);
    var items = [];
    for (var i = 0; i < texts.length; i++) {
        items.push({
            value: texts[i].trim(),
            height: 0,
            line: i
        })
    }
    return items
};
export var getTextWidth = text => {
    var {
        tspan: tspan,
        value: value
    } = text;
    return value.length && tspan ? tspan.getSubStringLength(0, value.length) : 0
};
export var setTextNodeAttribute = (item, name, value) => {
    var _item$tspan, _item$stroke;
    null === (_item$tspan = item.tspan) || void 0 === _item$tspan ? void 0 : _item$tspan.setAttribute(name, value);
    null === (_item$stroke = item.stroke) || void 0 === _item$stroke ? void 0 : _item$stroke.setAttribute(name, value)
};
export var getItemLineHeight = (item, defaultValue) => item.inherits ? maxLengthFontSize(item.height, defaultValue) : Number(item.height) || defaultValue;
export var getLineHeight = styles => styles && !Number.isNaN(parseFloat(styles[KEY_FONT_SIZE])) ? parseFloat(styles[KEY_FONT_SIZE]) : DEFAULT_FONT_SIZE;
export var textsAreEqual = (newItems, renderedItems) => {
    if (!renderedItems || renderedItems.length !== newItems.length) {
        return false
    }
    return renderedItems.every((item, index) => item.value === newItems[index].value)
};
export var convertAlignmentToAnchor = function(value) {
    var rtl = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : false;
    return value ? {
        left: rtl ? "end" : "start",
        center: "middle",
        right: rtl ? "start" : "end"
    } [value] : void 0
};

function getTransformation(props, x, y) {
    var {
        rotate: rotate,
        rotateX: rotateX,
        rotateY: rotateY,
        scaleX: scaleX,
        scaleY: scaleY,
        sharp: sharp,
        sharpDirection: sharpDirection,
        strokeWidth: strokeWidth,
        translateX: translateX,
        translateY: translateY
    } = props;
    var transformations = [];
    var transDir = "backward" === sharpDirection ? -1 : 1;
    var strokeOdd = (null !== strokeWidth && void 0 !== strokeWidth ? strokeWidth : 0) % 2;
    var correctionX = strokeOdd && ("h" === sharp || true === sharp) ? SHARPING_CORRECTION * transDir : 0;
    var correctionY = strokeOdd && ("v" === sharp || true === sharp) ? SHARPING_CORRECTION * transDir : 0;
    if (translateX || translateY || correctionX || correctionY) {
        transformations.push("translate(".concat((null !== translateX && void 0 !== translateX ? translateX : 0) + correctionX, ",").concat((null !== translateY && void 0 !== translateY ? translateY : 0) + correctionY, ")"))
    }
    if (rotate) {
        var _ref, _ref2;
        transformations.push("rotate(".concat(rotate, ",").concat(null !== (_ref = Number(rotateX) || x) && void 0 !== _ref ? _ref : 0, ",").concat(null !== (_ref2 = Number(rotateY) || y) && void 0 !== _ref2 ? _ref2 : 0, ")"))
    }
    var scaleXDefined = isDefined(scaleX);
    var scaleYDefined = isDefined(scaleY);
    if (scaleXDefined || scaleYDefined) {
        transformations.push("scale(".concat(scaleXDefined ? scaleX : 1, ",").concat(scaleYDefined ? scaleY : 1, ")"))
    }
    return transformations.length ? transformations.join(" ") : void 0
}

function getDashStyle(props) {
    var {
        dashStyle: dashStyle,
        strokeWidth: strokeWidth
    } = props;
    if (!dashStyle || "none" === dashStyle || "solid" === dashStyle) {
        return
    }
    var sw = Number(strokeWidth) || 1;
    var value = normalizeEnum(dashStyle);
    var dashArray = [];
    dashArray = value.replace(/longdash/g, "8,3,").replace(/dash/g, "4,3,").replace(/dot/g, "1,3,").replace(/,$/, "").split(",");
    var i = dashArray.length;
    while (i--) {
        dashArray[i] = parseInt(dashArray[i], 10) * sw
    }
    return dashArray.join(",")
}
export var getGraphicExtraProps = (props, x, y) => ({
    transform: getTransformation(props, x, y),
    "stroke-dasharray": getDashStyle(props)
});
