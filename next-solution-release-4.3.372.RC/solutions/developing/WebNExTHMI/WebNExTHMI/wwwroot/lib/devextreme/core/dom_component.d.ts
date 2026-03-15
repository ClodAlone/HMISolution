/**
* DevExtreme (core/dom_component.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    Component,
    ComponentOptions,
} from './component';

import {
    UserDefinedElement,
    DxElement,
} from './element';

import {
    ChangedOptionInfo,
    EventInfo,
    InitializedEventInfo,
} from '../events/index';

import { TemplateManager } from './template_manager';
import { FunctionTemplate } from './templates/function_template';
import { DefaultOptionsRule } from './options';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type OptionChangedEventInfo<TComponent> = EventInfo<TComponent> & ChangedOptionInfo;

/* eslint-disable no-underscore-dangle */

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DOMComponentOptions<TComponent> extends ComponentOptions<
    EventInfo<TComponent>,
    InitializedEventInfo<TComponent>,
    OptionChangedEventInfo<TComponent>
> {
    /**
     * 
     */
    bindingOptions?: { [key: string]: any };
    /**
     * Specifies the global attributes to be attached to the UI component&apos;s container element.
     */
    elementAttr?: { [key: string]: any };
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * A function that is executed before the UI component is disposed of.
     */
    onDisposing?: ((e: EventInfo<TComponent>) => void);
    /**
     * A function that is executed after a UI component property is changed.
     */
    onOptionChanged?: ((e: OptionChangedEventInfo<TComponent>) => void);
    /**
     * Switches the UI component to a right-to-left representation.
     */
    rtlEnabled?: boolean;
    /**
     * Specifies the UI component&apos;s width.
     */
    width?: number | string | (() => number | string);
}
/**
 * A base class for all components.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class DOMComponent<TProperties = Properties> extends Component<TProperties> {
    _templateManager: TemplateManager;

    _cancelOptionChange?: string;

    constructor(element: UserDefinedElement, options?: TProperties);

    /**
     * Gets the instance of a UI component found using its DOM node.
     */
    static getInstance(element: UserDefinedElement): DOMComponent<Properties>;

    /**
     * Specifies the device-dependent default configuration properties for this component.
     */
    static defaultOptions<TProperties = Properties>(rule: DefaultOptionsRule<TProperties>): void;

    /**
     * Disposes of all the resources allocated to the widget instance.
     */
    dispose(): void;
    /**
     * Gets the root UI component element.
     */
    element(): DxElement;

    $element(): UserDefinedElement;
    _getTemplate(template: unknown): FunctionTemplate;
    _invalidate(): void;
    _refresh(): void;
    _notifyOptionChanged(fullName: string, value: unknown, previousValue: unknown): void;
    _createElement(element: HTMLElement): void;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type ComponentClass<TProperties> = {
    new(element: HTMLDivElement, options?: TProperties): DOMComponent<TProperties>;
    getInstance(widgetRef: HTMLDivElement): DOMComponent<TProperties>;
};

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface DOMComponentInstance extends DOMComponent<Properties> { }

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type Properties = DOMComponentOptions<DOMComponentInstance>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = Properties;
