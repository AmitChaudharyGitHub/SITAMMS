$(document).ready(function () {
  
    $(document).on('click', '.amountDetails', function () {
        var qty = $(this).attr('data');
        var itemId = $(this).attr('data1');
        $("#dialog-amount-details").dialog({
            modal: true,
            title: "Amount Details",
            width: 350,
            height: 350,
            autoOpen: false,
            open: function () {
                $.get('/IndentRequistion/AmountDetails', { ProjectId: $('#ddlProject option:selected').val(), ItemId: itemId, Qty: qty },
                    function (response) {
                        $('#dialog-amount-details').html(response);
                    });
            },
            buttons: [
            {
                id: "Close",
                text: "Close",
                click: function () {
                    $(this).dialog('close');
                }
            }
            ]
        });

        $('#dialog-amount-details').dialog('open');
    });

    $('#txtDeliveryQty').change(function () {
        if ($('#ddlProject option:selected').val() == '0') {
            alert('Please select project');
            $('#quantity1').val('');
            return false;
        }
        if ($('#ItemMaster option:selected').val() == '') {
            alert('Please select item');
            $('#quantity1').val('');
            return false;
        }


        $('#txtEstimatedValue').val('');
        $.get('/IndentRequistion/GetAmount', { ProjectId: $('#ddlProject option:selected').val(), ItemId: $('#ItemMaster option:selected').val(), Qty: $('#txtDeliveryQty').val() },
        function (result) {
            console.log(result);
            $('#txtEstimatedValue').val(result);
        });
    });



  $("#txtTransferDate").val('');

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
                $("#ddlProject").html(procemessage).show();
               // $("#ddlTransferProject").html(procemessage).show();
                var markup = "<option value='0'>Select Project</option>";
              //  var markup1 = "<option value='0'>Select Transfer Project</option>";
                $("#ddlProject").html(markup).show();
               // $("#ddlTransferProject").html(markup1).show();

                result = $.parseJSON(result)


                var selectedDeviceModel = $('#ddlProject');
               // var selectedDeviceModel1 = $('#ddlTransferProject');
                $.each(result, function (index, item) {
                    selectedDeviceModel.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });

                //$.each(result, function (index, item) {
                //    selectedDeviceModel1.append(
                //        $('<option/>', {
                //            value: item.Value,
                //            text: item.Text
                //        })
                //    );
                //});

                //$("#ddlProject").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Project.' + ex);
            }
        });


        //--- Bind Transpoartation and Vehicle 

        $.ajax({
            type: 'POST',
            url: GetTranspoartationType, // Calling json method

            dataType: 'json',

            complete: function () {

            },
            success: function (result) {
                var procemessage = "<option value='0'> Please wait...</option>";
                $("#ddlmodeofTPT").html(procemessage).show();

                var markup = "<option value='0'>Select Mode Of TPT</option>";

                $("#ddlmodeofTPT").html(markup).show();


                result = $.parseJSON(result)


                var selectedDeviceModel = $('#ddlmodeofTPT');

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

        $.ajax({
            type: 'POST',
            url: GetVehicle, // Calling json method

            dataType: 'json',

            complete: function () {

            },
            success: function (result) {
                var procemessage = "<option value='0'> Please wait...</option>";
                $("#ddlVehicleOwnerType").html(procemessage).show();

                var markup = "<option value='0'>Select Vehicle Owner Type</option>";

                $("#ddlVehicleOwnerType").html(markup).show();


                result = $.parseJSON(result)


                var selectedDeviceModel = $('#ddlVehicleOwnerType');

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

        $.ajax({
            type: 'POST',
            url: GetGroup, // Calling json method

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

        $.ajax({
            type: 'POST',
            url: GetTaxPayableOnReverseCharges, // Calling json method

            dataType: 'json',

            complete: function () {

            },
            success: function (result) {
                var procemessage = "<option value='0'> Please wait...</option>";
                $("#ddlTaxPayableOnReverse").html(procemessage).show();

                var markup = "<option value='0'>Select Status</option>";

                $("#ddlTaxPayableOnReverse").html(markup).show();


                result = $.parseJSON(result)


                var selectedDeviceModel = $('#ddlTaxPayableOnReverse');

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
                alert('Failed to retrieve Status.' + ex);
            }
        });



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

    $(".DatePicker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: true,
        changeYear: true,
        value: "",
        maxDate: new Date()
    });


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

    $("#ItemMaster").change(function () {

        if ($("#ddlProject option:selected").val() == "0") {
            alert("Select Project"); return false;
        }

        debugger;
        if ($("#ItemMaster option:selected").val() != "0") {
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemDetail,

                dataType: 'json',

                data: { ProjectId: $("#ddlProject option:selected").val(), ItemGroupId: $("#ItemGroup option:selected").val(), ItemId: $("#ItemMaster option:selected").val() },

                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (item) {
                    item = $.parseJSON(item)


                    $("#Unit").val(item.UniteCode);
                    // $("#Unit").attr("UNo", item.UnitNo);
                    $("#HSNCode").val(item.HhnCode);
                    $("#GIETNCode").val(item.Gietm);
                    $("#txtStockQty").val(item.BalQty);
                    $("#txtBalanceQty").val(item.BalQty);
                    $("#txtDiscountedRate").val(item.DiscountedRate);
                    $("#txtDeliveryRate").val(item.DiscountedRate);

                },
                error: function (ex) {
                    alert('Failed to retrieve' + ex);
                }
            });

            return false;
        }


    })

    $("#ddlProject").change(function () {

        if ($("#ddlProject option:selected").val() != "0") {
            $("#pnl_toggel_Project_Detail").show();
            $("#hr1").show();
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetProjectDetail, // Calling json method

                dataType: 'json',

                data: { ProjectId: $("#ddlProject option:selected").val() },

                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)



                    $("#lbl_project_name").text(obj.ProjectName);
                    $("#lbl_Project_address").text(obj.Projectadd);
                    $("#lbl_Project_State").text(obj.StateName);
                    $("#lbl_project_GSTIN").text(obj.GSTNo);
                   // $("#txtTransferNumber").val(obj.InterStateTransferCode);
                    

                },
                error: function (ex) {
                    alert('Failed to retrieve Vendor Code.' + ex);
                }
            });

          


            $.ajax({
                type: 'POST',
                url: GetAllProjectexceptSelectedProject, // Calling json method

                dataType: 'json',
                data: { ProjectId: $("#ddlProject option:selected").val(), TransferType:'Intra' },
                complete: function () {

                },
                success: function (result) {
                    var procemessage = "<option value='0'> Please wait...</option>";

                    $("#ddlTransferProject").html(procemessage).show();

                    var markup1 = "<option value='0'>Select Transfer Project</option>";

                    $("#ddlTransferProject").html(markup1).show();

                    result = $.parseJSON(result)



                    var selectedDeviceModel1 = $('#ddlTransferProject');


                    $.each(result, function (index, item) {
                        selectedDeviceModel1.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });

                    //$("#ddlProject").prop('selectedIndex', 1).trigger('change');
                },
                error: function (ex) {
                    alert('Failed to retrieve Project.' + ex);
                }
            });


            return false;


        }
        else {
            $("#lbl_project_name").text('');
            $("#lbl_Project_address").text('');
            $("#lbl_Project_State").text('');
            $("#lbl_project_GSTIN").text('');
            $("#hr1").hide();
        }

    });
    $("#ddlTransferProject").change(function () {
        if ($("#ddlTransferProject option:selected").val() != "0") {
            $("#pnl_toggel_TransferProject_Detail").show();
            $("#pnl_toggel_TransferProject_Detail2").show();
            $('#dvLoading').show();
            $("#hr2").show();

            $.ajax({
                type: 'POST',
                url: GetProjectDetail, // Calling json method

                dataType: 'json',

                data: { ProjectId: $("#ddlTransferProject option:selected").val() },

                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)



                    $("#lbl_Transferproject_name").text(obj.ProjectName);
                    $("#lbl_TransferProject_address").text(obj.Projectadd);
                    $("#lbl_TransferProject_State").text(obj.StateName);
                    $("#lbl_Transferproject_GSTIN").text(obj.GSTNo);


                },
                error: function (ex) {
                    alert('Failed to retrieve Vendor Code.' + ex);
                }
            });
            return false;


        }
        else {
            $("#lbl_Transferproject_name").text('');
            $("#lbl_TransferProject_address").text('');
            $("#lbl_TransferProject_State").text('');
            $("#lbl_Transferproject_GSTIN").text('');
            $("#hr2").hide();
            $("#pnl_toggel_TransferProject_Detail").hide();
            $("#pnl_toggel_TransferProject_Detail2").hide();
        }

    });


    var gridList = [];
    var pf = 0;
    var Tdl = 0;
    var Tax = 0;
    var GT = 0;
    var f = 0;
    var child = 0;
    var nowTemp = new Date();

    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);


    $("#addNew").click(function () {
        debugger;

        var obj = new Object();
        obj.Id = getUniqueId();

        obj.IntraStateTransferMasterId = $("#txtTransferNumber").val();
        obj.ProjectId = $("#ddlProject option:selected").val();
        obj.ItemGroupId = $("#ItemGroup option:selected").val();
        obj.ItemId = $("#ItemMaster option:selected").val();
        obj.StockQty = $("#txtStockQty").val();
        obj.DeliveryQty = $("#txtDeliveryQty").val();
        obj.BalancedQty = $("#txtBalanceQty").val();
        obj.DiscountedRate = $("#txtDiscountedRate").val();
        obj.DeliveryRate = $("#txtDeliveryRate").val();
        obj.EstimatedValue = $("#txtEstimatedValue").val();

        obj.ItemGroupName = $("#ItemGroup option:selected").text();
        obj.ItemName = $("#ItemMaster option:selected").text();
        obj.UnitCode = $("#Unit").val();
        obj.HSNCode = $("#HSNCode").val();
        obj.GIETMCode = $("#GIETNCode").val();
   

        //obj.PurRequisitionNo = $("#PurchaseIRNo").val();        
        //obj.Date = $("#PurchaseIRDate").val();
        //obj.ProjectNo = $("#Project option:selected").val();
        //obj.ProjectName = $("#Project option:selected").text();

        if (obj.ItemGroupId === "0") {
            $('#ItemGroup').css('border-color', '#f0551b');
            return;
        }
        if (obj.ItemId === "0") {
            $('#ItemMaster').css('border-color', '#f0551b');
            return;
        }


        if (obj.DeliveryQty === "") {
            $('#txtDeliveryQty').css('border-color', '#f0551b');
            return;
        }
        if (obj.DeliveryRate === "") {
            $('#txtDeliveryRate').css('border-color', '#f0551b');
            return;
        }
        if (obj.EstimatedValue === "") {
            $('#txtEstimatedValue').css('border-color', '#f0551b');
            return;
        }
        //if (obj.CurrentRate <= 0) {
        //    alert('Rate should be greater than zero. ')
        //    $('#CRate').css('border-color', '#f0551b');
        //    $("#CRate").val('')
        //    return;
        //}
        if (gridList.findIndex(x=>x.ItemId == obj.ItemId) > -1) {
            alert('Item already added.');
            return;
        }

        gridList.push(obj);

        $("#grid").prepend("<tr class='danger _tempRow'><td>" + obj.ItemGroupName + "</td><td>" + obj.ItemName + "</td><td>" + obj.UnitCode + "</td><td>" + obj.HSNCode + "</td><td>" + obj.GIETMCode + "</td><td>" + obj.StockQty + "</td><td>" + obj.DeliveryQty + "</td><td>" + obj.BalancedQty + "</td><td>" + obj.EstimatedValue + "</td> <td><a href='#' id='removeItem' uniqueId='" + obj.Id + "'>Remove</a> &nbsp; &nbsp; <a class='amountDetails' href='javascript:void(0);' data='" + obj.DeliveryQty + "' data1='" + $('#ItemMaster option:selected').val() + "'>Amount Details</a></td></tr>");
        //$("#<tr class='danger _tempRow'><td></td><td></td><td></td><td></td><td></td><td>Total PI Value</td><td></td><td></td><td></td></tr>").insertAfter("#grid")


        $("#txtDeliveryQty").val('');
        $('#txtDeliveryQty').css('border-color', '');

        $("#txtDeliveryRate").val('');
        $('#txtDeliveryRate').css('border-color', '');

        $("#txtEstimatedValue").val('');
        $('#txtEstimatedValue').css('border-color', '');

        $("#txtStockQty").val('');
        $('#txtStockQty').css('border-color', '');





        $("#GIETNCode").val('');
        $('#GIETNCode').css('border-color', '');

        $("#txtBalanceQty").val('');
        $('#txtBalanceQty').css('border-color', '');

        $("#txtDiscountedRate").val('');
        $('#txtDiscountedRate').css('border-color', '');

        $("#GIETNCode").val('');
        $('#GIETNCode').css('border-color', '');

        $("#HSNCode").val('');
        $('#HSNCode').css('border-color', '');


        $("#Unit").val('');
        $('#Unit').css('border-color', '');


        //$("#CTotBalncd").val('');
        //$('#CTotBalncd').css('border-color', '');




    })


    var uniqueId = null;
    getUniqueId = function () {
        if (!uniqueId) uniqueId = (new Date()).getTime();
        return uniqueId++;
    };

    $("body").on("click", "#removeItem", function () {
        var id = $(this).attr("uniqueId");

        for (var index = 0; index < gridList.length; index++) {
            if (gridList[index].Id == id) {

                gridList.splice(index, 1);
                $(this).parents("._tempRow").remove();
            }
        }

    })


    $("#Submit").click(function () {


        if (gridList.length < 1) {
            alert("Add Any Item");
            return;
        }

      



        var Valid = ValidMaster();

        if (Valid === false)
            return;
        var obj = getMaster();


        var session =
            {
                'Child': gridList,
                'Master': obj
            };







        var url = AddIntraTransferDocuments;


        $('#dvLoading').show();
        $.ajax(
            {
                type: "Post",
                url: url,
                data: JSON.stringify(session),
                contentType: "application/json; charset=utf-8",
                dataType: "json", 	//Expected data format from server
                processdata: true, 	//True or False
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (json) {
                    console.log(json);
                     if (json === "3") {
                        alert('Duplicate Purchase Indent can not be created. Kindly Re-load to create indent again.');
                    }
                     else if (json === "4") {
                         alert('Some Administrator Problem. Kindly Re-load Page. ')
                     }
                     else if (json === "-1") {
                         alert("Something went wrong!");
                     }
                     else if (json.status == 0) {
                         alert('Transfer date must be greater than last Transfer Date ' + json.last_tran);
                     }
                     else {
                         //  $("#myModal").modal('show');
                         $("#txtTransferNumber").val(json.TransNo);
                         Moveto();
                         EmpityValue();
                         //child = 0;
                         gridList = [];
                         $("#grid").empty();
                     }
                },
                error: function () {
                    alert('Error in Submit Data');
                }
                // When Service call fails
            })
    })


    $("#txtTransferDate").change(function () {

        $.ajax({
            type: 'POST',
            url: GetTransferCodeOnly, // Calling json method

            dataType: 'json',

            data: { ProjectId: $("#ddlProject option:selected").val(), TransferType: 'OS', TransferDate: $("#txtTransferDate").val() },

            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (obj1) {
                obj1 = $.parseJSON(obj1)
                $("#txtTransferNumber").val(obj1.TransferCode);
                $('#txtLastDate').text(obj1.Last_ISDate);
                $('#txtLastNo').text(obj1.Last_IsNo);

            },
            error: function (ex) {
                alert('Failed to retrieve Project Number.' + ex);
            }
        });



        });





});



