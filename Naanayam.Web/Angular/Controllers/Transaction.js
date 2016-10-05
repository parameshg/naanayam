app.controller("transaction", function ($rootScope, $scope, $timeout, $log, $$transaction) {

    $scope.id = "";
    $scope.date = "";
    $scope.description = "";
    $scope.type = "";
    $scope.category = "";
    $scope.subCategory = "";
    $scope.amount = 0;
    $scope.calendar = false;
    $scope.month = new Date().getMonth() + 1;
    $scope.year = new Date().getFullYear();

    $scope.types = [];
    $scope.categories = [];
    $scope.subCategories = [];
    $scope.transactions = [];

    $timeout(function () {
        $("#id-month").val($scope.month).change();
        $("#id-year").val($scope.year).change();
    }, 10)

    $$transaction.getTypes(function (response) {
        $scope.types = response.data;
    }, function (response) {
        Error("Transaction types cannot be loaded.");
    });

    $scope.loadCategory = function () {
        $$transaction.getCategories($scope.type, function (response) {
            $scope.categories = response.data;
        }, function (response) {
            Error("Transaction categories cannot be loaded.");
        });
    };

    $scope.loadSubCategory = function () {
        $$transaction.getSubCategories($scope.type, $scope.category, function (response) {
            $scope.subCategories = response.data;
        }, function (response) {
            Error("Transaction sub-categories cannot be loaded.");
        });
    };

    $$transaction.get($rootScope.account.ID, $scope.year, $scope.month, function (response) {
        $scope.transactions = response.data;
    }, function (response) {
        Error("Transactions cannot be loaded.");
    });

    $scope.load = function () {
        $$transaction.get($rootScope.account.ID, $scope.year, $scope.month, function (response) {
            $scope.transactions = response.data;
        }, function (response) {
            Error("Transactions cannot be loaded.");
        });
    }

    $scope.save = function () {
        var date = new Date($("#id-date").val()).toLocaleDateString();
        if ($scope.id == "") {
            $$transaction.create($rootScope.account.ID, date, $scope.description, $scope.type, $scope.category + "." + $scope.subCategory, $scope.amount, function (response) {
                $scope.load();
                $scope.clear();
            }, function (response) {
                Error("Transaction cannot be added.");
            });
        } else {
            $$transaction.update($scope.id, $rootScope.account.ID, $scope.date, $scope.description, $scope.type, $scope.category + "." + $scope.subCategory, $scope.amount, function (response) {
                $scope.load();
                $scope.clear();
            }, function (response) {
                Error("Transaction cannot be updated.");
            });
        }
    };

    $scope.edit = function (id) {
        $scope.id = id;
        $scope.date = $("#date-" + id).val();
        $scope.description = $("#description-" + id).text();
        $scope.type = $("#type-" + id).text();
        $scope.category = $("#category-" + id).text();
        $scope.subCategory = $("#subCategory-" + id).text();
    };

    $scope.delete = function (id) {
        $$transaction.delete($rootScope.account.ID, id, function (response) {
            $scope.load();
            $scope.clear();
        }, function (response) {
            Error("Transactions cannot be deleted.");
        });
    };

    $scope.clear = function () {
        $scope.id = "";
        $scope.date = "";
        $scope.description = "";
        $scope.type = "";
        $scope.category = "";
        $scope.subCategory = "";
        $scope.amount = "";
    };

    $scope.label = function (type) {
        if (type === 0)
            return "label label-success";

        if (type === 1)
            return "label label-danger";

        if (type === 2)
            return "label label-primary";
    }

    $scope.openCalendar = function () {
        $scope.calendar = true;
    }
    
    $scope.calendarOptions = {
        maxDate: new Date(2020, 12, 31),
        minDate: new Date(1980, 1, 1),
        startingDay: 1
    };

    $rootScope.load = function () {
        $scope.load();
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