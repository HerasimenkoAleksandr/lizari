document.addEventListener("DOMContentLoaded", function () {
    const autoUpdateCheckbox =
        document.getElementById("AutoUpdatePrice");

    const priceInput =
        document.getElementById("Price");

    if (!autoUpdateCheckbox || !priceInput) {
        return;
    }

    function updatePriceState() {
        priceInput.readOnly = autoUpdateCheckbox.checked;
        priceInput.classList.toggle(
            "bg-light",
            autoUpdateCheckbox.checked
        );
    }

    autoUpdateCheckbox.addEventListener(
        "change",
        updatePriceState
    );

    updatePriceState();
});