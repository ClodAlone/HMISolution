/**
* DevExtreme (ui/map.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    DxPromise,
} from '../core/utils/deferred';

import {
  EventInfo,
  NativeEventInfo,
  InitializedEventInfo,
  ChangedOptionInfo,
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ClickEvent = NativeEventInfo<dxMap, MouseEvent | PointerEvent>;

export type DisposingEvent = EventInfo<dxMap>;

export type InitializedEvent = InitializedEventInfo<dxMap>;

export type MarkerAddedEvent = EventInfo<dxMap> & {
  readonly options: any;
  originalMarker: any;
};

export type MarkerRemovedEvent = EventInfo<dxMap> & {
  readonly options?: any;
};

export type OptionChangedEvent = EventInfo<dxMap> & ChangedOptionInfo;

export type ReadyEvent = EventInfo<dxMap> & {
  originalMap: any;
};

export type RouteAddedEvent = EventInfo<dxMap> & {
  readonly options: any;
  originalRoute: any;
};

export type RouteRemovedEvent = EventInfo<dxMap> & {
  readonly options?: any;
};

export interface MapLocation {
    /**
     * The latitude location of the UI component.
     */
    lat: number;
    /**
     * The longitude location of the UI component.
     */
    lng: number;
}

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxMapOptions extends WidgetOptions<dxMap> {
    /**
     * Keys to authenticate the component within map providers.
     */
    apiKey?: string | {
      /**
       * A key used to authenticate the component within Bing Maps.
       */
      bing?: string;
      /**
       * A key used to authenticate the component within Google Maps.
       */
      google?: string;
      /**
       * A key used to authenticate the component within Google Maps Static.
       */
      googleStatic?: string;
    };
    /**
     * Specifies whether the UI component automatically adjusts center and zoom property values when adding a new marker or route, or if a new UI component contains markers or routes by default.
     */
    autoAdjust?: boolean;
    /**
     * An object, a string, or an array specifying which part of the map is displayed at the UI component&apos;s center using coordinates. The UI component can change this value if autoAdjust is enabled.
     */
    center?: any | string | Array<number>;
    /**
     * Specifies whether or not map UI component controls are available.
     */
    controls?: boolean;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * A URL pointing to the custom icon to be used for map markers.
     */
    markerIconSrc?: string;
    /**
     * An array of markers displayed on a map.
     */
    markers?: Array<{
      /**
       * A URL pointing to the custom icon to be used for the marker.
       */
      iconSrc?: string;
      /**
       * Specifies the marker location.
       */
      location?: any | string | Array<number>;
      /**
       * A callback function performed when the marker is clicked.
       */
      onClick?: Function;
      /**
       * A tooltip to be used for the marker.
       */
      tooltip?: string | {
        /**
         * Specifies whether a tooltip is visible by default or not.
         */
        isShown?: boolean;
        /**
         * Specifies the text or HTML markup displayed in the tooltip.
         */
        text?: string;
      };
    }>;
    /**
     * A function that is executed when any location on the map is clicked or tapped.
     */
    onClick?: ((e: ClickEvent) => void) | string;
    /**
     * A function that is executed when a marker is created on the map.
     */
    onMarkerAdded?: ((e: MarkerAddedEvent) => void);
    /**
     * A function that is executed when a marker is removed from the map.
     */
    onMarkerRemoved?: ((e: MarkerRemovedEvent) => void);
    /**
     * A function that is executed when the map is ready.
     */
    onReady?: ((e: ReadyEvent) => void);
    /**
     * A function that is executed when a route is created on the map.
     */
    onRouteAdded?: ((e: RouteAddedEvent) => void);
    /**
     * A function that is executed when a route is removed from the map.
     */
    onRouteRemoved?: ((e: RouteRemovedEvent) => void);
    /**
     * The name of the current map data provider.
     */
    provider?: 'bing' | 'google' | 'googleStatic';
    /**
     * An array of routes shown on the map.
     */
    routes?: Array<{
      /**
       * Specifies the color of the line displaying the route.
       */
      color?: string;
      /**
       * Contains an array of objects making up the route.
       */
      locations?: Array<any>;
      /**
       * Specifies a transportation mode to be used in the displayed route.
       */
      mode?: 'driving' | 'walking';
      /**
       * Specifies the opacity of the line displaying the route.
       */
      opacity?: number;
      /**
       * Specifies the thickness of the line displaying the route in pixels.
       */
      weight?: number;
    }>;
    /**
     * The type of a map to display.
     */
    type?: 'hybrid' | 'roadmap' | 'satellite';
    /**
     * Specifies the UI component&apos;s width.
     */
    width?: number | string | (() => number | string);
    /**
     * The map&apos;s zoom level. The UI component can change this value if autoAdjust is enabled.
     */
    zoom?: number;
}
/**
 * The Map is an interactive UI component that displays a geographic map with markers and routes.
 */
export default class dxMap extends Widget<dxMapOptions> {
    /**
     * Adds a marker to the map.
     */
    addMarker(markerOptions: any | Array<any>): DxPromise<any>;
    /**
     * Adds a route to the map.
     */
    addRoute(options: any | Array<any>): DxPromise<any>;
    /**
     * Removes a marker from the map.
     */
    removeMarker(marker: any | number | Array<any>): DxPromise<void>;
    /**
     * Removes a route from the map.
     */
    removeRoute(route: any | number | Array<any>): DxPromise<void>;
}

export type Properties = dxMapOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxMapOptions;
