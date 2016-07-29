var app = angular.module("app", ["ngRoute", "ngMessages", "blockUI", "ui.bootstrap"]);

var ALERT_DELAY = 2000;

app.config(function ($routeProvider) {
    $routeProvider
    .when("/", {
        templateUrl: 'Angular/Views/Home.html',
        controller: 'home'
    })
    .when("/accounts", {
        templateUrl: "Angular/Views/Accounts.html",
        controller: "account"
    })
    .when("/transactions", {
        templateUrl: "Angular/Views/Transactions.html",
        controller: "transaction"
    })
    .when("/settings", {
        templateUrl: 'Angular/Views/Settings.html',
        controller: 'settings'
    })
    .when("/reports/monthly-expenses-category-pie", {
        templateUrl: "Angular/Views/Reports/Monthly-Expenses-Category-Pie.html",
        controller: "report"
    })
    .otherwise({ redirectTo: "/" });
});

app.config(function (blockUIConfig) {
    blockUIConfig.message = "Please wait. Loading...";
    blockUIConfig.delay = 100;
});