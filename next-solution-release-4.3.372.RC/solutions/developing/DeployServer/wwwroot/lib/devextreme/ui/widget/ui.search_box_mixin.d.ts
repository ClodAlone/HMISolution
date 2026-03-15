/**
* DevExtreme (ui/widget/ui.search_box_mixin.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    Properties as TextBoxProperties,
} from '../text_box';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface SearchBoxMixinOptions {
    /**
     * Configures the search panel.
     */
    searchEditorOptions?: TextBoxProperties;
    /**
     * Specifies whether the search panel is visible.
     */
    searchEnabled?: boolean;
    /**
     * Specifies a data object&apos;s field name or an expression whose value is compared to the search string.
     */
    searchExpr?: string | Function | Array<string | Function>;
    /**
     * Specifies a comparison operation used to search UI component items.
     */
    searchMode?: 'contains' | 'startswith' | 'equals';
    /**
     * Specifies a delay in milliseconds between when a user finishes typing, and the search is executed.
     */
    searchTimeout?: number;
    /**
     * Specifies the current search string.
     */
    searchValue?: string;
}
/**
                                                                    * 
                                                                    * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
                                                                    */
                                                                   export default class SearchBoxMixin {
    constructor(options?: SearchBoxMixinOptions)
}
