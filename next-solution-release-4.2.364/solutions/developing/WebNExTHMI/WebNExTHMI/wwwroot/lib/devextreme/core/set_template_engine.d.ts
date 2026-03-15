/**
* DevExtreme (core/set_template_engine.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * Sets a supported template engine to use when using jQuery.
 */
declare function setTemplateEngine(templateEngineName: string): void;

/**
 * Sets custom functions that compile and render templates.
 */
declare function setTemplateEngine(templateEngineOptions: { compile?: Function; render?: Function }): void;

export default setTemplateEngine;
