/**
* DevExtreme (ui/filter_builder.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DataSource, { DataSourceLike } from '../data/data_source';
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
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

import {
    Format,
} from '../localization';

export type ContentReadyEvent = EventInfo<dxFilterBuilder>;

export type DisposingEvent = EventInfo<dxFilterBuilder>;

export type EditorPreparedEvent = EventInfo<dxFilterBuilder> & {
    readonly value?: any;
    readonly setValue: any;
    readonly editorElement: DxElement;
    readonly editorName: string;
    readonly dataField?: string;
    readonly filterOperation?: string;
    readonly updateValueTimeout?: number;
    readonly width?: number;
    readonly readOnly: boolean;
    readonly disabled: boolean;
    readonly rtlEnabled: boolean;
};

export type EditorPreparingEvent = Cancelable & EventInfo<dxFilterBuilder> & {
    readonly value?: any;
    readonly setValue: any;
    readonly editorElement?: DxElement;
    editorName: string;
    editorOptions?: any;
    readonly dataField?: string;
    readonly filterOperation?: string;
    updateValueTimeout?: number;
    readonly width?: number;
    readonly readOnly: boolean;
    readonly disabled: boolean;
    readonly rtlEnabled: boolean;
};

export type InitializedEvent = InitializedEventInfo<dxFilterBuilder>;

export type OptionChangedEvent = EventInfo<dxFilterBuilder> & ChangedOptionInfo;

export type ValueChangedEvent = EventInfo<dxFilterBuilder> & {
    readonly value?: any;
    readonly previousValue?: any;
};

export type CustomOperationEditorTemplate = {
    readonly value?: string | number | Date;
    readonly field: Field;
    readonly setValue: Function;
};

export type FieldEditorTemplate = {
    readonly value?: string | number | Date;
    readonly filterOperation?: string;
    readonly field: Field;
    readonly setValue: Function;
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFilterBuilderOptions extends WidgetOptions<dxFilterBuilder> {
    /**
     * Specifies whether the UI component can display hierarchical data fields.
     */
    allowHierarchicalFields?: boolean;
    /**
     * Configures custom filter operations.
     */
    customOperations?: Array<CustomOperation>;
    /**
     * Configures fields.
     */
    fields?: Array<Field>;
    /**
     * Specifies filter operation descriptions.
     */
    filterOperationDescriptions?: {
      /**
       * The &apos;between&apos; operation&apos;s description.
       */
      between?: string;
      /**
       * The &apos;contains&apos; operation&apos;s description.
       */
      contains?: string;
      /**
       * The &apos;endswith&apos; operation&apos;s description.
       */
      endsWith?: string;
      /**
       * The &apos;=&apos; operation&apos;s description.
       */
      equal?: string;
      /**
       * The &apos;&gt;&apos; operation&apos;s description.
       */
      greaterThan?: string;
      /**
       * The &apos;&gt;=&apos; operation&apos;s description.
       */
      greaterThanOrEqual?: string;
      /**
       * The &apos;isblank&apos; operation&apos;s description.
       */
      isBlank?: string;
      /**
       * The &apos;isnotblank&apos; operation&apos;s description.
       */
      isNotBlank?: string;
      /**
       * The &apos;&lt;&apos; operation&apos;s description.
       */
      lessThan?: string;
      /**
       * The &apos;&lt;=&apos; operation&apos;s description.
       */
      lessThanOrEqual?: string;
      /**
       * The &apos;notcontains&apos; operation&apos;s description.
       */
      notContains?: string;
      /**
       * The &apos;&lt;&gt;&apos; operation&apos;s description.
       */
      notEqual?: string;
      /**
       * The &apos;startswith&apos; operation&apos;s description.
       */
      startsWith?: string;
    };
    /**
     * Specifies group operation descriptions.
     */
    groupOperationDescriptions?: {
      /**
       * The &apos;and&apos; operation&apos;s description.
       */
      and?: string;
      /**
       * The &apos;notand&apos; operation&apos;s description.
       */
      notAnd?: string;
      /**
       * The &apos;notor&apos; operation&apos;s description.
       */
      notOr?: string;
      /**
       * The &apos;or&apos; operation&apos;s description.
       */
      or?: string;
    };
    /**
     * Specifies a set of available group operations.
     */
    groupOperations?: Array<'and' | 'or' | 'notAnd' | 'notOr'>;
    /**
     * Specifies groups&apos; maximum nesting level.
     */
    maxGroupLevel?: number;
    /**
     * A function that is executed after an editor is created.
     */
    onEditorPrepared?: ((e: EditorPreparedEvent) => void);
    /**
     * A function that is executed before an editor is created.
     */
    onEditorPreparing?: ((e: EditorPreparingEvent) => void);
    /**
     * A function that is executed after the UI component&apos;s value is changed.
     */
    onValueChanged?: ((e: ValueChangedEvent) => void);
    /**
     * Allows you to specify a filter.
     */
    value?: string | Array<any> | Function;
}
/**
 * The FilterBuilder UI component allows a user to build complex filter expressions with an unlimited number of filter conditions, combined by logical operations using the UI.
 */
