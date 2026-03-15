/**
* DevExtreme (ui/form.d.ts)
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
    DxPromise,
} from '../core/utils/deferred';

import {
    template,
} from '../core/templates/template';

import {
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import dxButton, {
    dxButtonOptions,
} from './button';

import Editor from './editor/editor';

import {
    dxTabPanelOptions,
} from './tab_panel';

import {
    AsyncRule,
    CompareRule,
    CustomRule,
    EmailRule,
    NumericRule,
    PatternRule,
    RangeRule,
    RequiredRule,
    StringLengthRule,
} from './validation_rules';

import {
    ValidationResult,
} from './validation_group';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ContentReadyEvent = EventInfo<dxForm>;

export type DisposingEvent = EventInfo<dxForm>;

export type EditorEnterKeyEvent = EventInfo<dxForm> & {
    readonly dataField?: string;
};

export type FieldDataChangedEvent = EventInfo<dxForm> & {
    readonly dataField?: string;
    readonly value?: any;
};

export type InitializedEvent = InitializedEventInfo<dxForm>;

export type OptionChangedEvent = EventInfo<dxForm> & ChangedOptionInfo;

export type GroupItemTemplateData = {
    readonly component: dxForm;
    readonly formData?: any;
};

export type SimpleItemTemplateData = {
    readonly component: dxForm;
    readonly dataField?: string;
    readonly editorOptions?: any;
    readonly editorType?: string;
    readonly name?: string;
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFormOptions extends WidgetOptions<dxForm> {
    /**
     * Specifies whether all item labels are aligned. Applies only to labels outside their editors (see labelMode).
     */
    alignItemLabels?: boolean;
    /**
     * Specifies whether item labels in all groups are aligned. Applies only to labels outside their editors (see labelMode).
     */
    alignItemLabelsInAllGroups?: boolean;
    /**
     * The count of columns in the form layout.
     */
    colCount?: number | 'auto';
    /**
     * Specifies dependency between the screen factor and the count of columns in the form layout.
     */
    colCountByScreen?: any;
    /**
     * Specifies a function that customizes a form item after it has been created.
     */
    customizeItem?: ((item: Item) => void);
    /**
     * Provides the Form&apos;s data. Gets updated every time form fields change.
     */
    formData?: any;
    /**
     * Holds an array of form items.
     */
    items?: Array<Item>;
    /**
     * Specifies the location of a label against the editor. Applies only to labels outside their editors (see labelMode).
     */
    labelLocation?: 'left' | 'right' | 'top';
    /**
      * Specifies a display mode for item labels.
      */
     labelMode?: 'outside' | 'floating' | 'static' | 'hidden';
    /**
     * The minimum column width used for calculating column count in the form layout. Applies only if colCount property is &apos;auto&apos;.
     */
    minColWidth?: number;
    /**
     * A function that is executed when the Enter key has been pressed while an editor is focused.
     */
    onEditorEnterKey?: ((e: EditorEnterKeyEvent) => void);
    /**
     * A function that is executed when the value of a formData object field is changed.
     */
    onFieldDataChanged?: ((e: FieldDataChangedEvent) => void);
    /**
     * The text displayed for optional fields. Applies only if showOptionalMark is true.
     */
    optionalMark?: string;
    /**
     * Specifies whether all editors on the form are read-only. Applies only to non-templated items.
     */
    readOnly?: boolean;
    /**
     * The text displayed for required fields.
     */
    requiredMark?: string;
    /**
     * Specifies the message that is shown for end-users if a required field value is not specified.
     */
    requiredMessage?: string;
    /**
     * Specifies a function that categorizes screens by their width.
     */
    screenByWidth?: Function;
    /**
     * A Boolean value specifying whether to enable or disable form scrolling.
     */
    scrollingEnabled?: boolean;
    /**
     * Specifies whether a colon is displayed at the end of form labels. Applies only to labels outside their editors (see labelMode).
     */
    showColonAfterLabel?: boolean;
    /**
     * Specifies whether or not the optional mark is displayed for optional fields.
     */
    showOptionalMark?: boolean;
    /**
     * Specifies whether or not the required mark is displayed for required fields.
     */
    showRequiredMark?: boolean;
    /**
     * Specifies whether or not the total validation summary is displayed on the form.
     */
    showValidationSummary?: boolean;
    /**
     * Gives a name to the internal validation group.
     */
    validationGroup?: string;
}
/**
 * The Form UI component represents fields of a data object as a collection of label-editor pairs. These pairs can be arranged in several groups, tabs and columns.
 */
