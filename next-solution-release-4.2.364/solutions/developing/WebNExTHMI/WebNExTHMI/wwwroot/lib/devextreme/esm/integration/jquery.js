/**
 * DevExtreme (esm/integration/jquery.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import jQuery from "jquery";
import {
    compare as compareVersions
} from "../core/utils/version";
import errors from "../core/utils/error";
import useJQueryMethod from "./jquery/use_jquery";
var useJQuery = useJQueryMethod();
if (useJQuery && compareVersions(jQuery.fn.jquery, [1, 10]) < 0) {
    throw errors.Error("E0012")
}
import "./jquery/renderer";
import "./jquery/hooks";
import "./jquery/deferred";
import "./jquery/hold_ready";
import "./jquery/events";
import "./jquery/easing";
import "./jquery/element_data";
import "./jquery/element";
import "./jquery/component_registrator";
import "./jquery/ajax";