$(document).on('keyup', "#txtDeliveryQty", function () {
    debugger;
    var SQt = 0; var DQt = 0; var currVal = 0; var DisRt = 0; var DelivrRate = 0; var BQt = 0;
    SQt = $("#txtStockQty").val() ? $("#txtStockQty").val() : BQt;
    // BQt = $("#txtBalanceQty").val() ? $("#txtBalanceQty").val() : BQt;
    DQt = $("#txtDeliveryQty").val() ? $("#txtDeliveryQty").val() : DQt;
    DisRt = $("#txtDiscountedRate").val() ? $("#txtDiscountedRate").val() : DisRt;
    DelivrRate = $("#txtDeliveryRate").val() ? $("#txtDeliveryRate").val() : DelivrRate;
    if (SQt > 0) {
        if (DQt >= 0) {
            if (+DQt <= +SQt) {
                debugger;
                if (DelivrRate > 0 || DelivrRate == 0) {

                    BQt = parseFloat(parseFloat(SQt) - parseFloat(DQt));
                    currVal = parseFloat(DQt * DelivrRate).toFixed(2);
                    BQt = BQt.toFixed(3);
                    //$("#txtEstimatedValue").val(currVal);
                    $("#txtBalanceQty").val(BQt);
                }
                else {

                    $("#txtEstimatedValue").val('');
                }
                //  currVal = parseFloat(DQt * DelivrRate).toFixed(2);

                //  alert(SQt)
            }
            else {

                alert("Delivery qty. can not be more than stock qty.");
                $("#txtDeliveryQty").val('');
            }

        }

    }

});

