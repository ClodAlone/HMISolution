/**
* DevExtreme (ui/hierarchical_collection/ui.hierarchical_collection_widget.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import CollectionWidget, {
    CollectionWidgetOptions,
    ItemLike,
} from '../collection/ui.collection_widget.base';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface HierarchicalCollectionWidgetOptions<
    TComponent extends HierarchicalCollectionWidget<any, TItem, TKey>,
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidgetOptions<TComponent, TItem, TKey> {
    /**
     * Specifies the name of the data source item field whose value defines whether or not the corresponding UI component item is disabled.
     */
    disabledExpr?: string | Function;
    /**
     * Specifies the data field whose values should be displayed.
     */
    displayExpr?: string | ((item: TItem) => string);
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies whether the UI component changes its state when a user pauses on it.
     */
    hoverStateEnabled?: boolean;
    /**
     * Specifies which data field contains nested items.
     */
    itemsExpr?: string | Function;
    /**
     * Specifies which data field provides keys for TreeView items.
     */
    keyExpr?: string | Function;
    /**
     * Specifies the name of the data source item field whose value defines whether or not the corresponding UI component items is selected.
     */
    selectedExpr?: string | Function;
}
/**
 * The base class for UI components containing an item collection.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class HierarchicalCollectionWidget<
    TProperties extends HierarchicalCollectionWidgetOptions<any, TItem, TKey>,
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidget<TProperties, TItem, TKey> { }
