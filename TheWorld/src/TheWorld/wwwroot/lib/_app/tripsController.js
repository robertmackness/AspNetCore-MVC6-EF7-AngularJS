!function(){"use strict";function r(r){var i=this;i.trips=[],i.newTrip={},i.errorMessage="",i.isBusy=!0,r.get("/api/trips").then(function(r){angular.copy(r.data,i.trips)},function(r){i.errorMessage="Failed to load data: "+r})["finally"](function(){i.isBusy=!1}),i.addTrip=function(){i.isBusy=!0,i.errorMessage="",i.newTrip.created=new Date,r.post("/api/trips",i.newTrip).then(function(r){i.trips.push(r.data),i.newTrip=""},function(r){i.errorMessage="Failed to save new trip"})["finally"](function(){})}}angular.module("app-trips").controller("tripsController",r)}();