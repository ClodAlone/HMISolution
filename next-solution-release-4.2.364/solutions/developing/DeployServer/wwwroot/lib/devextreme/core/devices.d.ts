/**
* DevExtreme (core/devices.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * The device object defines the device on which the application is running.
 */
export type Device = {
    /**
     * Indicates whether or not the device platform is Android.
     */
    android?: boolean;
    /**
     * Specifies the type of the device on which the application is running.
     */
    deviceType?: 'phone' | 'tablet' | 'desktop';
    /**
     * Indicates whether or not the device platform is generic, which means that the application will look and behave according to a generic &apos;light&apos; or &apos;dark&apos; theme.
     */
    generic?: boolean;
    /**
     * Specifies a performance grade of the current device.
     */
    grade?: 'A' | 'B' | 'C';
    /**
     * Indicates whether or not the device platform is iOS.
     */
    ios?: boolean;
    /**
     * Indicates whether or not the device type is &apos;phone&apos;.
     */
    phone?: boolean;
    /**
     * Specifies the platform of the device on which the application is running.
     */
    platform?: 'android' | 'ios' | 'generic';
    /**
     * Indicates whether or not the device type is &apos;tablet&apos;.
     */
    tablet?: boolean;
    /**
     * Specifies an array with the major and minor versions of the device platform.
     */
    version?: Array<number>;
};

type EventName = 'orientationChanged';

/**
 * An object that serves as a namespace for the methods and events specifying information on the current device.
 */
declare class DevicesObject {
    constructor(options?: { window?: Window });
    /**
     * Gets information on the current device.
     */
    current(): Device;
    /**
     * Overrides actual device information to force the application to operate as if it was running on a specified device.
     */
    current(deviceName: string | Device): void;
    /**
     * Detaches all event handlers from a single event.
     */
    off(eventName: EventName): this;
    /**
     * Detaches a particular event handler from a single event.
     */
    off(eventName: EventName, eventHandler: Function): this;
    /**
     * Subscribes to an event.
     */
    on(eventName: EventName, eventHandler: Function): this;
    /**
     * Subscribes to events.
     */
    on(events: { [key in EventName]?: Function }): this;
    /**
     * Returns the current device orientation.
     */
    orientation(): 'portrait' | 'landscape' | undefined;
    /**
     * Returns real information about the current device regardless of the value passed to the DevExpress.devices.current(deviceName) method.
     */
    real(): Device;
    isSimulator(): boolean;
}

/**
  * 
  * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
  */
 declare const devices: DevicesObject;
export default devices;
