window.telegramDeviceStorage = {
    setItem: function (key, value) {
        return Telegram.WebApp.deviceStorage.setItem(key, value);
    },
    getItem: function (key) {
        return Telegram.WebApp.deviceStorage.getItem(key);
    },
    removeItem: function (key) {
        return Telegram.WebApp.deviceStorage.removeItem(key);
    },
    clear: function () {
        return Telegram.WebApp.deviceStorage.clear();
    }
};

window.telegramUserInfo = {
    getInitDataUnsafe: function () {
        return Telegram.WebApp.initDataUnsafe;
    }
};

window.getLocation = function () {
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(
            function (position) {
                DotNet.invokeMethodAsync("MiniShopApp", "ReceiveLocation",
                    position.coords.latitude,
                    position.coords.longitude
                );
            },
            function (error) {
                if (error.code === error.PERMISSION_DENIED) {
                    alert("To continue, please enable location on your device.");
                }
            }
        );
    }
};

window.forms = {
    submitForm: function (formId) {
        const form = document.getElementById(formId);
        if (form) {
            form.submit();
        } else {
            console.error("Form not found: " + formId);
        }
    }
};