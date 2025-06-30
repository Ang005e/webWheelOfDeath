
import { CMessageModal } from '/js/CAppModals.js';
/**
 * Handles saving form data via AJAX and displays a message modal upon success/fail.
 *
 * @param {string} saveButtonName - ID of the save button
 * @param {string} controller - Controller name for the AJAX request
 * @param {string} saveAction - Action name for the AJAX request
 */
export function saveHandler(saveButtonName, controller, saveAction) {

    // add a new message model for user info
    const modal = new CMessageModal('#modal-message-id')

    document.getElementById(saveButtonName).addEventListener('click', () => {
        const formData = $(this).closest('form').serialize();

        modal.display("Saving...", false);

        $.ajax({
            url: `/${controller}/${saveAction}`,
            type: 'POST',
            data: formData,
            success: function (response) {
                // disable the save button to prevent duplicates
                document.getElementById(saveButtonName).disabled = true;

                // clear the "saving" message modal
                modal.display("Saved!", false, 2000);
            },
            error: function () {
                modal.display("Save failed. Please try again.", false, 5000);
            }
        });
    });
}