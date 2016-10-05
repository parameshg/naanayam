app.controller("settings", function ($rootScope, $scope, $timeout, $log, $$settings) {

    $scope.currencies = [];
    $scope.types = [];
    $scope.categories = [];
    $scope.subCategories = [];

    $scope.selectedType = "";
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

    $scope.getCategories = function (type) {
        $scope.selectedType = type;
        $$settings.getCategories(type, function (response) {
            $scope.categories = response.data;
        }, function (response) {
            Error("Categories cannot be loaded.");
        });
    }

    $scope.getSubCategories = function (category) {
        $scope.selectedCategory = category;
        $$settings.getSubCategories($scope.selectedType, category, function (response) {
            $scope.subCategories = response.data;
        }, function (response) {
            Error("Categories cannot be loaded.");
        });
    }

    $scope.addCategory = function (category) {
        $$settings.addCategory($scope.selectedType, category, function (response) {
            $scope.categories = response.data;
        }, function (response) {
            Error("Categories cannot be added.");
        });
    }

    $scope.removeCategory = function (category) {
        $$settings.removeCategory($scope.selectedType, category, function (response) {
            $scope.categories = response.data;
        }, function (response) {
            Error("Categories cannot be removed.");
        });
    }

    $scope.addSubCategory = function (subCategory) {
        $$settings.addSubCategory($scope.selectedType, $scope.selectedCategory, subCategory, function (response) {
            $scope.subCategories = response.data;
        }, function (response) {
            Error("Categories cannot be added.");
        });
    }

    $scope.removeSubCategory = function (subCategory) {
        $$settings.removeSubCategory($scope.selectedType, $scope.selectedCategory, subCategory, function (response) {
            $scope.subCategories = response.data;
        }, function (response) {
            Error("Categories cannot be removed.");
        });
    }
});