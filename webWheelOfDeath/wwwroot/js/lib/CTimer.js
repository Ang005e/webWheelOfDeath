'use strict';

// Description: Statuses for the CTimer class. Return type for CTimer.status read-only property.
export const EnumTimerStatus = Object.freeze({
    NotStarted: 0,
    Running: 1,
    Completed: 2,
    Cancelled: 3
});


// Date: 03 June 2023
// Author: Ramin Majidi
// Description: Reusable JavaScript class CTimer
//
// Properties
//      duration: Duration of the timer (in milliseconds).
//      tickInterval: The interval (in milliseconds) at which progress method timerTicked() is repeatedly triggered.
//
// Virtual Methods for extension override implementation:
//      started()
//      timerTicked()
//      timerCompleted()
//      cancelled()
//
// The started() method fires just prior to the timer running.
//
// If tickInterval exceeds duration, then the timerTicked() method may not fire.
// If tickInterval is zero, then the timerTicked() method will not fire.
//
// The timerCompleted() method fires just after the timer completes.
// The cancelled() method fires only when the cancel() method is invoked to interrupt the timer.
export class CTimer {
    //----------------------------------------------------------------------
    // Private Backing Fields...
    //----------------------------------------------------------------------
    #id;
    #timeAtStart;
    #tickCount;
    #status = EnumTimerStatus.NotStarted;

    //----------------------------------------------------------------------
    // Constructor...
    //----------------------------------------------------------------------
    constructor(duration, tickInterval = 0) {
        this.duration = duration;
        this.tickInterval = tickInterval;
    }


    //----------------------------------------------------------------------
    // Properties
    //----------------------------------------------------------------------
    get #systemTimeNow() {
        return new Date().getTime();
    }

    get tickCount() {
        return this.#tickCount;
    }

    get elapsedTime() {
        return (this.#status === EnumTimerStatus.NotStarted) ? 0: this.#systemTimeNow - this.#timeAtStart;
    }

    get remainingTime() {
        return this.duration - this.elapsedTime;
    }

    get isRunning() {
        return this.status === EnumTimerStatus.Running;
    }

    get status() {
        return this.#status;
    }

    //----------------------------------------------------------------------
    // Methods
    //----------------------------------------------------------------------
    #tick() {
        this.#tickCount++;

        if (this.remainingTime <= 0) {
            clearInterval(this.#id);
            this.#status = EnumTimerStatus.Completed;
            this.timerCompleted();
        } else {
            this.timerTicked();
        }
    }

    #timeout() {
        clearTimeout(this.#id);
        this.#status = EnumTimerStatus.Completed;
        this.timerCompleted();
    }


    start() {

        this.#status = EnumTimerStatus.NotStarted;

        if (this.tickInterval < 0) {
            throw new Error('tickInterval may not be a negative value');
        }
        else if (this.duration < 0) {
            throw new Error('duration may not be a negative value');
        }
        else if (this.tickInterval > this.duration) {
            throw new Error('tickInterval may not exceed duration');
        }

        this.#status = EnumTimerStatus.Running;
        this.#tickCount = 0;
        this.#timeAtStart = this.#systemTimeNow;
        this.started();

        // Using 'this.xxx' doesn't work as a callback handler reference. Must use arrow function handler instead.
        // this.#id = setInterval(this.#tick, this.tickInterval);
        // See: https://developer.mozilla.org/en-US/docs/Web/API/setInterval#the_this_problem

        if (this.tickInterval > 0)
        {
            this.#id = setInterval(() => (this.#tick()), this.tickInterval);
        } else {
            this.#id = setTimeout(() => (this.#timeout()), this.duration);
        }

        return this;
    }

    cancel() {
        try {
            if (this.isRunning) {
                if (this.tickInterval > 0) {
                    clearInterval(this.#id);
                } else {
                    clearTimeout(this.#id);
                }

                this.#status = EnumTimerStatus.Cancelled;
                this.cancelled();
            }
        }
        finally { }
    }

    //----------------------------------------------------------------------
    // Virtual Methods - implementation to be overridden by extended classes
    //----------------------------------------------------------------------

    // Method that runs prior to the timer being primed.
    started() { }

    // Method that repeatedly runs at the set interval.
    timerTicked() { }

    // Method that runs once the specified duration has elapsed.
    timerCompleted() { }

    // Method that runs only if the cancel() method has been invoked while the timer is running.
    cancelled() { }
}

