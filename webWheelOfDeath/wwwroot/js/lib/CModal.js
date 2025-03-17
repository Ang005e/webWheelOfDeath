'use strict';
import * as Util from './Utilities.js';
import {CTimer} from "./CTimer.js";

//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------

export class CModal extends CTimer {
    #modalCanvas;
    #mainPanel;
    #modalClose;
    #hideEventName;

    constructor(modalCanvasSelector, outerCanvasClickClosesPopup = false, hideEventName = 'modal-hide') {
        super(0, 0);
        this.#modalCanvas = document.querySelector(modalCanvasSelector);
        this.#mainPanel = this.#modalCanvas.querySelector('.main-panel');
        this.#modalClose = this.#mainPanel.querySelector('.close-popup');
        this.#hideEventName = hideEventName

        // Wire-up event listeners...
        this.#modalClose.addEventListener('click', (event)=> {
            event.stopPropagation();    // prevent event bubbling up to parent(s)
            this.hide();
        });

        if (outerCanvasClickClosesPopup) {
            this.#modalCanvas.addEventListener('click', (event) => {
                if (event.target === this.#modalCanvas) {
                    this.hide();
                }
            });
        }
    }

    get modalCanvas() {
        return this.#modalCanvas;
    }

    get mainPanel() {
        return this.#mainPanel;
    }

    show(timeout = 0) {
        this.duration = timeout;

        Util.show(this.modalCanvas);

        if (timeout > 0) {
            this.start();   // start the timeout clock
        }
    }

    hide() {
        this.cancel();
        document.dispatchEvent(new CustomEvent(this.#hideEventName, {
            bubbles: true
        }));
        Util.hide(this.modalCanvas);
    }

    timerCompleted() {
        this.hide();
    }

}
