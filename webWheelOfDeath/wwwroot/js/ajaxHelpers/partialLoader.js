
import { CMessageModal } from '/js/CAppModals.js';

/**
 * Send form data via AJAX, update a target element, and dispatch a refresh event.
 *
 * @param {string} actionUrl         – URL to POST the form to
 * @param {string} refreshElementId  – jQuery selector for the element to replace
 * @param {string} formName          – identifier passed in the CustomEvent detail
 */
export function partialLoader(formData, actionUrl, refreshElementId, formName, isPost = false) {
    // ToDo: replace with Javascript fetch
    $.ajax({
        url: actionUrl,  // URL to the Login action
        type: isPost ? 'POST' : 'GET',
        data: formData,
        success: function (partial) {
            // Update the modal content with the returned partial view HTML
            $(refreshElementId).html(partial);

            document.dispatchEvent(new CustomEvent("partial-refresh", {
                bubbles: true,
                detail:
                {
                    form: formName
                }
            }));
        },
        error: function () {
            new CMessageModal('#modal-message-id').display("There was an error processing your request.", false, 5000);
        }
    });
}