$(document).on('keyup', "#txtDeliveryRate", function () {

    var SQt = 0; var DQt = 0; var currVal = 0; var DisRt = 0; var DelivrRate = 0;
    SQt = $("#txtStockQty").val() ? $("#txtStockQty").val() : SQt;
    DQt = $("#txtDeliveryQty").val() ? $("#txtDeliveryQty").val() : DQt;
    DisRt = $("#txtDiscountedRate").val() ? $("#txtDiscountedRate").val() : DisRt;
    DelivrRate = $("#txtDeliveryRate").val() ? $("#txtDeliveryRate").val() : DelivrRate;

    if (SQt > 0) {
        if (DQt > 0) {
            if (+DQt <= +SQt) {

                currVal = parseFloat(DQt * DelivrRate).toFixed(2);
                $("#txtEstimatedValue").val(currVal);

                //  alert(SQt)
            }
            else {

                alert("Delivery qty. can not be more than stock qty.");
                $("#txtDeliveryQty").val('');
                $("#txtEstimatedValue").val('');
            }

        }

    }


});

function Moveto()
{
    var movstr = $("#txtTransferNumber").val().replace("/", "#"); 
    debugger;
    var r = confirm("Do you want to add GST tax to respective items. ?? ");
    if (r == true) {
      window.location.href = "/IntraTransfer/CreateTaxableIntraStateDocument?" + $.param({ IntraTransferNumber: movstr });
    } else {
        //window.open("/Section/Search?" + $.param({ SearchResults: $("#search").val() }));
        window.location.href = geturlred;
    }
    debugger;
}


