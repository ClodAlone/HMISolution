/**
 * DevExtreme (esm/core/templates/template_base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../renderer";
import domAdapter from "../dom_adapter";
import Callbacks from "../utils/callbacks";
import {
    contains
} from "../utils/dom";
import {
    triggerShownEvent
} from "../../events/visibility_change";
import errors from "../errors";
export var renderedCallbacks = Callbacks({
    syncStrategy: true
});
export class TemplateBase {
    render(options) {
        options = options || {};
        var onRendered = options.onRendered;
        delete options.onRendered;
        var $result = this._renderCore(options);
        this._ensureResultInContainer($result, options.container);
        renderedCallbacks.fire($result, options.container);
        onRendered && onRendered();
        return $result
    }
    _ensureResultInContainer($result, container) {
        if (!container) {
            return
        }
        var $container = $(container);
        var resultInContainer = contains($container.get(0), $result.get(0));
        $container.append($result);
        if (resultInContainer) {
            return
        }
        var resultInBody = domAdapter.getBody().contains($container.get(0));
        if (!resultInBody) {
            return
        }
        triggerShownEvent($result)
    }
    _renderCore() {
        throw errors.Error("E0001")
    }
}
