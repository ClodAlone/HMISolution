/**
 * DevExtreme (esm/renovation/ui/pager/resizable_container.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["contentTemplate", "pagerProps"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import resizeCallbacks from "../../../core/utils/resize_callbacks";
import {
    getElementWidth,
    getElementStyle
} from "./utils/get_element_width";
import {
    isDefined
} from "../../../core/utils/type";
export var viewFunction = _ref => {
    var {
        contentAttributes: contentAttributes,
        infoTextRef: infoTextRef,
        infoTextVisible: infoTextVisible,
        isLargeDisplayMode: isLargeDisplayMode,
        pageSizesRef: pageSizesRef,
        pagesRef: pagesRef,
        parentRef: parentRef,
        props: {
            contentTemplate: Content
        }
    } = _ref;
    return Content(_extends({
        rootElementRef: parentRef,
        pageSizesRef: pageSizesRef,
        infoTextRef: infoTextRef,
        pagesRef: pagesRef,
        infoTextVisible: infoTextVisible,
        isLargeDisplayMode: isLargeDisplayMode
    }, contentAttributes))
};
export function calculateAdaptivityProps(_ref2) {
    var {
        info: infoWidth,
        pageSizes: pageSizesWidth,
        pages: pagesWidth,
        parent: parentWidth
    } = _ref2;
    var minimalWidth = pageSizesWidth + pagesWidth + infoWidth;
    var infoTextVisible = parentWidth - minimalWidth > 0;
    var isLargeDisplayMode = parentWidth - (pageSizesWidth + pagesWidth) > 0;
    return {
        infoTextVisible: infoTextVisible,
        isLargeDisplayMode: isLargeDisplayMode
    }
}

function getElementsWidth(_ref3) {
    var {
        info: info,
        pageSizes: pageSizes,
        pages: pages,
        parent: parent
    } = _ref3;
    var parentWidth = getElementWidth(parent);
    var pageSizesWidth = getElementWidth(pageSizes);
    var infoWidth = getElementWidth(info);
    var pagesHtmlWidth = getElementWidth(pages);
    return {
        parent: parentWidth,
        pageSizes: pageSizesWidth,
        info: infoWidth + getElementStyle("marginLeft", info) + getElementStyle("marginRight", info),
        pages: pagesHtmlWidth
    }
}
export var ResizableContainerProps = {};
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class ResizableContainer extends InfernoComponent {
    constructor(props) {
        super(props);
        this.parentRef = infernoCreateRef();
        this.pageSizesRef = infernoCreateRef();
        this.infoTextRef = infernoCreateRef();
        this.pagesRef = infernoCreateRef();
        this.state = {
            infoTextVisible: true,
            isLargeDisplayMode: true
        };
        this.subscribeToResize = this.subscribeToResize.bind(this);
        this.effectUpdateChildProps = this.effectUpdateChildProps.bind(this);
        this.updateAdaptivityProps = this.updateAdaptivityProps.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.subscribeToResize, [this.state.infoTextVisible, this.state.isLargeDisplayMode]), new InfernoEffect(this.effectUpdateChildProps, [this.props, this.state.infoTextVisible, this.state.isLargeDisplayMode, this.props.pagerProps, this.props.contentTemplate])]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.state.infoTextVisible, this.state.isLargeDisplayMode]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props, this.state.infoTextVisible, this.state.isLargeDisplayMode, this.props.pagerProps, this.props.contentTemplate])
    }
    subscribeToResize() {
        var callback = () => {
            this.parentWidth > 0 && this.updateAdaptivityProps()
        };
        resizeCallbacks.add(callback);
        return () => {
            resizeCallbacks.remove(callback)
        }
    }
    effectUpdateChildProps() {
        if (this.parentWidth > 0) {
            this.updateAdaptivityProps()
        }
    }
    get contentAttributes() {
        var {
            className: className,
            displayMode: displayMode,
            gridCompatibility: gridCompatibility,
            hasKnownLastPage: hasKnownLastPage,
            infoText: infoText,
            lightModeEnabled: lightModeEnabled,
            maxPagesCount: maxPagesCount,
            onKeyDown: onKeyDown,
            pageCount: pageCount,
            pageIndex: pageIndex,
            pageIndexChange: pageIndexChange,
            pageSize: pageSize,
            pageSizeChange: pageSizeChange,
            pageSizes: pageSizes,
            pagesCountText: pagesCountText,
            pagesNavigatorVisible: pagesNavigatorVisible,
            rtlEnabled: rtlEnabled,
            showInfo: showInfo,
            showNavigationButtons: showNavigationButtons,
            showPageSizes: showPageSizes,
            totalCount: totalCount,
            visible: visible
        } = this.props.pagerProps;
        return _extends({}, this.restAttributes, {
            pageSize: pageSize,
            pageIndex: pageIndex,
            pageIndexChange: pageIndexChange,
            pageSizeChange: pageSizeChange,
            gridCompatibility: gridCompatibility,
            className: className,
            showInfo: showInfo,
            infoText: infoText,
            lightModeEnabled: lightModeEnabled,
            displayMode: displayMode,
            maxPagesCount: maxPagesCount,
            pageCount: pageCount,
            pagesCountText: pagesCountText,
            visible: visible,
            hasKnownLastPage: hasKnownLastPage,
            pagesNavigatorVisible: pagesNavigatorVisible,
            showPageSizes: showPageSizes,
            pageSizes: pageSizes,
            rtlEnabled: rtlEnabled,
            showNavigationButtons: showNavigationButtons,
            totalCount: totalCount,
            onKeyDown: onKeyDown
        })
    }
    get parentWidth() {
        return this.parentRef.current ? getElementWidth(this.parentRef.current) : 0
    }
    updateAdaptivityProps() {
        var currentElementsWidth = getElementsWidth({
            parent: this.parentRef.current,
            pageSizes: this.pageSizesRef.current,
            info: this.infoTextRef.current,
            pages: this.pagesRef.current
        });
        if (isDefined(this.actualAdaptivityProps) && (this.actualAdaptivityProps.infoTextVisible !== this.state.infoTextVisible || this.actualAdaptivityProps.isLargeDisplayMode !== this.state.isLargeDisplayMode)) {
            return
        }
        var isEmpty = !isDefined(this.elementsWidth);
        if (isEmpty) {
            this.elementsWidth = {}
        }
        if (isEmpty || this.state.isLargeDisplayMode) {
            this.elementsWidth.pageSizes = currentElementsWidth.pageSizes;
            this.elementsWidth.pages = currentElementsWidth.pages
        }
        if (isEmpty || this.state.infoTextVisible) {
            this.elementsWidth.info = currentElementsWidth.info
        }
        this.actualAdaptivityProps = calculateAdaptivityProps(_extends({
            parent: currentElementsWidth.parent
        }, this.elementsWidth));
        this.setState(__state_argument => ({
            infoTextVisible: this.actualAdaptivityProps.infoTextVisible
        }));
        this.setState(__state_argument => ({
            isLargeDisplayMode: this.actualAdaptivityProps.isLargeDisplayMode
        }))
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            infoTextVisible: this.state.infoTextVisible,
            isLargeDisplayMode: this.state.isLargeDisplayMode,
            parentRef: this.parentRef,
            pageSizesRef: this.pageSizesRef,
            infoTextRef: this.infoTextRef,
            pagesRef: this.pagesRef,
            contentAttributes: this.contentAttributes,
            parentWidth: this.parentWidth,
            updateAdaptivityProps: this.updateAdaptivityProps,
            restAttributes: this.restAttributes
        })
    }
}
ResizableContainer.defaultProps = ResizableContainerProps;
