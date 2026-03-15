/**
* DevExtreme (ui/themes.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
                                                                    * An object that serves as a namespace for the methods that work with DevExtreme CSS Themes.
                                                                    */
                                                                   export default class themes {
    /**
     * Gets the current theme&apos;s name.
     */
    static current(): string;
    /**
     * Sets a theme with a specific name.
     */
    static current(themeName: string): void;
    /**
     * Specifies a function to be executed each time a theme is switched.
     */
    static ready(callback: Function): void;
    /**
     * Specifies a function to be executed after a theme is loaded.
     */
    static initialized(callback: Function): void;
    static isMaterial(theme: string): boolean;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export function current(): string;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export function isMaterial(theme: string): boolean;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export function isCompact(theme: string): boolean;
