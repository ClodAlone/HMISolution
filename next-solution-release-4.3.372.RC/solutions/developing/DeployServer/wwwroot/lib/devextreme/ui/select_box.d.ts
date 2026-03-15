/**
* DevExtreme (ui/select_box.d.ts)
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
    DropDownButtonTemplateDataModel,
} from './drop_down_editor/ui.drop_down_editor';

import dxDropDownList, {
    dxDropDownListOptions,
    SelectionChangedInfo,
} from './drop_down_editor/ui.drop_down_list';

import {
    ValueChangedInfo,
} from './editor/editor';

import {
    Properties as PopupProperties,
} from './popup';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CustomItemCreatingInfo {
    readonly text?: string;
    customItem?: string | any | PromiseLike<any>;
}

export type ChangeEvent = NativeEventInfo<dxSelectBox, Event>;

export type ClosedEvent = EventInfo<dxSelectBox>;

export type ContentReadyEvent = EventInfo<dxSelectBox>;

export type CopyEvent = NativeEventInfo<dxSelectBox, ClipboardEvent>;

export type CustomItemCreatingEvent = EventInfo<dxSelectBox> & CustomItemCreatingInfo;

export type CutEvent = NativeEventInfo<dxSelectBox, ClipboardEvent>;

export type DisposingEvent = EventInfo<dxSelectBox>;

export type EnterKeyEvent = NativeEventInfo<dxSelectBox, KeyboardEvent>;

export type FocusInEvent = NativeEventInfo<dxSelectBox, FocusEvent>;

export type FocusOutEvent = NativeEventInfo<dxSelectBox, FocusEvent>;

export type InitializedEvent = InitializedEventInfo<dxSelectBox>;

export type InputEvent = NativeEventInfo<dxSelectBox, UIEvent>;

export type ItemClickEvent = NativeEventInfo<dxSelectBox, KeyboardEvent | MouseEvent | PointerEvent> & ItemInfo;

export type KeyDownEvent = NativeEventInfo<dxSelectBox, KeyboardEvent>;

export type KeyPressEvent = NativeEventInfo<dxSelectBox, KeyboardEvent>;

export type KeyUpEvent = NativeEventInfo<dxSelectBox, KeyboardEvent>;

export type OpenedEvent = EventInfo<dxSelectBox>;

export type OptionChangedEvent = EventInfo<dxSelectBox> & ChangedOptionInfo;

export type PasteEvent = NativeEventInfo<dxSelectBox, ClipboardEvent>;

export type SelectionChangedEvent = EventInfo<dxSelectBox> & SelectionChangedInfo;

export type ValueChangedEvent = NativeEventInfo<dxSelectBox, KeyboardEvent | MouseEvent | Event> & ValueChangedInfo;

export type DropDownButtonTemplateData = DropDownButtonTemplateDataModel;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSelectBoxOptions<TComponent> extends dxDropDownListOptions<TComponent> {
    /**
     * Specifies whether the UI component allows a user to enter a custom value. Requires the onCustomItemCreating handler implementation.
     */
    acceptCustomValue?: boolean;
    /**
     * Specifies a custom template for the text field. Must contain the TextBox UI component.
     */
    fieldTemplate?: template | ((selectedItem: any, fieldElement: DxElement) => string | UserDefinedElement);
    /**
     * A function that is executed when a user adds a custom item. Requires acceptCustomValue to be set to true.
     */
    onCustomItemCreating?: ((e: EventInfo<TComponent> & CustomItemCreatingInfo) => void);
    /**
     * Specifies whether a user can open the drop-down list by clicking a text field.
     */
    openOnFieldClick?: boolean;
    /**
     * The text that is provided as a hint in the select box editor.
     */
    placeholder?: string;
    /**
     * Specifies whether the drop-down button is visible.
     */
    showDropDownButton?: boolean;
    /**
     * Specifies whether or not to display selection controls.
     */
    showSelectionControls?: boolean;
    /**
     * Specifies the DOM events after which the UI component&apos;s value should be updated. Applies only if acceptCustomValue is set to true.
     */
    valueChangeEvent?: string;

    /**
     * Configures the drop-down field which holds the content.
     */
    dropDownOptions?: PopupProperties;
}
/**
 * The SelectBox UI component is an editor that allows an end user to select an item from a drop-down list.
 */
export default class dxSelectBox<TProperties = Properties> extends dxDropDownList<TProperties> { }

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface SelectBoxInstance extends dxSelectBox<Properties> { }

export type Properties = dxSelectBoxOptions<SelectBoxInstance>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = Properties;
