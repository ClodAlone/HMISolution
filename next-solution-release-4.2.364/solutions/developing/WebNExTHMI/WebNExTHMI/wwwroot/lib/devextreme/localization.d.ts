/**
* DevExtreme (localization.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * Converts a Date object to a string using the specified format.
 */
export function formatDate(value: Date, format: Format): string;

/**
 * Substitutes the provided value(s) for placeholders in a message that the key specifies.
 */
export function formatMessage(key: string, ...values: Array<string>): string;

/**
 * Converts a numeric value to a string using the specified format.
 */
export function formatNumber(value: number, format: Format): string;

/**
 * Loads localized messages.
 */
export function loadMessages(messages: any): void;

/**
 * Gets the current locale identifier.
 */
export function locale(): string;

/**
                                                          * Sets the current locale identifier.
                                                          */
                                                         export function locale(locale: string): void;

/**
 * Parses a string into a Date object.
 */
export function parseDate(text: string, format: Format): Date;

/**
 * Parses a string into a numeric value.
 */
export function parseNumber(text: string, format: Format): number;

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type PredefinedFormat = 'billions' | 'currency' | 'day' | 'decimal' | 'exponential' | 'fixedPoint' | 'largeNumber' | 'longDate' | 'longTime' | 'millions' | 'millisecond' | 'month' | 'monthAndDay' | 'monthAndYear' | 'percent' | 'quarter' | 'quarterAndYear' | 'shortDate' | 'shortTime' | 'thousands' | 'trillions' | 'year' | 'dayOfWeek' | 'hour' | 'longDateLongTime' | 'minute' | 'second' | 'shortDateShortTime';
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface FormatObject {
    /**
    * Specifies a 3-letter ISO 4217 code for currency. Applies only if the type is &apos;currency&apos;.
    */
   currency?: string;
   /**
    * A function that converts numeric or date-time values to a string.
    */
   formatter?: ((value: number | Date) => string);
   /**
    * Parses string values into numeric or date-time values. Should be used with formatter or one of the predefined formats.
    */
   parser?: ((value: string) => number | Date);
   /**
    * Specifies a precision for values of numeric or currency format types.
    */
   precision?: number;
   /**
    * Specifies a predefined format. Does not apply if you have specified the formatter function.
    */
   type?: PredefinedFormat | string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type ExternalFormat = never;

/**
 * Formats values.
 */
export type Format =
  FormatObject |
  PredefinedFormat |
  string |
  ((value: number | Date) => string) |
  ((value: Date) => string) |
  ((value: number) => string) |
  ExternalFormat;
