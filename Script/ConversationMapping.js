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


})


$(document).ready(function () {

    $(window).load(function () {
        $.ajax({
            type: 'Get',
            url: GetItemGroup, // Calling json method
            success: function (data) {


                $.each(data, function (i, itname) {
                    $("#ddlItemGroup").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');



                });


                // $("#Projects").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve ItemGroup.' + ex);
            }
        });


        $.ajax({

            type: 'Get',
            url: GetFromUnitDetail,
            success: function (data) {
                $.each(data, function (i, itname) {

                    $("#ddlUnit").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');

                });
            },
            error: function (ex) {
                alert('Failed to retrieve Unit.' + ex);
            }

        });



    });




    $("#Projects").change(function () {
        $("#ddlSourceLocation").empty();
        $("#ddlSourceLocation").append($("<option></option").val("").html("Select SourceLocation"));
        if ($('#Projects option:selected').val() != 0) {

            $.ajax({

                type: 'POST',
                url: GetSourceLocation,
                dataType: 'json',
                data: { PrID: $("#Projects").val() },
                success: function (data) {

                    $.each(data, function (i, itname) {
                        $("#ddlSourceLocation").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');



                    });

                }

            })
        }

    }

    )


    $("#ddlItemGroup").change(function () {
        $("#ddlItemName").empty();
        $("#ddlItemName").append($("<option></option").val("").html("Select ItemName"));
        if ($('#ddlItemGroup option:selected').val() != 0) {
            $.ajax({

                type: 'POST',
                url: GetItemDetail,
                dataType: 'json',
                data: { GrpId: $("#ddlItemGroup").val() },
                success: function (data) {

                    $.each(data, function (i, itname) {
                        $("#ddlItemName").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');



                    });

                }

            })
        }
    });

    $("#ddlUnit").change(function () {
        $("#ddlUnitConversation").empty();
        $("#ddlUnitConversation").append($("<option></option").val("").html("Select UniteConversion"));
        if ($('#ddlUnit option:selected').val() != 0) {
            $.ajax({

                type: 'POST',
                url: GetUnitConversionTo,
                dataType: 'json',
                data: { UnitId: $("#ddlUnit").val() },
                success: function (data) {

                    $.each(data, function (i, itname) {
                        $("#ddlUnitConversation").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');



                    });

                }

            })
        }
    });


    $("#ddlUnitConversation").change(function () {
        $('#txtConversionRate').val('');
        if ($('#ddlUnitConversation option:selected').val() != 0) {
            $.ajax({

                type: 'POST',
                url: GetConversionRate,
                dataType: 'json',
                data: { To_UnitConversionId: $("#ddlUnitConversation").val(), From_UnitId: $("#ddlUnit").val() },
                success: function (data) {
                    $("#txtConversionRate").val(data);
                }

            })
        }





    });


    $('#btnsave').click(function (e) {


        var _griddata = gridToArray();

        var url = MappData;
        $.ajax({
            type: 'POST',
            url: url,
            data: JSON.stringify(_griddata),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            processdata: true,

            success: function (json) {


                if (json.Status == "1") {



                    alert("Data Saved Successfully");
                    window.location.href = RedirUrl;



                }

            },







            error: function () {
                alert('Error in Submit Data');
            }
        });
    });

    function gridToArray() {
        var obj = new Object();

        obj.ProjectId = $("#Projects option:selected").val();
        obj.SourceLocationId = $("#ddlSourceLocation option:selected").val();
        obj.ItemGroupId = $("#ddlItemGroup option:selected").val();
        obj.ItemId = $("#ddlItemName option:selected").val();
        obj.UnitFromId = $("#ddlUnit option:selected").val();
        obj.UnitConversionId = $("#ddlUnitConversation option:selected").val();
        obj.UnitConversationRate = $("#txtConversionRate").val();
        return obj;
    }



});