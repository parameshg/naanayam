app.filter("filterByTransactionType", function () {
    return function (transactions, type) {
        var result = [];
        for (i in transactions) {
            if (transactions[i].Type === type) {
                result.push(transactions[i]);
            }
        }
        return result;
    }
});