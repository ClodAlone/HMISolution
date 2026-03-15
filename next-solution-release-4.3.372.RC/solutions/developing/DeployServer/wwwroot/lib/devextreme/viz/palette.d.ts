/**
* DevExtreme (viz/palette.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type PaletteType = 'Bright' | 'Harmony Light' | 'Ocean' | 'Pastel' | 'Soft' | 'Soft Pastel' | 'Vintage' | 'Violet' | 'Carmine' | 'Dark Moon' | 'Dark Violet' | 'Green Mist' | 'Soft Blue' | 'Material' | 'Office';
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type PaletteExtensionModeType = 'alternate' | 'blend' | 'extrapolate';
/**
 * Gets the current palette&apos;s name.
 */
export function currentPalette(): string;

/**
 * Changes the current palette for all data visualization UI components on the page.
 */
export function currentPalette(paletteName: string): void;

/**
 * Returns a subset of palette colors.
 */
export function generateColors(palette: PaletteType | Array<string>, count: number, options: { paletteExtensionMode?: PaletteExtensionModeType; baseColorSet?: 'simpleSet' | 'indicatingSet' | 'gradientSet' }): Array<string>;

/**
 * Gets the color sets of a predefined or registered palette.
 */
export function getPalette(paletteName: string): any;

/**
 * Registers a new palette.
 */
export function registerPalette(paletteName: string, palette: any): void;
