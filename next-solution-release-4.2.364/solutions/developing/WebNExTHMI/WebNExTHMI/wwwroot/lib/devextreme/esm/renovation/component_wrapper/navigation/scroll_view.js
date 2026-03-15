/**
 * DevExtreme (esm/renovation/component_wrapper/navigation/scroll_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Component from "../common/component";
import {
    Deferred
} from "../../../core/utils/deferred";
export class ScrollViewWrapper extends Component {
    update() {
        var _this$viewRef;
        null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.updateHandler();
        return Deferred().resolve()
    }
    release(preventScrollBottom) {
        this.viewRef.release(preventScrollBottom);
        return Deferred().resolve()
    }
    isRenovated() {
        return !!Component.IS_RENOVATED_WIDGET
    }
    _dimensionChanged() {
        var _this$viewRef2;
        null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.updateHandler()
    }
    _optionChanged(option) {
        var {
            name: name
        } = option;
        if ("useNative" === name) {
            this._isNodeReplaced = false
        }
        super._optionChanged(option)
    }
}
