
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