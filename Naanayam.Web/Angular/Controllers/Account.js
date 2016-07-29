app.controller("account", function ($rootScope, $scope, $timeout, $log, $$settings, $$account) {

    $scope.id = "";
    $scope.name = "";
    $scope.description = "";
    $scope.currency = "";
    $scope.currencies = [];

    $$settings.getCurrencies(function (response) {
        $scope.currencies = response.data;
    }, function (response) {
        Error("Currency cannot be loaded.");
    });

    $$account.get(function (response) {
        $rootScope.accounts = response.data;
    }, function (response) {
        Error("Accounts cannot be loaded.");
    });

    $scope.select = function (account) {
        for (var i in $rootScope.accounts[0]) {
            if (i.ID === account) {
                $rootScope.account = i;
                break;
            }
        }
    }

    $scope.display = function (account) {
        if ($rootScope.account.ID != account)
            return "";
        else
            return "success";
    }

    $scope.save = function () {
        if ($scope.id == "") {
            $$account.create($scope.name, $scope.description, $scope.currency, function (response) {
                $rootScope.accounts = response.data;
                $scope.clear();
                Alert("Accounts added successfully.");
            }, function (response) {
                Error("Accounts cannot be added.");
            });
        } else {
            $$account.update($scope.id, $scope.name, $scope.description, $scope.currency, function (response) {
                $rootScope.accounts = response.data;
                $scope.clear();
                Alert("Accounts updated successfully.");
            }, function (response) {
                Error("Accounts cannot be updated.");
            });
        }
    };

    $scope.edit = function (id) {
        $scope.id = id;
        $scope.name = $("#name-" + id).text();
        $scope.description = $("#description-" + id).text();
        $scope.currency = $("#currency-" + id).text();
    };

    $scope.delete = function (id) {
        $$account.delete(id, function (response) {
            $rootScope.accounts = response.data;
            $scope.clear();
            Alert("Accounts deleted successfully.");
        }, function (response) {
            Error("Accounts cannot be deleted.");
        });
    };

    $scope.clear = function () {
        $scope.id = "";
        $scope.name = "";
        $scope.description = "";
        $scope.currency = "";
    };

    $rootScope.load = function () {
    }

    function Alert(message) {
        $("#alert-success").html("<strong>Success!</strong> " + message);
        $("#alert-success").slideDown();
        $timeout(function () { $("#alert-success").slideUp(); }, ALERT_DELAY);
    }

    function Error(message) {
        $("#alert-error").html("<strong>Error!</strong> " + message);
        $("#alert-error").slideDown();
        $timeout(function () { $("#alert-error").slideUp(); }, ALERT_DELAY);
    }
});