/**
* DevExtreme (ui/drop_down_box.d.ts)
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

import DataSource, { DataSourceLike } from '../data/data_source';

import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import dxDropDownEditor, {
    dxDropDownEditorOptions,
    DropDownButtonTemplateDataModel,
} from './drop_down_editor/ui.drop_down_editor';

import {
    ValueChangedInfo,
} from './editor/editor';

import {
    DataExpressionMixinOptions,
} from './editor/ui.data_expression';

import {
    Properties as PopupProperties,
} from './popup';

export type ChangeEvent = NativeEventInfo<dxDropDownBox, Event>;

export type ClosedEvent = EventInfo<dxDropDownBox>;

export type CopyEvent = NativeEventInfo<dxDropDownBox, ClipboardEvent>;

export type CutEvent = NativeEventInfo<dxDropDownBox, ClipboardEvent>;

export type DisposingEvent = EventInfo<dxDropDownBox>;

export type EnterKeyEvent = NativeEventInfo<dxDropDownBox, KeyboardEvent>;

export type FocusInEvent = NativeEventInfo<dxDropDownBox, FocusEvent>;

export type FocusOutEvent = NativeEventInfo<dxDropDownBox, FocusEvent>;

export type InitializedEvent = InitializedEventInfo<dxDropDownBox>;

export type InputEvent = NativeEventInfo<dxDropDownBox, UIEvent>;

export type KeyDownEvent = NativeEventInfo<dxDropDownBox, KeyboardEvent>;

export type KeyPressEvent = NativeEventInfo<dxDropDownBox, KeyboardEvent>;

export type KeyUpEvent = NativeEventInfo<dxDropDownBox, KeyboardEvent>;

export type OpenedEvent = EventInfo<dxDropDownBox>;

export type OptionChangedEvent = EventInfo<dxDropDownBox> & ChangedOptionInfo;

export type PasteEvent = NativeEventInfo<dxDropDownBox, ClipboardEvent>;

export type ValueChangedEvent = NativeEventInfo<dxDropDownBox, KeyboardEvent | MouseEvent | PointerEvent | Event> & ValueChangedInfo;

export type ContentTemplateData = {
    component: dxDropDownBox;
    readonly value?: any;
};

export type DropDownButtonTemplateData = DropDownButtonTemplateDataModel;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDropDownBoxOptions extends DataExpressionMixinOptions<dxDropDownBox>, dxDropDownEditorOptions<dxDropDownBox> {
    /**
     * Specifies whether the UI component allows a user to enter a custom value.
     */
    acceptCustomValue?: boolean;
    /**
     * Specifies a custom template for the drop-down content.
     */
    contentTemplate?: template | ((templateData: ContentTemplateData, contentElement: DxElement) => string | UserDefinedElement);
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<any>;
    /**
     * Customizes text before it is displayed in the input field.
     */
    displayValueFormatter?: ((value: string | Array<any>) => string);
    /**
     * Specifies a custom template for the text field. Must contain the TextBox UI component.
     */
    fieldTemplate?: template | ((value: any, fieldElement: DxElement) => string | UserDefinedElement);
    /**
     * An array of items used to synchronize the DropDownBox with an embedded UI component.
     */
    items?: Array<any>;
    /**
     * Specifies whether a user can open the drop-down list by clicking a text field.
     */
    openOnFieldClick?: boolean;
    /**
     * Specifies the DOM events after which the UI component&apos;s value should be updated.
     */
    valueChangeEvent?: string;

    /**
     * Configures the drop-down field which holds the content.
     */
    dropDownOptions?: PopupProperties;
}
/**
 * The DropDownBox UI component consists of a text field, which displays the current value, and a drop-down field, which can contain any UI element.
 */
export default class dxDropDownBox extends dxDropDownEditor<dxDropDownBoxOptions> {
    getDataSource(): DataSource;
}

export type Properties = dxDropDownBoxOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxDropDownBoxOptions;
