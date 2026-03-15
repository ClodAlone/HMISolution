/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.accessibility.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import * as accessibility from "../../ui/shared/accessibility";
export var registerKeyboardAction = function(viewName, instance, $element, selector, action) {
    var keyboardController = instance.getController("keyboardNavigation");
    if (instance.option("useLegacyKeyboardNavigation") || keyboardController && !keyboardController.isKeyboardEnabled()) {
        return
    }
    instance.createAction("onKeyDown");
    accessibility.registerKeyboardAction(viewName, instance, $element, selector, action, args => {
        instance.executeAction("onKeyDown", args)
    })
};
