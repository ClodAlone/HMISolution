/**
 * DevExtreme (esm/core/dom_component.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import config from "./config";
import errors from "./errors";
import windowResizeCallbacks from "../core/utils/resize_callbacks";
import {
    Component
} from "./component";
import {
    TemplateManager
} from "./template_manager";
import {
    attachInstanceToElement,
    getInstanceByElement
} from "./utils/public_component";
import {
    cleanDataRecursive
} from "./element_data";
import {
    each
} from "./utils/iterator";
import {
    extend
} from "./utils/extend";
import {
    getPublicElement
} from "../core/element";
import {
    grep,
    noop
} from "./utils/common";
import {
    inArray
} from "./utils/array";
import {
    isString,
    isDefined,
    isFunction
} from "./utils/type";
import {
    hasWindow
} from "../core/utils/window";
import {
    resize as resizeEvent,
    visibility as visibilityEvents
} from "../events/short";
var {
    abstract: abstract
} = Component;
var DOMComponent = Component.inherit({
    _getDefaultOptions() {
        return extend(this.callBase(), {
            width: void 0,
            height: void 0,
            rtlEnabled: config().rtlEnabled,
            elementAttr: {},
            disabled: false,
            integrationOptions: {}
        }, this._useTemplates() ? TemplateManager.createDefaultOptions() : {})
    },
    ctor(element, options) {
        this._customClass = null;
        this._createElement(element);
        attachInstanceToElement(this._$element, this, this._dispose);
        this.callBase(options)
    },
    _createElement(element) {
        this._$element = $(element)
    },
    _getSynchronizableOptionsForCreateComponent: () => ["rtlEnabled", "disabled", "templatesRenderAsynchronously"],
    _checkFunctionValueDeprecation: function(optionNames) {
        if (!this.option("_ignoreFunctionValueDeprecation")) {
            optionNames.forEach(optionName => {
                if (isFunction(this.option(optionName))) {
                    errors.log("W0017", optionName)
                }
            })
        }
    },
    _visibilityChanged: abstract,
    _dimensionChanged: abstract,
    _init() {
        this.callBase();
        this._checkFunctionValueDeprecation(["width", "height", "maxHeight", "maxWidth", "minHeight", "minWidth", "popupHeight", "popupWidth"]);
        this._attachWindowResizeCallback();
        this._initTemplateManager()
    },
    _setOptionsByDevice(instanceCustomRules) {
        this.callBase([].concat(this.constructor._classCustomRules || [], instanceCustomRules || []))
    },
    _isInitialOptionValue(name) {
        var isCustomOption = this.constructor._classCustomRules && Object.prototype.hasOwnProperty.call(this._convertRulesToOptions(this.constructor._classCustomRules), name);
        return !isCustomOption && this.callBase(name)
    },
    _attachWindowResizeCallback() {
        if (this._isDimensionChangeSupported()) {
            var windowResizeCallBack = this._windowResizeCallBack = this._dimensionChanged.bind(this);
            windowResizeCallbacks.add(windowResizeCallBack)
        }
    },
    _isDimensionChangeSupported() {
        return this._dimensionChanged !== abstract
    },
    _renderComponent() {
        this._initMarkup();
        hasWindow() && this._render()
    },
    _initMarkup() {
        var {
            rtlEnabled: rtlEnabled
        } = this.option() || {};
        this._renderElementAttributes();
        this._toggleRTLDirection(rtlEnabled);
        this._renderVisibilityChange();
        this._renderDimensions()
    },
    _render() {
        this._attachVisibilityChangeHandlers()
    },
    _renderElementAttributes() {
        var {
            elementAttr: elementAttr
        } = this.option() || {};
        var attributes = extend({}, elementAttr);
        var classNames = attributes.class;
        delete attributes.class;
        this.$element().attr(attributes).removeClass(this._customClass).addClass(classNames);
        this._customClass = classNames
    },
    _renderVisibilityChange() {
        if (this._isDimensionChangeSupported()) {
            this._attachDimensionChangeHandlers()
        }
        if (this._isVisibilityChangeSupported()) {
            var $element = this.$element();
            $element.addClass("dx-visibility-change-handler")
        }
    },
    _renderDimensions() {
        var $element = this.$element();
        var element = $element.get(0);
        var width = this._getOptionValue("width", element);
        var height = this._getOptionValue("height", element);
        if (this._isCssUpdateRequired(element, height, width)) {
            $element.css({
                width: null === width ? "" : width,
                height: null === height ? "" : height
            })
        }
    },
    _isCssUpdateRequired: (element, height, width) => !!(isDefined(width) || isDefined(height) || element.style.width || element.style.height),
    _attachDimensionChangeHandlers() {
        var $el = this.$element();
        var namespace = "".concat(this.NAME, "VisibilityChange");
        resizeEvent.off($el, {
            namespace: namespace
        });
        resizeEvent.on($el, () => this._dimensionChanged(), {
            namespace: namespace
        })
    },
    _attachVisibilityChangeHandlers() {
        if (this._isVisibilityChangeSupported()) {
            var $el = this.$element();
            var namespace = "".concat(this.NAME, "VisibilityChange");
            this._isHidden = !this._isVisible();
            visibilityEvents.off($el, {
                namespace: namespace
            });
            visibilityEvents.on($el, () => this._checkVisibilityChanged("shown"), () => this._checkVisibilityChanged("hiding"), {
                namespace: namespace
            })
        }
    },
    _isVisible() {
        var $element = this.$element();
        return $element.is(":visible")
    },
    _checkVisibilityChanged(action) {
        var isVisible = this._isVisible();
        if (isVisible) {
            if ("hiding" === action && !this._isHidden) {
                this._visibilityChanged(false);
                this._isHidden = true
            } else if ("shown" === action && this._isHidden) {
                this._isHidden = false;
                this._visibilityChanged(true)
            }
        }
    },
    _isVisibilityChangeSupported() {
        return this._visibilityChanged !== abstract && hasWindow()
    },
    _clean: noop,
    _modelByElement() {
        var {
            modelByElement: modelByElement
        } = this.option();
        var $element = this.$element();
        return modelByElement ? modelByElement($element) : void 0
    },
    _invalidate() {
        if (this._isUpdateAllowed()) {
            throw errors.Error("E0007")
        }
        this._requireRefresh = true
    },
    _refresh() {
        this._clean();
        this._renderComponent()
    },
    _dispose() {
        this._templateManager && this._templateManager.dispose();
        this.callBase();
        this._clean();
        this._detachWindowResizeCallback()
    },
    _detachWindowResizeCallback() {
        if (this._isDimensionChangeSupported()) {
            windowResizeCallbacks.remove(this._windowResizeCallBack)
        }
    },
    _toggleRTLDirection(rtl) {
        var $element = this.$element();
        $element.toggleClass("dx-rtl", rtl)
    },
    _createComponent(element, component) {
        var config = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : {};
        var synchronizableOptions = grep(this._getSynchronizableOptionsForCreateComponent(), value => !(value in config));
        var {
            integrationOptions: integrationOptions
        } = this.option();
        var {
            nestedComponentOptions: nestedComponentOptions
        } = this.option();
        nestedComponentOptions = nestedComponentOptions || noop;
        var nestedComponentConfig = extend({
            integrationOptions: integrationOptions
        }, nestedComponentOptions(this));
        synchronizableOptions.forEach(optionName => nestedComponentConfig[optionName] = this.option(optionName));
        this._extendConfig(config, nestedComponentConfig);
        var instance = void 0;
        if (isString(component)) {
            var $element = $(element)[component](config);
            instance = $element[component]("instance")
        } else if (element) {
            instance = component.getInstance(element);
            if (instance) {
                instance.option(config)
            } else {
                instance = new component(element, config)
            }
        }
        if (instance) {
            var optionChangedHandler = _ref => {
                var {
                    name: name,
                    value: value
                } = _ref;
                if (inArray(name, synchronizableOptions) >= 0) {
                    instance.option(name, value)
                }
            };
            this.on("optionChanged", optionChangedHandler);
            instance.on("disposing", () => this.off("optionChanged", optionChangedHandler))
        }
        return instance
    },
    _extendConfig(config, extendConfig) {
        each(extendConfig, (key, value) => {
            !Object.prototype.hasOwnProperty.call(config, key) && (config[key] = value)
        })
    },
    _defaultActionConfig() {
        var $element = this.$element();
        var context = this._modelByElement($element);
        return extend(this.callBase(), {
            context: context
        })
    },
    _defaultActionArgs() {
        var $element = this.$element();
        var model = this._modelByElement($element);
        var element = this.element();
        return extend(this.callBase(), {
            element: element,
            model: model
        })
    },
    _optionChanged(args) {
        switch (args.name) {
            case "width":
            case "height":
                this._renderDimensions();
                break;
            case "rtlEnabled":
                this._invalidate();
                break;
            case "elementAttr":
                this._renderElementAttributes();
                break;
            case "disabled":
            case "integrationOptions":
                break;
            default:
                this.callBase(args)
        }
    },
    _removeAttributes(element) {
        var attrs = element.attributes;
        for (var i = attrs.length - 1; i >= 0; i--) {
            var attr = attrs[i];
            if (attr) {
                var {
                    name: name
                } = attr;
                if (!name.indexOf("aria-") || -1 !== name.indexOf("dx-") || "role" === name || "style" === name || "tabindex" === name) {
                    element.removeAttribute(name)
                }
            }
        }
    },
    _removeClasses(element) {
        element.className = element.className.split(" ").filter(cssClass => 0 !== cssClass.lastIndexOf("dx-", 0)).join(" ")
    },
    _updateDOMComponent(renderRequired) {
        if (renderRequired) {
            this._renderComponent()
        } else if (this._requireRefresh) {
            this._requireRefresh = false;
            this._refresh()
        }
    },
    endUpdate() {
        var renderRequired = this._isInitializingRequired();
        this.callBase();
        this._isUpdateAllowed() && this._updateDOMComponent(renderRequired)
    },
    $element() {
        return this._$element
    },
    element() {
        var $element = this.$element();
        return getPublicElement($element)
    },
    dispose() {
        var element = this.$element().get(0);
        cleanDataRecursive(element, true);
        element.textContent = "";
        this._removeAttributes(element);
        this._removeClasses(element)
    },
    resetOption(optionName) {
        this.callBase(optionName);
        if ("width" === optionName || "height" === optionName) {
            var initialOption = this.initialOption(optionName);
            !isDefined(initialOption) && this.$element().css(optionName, "")
        }
    },
    _getAnonymousTemplateName() {
        return
    },
    _initTemplateManager() {
        if (this._templateManager || !this._useTemplates()) {
            return
        }
        var {
            integrationOptions: integrationOptions = {}
        } = this.option();
        var {
            createTemplate: createTemplate
        } = integrationOptions;
        this._templateManager = new TemplateManager(createTemplate, this._getAnonymousTemplateName());
        this._initTemplates()
    },
    _initTemplates() {
        var {
            templates: templates,
            anonymousTemplateMeta: anonymousTemplateMeta
        } = this._templateManager.extractTemplates(this.$element());
        var anonymousTemplate = this.option("integrationOptions.templates.".concat(anonymousTemplateMeta.name));
        templates.forEach(_ref2 => {
            var {
                name: name,
                template: template
            } = _ref2;
            this._options.silent("integrationOptions.templates.".concat(name), template)
        });
        if (anonymousTemplateMeta.name && !anonymousTemplate) {
            this._options.silent("integrationOptions.templates.".concat(anonymousTemplateMeta.name), anonymousTemplateMeta.template);
            this._options.silent("_hasAnonymousTemplateContent", true)
        }
    },
    _getTemplateByOption(optionName) {
        return this._getTemplate(this.option(optionName))
    },
    _getTemplate(templateSource) {
        var templates = this.option("integrationOptions.templates");
        var isAsyncTemplate = this.option("templatesRenderAsynchronously");
        var skipTemplates = this.option("integrationOptions.skipTemplates");
        return this._templateManager.getTemplate(templateSource, templates, {
            isAsyncTemplate: isAsyncTemplate,
            skipTemplates: skipTemplates
        }, this)
    },
    _saveTemplate(name, template) {
        this._setOptionWithoutOptionChange("integrationOptions.templates." + name, this._templateManager._createTemplate(template))
    },
    _useTemplates: () => true
});
DOMComponent.getInstance = function(element) {
    return getInstanceByElement($(element), this)
};
DOMComponent.defaultOptions = function(rule) {
    this._classCustomRules = this._classCustomRules || [];
    this._classCustomRules.push(rule)
};
export default DOMComponent;
