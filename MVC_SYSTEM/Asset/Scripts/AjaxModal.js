$(function () {
    $.ajaxSetup({ cache: false });
    $(document).on("click", "a[data-modal]", function (e) {
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                backdrop: 'static', keyboard: false
            }, 'show');
            //bindForm(this);
        });
        return false;
    });
});

$(function () {
    $.ajaxSetup({ cache: false });
    $(document).on("click", "a[data-modal1]", function (e) {
        $('#myModal').modal('hide');
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
                    console.log(result.method);
                    //reload div after return json
                    if (result.checkingdata === '0') {
                        $('#progress').hide();
                        if (result.method === '1') {
                            $('#myModal1').modal('hide');
                            var div = result.div;
                            console.log(div);
                            var controller = result.controller;
                            console.log(controller);
                            var action = result.action;
                            console.log(action);
                            var rootUrl = result.rootUrl;
                            console.log(rootUrl);
                            var url = rootUrl + "/" + controller + "/" + action;
                            console.log(url);

                            $("#" + div + "").load(url + "?page=1");
                            //$.get("@url.action(" + action + "," + controller + ")",
                            //    function (data) {
                            //        console.log(data)
                            //        $("#" + div + "").html(data);
                            //    });

                            $.simplyToast(result.msg, result.status);
                        }

                        //reload div after return json with parameter
                        else if (result.method === '2') {
                            $('#myModal1').modal('hide');
                            var div = result.div;
                            console.log(div);
                            var controller = result.controller;
                            console.log(controller);
                            var action = result.action;
                            console.log(action);
                            var rootUrl = result.rootUrl;
                            console.log(rootUrl);
                            var paramName = result.paramName;
                            console.log(paramName);
                            var paramValue = result.paramValue;
                            console.log(paramValue);
                            var url = rootUrl + "/" + controller + "/" + action + "/?" + paramName + "=" + paramValue;
                            console.log(url);

                            $("#" + div + "").load(url);
                            $.simplyToast(result.msg, result.status);
                        }

                        //reload div after return json with parameter
                        else if (result.method === '3') {
                            //$('#myModal1').modal('hide');
                            var div = result.div;
                            console.log(div);
                            var controller = result.controller;
                            console.log(controller);
                            var action = result.action;
                            console.log(action);
                            var rootUrl = result.rootUrl;
                            console.log(rootUrl);
                            var paramName = result.paramName;
                            console.log(paramName);
                            var paramValue = result.paramValue;
                            console.log(paramValue);
                            var paramName2 = result.paramName2;
                            console.log(paramName2);
                            var paramValue2 = result.paramValue2;
                            console.log(paramValue2);
                            var url = rootUrl + "/" + controller + "/" + action + "/?" + paramName + "=" + paramValue + "&" + paramName2 + "=" + paramValue2;
                            console.log(url);

                            $("#" + div + "").load(url);
                            $.simplyToast(result.msg, result.status);
                        }

                        else if (result.method === '4') {
                            $('#myModal1').modal('hide');
                            var div = result.div;
                            console.log(div);
                            var controller = result.controller;
                            console.log(controller);
                            var action = result.action;
                            console.log(action);
                            var rootUrl = result.rootUrl;
                            console.log(rootUrl);
                            var paramName = result.paramName;
                            console.log(paramName);
                            var paramValue = result.paramValue;
                            console.log(paramValue);
                            var paramName2 = result.paramName2;
                            console.log(paramName2);
                            var paramValue2 = result.paramValue2;
                            console.log(paramValue2);
                            var paramName3 = result.paramName3;
                            console.log(paramName2);
                            var paramValue3 = result.paramValue3;
                            console.log(paramValue3);
                            var url = rootUrl + "/" + controller + "/" + action + "/?" + paramName + "=" + paramValue + "&" + paramName2 + "=" + paramValue2 + "&" + paramName3 + "=" + paramValue3;
                            console.log(url);

                            $("#" + div + "").load(url);
                            $.simplyToast(result.msg, result.status);
                        }

                        else if (result.method === '5') {
                            $('#myModal1').modal('hide');
                            var div = result.div;
                            console.log(div);
                            var controller = result.controller;
                            console.log(controller);
                            var action = result.action;
                            console.log(action);
                            var rootUrl = result.rootUrl;
                            console.log(rootUrl);
                            var paramName = result.paramName;
                            console.log(paramName);
                            var paramValue = result.paramValue;
                            console.log(paramValue);
                            var paramName2 = result.paramName2;
                            console.log(paramName2);
                            var paramValue2 = result.paramValue2;
                            console.log(paramValue2);
                            var paramName3 = result.paramName3;
                            console.log(paramName3);
                            var paramValue3 = result.paramValue3;
                            console.log(paramValue3);
                            var paramName4 = result.paramName4;
                            console.log(paramName4);
                            var paramValue4 = result.paramValue4;
                            console.log(paramValue4);
                            var url = rootUrl + "/" + controller + "/" + action + "/?" + paramName + "=" + paramValue + "&" + paramName2 + "=" + paramValue2 + "&" + paramName3 + "=" + paramValue3 + "&" + paramName4 + "=" + paramValue4;
                            console.log(url);

                            $("#" + div + "").load(url);
                            $.simplyToast(result.msg, result.status);
                        }
                    }
                }

                else {
                    //return error alert
                    if (result.checkingdata === '0') {
                        $.simplyToast(result.msg, result.status);
                    }
                }
            }
        });
        return false;
    });
}