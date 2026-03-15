/**
* DevExtreme (ui/tag_box.d.ts)
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
    Cancelable,
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import {
    SelectionChangedInfo,
} from './collection/ui.collection_widget.base';

import {
    DropDownButtonTemplateDataModel,
} from './drop_down_editor/ui.drop_down_editor';

import {
    ValueChangedInfo,
} from './editor/editor';

import dxSelectBox, {
    dxSelectBoxOptions,
    CustomItemCreatingInfo,
} from './select_box';

export type ChangeEvent = NativeEventInfo<dxTagBox, Event>;

export type ClosedEvent = EventInfo<dxTagBox>;

export type ContentReadyEvent = EventInfo<dxTagBox>;

export type CustomItemCreatingEvent = EventInfo<dxTagBox> & CustomItemCreatingInfo;

export type DisposingEvent = EventInfo<dxTagBox>;

export type EnterKeyEvent = NativeEventInfo<dxTagBox, KeyboardEvent>;

export type FocusInEvent = NativeEventInfo<dxTagBox, FocusEvent>;

export type FocusOutEvent = NativeEventInfo<dxTagBox, FocusEvent>;

export type InitializedEvent = InitializedEventInfo<dxTagBox>;

export type InputEvent = NativeEventInfo<dxTagBox, UIEvent>;

export type ItemClickEvent = NativeEventInfo<dxTagBox> & ItemInfo;

export type KeyDownEvent = NativeEventInfo<dxTagBox, KeyboardEvent>;

export type KeyPressEvent = NativeEventInfo<dxTagBox, KeyboardEvent>;

export type KeyUpEvent = NativeEventInfo<dxTagBox, KeyboardEvent>;

export type MultiTagPreparingEvent = Cancelable & EventInfo<dxTagBox> & {
    readonly multiTagElement: DxElement;
    readonly selectedItems?: Array<string | number | any>;
    text?: string;
};

export type OpenedEvent = EventInfo<dxTagBox>;

export type OptionChangedEvent = EventInfo<dxTagBox> & ChangedOptionInfo;

export type SelectAllValueChangedEvent = EventInfo<dxTagBox> & {
    readonly value: boolean;
};

export type SelectionChangedEvent = EventInfo<dxTagBox> & SelectionChangedInfo<string | number | any>;

export type ValueChangedEvent = NativeEventInfo<dxTagBox, KeyboardEvent | MouseEvent | PointerEvent | Event> & ValueChangedInfo;

export type DropDownButtonTemplateData = DropDownButtonTemplateDataModel;

/**
  * @deprecated use Properties instead
  * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
  */
 export interface dxTagBoxOptions extends Pick<dxSelectBoxOptions<dxTagBox>, Exclude<keyof dxSelectBoxOptions<dxTagBox>, 'onSelectionChanged'>> {
    /**
     * Specifies how the UI component applies values.
     */
    applyValueMode?: 'instantly' | 'useButtons';
    /**
     * A Boolean value specifying whether or not to hide selected items.
     */
    hideSelectedItems?: boolean;
    /**
     * Specifies the limit on displayed tags. On exceeding it, the UI component replaces all tags with a single multi-tag that displays the number of selected items.
     */
    maxDisplayedTags?: number;
    /**
     * A Boolean value specifying whether or not the UI component is multiline.
     */
    multiline?: boolean;
    /**
     * A function that is executed before the multi-tag is rendered.
     */
    onMultiTagPreparing?: ((e: MultiTagPreparingEvent) => void);
    /**
     * A function that is executed when the &apos;Select All&apos; check box value is changed. Applies only if showSelectionControls is true.
     */
    onSelectAllValueChanged?: ((e: SelectAllValueChangedEvent) => void);
    /**
     * A function that is executed when a list item is selected or selection is canceled.
     */
    onSelectionChanged?: ((e: SelectionChangedEvent) => void);
    /**
     * Specifies the mode in which all items are selected.
     */
    selectAllMode?: 'allPages' | 'page';
    /**
     * Gets the currently selected items.
     */
    selectedItems?: Array<string | number | any>;
    /**
     * Specifies the text displayed at the &apos;Select All&apos; check box.
     */
    selectAllText?: string;
    /**
     * Specifies whether the drop-down button is visible.
     */
    showDropDownButton?: boolean;
    /**
     * Specifies the maximum filter query length in characters.
     */
    maxFilterQueryLength?: number;
    /**
     * Specifies whether the multi-tag is shown without ordinary tags.
     */
    showMultiTagOnly?: boolean;
    /**
     * Specifies a custom template for tags.
     */
    tagTemplate?: template | ((itemData: any, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the selected items.
     */
    value?: Array<string | number | any>;
}
/**
 * The TagBox UI component is an editor that allows an end user to select multiple items from a drop-down list.
 */
export default class dxTagBox extends dxSelectBox<dxTagBoxOptions> { }

export type Properties = dxTagBoxOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxTagBoxOptions;
