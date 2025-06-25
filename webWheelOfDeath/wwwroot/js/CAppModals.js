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
        this.#form.btnLogin.addEventListener('click', event=> {
            //console.log(`event.cancelable: ${event.cancelable}`);
            // event.preventDefault();     // As the button type is submit, this prevents postback to the server
            event.stopPropagation();    // prevent event bubbling up to parent(s)

            //this.#form.Username.value = this.#form.Username.value.trim();
            //this.#form.Password.value = this.#form.Password.value.trim();

            if (!this.#form.Username.value) {
                this.#form.Username.focus();
                return;
            }

            if (!this.#form.Password.value) {
                this.#form.Password.focus();
                return;
            }

            this.playerUsername = this.#form.Username.value;
            this.playerPassword = this.#form.Password.value;
            // this.hide();

            //document.dispatchEvent(new CustomEvent("login-complete", {
            //    bubbles: true
            //}))

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
        this.#form.Username.value = this.playerUsername;
        this.#form.Password.value = this.playerPassword;
        super.show(timeout);
        this.#form.Username.select();
        this.#form.Username.focus();
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


            this.#form.FirstName.value = this.#form.FirstName.value.trim();
            this.#form.LastName.value = this.#form.LastName.value.trim();
            this.#form.Username.value = this.#form.Username.value.trim();
            this.#form.Password.value = this.#form.Password.value.trim();


            // if the user has not entered a value, focus the field
            if (!this.#form.FirstName.value) {
                this.#form.FirstName.focus();
                return;
            }
            if (!this.#form.LastName.value) {
                this.#form.LastName.focus();
                return;
            }
            if (!this.#form.Username.value) {
                this.#form.Username.focus();
                return;
            }
            if (!this.#form.Password.value) {
                this.#form.Password.focus();
                return;
            }

            this.playerFirstName = this.#form.FirstName.value;
            this.playerLastName = this.#form.LastName.value;
            this.playerUsername = this.#form.Username.value;
            this.playerPassword = this.#form.Password.value;
            this.hide();

            callbackFunction();     // invoke the callback function
        });
    }

    // make the form display corrected values
    display(timeout = 0) {
        this.#form.FirstName.value = this.playerFirstName;
        this.#form.LastName.value = this.playerLastName;
        this.#form.Username.value = this.playerUsername;
        this.#form.Password.value = this.playerPassword;
        super.show(timeout);
        this.#form.Username.select();
        this.#form.Username.focus();
    }
}


//-------------------------------------------------------------------------------
//--------------------------------CGameSelectModal-----------------------------------
//-----------------------------------------------------------------------------

export class CGameSelectModal extends CModal {
    selectedGame = '';
    #form = this.mainPanel.querySelector('form');
    #callbackFunction = () => { };

    constructor(modalCanvasSelector, outerCanvasClickClosesPopup = false, onHideEventName) {

        super(modalCanvasSelector, outerCanvasClickClosesPopup, onHideEventName);
        this.startupCallbackFunction(); 
    }

    get callbackFunction() {
        return this.#callbackFunction;
    }
    set callbackFunction(value) {
        this.#callbackFunction = value;
    }
    startupCallbackFunction() {
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
            this.hide();

            this.#callbackFunction();     // invoke the callback function
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