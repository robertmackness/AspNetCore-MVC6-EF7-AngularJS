// tripsController.js
(function () {
    "use strict";
    // Get the module
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {
        var viewModel = this;

        viewModel.trips = [];
        viewModel.newTrip = {};
        viewModel.errorMessage = "";
        viewModel.isBusy = true;
        $http.get("/api/trips")
            .then(function (response) {
                // Success
                angular.copy(response.data, viewModel.trips);
            }, function (error) {
                // Failure
                viewModel.errorMessage = "Failed to load data: " + error;
            })
        .finally(function () {
            viewModel.isBusy = false;
        });
        viewModel.addTrip = function (){
            viewModel.isBusy = true;
            viewModel.errorMessage = "";
            viewModel.newTrip.created = new Date();
            $http.post("/api/trips", viewModel.newTrip)
            .then(function (response) {
                // success
                viewModel.trips.push(response.data)
                viewModel.newTrip = "";
            }, function (error) {
                // failure
                viewModel.errorMessage = "Failed to save new trip";
            })
            .finally(function () {

            });
        }
    }
})();