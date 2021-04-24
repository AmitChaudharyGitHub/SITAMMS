$(document).ready(function () {

    var EmpType = $("#hdnLoggedType").val();


    if (EmpType == "Emp") {
        $("#btnCreate").show();
    }
    else if (EmpType == "PIC") {
        $("#btnCreate").hide();
    }
    else {
        $("#btnCreate").hide();
    }

    window['GetPrj'] = function (dateString) {

        $.ajax({
            type: 'POST',
            url: GetProjectByEmpId, // Calling json method

            dataType: 'json',
            data: { E: ss },
            complete: function () {

            },
            success: function (result) {
                var procemessage = "<option value='0'> Please wait...</option>";
                $("#Projects").html(procemessage).show();
                var markup = "<option value='0'>Select Project</option>";
                $("#Projects").html(markup).show();
                result = $.parseJSON(result)

                var selectedDeviceModel = $('#Projects');
                $.each(result, function (index, item) {
                    selectedDeviceModel.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });
                if (PId != '') {
                    $('#Projects option:selected').val(PId);
                    GetData();
                }
                else {
                    $("#Projects").prop('selectedIndex', 1).trigger('change');
                }
            },
            error: function (ex) {
                alert('Failed to retrieve Project.' + ex);
            }
        });
      

        //-- For Bind another project on Create page

        $.ajax({
            type: 'POST',
            url: GetProjectByEmpId, // Calling json method

            dataType: 'json',
            data: { E: ss },
            complete: function () {

            },
            success: function (result1) {
                var procemessage = "<option value='0'> Please wait...</option>";
                $("#ddlProject").html(procemessage).show();
                var markup = "<option value='0'>Select Project</option>";
                $("#ddlProject").html(markup).show();
                result1 = $.parseJSON(result1)
                debugger;
                var selectedDeviceModel1 = $('#ddlProject');
                $.each(result1, function (index, item) {
                    selectedDeviceModel1.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });

                //$("#Projects").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Project.' + ex);
            }
        });
       

        //-- for Bind Transfer Site
       


        return false;
    };
    GetPrj();


    var timer;
    $(function () {
        $('#dateTime').html(getDateTime());

        // Do a $.ajax call and update start time in your database

        timer = setTimeout(function () {
            update();
        }, 1000);


    });

    function update() {
        $('#dateTime').html(getDateTime());

        timer = setTimeout(function () {
            update();
        }, 1000);
    }

    function getDateTime() {
        var currentdate = new Date();
        var datetime = "System Date Time: " + currentdate.getDate() + "/"
                    + (currentdate.getMonth() + 1) + "/"
                    + currentdate.getFullYear() + " "
                    + currentdate.getHours() + ":"
                    + currentdate.getMinutes() + ":"
                    + currentdate.getSeconds();
        return datetime;
    }

    $('#formbody').hide();
    $('#formbodyApproval').hide();

    $("#Projects").change(function () {
        $("#loading").show();

        if ($('#Projects option:selected').val() != 0) {
            debugger;


            $.get(GetPendingTransfer,
            { ProjectId: $('#Projects option:selected').val() },
            function (result) {
                $('#formbody').show();
                $('#formbody').html(result);
                $("#loading").hide();
            });

            $.get(GetApprovedTransfer,
           { ProjectId: $('#Projects option:selected').val() },
           function (result) {
               $('#formbodyApproval').show();
               $('#formbodyApproval').html(result);
               $("#loadingApproval").hide();
           });





        }
    });




});