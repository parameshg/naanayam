app.service("$$settings", function ($http) {

    this.getCurrencies = function (successCallback, errorCallback) {
        $http.get("api/settings/currency")
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.getTypes = function (successCallback, errorCallback) {
        $http.get("api/settings/type")
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.getCategories = function (successCallback, errorCallback) {
        $http.get("api/settings/category")
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.addCategory = function (category, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = category;
        $http.post("api/settings/category", data, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }

    this.removeCategory = function (category, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        $http.delete("api/settings/category/" + category, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }

    this.addSubCategory = function (category, subCategory, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = subCategory;
        $http.post("api/settings/sub-category/" + category, data, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }

    this.removeSubCategory = function (category, subCategory, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        $http.delete("api/settings/sub-category/" + category + "/" + subCategory, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }
});