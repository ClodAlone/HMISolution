/**
* DevExtreme (exporter/export_load_panel.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * Configures the load panel.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ExportLoadPanel {
    /**
     * Specifies whether the load panel is enabled.
     */
    enabled?: boolean;
    /**
     * Specifies text displayed on the load panel.
     */
    text?: string;
    /**
     * Specifies the width of the load panel in pixels.
     */
    width?: number;
    /**
     * Specifies the height of the load panel in pixels.
     */
    height?: number;
    /**
     * Specifies whether to show the loading indicator.
     */
    showIndicator?: boolean;
    /**
     * Specifies a URL pointing to an image to be used as a loading indicator.
     */
    indicatorSrc?: string;
    /**
     * Specifies whether to show the pane of the load panel.
     */
    showPane?: boolean;
    /**
     * Specifies whether to shade the UI component when the load panel is shown.
     */
    shading?: boolean;
    /**
     * Specifies the shading color. Applies only if shading is true.
     */
    shadingColor?: string;
}
