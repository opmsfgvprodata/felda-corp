$(function () {
    $.ajaxSetup({ cache: false });
    $(document).on("click", "a[data-modal]", function (e) {
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                keyboard: true
            }, 'show');
            bindForm(this);
        });
        return false;
    });
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $('#progress').show();
        var formid = $(this).closest("form").attr("id");
        var dataform = $('#'+ formid)[0];
        //alert(formid);
        var data = new FormData(dataform);
        if (formid == 'create') {
            var file = document.getElementById("file").files[0];
            data.append("file", $("#file").file);
            var extension = $("#file").val().split('.').pop().toUpperCase();
            if (extension != "PDF") {
                $('#progress').hide();
                $.simplyToast('System can upload PDF format only', 'warning');
                return false;
            }
            $.ajax({
                url: this.action,
                type: this.method,
                data: data,
                dataType: 'json',
                contentType: false,
                processData: false,
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

                    $.simplyToast(result.msg, result.status);
                }
            });
        }
        else {
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

                    $.simplyToast(result.msg, result.status);
                }
            });
        }

        return false;
    });
}