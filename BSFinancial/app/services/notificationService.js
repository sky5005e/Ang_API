app.factory('notificationService', function (toastr) {
    toastr.options = {
        "showDuration": "100",
        "hideDuration": "100",
        "timeOut": "1000",
        "extendedTimeOut": "1000",
    }

    return {
        success: function (text) {
            if (text === undefined) {
                text = '';
            }
            toastr.success("Success. " + text);
        },
        error: function (text) {
            if (text === undefined) {
                text = '';
            }
            toastr.error("Error. " + text);
        },
    };
});