/**
 * DevExtreme (esm/integration/jquery/hold_ready.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import jQuery from "jquery";
import {
    themeReadyCallback
} from "../../ui/themes_callback";
import readyCallbacks from "../../core/utils/ready_callbacks";
if (jQuery && !themeReadyCallback.fired()) {
    var holdReady = jQuery.holdReady || jQuery.fn.holdReady;
    holdReady(true);
    themeReadyCallback.add((function() {
        readyCallbacks.add((function() {
            holdReady(false)
        }))
    }))
}
