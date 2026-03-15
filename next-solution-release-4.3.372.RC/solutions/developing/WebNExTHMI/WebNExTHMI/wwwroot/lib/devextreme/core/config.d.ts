/**
* DevExtreme (core/config.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    PositionConfig,
} from '../animation/position';

/**
 * Gets the current global configuration.
 */
declare function config(): globalConfig;

/**
                                                          * Configures your application before its launch.
                                                          */
                                                         declare function config(config: globalConfig): void;

/**
 * Specifies settings that affect all DevExtreme UI components.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface globalConfig {
    /**
     * A decimal separator. No longer applies.
     * @deprecated 
     */
    decimalSeparator?: string;
    /**
     * The default currency. Accepts a 3-letter ISO 4217 code.
     */
    defaultCurrency?: string;
    /**
     * Specifies how editors&apos; text fields are styled in your application.
     */
    editorStylingMode?: 'outlined' | 'underlined' | 'filled';
    /**
     * Configures a Floating Action Button (FAB) that emits a stack of related actions (speed dial).
     */
    floatingActionButtonConfig?: {
      /**
       * Specifies the icon the FAB displays when the speed dial is opened.
       */
      closeIcon?: string;
      /**
       * Specifies the direction in which to open the speed dial menu.
       */
      direction?: 'auto' | 'up' | 'down';
      /**
       * Specifies the icon the FAB displays when the speed dial is closed.
       */
      icon?: string;
      /**
       * Specifies the text label displayed inside the FAB.
       */
      label?: string;
      /**
       * Limits the number of speed dial actions.
       */
      maxSpeedDialActionCount?: number;
      /**
       * Positions the FAB on the screen.
       */
      position?: 'bottom' | 'center' | 'left' | 'left bottom' | 'left top' | 'right' | 'right bottom' | 'right top' | 'top' | PositionConfig | Function;
      /**
       * If true, the background should be shaded when the speed dial menu is open.
       */
      shading?: boolean;
    };
    /**
     * Specifies whether dates are parsed and serialized according to the ISO 8601 standard in all browsers.
     */
    forceIsoDateParsing?: boolean;
    /**
     * Specifies whether to convert string values to lowercase in filter and search requests to OData services. Applies to the following operations: &apos;startswith&apos;, &apos;endswith&apos;, &apos;contains&apos;, and &apos;notcontains&apos;.
     */
    oDataFilterToLower?: boolean;
    /**
     * Specifies whether the UI components support a right-to-left representation. Available for individual UI components as well.
     */
    rtlEnabled?: boolean;
    /**
     * The decimal separator that is used when submitting a value to the server.
     */
    serverDecimalSeparator?: string;
    /**
     * A group separator. No longer applies.
     * @deprecated 
     */
    thousandsSeparator?: string;
    /**
     * 
     */
    useLegacyStoreResult?: boolean;
    /**
     * 
     */
    useLegacyVisibleIndex?: boolean;
}

export default config;
