app.service("$$account", function ($http) {

    this.get = function (successCallback, errorCallback) {
        $http.get("api/accounts").then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.create = function (name, description, currency, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = { ID: "", Name: name, Description: description, Currency: currency };
        $http.post("api/accounts", data, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.update = function (id, name, description, currency, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = { ID: id, Name: name, Description: description, Currency: currency };
        $http.put("api/accounts", data, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.delete = function (id, successCallback, errorCallback) {
        $http.delete("api/accounts/" + id)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };
});