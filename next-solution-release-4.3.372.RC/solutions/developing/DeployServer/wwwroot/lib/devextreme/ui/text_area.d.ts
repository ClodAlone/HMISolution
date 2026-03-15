/**
* DevExtreme (ui/text_area.d.ts)
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

import {
    ValueChangedInfo,
} from './editor/editor';

import dxTextBox, {
    dxTextBoxOptions,
} from './text_box';

export type ChangeEvent = NativeEventInfo<dxTextArea, Event>;

export type ContentReadyEvent = EventInfo<dxTextArea>;

export type CopyEvent = NativeEventInfo<dxTextArea, ClipboardEvent>;

export type CutEvent = NativeEventInfo<dxTextArea, ClipboardEvent>;

export type DisposingEvent = EventInfo<dxTextArea>;

export type EnterKeyEvent = NativeEventInfo<dxTextArea, KeyboardEvent>;

export type FocusInEvent = NativeEventInfo<dxTextArea, FocusEvent>;

export type FocusOutEvent = NativeEventInfo<dxTextArea, FocusEvent>;

export type InitializedEvent = InitializedEventInfo<dxTextArea>;

export type InputEvent = NativeEventInfo<dxTextArea, UIEvent>;

export type KeyDownEvent = NativeEventInfo<dxTextArea, KeyboardEvent>;

export type KeyPressEvent = NativeEventInfo<dxTextArea, KeyboardEvent>;

export type KeyUpEvent = NativeEventInfo<dxTextArea, KeyboardEvent>;

export type OptionChangedEvent = EventInfo<dxTextArea> & ChangedOptionInfo;

export type PasteEvent = NativeEventInfo<dxTextArea, ClipboardEvent>;

export type ValueChangedEvent = NativeEventInfo<dxTextArea, KeyboardEvent | MouseEvent | PointerEvent | TouchEvent | Event> & ValueChangedInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxTextAreaOptions extends dxTextBoxOptions<dxTextArea> {
    /**
     * A Boolean value specifying whether or not the auto resizing mode is enabled.
     */
    autoResizeEnabled?: boolean;
    /**
     * Specifies the maximum height of the UI component.
     */
    maxHeight?: number | string;
    /**
     * Specifies the minimum height of the UI component.
     */
    minHeight?: number | string;
    /**
     * Specifies whether or not the UI component checks the inner text for spelling mistakes.
     */
    spellcheck?: boolean;
}
/**
 * The TextArea is a UI component that enables a user to enter and edit a multi-line text.
 */
export default class dxTextArea extends dxTextBox<dxTextAreaOptions> { }

export type Properties = dxTextAreaOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxTextAreaOptions;
