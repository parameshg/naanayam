app.controller("report", function ($rootScope, $scope, $timeout, $log, $$report) {

    $scope.month = new Date().getMonth() + 1;
    $scope.year = new Date().getFullYear();
    $scope.series = [];
    $scope.chart = {
        chart: { plotBackgroundColor: null, plotBorderWidth: null, plotShadow: false, type: 'pie' },
        title: { text: '' },
        tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' },
        plotOptions: {
            pie: {
                allowPointSelect: true, cursor: 'pointer',
                dataLabels: { enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: { color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black' } } } },
        series: [{ name: 'Category', colorByPoint: true, data: [] }]
    };

    $timeout(function () {
        $("#id-month").val($scope.month).change();
        $("#id-year").val($scope.year).change();
    }, 10)

    $scope.load = function () {
        $$report.getMonthyExpensesByCategory($rootScope.account.ID, $scope.year, $scope.month, function (response) {
            $scope.chart.series[0].data = [];
            for (var i in response.data) {
                $scope.chart.series[0].data.push({ "name": response.data[i].Category, y: response.data[i].Value });
            }

            $('#chart-container').highcharts($scope.chart);

        }, function (response) {
            Error("Report cannot be loaded");
        })
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