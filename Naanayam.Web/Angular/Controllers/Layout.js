app.controller("layout", function ($rootScope, $scope, $timeout, $log, $$account) {

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