/**
* DevExtreme (ui/pivot_grid_field_chooser.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    DxEvent,
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import PivotGridDataSource, {
    Field,
} from './pivot_grid/data_source';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ContentReadyEvent = EventInfo<dxPivotGridFieldChooser>;

export type ContextMenuPreparingEvent = EventInfo<dxPivotGridFieldChooser> & {
    readonly area?: string;
    readonly field?: Field;
    readonly event?: DxEvent;
    items?: Array<any>;
};

export type DisposingEvent = EventInfo<dxPivotGridFieldChooser>;

export type InitializedEvent = InitializedEventInfo<dxPivotGridFieldChooser>;

export type OptionChangedEvent = EventInfo<dxPivotGridFieldChooser> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxPivotGridFieldChooserOptions extends WidgetOptions<dxPivotGridFieldChooser> {
    /**
     * Specifies whether the field chooser allows searching in the &apos;All Fields&apos; section.
     */
    allowSearch?: boolean;
    /**
     * Specifies when to apply changes made in the UI component to the PivotGrid.
     */
    applyChangesMode?: 'instantly' | 'onDemand';
    /**
     * The data source of a PivotGrid UI component.
     */
    dataSource?: PivotGridDataSource;
    /**
     * Configures the header filter feature.
     */
    headerFilter?: {
      /**
       * Specifies whether searching is enabled in the header filter.
       */
      allowSearch?: boolean;
      /**
       * Specifies the height of the popup menu containing filtering values.
       */
      height?: number;
      /**
       * Specifies a delay in milliseconds between when a user finishes typing in the header filter&apos;s search panel, and when the search is executed.
       */
      searchTimeout?: number;
      /**
       * Specifies whether to show all field values or only those that satisfy the other applied filters.
       */
      showRelevantValues?: boolean;
      /**
       * Configures the texts of the popup menu&apos;s elements.
       */
      texts?: {
        /**
         * Specifies the text of the button that closes the popup menu without applying a filter.
         */
        cancel?: string;
        /**
         * Specifies the name of the item that represents empty values in the popup menu.
         */
        emptyValue?: string;
        /**
         * Specifies the text of the button that applies a filter.
         */
        ok?: string;
      };
      /**
       * Specifies the width of the popup menu containing filtering values.
       */
      width?: number;
    };
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * Specifies the field chooser layout.
     */
    layout?: 0 | 1 | 2;
    /**
     * A function that is executed before the context menu is rendered.
     */
    onContextMenuPreparing?: ((e: ContextMenuPreparingEvent) => void);
    /**
     * Specifies a delay in milliseconds between when a user finishes typing in the field chooser&apos;s search panel, and when the search is executed.
     */
    searchTimeout?: number;
    /**
     * The UI component&apos;s state.
     */
    state?: any;
    /**
     * Strings that can be changed or localized in the PivotGridFieldChooser UI component.
     */
    texts?: {
      /**
       * The string to display instead of All Fields.
       */
      allFields?: string;
      /**
       * The string to display instead of Column Fields.
       */
      columnFields?: string;
      /**
       * The string to display instead of Data Fields.
       */
      dataFields?: string;
      /**
       * The string to display instead of Filter Fields.
       */
      filterFields?: string;
      /**
       * The string to display instead of Row Fields.
       */
      rowFields?: string;
    };
}
/**
 * A complementary UI component for the PivotGrid that allows you to manage data displayed in the PivotGrid. The field chooser is already integrated in the PivotGrid and can be invoked using the context menu. If you need to continuously display the field chooser near the PivotGrid UI component, use the PivotGridFieldChooser UI component.
 */
export default class dxPivotGridFieldChooser extends Widget<dxPivotGridFieldChooserOptions> {
    /**
     * Applies changes made in the UI component to the PivotGrid. Takes effect only if applyChangesMode is &apos;onDemand&apos;.
     */
    applyChanges(): void;
    /**
     * Cancels changes made in the UI component without applying them to the PivotGrid. Takes effect only if applyChangesMode is &apos;onDemand&apos;.
     */
    cancelChanges(): void;
    /**
     * Gets the PivotGridDataSource instance.
     */
    getDataSource(): PivotGridDataSource;
    /**
     * Updates the UI component to the size of its content.
     */
    updateDimensions(): void;
}

export type Properties = dxPivotGridFieldChooserOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxPivotGridFieldChooserOptions;
