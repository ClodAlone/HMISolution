/**
* DevExtreme (animation/presets.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    Device,
} from '../core/devices';

import {
    AnimationConfig,
} from './fx';

/**
 * A repository of animations.
 */
declare const animationPresets: {
    /**
     * Applies the changes made in the animation repository.
     */
    applyChanges(): void;
    /**
     * Removes all animations from the repository.
     */
    clear(): void;
    /**
     * Deletes an animation with a specific name.
     */
    clear(name: string): void;
    /**
     * Gets the configuration of an animation with a specific name.
     */
    getPreset(name: string): AnimationConfig;
    /**
     * Registers predefined animations in the animation repository.
     */
    registerDefaultPresets(): void;
    /**
     * Adds an animation with a specific name to the animation repository.
     */
    registerPreset(name: string, config: { animation: AnimationConfig; device?: Device }): void;
    /**
     * Deletes all custom animations.
     */
    resetToDefaults(): void;
};

export default animationPresets;
