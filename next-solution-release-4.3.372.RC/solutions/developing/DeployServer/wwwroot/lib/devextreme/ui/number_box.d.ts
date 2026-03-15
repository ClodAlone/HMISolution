/**
* DevExtreme (ui/number_box.d.ts)
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
} from '../events/index';

import dxTextEditor, {
    dxTextEditorButton,
    dxTextEditorOptions,
} from './text_box/ui.text_editor.base';

import {
    ValueChangedInfo,
} from './editor/editor';

import {
    Format,
  } from '../localization';

export type ChangeEvent = NativeEventInfo<dxNumberBox, Event>;

export type ContentReadyEvent = EventInfo<dxNumberBox>;

export type CopyEvent = NativeEventInfo<dxNumberBox, ClipboardEvent>;

export type CutEvent = NativeEventInfo<dxNumberBox, ClipboardEvent>;

export type DisposingEvent = EventInfo<dxNumberBox>;

export type EnterKeyEvent = NativeEventInfo<dxNumberBox, KeyboardEvent>;

export type FocusInEvent = NativeEventInfo<dxNumberBox, FocusEvent>;

export type FocusOutEvent = NativeEventInfo<dxNumberBox, FocusEvent>;

export type InitializedEvent = InitializedEventInfo<dxNumberBox>;

export type InputEvent = NativeEventInfo<dxNumberBox, UIEvent>;

export type KeyDownEvent = NativeEventInfo<dxNumberBox, KeyboardEvent>;

export type KeyPressEvent = NativeEventInfo<dxNumberBox, KeyboardEvent>;

export type KeyUpEvent = NativeEventInfo<dxNumberBox, KeyboardEvent>;

export type OptionChangedEvent = EventInfo<dxNumberBox> & ChangedOptionInfo;

export type PasteEvent = NativeEventInfo<dxNumberBox, ClipboardEvent>;

export type ValueChangedEvent = NativeEventInfo<dxNumberBox, KeyboardEvent | MouseEvent | PointerEvent | TouchEvent | Event> & ValueChangedInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxNumberBoxOptions extends dxTextEditorOptions<dxNumberBox> {
    /**
     * Allows you to add custom buttons to the input text field.
     */
    buttons?: Array<'clear' | 'spins' | dxTextEditorButton>;
    /**
     * Specifies the value&apos;s display format and controls user input accordingly.
     */
    format?: Format;
    /**
     * Specifies the text of the message displayed if the specified value is not a number.
     */
    invalidValueMessage?: string;
    /**
     * The maximum value accepted by the number box.
     */
    max?: number;
    /**
     * The minimum value accepted by the number box.
     */
    min?: number;
    /**
     * Specifies the value to be passed to the type attribute of the underlying `` element.
     */
    mode?: 'number' | 'text' | 'tel';
    /**
     * Specifies whether to show the buttons that change the value by a step.
     */
    showSpinButtons?: boolean;
    /**
     * Specifies how much the UI component&apos;s value changes when using the spin buttons, Up/Down arrow keys, or mouse wheel.
     */
    step?: number;
    /**
     * Specifies whether to use touch friendly spin buttons. Applies only if showSpinButtons is true.
     */
    useLargeSpinButtons?: boolean;
    /**
     * The current number box value.
     */
    value?: number;
}
/**
 * The NumberBox is a UI component that displays a numeric value and allows a user to modify it by typing in a value, and incrementing or decrementing it using the keyboard or mouse.
 */
export default class dxNumberBox extends dxTextEditor<dxNumberBoxOptions> { }

export type Properties = dxNumberBoxOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxNumberBoxOptions;
