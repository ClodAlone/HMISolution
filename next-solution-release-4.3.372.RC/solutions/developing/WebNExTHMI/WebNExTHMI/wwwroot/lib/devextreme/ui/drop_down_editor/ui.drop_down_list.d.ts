/**
* DevExtreme (ui/drop_down_editor/ui.drop_down_list.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    UserDefinedElement,
    DxElement,
} from '../../core/element';

import {
    template,
} from '../../core/templates/template';

import DataSource from '../../data/data_source';

import {
    EventInfo,
    NativeEventInfo,
    ItemInfo,
} from '../../events/index';

import {
    ValueChangedInfo,
} from '../editor/editor';

import {
    DataExpressionMixinOptions,
} from '../editor/ui.data_expression';

import dxDropDownEditor, {
    dxDropDownEditorOptions,
} from './ui.drop_down_editor';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface SelectionChangedInfo<T = any> {
    readonly selectedItem: T;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDropDownListOptions<TComponent> extends DataExpressionMixinOptions<TComponent>, dxDropDownEditorOptions<TComponent> {
    /**
     * Returns the value currently displayed by the UI component.
     */
    displayValue?: string;
    /**
     * Specifies a custom template for group captions.
     */
    groupTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies whether data items should be grouped.
     */
    grouped?: boolean;
    /**
     * The minimum number of characters that must be entered into the text box to begin a search. Applies only if searchEnabled is true.
     */
    minSearchLength?: number;
    /**
     * Specifies the text or HTML markup displayed by the UI component if the item collection is empty.
     */
    noDataText?: string;
    /**
     * A function that is executed when a list item is clicked or tapped.
     */
    onItemClick?: ((e: NativeEventInfo<TComponent, KeyboardEvent | MouseEvent | PointerEvent> & ItemInfo) => void);
    /**
     * A function that is executed when a list item is selected or selection is canceled.
     */
    onSelectionChanged?: ((e: EventInfo<TComponent> & SelectionChangedInfo) => void);
    /**
     * A function that is executed after the UI component&apos;s value is changed.
     */
    onValueChanged?: ((e: NativeEventInfo<TComponent, KeyboardEvent | MouseEvent | PointerEvent | Event> & ValueChangedInfo) => void);
    /**
     * Specifies whether to allow searching.
     */
    searchEnabled?: boolean;
    /**
     * Specifies the name of a data source item field or an expression whose value is compared to the search criterion.
     */
    searchExpr?: string | Function | Array<string | Function>;
    /**
     * Specifies a comparison operation used to search UI component items.
     */
    searchMode?: 'contains' | 'startswith';
    /**
     * Specifies the time delay, in milliseconds, after the last character has been typed in, before a search is executed.
     */
    searchTimeout?: number;
    /**
     * Gets the currently selected item.
     */
    selectedItem?: any;
    /**
     * Specifies whether or not the UI component displays unfiltered values until a user types a number of characters exceeding the minSearchLength property value.
     */
    showDataBeforeSearch?: boolean;
    /**
     * Specifies the currently selected value. May be an object if dataSource contains objects and valueExpr is not set.
     */
    value?: any;
    /**
     * Specifies the DOM events after which the UI component&apos;s value should be updated.
     */
    valueChangeEvent?: string;
    /**
     * Specifies whether text that exceeds the drop-down list width should be wrapped.
     */
    wrapItemText?: boolean;
    /**
     * Specifies whether the widget uses item&apos;s text a title attribute.
     */
    useItemTextAsTitle?: boolean;
}
/**
 * A base class for drop-down list UI components.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class dxDropDownList<TProperties> extends dxDropDownEditor<TProperties> {
    getDataSource(): DataSource;
}
