/**
* DevExtreme (ui/overlay.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    AnimationConfig,
} from '../animation/fx';

import {
    UserDefinedElement,
    DxElement,
} from '../core/element';

import {
    template,
} from '../core/templates/template';

import {
    DxPromise,
} from '../core/utils/deferred';

import {
    DxEvent,
    Cancelable,
    EventInfo,
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxOverlayOptions<TComponent> extends WidgetOptions<TComponent> {
    /**
     * Configures UI component visibility animations. This object contains two fields: show and hide.
     */
    animation?: dxOverlayAnimation;
    /**
     * Specifies whether to close the UI component if a user clicks outside it.
     */
    closeOnOutsideClick?: boolean | ((event: DxEvent<MouseEvent | PointerEvent | TouchEvent>) => boolean);
    /**
     * Specifies a custom template for the UI component content.
     */
    contentTemplate?: template | ((contentElement: DxElement) => string | UserDefinedElement);
    /**
      * Copies your custom CSS classes from the root element to the wrapper element.
      * @deprecated Use the wrapperAttr property instead.
      */
     copyRootClassesToWrapper?: boolean;
    /**
     * Specifies whether to render the UI component&apos;s content when it is displayed. If false, the content is rendered immediately.
     */
    deferRendering?: boolean;
    /**
     * Specifies an element with boundaries within which users can drag and resize the widget. Ignored if the dragOutsideBoundary property is set to true.
     */
    dragAndResizeArea?: string | UserDefinedElement;
    /**
     * Specifies whether or not an end-user can drag the UI component.
     */
    dragEnabled?: boolean;
    /**
      * Allows users to drag the widget within the browser window or beyond the window&apos;s borders.
      */
     dragOutsideBoundary?: boolean;
    /**
     * Specifies the global attributes to be attached to the UI component&apos;s container element.
     * @deprecated Use the wrapperAttr option instead.
     */
    elementAttr?: any;
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * Specifies the maximum height the UI component can reach while resizing.
     */
    maxHeight?: number | string | (() => number | string);
    /**
     * Specifies the maximum width the UI component can reach while resizing.
     */
    maxWidth?: number | string | (() => number | string);
    /**
     * Specifies the minimum height the UI component can reach while resizing.
     */
    minHeight?: number | string | (() => number | string);
    /**
     * Specifies the minimum width the UI component can reach while resizing.
     */
    minWidth?: number | string | (() => number | string);
    /**
     * A function that is executed after the UI component is hidden.
     */
    onHidden?: ((e: EventInfo<TComponent>) => void);
    /**
     * A function that is executed before the UI component is hidden.
     */
    onHiding?: ((e: Cancelable & EventInfo<TComponent>) => void);
    /**
     * A function that is executed before the UI component is displayed.
     */
    onShowing?: ((e: Cancelable & EventInfo<TComponent>) => void);
    /**
     * A function that is executed after the UI component is displayed.
     */
    onShown?: ((e: EventInfo<TComponent>) => void);
    /**
     * Positions the UI component.
     */
    position?: any;
    /**
     * Specifies whether to shade the background when the UI component is active.
     */
    shading?: boolean;
    /**
     * Specifies the shading color. Applies only if shading is enabled.
     */
    shadingColor?: string;
    /**
     * A Boolean value specifying whether or not the UI component is visible.
     */
    visible?: boolean;
    /**
     * Specifies the UI component&apos;s width.
     */
    width?: number | string | (() => number | string);
    /**
     * Specifies the global attributes for the UI component&apos;s wrapper element.
     */
    wrapperAttr?: any;
    /**
      * Specifies whether to hide the widget when users scroll one of its parent elements.
      */
     hideOnParentScroll?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxOverlayAnimation {
    /**
     * An object that defines the animation properties used when the UI component is being hidden.
     */
    hide?: AnimationConfig;
    /**
     * An object that defines the animation properties used when the UI component is being shown.
     */
    show?: AnimationConfig;
}
/**
 * The Overlay UI component represents a window overlaying the current view. It displays data located within the HTML element representing the UI component.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class dxOverlay<TProperties> extends Widget<TProperties> {
    /**
     * Gets the UI component&apos;s content.
     */
    content(): DxElement;
    /**
     * Hides the UI component.
     */
    hide(): DxPromise<boolean>;
    /**
     * Recalculates the UI component&apos;s size and position without rerendering.
     */
    repaint(): void;
    /**
     * Shows the UI component.
     */
    show(): DxPromise<boolean>;
    /**
     * Shows or hides the UI component depending on the argument.
     */
    toggle(showing: boolean): DxPromise<boolean>;
}

/**
 * Specifies the base z-index for all overlay UI components.
 */
export function baseZIndex(zIndex: number): void;

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface OverlayInstance extends dxOverlay<Properties> { }

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type Properties = dxOverlayOptions<OverlayInstance>;

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = Properties;
