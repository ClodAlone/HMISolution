/**
* DevExtreme (ui/draggable.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DOMComponent, {
    DOMComponentOptions,
} from '../core/dom_component';

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
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import dxSortable from './sortable';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DraggableBaseOptions<TComponent> extends DOMComponentOptions<TComponent> {
    /**
     * Enables automatic scrolling while dragging an item beyond the viewport.
     */
    autoScroll?: boolean;
    /**
     * Specifies a DOM element that limits the dragging area.
     */
    boundary?: string | UserDefinedElement;
    /**
     * Specifies a custom container in which the draggable item should be rendered.
     */
    container?: string | UserDefinedElement;
    /**
     * Specifies the cursor offset from the dragged item.
     */
    cursorOffset?: string | {
      /**
       * Specifies the horizontal cursor offset from the dragged item in pixels.
       */
      x?: number;
      /**
       * Specifies the vertical cursor offset from the dragged item in pixels.
       */
      y?: number;
    };
    /**
     * A container for custom data.
     */
    data?: any;
    /**
     * Specifies the directions in which an item can be dragged.
     */
    dragDirection?: 'both' | 'horizontal' | 'vertical';
    /**
     * Allows you to group several UI components, so that users can drag and drop items between them.
     */
    group?: string;
    /**
     * Specifies a CSS selector (ID or class) that should act as the drag handle(s) for the item(s).
     */
    handle?: string;
    /**
     * Specifies the distance in pixels from the edge of viewport at which scrolling should start. Applies only if autoScroll is true.
     */
    scrollSensitivity?: number;
    /**
     * Specifies the scrolling speed when dragging an item beyond the viewport. Applies only if autoScroll is true.
     */
    scrollSpeed?: number;
}
/**
                                                                   * 
                                                                   * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
                                                                   */
                                                                  export interface DraggableBase { }

export type DisposingEvent = EventInfo<dxDraggable>;

export type DragEndEvent = Cancelable & NativeEventInfo<dxDraggable, PointerEvent | MouseEvent | TouchEvent> & {
    readonly itemData?: any;
    readonly itemElement?: DxElement;
    readonly fromComponent: dxSortable | dxDraggable;
    readonly toComponent: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
};

export type DragMoveEvent = Cancelable & NativeEventInfo<dxDraggable, PointerEvent | MouseEvent | TouchEvent> & {
    readonly itemData?: any;
    readonly itemElement?: DxElement;
    readonly fromComponent: dxSortable | dxDraggable;
    readonly toComponent: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
};

export type DragStartEvent = Cancelable & NativeEventInfo<dxDraggable, PointerEvent | MouseEvent | TouchEvent> & {
    itemData?: any;
    readonly itemElement?: DxElement;
    readonly fromData?: any;
};

export type InitializedEvent = InitializedEventInfo<dxDraggable>;

export type OptionChangedEvent = EventInfo<dxDraggable> & ChangedOptionInfo;

export type DragTemplateData = {
    readonly itemData?: any;
    readonly itemElement: DxElement;
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDraggableOptions extends DraggableBaseOptions<dxDraggable> {
    /**
     * Allows a user to drag clones of items instead of actual items.
     */
    clone?: boolean;
    /**
     * Specifies custom markup to be shown instead of the item being dragged.
     */
    dragTemplate?: template | ((dragInfo: DragTemplateData, containerElement: DxElement) => string | UserDefinedElement);
    /**
     * A function that is called when drag gesture is finished.
     */
    onDragEnd?: ((e: DragEndEvent) => void);
    /**
     * A function that is called every time a draggable item is moved.
     */
    onDragMove?: ((e: DragMoveEvent) => void);
    /**
     * A function that is called when the drag gesture is initialized.
     */
    onDragStart?: ((e: DragStartEvent) => void);
}
/**
 * Draggable is a user interface utility that allows UI component elements to be dragged and dropped.
 */
export default class dxDraggable extends DOMComponent<dxDraggableOptions> implements DraggableBase { }

export type Properties = dxDraggableOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxDraggableOptions;
