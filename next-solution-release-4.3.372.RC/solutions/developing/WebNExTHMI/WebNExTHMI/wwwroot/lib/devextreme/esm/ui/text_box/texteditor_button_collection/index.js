/**
 * DevExtreme (esm/ui/text_box/texteditor_button_collection/index.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import CustomButton from "./custom";
import {
    extend
} from "../../../core/utils/extend";
import {
    find
} from "../../../core/utils/array";
import errors from "../../widget/ui.errors";
var TEXTEDITOR_BUTTONS_CONTAINER_CLASS = "dx-texteditor-buttons-container";

function checkButtonInfo(buttonInfo) {
    (() => {
        if (!buttonInfo || "object" !== typeof buttonInfo || Array.isArray(buttonInfo)) {
            throw errors.Error("E1053")
        }
    })();
    (() => {
        if (!("name" in buttonInfo)) {
            throw errors.Error("E1054")
        }
    })();
    (() => {
        var {
            name: name
        } = buttonInfo;
        if ("string" !== typeof name) {
            throw errors.Error("E1055")
        }
    })();
    (() => {
        var {
            location: location
        } = buttonInfo;
        if ("location" in buttonInfo && "after" !== location && "before" !== location) {
            buttonInfo.location = "after"
        }
    })()
}

function checkNamesUniqueness(existingNames, newName) {
    if (-1 !== existingNames.indexOf(newName)) {
        throw errors.Error("E1055", newName)
    }
    existingNames.push(newName)
}

function isPredefinedButtonName(name, predefinedButtonsInfo) {
    return !!find(predefinedButtonsInfo, info => info.name === name)
}
export default class TextEditorButtonCollection {
    constructor(editor, defaultButtonsInfo) {
        this.buttons = [];
        this.defaultButtonsInfo = defaultButtonsInfo;
        this.editor = editor
    }
    _compileButtonInfo(buttons) {
        var names = [];
        return buttons.map(button => {
            var isStringButton = "string" === typeof button;
            if (!isStringButton) {
                checkButtonInfo(button)
            }
            var isDefaultButton = isStringButton || isPredefinedButtonName(button.name, this.defaultButtonsInfo);
            if (isDefaultButton) {
                var defaultButtonInfo = find(this.defaultButtonsInfo, _ref => {
                    var {
                        name: name
                    } = _ref;
                    return name === button || name === button.name
                });
                if (!defaultButtonInfo) {
                    throw errors.Error("E1056", this.editor.NAME, button)
                }
                checkNamesUniqueness(names, button);
                return defaultButtonInfo
            } else {
                var {
                    name: name
                } = button;
                checkNamesUniqueness(names, name);
                return extend(button, {
                    Ctor: CustomButton
                })
            }
        })
    }
    _createButton(buttonsInfo) {
        var {
            Ctor: Ctor,
            options: options,
            name: name
        } = buttonsInfo;
        var button = new Ctor(name, this.editor, options);
        this.buttons.push(button);
        return button
    }
    _renderButtons(buttons, $container, targetLocation) {
        var $buttonsContainer = null;
        var buttonsInfo = buttons ? this._compileButtonInfo(buttons) : this.defaultButtonsInfo;
        buttonsInfo.forEach(buttonsInfo => {
            var {
                location: location = "after"
            } = buttonsInfo;
            if (location === targetLocation) {
                this._createButton(buttonsInfo).render((() => {
                    $buttonsContainer = $buttonsContainer || $("<div>").addClass(TEXTEDITOR_BUTTONS_CONTAINER_CLASS);
                    "before" === targetLocation ? $container.prepend($buttonsContainer) : $container.append($buttonsContainer);
                    return $buttonsContainer
                })())
            }
        });
        return $buttonsContainer
    }
    clean() {
        this.buttons.forEach(button => button.dispose());
        this.buttons = []
    }
    getButton(buttonName) {
        var button = find(this.buttons, _ref2 => {
            var {
                name: name
            } = _ref2;
            return name === buttonName
        });
        return button && button.instance
    }
    renderAfterButtons(buttons, $container) {
        return this._renderButtons(buttons, $container, "after")
    }
    renderBeforeButtons(buttons, $container) {
        return this._renderButtons(buttons, $container, "before")
    }
    updateButtons(names) {
        this.buttons.forEach(button => {
            if (!names || -1 !== names.indexOf(button.name)) {
                button.update()
            }
        })
    }
}
