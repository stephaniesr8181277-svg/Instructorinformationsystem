// Ask for confirmation before submitting a form marked with data-confirm.
document.addEventListener("submit", function (event) {
    const form = event.target;
    if (!(form instanceof HTMLFormElement)) return;

    const confirmationMessage = form.dataset.confirm;
    if (confirmationMessage && !window.confirm(confirmationMessage)) {
        event.preventDefault();
    }
});
