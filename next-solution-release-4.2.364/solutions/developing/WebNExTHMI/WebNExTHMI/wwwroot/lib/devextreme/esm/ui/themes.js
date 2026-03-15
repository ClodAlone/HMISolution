/**
 * DevExtreme (esm/ui/themes.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterHeight
} from "../core/utils/size";
import devices from "../core/devices";
import domAdapter from "../core/dom_adapter";
import Promise from "../core/polyfills/promise";
import $ from "../core/renderer";
import {
    Deferred
} from "../core/utils/deferred";
import {
    parseHTML
} from "../core/utils/html_parser";
import {
    each
} from "../core/utils/iterator";
import readyCallbacks from "../core/utils/ready_callbacks";
import {
    value as viewPortValue,
    changeCallback,
    originalViewPort
} from "../core/utils/view_port";
import {
    getWindow,
    hasWindow
} from "../core/utils/window";
import {
    themeReadyCallback
} from "./themes_callback";
import errors from "./widget/ui.errors";
var window = getWindow();
var ready = readyCallbacks.add;
var viewPort = viewPortValue;
var viewPortChanged = changeCallback;
var initDeferred = new Deferred;
var DX_LINK_SELECTOR = "link[rel=dx-theme]";
var THEME_ATTR = "data-theme";
var ACTIVE_ATTR = "data-active";
var DX_HAIRLINES_CLASS = "dx-hairlines";
var ANY_THEME = "any";
var context;
var $activeThemeLink;
var knownThemes;
var currentThemeName;
var pendingThemeName;
var defaultTimeout = 15e3;
var THEME_MARKER_PREFIX = "dx.";

function readThemeMarker() {
    if (!hasWindow()) {
        return null
    }
    var element = $("<div>", context).addClass("dx-theme-marker").appendTo(context.documentElement);
    var result;
    try {
        result = window.getComputedStyle(element.get(0)).fontFamily;
        if (!result) {
            return null
        }
        result = result.replace(/["']/g, "");
        if (result.substr(0, THEME_MARKER_PREFIX.length) !== THEME_MARKER_PREFIX) {
            return null
        }
        return result.substr(THEME_MARKER_PREFIX.length)
    } finally {
        element.remove()
    }
}
export function waitForThemeLoad(themeName) {
    var waitStartTime;
    var timerId;
    var intervalCleared = true;
    pendingThemeName = themeName;

    function handleLoaded() {
        pendingThemeName = null;
        clearInterval(timerId);
        intervalCleared = true;
        themeReadyCallback.fire();
        themeReadyCallback.empty();
        initDeferred.resolve()
    }
    if (isPendingThemeLoaded() || !defaultTimeout) {
        handleLoaded()
    } else {
        if (!intervalCleared) {
            if (pendingThemeName) {
                pendingThemeName = themeName
            }
            return
        }
        waitStartTime = Date.now();
        intervalCleared = false;
        timerId = setInterval((function() {
            var isLoaded = isPendingThemeLoaded();
            var isTimeout = !isLoaded && Date.now() - waitStartTime > defaultTimeout;
            if (isTimeout) {
                errors.log("W0004", pendingThemeName)
            }
            if (isLoaded || isTimeout) {
                handleLoaded()
            }
        }), 10)
    }
}
export function isPendingThemeLoaded() {
    if (!pendingThemeName) {
        return true
    }
    var anyThemePending = pendingThemeName === ANY_THEME;
    if ("resolved" === initDeferred.state() && anyThemePending) {
        return true
    }
    var themeMarker = readThemeMarker();
    if (themeMarker && anyThemePending) {
        return true
    }
    return themeMarker === pendingThemeName
}

function processMarkup() {
    var $allThemeLinks = $(DX_LINK_SELECTOR, context);
    if (!$allThemeLinks.length) {
        return
    }
    knownThemes = {};
    $activeThemeLink = $(parseHTML("<link rel=stylesheet>"), context);
    $allThemeLinks.each((function() {
        var link = $(this, context);
        var fullThemeName = link.attr(THEME_ATTR);
        var url = link.attr("href");
        var isActive = "true" === link.attr(ACTIVE_ATTR);
        knownThemes[fullThemeName] = {
            url: url,
            isActive: isActive
        }
    }));
    $allThemeLinks.last().after($activeThemeLink);
    $allThemeLinks.remove()
}

function resolveFullThemeName(desiredThemeName) {
    var desiredThemeParts = desiredThemeName ? desiredThemeName.split(".") : [];
    var result = null;
    if (knownThemes) {
        if (desiredThemeName in knownThemes) {
            return desiredThemeName
        }
        each(knownThemes, (function(knownThemeName, themeData) {
            var knownThemeParts = knownThemeName.split(".");
            if (desiredThemeParts[0] && knownThemeParts[0] !== desiredThemeParts[0]) {
                return
            }
            if (desiredThemeParts[1] && desiredThemeParts[1] !== knownThemeParts[1]) {
                return
            }
            if (desiredThemeParts[2] && desiredThemeParts[2] !== knownThemeParts[2]) {
                return
            }
            if (!result || themeData.isActive) {
                result = knownThemeName
            }
            if (themeData.isActive) {
                return false
            }
        }))
    }
    return result
}

function initContext(newContext) {
    try {
        if (newContext !== context) {
            knownThemes = null
        }
    } catch (x) {
        knownThemes = null
    }
    context = newContext
}
export function init(options) {
    options = options || {};
    initContext(options.context || domAdapter.getDocument());
    if (!context) {
        return
    }
    processMarkup();
    currentThemeName = void 0;
    current(options)
}
export function current(options) {
    if (!arguments.length) {
        currentThemeName = currentThemeName || readThemeMarker();
        return currentThemeName
    }
    detachCssClasses(viewPort());
    options = options || {};
    if ("string" === typeof options) {
        options = {
            theme: options
        }
    }
    var isAutoInit = options._autoInit;
    var loadCallback = options.loadCallback;
    var currentThemeData;
    currentThemeName = resolveFullThemeName(options.theme || currentThemeName);
    if (currentThemeName) {
        currentThemeData = knownThemes[currentThemeName]
    }
    if (loadCallback) {
        themeReadyCallback.add(loadCallback)
    }
    if (currentThemeData) {
        $activeThemeLink.attr("href", knownThemes[currentThemeName].url);
        if (themeReadyCallback.has() || "resolved" !== initDeferred.state() || options._forceTimeout) {
            waitForThemeLoad(currentThemeName)
        }
    } else if (isAutoInit) {
        if (hasWindow()) {
            waitForThemeLoad(ANY_THEME)
        }
        themeReadyCallback.fire();
        themeReadyCallback.empty()
    } else {
        throw errors.Error("E0021", currentThemeName)
    }
    initDeferred.done(() => attachCssClasses(originalViewPort(), currentThemeName))
}

function getCssClasses(themeName) {
    themeName = themeName || current();
    var result = [];
    var themeNameParts = themeName && themeName.split(".");
    if (themeNameParts) {
        result.push("dx-theme-" + themeNameParts[0], "dx-theme-" + themeNameParts[0] + "-typography");
        if (themeNameParts.length > 1) {
            result.push("dx-color-scheme-" + themeNameParts[1] + (isMaterial(themeName) ? "-" + themeNameParts[2] : ""))
        }
    }
    return result
}
var themeClasses;
export function attachCssClasses(element, themeName) {
    themeClasses = getCssClasses(themeName).join(" ");
    $(element).addClass(themeClasses);
    ! function() {
        var pixelRatio = hasWindow() && window.devicePixelRatio;
        if (!pixelRatio || pixelRatio < 2) {
            return
        }
        var $tester = $("<div>");
        $tester.css("border", ".5px solid transparent");
        $("body").append($tester);
        if (1 === getOuterHeight($tester)) {
            $(element).addClass(DX_HAIRLINES_CLASS);
            themeClasses += " " + DX_HAIRLINES_CLASS
        }
        $tester.remove()
    }()
}
export function detachCssClasses(element) {
    $(element).removeClass(themeClasses)
}

function themeReady(callback) {
    themeReadyCallback.add(callback)
}

function isTheme(themeRegExp, themeName) {
    if (!themeName) {
        themeName = currentThemeName || readThemeMarker()
    }
    return new RegExp(themeRegExp).test(themeName)
}
export function isMaterial(themeName) {
    return isTheme("material", themeName)
}
export function isGeneric(themeName) {
    return isTheme("generic", themeName)
}
export function isDark(themeName) {
    return isTheme("dark", themeName)
}
export function isCompact(themeName) {
    return isTheme("compact", themeName)
}
export function isWebFontLoaded(text, fontWeight) {
    var document = domAdapter.getDocument();
    var testElement = document.createElement("span");
    testElement.style.position = "absolute";
    testElement.style.top = "-9999px";
    testElement.style.left = "-9999px";
    testElement.style.visibility = "hidden";
    testElement.style.fontFamily = "Arial";
    testElement.style.fontSize = "250px";
    testElement.style.fontWeight = fontWeight;
    testElement.innerHTML = text;
    document.body.appendChild(testElement);
    var etalonFontWidth = testElement.offsetWidth;
    testElement.style.fontFamily = "Roboto, RobotoFallback, Arial";
    var testedFontWidth = testElement.offsetWidth;
    testElement.parentNode.removeChild(testElement);
    return etalonFontWidth !== testedFontWidth
}
export function waitWebFont(text, fontWeight) {
    return new Promise(resolve => {
        var clear = () => {
            clearInterval(intervalId);
            clearTimeout(timeoutId);
            resolve()
        };
        var intervalId = setInterval(() => {
            if (isWebFontLoaded(text, fontWeight)) {
                clear()
            }
        }, 15);
        var timeoutId = setTimeout(clear, 2e3)
    })
}

function autoInit() {
    init({
        _autoInit: true,
        _forceTimeout: true
    });
    if ($(DX_LINK_SELECTOR, context).length) {
        throw errors.Error("E0022")
    }
}
if (hasWindow()) {
    autoInit()
} else {
    ready(autoInit)
}
viewPortChanged.add((function(viewPort, prevViewPort) {
    initDeferred.done((function() {
        detachCssClasses(prevViewPort);
        attachCssClasses(viewPort)
    }))
}));
devices.changed.add((function() {
    init({
        _autoInit: true
    })
}));
export {
    themeReady as ready
};
export function resetTheme() {
    $activeThemeLink && $activeThemeLink.attr("href", "about:blank");
    currentThemeName = null;
    pendingThemeName = null;
    initDeferred = new Deferred
}
export function initialized(callback) {
    initDeferred.done(callback)
}
export function setDefaultTimeout(timeout) {
    defaultTimeout = timeout
}
export default {
    setDefaultTimeout: setDefaultTimeout,
    initialized: initialized,
    resetTheme: resetTheme,
    ready: themeReady,
    waitWebFont: waitWebFont,
    isWebFontLoaded: isWebFontLoaded,
    isCompact: isCompact,
    isDark: isDark,
    isGeneric: isGeneric,
    isMaterial: isMaterial,
    detachCssClasses: detachCssClasses,
    attachCssClasses: attachCssClasses,
    current: current,
    waitForThemeLoad: waitForThemeLoad,
    isPendingThemeLoaded: isPendingThemeLoaded
};
