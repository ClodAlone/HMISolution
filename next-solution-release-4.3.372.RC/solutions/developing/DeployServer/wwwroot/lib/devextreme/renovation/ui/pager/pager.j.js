/**
 * DevExtreme (renovation/ui/pager/pager.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _grid_pager = require("../../component_wrapper/grid_pager");
var _pager = require("./pager");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var Pager = function(_GridPagerWrapper) {
    _inheritsLoose(Pager, _GridPagerWrapper);

    function Pager() {
        return _GridPagerWrapper.apply(this, arguments) || this
    }
    var _proto = Pager.prototype;
    _proto.getProps = function() {
        var props = _GridPagerWrapper.prototype.getProps.call(this);
        props.onKeyDown = this._wrapKeyDownHandler(props.onKeyDown);
        return props
    };
    _createClass(Pager, [{
        key: "_propsInfo",
        get: function() {
            return {
                twoWay: [
                    ["pageSize", "defaultPageSize", "pageSizeChange"],
                    ["pageIndex", "defaultPageIndex", "pageIndexChange"]
                ],
                allowNull: [],
                elements: [],
                templates: [],
                props: ["defaultPageSize", "pageSizeChange", "defaultPageIndex", "pageIndexChange", "gridCompatibility", "className", "showInfo", "infoText", "lightModeEnabled", "displayMode", "maxPagesCount", "pageCount", "pagesCountText", "visible", "hasKnownLastPage", "pagesNavigatorVisible", "showPageSizes", "pageSizes", "rtlEnabled", "showNavigationButtons", "totalCount", "onKeyDown", "pageSize", "pageIndex"]
            }
        }
    }, {
        key: "_viewComponent",
        get: function() {
            return _pager.Pager
        }
    }]);
    return Pager
}(_grid_pager.GridPagerWrapper);
exports.default = Pager;
(0, _component_registrator.default)("dxPager", Pager);
module.exports = exports.default;
module.exports.default = exports.default;
