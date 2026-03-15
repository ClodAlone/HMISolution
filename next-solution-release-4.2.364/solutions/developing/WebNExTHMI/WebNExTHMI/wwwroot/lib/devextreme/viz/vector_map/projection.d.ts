/**
* DevExtreme (viz/vector_map/projection.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface VectorMapProjectionConfig {
    /**
     * Specifies the projection&apos;s ratio of the width to the height.
     */
    aspectRatio?: number;
    /**
     * Converts coordinates from [x, y] to [lon, lat].
     */
    from?: ((coordinates: Array<number>) => Array<number>);
    /**
     * Converts coordinates from [lon, lat] to [x, y].
     */
    to?: ((coordinates: Array<number>) => Array<number>);
}

/**
                                                                  * Creates a new projection.
                                                                  * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
                                                                  */
                                                                 export const projection: {
    /**
     * Adds a new projection to the internal projection storage.
     */
    add(name: string, projection: VectorMapProjectionConfig | any): void;

    /**
     * Gets a predefined or custom projection from the projection storage.
     */
    get(name: 'equirectangular' | 'lambert' | 'mercator' | 'miller' | string): any;

    (data: VectorMapProjectionConfig): any;
};
