'use strict';
import {CModal} from './lib/CModal.js';


//-------------------------------------------------------------------------------
//--------------------------------CMessageModal----------------------------------
//-------------------------------------------------------------------------------
export class CMessageModal extends CModal {
    #messageElem;
    constructor(modalCanvasSelector, outerCanvasClickClosesPopup = false) {
        super(modalCanvasSelector, outerCanvasClickClosesPopup);
        this.#messageElem = document.querySelector(`${modalCanvasSelector} .message-display`);
    }

    display(message, isHtml = true, timeout = 0) {
        if (isHtml) {
            this.#messageElem.innerHTML = message;
        } else {
            this.#messageElem.innerText = message;
        }

        super.show(timeout);
    }

    timerCompleted() {
        this.#messageElem.innerText = "";
        super.timerCompleted();
    }
}


//-------------------------------------------------------------------------------
//--------------------------------CLoginModal-----------------------------------
//-------------------------------------------------------------------------------
export class CLoginModal extends CModal {

    #form = this.mainPanel.querySelector('form');
    #callbackFunction = () => { };

    constructor(modalCanvasSelector, outerCanvasClickClosesPopup = false, onHideEventName /*, startupCallbackFunction = ()=>{}*/) {
        super(modalCanvasSelector, outerCanvasClickClosesPopup, onHideEventName);
        // this.startupCallbackFunction = startupCallbackFunction; KJBGHDBDFJKBNK
        this.playerUsername = '';
        this.playerPassword = '';
        this.startupCallbackFunction(); // KJBGHDBDFJKBNKKJBGHDBDFJKBNK
    }

    get callbackFunction() {
        return this.#callbackFunction;
    }
    set callbackFunction(value) {
        this.#callbackFunction = value;
    }
    startupCallbackFunction() {
        this.#form.btnBeginGame.addEventListener('click', event=> {
            //console.log(`event.cancelable: ${event.cancelable}`);
            event.preventDefault();     // As the button type is submit, this prevents postback to the server
            event.stopPropagation();    // prevent event bubbling up to parent(s)

            //this.#form.txtPlayerUsername.value = this.#form.txtPlayerUsername.value.trim();
            //this.#form.txtPlayerPassword.value = this.#form.txtPlayerPassword.value.trim();

            if (!this.#form.txtPlayerUsername.value) {
                this.#form.txtPlayerUsername.focus();
                return;
            }

            if (!this.#form.txtPlayerPassword.value) {
                this.#form.txtPlayerPassword.focus();
                return;
            }

            this.playerUsername = this.#form.txtPlayerUsername.value;
            this.playerPassword = this.#form.txtPlayerPassword.value;
            this.hide();

            this.#callbackFunction();     // invoke the callback function
        });
        this.#form.btnCreateAccount.addEventListener('click', event => {

            // dispatch a new event
            document.dispatchEvent(new CustomEvent("create-account", {
                bubbles: true //,
                // detail: no need for a detail here
            }))
        })
    }


    display(timeout = 0) {
        this.#form.txtPlayerUsername.value = this.playerUsername;
        this.#form.txtPlayerPassword.value = this.playerPassword;
        super.show(timeout);
        this.#form.txtPlayerUsername.select();
        this.#form.txtPlayerUsername.focus();
    }
}


//-------------------------------------------------------------------------------
//--------------------------------CRegisterModal--------------------------------
//-----------------------------------------------------------------------------

export class CRegisterModal extends CModal {
    playerFirstName = '';
    playerLastName = '';
    playerUsername = '';
    playerPassword = '';
    #form = this.mainPanel.querySelector('form');

    constructor(modalCanvasSelector, outerCanvasClickClosesPopup = false, onHideEventName, startupCallbackFunction = () => { }) {

        super(modalCanvasSelector, outerCanvasClickClosesPopup, onHideEventName);
        this.startupCallbackFunction = startupCallbackFunction;
    }

    get playerDetails() {
        return `${this.playerFirstName} ${this.playerLastName}`;
    }

