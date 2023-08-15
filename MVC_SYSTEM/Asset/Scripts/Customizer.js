function GetDataToList(data, data2, link) {
    var passparam = document.getElementById(data.id).value;
    $.ajax({
        url: '/Contacts/' + link,
        type: "GET",
        dataType: "JSON",
        data: { param: passparam },
        success: function (results) {
            $("#" + data2 + "").html(""); // clear before appending new list
            $.each(results, function (i, result) {
                $("#" + data2 + "").append(
                    $('<option></option>').val(result.Value).html(result.Text));
            });
        }
    });
}