/**
 * DevExtreme (esm/ui/shared/accessibility.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import {
    normalizeKeyName
} from "../../events/utils/index";
import {
    extend
} from "../../core/utils/extend";
import domAdapter from "../../core/dom_adapter";
import {
    noop
} from "../../core/utils/common";
var FOCUS_STATE_CLASS = "dx-state-focused";
var FOCUS_DISABLED_CLASS = "dx-cell-focus-disabled";
var FOCUSED_ROW_SELECTOR = ".dx-row-focused";
var GRID_ROW_SELECTOR = ".dx-datagrid-rowsview .dx-row";
var GRID_CELL_SELECTOR = "".concat(GRID_ROW_SELECTOR, " > td");
var TREELIST_ROW_SELECTOR = ".dx-treelist-rowsview .dx-row";
var TREELIST_CELL_SELECTOR = "".concat(TREELIST_ROW_SELECTOR, " > td");
var viewItemSelectorMap = {
    groupPanel: [".dx-datagrid-group-panel .dx-group-panel-item[tabindex]"],
    columnHeaders: [".dx-datagrid-headers .dx-header-row > td.dx-datagrid-action", ".dx-treelist-headers .dx-header-row > td.dx-treelist-action"],
    filterRow: [".dx-datagrid-headers .dx-datagrid-filter-row .dx-editor-cell .dx-texteditor-input", ".dx-treelist-headers .dx-treelist-filter-row .dx-editor-cell .dx-texteditor-input"],
    rowsView: ["".concat(FOCUSED_ROW_SELECTOR), "".concat(GRID_ROW_SELECTOR, "[tabindex]"), "".concat(GRID_CELL_SELECTOR, "[tabindex]"), "".concat(GRID_CELL_SELECTOR), "".concat(TREELIST_ROW_SELECTOR, "[tabindex]"), "".concat(TREELIST_CELL_SELECTOR, "[tabindex]"), "".concat(TREELIST_CELL_SELECTOR)],
    footer: [".dx-datagrid-total-footer .dx-datagrid-summary-item", ".dx-treelist-total-footer .dx-treelist-summary-item"],
    filterPanel: [".dx-datagrid-filter-panel .dx-icon-filter", ".dx-treelist-filter-panel .dx-icon-filter"],
    pager: [".dx-datagrid-pager [tabindex]", ".dx-treelist-pager [tabindex]"]
};
var isMouseDown = false;
var isHiddenFocusing = false;
var focusedElementInfo = null;

function processKeyDown(viewName, instance, event, action, $mainElement, executeKeyDown) {
    var isHandled = fireKeyDownEvent(instance, event.originalEvent, executeKeyDown);
    if (isHandled) {
        return
    }
    var keyName = normalizeKeyName(event);
    if ("enter" === keyName || "space" === keyName) {
        saveFocusedElementInfo(event.target, instance);
        action && action({
            event: event
        })
    } else if ("tab" === keyName) {
        $mainElement.addClass(FOCUS_STATE_CLASS)
    } else {
        selectView(viewName, instance, event)
    }
}

function saveFocusedElementInfo(target, instance) {
    var $target = $(target);
    var ariaLabel = $target.attr("aria-label");
    var $activeElements = getActiveAccessibleElements(ariaLabel, instance.element());
    var targetIndex = $activeElements.index($target);
    focusedElementInfo = extend({}, {
        ariaLabel: ariaLabel,
        index: targetIndex
    }, {
        viewInstance: instance
    })
}

function getActiveAccessibleElements(ariaLabel, viewElement) {
    var $viewElement = $(viewElement);
    var $activeElements;
    if (ariaLabel) {
        $activeElements = $viewElement.find('[aria-label="'.concat(ariaLabel, '"][tabindex]'))
    } else {
        $activeElements = $viewElement.find("[tabindex]")
    }
    return $activeElements
}

function findFocusedViewElement(viewSelectors) {
    for (var index in viewSelectors) {
        var selector = viewSelectors[index];
        var $focusViewElement = $(selector).first();
        if ($focusViewElement.length) {
            return $focusViewElement
        }
    }
}

function fireKeyDownEvent(instance, event, executeAction) {
    var args = {
        event: event,
        handled: false
    };
    if (executeAction) {
        executeAction(args)
    } else {
        instance._createActionByOption("onKeyDown")(args)
    }
    return args.handled
}

function onDocumentVisibilityChange() {
    isHiddenFocusing = "visible" === domAdapter.getDocument().visibilityState
}
export function subscribeVisibilityChange() {
    eventsEngine.on(domAdapter.getDocument(), "visibilitychange", onDocumentVisibilityChange)
}
export function unsubscribeVisibilityChange() {
    eventsEngine.off(domAdapter.getDocument(), "visibilitychange", onDocumentVisibilityChange)
}
export function hiddenFocus(element) {
    isHiddenFocusing = true;
    element.focus();
    isHiddenFocusing = false
}
export function registerKeyboardAction(viewName, instance, $element, selector, action, executeKeyDown) {
    if (instance.option("useLegacyKeyboardNavigation")) {
        return noop
    }
    var getMainElement = () => $(instance.element());
    var keyDownHandler = e => processKeyDown(viewName, instance, e, action, getMainElement(), executeKeyDown);
    var mouseDownHandler = () => {
        isMouseDown = true;
        getMainElement().removeClass(FOCUS_STATE_CLASS)
    };
    var focusinHandler = () => {
        var needShowOverlay = !isMouseDown && !isHiddenFocusing;
        if (needShowOverlay) {
            getMainElement().addClass(FOCUS_STATE_CLASS)
        }
        isMouseDown = false
    };
    eventsEngine.on($element, "keydown", selector, keyDownHandler);
    eventsEngine.on($element, "mousedown", selector, mouseDownHandler);
    eventsEngine.on($element, "focusin", selector, focusinHandler);
    return () => {
        eventsEngine.off($element, "keydown", selector, keyDownHandler);
        eventsEngine.off($element, "mousedown", selector, mouseDownHandler);
        eventsEngine.off($element, "focusin", selector, focusinHandler)
    }
}
export function restoreFocus(instance) {
    if (!instance.option("useLegacyKeyboardNavigation") && focusedElementInfo) {
        var viewInstance = focusedElementInfo.viewInstance;
        if (viewInstance) {
            var $activeElements = getActiveAccessibleElements(focusedElementInfo.ariaLabel, viewInstance.element());
            var $targetElement = $activeElements.eq(focusedElementInfo.index);
            focusedElementInfo = null;
            eventsEngine.trigger($targetElement, "focus")
        }
    }
}
export function selectView(viewName, instance, event) {
    var keyName = normalizeKeyName(event);
    if (event.ctrlKey && ("upArrow" === keyName || "downArrow" === keyName)) {
        var viewNames = Object.keys(viewItemSelectorMap);
        var viewItemIndex = viewNames.indexOf(viewName);
        while (viewItemIndex >= 0 && viewItemIndex < viewNames.length) {
            viewItemIndex = "upArrow" === keyName ? --viewItemIndex : ++viewItemIndex;
            var _viewName = viewNames[viewItemIndex];
            var viewSelectors = viewItemSelectorMap[_viewName];
            var $focusViewElement = findFocusedViewElement(viewSelectors);
            if ($focusViewElement && $focusViewElement.length) {
                $focusViewElement.attr("tabindex", instance.option("tabindex") || 0);
                eventsEngine.trigger($focusViewElement, "focus");
                $focusViewElement.removeClass(FOCUS_DISABLED_CLASS);
                break
            }
        }
    }
}
export function setTabIndex(instance, $element) {
    if (!instance.option("useLegacyKeyboardnavigation")) {
        $element.attr("tabindex", instance.option("tabindex") || 0)
    }
}
