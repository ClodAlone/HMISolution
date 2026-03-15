/**
 * DevExtreme (esm/ui/scheduler/appointments/dataProvider/appointmentDataSource.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    Deferred
} from "../../../../core/utils/deferred";
export class AppointmentDataSource {
    constructor(dataSource) {
        this.setDataSource(dataSource);
        this._updatedAppointmentKeys = []
    }
    get keyName() {
        var store = this._dataSource.store();
        return store.key()
    }
    _getStoreKey(target) {
        var store = this._dataSource.store();
        return store.keyOf(target)
    }
    setDataSource(dataSource) {
        this._dataSource = dataSource;
        this.cleanState();
        this._initStoreChangeHandlers()
    }
    _initStoreChangeHandlers() {
        var dataSource = this._dataSource;
        var store = null === dataSource || void 0 === dataSource ? void 0 : dataSource.store();
        if (store) {
            store.on("updating", newItem => {
                this._updatedAppointment = newItem
            });
            store.on("push", pushItems => {
                var items = dataSource.items();
                var keyName = store.key();
                pushItems.forEach(pushItem => {
                    var itemExists = 0 !== items.filter(item => item[keyName] === pushItem.key).length;
                    if (itemExists) {
                        this._updatedAppointmentKeys.push({
                            key: keyName,
                            value: pushItem.key
                        })
                    } else {
                        var {
                            data: data
                        } = pushItem;
                        data && items.push(data)
                    }
                });
                dataSource.load()
            })
        }
    }
    getUpdatedAppointment() {
        return this._updatedAppointment
    }
    getUpdatedAppointmentKeys() {
        return this._updatedAppointmentKeys
    }
    cleanState() {
        this._updatedAppointment = null;
        this._updatedAppointmentKeys = []
    }
    add(rawAppointment) {
        return this._dataSource.store().insert(rawAppointment).done(() => this._dataSource.load())
    }
    update(target, data) {
        var key = this._getStoreKey(target);
        var d = new Deferred;
        this._dataSource.store().update(key, data).done(result => this._dataSource.load().done(() => d.resolve(result)).fail(d.reject)).fail(d.reject);
        return d.promise()
    }
    remove(rawAppointment) {
        var key = this._getStoreKey(rawAppointment);
        return this._dataSource.store().remove(key).done(() => this._dataSource.load())
    }
}
