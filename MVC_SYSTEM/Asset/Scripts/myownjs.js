$(document).ready(function () {
    var checkinput = $('input[type=radio][name="SearchBy"]:checked').val();
    if (checkinput == 'Name') {
        $("#statusdiv").hide();
        $("#searchtextdiv").show();
    }
    if (checkinput == 'Client') {
        $("#statusdiv").hide();
        $("#searchtextdiv").show();
    }
    if (checkinput == 'Status') {
        $("#searchtextdiv").hide();
        $("#statusdiv").show();
        $('#Search').val("");
    }
    $('input[type="radio"]').click(function () {
        if ($(this).attr("value") == "Name") {
            //alert("Name");
            $("#statusdiv").hide();
            $("#searchtextdiv").show();
        }
        if ($(this).attr("value") == "Client") {
            //alert("Client");
            $("#statusdiv").hide();
            $("#searchtextdiv").show();
        }
        if ($(this).attr("value") == "Status") {
            //alert("Status");
            $("#searchtextdiv").hide();
            $("#statusdiv").show();
            $('#Search').val("");
        }
    });
});