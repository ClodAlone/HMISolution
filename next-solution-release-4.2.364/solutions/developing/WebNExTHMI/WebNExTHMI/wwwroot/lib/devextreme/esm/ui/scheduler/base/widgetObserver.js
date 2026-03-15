/**
 * DevExtreme (esm/ui/scheduler/base/widgetObserver.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Widget from "../../widget/ui.widget";
class WidgetObserver extends Widget {
    notifyObserver(subject, args) {
        var observer = this.option("observer");
        if (observer) {
            observer.fire(subject, args)
        }
    }
    invoke() {
        var observer = this.option("observer");
        if (observer) {
            return observer.fire.apply(observer, arguments)
        }
    }
}
export default WidgetObserver;
