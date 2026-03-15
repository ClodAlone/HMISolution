/**
* DevExtreme (core/dom_adapter.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DomAdapter {
  getActiveElement(): HTMLElement;
  getDocument(): Document;
  getDocumentElement(): HTMLDocument & {
    scrollLeft: number;
    scrollTop: number;
    clientWidth: number;
    scrollHeight: number;
    offsetHeight: number;
    clientHeight: number;
  };
  isNode(node: unknown): boolean;
  getBody(): HTMLBodyElement;
  isElementNode(element: unknown): boolean;
  createElement(tagName: string, context?: Document): HTMLElement;
  createDocumentFragment(): DocumentFragment;
  setClass(element: HTMLElement, className: string, isAdd: boolean): void;
  removeElement(element: HTMLElement): void;
  inject(obj: Record<string, unknown>): void;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
declare const domAdapter: DomAdapter;
export default domAdapter;
