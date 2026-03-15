/**
 * DevExtreme (esm/ui/html_editor/formats/mention.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Quill from "devextreme-quill";
import $ from "../../../core/renderer";
var Mention = {};
if (Quill) {
    var Embed = Quill.import("blots/embed");
    var MENTION_CLASS = "dx-mention";
    Mention = class extends Embed {
        static create(data) {
            var node = super.create();
            node.setAttribute("spellcheck", false);
            node.dataset.marker = data.marker;
            node.dataset.mentionValue = data.value;
            node.dataset.id = data.id;
            this.renderContent(node, data);
            return node
        }
        static value(node) {
            return {
                marker: node.dataset.marker,
                id: node.dataset.id,
                value: node.dataset.mentionValue
            }
        }
        static renderContent(node, data) {
            var template = this._templates.get(data.marker);
            if (template) {
                template.render({
                    model: data,
                    container: node
                })
            } else {
                this.baseContentRender(node, data)
            }
        }
        static baseContentRender(node, data) {
            var $marker = $("<span>").text(data.marker);
            $(node).append($marker).append(data.value)
        }
        static addTemplate(marker, template) {
            this._templates.set(marker, template)
        }
        static removeTemplate(marker) {
            this._templates.delete(marker)
        }
    };
    Mention.blotName = "mention";
    Mention.tagName = "span";
    Mention.className = MENTION_CLASS;
    Mention._templates = new Map
}
export default Mention;
