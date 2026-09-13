document.addEventListener("DOMContentLoaded", function () {
    // =========================
    // АВТООБНОВЛЕНИЕ ЦЕНЫ
    // =========================

    const autoUpdateCheckbox =
        document.getElementById("AutoUpdatePrice");

    const priceInput =
        document.getElementById("Price");

    if (autoUpdateCheckbox && priceInput) {
        function updatePriceState() {
            priceInput.readOnly =
                autoUpdateCheckbox.checked;

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
    }

    // =========================
    // ПОДТВЕРЖДЕНИЕ УДАЛЕНИЯ
    // =========================

    const deleteForm =
        document.querySelector(".delete-product-form");

    if (deleteForm) {
        deleteForm.addEventListener(
            "submit",
            function (event) {
                const confirmed = confirm(
                    "Удалить товар из базы? Это действие нельзя отменить."
                );

                if (!confirmed) {
                    event.preventDefault();
                }
            }
        );
    }
});