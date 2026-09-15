document.addEventListener("DOMContentLoaded", function () {
    initializeQuantity();
    initializeNovaPoshta();
});


function initializeQuantity() {
    const quantityInput =
        document.getElementById("Quantity");

    const totalElement =
        document.getElementById("orderTotal");

    if (!quantityInput || !totalElement) {
        return;
    }

    const unitPrice = Number(
        totalElement.dataset.unitPrice
    );

    function updateTotal() {
        const quantity =
            Number(quantityInput.value);

        const validQuantity =
            Number.isFinite(quantity) && quantity > 0
                ? quantity
                : 1;

        const total =
            unitPrice * validQuantity;

        totalElement.textContent =
            new Intl.NumberFormat("uk-UA", {
                maximumFractionDigits: 0
            }).format(total) + " ₴";
    }

    quantityInput.addEventListener(
        "input",
        updateTotal
    );

    updateTotal();
}


function initializeNovaPoshta() {
    const cityInput =
        document.getElementById("City");

    const cityRefInput =
        document.getElementById("CityRef");

    const citySuggestions =
        document.getElementById("citySuggestions");

    const warehouseInput =
        document.getElementById("DeliveryAddress");

    const warehouseRefInput =
        document.getElementById("WarehouseRef");

    const warehouseSuggestions =
        document.getElementById("warehouseSuggestions");

    if (!cityInput ||
        !cityRefInput ||
        !citySuggestions ||
        !warehouseInput ||
        !warehouseRefInput ||
        !warehouseSuggestions) {
        return;
    }

    let cityTimer;
    let warehouseTimer;


    // =========================
    // ВВОД ГОРОДА
    // =========================

    cityInput.addEventListener("input", function () {
        cityRefInput.value = "";

        warehouseInput.value = "";
        warehouseRefInput.value = "";

        hideSuggestions(warehouseSuggestions);

        clearTimeout(cityTimer);

        const query = cityInput.value.trim();

        if (query.length < 2) {
            hideSuggestions(citySuggestions);
            return;
        }

        cityTimer = setTimeout(
            function () {
                searchCities(query);
            },
            300
        );
    });


    // =========================
    // ВВОД ОТДЕЛЕНИЯ
    // =========================

    warehouseInput.addEventListener("input", function () {
        warehouseRefInput.value = "";

        clearTimeout(warehouseTimer);

        if (!cityRefInput.value) {
            hideSuggestions(warehouseSuggestions);
            return;
        }

        const query =
            warehouseInput.value.trim();

        warehouseTimer = setTimeout(
            function () {
                loadWarehouses(query);
            },
            300
        );
    });


    // Открываем список при нажатии на поле
    warehouseInput.addEventListener("focus", function () {
        if (!cityRefInput.value) {
            return;
        }

        if (!warehouseRefInput.value) {
            loadWarehouses(
                warehouseInput.value.trim()
            );
        }
    });


    // =========================
    // ПОИСК ГОРОДОВ
    // =========================

    async function searchCities(query) {
        try {
            const url =
                "/api/nova-poshta/cities?query=" +
                encodeURIComponent(query);

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(
                    "Не вдалося завантажити міста."
                );
            }

            const cities =
                await response.json();

            showCities(cities);
        }
        catch (error) {
            console.error(error);

            showError(
                citySuggestions,
                "Не вдалося завантажити міста."
            );
        }
    }


    function showCities(cities) {
        citySuggestions.innerHTML = "";

        if (!cities.length) {
            showError(
                citySuggestions,
                "Населений пункт не знайдено."
            );

            return;
        }

        cities.forEach(function (city) {
            const button =
                document.createElement("button");

            button.type = "button";

            button.className =
                "list-group-item " +
                "list-group-item-action";

            const area = city.areaDescription
                ? `, ${city.areaDescription} обл.`
                : "";

            button.textContent =
                `${city.description}${area}`;

            button.addEventListener(
                "click",
                function () {
                    cityInput.value =
                        city.description;

                    cityRefInput.value =
                        city.ref;

                    warehouseInput.value = "";
                    warehouseRefInput.value = "";

                    hideSuggestions(
                        citySuggestions
                    );

                    warehouseInput.focus();
                }
            );

            citySuggestions.appendChild(button);
        });

        showSuggestions(citySuggestions);
    }


    // =========================
    // ЗАГРУЗКА ОТДЕЛЕНИЙ
    // =========================

    async function loadWarehouses(query) {
        if (!cityRefInput.value) {
            return;
        }

        try {
            const parameters =
                new URLSearchParams();

            parameters.set(
                "cityRef",
                cityRefInput.value
            );

            if (query) {
                parameters.set(
                    "query",
                    query
                );
            }

            const response = await fetch(
                "/api/nova-poshta/warehouses?" +
                parameters.toString()
            );

            if (!response.ok) {
                throw new Error(
                    "Не вдалося завантажити відділення."
                );
            }

            const warehouses =
                await response.json();

            showWarehouses(warehouses);
        }
        catch (error) {
            console.error(error);

            showError(
                warehouseSuggestions,
                "Не вдалося завантажити відділення."
            );
        }
    }


    function showWarehouses(warehouses) {
        warehouseSuggestions.innerHTML = "";

        if (!warehouses.length) {
            showError(
                warehouseSuggestions,
                "Відділення не знайдено."
            );

            return;
        }

        warehouses.forEach(function (warehouse) {
            const button =
                document.createElement("button");

            button.type = "button";

            button.className =
                "list-group-item " +
                "list-group-item-action";

            button.textContent =
                warehouse.description;

            button.addEventListener(
                "click",
                function () {
                    warehouseInput.value =
                        warehouse.description;

                    warehouseRefInput.value =
                        warehouse.ref;

                    hideSuggestions(
                        warehouseSuggestions
                    );
                }
            );

            warehouseSuggestions.appendChild(button);
        });

        showSuggestions(
            warehouseSuggestions
        );
    }


    // =========================
    // ЗАКРЫТИЕ СПИСКОВ
    // =========================

    document.addEventListener("click", function (event) {
        const clickedCity =
            event.target === cityInput ||
            citySuggestions.contains(event.target);

        const clickedWarehouse =
            event.target === warehouseInput ||
            warehouseSuggestions.contains(event.target);

        if (!clickedCity) {
            hideSuggestions(citySuggestions);
        }

        if (!clickedWarehouse) {
            hideSuggestions(
                warehouseSuggestions
            );
        }
    });
}


function showSuggestions(element) {
    element.classList.remove("d-none");
}


function hideSuggestions(element) {
    element.classList.add("d-none");
}


function showError(element, message) {
    element.innerHTML = "";

    const messageElement =
        document.createElement("div");

    messageElement.className =
        "list-group-item text-secondary small";

    messageElement.textContent =
        message;

    element.appendChild(messageElement);

    showSuggestions(element);
}