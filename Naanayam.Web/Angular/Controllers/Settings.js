app.controller("settings", function ($rootScope, $scope, $timeout, $log, $$settings) {

    $scope.currencies = [];
    $scope.types = [];
    $scope.categories = [];
    $scope.category = "";
    $scope.subCategory = "";
    $scope.selectedCategory = "";

    $$settings.getCurrencies(function (response) {
        $scope.currencies = response.data;
    }, function (response) {
        Error("Currencies cannot be loaded.");
    });

    $$settings.getTypes(function (response) {
        $scope.types = response.data;
    }, function (response) {
        Error("Types cannot be loaded.");
    });

    $$settings.getCategories(function (response) {
        $scope.categories = response.data;
        if ($scope.categories.length > 0) {
            $scope.category = $scope.categories[0];
        }
    }, function (response) {
        Error("Categories cannot be loaded.");
    });

    $scope.getCategories = function (category) {
        $scope.selectedCategory = category;
    }

    $scope.addCategory = function (category) {
        $$settings.addCategory(category, function (response) {
            $scope.categories = response.data;
            $scope.category = ""
        }, function (response) {
            Error("Categories cannot be added.");
        });
    }

    $scope.removeCategory = function (category) {
        $$settings.removeCategory(category, function (response) {
            $scope.categories = response.data;
            $scope.category = ""
        }, function (response) {
            Error("Categories cannot be removed.");
        });
    }

    $scope.addSubCategory = function (category, subCategory) {
        $$settings.addSubCategory(category, subCategory, function (response) {
            $scope.categories = response.data;
            $scope.subCategory = ""
        }, function (response) {
            Error("Categories cannot be added.");
        });
    }

    $scope.removeSubCategory = function (category, subCategory) {
        $$settings.removeSubCategory(category, subCategory, function (response) {
            $scope.categories = response.data;
            $scope.subCategory = ""
        }, function (response) {
            Error("Categories cannot be removed.");
        });
    }
});