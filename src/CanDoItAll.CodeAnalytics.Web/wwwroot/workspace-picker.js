(function () {
    function setStatus(statusElement, message, isError) {
        if (!statusElement) {
            return;
        }

        statusElement.textContent = message || "";
        statusElement.dataset.state = isError ? "error" : "info";
    }

    async function handlePickerClick(button) {
        const targetId = button.dataset.targetId;
        const statusId = button.dataset.statusId;
        const input = document.getElementById(targetId);
        const status = document.getElementById(statusId);
        if (!input) {
            return;
        }

        button.disabled = true;
        setStatus(status, "Opening local workspace picker...", false);

        try {
            const response = await fetch("/api/workspace-picker", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    currentPath: input.value
                })
            });
            const result = await response.json();
            if (result.isSuccess && result.workspacePath) {
                input.value = result.workspacePath;
                setStatus(status, result.workspacePath, false);
                return;
            }

            if (result.isCanceled) {
                setStatus(status, "Selection canceled.", false);
                return;
            }

            setStatus(status, result.errorMessage || "The picker could not open.", true);
        }
        catch (error) {
            setStatus(status, error && error.message ? error.message : "The picker request failed.", true);
        }
        finally {
            button.disabled = false;
        }
    }

    function bindPickers() {
        const buttons = document.querySelectorAll("[data-workspace-picker]");
        for (const button of buttons) {
            if (button.dataset.bound === "true") {
                continue;
            }

            button.dataset.bound = "true";
            button.addEventListener("click", function () {
                handlePickerClick(button);
            });
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", bindPickers);
    }
    else {
        bindPickers();
    }
})();
