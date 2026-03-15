/**
 * DevExtreme (esm/core/utils/queue.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import errors from "../errors";
import {
    when
} from "../../core/utils/deferred";

function createQueue(discardPendingTasks) {
    var _tasks = [];
    var _busy = false;

    function exec() {
        while (_tasks.length) {
            _busy = true;
            var task = _tasks.shift();
            var result = task();
            if (void 0 === result) {
                continue
            }
            if (result.then) {
                when(result).always(exec);
                return
            }
            throw errors.Error("E0015")
        }
        _busy = false
    }
    return {
        add: function(task, removeTaskCallback) {
            if (!discardPendingTasks) {
                _tasks.push(task)
            } else {
                if (_tasks[0] && removeTaskCallback) {
                    removeTaskCallback(_tasks[0])
                }
                _tasks = [task]
            }
            if (!_busy) {
                exec()
            }
        },
        busy: function() {
            return _busy
        }
    }
}
export {
    createQueue as create
};
export var enqueue = createQueue().add;
