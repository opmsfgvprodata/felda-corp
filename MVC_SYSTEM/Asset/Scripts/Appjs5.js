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
                    //location.reload();
                    $('#myModal').modal('hide');
                    $('#ListofDetail').empty();
                    $('#ListofDetail').append('<table class="table table-hover table-bordered" style="font-size: 11px;" border="0"></table>');
                    var table = $('#ListofDetail').children();
                    table.append(result.tablelisting);
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