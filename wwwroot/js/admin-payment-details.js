document.addEventListener("DOMContentLoaded", function () {
    const typeSelect =
        document.getElementById("Type");

    const cardNumberSection =
        document.getElementById("cardNumberSection");

    const ibanSection =
        document.getElementById("ibanSection");

    const recipientSection =
        document.getElementById("recipientSection");

    if (!typeSelect ||
        !cardNumberSection ||
        !ibanSection ||
        !recipientSection) {
        return;
    }

    function updateVisibleFields() {
        const selectedType =
            Number(typeSelect.value);

        const isCard = selectedType === 1;

        const isBankAccount =
            selectedType === 2 ||
            selectedType === 3;

        const isOther =
            selectedType === 4;

        cardNumberSection.classList.toggle(
            "d-none",
            !isCard
        );

        ibanSection.classList.toggle(
            "d-none",
            !isBankAccount
        );

        recipientSection.classList.toggle(
            "d-none",
            isOther
        );
    }

    typeSelect.addEventListener(
        "change",
        updateVisibleFields
    );

    updateVisibleFields();
});