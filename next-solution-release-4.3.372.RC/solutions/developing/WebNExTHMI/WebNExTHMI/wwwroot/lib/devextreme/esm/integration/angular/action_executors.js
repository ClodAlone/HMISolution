/**
 * DevExtreme (esm/integration/angular/action_executors.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Action from "../../core/action";
import angular from "angular";
if (angular) {
    Action.registerExecutor({
        ngExpression: {
            execute: function(e) {
                if ("string" === typeof e.action) {
                    e.context.$eval(e.action)
                }
            }
        }
    })
}
