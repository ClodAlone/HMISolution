/**
* DevExtreme (ui/button.d.ts)
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
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ClickEvent = NativeEventInfo<dxButton, KeyboardEvent | MouseEvent | PointerEvent> & {
    validationGroup?: any;
};

export type ContentReadyEvent = EventInfo<dxButton>;

export type DisposingEvent = EventInfo<dxButton>;

export type InitializedEvent = InitializedEventInfo<dxButton>;

export type OptionChangedEvent = EventInfo<dxButton> & ChangedOptionInfo;

export type TemplateData = {
    readonly text?: string;
    readonly icon?: string;
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxButtonOptions extends WidgetOptions<dxButton> {
    /**
     * A Boolean value specifying whether or not the UI component changes its state when interacting with a user.
     */
    activeStateEnabled?: boolean;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies whether the UI component changes its state when a user pauses on it.
     */
    hoverStateEnabled?: boolean;
    /**
     * Specifies the icon to be displayed on the button.
     */
    icon?: string;
    /**
     * A function that is executed when the Button is clicked or tapped.
     */
    onClick?: ((e: ClickEvent) => void);
    /**
     * Specifies how the button is styled.
     */
    stylingMode?: 'text' | 'outlined' | 'contained';
    /**
     * Specifies a custom template for the Button UI component.
     */
    template?: template | ((data: TemplateData, contentElement: DxElement) => string | UserDefinedElement);
    /**
     * The text displayed on the button.
     */
    text?: string;
    /**
     * Specifies the button type.
     */
    type?: 'back' | 'danger' | 'default' | 'normal' | 'success';
    /**
     * Specifies whether the button submits an HTML form.
     */
    useSubmitBehavior?: boolean;
    /**
     * Specifies the name of the validation group to be accessed in the click event handler.
     */
    validationGroup?: string;
}
/**
 * The Button UI component is a simple button that performs specified commands when a user clicks it.
 */
export default class dxButton extends Widget<dxButtonOptions> { }

export type Properties = dxButtonOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxButtonOptions;
