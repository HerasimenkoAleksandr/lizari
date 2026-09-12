document.addEventListener("DOMContentLoaded", function () {

    const description =
        document.getElementById("productDescription");

    const button =
        document.getElementById("descriptionToggle");

    if (!description || !button) {
        return;
    }

    button.addEventListener("click", function () {

        description.classList.toggle("expanded");

        if (description.classList.contains("expanded")) {
            button.textContent = "Згорнути";
        }
        else {
            button.textContent = "Читати далі";
        }

    });

});