function getMaster() {

    var obj1 = new Object();

    obj1.IntraTransferNumber = $("#txtTransferNumber").val();
    obj1.ProjectId = $("#ddlProject option:selected").val();
    obj1.TransferProjectId = $("#ddlTransferProject option:selected").val();
    obj1.ModeofTPT = $("#ddlmodeofTPT option:selected").val();
    obj1.VehicleType = $("#ddlVehicleOwnerType option:selected").val();
    obj1.TaxPayableOnReverseCharges = $("#ddlTaxPayableOnReverse option:selected").val();
    obj1.VehicleNo = $("#txtVehicalNo").val();
    obj1.E_WayBillNo = $("#txtGRNo").val();
    obj1.TransferDate = $("#txtTransferDate").val();
    obj1.Remarks = $("#txtRemarks").val();
   
    return obj1;
}


function ValidMaster() {

    var IntraTransferNumber = $("#txtTransferNumber").val();
    var ProjectId = $("#ddlProject option:selected").val();
    var TransferProjectId = $("#ddlTransferProject option:selected").val();
    var ModeofTPT = $("#ddlmodeofTPT option:selected").val();
    var VehicleType = $("#ddlVehicleOwnerType option:selected").val();
    var VehicleNo = $("#txtVehicalNo").val();
    var E_WayBillNo = $("#txtGRNo").val();
    var TransferDate = $("#txtTransferDate").val();
    var Remarks = $("#txtRemarks").val();
    var TaxPayableOnReverseCharges = $("#ddlTaxPayableOnReverse option:selected").val();;



    var rr = true;
    if (IntraTransferNumber === "") {
        $('#txtTransferNumber').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#txtTransferNumber').css('border-color', '');

    }
    if (ProjectId === "0") {
        $('#ddlProject').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlProject').css('border-color', '');

    }

    if (TransferProjectId === "0") {
        $('#ddlTransferProject').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlTransferProject').css('border-color', '');

    }


    if (ModeofTPT === "0") {
        $('#ddlmodeofTPT').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlmodeofTPT').css('border-color', '');

    }

    if (VehicleType === "0") {
        $('#ddlVehicleOwnerType').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlVehicleOwnerType').css('border-color', '');

    }

    if (VehicleNo === "") {
        $('#txtVehicalNo').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#txtVehicalNo').css('border-color', '');
    }

    //if (E_WayBillNo === "") {
    //    $('#txtGRNo').css('border-color', '#f0551b');
    //    rr = false;
    //}
    //else {
    //    $('#txtGRNo').css('border-color', '');
    //}

    if (TransferDate === "") {
        $('#txtTransferDate').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#txtTransferDate').css('border-color', '');
    }

    if (Remarks === "") {
        $('#txtRemarks').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#txtRemarks').css('border-color', '');
    }
    if (TaxPayableOnReverseCharges === "0") {
        $('#ddlTaxPayableOnReverse').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlTaxPayableOnReverse').css('border-color', '');
    }


    return rr;

}

function EmpityValue()
{
     $("#txtTransferNumber").val('');
     $("#ddlProject option:selected").prop("selected", false);
     $("#ddlTransferProject option:selected").prop("selected", false);
     $("#ddlmodeofTPT option:selected").prop("selected", false);
     $("#ddlVehicleOwnerType option:selected").prop("selected", false);
     $("#txtVehicalNo").val('');
     $("#txtGRNo").val('');
     $("#txtTransferDate").val('');
     $("#txtRemarks").val('');
     $("#ddlTaxPayableOnReverse option:selected").prop("selected", false);
}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;

    return true;
}