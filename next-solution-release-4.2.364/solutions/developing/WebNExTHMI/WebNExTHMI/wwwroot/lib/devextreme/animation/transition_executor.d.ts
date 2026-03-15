/**
* DevExtreme (animation/transition_executor.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    UserDefinedElementsArray,
} from '../core/element';

import {
    DxPromise,
} from '../core/utils/deferred';

import {
    AnimationConfig,
} from './fx';

/**
 * The manager that performs several specified animations at a time.
 */
export default class TransitionExecutor {
    /**
     * Registers the set of elements that should be animated as &apos;entering&apos; using the specified animation configuration.
     */
    enter(elements: UserDefinedElementsArray, animation: AnimationConfig | string): void;
    /**
     * Registers a set of elements that should be animated as &apos;leaving&apos; using the specified animation configuration.
     */
    leave(elements: UserDefinedElementsArray, animation: AnimationConfig | string): void;
    /**
     * Deletes all the animations registered in the Transition Executor by using the enter(elements, animation) and leave(elements, animation) methods.
     */
    reset(): void;
    /**
     * Starts all the animations registered using the enter(elements, animation) and leave(elements, animation) methods beforehand.
     */
    start(): DxPromise<void>;
    /**
     * Stops all started animations.
     */
    stop(): void;
}
