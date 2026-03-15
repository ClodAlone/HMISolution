/**
 * DevExtreme (esm/renovation/ui/scheduler/semaphore.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export class Semaphore {
    constructor() {
        this.counter = 0
    }
    isFree() {
        return 0 === this.counter
    }
    take() {
        this.counter += 1
    }
    release() {
        this.counter -= 1;
        if (this.counter < 0) {
            this.counter = 0
        }
    }
}
