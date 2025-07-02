
// Monitors for, intercepts, and processes feedback (informational, warning, success, or error) messages,
// sent from the server, that are intended to be displayed to the user.
// Please note this class was written with assistance from the Claude.ai tool. I did not wirte this code from scratch.
// However, I sat very patiently and drunk some tea while I waited for it to generate. It was quite taxing.
export class CFeedbackManager {
    static instance = null;

    constructor() {
        if (CFeedbackManager.instance) {
            return CFeedbackManager.instance;
        }

        this.modal = new window.CMessageModal('#modal-message-id');
        CFeedbackManager.instance = this;

        // Auto-display on page load
        this.checkForServerFeedback();

        // Intercept AJAX responses
        this.interceptAjax();
    }

    display(message, type, duration = 5000) {
        // CRITICAL: Clear any existing timeout first
        if (this.modal) {
            this.modal.cancel(); // Cancel any running timer
        }

        // Convert enum to string if it's a number
        const typeString = this.getTypeString(type);

        // Get modal elements
        const modalPanel = document.querySelector('#modal-message-id .main-panel');
        const messageDisplay = document.querySelector('#modal-message-id .message-display');

        if (modalPanel && messageDisplay) {
            // CRITICAL: Clear old message first
            messageDisplay.textContent = '';

            // Remove ALL existing type classes
            modalPanel.classList.remove('modal-success', 'modal-error', 'modal-warning', 'modal-info', 'modal-player', 'modal-winner');

            // Add new type class
            const cssClass = this.getFeedbackClass(typeString);
            modalPanel.classList.add(cssClass);

            // Set new message
            messageDisplay.textContent = message;
        }

        // Display with proper duration
        this.modal.show(duration); // Use show() instead of display() to ensure timer works

        // Log for debugging
        console.log(`[Feedback ${typeString}]: ${message} (duration: ${duration}ms)`);
    }

    checkForServerFeedback() {
        const feedbackEl = document.getElementById('server-feedback');
        if (feedbackEl) {
            const type = feedbackEl.dataset.feedbackTypeName || feedbackEl.dataset.feedbackType;
            const message = feedbackEl.dataset.feedbackMessage;
            const duration = parseInt(feedbackEl.dataset.feedbackDuration) || 5000; // Default fallback

            this.display(message, type, duration);
            feedbackEl.remove();
        }
    }

    interceptAjax() {
        const originalAjax = $.ajax;
        $.ajax = (options) => {
            const originalSuccess = options.success;

            options.success = (data, status, xhr) => {
                const feedbackHeader = xhr.getResponseHeader('X-Feedback');
                if (feedbackHeader) {
                    try {
                        const feedback = JSON.parse(feedbackHeader);
                        this.displayFeedback(feedback);
                    } catch (e) {
                        console.error('Failed to parse feedback header:', e);
                    }
                }

                if (typeof data === 'string') {
                    const $html = $('<div>').html(data);
                    const feedbackEl = $html.find('#server-feedback')[0];
                    if (feedbackEl) {
                        const type = feedbackEl.dataset.feedbackTypeName || feedbackEl.dataset.feedbackType;
                        const message = feedbackEl.dataset.feedbackMessage;
                        const duration = parseInt(feedbackEl.dataset.feedbackDuration) || 5000;
                        this.display(message, type, duration);
                    }
                }

                if (originalSuccess) originalSuccess(data, status, xhr);
            };

            return originalAjax.call(this, options);
        };
    }

    displayFeedback(feedback) {
        this.display(feedback.Message, feedback.Type, feedback.Duration || 5000);
    }

    getTypeString(type) {
        if (typeof type === 'string') return type;

        const typeMap = {
            0: 'None',
            1: 'Success',
            2: 'Warning',
            3: 'Error',
            4: 'Info'
        };
        return typeMap[type] || 'Info';
    }

    getFeedbackClass(type) {
        const classMap = {
            'Success': 'modal-success',
            'Error': 'modal-error',
            'Warning': 'modal-warning',
            'Info': 'modal-info',
            'None': 'modal-info'
        };
        return classMap[type] || 'modal-info';
    }

    static success(message, duration = 5000) {
        new CFeedbackManager().display(message, 'Success', duration);
    }

    static error(message, duration = 5000) {
        new CFeedbackManager().display(message, 'Error', duration);
    }

    static warning(message, duration = 5000) {
        new CFeedbackManager().display(message, 'Warning', duration);
    }

    static info(message, duration = 5000) {
        new CFeedbackManager().display(message, 'Info', duration);
    }
}

$(document).ready(() => {
    window.feedbackManager = new CFeedbackManager();
});