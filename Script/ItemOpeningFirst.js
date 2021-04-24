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

                $("#Projects").prop('selectedIndex', 1);
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
    //Country Dropdown Selectedchange event
    $("#ItemGroups").change(function () {
        $("#Items").empty();
        $("#Items").append($("<option></option").val("").html("Select Item Name"));
        $('#dvLoading').show();
        $.ajax({
            type: 'POST',
            url: GetItemByGroup, // Calling json method

            dataType: 'json',

            data: { id: $("#ItemGroups").val() },
            // Get Selected Country ID.
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (names) {
                names = $.parseJSON(names)


                $.each(names, function (i, itname) {
                    $("#Items").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');

                });

                FindAllGroupLastOpeningInSite();
            },
            error: function (ex) {
                alert('Failed to retrieve Item Name.' + ex);
            }
        });
        return false;
    });
    $("#Projects").change(function () {
        $("#grid").empty();
    });
});

$(document).ready(function () {
    $("#TotalPurchase").keyup(function () {

        CalcTheoritical();


    });


    $("#TotalIssue").keyup(function () {

        //alert("A");
        CalcTheoritical();


    });


    function CalcTheoritical() {
        var LastOpening = $("#LastOpening").val();
        var TotalPurchase = $("#TotalPurchase").val();
        var TotalIssue = $("#TotalIssue").val();

        if (LastOpening == "" || LastOpening == null) {
            LastOpening = 0;
        }
        if (TotalPurchase == "" || TotalPurchase == null) {
            TotalPurchase = 0;
        }
        if (TotalIssue == "" || TotalIssue == null) {
            TotalIssue = 0;
        }

        var Theoritical = ((LastOpening + TotalPurchase) - TotalIssue).toFixed(2);
        $("#Theoritical").val(Theoritical);
        $("#Physical").val(Theoritical);
        DiffPT();
    }
    $("#Rate").keyup(function () {

        //alert("A");
        CalcTheoriticalRate();
        CalcPhysicalRate();

    });
    $("#Theoritical").keyup(function () {

        //alert("A");
        CalcTheoriticalRate();
        DiffPT();

    });
    $("#Physical").keyup(function () {

        //alert("A");

        CalcPhysicalRate();
        DiffPT();

    });
    function CalcTheoriticalRate() {
        var Theoritical = $("#Theoritical").val();
        var Rate = $("#Rate").val();
        if (Rate == "" || Rate == null) {
            Rate = 0;
        }
        if (Theoritical == "" || Theoritical == null) {
            Theoritical = 0;
        }

        var TheoriticalRate = +Theoritical * +Rate;
        $("#TStockValue").val(TheoriticalRate);
    }
    function CalcPhysicalRate() {
        var Physical = $("#Physical").val();
        var Rate = $("#Rate").val();
        if (Rate == "" || Rate == null) {
            Rate = 0;
        }
        if (Physical == "" || Physical == null) {
            Physical = 0;
        }

        var PhysicalRate = +Physical * +Rate;
        $("#PStockValue").val(PhysicalRate);
    }


    function DiffPT() {
        var Physical = $("#Physical").val();
        var Theoritical = $("#Theoritical").val();

        if (Theoritical == "" || Theoritical == null) {
            Theoritical = 0;
        }
        if (Physical == "" || Physical == null) {
            Physical = 0;
        }

        var Diff = (Theoritical - Physical).toFixed(2);
        $("#Difference").val(Diff);
    }

    window['CalStock'] = function () {
        //alert("A");
        CalcTheoritical();
        DiffPT()
        CalcTheoriticalRate();
        CalcPhysicalRate();
    }


    // alert("A");
    //$(document).on('keyup', "#TotalPurchase", function () {
    //    var current = $(this).find('input').val();
    //    var str = $(this).attr('class');
    //    var a = str.substring(8);
    //    var Qty = $("#Qty_" + a).find('input').val();
    //    var PReceive = $("#PReceive_" + a).find('input').val();
    //    var bal = +current + +PReceive;
    //    if (+bal > +Qty) {
    //        alert("Total Greater than  Purchase Qty");
    //        $(this).find('input').val('');
    //    }


    //});
})


