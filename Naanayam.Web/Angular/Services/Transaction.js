app.service("$$transaction", function ($http) {

    this.getTypes = function (success, error) {
        $http.get("api/settings/type")
        .then(function (response) {
            success(response);
        }, function (response) {
            error(response);
        });
    };

    this.getCategories = function (success, error) {
        $http.get("api/settings/category")
        .then(function (response) {
            success(response);
        }, function (response) {
            error(response);
        });
    };

    this.get = function (account, year, month, success, error) {
        $http.get("api/account/" + account + "/transaction/" + year + "/" + month)
        .then(function (response) {
            success(response);
        }, function (response) {
            error(response);
        });
    };

    this.create = function (account, date, description, type, category, amount, success, error) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = { ID: "", Account: account, Timestamp: date, Description: description, Type: type, Category: category, Amount: amount };
        $http.post("api/transaction", data, config)
        .then(function (response) {
            success(response);
        }, function (response) {
            error(response);
        });
    };

    this.update = function (id, account, date, description, type, category, amount, success, error) {
        var config = { headers: { "Content-Type": "application/json" } };
        var data = { ID: id, Account: account, Timestamp: date, Description: description, Type: type, Category: category, Amount: amount };
        $http.put("api/transaction", data, config)
        .then(function (response) {
            success(response);
        }, function (response) {
            error(response);
        });
    };

    this.delete = function (account, id, success, error) {
        $http.delete("api/account/" + account + "/transaction/" + id)
        .then(function (response) {
            success(response);
        }, function (response) {
            error(response);
        });
    };
});