/**
* DevExtreme (ui/validator.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DOMComponent, {
    DOMComponentOptions,
} from '../core/dom_component';

import {
    DxPromise,
} from '../core/utils/deferred';

import {
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import {
    AsyncRule,
    ValidationRule,
} from './validation_rules';

export type DisposingEvent = EventInfo<dxValidator>;

export type InitializedEvent = InitializedEventInfo<dxValidator>;

export type OptionChangedEvent = EventInfo<dxValidator> & ChangedOptionInfo;

export type ValidatedEvent = {
    name?: string;
    isValid?: boolean;
    value?: any;
    validationRules?: Array<ValidationRule>;
    brokenRule?: ValidationRule;
    brokenRules?: ValidationRule;
    status?: 'valid' | 'invalid' | 'pending';
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxValidatorOptions extends DOMComponentOptions<dxValidator> {
    /**
     * An object that specifies what and when to validate, and how to apply the validation result.
     */
    adapter?: {
      /**
       * A function that the Validator UI component calls after validating a specified value.
       */
      applyValidationResults?: Function;
      /**
       * A function that returns a Boolean value specifying whether or not to bypass validation.
       */
      bypass?: Function;
      /**
       * A function that sets focus to a validated editor when the corresponding ValidationSummary item is focused.
       */
      focus?: Function;
      /**
       * A function that returns the value to be validated.
       */
      getValue?: Function;
      /**
       * A function that resets the validated values.
       */
      reset?: Function;
      /**
       * Callbacks to be executed on the value validation.
       */
      validationRequestsCallbacks?: Array<Function>;
    };
    /**
     * Specifies the editor name to be used in the validation default messages.
     */
    name?: string;
    /**
     * A function that is executed after a value is validated.
     */
    onValidated?: ((validatedInfo: ValidatedEvent) => void);
    /**
     * Specifies the validation group the editor will be related to.
     */
    validationGroup?: string;
    /**
     * An array of validation rules to be checked for the editor with which the dxValidator object is associated.
     */
    validationRules?: Array<ValidationRule>;
}
/**
 * A UI component that is used to validate the associated DevExtreme editors against the defined validation rules.
 */
export default class dxValidator extends DOMComponent<dxValidatorOptions> {
    /**
     * Sets focus to the editor associated with the current Validator object.
     */
    focus(): void;
    /**
     * Resets the value and validation result of the editor associated with the current Validator object.
     */
    reset(): void;
    /**
     * Validates the value of the editor that is controlled by the current Validator object against the list of the specified validation rules.
     */
    validate(): ValidationResult;
}

export type ValidationResult = dxValidatorResult;

/**
 * A validation result.
 * @deprecated Use ValidationResult instead.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxValidatorResult {
    /**
     * A rule that failed to pass the check. Contains the first item from the brokenRules array.
     */
    brokenRule?: ValidationRule;
    /**
     * An array of the validationRules that failed to pass the check.
     */
    brokenRules?: Array<ValidationRule>;
    /**
     * A promise that is fulfilled when all async rules are validated.
     */
    complete?: DxPromise<dxValidatorResult>;
    /**
     * Indicates whether all the checked rules are satisfied.
     */
    isValid?: boolean;
    /**
     * An array of async rules whose promises are not fulfilled or rejected. Contains items only when the status is &apos;pending&apos;.
     */
    pendingRules?: Array<AsyncRule>;
    /**
     * Indicates the validation status.
     */
    status?: 'valid' | 'invalid' | 'pending';
    /**
     * Validation rules specified for the Validator.
     */
    validationRules?: Array<ValidationRule>;
    /**
     * The value being validated.
     */
    value?: any;
}

export type Properties = dxValidatorOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxValidatorOptions;