export default class dxForm extends Widget<dxFormOptions> {
    /**
     * Gets a button&apos;s instance.
     */
    getButton(name: string): dxButton | undefined;
    /**
     * Gets an editor instance. Takes effect only if the form item is visible.
     */
    getEditor(dataField: string): Editor | undefined;
    /**
     * Gets a form item&apos;s configuration.
     */
    itemOption(id: string): any;
    /**
     * Updates the value of a single item option.
     */
    itemOption(id: string, option: string, value: any): void;
    /**
     * Updates the values of several item properties.
     */
    itemOption(id: string, options: any): void;
    /**
     * Resets the editor&apos;s value to undefined.
     */
    resetValues(): void;
    /**
     * Merges the passed `data` object with formData. Matching properties in formData are overwritten and new properties added.
     */
    updateData(data: any): void;
    /**
     * Updates a formData field and the corresponding editor.
     */
    updateData(dataField: string, value: any): void;
    /**
     * Updates the dimensions of the UI component contents.
     */
    updateDimensions(): DxPromise<void>;
    /**
     * Validates the values of all editors on the form against the list of the validation rules specified for each form item.
     */
    validate(): ValidationResult;
}

export type Item = SimpleItem | GroupItem | TabbedItem | EmptyItem | ButtonItem;

export type ButtonItem = dxFormButtonItem;

/**
 * @deprecated Use ButtonItem instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFormButtonItem {
    /**
     * Configures the button.
     */
    buttonOptions?: dxButtonOptions;
    /**
     * Specifies how many columns the item spans.
     */
    colSpan?: number;
    /**
     * Specifies a CSS class to be applied to the item.
     */
    cssClass?: string;
    /**
     * Specifies the button&apos;s horizontal alignment.
     */
    horizontalAlignment?: 'center' | 'left' | 'right';
    /**
     * Specifies the item&apos;s type. Set it to &apos;button&apos; to create a button item.
     */
    itemType?: 'empty' | 'group' | 'simple' | 'tabbed' | 'button';
    /**
     * Specifies the item&apos;s identifier.
     */
    name?: string;
    /**
     * Specifies the button&apos;s vertical alignment.
     */
    verticalAlignment?: 'bottom' | 'center' | 'top';
    /**
     * Specifies whether the item is visible.
     */
    visible?: boolean;
    /**
     * Specifies the item&apos;s position regarding other items in a group, tab, or the whole UI component.
     */
    visibleIndex?: number;
}

export type EmptyItem = dxFormEmptyItem;

/**
 * @deprecated Use EmptyItem instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFormEmptyItem {
    /**
     * Specifies the number of columns spanned by the item.
     */
    colSpan?: number;
    /**
     * Specifies a CSS class to be applied to the form item.
     */
    cssClass?: string;
    /**
     * Specifies the item&apos;s type. Set it to &apos;empty&apos; to create an empty item.
     */
    itemType?: 'empty' | 'group' | 'simple' | 'tabbed' | 'button';
    /**
     * Specifies a name that identifies the form item.
     */
    name?: string;
    /**
     * Specifies whether or not the current form item is visible.
     */
    visible?: boolean;
    /**
     * Specifies the sequence number of the item in a form, group or tab.
     */
    visibleIndex?: number;
}

export type GroupItem = dxFormGroupItem;