$(document).ready(function () {
    //Item Dropdown Selectedchange event
    $("#Items").change(function () {
        //alert("A");
        $("#Unit").empty();
        //$("#grid").empty();
        $("#LastOpening").val('');
        $("#TotalPurchase").val('');

        $("#Rate").val('');

        $("#TotalIssue").val('');

        $("#Theoritical").val('');

        $("#Physical").val('');

        $("#Difference").val('');

        $("#TStockValue").val('');

        $("#PStockValue").val('');

        $('#dvLoading').show();
        $.ajax({
            type: 'POST',
            url: GetItemDetail2, // Calling json method

            dataType: 'json',

            data: { id: $("#Items").val(), pid: $("#Projects").val() },
            // Get Selected Country ID.
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (item) {
                item = $.parseJSON(item)


                $("#Unit").val(item.Unit);
                $("#Unit").attr('uid', item.UnitNo);

                $("#LastOpening").val(item.Opening);
                $("#TotalPurchase").val(item.Purchase);
                $("#TotalIssue").val(item.Issue);
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});


function SaveStock() {
    var url = SaveOpening;
    if ($("#HiddenOpeningDate").val() != "") {

        if ($('#Projects option:selected').val() != 0) {
            if ($('#Items option:selected').val() != 0) {
                if ($("#Opening").val() != 0 && $("#Opening").val() != "") {
                    if ($("#TotalPurchase").val() == null)
                        $("#TotalPurchase").val() = 0.00;
                    if ($("#Rate").val() == null)
                        $("#Rate").val() = 0.00;
                    if ($("#TotalIssue").val() == null)
                        $("#TotalIssue").val() = 0.00;
                    if ($("#Theoritical").val() == null)
                        $("#Theoritical").val() = 0.00;
                    if ($("#Physical").val() == null)
                        $("#Physical").val() = 0.00;
                    if ($("#Difference").val() == null)
                        $("#Difference").val() = 0.00;
                    if ($("#TStockValue").val() == null)
                        $("#TStockValue").val() = 0.00;
                    if ($("#PStockValue").val() == null)
                        $("#PStockValue").val() = 0.00;
                    $('#dvLoading').show();
                    $.ajax({

                        url: url,
                        type: 'GET',
                        data: { Project: $('#Projects option:selected').val(), ItemGroup: $('#ItemGroups option:selected').val(), Item: $('#Items option:selected').val(), Unit: $("#Unit").attr('uid'), TotalPurchase: $("#TotalPurchase").val(), Rate: $("#Rate").val(), TotalIssue: $("#TotalIssue").val(), Theoritical: $("#Theoritical").val(), Physical: $("#Physical").val(), Difference: $("#Difference").val(), TStockValue: $("#TStockValue").val(), PStockValue: $("#PStockValue").val(), ProjectName: $('#Projects option:selected').text(), LastOpening: $("#LastOpening").val(), OpeningDate: $("#HiddenOpeningDate").val() },
                        complete: function () {
                            $('#dvLoading').hide();
                        },
                        success: function (result) {
                            if (result == "1") {

                                $("#myModal").modal('show');
                                $("#Opening").val('');
                                FindOneItemLastOpeningInSite();
                            }
                        }
                    });
                    return false;
                }
                else {
                    alert("Give Quantity");
                }
            }
            else {
                alert("Enter Item");
            }
        }
        else {
            alert("Enter Project");
        }
    }
    else {
        alert("Kindly contact to admin for opening date.");
    }
}
window['FindAllGroupLastOpeningInSite'] = function () {


    var url = GetAllGroupLastOpeningInSite;
    if ($('#Projects option:selected').val() != 0 && $('#ItemGroups option:selected').val() != 0) {
        $('#dvLoading').show();
        $.ajax({

            url: url,
            type: 'GET',
            data: { Project: $('#Projects option:selected').val(), ItemGroup: $('#ItemGroups option:selected').val() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                //alert(result);
                $("#grid").empty();
                item = $.parseJSON(result)
                // alert(item);
                if (item != null) {
                    $.each(item, function (i, itname) {

                        //$("#grid").prepend("<tr class='danger _tempRow'><td>" + itname.ProjectName + "</td><td>" + itname.ItemGroupName + "</td><td>" + itname.ItemName + "</td><td>" + itname.tblItemMasterStock.Opening + "</td><td>" + itname.tblItemMasterStock.Rate + "</td></tr>");

                        $("#grid").prepend("<tr class='danger _tempRow'><td>" + itname.ItemGroupName + "</td><td>" + itname.ItemName + "</td><td>" + itname.tblItemMasterStock.LastOpening + "</td><td>" + itname.tblItemMasterStock.TotalPurchase + "</td><td>" + itname.tblItemMasterStock.TotalIssue + "</td><td>" + itname.tblItemMasterStock.TheoriticalQty + "</td><td>" + itname.tblItemMasterStock.PhysicalQty + "</td><td>" + itname.tblItemMasterStock.DiffFromTheoritical + "</td><td>" + itname.tblItemMasterStock.Rate + "</td><td>" + itname.tblItemMasterStock.TStockValue + "</td><td>" + itname.tblItemMasterStock.PStockValue + "</td></tr>");
                    })
                }
            }
        });
        return false;
    }

}
window['FindOneItemLastOpeningInSite'] = function () {


    var url = GetOneItemLastOpeningInSite;
    if ($('#Projects option:selected').val() != 0 && $('#Items option:selected').val() != 0) {
        $('#dvLoading').show();
        $.ajax
            ({

                url: url,
                type: 'GET',
                data: { Project: $('#Projects option:selected').val(), Item: $('#Items option:selected').val() },
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (result) {
                    $("#grid").empty();
                    item = $.parseJSON(result)
                    if (item != null) {
                        $.each(item, function (i, itname) {

                            // $("#grid").prepend("<tr class='danger _tempRow'><td>" + itname.ProjectName + "</td><td>" + itname.ItemGroupName + "</td><td>" + itname.ItemName + "</td><td>" + itname.tblItemMasterStock.Opening + "</td><td>" + itname.tblItemMasterStock.Rate + "</td></tr>");
                            $("#grid").prepend("<tr class='danger _tempRow'><td>" + itname.ItemGroupName + "</td><td>" + itname.ItemName + "</td><td>" + itname.tblItemMasterStock.LastOpening + "</td><td>" + itname.tblItemMasterStock.TotalPurchase + "</td><td>" + itname.tblItemMasterStock.TotalIssue + "</td><td>" + itname.tblItemMasterStock.TheoriticalQty + "</td><td>" + itname.tblItemMasterStock.PhysicalQty + "</td><td>" + itname.tblItemMasterStock.DiffFromTheoritical + "</td><td>" + itname.tblItemMasterStock.Rate + "</td><td>" + itname.tblItemMasterStock.TStockValue + "</td><td>" + itname.tblItemMasterStock.PStockValue + "</td></tr>");
                        })
                    }
                }
            });
        return false;
    }

}


$(document).ready(function () {
    $(".DatePicker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: true,
        changeYear: true,
    }).val('');
});