/**
* DevExtreme (ui/sortable.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DOMComponent from '../core/dom_component';

import {
    UserDefinedElement,
    DxElement,
} from '../core/element';

import {
    template,
} from '../core/templates/template';

import {
    DxEvent,
    Cancelable,
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import dxDraggable, {
    DraggableBase,
    DraggableBaseOptions,
} from './draggable';

export interface AddEvent {
    readonly component: dxSortable;
    readonly element: DxElement;
    readonly model?: any;
    readonly event: DxEvent<PointerEvent | MouseEvent | TouchEvent>;
    readonly itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex: number;
    readonly toIndex: number;
    readonly fromComponent: dxSortable | dxDraggable;
    readonly toComponent: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
    readonly dropInsideItem: boolean;
}

export type DisposingEvent = EventInfo<dxSortable>;

export type DragChangeEvent = Cancelable & NativeEventInfo<dxSortable, PointerEvent | MouseEvent | TouchEvent> & {
    readonly itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex?: number;
    readonly toIndex?: number;
    readonly fromComponent?: dxSortable | dxDraggable;
    readonly toComponent?: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
    readonly dropInsideItem?: boolean;
};

export type DragEndEvent = Cancelable & NativeEventInfo<dxSortable, PointerEvent | MouseEvent | TouchEvent> & {
    readonly itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex: number;
    readonly toIndex: number;
    readonly fromComponent: dxSortable | dxDraggable;
    readonly toComponent: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
    readonly dropInsideItem: boolean;
};

export type DragMoveEvent = Cancelable & NativeEventInfo<dxSortable, PointerEvent | MouseEvent | TouchEvent> & {
    readonly itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex: number;
    readonly toIndex: number;
    readonly fromComponent: dxSortable | dxDraggable;
    readonly toComponent: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
    readonly dropInsideItem: boolean;
};

export type DragStartEvent = Cancelable & NativeEventInfo<dxSortable, PointerEvent | MouseEvent | TouchEvent> & {
    itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex: number;
    readonly fromData?: any;
};

export type InitializedEvent = InitializedEventInfo<dxSortable>;

export type OptionChangedEvent = EventInfo<dxSortable> & ChangedOptionInfo;

export type RemoveEvent = NativeEventInfo<dxSortable, PointerEvent | MouseEvent | TouchEvent> & {
    readonly itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex: number;
    readonly toIndex: number;
    readonly fromComponent: dxSortable | dxDraggable;
    readonly toComponent: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
};

export type ReorderEvent = NativeEventInfo<dxSortable, PointerEvent | MouseEvent | TouchEvent> & {
    readonly itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex: number;
    readonly toIndex: number;
    readonly fromComponent: dxSortable | dxDraggable;
    readonly toComponent: dxSortable | dxDraggable;
    readonly fromData?: any;
    readonly toData?: any;
    readonly dropInsideItem: boolean;
    promise?: PromiseLike<void>;
};

export interface DragTemplateData {
    readonly itemData?: any;
    readonly itemElement: DxElement;
    readonly fromIndex: number;
}

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSortableOptions extends DraggableBaseOptions<dxSortable> {
    /**
     * Allows a user to drop an item inside another item.
     */
    allowDropInsideItem?: boolean;
    /**
     * Allows a user to reorder sortable items.
     */
    allowReordering?: boolean;
    /**
     * Specifies custom markup to be shown instead of the item being dragged.
     */
    dragTemplate?: template | ((dragInfo: DragTemplateData, containerElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies how to highlight the item&apos;s drop position.
     */
    dropFeedbackMode?: 'push' | 'indicate';
    /**
     * Specifies a CSS selector for the items that can be dragged.
     */
    filter?: string;
    /**
     * Notifies the UI component of the items&apos; orientation.
     */
    itemOrientation?: 'horizontal' | 'vertical';
    /**
     * Moves an element in the HTML markup when it is dropped.
     */
    moveItemOnDrop?: boolean;
    /**
     * A function that is called when a new item is added.
     */
    onAdd?: ((e: AddEvent) => void);
    /**
     * A function that is called when the dragged item&apos;s position in the list is changed.
     */
    onDragChange?: ((e: DragChangeEvent) => void);
    /**
     * A function that is called when the drag gesture is finished.
     */
    onDragEnd?: ((e: DragEndEvent) => void);
    /**
     * A function that is called every time a draggable item is moved.
     */
    onDragMove?: ((e: DragMoveEvent) => void);
    /**
     * A function that is called when drag gesture is initialized.
     */
    onDragStart?: ((e: DragStartEvent) => void);
    /**
     * A function that is called when a draggable item is removed.
     */
    onRemove?: ((e: RemoveEvent) => void);
    /**
     * A function that is called when the draggable items are reordered.
     */
    onReorder?: ((e: ReorderEvent) => void);
}
/**
 * Sortable is a user interface utility that allows a UI component&apos;s items to be reordered via drag and drop gestures.
 */
export default class dxSortable extends DOMComponent<dxSortableOptions> implements DraggableBase {
    /**
     * Updates Sortable&apos;s dimensions. Call this method after items are added or their dimensions are changed during dragging.
     */
    update(): void;
}

export type Properties = dxSortableOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxSortableOptions;