/**
 * @deprecated Use GroupItem instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFormGroupItem {
    /**
     * Specifies whether or not all group item labels are aligned.
     */
    alignItemLabels?: boolean;
    /**
     * Specifies the group caption.
     */
    caption?: string;
    /**
     * The count of columns in the group layout.
     */
    colCount?: number;
    /**
     * Specifies the relation between the screen size qualifier and the number of columns in the grouped layout.
     */
    colCountByScreen?: any;
    /**
     * Specifies the number of columns spanned by the item.
     */
    colSpan?: number;
    /**
     * Specifies a CSS class to be applied to the form item.
     */
    cssClass?: string;
    /**
     * Specifies the item&apos;s type. Set it to &apos;group&apos; to create a group item.
     */
    itemType?: 'empty' | 'group' | 'simple' | 'tabbed' | 'button';
    /**
     * Holds an array of form items displayed within the group.
     */
    items?: Array<Item>;
    /**
     * Specifies a name that identifies the form item.
     */
    name?: string;
    /**
     * A template to be used for rendering a group item.
     */
    template?: template | ((data: GroupItemTemplateData, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies whether or not the current form item is visible.
     */
    visible?: boolean;
    /**
     * Specifies the sequence number of the item in a form, group or tab.
     */
    visibleIndex?: number;
}

export type SimpleItem = dxFormSimpleItem;

/**
 * @deprecated Use SimpleItem instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFormSimpleItem {
    /**
     * Specifies the number of columns spanned by the item.
     */
    colSpan?: number;
    /**
     * Specifies a CSS class to be applied to the form item.
     */
    cssClass?: string;
    /**
     * Specifies the path to the formData object field bound to the current form item.
     */
    dataField?: string;
    /**
     * Configures the form item&apos;s editor.
     */
    editorOptions?: any;
    /**
     * Specifies which editor UI component is used to display and edit the form item value.
     */
    editorType?: 'dxAutocomplete' | 'dxCalendar' | 'dxCheckBox' | 'dxColorBox' | 'dxDateBox' | 'dxDropDownBox' | 'dxHtmlEditor' | 'dxLookup' | 'dxNumberBox' | 'dxRadioGroup' | 'dxRangeSlider' | 'dxSelectBox' | 'dxSlider' | 'dxSwitch' | 'dxTagBox' | 'dxTextArea' | 'dxTextBox';
    /**
     * Specifies the help text displayed for the current form item.
     */
    helpText?: string;
    /**
     * Specifies whether the current form item is required.
     */
    isRequired?: boolean;
    /**
     * Specifies the item&apos;s type. Set it to &apos;simple&apos; to create a simple item.
     */
    itemType?: 'empty' | 'group' | 'simple' | 'tabbed' | 'button';
    /**
     * Specifies properties for the form item label.
     */
    label?: {
      /**
       * Specifies the label&apos;s horizontal alignment. Applies only to labels outside their editors (see labelMode).
       */
      alignment?: 'center' | 'left' | 'right';
      /**
       * Specifies the location of a label against the editor. Applies only to labels outside their editors (see labelMode).
       */
      location?: 'left' | 'right' | 'top';
      /**
       * Specifies whether a colon is displayed at the end of the current label. Applies only to labels outside their editors (see labelMode).
       */
      showColon?: boolean;
      /**
       * Specifies the label text.
       */
      text?: string;
      /**
       * Controls the visibility of the label outside the editor.
       */
      visible?: boolean;
    };
    /**
     * Specifies a name that identifies the form item.
     */
    name?: string;
    /**
     * A template that can be used to replace the default editor with custom content.
     */
    template?: template | ((data: SimpleItemTemplateData, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * An array of validation rules to be checked for the form item editor.
     */
    validationRules?: Array<RequiredRule | NumericRule | RangeRule | StringLengthRule | CustomRule | CompareRule | PatternRule | EmailRule | AsyncRule>;
    /**
     * Specifies whether or not the current form item is visible.
     */
    visible?: boolean;
    /**
     * Specifies the sequence number of the item in a form, group or tab.
     */
    visibleIndex?: number;
}

export type TabbedItem = dxFormTabbedItem;

/**
 * @deprecated Use TabbedItem instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFormTabbedItem {
    /**
     * Specifies the number of columns spanned by the item.
     */
    colSpan?: number;
    /**
     * Specifies a CSS class to be applied to the form item.
     */
    cssClass?: string;
    /**
     * Specifies the item&apos;s type. Set it to &apos;tabbed&apos; to create a tabbed item.
     */
    itemType?: 'empty' | 'group' | 'simple' | 'tabbed' | 'button';
    /**
     * Specifies a name that identifies the form item.
     */
    name?: string;
    /**
     * Holds a configuration object for the TabPanel UI component used to display the current form item.
     */
    tabPanelOptions?: dxTabPanelOptions;
    /**
     * An array of tab configuration objects.
     */
    tabs?: Array<{
      /**
       * Specifies whether or not labels of items displayed within the current tab are aligned.
       */
      alignItemLabels?: boolean;
      /**
       * Specifies a badge text for the tab.
       */
      badge?: string;
      /**
       * The count of columns in the tab layout.
       */
      colCount?: number;
      /**
       * Specifies the relation between the screen size qualifier and the number of columns in the tabbed layout.
       */
      colCountByScreen?: any;
      /**
       * Specifies whether the tab responds to user interaction.
       */
      disabled?: boolean;
      /**
       * Specifies the icon to be displayed on the tab.
       */
      icon?: string;
      /**
       * Holds an array of form items displayed within the tab.
       */
      items?: Array<Item>;
      /**
       * The template to be used for rendering the tab.
       */
      tabTemplate?: template | ((tabData: any, tabIndex: number, tabElement: DxElement) => any);
      /**
       * The template to be used for rendering the tab content.
       */
      template?: template | ((tabData: any, tabIndex: number, tabElement: DxElement) => any);
      /**
       * Specifies the tab title.
       */
      title?: string;
    }>;
    /**
     * Specifies whether or not the current form item is visible.
     */
    visible?: boolean;
    /**
     * Specifies the sequence number of the item in a form, group or tab.
     */
    visibleIndex?: number;
}

export type Properties = dxFormOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxFormOptions;
