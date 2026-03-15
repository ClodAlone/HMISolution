/**
 * DevExtreme (esm/core/devices.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight,
    getWidth
} from "./utils/size";
import $ from "../core/renderer";
import {
    getWindow,
    getNavigator,
    hasWindow
} from "./utils/window";
import {
    extend
} from "./utils/extend";
import {
    isPlainObject
} from "./utils/type";
import {
    each
} from "./utils/iterator";
import errors from "./errors";
import Callbacks from "./utils/callbacks";
import readyCallbacks from "./utils/ready_callbacks";
import resizeCallbacks from "./utils/resize_callbacks";
import {
    EventsStrategy
} from "./events_strategy";
import {
    sessionStorage as SessionStorage
} from "./utils/storage";
import {
    changeCallback,
    value as viewPort
} from "./utils/view_port";
import Config from "./config";
var navigator = getNavigator();
var window = getWindow();
var KNOWN_UA_TABLE = {
    iPhone: "iPhone",
    iPhone5: "iPhone",
    iPhone6: "iPhone",
    iPhone6plus: "iPhone",
    iPad: "iPad",
    iPadMini: "iPad Mini",
    androidPhone: "Android Mobile",
    androidTablet: "Android",
    msSurface: "Windows ARM Tablet PC",
    desktop: "desktop"
};
var DEFAULT_DEVICE = {
    deviceType: "desktop",
    platform: "generic",
    version: [],
    phone: false,
    tablet: false,
    android: false,
    ios: false,
    generic: true,
    grade: "A",
    mac: false
};
var uaParsers = {
    generic(userAgent) {
        var isPhone = /windows phone/i.test(userAgent) || userAgent.match(/WPDesktop/);
        var isTablet = !isPhone && /Windows(.*)arm(.*)Tablet PC/i.test(userAgent);
        var isDesktop = !isPhone && !isTablet && /msapphost/i.test(userAgent);
        var isMac = /((intel|ppc) mac os x)/.test(userAgent.toLowerCase());
        if (!(isPhone || isTablet || isDesktop || isMac)) {
            return
        }
        return {
            deviceType: isPhone ? "phone" : isTablet ? "tablet" : "desktop",
            platform: "generic",
            version: [],
            grade: "A",
            mac: isMac
        }
    },
    ios(userAgent) {
        if (!/ip(hone|od|ad)/i.test(userAgent)) {
            return
        }
        var isPhone = /ip(hone|od)/i.test(userAgent);
        var matches = userAgent.match(/os (\d+)_(\d+)_?(\d+)?/i);
        var version = matches ? [parseInt(matches[1], 10), parseInt(matches[2], 10), parseInt(matches[3] || 0, 10)] : [];
        var isIPhone4 = 480 === window.screen.height;
        var grade = isIPhone4 ? "B" : "A";
        return {
            deviceType: isPhone ? "phone" : "tablet",
            platform: "ios",
            version: version,
            grade: grade
        }
    },
    android(userAgent) {
        if (!/android|htc_|silk/i.test(userAgent)) {
            return
        }
        var isPhone = /mobile/i.test(userAgent);
        var matches = userAgent.match(/android (\d+)\.?(\d+)?\.?(\d+)?/i);
        var version = matches ? [parseInt(matches[1], 10), parseInt(matches[2] || 0, 10), parseInt(matches[3] || 0, 10)] : [];
        var worseThan4_4 = version.length > 1 && (version[0] < 4 || 4 === version[0] && version[1] < 4);
        var grade = worseThan4_4 ? "B" : "A";
        return {
            deviceType: isPhone ? "phone" : "tablet",
            platform: "android",
            version: version,
            grade: grade
        }
    }
};
class Devices {
    constructor(options) {
        this._window = (null === options || void 0 === options ? void 0 : options.window) || window;
        this._realDevice = this._getDevice();
        this._currentDevice = void 0;
        this._currentOrientation = void 0;
        this._eventsStrategy = new EventsStrategy(this);
        this.changed = Callbacks();
        if (hasWindow()) {
            readyCallbacks.add(this._recalculateOrientation.bind(this));
            resizeCallbacks.add(this._recalculateOrientation.bind(this))
        }
    }
    current(deviceOrName) {
        if (deviceOrName) {
            this._currentDevice = this._getDevice(deviceOrName);
            this._forced = true;
            this.changed.fire();
            return
        }
        if (!this._currentDevice) {
            deviceOrName = void 0;
            try {
                deviceOrName = this._getDeviceOrNameFromWindowScope()
            } catch (e) {
                deviceOrName = this._getDeviceNameFromSessionStorage()
            } finally {
                if (!deviceOrName) {
                    deviceOrName = this._getDeviceNameFromSessionStorage()
                }
                if (deviceOrName) {
                    this._forced = true
                }
            }
            this._currentDevice = this._getDevice(deviceOrName)
        }
        return this._currentDevice
    }
    real(forceDevice) {
        return extend({}, this._realDevice)
    }
    orientation() {
        return this._currentOrientation
    }
    isForced() {
        return this._forced
    }
    isRippleEmulator() {
        return !!this._window.tinyHippos
    }
    _getCssClasses(device) {
        var result = [];
        var realDevice = this._realDevice;
        device = device || this.current();
        if (device.deviceType) {
            result.push("dx-device-".concat(device.deviceType));
            if ("desktop" !== device.deviceType) {
                result.push("dx-device-mobile")
            }
        }
        result.push("dx-device-".concat(realDevice.platform));
        if (realDevice.version && realDevice.version.length) {
            result.push("dx-device-".concat(realDevice.platform, "-").concat(realDevice.version[0]))
        }
        if (this.isSimulator()) {
            result.push("dx-simulator")
        }
        if (Config().rtlEnabled) {
            result.push("dx-rtl")
        }
        return result
    }
    attachCssClasses(element, device) {
        this._deviceClasses = this._getCssClasses(device).join(" ");
        $(element).addClass(this._deviceClasses)
    }
    detachCssClasses(element) {
        $(element).removeClass(this._deviceClasses)
    }
    isSimulator() {
        try {
            return this._isSimulator || hasWindow() && this._window.top !== this._window.self && this._window.top["dx-force-device"] || this.isRippleEmulator()
        } catch (e) {
            return false
        }
    }
    forceSimulator() {
        this._isSimulator = true
    }
    _getDevice(deviceName) {
        if ("genericPhone" === deviceName) {
            deviceName = {
                deviceType: "phone",
                platform: "generic",
                generic: true
            }
        }
        if (isPlainObject(deviceName)) {
            return this._fromConfig(deviceName)
        } else {
            var ua;
            if (deviceName) {
                ua = KNOWN_UA_TABLE[deviceName];
                if (!ua) {
                    throw errors.Error("E0005")
                }
            } else {
                ua = navigator.userAgent
            }
            return this._fromUA(ua)
        }
    }
    _getDeviceOrNameFromWindowScope() {
        var result;
        if (hasWindow() && (this._window.top["dx-force-device-object"] || this._window.top["dx-force-device"])) {
            result = this._window.top["dx-force-device-object"] || this._window.top["dx-force-device"]
        }
        return result
    }
    _getDeviceNameFromSessionStorage() {
        var sessionStorage = SessionStorage();
        if (!sessionStorage) {
            return
        }
        var deviceOrName = sessionStorage.getItem("dx-force-device");
        try {
            return JSON.parse(deviceOrName)
        } catch (ex) {
            return deviceOrName
        }
    }
    _fromConfig(config) {
        var result = extend({}, DEFAULT_DEVICE, this._currentDevice, config);
        var shortcuts = {
            phone: "phone" === result.deviceType,
            tablet: "tablet" === result.deviceType,
            android: "android" === result.platform,
            ios: "ios" === result.platform,
            generic: "generic" === result.platform
        };
        return extend(result, shortcuts)
    }
    _fromUA(ua) {
        var config;
        each(uaParsers, (platform, parser) => {
            config = parser(ua);
            return !config
        });
        if (config) {
            return this._fromConfig(config)
        }
        return DEFAULT_DEVICE
    }
    _changeOrientation() {
        var $window = $(this._window);
        var orientation = getHeight($window) > getWidth($window) ? "portrait" : "landscape";
        if (this._currentOrientation === orientation) {
            return
        }
        this._currentOrientation = orientation;
        this._eventsStrategy.fireEvent("orientationChanged", [{
            orientation: orientation
        }])
    }
    _recalculateOrientation() {
        var windowWidth = getWidth(this._window);
        if (this._currentWidth === windowWidth) {
            return
        }
        this._currentWidth = windowWidth;
        this._changeOrientation()
    }
    on(eventName, eventHandler) {
        this._eventsStrategy.on(eventName, eventHandler);
        return this
    }
    off(eventName, eventHandler) {
        this._eventsStrategy.off(eventName, eventHandler);
        return this
    }
}
var devices = new Devices;
var viewPortElement = viewPort();
if (viewPortElement) {
    devices.attachCssClasses(viewPortElement)
}
changeCallback.add((viewPort, prevViewport) => {
    devices.detachCssClasses(prevViewport);
    devices.attachCssClasses(viewPort)
});
export default devices;
