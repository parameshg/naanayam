app.directive("alertSuccess", function () {
    return {
        template: "<div id='alert-success' class='alert alert-success' role='alert' style='display: none;'><strong>Success!</strong> Operation completed successfully.</div>"
    }
});

app.directive("alertError", function () {
    return {
        template: "<div id='alert-error' class='alert alert-danger' role='alert' style='display: none;'><strong>Error!</strong> Operation cannot be completed.</div>"
    }
});