'use strict';
import { CLoginModal, CRegisterModal } from "./CAppModals.js";



// This class is responsible for managing the login popups in the application.
export class CAccountPopups {

    #loginModal;
    #registerModal;

    constructor() {

        /// Configure the login modal popup
        this.#loginModal = new CLoginModal('#modal-login-id', true, 'login-modal-hide');
        // this.#loginModal.callbackFunction = () => {this.start()};

        // Configure the register modal popup
        this.#registerModal = new CRegisterModal('#modal-register-id', true, 'register-modal-hide');

        // To prevent the whole page from reloading when data is transferred from the server...
        // 1. Intercept the form submission
        // 2. Perform fetch
        // 3. Update ONLY the targeted element with the response

        // Whenever any links are clicked inside this section, request that content via fetch
        // to prevent the need for a full re-render

        // UPDATE THE LOGIN FORM WITH NEWLY CREATED ACCOUNT'S DETAILS

        //fetch('/Home/Login', { method: 'POST', body: formData })
        //    .then(r => r.text())
        //    .then(html => {
        //        document.querySelector('#modal-login-id').innerHTML = html;
        //        //();   // call a function that hooks listeners again
        //    });



        //// ########### FETCH CALL/CONTENT REFRESH LISTENERS ############
        // Call the login startup function to reapply its javascript/event listeners.
        document.addEventListener("partial-refresh", (event) => {
            if (event.detail.form === "login-form") {
                // Use after the login HTML is replaced by an AJAX call to the server.
                this.#loginModal.startupCallbackFunction();
                // this.#registerModal.startupCallbackFunction();
            }
        })

        // ########### NAVIGATION LISTENERS ############

        // Listen for a register request (links to the "create account" button on CLoginModal)
        document.addEventListener("create-account", event => {
            // event.preventDefault();     // As the button type is submit, this prevents postback to the server
            event.stopPropagation();    // prevent event bubbling up to parent(s)
            this.#loginModal.hide();
            this.#registerModal.display();
        })

        // Ensure that the when the register modal is closed, the login modal re-opens
        document.addEventListener('register-modal-hide', event => {
            event.stopPropagation();
            this.#loginModal.display();
        })
    }
}


//this.#registerModal.hide();
//this.#loginModal.display();