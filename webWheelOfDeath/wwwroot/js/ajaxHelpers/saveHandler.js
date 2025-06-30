
import $ from 'jquery';

export function addSaveHandler(saveButtonName, controller, saveAction) {
    document.getElementById(saveButtonName).addEventListener('click', () => {
        const formData = $(this).closest('form').serialize();

        $.ajax({
            url: `/${controller}/${saveAction}`,
            type: 'POST',
            data: formData,
            success: function (response) {
                new CMessageModal('#modal-message-id').display("Saved!", false, 3000);
            },
            error: function () {
                new CMessageModal('#modal-message-id').display("Save failed. Please try again.", false, 5000);
            }
        });
    });
}