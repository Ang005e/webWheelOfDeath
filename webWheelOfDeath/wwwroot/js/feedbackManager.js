
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

    checkForServerFeedback() {
        const feedbackEl = document.getElementById('server-feedback');
        if (feedbackEl) {
            const type = feedbackEl.dataset.feedbackType;
            const message = feedbackEl.dataset.feedbackMessage;
            const duration = parseInt(feedbackEl.dataset.feedbackDuration);

            this.display(message, type, duration);
            feedbackEl.remove(); // Clean up... >:|
        }
    }

    interceptAjax() {
        // Override jQuery AJAX
        const originalAjax = $.ajax;
        $.ajax = (options) => {
            const originalSuccess = options.success;
            const originalError = options.error;

            options.success = (data, status, xhr) => {
                // Check for feedback in header
                const feedbackHeader = xhr.getResponseHeader('X-Feedback');
                if (feedbackHeader) {
                    const feedback = JSON.parse(feedbackHeader);
                    this.displayFeedback(feedback);
                }

                // Check for feedback in response body
                if (typeof data === 'string') {
                    const $html = $(data);
                    const feedbackEl = $html.find('#server-feedback');
                    if (feedbackEl.length) {
                        this.displayFromElement(feedbackEl[0]);
                    }
                }

                if (originalSuccess) originalSuccess(data, status, xhr);
            };

            return originalAjax.call(this, options);
        };
    }

    displayFeedback(feedback) {
        this.display(feedback.Message, feedback.Type, feedback.Duration);
    }

    display(message, type, duration = 5000) {
        // Map type to display style
        const isError = type === 'Error' || type === 'Warning';

        // ToDo: could enhance CMessageModal to support types
        // using the existing display method for now
        this.modal.display(message, false, duration);

        // Log for debugging
        console.log(`[Feedback ${type}]: ${message}`);
    }

    // Helpers for adding manual feedback
    static success(message, duration) {
        new CFeedbackManager().display(message, 'Success', duration);
    }

    static error(message, duration) {
        new CFeedbackManager().display(message, 'Error', duration);
    }

    static warning(message, duration) {
        new CFeedbackManager().display(message, 'Warning', duration);
    }

    static info(message, duration) {
        new CFeedbackManager().display(message, 'Info', duration);
    }
}

// Initialize on document ready
$(document).ready(() => {
    window.feedbackManager = new CFeedbackManager();
});