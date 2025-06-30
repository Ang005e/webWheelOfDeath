
$(document).ready(() => {
    $(document).on('click', '[data-nav]', async function (e) {
        e.preventDefault();
        const target = $(this).data('nav');
        const data = $(this).closest('form').serialize() || null;
        await navManager.navigate(target, data);
    });
});