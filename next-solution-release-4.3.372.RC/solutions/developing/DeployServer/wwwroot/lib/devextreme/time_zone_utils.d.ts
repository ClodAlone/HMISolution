/**
* DevExtreme (time_zone_utils.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * A time zone object.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSchedulerTimeZone {
    /**
     * A time zone text string from the IANA database.
     */
    id: string;
    /**
     * A GMT offset.
     */
    offset: number;
    /**
     * A time zone in the following format: `(GMT ±[hh]:[mm]) [id]`.
     */
    title: string;
}

/**
 * Gets the list of IANA time zone objects.
 */
export function getTimeZones(date?: Date): Array<dxSchedulerTimeZone>;
