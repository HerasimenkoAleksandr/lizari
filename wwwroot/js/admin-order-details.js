document.addEventListener("DOMContentLoaded", function () {
    const copyButtons =
        document.querySelectorAll(".copy-message-button");

    copyButtons.forEach(function (button) {
        button.addEventListener("click", async function () {
            const targetId =
                button.dataset.copyTarget;

            const messageField =
                document.getElementById(targetId);

            if (!messageField ||
                button.disabled) {
                return;
            }

            try {
                await navigator.clipboard.writeText(
                    messageField.value
                );
            }
            catch {
                messageField.focus();
                messageField.select();

                document.execCommand("copy");
            }

            showCopiedState(button);
        });
    });

    function showCopiedState(button) {
        const originalHtml =
            button.innerHTML;

        button.innerHTML =
            '<i class="bi bi-check-lg"></i> Скопійовано';

        button.classList.remove(
            "btn-outline-success"
        );

        button.classList.add(
            "btn-success"
        );

        window.setTimeout(function () {
            button.innerHTML =
                originalHtml;

            button.classList.remove(
                "btn-success"
            );

            button.classList.add(
                "btn-outline-success"
            );
        }, 2500);
    }
});