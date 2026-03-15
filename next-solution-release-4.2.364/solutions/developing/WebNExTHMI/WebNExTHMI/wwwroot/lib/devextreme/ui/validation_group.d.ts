/**
* DevExtreme (ui/validation_group.d.ts)
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
    CompareRule,
    CustomRule,
    EmailRule,
    NumericRule,
    PatternRule,
    RangeRule,
    RequiredRule,
    StringLengthRule,
} from './validation_rules';

export type DisposingEvent = EventInfo<dxValidationGroup>;

export type InitializedEvent = InitializedEventInfo<dxValidationGroup>;

export type OptionChangedEvent = EventInfo<dxValidationGroup> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxValidationGroupOptions extends DOMComponentOptions<dxValidationGroup> {
}
/**
 * The ValidationGroup is a UI component that allows you to validate several editors simultaneously.
 */
export default class dxValidationGroup extends DOMComponent<dxValidationGroupOptions> {
    /**
     * Resets the value and validation result of the editors that are included to the current validation group.
     */
    reset(): void;
    /**
     * Validates rules of the validators that belong to the current validation group.
     */
    validate(): ValidationResult;
}

export type ValidationResult = dxValidationGroupResult;

/**
 * A group validation result.
 * @deprecated Use ValidationResult instead.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxValidationGroupResult {
    /**
     * An array of the validation rules that failed.
     */
    brokenRules?: Array<RequiredRule | NumericRule | RangeRule | StringLengthRule | CustomRule | CompareRule | PatternRule | EmailRule | AsyncRule>;
    /**
     * A promise that is fulfilled when all async rules are validated.
     */
    complete?: DxPromise<dxValidationGroupResult>;
    /**
     * Indicates whether all the rules checked for the group are satisfied.
     */
    isValid?: boolean;
    /**
     * Indicates the validation status.
     */
    status?: 'valid' | 'invalid' | 'pending';
    /**
     * Validator UI components included in the validated group.
     */
    validators?: Array<any>;
}

export type Properties = dxValidationGroupOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxValidationGroupOptions;
