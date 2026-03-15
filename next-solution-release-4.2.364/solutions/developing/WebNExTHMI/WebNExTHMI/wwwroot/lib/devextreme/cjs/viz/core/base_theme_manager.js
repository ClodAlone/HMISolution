/**
 * DevExtreme (cjs/viz/core/base_theme_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.BaseThemeManager = void 0;
var _class = _interopRequireDefault(require("../../core/class"));
var _extend2 = require("../../core/utils/extend");
var _type = require("../../core/utils/type");
var _iterator = require("../../core/utils/iterator");
var _palette = require("../palette");
var _utils = require("./utils");
var _themes = require("../themes");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _getTheme = _themes.getTheme;
var _addCacheItem = _themes.addCacheItem;
var _removeCacheItem = _themes.removeCacheItem;
var _extend = _extend2.extend;
var _each = _iterator.each;

function getThemePart(theme, path) {
    var _theme = theme;
    path && _each(path.split("."), (function(_, pathItem) {
        return _theme = _theme[pathItem]
    }));
    return _theme
}
var BaseThemeManager = _class.default.inherit({
    ctor: function(options) {
        this._themeSection = options.themeSection;
        this._fontFields = options.fontFields || [];
        _addCacheItem(this)
    },
    dispose: function() {
        _removeCacheItem(this);
        this._callback = this._theme = this._font = null;
        return this
    },
    setCallback: function(callback) {
        this._callback = callback;
        return this
    },
    setTheme: function(theme, rtl) {
        this._current = theme;
        this._rtl = rtl;
        return this.refresh()
    },
    refresh: function() {
        var current = this._current || {};
        var theme = _getTheme(current.name || current);
        this._themeName = theme.name;
        this._defaultPalette = theme.defaultPalette;
        this._font = _extend({}, theme.font, current.font);
        this._themeSection && _each(this._themeSection.split("."), (function(_, path) {
            theme = _extend(true, {}, theme[path])
        }));
        this._theme = _extend(true, {}, theme, (0, _type.isString)(current) ? {} : current);
        this._initializeTheme();
        if ((0, _utils.parseScalar)(this._rtl, this._theme.rtlEnabled)) {
            _extend(true, this._theme, this._theme._rtl)
        }
        this._callback();
        return this
    },
    theme: function(path) {
        return getThemePart(this._theme, path)
    },
    themeName: function() {
        return this._themeName
    },
    createPalette: function(palette, options) {
        return (0, _palette.createPalette)(palette, options, this._defaultPalette)
    },
    createDiscretePalette: function(palette, count) {
        return (0, _palette.getDiscretePalette)(palette, count, this._defaultPalette)
    },
    createGradientPalette: function(palette) {
        return (0, _palette.getGradientPalette)(palette, this._defaultPalette)
    },
    getAccentColor: function(palette) {
        return (0, _palette.getAccentColor)(palette, this._defaultPalette)
    },
    _initializeTheme: function() {
        var that = this;
        _each(that._fontFields || [], (function(_, path) {
            that._initializeFont(getThemePart(that._theme, path))
        }))
    },
    _initializeFont: function(font) {
        _extend(font, this._font, _extend({}, font))
    }
});
exports.BaseThemeManager = BaseThemeManager;
