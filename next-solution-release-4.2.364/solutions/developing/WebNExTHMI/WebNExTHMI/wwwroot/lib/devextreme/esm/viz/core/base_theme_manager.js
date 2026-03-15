/**
 * DevExtreme (esm/viz/core/base_theme_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Class from "../../core/class";
import {
    extend
} from "../../core/utils/extend";
import {
    isString as _isString
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    createPalette as getPalette,
    getDiscretePalette,
    getGradientPalette,
    getAccentColor as accentColor
} from "../palette";
import {
    parseScalar as _parseScalar
} from "./utils";
import {
    getTheme,
    addCacheItem,
    removeCacheItem
} from "../themes";
var _getTheme = getTheme;
var _addCacheItem = addCacheItem;
var _removeCacheItem = removeCacheItem;
var _extend = extend;
var _each = each;

function getThemePart(theme, path) {
    var _theme = theme;
    path && _each(path.split("."), (function(_, pathItem) {
        return _theme = _theme[pathItem]
    }));
    return _theme
}
export var BaseThemeManager = Class.inherit({
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
        this._theme = _extend(true, {}, theme, _isString(current) ? {} : current);
        this._initializeTheme();
        if (_parseScalar(this._rtl, this._theme.rtlEnabled)) {
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
        return getPalette(palette, options, this._defaultPalette)
    },
    createDiscretePalette: function(palette, count) {
        return getDiscretePalette(palette, count, this._defaultPalette)
    },
    createGradientPalette: function(palette) {
        return getGradientPalette(palette, this._defaultPalette)
    },
    getAccentColor: function(palette) {
        return accentColor(palette, this._defaultPalette)
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
