/**
 * DevExtreme (esm/integration/jquery/element.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    setPublicElementWrapper
} from "../../core/element";
import useJQueryFn from "./use_jquery";
var useJQuery = useJQueryFn();
var getPublicElement = function($element) {
    return $element
};
if (useJQuery) {
    setPublicElementWrapper(getPublicElement)
}
