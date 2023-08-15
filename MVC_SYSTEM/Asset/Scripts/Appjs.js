$(function () {
    $.ajaxSetup({ cache: false });
    $(document).on("click", "a[data-modal]", function (e) {
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                backdrop: 'static', keyboard: false
            }, 'show'); 
            bindForm(this);
        });
        return false;
    });
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $('#progress').show();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    if (result.checkingdata == '0') {
                        $('#myModal').modal('hide');
                        $('#progress').hide();
                        if (result.method == '1') {
                            //$("#grid").last().append("<tr><td>" + result.data1 + "</td><td>" + result.data2 + "</td><td>" + result.data3 + "</td><td><a data-modal='' href='/Admin/AgentDetail/Details/" + result.getid + "' id='" + result.getid + "' title='Detail'> <span class='glyphicon glyphicon-search'> </span> </a><a data-modal='' href='/Admin/AgentDetail/edit/" + result.getid + "' id='" + result.getid + "' title='Edit'> <span class='glyphicon glyphicon-edit'> </span> </a><a data-modal='' href='/phone/delete/" + result.getid + "' id='" + result.getid + "' title='Delete'> <span class='glyphicon glyphicon-trash'> </span> </a></td></tr>");
                        }
                        else if (result.method == '2') {
                            //getrow = getrow - 1
                            //alert(getrow);
                            //$("#grid").find("tr:eq(" + getrow + ")").remove();
                        }

                        if (result.data2 == 'creditcard') {
                            document.getElementById("creditcard").innerText = result.data1;
                        }
                    }
                    else {
                        $('#progress').hide();
                    }
                    location.reload();
                } else {
                    $('#progress').hide();
                    //$('#myModalContent').html(result);
                    //bindForm();
                }

                $.simplyToast(result.msg, result.status);
            }
        });

        return false;
    });
}

$(function () {
    $.ajaxSetup({ cache: false });
    $(document).on("click", "a[data-modal1]", function (e) {
        $('#myModalContent1').load(this.href, function () {
            $('#myModal1').modal({
                backdrop: 'static', keyboard: false
            }, 'show');
            bindForm(this);
        });
        return false;
    });
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $('#progress').show();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    if (result.checkingdata == '0') {
                        $('#myModal').modal('hide');
                        $('#progress').hide();
                        if (result.method == '1') {
                            //$("#grid").last().append("<tr><td>" + result.data1 + "</td><td>" + result.data2 + "</td><td>" + result.data3 + "</td><td><a data-modal='' href='/Admin/AgentDetail/Details/" + result.getid + "' id='" + result.getid + "' title='Detail'> <span class='glyphicon glyphicon-search'> </span> </a><a data-modal='' href='/Admin/AgentDetail/edit/" + result.getid + "' id='" + result.getid + "' title='Edit'> <span class='glyphicon glyphicon-edit'> </span> </a><a data-modal='' href='/phone/delete/" + result.getid + "' id='" + result.getid + "' title='Delete'> <span class='glyphicon glyphicon-trash'> </span> </a></td></tr>");
                        }
                        else if (result.method == '2') {
                            //getrow = getrow - 1
                            //alert(getrow);
                            //$("#grid").find("tr:eq(" + getrow + ")").remove();
                        }

                        if (result.data2 == 'creditcard') {
                            document.getElementById("creditcard").innerText = result.data1;
                        }
                    }
                    else {
                        $('#progress').hide();
                    }
                    //location.reload();
                } else {
                    $('#progress').hide();
                    //$('#myModalContent').html(result);
                    //bindForm();
                }

                $('#myModal').modal('hide');
                $.simplyToast(result.msg, result.status);
            }
        });

        return false;
    });
}