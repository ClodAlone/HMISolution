/**
* DevExtreme (ui/color_box.d.ts)
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
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import {
    template,
} from '../core/templates/template';

import dxDropDownEditor, {
    dxDropDownEditorOptions,
    DropDownButtonTemplateDataModel,
} from './drop_down_editor/ui.drop_down_editor';

import {
    ValueChangedInfo,
} from './editor/editor';

import {
    Properties as PopupProperties,
} from './popup';

export type ChangeEvent = NativeEventInfo<dxColorBox, Event>;

export type ClosedEvent = EventInfo<dxColorBox>;

export type CopyEvent = NativeEventInfo<dxColorBox, ClipboardEvent>;

export type CutEvent = NativeEventInfo<dxColorBox, ClipboardEvent>;

export type DisposingEvent = EventInfo<dxColorBox>;

export type EnterKeyEvent = NativeEventInfo<dxColorBox, KeyboardEvent>;

export type FocusInEvent = NativeEventInfo<dxColorBox, FocusEvent>;

export type FocusOutEvent = NativeEventInfo<dxColorBox, FocusEvent>;

export type InitializedEvent = InitializedEventInfo<dxColorBox>;

export type InputEvent = NativeEventInfo<dxColorBox, UIEvent>;

export type KeyDownEvent = NativeEventInfo<dxColorBox, KeyboardEvent>;

export type KeyPressEvent = NativeEventInfo<dxColorBox, KeyboardEvent>;

export type KeyUpEvent = NativeEventInfo<dxColorBox, KeyboardEvent>;

export type OpenedEvent = EventInfo<dxColorBox>;

export type OptionChangedEvent = EventInfo<dxColorBox> & ChangedOptionInfo;

export type PasteEvent = NativeEventInfo<dxColorBox, ClipboardEvent>;

export type ValueChangedEvent = NativeEventInfo<dxColorBox, KeyboardEvent | MouseEvent | PointerEvent | UIEvent | Event> & ValueChangedInfo;

export type DropDownButtonTemplateData = DropDownButtonTemplateDataModel;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxColorBoxOptions extends dxDropDownEditorOptions<dxColorBox> {
    /**
     * Specifies the text displayed on the button that applies changes and closes the drop-down editor.
     */
    applyButtonText?: string;
    /**
     * Specifies the way an end-user applies the selected value.
     */
    applyValueMode?: 'instantly' | 'useButtons';
    /**
     * Specifies the text displayed on the button that cancels changes and closes the drop-down editor.
     */
    cancelButtonText?: string;
    /**
     * Specifies whether or not the UI component value includes the alpha channel component.
     */
    editAlphaChannel?: boolean;
    /**
     * Specifies a custom template for the input field. Must contain the TextBox UI component.
     */
    fieldTemplate?: template | ((value: string, fieldElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the size of a step by which a handle is moved using a keyboard shortcut.
     */
    keyStep?: number;
    /**
     * Specifies the currently selected value.
     */
    value?: string;

    /**
     * Configures the drop-down field which holds the content.
     */
    dropDownOptions?: PopupProperties;
}
/**
 * The ColorBox is a UI component that allows an end user to enter a color or pick it out from the drop-down editor.
 */
export default class dxColorBox extends dxDropDownEditor<dxColorBoxOptions> { }

export type Properties = dxColorBoxOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxColorBoxOptions;
