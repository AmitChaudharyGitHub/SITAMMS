
$(document).ready(function () {



    window['GetPrj'] = function (dateString) {
        $('#dvLoading').show();
        $.ajax({
            type: 'POST',
            url: GetProjectByEmpId, // Calling json method

            dataType: 'json',
            data: { E: ss },
            complete: function () {
                $('#dvLoading').hide();
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






}
    )


$(document).ready(function () {
    $("#Projects").change(function () {
        // alert("vx");
        $("#ItemGroups").empty();
      //  $("#grid").empty();
        $("#ItemGroups").append($("<option></option").val("").html("Select Item Group"));
        if ($('#Projects option:selected').val() != 0) {

            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemGroup, // Calling json method

                dataType: 'json',

                data: { Pid: $("#Projects option:selected").val() },
           
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (names) {
                    names = $.parseJSON(names)


                    $.each(names, function (i, itname) {
                        $("#ItemGroups").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');

                    });


                },
                error: function (ex) {
                    alert('Failed to retrieve Item Groups.' + ex);
                }
            });
            return false;
        }
    });

});