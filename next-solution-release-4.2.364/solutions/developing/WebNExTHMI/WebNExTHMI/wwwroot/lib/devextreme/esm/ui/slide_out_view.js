/**
 * DevExtreme (esm/ui/slide_out_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth
} from "../core/utils/size";
import $ from "../core/renderer";
import eventsEngine from "../events/core/events_engine";
import {
    noop
} from "../core/utils/common";
import {
    name as clickEventName
} from "../events/click";
import {
    move
} from "../animation/translator";
import {
    getPublicElement
} from "../core/element";
import {
    hideCallback as hideTopOverlayCallback
} from "../mobile/hide_callback";
import registerComponent from "../core/component_registrator";
import {
    extend
} from "../core/utils/extend";
import Widget from "./widget/ui.widget";
import Swipeable from "../events/gesture/swipeable";
import {
    EmptyTemplate
} from "../core/templates/empty_template";
import {
    Deferred
} from "../core/utils/deferred";
import {
    hasWindow
} from "../core/utils/window";
import {
    animation
} from "./slide_out_view/ui.slide_out_view.animation";
var SLIDEOUTVIEW_CLASS = "dx-slideoutview";
var SLIDEOUTVIEW_WRAPPER_CLASS = "dx-slideoutview-wrapper";
var SLIDEOUTVIEW_MENU_CONTENT_CLASS = "dx-slideoutview-menu-content";
var SLIDEOUTVIEW_CONTENT_CLASS = "dx-slideoutview-content";
var SLIDEOUTVIEW_SHIELD_CLASS = "dx-slideoutview-shield";
var INVISIBLE_STATE_CLASS = "dx-state-invisible";
var ANONYMOUS_TEMPLATE_NAME = "content";
var SlideOutView = Widget.inherit({
    ctor: function(element, options) {
        this.callBase(element, options);
        this._logDeprecatedComponentWarning("20.1", "dxDrawer")
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            menuPosition: "normal",
            menuVisible: false,
            swipeEnabled: true,
            menuTemplate: "menu",
            contentTemplate: "content",
            contentOffset: 45
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: {
                android: true
            },
            options: {
                contentOffset: 54
            }
        }, {
            device: function(_device) {
                return "generic" === _device.platform && "desktop" !== _device.deviceType
            },
            options: {
                contentOffset: 56
            }
        }, {
            device: {
                win: true,
                phone: false
            },
            options: {
                contentOffset: 76
            }
        }])
    },
    _init: function() {
        this.callBase();
        this.$element().addClass(SLIDEOUTVIEW_CLASS);
        this._whenAnimationComplete = void 0;
        this._whenMenuRendered = void 0;
        this._initHideTopOverlayHandler()
    },
    _initHideTopOverlayHandler: function() {
        this._hideMenuHandler = this.hideMenu.bind(this)
    },
    _getAnonymousTemplateName: function() {
        return ANONYMOUS_TEMPLATE_NAME
    },
    _initTemplates: function() {
        this._templateManager.addDefaultTemplates({
            menu: new EmptyTemplate,
            content: new EmptyTemplate
        });
        this.callBase()
    },
    _initMarkup: function() {
        this.callBase();
        this._renderMarkup();
        this._whenMenuRendered = new Deferred;
        var menuTemplate = this._getTemplate(this.option("menuTemplate"));
        menuTemplate && menuTemplate.render({
            container: this.menuContent(),
            onRendered: () => {
                this._whenMenuRendered.resolve()
            }
        });
        var contentTemplateOption = this.option("contentTemplate");
        var contentTemplate = this._getTemplate(contentTemplateOption);
        var transclude = this._templateManager.anonymousTemplateName === contentTemplateOption;
        contentTemplate && contentTemplate.render({
            container: this.content(),
            noModel: true,
            transclude: transclude
        });
        this._renderShield();
        this._toggleMenuPositionClass()
    },
    _render: function() {
        this.callBase();
        this._whenMenuRendered.always(() => {
            this._initSwipeHandlers();
            this._dimensionChanged()
        })
    },
    _renderMarkup: function() {
        var $wrapper = $("<div>").addClass(SLIDEOUTVIEW_WRAPPER_CLASS);
        this._$menu = $("<div>").addClass(SLIDEOUTVIEW_MENU_CONTENT_CLASS);
        this._$container = $("<div>").addClass(SLIDEOUTVIEW_CONTENT_CLASS);
        $wrapper.append(this._$menu);
        $wrapper.append(this._$container);
        this.$element().append($wrapper);
        eventsEngine.on(this._$container, "MSPointerDown", noop)
    },
    _renderShield: function() {
        this._$shield = this._$shield || $("<div>").addClass(SLIDEOUTVIEW_SHIELD_CLASS);
        this._$shield.appendTo(this.content());
        eventsEngine.off(this._$shield, clickEventName);
        eventsEngine.on(this._$shield, clickEventName, this.hideMenu.bind(this));
        this._toggleShieldVisibility(this.option("menuVisible"))
    },
    _initSwipeHandlers: function() {
        this._createComponent($(this.content()), Swipeable, {
            disabled: !this.option("swipeEnabled"),
            elastic: false,
            itemSizeFunc: this._getMenuWidth.bind(this),
            onStart: this._swipeStartHandler.bind(this),
            onUpdated: this._swipeUpdateHandler.bind(this),
            onEnd: this._swipeEndHandler.bind(this)
        })
    },
    _isRightMenuPosition: function() {
        var invertedPosition = "inverted" === this.option("menuPosition");
        var rtl = this.option("rtlEnabled");
        return rtl && !invertedPosition || !rtl && invertedPosition
    },
    _swipeStartHandler: function(e) {
        animation.complete($(this.content()));
        var event = e.event;
        var menuVisible = this.option("menuVisible");
        var rtl = this._isRightMenuPosition();
        event.maxLeftOffset = +(rtl ? !menuVisible : menuVisible);
        event.maxRightOffset = +(rtl ? menuVisible : !menuVisible);
        this._toggleShieldVisibility(true)
    },
    _swipeUpdateHandler: function(e) {
        var event = e.event;
        var offset = this.option("menuVisible") ? event.offset + 1 * this._getRTLSignCorrection() : event.offset;
        offset *= this._getRTLSignCorrection();
        this._renderPosition(offset, false)
    },
    _swipeEndHandler: function(e) {
        var targetOffset = e.event.targetOffset * this._getRTLSignCorrection() + this.option("menuVisible");
        var menuVisible = 0 !== targetOffset;
        if (this.option("menuVisible") === menuVisible) {
            this._renderPosition(this.option("menuVisible"), true)
        } else {
            this.option("menuVisible", menuVisible)
        }
    },
    _toggleMenuPositionClass: function() {
        var left = SLIDEOUTVIEW_CLASS + "-left";
        var right = SLIDEOUTVIEW_CLASS + "-right";
        var menuPosition = this._isRightMenuPosition() ? "right" : "left";
        this._$menu.removeClass(left + " " + right);
        this._$menu.addClass(SLIDEOUTVIEW_CLASS + "-" + menuPosition)
    },
    _renderPosition: function(offset, animate) {
        if (!hasWindow()) {
            return
        }
        var pos = this._calculatePixelOffset(offset) * this._getRTLSignCorrection();
        this._toggleHideMenuCallback(offset);
        if (animate) {
            this._toggleShieldVisibility(true);
            animation.moveTo($(this.content()), pos, this._animationCompleteHandler.bind(this))
        } else {
            move($(this.content()), {
                left: pos
            })
        }
    },
    _calculatePixelOffset: function(offset) {
        offset = offset || 0;
        return offset * this._getMenuWidth()
    },
    _getMenuWidth: function() {
        if (!this._menuWidth) {
            var maxMenuWidth = getWidth(this.$element()) - this.option("contentOffset");
            var menuContent = $(this.menuContent());
            menuContent.css("maxWidth", maxMenuWidth < 0 ? 0 : maxMenuWidth);
            var currentMenuWidth = getWidth(menuContent);
            this._menuWidth = Math.min(currentMenuWidth, maxMenuWidth)
        }
        return this._menuWidth
    },
    _animationCompleteHandler: function() {
        this._toggleShieldVisibility(this.option("menuVisible"));
        if (this._whenAnimationComplete) {
            this._whenAnimationComplete.resolveWith(this)
        }
    },
    _toggleHideMenuCallback: function(subscribe) {
        if (subscribe) {
            hideTopOverlayCallback.add(this._hideMenuHandler)
        } else {
            hideTopOverlayCallback.remove(this._hideMenuHandler)
        }
    },
    _getRTLSignCorrection: function() {
        return this._isRightMenuPosition() ? -1 : 1
    },
    _dispose: function() {
        animation.complete($(this.content()));
        this._toggleHideMenuCallback(false);
        this.callBase()
    },
    _visibilityChanged: function(visible) {
        if (visible) {
            this._dimensionChanged()
        }
    },
    _dimensionChanged: function() {
        delete this._menuWidth;
        this._renderPosition(this.option("menuVisible"), false)
    },
    _toggleShieldVisibility: function(visible) {
        this._$shield.toggleClass(INVISIBLE_STATE_CLASS, !visible)
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "width":
                this.callBase(args);
                this._dimensionChanged();
                break;
            case "contentOffset":
                this._dimensionChanged();
                break;
            case "menuVisible":
                this._renderPosition(args.value, true);
                break;
            case "menuPosition":
                this._renderPosition(this.option("menuVisible"), true);
                this._toggleMenuPositionClass();
                break;
            case "swipeEnabled":
                this._initSwipeHandlers();
                break;
            case "contentTemplate":
            case "menuTemplate":
                this._invalidate();
                break;
            default:
                this.callBase(args)
        }
    },
    menuContent: function() {
        return getPublicElement(this._$menu)
    },
    content: function() {
        return getPublicElement(this._$container)
    },
    showMenu: function() {
        return this.toggleMenuVisibility(true)
    },
    hideMenu: function() {
        return this.toggleMenuVisibility(false)
    },
    toggleMenuVisibility: function(showing) {
        showing = void 0 === showing ? !this.option("menuVisible") : showing;
        this._whenAnimationComplete = new Deferred;
        this.option("menuVisible", showing);
        return this._whenAnimationComplete.promise()
    }
});
registerComponent("dxSlideOutView", SlideOutView);
export default SlideOutView;
