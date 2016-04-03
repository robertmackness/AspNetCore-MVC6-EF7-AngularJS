(function () {

    "use strict";
    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    function tripEditorController($routeParams, $http) {
        var viewModel = this;    
        viewModel.tripName = $routeParams.tripName;
        viewModel.stops = [];
        viewModel.errorMessage = "";
        viewModel.isBusy = true;
        viewModel.newStop = {};
        var url = "/api/trips/" + viewModel.tripName + "/stops";

        $http.get(url)
        .then(function (response) {
            //success
            angular.copy(response.data, viewModel.stops);
            _showMap(viewModel.stops);
        }, function (error) {
            //failure
            viewModel.errorMessage = "Failed to load stops."
        })
        .finally(function () {
            viewModel.isBusy = false;
        });

        viewModel.addStop = function () {
            viewModel.isBusy = true;
            $http.post(url, viewModel.newStop)
            .then(function (response) {
                // success
                viewModel.stops.push(response.data);
                _showMap(viewModel.stops);
                viewModel.newStop = {};
            }, function (failure) {
                // error
                viewModel.errorMessage = "Failed to add new stop";
            })
            .finally(function () {
                viewModel.isBusy = false;
            });

        }
    }

    function _showMap(stops) {
        if (stops && stops.length > 0) {
            
            var mapStops = _.map(stops, function (item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                };
            });

            travelMap.createMap({
                stops: mapStops,
                selector: "#map",
                currentStop: 1,
                intialZoom:3
            })
        }
    }

})();