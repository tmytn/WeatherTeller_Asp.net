$(document).ready(function () {

    $("#btn_curLocation").on("click", function () {
        $("#curlat").val("");
        $("#curlon").val("");
        $("#requestString").val("");

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition);
        } else {
            $("#currentLocation").append("Geolocation is not supported by this browser.");
        }
        alert(position.coords.latitude + "****" + position.coords.longitude);
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(function (position) {
                infoWindow = new google.maps.InfoWindow({ map: map });
                var pos = { lat: position.coords.latitude, lng: position.coords.longitude };
                infoWindow.setPosition(pos);
                infoWindow.setContent("Found your location <br />Lat : " + position.coords.latitude + " </br>Lang :" + position.coords.longitude);
                map.panTo(pos);
               
                $("#apiDiv form").submit();
            });
        } else {
           alert("Browser doesn't support geolocation!");
        }
    });
    function showPosition(position) {
        alert(position.coords.latitude + "-" + position.coords.longitude);
        $("#curlat").val(position.coords.latitude);
        $("#curlon").val(position.coords.longitude);
        $("#map").css("width", "40%");
        $("#map").css("height", "200px");

    }

    //===================================================================================

    var map;
    function initMap() {
        alert("initmap : " + $("#curlat").val() + "/" + $("#curlon").val());
        var mapCenter = new google.maps.LatLng($("#curlat").val(), $("#curlon").val()); //Google map Coordinates
        map = new google.maps.Map($("#map")[0], {
            center: mapCenter,
            zoom: 8
        });
    }
    $("#historyarea table tr").click(function () {
        alert($(this).html());
        //var latInfo = $(this).find("td label:eq(0)").html();
        //var lonInfo = $(this).find("td label:eq(1)").html();
        //alert(latInfo + "-----" + lonInfo);

        //if ("geolocation" in navigator) {
        //    navigator.geolocation.getCurrentPosition(function (position) {
        //        position.coords.latitude = $("#curlat").val;
        //        position.coords.longitude = $("#curlon").val;
        //        infoWindow = new google.maps.InfoWindow({ map: map });
        //        var pos = { lat: position.coords.latitude, lng: position.coords.longitude };
        //        infoWindow.setPosition(pos);
        //        infoWindow.setContent("Found your location <br />Lat : " + position.coords.latitude + " </br>Lang :" + position.coords.longitude);
        //        map.panTo(pos);
        //    });
        //} else {
        //    console.log("Browser doesn't support geolocation!");
        //}
    });
});