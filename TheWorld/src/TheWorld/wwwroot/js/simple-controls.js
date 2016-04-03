// simpleControls.js
(function () {
    "use strict";

    angular.module("simpleControls", [])
    .directive("waitCursor", waitCursor);

    function waitCursor() {
        return {
            restrict: "E",
            templateUrl: "/views/waitCursor.html",
            scope: {
                displayWhen: "="
            }
        }
    }

})();