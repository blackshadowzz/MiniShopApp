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
