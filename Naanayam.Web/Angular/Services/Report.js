app.service("$$report", function ($http) {

    this.getMonthyExpensesByCategory = function (account, year, month, success, error) {
        $http.get("api/account/" + account + "/report/monthly-expenses-by-category/" + year + "/" + month)
        .then(function (response) {
            success(response);
        }, function (response) {
            error(response);
        });
    };
});