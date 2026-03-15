/**
* DevExtreme (ui/button_group.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    UserDefinedElement,
    DxElement,
} from '../core/element';

import {
    template,
} from '../core/templates/template';

import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import {
    CollectionWidgetItem,
    SelectionChangedInfo,
} from './collection/ui.collection_widget.base';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ContentReadyEvent = EventInfo<dxButtonGroup>;

export type DisposingEvent = EventInfo<dxButtonGroup>;

export type InitializedEvent = InitializedEventInfo<dxButtonGroup>;

export type ItemClickEvent = NativeEventInfo<dxButtonGroup, KeyboardEvent | MouseEvent | PointerEvent> & ItemInfo;

export type OptionChangedEvent = EventInfo<dxButtonGroup> & ChangedOptionInfo;

export type SelectionChangedEvent = EventInfo<dxButtonGroup> & SelectionChangedInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxButtonGroupOptions extends WidgetOptions<dxButtonGroup> {
    /**
     * Specifies a template for all the buttons in the group.
     */
    buttonTemplate?: template | ((buttonData: any, buttonContent: DxElement) => string | UserDefinedElement);
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies whether the UI component changes its state when a user pauses on it.
     */
    hoverStateEnabled?: boolean;
    /**
     * Configures buttons in the group.
     */
    items?: Array<Item>;
    /**
     * Specifies which data field provides keys used to distinguish between the selected buttons.
     */
    keyExpr?: string | Function;
    /**
     * A function that is executed when a button is clicked or tapped.
     */
    onItemClick?: ((e: ItemClickEvent) => void);
    /**
     * A function that is executed when a button is selected or selection is canceled.
     */
    onSelectionChanged?: ((e: SelectionChangedEvent) => void);
    /**
     * Contains the keys of the selected buttons and allows selecting buttons initially.
     */
    selectedItemKeys?: Array<any>;
    /**
     * Contains the data objects that correspond to the selected buttons. The data objects are taken from the items array.
     */
    selectedItems?: Array<any>;
    /**
     * Specifies the button selection mode.
     */
    selectionMode?: 'multiple' | 'single' | 'none';
    /**
     * Specifies how buttons in the group are styled.
     */
    stylingMode?: 'text' | 'outlined' | 'contained';
}
/**
 * The ButtonGroup is a UI component that contains a set of toggle buttons and can be used as a mode switcher.
 */
export default class dxButtonGroup extends Widget<dxButtonGroupOptions> { }

export type Item = dxButtonGroupItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxButtonGroupItem extends CollectionWidgetItem {
    /**
     * Specifies a text for the hint that appears when the button is hovered over or long-pressed.
     */
    hint?: string;
    /**
     * Specifies the icon to be displayed on the button.
     */
    icon?: string;
    /**
     * Specifies the button type.
     */
    type?: 'back' | 'danger' | 'default' | 'normal' | 'success';

    /**
     * Specifies the global attributes to be attached to the button group item&apos;s container element.
     */
    elementAttr?: { [key: string]: any };
}

export type Properties = dxButtonGroupOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxButtonGroupOptions;
