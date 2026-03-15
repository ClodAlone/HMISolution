/**
* DevExtreme (ui/validation_summary.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import CollectionWidget, {
    CollectionWidgetOptions,
    ItemLike,
} from './collection/ui.collection_widget.base';

export type ContentReadyEvent<
    TItem extends ItemLike = any,
    TKey = any,
> = EventInfo<dxValidationSummary<TItem, TKey>>;

export type DisposingEvent<
    TItem extends ItemLike = any,
    TKey = any,
> = EventInfo<dxValidationSummary<TItem, TKey>>;

export type InitializedEvent<
    TItem extends ItemLike = any,
    TKey = any,
> = InitializedEventInfo<dxValidationSummary<TItem, TKey>>;

export type ItemClickEvent<
    TItem extends ItemLike = any,
    TKey = any,
> = NativeEventInfo<dxValidationSummary<TItem, TKey>, MouseEvent | PointerEvent> & ItemInfo<TItem>;

export type OptionChangedEvent<
    TItem extends ItemLike = any,
    TKey = any,
> = EventInfo<dxValidationSummary<TItem, TKey>> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 */
export interface dxValidationSummaryOptions<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidgetOptions<dxValidationSummary<TItem, TKey>, TItem, TKey> {
    /**
     * Specifies the validation group for which summary should be generated.
     */
    validationGroup?: string;
}
/**
 * A UI component for displaying the result of checking validation rules for editors.
 */
export default class dxValidationSummary<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidget<dxValidationSummaryOptions<TItem, TKey>, TItem, TKey> { }

export type ExplicitTypes<
    TItem extends ItemLike,
    TKey,
> = {
    Properties: Properties<TItem, TKey>;
    ContentReadyEvent: ContentReadyEvent<TItem, TKey>;
    DisposingEvent: DisposingEvent<TItem, TKey>;
    InitializedEvent: InitializedEvent<TItem, TKey>;
    ItemClickEvent: ItemClickEvent<TItem, TKey>;
    OptionChangedEvent: OptionChangedEvent<TItem, TKey>;
};

export type Properties<
    TItem extends ItemLike = any,
    TKey = any,
> = dxValidationSummaryOptions<TItem, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
    TItem extends ItemLike = any,
    TKey = any,
> = Properties<TItem, TKey>;
