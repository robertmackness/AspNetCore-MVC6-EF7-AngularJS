// app-trips.js
(function () {

    // Create the module
    angular.module("app-trips", ["simpleControls", "ngRoute"])
    .config(function ($routeProvider) {

        $routeProvider.when("/", {
            controller: "tripsController",
            controllerAs: "viewModel",
            templateUrl: "/views/tripsView.html"
        });
        $routeProvider.when("/editor/:tripName", {
            controller: "tripEditorController",
            controllerAs: "viewModel",
            templateUrl: "/views/tripEditorView.html"
        });
        $routeProvider.otherwise({ redirectTo: "/" });

    });



})();