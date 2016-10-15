app.controller("menu", function ($rootScope, $scope, $timeout, $cookies, $log, $$account) {
    $scope.menuAccounts = "";
    $scope.menuTransactions = "";

    $scope.highlight = function (menu) {
        if (menu === "accounts") {
            $scope.menuAccounts = "active";
            $scope.menuTransactions = "";
        }
        if (menu === "transactions") {
            $scope.menuAccounts = "";
            $scope.menuTransactions = "active";
        }
    }

    $scope.select = function (account) {
        $cookies.put("account", account);
        for (var i in $rootScope.accounts) {
            if ($rootScope.accounts[i].ID === account) {
                $rootScope.account = $rootScope.accounts[i];
                $rootScope.load();
                break;
            }
        }
    }

    $scope.display = function (account) {
        if ($rootScope.account.ID != account)
            return "";
        else
            return "glyphicon glyphicon-hand-left";
    }

    $$account.get(function (response) {
        $rootScope.accounts = response.data;
        if ($rootScope.accounts.length > 0)
            $rootScope.account = $rootScope.accounts[0];
    }, function (response) {
        Error("Accounts cannot be loaded.");
    });

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