export default class dxFilterBuilder extends Widget<dxFilterBuilderOptions> {
    /**
     * Gets a filter expression that contains only operations supported by the DataSource.
     */
    getFilterExpression(): string | Array<any> | Function;
}

export type CustomOperation = dxFilterBuilderCustomOperation;

/**
 * @deprecated Use the CustomOperation type instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFilterBuilderCustomOperation {
    /**
     * Specifies a function that returns a filter expression for this custom operation.
     */
    calculateFilterExpression?: ((filterValue: any, field: Field) => string | Array<any> | Function);
    /**
     * Specifies the operation&apos;s caption.
     */
    caption?: string;
    /**
     * Customizes the field value&apos;s text representation.
     */
    customizeText?: ((fieldInfo: { value?: string | number | Date; valueText?: string; field?: Field }) => string);
    /**
     * Specifies for which data types the operation is available by default.
     */
    dataTypes?: Array<'string' | 'number' | 'date' | 'boolean' | 'object' | 'datetime'>;
    /**
     * Specifies a custom template for the UI component used to edit the field value.
     */
    editorTemplate?: template | ((conditionInfo: CustomOperationEditorTemplate, container: DxElement) => string | UserDefinedElement);
    /**
     * Specifies whether the operation can have a value. If it can, the editor is displayed.
     */
    hasValue?: boolean;
    /**
     * Specifies the icon that should represent the filter operation.
     */
    icon?: string;
    /**
     * Specifies the operation&apos;s identifier.
     */
    name?: string;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type FilterLookupDataSource<T> = Exclude<DataSourceLike<T>, string | DataSource>;

export type Field = dxFilterBuilderField;

/**
 * @deprecated Use the Field type instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFilterBuilderField {
    /**
     * Specifies the field&apos;s custom rules to filter data.
     */
    calculateFilterExpression?: ((filterValue: any, selectedFilterOperation: string) => string | Array<any> | Function);
    /**
     * Specifies the data field&apos;s caption.
     */
    caption?: string;
    /**
     * Customizes the input value&apos;s display text.
     */
    customizeText?: ((fieldInfo: { value?: string | number | Date; valueText?: string }) => string);
    /**
     * Specifies the name of a field to be filtered.
     */
    dataField?: string;
    /**
     * Casts field values to a specific data type.
     */
    dataType?: 'string' | 'number' | 'date' | 'boolean' | 'object' | 'datetime';
    /**
     * Configures the UI component used to edit the field value.
     */
    editorOptions?: any;
    /**
     * Specifies the editor&apos;s custom template.
     */
    editorTemplate?: template | ((conditionInfo: FieldEditorTemplate, container: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the false value text. Applies only if dataType is &apos;boolean&apos;.
     */
    falseText?: string;
    /**
     * Specifies a set of available filter operations.
     */
    filterOperations?: Array<'=' | '<>' | '<' | '<=' | '>' | '>=' | 'contains' | 'endswith' | 'isblank' | 'isnotblank' | 'notcontains' | 'startswith' | 'between' | string>;
    /**
     * Formats a value before it is displayed.
     */
    format?: Format;
    /**
     * Configures the lookup field.
     */
    lookup?: {
      /**
       * Specifies whether to display the Clear button in the lookup field while it is being edited.
       */
      allowClearing?: boolean;
      /**
       * Specifies the lookup data source.
       */
      dataSource?: FilterLookupDataSource<any>;
      /**
       * Specifies the data field whose values should be displayed.
       */
      displayExpr?: string | ((data: any) => string);
      /**
       * Specifies the data field whose values should be replaced with values from the displayExpr field.
       */
      valueExpr?: string | ((data: any) => string | number | boolean);
    };
    /**
     * Specifies the field&apos;s name. Use it to distinguish the field from other fields when they have identical dataField values.
     */
    name?: string;
    /**
     * Specifies the true value text. Applies only if dataType is &apos;boolean&apos;.
     */
    trueText?: string;
}

export type Properties = dxFilterBuilderOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxFilterBuilderOptions;
