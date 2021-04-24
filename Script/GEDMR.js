  
  
    $(document).ready(function () {
        $("#empName1").change(function () {
            // alert("A");
            GetPrj();
        });
    });
    $(document).ready(function ()
    {
    
        window['lol'] = function() { alert('lol');   };
       
        window['GetPrj'] =    function (dateString) 
        {
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetProjectByEmpId, // Calling json method

                dataType: 'json',               
                data: { E: $("#empName1 option:selected").val() },
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (result) {
                    var procemessage = "<option value='0'> Please wait...</option>";
                    $("#Prj").html(procemessage).show();
                    var markup = "<option value='0'>Select Project</option>";
                    $("#Prj").html(markup).show();
                    result = $.parseJSON(result)
                    
                    var selectedDeviceModel = $('#Prj');
                    $.each(result, function (index, item)
                    {
                        selectedDeviceModel.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });
                    $("#Prj").prop('selectedIndex', 1).trigger('change');
                    //$.each(partes, function (i, state)
                    //{                       
                    //markup += "<option value=" + state.Value + ">" + state.Text + "</option>";
                    //});
                    //$("#Prj").html(markup).show();
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
        $(".DatePicker").datepicker({
            dateFormat: 'dd M yy',
            changeMonth: true,
            changeYear: true,
        });
    });
        
function getblock()
{
    //  alert("A");


    if ($("#Prj option:selected").val() != 0) {
        var url = Grid;

        $('#dvLoading').show();
        $.ajax({

            url: url,
            type: 'GET',
            data: { ReceiveType: $("input[name='P']:checked").val(), fromDate: $('#fromDate').val(), toDate: $('#toDate').val(), PrjId: $("#Prj option:selected").val(), IG: $("#ItemGroup option:selected").val(), IT: $("#ItemName option:selected").val(), Vg: $("#VNAME option:selected").text() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                $('#formbody').html(result);
            }
        });
        return false;
    }
    else
    {
        alert("Select Project");
    }

}
   
$(document).ready(function () {
    //Country Dropdown Selectedchange event
    $("#ItemGroup").change(function () {
        $("#ItemName").empty();
        $("#ItemName").append($("<option></option").val("").html("For All Items "));
        if ($("#ItemGroup option:selected").val() != 0) {
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemByGroup, // Calling json method

                dataType: 'json',

                data: { id: $("#ItemGroup").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (names) {
                    names = $.parseJSON(names)


                    $.each(names, function (i, itname) {
                        $("#ItemName").append('<option value="' + itname.Value + '">' +
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
});

$(document).ready(function () {
//    //Country Dropdown Selectedchange event

//    $("#VGID").change(function () {
//        $("#VNAME").empty();
//        if ($("#VGID option:selected").val() != 0) {
//            $('#dvLoading').show();
//            $("#VNAME").empty();
//            $.ajax({
//                type: 'POST',
//                url: GetVendorByGroup, // Calling json method

//                dataType: 'json',

//                data: { Vid: $("#VGID").val(), Pid: $("#Prj").val() },
//                // Get Selected Country ID.
//                complete: function () {
//                    $('#dvLoading').hide();
//                },
//                success: function (obj) {
//                    obj = $.parseJSON(obj)



//                    // console.log(obj)
//                    GetVendor(obj);

//                },
//                error: function (ex) {
//                    // alert('Failed to retrieve Project Code.' + ex);
//                }
//            });
//            return false;
//        }
//    })
});
$(document).ready(function () {
    //Country Dropdown Selectedchange event

    $("#Prj").change(function () {
        debugger;
        $("#VNAME").empty();
        if ($("#Prj option:selected").val() != 0) {
            $('#dvLoading').show();
            $("#VNAME").empty();
            $.ajax({
                type: 'POST',
                url: GetVendorByGate, // Calling json method

                dataType: 'json',

                data: { Pid: $("#Prj option:selected").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)

                    var Vendor = $.parseJSON(obj.Vendor);
                    //var Item = $.parseJSON(obj.Item);
                    var ItemGroup = $.parseJSON(obj.ItemGroup);

                    var markup1 = "<option value='0'>For All Vendors </option>";
                    $("#VNAME").html(markup1).show();
                    // markup1 = "<option value='0'>For All Items </option>";
                    //$("#ItemName").html(markup1).show();
                    markup1 = "<option value='0'>For All Item Group </option>";
                    $("#ItemGroup").html(markup1).show();

                    var selectedDeviceModel1 = $('#VNAME');
                    $.each(Vendor, function (index, item) {
                        //alert("A");
                        selectedDeviceModel1.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });
                    //var selectedDeviceModel2 = $('#ItemName');
                    //$.each(Item, function (index, item) {
                    //    selectedDeviceModel2.append(
                    //        $('<option/>', {
                    //            value: item.Value,
                    //            text: item.Text
                    //        })
                    //    );
                    //});
                    var selectedDeviceModel3 = $('#ItemGroup');
                    $.each(ItemGroup, function (index, item) {
                        selectedDeviceModel3.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });
                    // console.log(obj)
                   // GetVendor(obj);

                },
                error: function (ex) {
                    // alert('Failed to retrieve Project Code.' + ex);
                }
            });
            return false;
        }
    })
});

function GetVendor(data) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#VNAME").html(procemessage).show();
    var markup = "<option value='0'>For All Vendors </option>";
    for (var x = 0; x < data.length; x++) {


        //console.log(data[x].Value);
        if (data[x].Value != null)
            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

    }
    $("#VNAME").html(markup).show();

}