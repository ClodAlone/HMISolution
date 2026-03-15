/**
 * DevExtreme (esm/ui/validation_group.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import registerComponent from "../core/component_registrator";
import DOMComponent from "../core/dom_component";
import ValidationSummary from "./validation_summary";
import ValidationEngine from "./validation_engine";
import Validator from "./validator";
var VALIDATION_ENGINE_CLASS = "dx-validationgroup";
var VALIDATOR_CLASS = "dx-validator";
var VALIDATION_SUMMARY_CLASS = "dx-validationsummary";
class ValidationGroup extends DOMComponent {
    _getDefaultOptions() {
        return super._getDefaultOptions()
    }
    _init() {
        super._init();
        ValidationEngine.addGroup(this)
    }
    _initMarkup() {
        var $element = this.$element();
        $element.addClass(VALIDATION_ENGINE_CLASS);
        $element.find(".".concat(VALIDATOR_CLASS)).each((function(_, validatorContainer) {
            Validator.getInstance($(validatorContainer))._initGroupRegistration()
        }));
        $element.find(".".concat(VALIDATION_SUMMARY_CLASS)).each((function(_, summaryContainer) {
            ValidationSummary.getInstance($(summaryContainer))._initGroupRegistration()
        }));
        super._initMarkup()
    }
    validate() {
        return ValidationEngine.validateGroup(this)
    }
    reset() {
        return ValidationEngine.resetGroup(this)
    }
    _dispose() {
        ValidationEngine.removeGroup(this);
        this.$element().removeClass(VALIDATION_ENGINE_CLASS);
        super._dispose()
    }
    _useTemplates() {
        return false
    }
}
registerComponent("dxValidationGroup", ValidationGroup);
export default ValidationGroup;
