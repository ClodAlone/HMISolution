/**
* DevExtreme (ui/editor/editor.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    NativeEventInfo,
} from '../../events/index';

import Widget, {
    WidgetOptions,
} from '../widget/ui.widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ValueChangedInfo {
    readonly previousValue?: any;
    readonly value?: any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface EditorOptions<TComponent> extends WidgetOptions<TComponent> {
    /**
     * Specifies or indicates whether the editor&apos;s value is valid.
     */
    isValid?: boolean;
    /**
     * A function that is executed after the UI component&apos;s value is changed.
     */
    onValueChanged?: ((e: NativeEventInfo<TComponent, Event> & ValueChangedInfo) => void);
    /**
     * Specifies whether the editor is read-only.
     */
    readOnly?: boolean;
    /**
     * Information on the broken validation rule. Contains the first item from the validationErrors array.
     */
    validationError?: any;
    /**
     * An array of the validation rules that failed.
     */
    validationErrors?: Array<any>;
    /**
     * Specifies how the message about the validation rules that are not satisfied by this editor&apos;s value is displayed.
     */
    validationMessageMode?: 'always' | 'auto';
    /**
     * Indicates or specifies the current validation status.
     */
    validationStatus?: 'valid' | 'invalid' | 'pending';
    /**
     * Specifies the UI component&apos;s value.
     */
    value?: any;
    /**
     * 
     */
    stylingMode?: 'outlined' | 'underlined' | 'filled';
}
/**
 * A base class for editors.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class Editor<TProperties = Properties> extends Widget<TProperties> {
    /**
     * Resets the value property to the default value.
     */
    reset(): void;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface EditorInstance extends Editor<Properties> { }

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type Properties = EditorOptions<EditorInstance>;
