/**
* DevExtreme (ui/validation_rules.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
export interface ValidationCallbackData {
    value?: string | number;
    rule: any;
    validator: any;
    data?: any;
    column?: any;
    formItem?: any;
}

/**
 * A custom validation rule that is checked asynchronously. Use async rules for server-side validation.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface AsyncRule {
    /**
     * If true, the validationCallback is not executed for null, undefined, false, and empty strings.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Indicates whether the rule should always be checked for the target value or only when the value changes.
     */
    reevaluate?: boolean;
    /**
     * Specifies the rule type. Set it to &apos;async&apos; to use the AsyncRule.
     */
    type: 'async';
    /**
     * A function that validates the target value.
     */
    validationCallback?: ((options: ValidationCallbackData) => PromiseLike<any>);
}

/**
 * A validation rule that demands that a validated editor has a value that is equal to a specified expression.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CompareRule {
    /**
     * Specifies the function whose return value is used for comparison with the validated value.
     */
    comparisonTarget?: (() => any);
    /**
     * Specifies the operator to be used for comparing the validated value with the target.
     */
    comparisonType?: '!=' | '!==' | '<' | '<=' | '==' | '===' | '>' | '>=';
    /**
     * If set to true, empty values are valid.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Specifies the rule type. Set it to &apos;compare&apos; to use the CompareRule.
     */
    type: 'compare';
}

/**
 * A rule with custom validation logic.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CustomRule {
    /**
     * If true, the validationCallback is not executed for null, undefined, false, and empty strings.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Indicates whether the rule should be always checked for the target value or only when the target value changes.
     */
    reevaluate?: boolean;
    /**
     * Specifies the rule type. Set it to &apos;custom&apos; to use the CustomRule.
     */
    type: 'custom';
    /**
     * A function that validates the target value.
     */
    validationCallback?: ((options: ValidationCallbackData) => boolean);
}

/**
 * A validation rule that demands that the validated field match the Email pattern.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface EmailRule {
    /**
     * If set to true, empty values are valid.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Specifies the rule type. Set it to &apos;email&apos; to use the EmailRule.
     */
    type: 'email';
}

/**
 * A validation rule that demands that the validated field has a numeric value.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface NumericRule {
    /**
     * If set to true, empty values are valid.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Specifies the rule type. Set it to &apos;numeric&apos; to use the NumericRule.
     */
    type: 'numeric';
}

/**
 * A validation rule that demands that the validated field match a specified pattern.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface PatternRule {
    /**
     * If set to true, empty values are valid.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Specifies the regular expression that the validated value must match.
     */
    pattern?: RegExp | string;
    /**
     * Specifies the rule type. Set it to &apos;pattern&apos; to use the PatternRule.
     */
    type: 'pattern';
}

/**
 * A validation rule that demands the target value be within the specified value range (including the range&apos;s end points).
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RangeRule {
    /**
     * If set to true, empty values are valid.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the maximum value allowed for the validated value.
     */
    max?: Date | number;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Specifies the minimum value allowed for the validated value.
     */
    min?: Date | number;
    /**
     * Indicates whether the rule should be always checked for the target value or only when the target value changes.
     */
    reevaluate?: boolean;
    /**
     * Specifies the rule type. Set it to &apos;range&apos; to use the RangeRule.
     */
    type: 'range';
}

/**
 * A validation rule that demands that a validated field has a value.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RequiredRule {
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Indicates whether to remove the Space characters from the validated value.
     */
    trim?: boolean;
    /**
     * Specifies the rule type. Set it to &apos;required&apos; to use the RequiredRule.
     */
    type: 'required';
}

/**
 * A validation rule that demands the target value length be within the specified value range (including the range&apos;s end points).
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface StringLengthRule {
    /**
     * If set to true, empty values are valid.
     */
    ignoreEmptyValue?: boolean;
    /**
     * Specifies the maximum length allowed for the validated value.
     */
    max?: number;
    /**
     * Specifies the message that is shown if the rule is broken.
     */
    message?: string;
    /**
     * Specifies the minimum length allowed for the validated value.
     */
    min?: number;
    /**
     * Indicates whether or not to remove the Space characters from the validated value.
     */
    trim?: boolean;
    /**
     * Specifies the rule type. Set it to &apos;stringLength&apos; to use the StringLengthRule.
     */
    type: 'stringLength';
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type ValidationRule = AsyncRule | CompareRule | CustomRule | EmailRule | NumericRule | PatternRule | RangeRule | RequiredRule | StringLengthRule;
