/**
 * DevExtreme (esm/ui/html_editor/quill_registrator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getQuill
} from "./quill_importer";
import BaseTheme from "./themes/base";
import Image from "./formats/image";
import Link from "./formats/link";
import FontStyle from "./formats/font";
import SizeStyle from "./formats/size";
import AlignStyle from "./formats/align";
import Toolbar from "./modules/toolbar";
import DropImage from "./modules/dropImage";
import Variables from "./modules/variables";
import Resizing from "./modules/resizing";
import TableResizing from "./modules/tableResizing";
import TableContextMenu from "./modules/tableContextMenu";
import MentionsModule from "./modules/mentions";
class QuillRegistrator {
    constructor() {
        if (QuillRegistrator.initialized) {
            return
        }
        var quill = this.getQuill();
        var DirectionStyle = quill.import("attributors/style/direction");
        quill.register({
            "formats/align": AlignStyle,
            "formats/direction": DirectionStyle,
            "formats/font": FontStyle,
            "formats/size": SizeStyle,
            "formats/extendedImage": Image,
            "formats/link": Link,
            "modules/toolbar": Toolbar,
            "modules/dropImage": DropImage,
            "modules/variables": Variables,
            "modules/resizing": Resizing,
            "modules/tableResizing": TableResizing,
            "modules/tableContextMenu": TableContextMenu,
            "modules/mentions": MentionsModule,
            "themes/basic": BaseTheme
        }, true);
        this._customModules = [];
        QuillRegistrator._initialized = true
    }
    createEditor(container, config) {
        var quill = this.getQuill();
        return new quill(container, config)
    }
    registerModules(modulesConfig) {
        var isModule = RegExp("modules/*");
        var quill = this.getQuill();
        var isRegisteredModule = modulePath => !!quill.imports[modulePath];
        for (var modulePath in modulesConfig) {
            if (isModule.test(modulePath) && !isRegisteredModule(modulePath)) {
                this._customModules.push(modulePath.slice(8))
            }
        }
        quill.register(modulesConfig, true)
    }
    getRegisteredModuleNames() {
        return this._customModules
    }
    getQuill() {
        return getQuill()
    }
}
export default QuillRegistrator;
