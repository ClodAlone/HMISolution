/**
* DevExtreme (ui/drop_down_editor/ui.drop_down_editor.d.ts)
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

import dxTextBox, {
    dxTextBoxOptions,

} from '../text_box';

import {
    dxTextEditorButton,
} from '../text_box/ui.text_editor.base';

import {
    Properties as PopoverProperties,
} from '../popover';

import {
    Properties as PopupProperties,
} from '../popup';

import {
    EventInfo,
} from '../../events/index';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DropDownButtonTemplateDataModel {
    readonly text?: string;
    readonly icon?: string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDropDownEditorOptions<TComponent> extends dxTextBoxOptions<TComponent> {
    /**
     * Specifies whether or not the UI component allows an end-user to enter a custom value.
     */
    acceptCustomValue?: boolean;
    /**
     * Specifies whether or not the UI component changes its state when interacting with a user.
     */
    activeStateEnabled?: boolean;
    /**
     * Specifies the way an end-user applies the selected value.
     */
    applyValueMode?: 'instantly' | 'useButtons';
    /**
     * Configures the drop-down field which holds the content.
     */
    dropDownOptions?: PopupProperties | PopoverProperties;
    /**
     * Allows you to add custom buttons to the input text field.
     */
    buttons?: Array<'clear' | 'dropDown' | dxTextEditorButton>;
    /**
     * Specifies whether to render the drop-down field&apos;s content when it is displayed. If false, the content is rendered immediately.
     */
    deferRendering?: boolean;
    /**
     * Specifies a custom template for the drop-down button.
     */
    dropDownButtonTemplate?: template | ((buttonData: DropDownButtonTemplateDataModel, contentElement: DxElement) => string | UserDefinedElement);
    /**
     * A function that is executed once the drop-down editor is closed.
     */
    onClosed?: ((e: EventInfo<TComponent>) => void);
    /**
     * A function that is executed once the drop-down editor is opened.
     */
    onOpened?: ((e: EventInfo<TComponent>) => void);
    /**
     * Specifies whether a user can open the drop-down list by clicking a text field.
     */
    openOnFieldClick?: boolean;
    /**
     * Specifies whether or not the drop-down editor is displayed.
     */
    opened?: boolean;
    /**
     * Specifies whether the drop-down button is visible.
     */
    showDropDownButton?: boolean;
    /**
     * Specifies the currently selected value.
     */
    value?: any;
}
/**
 * A drop-down editor UI component.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class dxDropDownEditor<TProperties> extends dxTextBox<TProperties> {
    /**
     * Closes the drop-down editor.
     */
    close(): void;
    /**
     * Gets the popup window&apos;s content.
     */
    content(): DxElement;
    /**
     * Gets the UI component&apos;s `` element.
     */
    field(): DxElement;
    /**
     * Opens the drop-down editor.
     */
    open(): void;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface DropDownEditorInstance extends dxDropDownEditor<Properties> { }
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type Properties = dxDropDownEditorOptions<DropDownEditorInstance>;
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = Properties;
