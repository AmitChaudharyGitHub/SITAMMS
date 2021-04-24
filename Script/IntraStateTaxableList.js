$(document).ready(function () {

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

                $("#Projects").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Project.' + ex);
            }
        });
        return false;

    };

    GetPrj();


    $("#Projects").change(function () {
        debugger;
        $("#loading").show();

        if ($('#Projects option:selected').val() != 0) {
            debugger;


            $.get(GetPendingIntraTaxableTransfer,
            { ProjectId: $('#Projects option:selected').val() },
            function (result) {
                $('#formbody').show();
                $('#formbody').html(result);
                $("#loading").hide();
            });

            $.get(GetUpdatedIntraTaxableTransfer,
           { ProjectId: $('#Projects option:selected').val() },
           function (result) {
               $('#formbodyApproval').show();
               $('#formbodyApproval').html(result);
               $("#loading").hide();
           });


        }
    });
});