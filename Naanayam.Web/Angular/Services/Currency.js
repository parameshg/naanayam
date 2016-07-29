app.service("$$currency", function ($http) {

    this.get = function (successCallback, errorCallback) {
        $http.get("api/settings/currency")
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };
});