$(document).ready(function () {

    window['GetPrj'] = function (dateString) {

        $.ajax({
            type: 'POST',
            url: GetGroup,

            dataType: 'json',

            complete: function () {

            },
            success: function (result) {
                var procemessage = "<option value='0'> Please wait...</option>";
                $("#ItemGroup").html(procemessage).show();

                var markup = "<option value='0'>Select Item Group</option>";

                $("#ItemGroup").html(markup).show();


                result = $.parseJSON(result)


                var selectedDeviceModel = $('#ItemGroup');

                $.each(result, function (index, item) {
                    selectedDeviceModel.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });


            },
            error: function (ex) {
                alert('Failed to retrieve.' + ex);
            }
        });

        return false;

    };
    GetPrj();
    $("#ItemGroup").change(function () {
        $("#ItemMaster").empty();
        $("#ItemMaster").append($("<option></option").val("").html("Select Item Name"));

        if ($("#ItemGroup option:selected").val() !== 0) {

            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemByGroup,

                dataType: 'json',


                data: { id: $("#ItemGroup").val() },

                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (names) {
                    names = $.parseJSON(names)


                    $.each(names, function (i, itname) {
                        $("#ItemMaster").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');

                    });
                },
                error: function (ex) {
                    alert('Failed to retrieve Item Name.' + ex);
                }
            });
            return false;
        }
    })


    $("body").on("keyup", 'input[type="number"]', function () {
        var valid = true;
        $('input[type="number"]').each(function () {

            if ($(this).is(':invalid')) {
                console.log("not valid");

                valid = false;
            }
        })
        if (valid == false) {
            $("#btnSubmit").attr("disabled", "disabled");
        }
        else {
            $("#btnSubmit").removeAttr("disabled");
        }

    });


})

$('.webgrid-table td:nth-child(3),th:nth-child(3)').hide();
$('.webgrid-table td:nth-child(4),th:nth-child(4)').hide();

$('#form_body').hide(); $('#dvLoading').hide();
$('#btn_Search').click(function (e) {
    $("#dvLoading").show();
    e.preventDefault();

    $.ajax({
        url: GetGrid,
        type: 'GET',
        data: { ItemgrpId: $("#ItemGroup option:selected").val(), ItemId: $("#ItemMaster option:selected").val() },
        complete: function () {
            $('#dvLoading').hide();

        },
        success: function (result) {
            $('#form_body').html(result);
            $('#form_body').show();
        }
    });
    return false;

});

$("#btnSubmit").click(function (e) {
    var valid = true;
    $('input[type="number"]').each(function () {

        if ($(this).is(':invalid')) {
            console.log("not valid");
            valid = false;

        }
    })

    if (valid) {
        debugger;
        var json = JSON.parse(gridTojson());
        if (jQuery.isEmptyObject(json)) {
            alert('Somthing is missing.');
            return false;
        }


        var _griddata = gridTojson();

        $.ajax({
            url: UpdateGrid,
            type: 'POST',
            data: { griddata: _griddata }
        }).done(function (data) {
            debugger;
            if (data.Status == 1) {
                // $("mymodel").modal('show');
                alert("Successfully Updated.");
                $('#form_body').hide();
            }
            else if (data.Status == 2) {
                alert("Unale to bind items, Please refreash page and try again");
            }
            else if (data.Status == 3) {
                alert("Session value is expired, please login again.");
            }
            else if (data.Status == 4) {
                alert("Item is not found. Please refreash page ,if still then contact to administrator.");
            }
            else if (data.Status == 5) {
                alert("Item percentage should be greater than Zero.");
            }
            else if (data.Status == 6) {
                alert("Nullable List. Try again later. ");
            }
            else if (data.Status == 7) {
                alert("Some Internal problem.Kindly contact to administrator");
            }
        });


    }





});


function gridTojson() {
    debugger;
    var json = '{';
    var otArr = [];
    debugger;
    var tbl2 = $('#grid tbody tr').each(function (i) {
        debugger;
        // var koko = $(this).children('td:nth-child(8) input').val();
        if ($(this)[0].rowIndex != 0) {
            x = $(this).children();
            var itArr = [];
            x.each(function (b) {


                debugger;
                if ($(this).children('input').length > 0) {
                    itArr.push('"' + $(this).children('input').val() + '"');
                }
                else {
                    debugger;
                    if (b == 0 || b == 2 || b == 3 || b == 8) {
                        var x = $(this).text();

                        itArr.push('"' + x.replace('"', "").replace('"', '_') + '"');
                    }

                }
            });

            otArr.push('"' + i + '": [' + itArr.join(',') + ']');
        }
    })
    json += otArr.join(",") + '}'
    //debugger;
    //alert(json)
    return json;
}