    // clean certain fields on the form and configure properties
    set startupCallbackFunction(callbackFunction) {
        this.#form.btnConfirm.addEventListener('click', event => {
            //console.log(`event.cancelable: ${event.cancelable}`);
            // event.preventDefault();     // As the button type is submit, this prevents postback to the server // but we want postback now hehehehehehehe
            event.stopPropagation();    // prevent event bubbling up to parent(s)


            this.#form.txtPlayerFirstName.value = this.#form.txtPlayerFirstName.value.trim();
            this.#form.txtPlayerLastName.value = this.#form.txtPlayerLastName.value.trim();
            this.#form.txtPlayerUsername.value = this.#form.txtPlayerUsername.value.trim();
            this.#form.txtPlayerPassword.value = this.#form.txtPlayerLastName.value.trim();


            // if the user has not entered a value, focus the field
            if (!this.#form.txtPlayerFirstName.value) {
                this.#form.txtPlayerFirstName.focus();
                return;
            }
            if (!this.#form.txtPlayerLastName.value) {
                this.#form.txtPlayerLastName.focus();
                return;
            }
            if (!this.#form.txtPlayerUsername.value) {
                this.#form.txtPlayerUsername.focus();
                return;
            }
            if (!this.#form.txtPlayerPassword.value) {
                this.#form.txtPlayerPassword.focus();
                return;
            }

            this.playerFirstName = this.#form.txtPlayerFirstName.value;
            this.playerLastName = this.#form.txtPlayerLastName.value;
            this.playerUsername = this.#form.txtPlayerUsername.value;
            this.playerPassword = this.#form.txtPlayerPassword.value;
            this.hide();

            callbackFunction();     // invoke the callback function
        });
    }

    // make the form display corrected values
    display(timeout = 0) {
        this.#form.txtPlayerFirstName.value = this.playerFirstName;
        this.#form.txtPlayerLastName.value = this.playerLastName;
        this.#form.txtPlayerUsername.value = this.playerUsername;
        this.#form.txtPlayerPassword.value = this.playerPassword;
        super.show(timeout);
        this.#form.txtPlayerUsername.select();
        this.#form.txtPlayerUsername.focus();
    }
}


//-------------------------------------------------------------------------------
//--------------------------------CGameSelectModal-----------------------------------
//-----------------------------------------------------------------------------

export class CGameSelectModal extends CModal {
    selectedGame = '';
    #form = this.mainPanel.querySelector('form');

    constructor(modalCanvasSelector, outerCanvasClickClosesPopup = false, onHideEventName, startupCallbackFunction = () => { }) {

        super(modalCanvasSelector, outerCanvasClickClosesPopup, onHideEventName);
        this.startupCallbackFunction = startupCallbackFunction;
    }

    // clean certain fields on the form and configure properties
    set startupCallbackFunction(callbackFunction) {
        this.#form.btnConfirm.addEventListener('click', event => {
            //console.log(`event.cancelable: ${event.cancelable}`);
            // event.preventDefault();     // As the button type is submit, this prevents postback to the server
            event.stopPropagation();    // prevent event bubbling up to parent(s)

            // this.#form.cboGameSelect.value = this.#form.cboGameSelect.value.trim();


            // if the user has not entered a value, focus the field
            if (!this.#form.cboGameSelect.value) {
                this.#form.cboGameSelect.focus();
                return;
            }

            this.selectedGame = this.#form.cboGameSelect.value;
            // this.hide();

            callbackFunction();     // invoke the callback function
        });
    }

    // make the form display corrected values
    display(timeout = 0) {
        this.#form.cboGameSelect.value = this.selectedGame;
        super.show(timeout);
        this.#form.cboGameSelect.focus();
    }
}


//-------------------------------------------------------------------------------
//--------------------------------CWinnerModal-----------------------------------
//-------------------------------------------------------------------------------
export class CWinnerModal extends CModal {
    display(elapsed, hits, misses, miscMessage = '', timeout = 0) {

        const form = this.mainPanel.querySelector('form');
        form.numElapsedTime.value = elapsed;
        form.numHits.value = hits;
        form.numMisses.value = misses;
        form.txtMiscMessage.value = miscMessage;

        super.show(timeout);
    }
}