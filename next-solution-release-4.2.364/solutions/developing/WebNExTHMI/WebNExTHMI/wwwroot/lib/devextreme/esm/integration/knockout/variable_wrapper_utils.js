/**
 * DevExtreme (esm/integration/knockout/variable_wrapper_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import ko from "knockout";
import variableWrapper from "../../core/utils/variable_wrapper";
if (ko) {
    variableWrapper.inject({
        isWrapped: ko.isObservable,
        isWritableWrapped: ko.isWritableObservable,
        wrap: ko.observable,
        unwrap: function(value) {
            if (ko.isObservable(value)) {
                return ko.utils.unwrapObservable(value)
            }
            return this.callBase(value)
        },
        assign: function(variable, value) {
            if (ko.isObservable(variable)) {
                variable(value)
            } else {
                this.callBase(variable, value)
            }
        }
    })
}
