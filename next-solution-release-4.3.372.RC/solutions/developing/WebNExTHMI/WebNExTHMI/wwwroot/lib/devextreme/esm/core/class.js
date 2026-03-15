/**
 * DevExtreme (esm/core/class.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import errors from "./errors";
import {
    isWindow
} from "./utils/type";
var wrapOverridden = function(baseProto, methodName, method) {
    return function() {
        var prevCallBase = this.callBase;
        this.callBase = baseProto[methodName];
        try {
            return method.apply(this, arguments)
        } finally {
            this.callBase = prevCallBase
        }
    }
};
var clonePrototype = function(obj) {
    var func = function() {};
    func.prototype = obj.prototype;
    return new func
};
var redefine = function(members) {
    var overridden;
    var memberName;
    var member;
    if (!members) {
        return this
    }
    for (memberName in members) {
        member = members[memberName];
        overridden = "function" === typeof this.prototype[memberName] && "function" === typeof member;
        this.prototype[memberName] = overridden ? wrapOverridden(this.parent.prototype, memberName, member) : member
    }
    return this
};
var include = function() {
    var classObj = this;
    var argument;
    var name;
    var i;
    var hasClassObjOwnProperty = Object.prototype.hasOwnProperty.bind(classObj);
    var isES6Class = !hasClassObjOwnProperty("_includedCtors") && !hasClassObjOwnProperty("_includedPostCtors");
    if (isES6Class) {
        classObj._includedCtors = classObj._includedCtors.slice(0);
        classObj._includedPostCtors = classObj._includedPostCtors.slice(0)
    }
    for (i = 0; i < arguments.length; i++) {
        argument = arguments[i];
        if (argument.ctor) {
            classObj._includedCtors.push(argument.ctor)
        }
        if (argument.postCtor) {
            classObj._includedPostCtors.push(argument.postCtor)
        }
        for (name in argument) {
            if ("ctor" === name || "postCtor" === name || "default" === name) {
                continue
            }
            classObj.prototype[name] = argument[name]
        }
    }
    return classObj
};
var subclassOf = function(parentClass) {
    var hasParentProperty = Object.prototype.hasOwnProperty.bind(this)("parent");
    var isES6Class = !hasParentProperty && this.parent;
    if (isES6Class) {
        var baseClass = Object.getPrototypeOf(this);
        return baseClass === parentClass || baseClass.subclassOf(parentClass)
    } else {
        if (this.parent === parentClass) {
            return true
        }
        if (!this.parent || !this.parent.subclassOf) {
            return false
        }
        return this.parent.subclassOf(parentClass)
    }
};
var abstract = function() {
    throw errors.Error("E0001")
};
var copyStatic = function() {
    var hasOwn = Object.prototype.hasOwnProperty;
    return function(source, destination) {
        for (var key in source) {
            if (!hasOwn.call(source, key)) {
                return
            }
            destination[key] = source[key]
        }
    }
}();
var classImpl = function() {};
classImpl.inherit = function(members) {
    var inheritor = function() {
        if (!this || isWindow(this) || "function" !== typeof this.constructor) {
            throw errors.Error("E0003")
        }
        var instance = this;
        var ctor = instance.ctor;
        var includedCtors = instance.constructor._includedCtors;
        var includedPostCtors = instance.constructor._includedPostCtors;
        var i;
        for (i = 0; i < includedCtors.length; i++) {
            includedCtors[i].call(instance)
        }
        if (ctor) {
            ctor.apply(instance, arguments)
        }
        for (i = 0; i < includedPostCtors.length; i++) {
            includedPostCtors[i].call(instance)
        }
    };
    inheritor.prototype = clonePrototype(this);
    copyStatic(this, inheritor);
    inheritor.inherit = this.inherit;
    inheritor.abstract = abstract;
    inheritor.redefine = redefine;
    inheritor.include = include;
    inheritor.subclassOf = subclassOf;
    inheritor.parent = this;
    inheritor._includedCtors = this._includedCtors ? this._includedCtors.slice(0) : [];
    inheritor._includedPostCtors = this._includedPostCtors ? this._includedPostCtors.slice(0) : [];
    inheritor.prototype.constructor = inheritor;
    inheritor.redefine(members);
    return inheritor
};
classImpl.abstract = abstract;
export default classImpl;
