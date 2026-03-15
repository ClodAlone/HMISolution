/**
* DevExtreme (ui/collection/ui.collection_widget.base.d.ts)
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

import DataSource, { DataSourceLike } from '../../data/data_source';

import {
    EventInfo,
    NativeEventInfo,
    ItemInfo,
} from '../../events/index';

import Widget, {
    WidgetOptions,
} from '../widget/ui.widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type ItemLike = string | CollectionWidgetItem | any;

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface SelectionChangedInfo<TItem extends ItemLike = any> {
    readonly addedItems: Array<TItem>;
    readonly removedItems: Array<TItem>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CollectionWidgetOptions<
    TComponent extends CollectionWidget<any, TItem, TKey> | any,
    TItem extends ItemLike = any,
    TKey = any,
> extends WidgetOptions<TComponent> {
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TItem, TKey>;
    /**
     * The time period in milliseconds before the onItemHold event is raised.
     */
    itemHoldTimeout?: number;
    /**
     * Specifies a custom template for items.
     */
    itemTemplate?: template | ((itemData: TItem, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * An array of items displayed by the UI component.
     */
    items?: Array<TItem>;
    /**
     * Specifies the key property that provides key values to access data items. Each key value must be unique.
     */
    keyExpr?: string | Function;
    /**
     * Specifies the text or HTML markup displayed by the UI component if the item collection is empty.
     */
    noDataText?: string;
    /**
     * A function that is executed when a collection item is clicked or tapped.
     */
    onItemClick?: ((e: NativeEventInfo<TComponent, MouseEvent | PointerEvent> & ItemInfo<TItem>) => void) | string;
    /**
     * A function that is executed when a collection item is right-clicked or pressed.
     */
    onItemContextMenu?: ((e: NativeEventInfo<TComponent, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>) => void);
    /**
     * A function that is executed when a collection item has been held for a specified period.
     */
    onItemHold?: ((e: NativeEventInfo<TComponent, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>) => void);
    /**
     * A function that is executed after a collection item is rendered.
     */
    onItemRendered?: ((e: EventInfo<TComponent> & ItemInfo<TItem>) => void);
    /**
     * A function that is executed when a collection item is selected or selection is canceled.
     */
    onSelectionChanged?: ((e: EventInfo<TComponent> & SelectionChangedInfo<TItem>) => void);
    /**
     * The index of the currently selected UI component item.
     */
    selectedIndex?: number;
    /**
     * The selected item object.
     */
    selectedItem?: TItem;
    /**
     * Specifies an array of currently selected item keys.
     */
    selectedItemKeys?: Array<TKey>;
    /**
     * An array of currently selected item objects.
     */
    selectedItems?: Array<TItem>;
}
/**
 * The base class for UI components containing an item collection.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class CollectionWidget<
    TProperties extends CollectionWidgetOptions<any, TItem, TKey>,
    TItem extends ItemLike = any,
    TKey = any,
> extends Widget<TProperties> {
    getDataSource(): DataSource<TItem, TKey>;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CollectionWidgetItem {
    /**
     * Specifies whether the UI component item responds to user interaction.
     */
    disabled?: boolean;
    /**
     * Specifies the HTML markup to be inserted into the item element.
     */
    html?: string;
    /**
     * Specifies a template that should be used to render this item only.
     */
    template?: template | ((itemData: this, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies text displayed for the UI component item.
     */
    text?: string;
    /**
     * Specifies whether or not a UI component item must be displayed.
     */
    visible?: boolean;
}
