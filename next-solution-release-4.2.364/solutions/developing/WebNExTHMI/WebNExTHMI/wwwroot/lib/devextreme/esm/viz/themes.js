/**
 * DevExtreme (esm/viz/themes.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../core/utils/extend";
import {
    each
} from "../core/utils/iterator";
import {
    normalizeEnum
} from "./core/utils";
import {
    current as getCurrentTheme
} from "../ui/themes";
import {
    isEmptyObject
} from "../core/utils/type";
import lightThemes from "./core/themes/generic.light";
import carmineThemes from "./core/themes/generic.carmine";
import darkThemes from "./core/themes/generic.dark";
import contrastThemes from "./core/themes/generic.contrast";
import darkMoonThemes from "./core/themes/generic.darkmoon";
import darkVioletThemes from "./core/themes/generic.darkviolet";
import greenMistThemes from "./core/themes/generic.greenmist";
import softBlueThemes from "./core/themes/generic.softblue";
import materialThemes from "./core/themes/material";
var themes = {};
var themesMapping = {};
var themesSchemeMapping = {};
var _extend = extend;
var _each = each;
var currentThemeName = null;
var defaultTheme;
var nextCacheUid = 0;
var widgetsCache = {};
export function getTheme(themeName) {
    var name = normalizeEnum(themeName);
    return themes[name] || themes[themesMapping[name] || currentTheme()]
}

function findThemeNameByName(name, scheme) {
    return themesMapping[name + "." + scheme] || themesSchemeMapping[name + "." + scheme] || themesMapping[name]
}

function findThemeNameByPlatform(platform, version, scheme) {
    return findThemeNameByName(platform + version, scheme) || findThemeNameByName(platform, scheme)
}
export function currentTheme(themeName, colorScheme) {
    if (!arguments.length) {
        return currentThemeName || findThemeNameByName(getCurrentTheme()) || defaultTheme
    }
    var scheme = normalizeEnum(colorScheme);
    currentThemeName = (themeName && themeName.platform ? findThemeNameByPlatform(normalizeEnum(themeName.platform), themeName.version, scheme) : findThemeNameByName(normalizeEnum(themeName), scheme)) || currentThemeName;
    return this
}

function getThemeInfo(themeName, splitter) {
    var k = themeName.indexOf(splitter);
    return k > 0 ? {
        name: themeName.substring(0, k),
        scheme: themeName.substring(k + 1)
    } : null
}

function registerThemeName(themeName, targetThemeName) {
    var themeInfo = getThemeInfo(themeName, ".") || {
        name: themeName
    };
    var name = themeInfo.name;
    var scheme = themeInfo.scheme;
    if (scheme) {
        themesMapping[name] = themesMapping[name] || targetThemeName;
        themesMapping[name + "." + scheme] = targetThemeName
    } else {
        themesMapping[name] = targetThemeName
    }
}
export function registerTheme(theme, baseThemeName) {
    var themeName = normalizeEnum(theme && theme.name);
    if (themeName) {
        theme.isDefault && (defaultTheme = themeName);
        registerThemeName(themeName, themeName);
        themes[themeName] = _extend(true, {}, getTheme(baseThemeName), patchTheme(theme))
    }
}
export function registerThemeSchemeAlias(from, to) {
    themesSchemeMapping[from] = to
}

function mergeScalar(target, field, source, sourceValue) {
    var _value = source ? source[field] : sourceValue;
    if (void 0 !== _value && void 0 === target[field]) {
        target[field] = _value
    }
}

function mergeObject(target, field, source, sourceValue) {
    var _value = source ? source[field] : sourceValue;
    if (void 0 !== _value) {
        target[field] = _extend(true, {}, _value, target[field])
    }
}

function patchTheme(theme) {
    theme = _extend(true, {
        loadingIndicator: {
            font: {}
        },
        export: {
            font: {}
        },
        legend: {
            font: {},
            border: {}
        },
        title: {
            font: {}
        },
        tooltip: {
            font: {}
        },
        "chart:common": {},
        "chart:common:axis": {
            grid: {},
            minorGrid: {},
            tick: {},
            minorTick: {},
            title: {
                font: {}
            },
            label: {
                font: {}
            }
        },
        "chart:common:annotation": {
            font: {},
            border: {}
        },
        chart: {
            commonSeriesSettings: {
                candlestick: {}
            }
        },
        pie: {},
        polar: {},
        gauge: {
            scale: {
                tick: {},
                minorTick: {},
                label: {
                    font: {}
                }
            }
        },
        barGauge: {},
        funnel: {},
        sankey: {},
        map: {
            background: {}
        },
        treeMap: {
            tile: {
                selectionStyle: {
                    border: {}
                }
            },
            group: {
                border: {},
                selectionStyle: {
                    border: {}
                },
                label: {
                    font: {}
                }
            }
        },
        rangeSelector: {
            scale: {
                tick: {},
                minorTick: {},
                label: {
                    font: {}
                }
            },
            chart: {}
        },
        sparkline: {},
        bullet: {}
    }, theme);
    mergeScalar(theme.loadingIndicator, "backgroundColor", theme);
    mergeScalar(theme.chart.commonSeriesSettings.candlestick, "innerColor", null, theme.backgroundColor);
    mergeScalar(theme.map.background, "color", null, theme.backgroundColor);
    mergeScalar(theme.title.font, "color", null, theme.primaryTitleColor);
    mergeObject(theme.title, "subtitle", null, theme.title);
    mergeScalar(theme.legend.font, "color", null, theme.secondaryTitleColor);
    mergeScalar(theme.legend.border, "color", null, theme.gridColor);
    patchAxes(theme);
    _each(["chart", "pie", "polar", "gauge", "barGauge", "map", "treeMap", "funnel", "rangeSelector", "sparkline", "bullet", "sankey"], (function(_, section) {
        mergeScalar(theme[section], "redrawOnResize", theme);
        mergeScalar(theme[section], "containerBackgroundColor", null, theme.backgroundColor);
        mergeObject(theme[section], "tooltip", theme);
        mergeObject(theme[section], "export", theme)
    }));
    _each(["chart", "pie", "polar", "gauge", "barGauge", "map", "treeMap", "funnel", "rangeSelector", "sankey"], (function(_, section) {
        mergeObject(theme[section], "loadingIndicator", theme);
        mergeObject(theme[section], "legend", theme);
        mergeObject(theme[section], "title", theme)
    }));
    _each(["chart", "pie", "polar"], (function(_, section) {
        mergeObject(theme, section, null, theme["chart:common"])
    }));
    _each(["chart", "polar"], (function(_, section) {
        theme[section] = theme[section] || {};
        mergeObject(theme[section], "commonAxisSettings", null, theme["chart:common:axis"])
    }));
    _each(["chart", "polar", "map", "pie"], (function(_, section) {
        theme[section] = theme[section] || {};
        mergeObject(theme[section], "commonAnnotationSettings", null, theme["chart:common:annotation"])
    }));
    mergeObject(theme.rangeSelector.chart, "commonSeriesSettings", theme.chart);
    mergeObject(theme.rangeSelector.chart, "dataPrepareSettings", theme.chart);
    mergeScalar(theme.treeMap.group.border, "color", null, theme.gridColor);
    mergeScalar(theme.treeMap.tile.selectionStyle.border, "color", null, theme.primaryTitleColor);
    mergeScalar(theme.treeMap.group.selectionStyle.border, "color", null, theme.primaryTitleColor);
    mergeScalar(theme.map.legend, "backgroundColor", theme);
    patchMapLayers(theme);
    return theme
}

function patchAxes(theme) {
    var commonAxisSettings = theme["chart:common:axis"];
    _each([commonAxisSettings.grid, commonAxisSettings.minorGrid], (function(_, obj) {
        mergeScalar(obj, "color", null, theme.gridColor)
    }));
    _each([commonAxisSettings, commonAxisSettings.tick, commonAxisSettings.minorTick, commonAxisSettings.label.font], (function(_, obj) {
        mergeScalar(obj, "color", null, theme.axisColor)
    }));
    mergeScalar(commonAxisSettings.title.font, "color", null, theme.secondaryTitleColor);
    mergeScalar(theme.gauge.scale.label.font, "color", null, theme.axisColor);
    mergeScalar(theme.gauge.scale.tick, "color", null, theme.backgroundColor);
    mergeScalar(theme.gauge.scale.minorTick, "color", null, theme.backgroundColor);
    mergeScalar(theme.rangeSelector.scale.label.font, "color", null, theme.axisColor)
}

function patchMapLayers(theme) {
    var map = theme.map;
    _each(["area", "line", "marker"], (function(_, section) {
        mergeObject(map, "layer:" + section, null, map.layer)
    }));
    _each(["dot", "bubble", "pie", "image"], (function(_, section) {
        mergeObject(map, "layer:marker:" + section, null, map["layer:marker"])
    }))
}
export function addCacheItem(target) {
    var cacheUid = ++nextCacheUid;
    target._cache = cacheUid;
    widgetsCache[cacheUid] = target
}
export function removeCacheItem(target) {
    delete widgetsCache[target._cache]
}
export function refreshTheme() {
    _each(widgetsCache, (function() {
        this.refresh()
    }));
    return this
}
if (isEmptyObject(themes) && isEmptyObject(themesMapping) && !defaultTheme) {
    [].concat(lightThemes, carmineThemes, darkThemes, contrastThemes, darkMoonThemes, darkVioletThemes, greenMistThemes, softBlueThemes, materialThemes).forEach(t => {
        registerTheme(t.theme, t.baseThemeName)
    })
}
