/**
* DevExtreme (animation/position.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    UserDefinedElement,
} from '../core/element';

/**
 * Configures the position of an overlay element.
 */
export interface PositionConfig {
    /**
     * Specifies the target element&apos;s side or corner where the overlay element should be positioned.
     */
    at?: 'bottom' | 'center' | 'left' | 'left bottom' | 'left top' | 'right' | 'right bottom' | 'right top' | 'top' | {
      /**
       * Specifies a position in the horizontal direction (for left, right, or center alignment).
       */
      x?: 'center' | 'left' | 'right';
      /**
       * Specifies a position in the vertical direction (for top, bottom, or center alignment).
       */
      y?: 'bottom' | 'center' | 'top';
    };
    /**
     * A boundary element in which the overlay element must be positioned.
     */
    boundary?: string | UserDefinedElement | Window;
    /**
     * Specifies the offset of boundaries from the boundary element.
     */
    boundaryOffset?: string | {
      /**
       * Specifies a horizontal offset.
       */
      x?: number;
      /**
       * Specifies a vertical offset.
       */
      y?: number;
    };
    /**
     * Specifies how to resolve collisions - when the overlay element exceeds the boundary element.
     */
    collision?: 'fit' | 'fit flip' | 'fit flipfit' | 'fit none' | 'flip' | 'flip fit' | 'flip none' | 'flipfit' | 'flipfit fit' | 'flipfit none' | 'none' | 'none fit' | 'none flip' | 'none flipfit' | {
      /**
       * Specifies how to resolve horizontal collisions.
       */
      x?: 'fit' | 'flip' | 'flipfit' | 'none';
      /**
       * Specifies how to resolve vertical collisions.
       */
      y?: 'fit' | 'flip' | 'flipfit' | 'none';
    };
    /**
     * Specifies the overlay element&apos;s side or corner to align with a target element.
     */
    my?: 'bottom' | 'center' | 'left' | 'left bottom' | 'left top' | 'right' | 'right bottom' | 'right top' | 'top' | {
      /**
       * Specifies a position in the horizontal direction (for left, right, or center alignment).
       */
      x?: 'center' | 'left' | 'right';
      /**
       * Specifies a position in the vertical direction (for top, bottom, or center alignment).
       */
      y?: 'bottom' | 'center' | 'top';
    };
    /**
     * The target element relative to which the overlay element should be positioned.
     */
    of?: string | UserDefinedElement | Window;
    /**
     * Specifies the overlay element&apos;s offset from a specified position.
     */
    offset?: string | {
      /**
       * Specifies a horizontal offset.
       */
      x?: number;
      /**
       * Specifies a vertical offset.
       */
      y?: number;
    };
}

/**
 * @deprecated Use the PositionConfig type instead
 */
export interface positionConfig extends PositionConfig { }
