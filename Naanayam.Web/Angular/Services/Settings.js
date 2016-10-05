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
        $http.get("api/settings/types")
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.getCategories = function (type, successCallback, errorCallback) {
        $http.get("api/settings/types/" + type + "/category")
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.getSubCategories = function (type, category, successCallback, errorCallback) {
        $http.get("api/settings/types/" + type + "/category/" + category)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    };

    this.addCategory = function (type, category, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = { "Key": "category", "Value" : category };
        $http.post("api/settings/types/" + type + "/category", data, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }

    this.removeCategory = function (type, category, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        $http.delete("api/settings/types/" + type + "/category/" + category, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }

    this.addSubCategory = function (type, category, subCategory, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = { "Key": "subCategory", "Value": subCategory };
        $http.post("api/settings/types/" + type + "/category/" + category, data, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }

    this.removeSubCategory = function (type, category, subCategory, successCallback, errorCallback) {
        var config = { headers: { "Content-Type": "application/json" } };
        $http.delete("api/settings/types/" + type + "/category/" + category + "/" + subCategory, config)
        .then(function (response) {
            successCallback(response);
        }, function (response) {
            errorCallback(response);
        });
    }
});