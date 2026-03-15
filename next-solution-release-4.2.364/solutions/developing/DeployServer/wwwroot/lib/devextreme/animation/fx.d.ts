/**
* DevExtreme (animation/fx.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DxElement } from '../core/element';
import { DxPromise } from '../core/utils/deferred';
import { PositionConfig } from './position';

/**
 * Describes an animation state.
 */
export type AnimationState = string | number | {
    /**
     * Element opacity.
     */
    opacity: number;
} | {
    /**
     * A value that controls element size.
     */
    scale: number;
} | {
    /**
     * Element position.
     */
    position: PositionConfig;
} | {
    /**
     * A shortcut that positions the element&apos;s left side relative to the parent element.
     */
    left: number;
} | {
    /**
     * A shortcut that positions the element&apos;s top side relative to the parent element.
     */
    top: number;
};

/**
 * Defines animation properties.
 */
export type AnimationConfig = {
    /**
     * A function called after animation is completed.
     */
    complete?: (($element: DxElement, config: AnimationConfig) => void);
    /**
     * A number specifying wait time before animation execution.
     */
    delay?: number;
    /**
     * Specifies the animation direction for the &apos;slideIn&apos; and &apos;slideOut&apos; animation types.
     */
    direction?: 'bottom' | 'left' | 'right' | 'top';
    /**
     * A number specifying the time in milliseconds spent on animation.
     */
    duration?: number;
    /**
     * A string specifying the easing function for animation.
     */
    easing?: string;
    /**
     * Specifies an initial animation state. Use the to property to specify the final state.
     */
    from?: AnimationState;
    /**
     * A number specifying the time period to wait before the animation of the next stagger item starts.
     */
    staggerDelay?: number;
    /**
     * A function called before animation is started.
     */
    start?: (($element: DxElement, config: AnimationConfig) => void);
    /**
     * Specifies a final animation state. Use the from property to specify an initial state.
     */
    to?: AnimationState;
    /**
     * A string value specifying the animation type.
     */
    type?: 'css' | 'fade' | 'fadeIn' | 'fadeOut' | 'pop' | 'slide' | 'slideIn' | 'slideOut';
};

/**
 * @deprecated Use the AnimationConfig type instead
 */
export type animationConfig = AnimationConfig;

/**
 * An object that serves as a namespace for the methods that are used to animate UI elements.
 */
declare const fx: {
    /**
     * Animates an element.
     */
    animate(element: Element, config: AnimationConfig): DxPromise<void>;

    /**
     * Checks whether an element is being animated.
     */
    isAnimating(element: Element): boolean;

    /**
     * Stops an element&apos;s animation.
     */
    stop(element: Element, jumpToEnd: boolean): void;
};
export default fx;
