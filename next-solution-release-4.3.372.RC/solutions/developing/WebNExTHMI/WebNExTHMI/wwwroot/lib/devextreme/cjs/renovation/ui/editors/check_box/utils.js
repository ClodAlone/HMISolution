/**
 * DevExtreme (cjs/renovation/ui/editors/check_box/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getDefaultFontSize = getDefaultFontSize;
exports.getDefaultIconSize = getDefaultIconSize;
exports.getFontSizeByIconSize = getFontSizeByIconSize;
var _themes = require("../../../../ui/themes");
var defaultIconSizes = [
    [22, 16],
    [18, 16]
];
var defaultFontSizes = [
    [
        [12, 8],
        [20, 18]
    ],
    [
        [16, 10],
        [16, 14]
    ]
];

function getThemeType() {
    var theme = (0, _themes.current)();
    return {
        isMaterialTheme: (0, _themes.isMaterial)(theme),
        isCompactTheme: (0, _themes.isCompact)(theme)
    }
}

function getDefaultIconSize() {
    var _getThemeType = getThemeType(),
        isCompactTheme = _getThemeType.isCompactTheme,
        isMaterialTheme = _getThemeType.isMaterialTheme;
    return defaultIconSizes[+isMaterialTheme][+isCompactTheme]
}

function getDefaultFontSize(isChecked) {
    var _getThemeType2 = getThemeType(),
        isCompactTheme = _getThemeType2.isCompactTheme,
        isMaterialTheme = _getThemeType2.isMaterialTheme;
    return defaultFontSizes[+isChecked][+isMaterialTheme][+isCompactTheme]
}

function getFontSizeByIconSize(iconSize, isChecked) {
    var defaultFontSize = getDefaultFontSize(isChecked);
    var defaultIconSize = getDefaultIconSize();
    var fontToIconSizeRatio = defaultFontSize / defaultIconSize;
    return Math.ceil(fontToIconSizeRatio * iconSize)
}
