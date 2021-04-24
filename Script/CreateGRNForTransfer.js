$(document).ready(function () {

    $(document).on('keyup', '[class^="Receive_"] input', function () {
        //alert($('#Receive_^ input').lenght);
        var transQty = $(this).closest('tr').find('[id^="Qty_"] input').val();
        var gateRecvQty = $(this).closest('tr').find('[id^="GateReceive_"] input').val();
        var itemStoreRecevQty = $(this).closest('tr').find('[id^="PReceive_"] input').attr('placeholder');
        if (parseFloat($(this).val()) > parseFloat(transQty - gateRecvQty)) {
            $(this).val('');
            alert('Receive Qty can not be more than Qty');
            return false;
        }
    });


    $('#Date').val('');

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
        var datetime = "Current Time: "
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


    $.ajax({
        type: 'POST',
        url: GetDetail, // Calling json method

        dataType: 'json',

        data: { TransferNumber: $("#hdn_Transfer_No").val(), TransferType: $("#hdn_TransferType").val() },


        success: function (obj) {
            obj = $.parseJSON(obj)



            $("#txtProjectName").val(obj.ProjName);
            // $("#GateEntryNo").val(obj.GateEntryNo);
            $("#txtStatus").val(obj.PurchaseType);
            $("#txtTransferNo").val(obj.TransferNo);

            $("#ChallanNo").val(obj.Chalano);
            // $("#ChallanDate").val(obj.Chalandt);
            $("#VehicleNo").val(obj.VehicleNumber);
            GetLoadChilddetail(obj.TransferNo, $("#hdn_TransferType").val());
        },
        complete: function () {
            $('#dvLoading').hide();
        },
        error: function (ex) {
            alert('Failed to retrieve Project Code.' + ex);
        }
    });



    $('#btnsave').click(function (e) {
      //  debugger;

        if ($('#ChallanNo').val() == '') {
            alert('Please enter challan no.');
            return false;
        }

        if ($('#VehicleNo').val() == '') {
            alert('Please enter Vehicle no.');
            return false;
        }

        var Valid = ValidMaster();
        if (Valid == false)
            return;

        //if (Valid != false) {
        //    $.get(CheckDuplicate, { BillNo: $('#ChallanNo').val() }, function (response) {
        //        if (response.Status != "1") {
        //            alert("Bill Number/Challan Number already exists.");
        //            return false;
        //        }
        //        else {
        var otArr = [];
        var url = AddM_UpC;
        if ($("#txtStatus").val() == "Intra Transfer (Within State) Purchase") {
          //  debugger;
            var values = INTToArray();
          //  debugger;
            Valid = values[0];
            if (Valid == false)
                return;
            otArr = values[1];

        } else if ($("#txtStatus").val() == "Inter Transfer (Other State) Purchase") {
            var values = INTERToArray();
           // debugger;
            Valid = values[0];
            if (Valid == false)
                return;
            otArr = values[1];

        }

        var obj1 = getMaster();
        var data = {
            'Child': otArr,
            'Master': obj1
        };
        //console.log(data);

        $('#dvLoading').show();
        $.ajax({
            type: 'POST',
            url: url,
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            processdata: true,

            success: function (json) {
                if (json == '2') {
                    alert('Something went wrong!');
                }
                else if (json == '101')
                {
                    alert("GRN date must be less than transfer date.");
                    $('#btnsave').removeAttr('disabled');
                }
                else if (json.Status == "1") {
                    $('#trans_No').text(json.TransNo);
                    $("#myModal").modal('show');
                  //  window.location.href = redirectUrl;
                    //$("#Date").datepicker("setDate", "");
                    $("#Time").prop('selectedIndex', 0);
                    $("#txtProjectName").val('');

                    $("#txtStatus").val('');
                    $("#txtPONumber").val('');

                    $('#formbody').html('');

                    $("#GateEntryNo").val('');
                    $("#ChallanNo").val('');
                    $("#ChallanDate").datepicker("setDate", "");
                    $("#VehicleNo").val('');

                }
            },
            complete: function () {
                $('#dvLoading').hide();
            },




            error: function () {
                alert('Error in Submit Data');
            }
        });
        // }
        //    });
        //}
    });

    $('#closeBtn').click(function () {
        window.location.href = redirectUrl;
    })

    function INTToArray() {
        debugger;
        var otArr = [];
        var ValidChild = false;
        var tbl2 = $('#grid tbody tr').each(function (i) {
            debugger;
            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {
                debugger;
                x = $(this).children();
                obj.ReceiveQty = $(this).children().eq(9).find('input').val();
                if (obj.ReceiveQty != null && obj.ReceiveQty != 0) {
                    ValidChild = true;

                    obj.GateEntryNo = $("#GateEntryNo").val();
                    debugger;
                    //obj.Status = $("#Status option:selected").val();
                    var st = $("#txtStatus").val();
                    if (st == "Intra Transfer (Within State) Purchase") {
                        obj.Status = 'OSP';
                    } else if (st == "Inter Transfer (Other State) Purchase") {
                        obj.Status = 'ISP';
                    }
                    else {
                        obj.Status = null;
                    }

                    obj.StatusTypeId = $("#hdn_transfer_TransId").val();
                    obj.StatusTypeNo = $("#hdn_Transfer_No").val();
                    // UnitNo ItemGroupNo Added from controller
                    obj.SNo = $(this).attr('id');
                    obj.ItemNo = $(this).children().eq(0).attr('it');
                    obj.ItemGroupNo = $(this).children().eq(0).attr('Igd');
                    obj.UnitNo = $(this).children().eq(0).attr('untid');
                    obj.Item = $(this).children().eq(2).text().trim();
                    obj.ItemGroup = $(this).children().eq(1).text().trim();
                    obj.Unit = $(this).children().eq(3).text().trim();

                    obj.StatusChildId = $(this).children().eq(0).attr('ipouid');
                    // obj.Vendor = $("td[class='Ven']").text().trim();
                    //  obj.VendorNo = $("td[class='Ven']").attr('vid');
                    debugger;
                    obj.Rate = $(this).children().eq(0).attr('rate');





                    otArr.push(obj);
                }
            }
        })
        return [ValidChild, otArr];
    }

    function getMaster() {

        debugger;

        var obj1 = new Object();

        obj1.Date = $("#Date").val();
        obj1.Time = $("#Time").val();

        obj1.ProjectNo = $("#hdn_PRJID").val();
        obj1.ProjectName = $("#txtProjectName").val();

        obj1.GateEntryNo = $("#GateEntryNo").val();



        obj1.ChallanNo = $("#ChallanNo").val();
        obj1.ChallanDate = $("#ChallanDate").val();
        obj1.VehicleNo = $("#VehicleNo").val();
        var st = $("#txtStatus").val();
        if (st == "Intra Transfer (Within State) Purchase") {
            obj1.Status = 'OSP';
        } else if (st == "Inter Transfer (Other State) Purchase") {
            obj1.Status = 'ISP';
        }
        else {
            obj1.Status = null;
        }



        obj1.StatusTypeId = $("#hdn_transfer_TransId").val();
        obj1.StatusTypeNo = $("#hdn_Transfer_No").val();


        //  obj1.EmpId = $("#Employee option:selected").val();
        // obj1.EmpName = $("#Employee option:selected").text();



        return obj1;
    }

    function ValidMaster() {
        var Projects = $("#txtProjectName").val();
        var GateEntryNo = $("#GateEntryNo").val();
        var Status = $("#txtStatus").val();
        var StatusTypeNo = $("#hdn_Transfer_No").val();
        var Date = $("#Date").val();
        var rr = true;
        if (Projects == "0") {
            $('#Projects').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#Projects').css('border-color', '');

        }
        if (GateEntryNo == "") {
            $('#GateEntryNo').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#GateEntryNo').css('border-color', '');
        }

        if (Status == "") {
            $('#Status').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#Status').css('border-color', '');
        }

        //if (StatusTypeNo == "") {
        //    $('#StatusTypeNo').css('border-color', '#f0551b');
        //    rr = false;
        //}
        //else {
        //    $('#StatusTypeNo').css('border-color', '');
        //}

        if (Date == "") {
            $('#Date').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#Date').css('border-color', '');
        }

        return rr;

    }

    function INTERToArray() {
        debugger;
        var otArr = [];
        var ValidChild = false;
        var tbl2 = $('#grid tbody tr').each(function (i) {
            debugger;
            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {
                debugger;
                x = $(this).children();
                obj.ReceiveQty = $(this).children().eq(9).find('input').val();
                if (obj.ReceiveQty != null && obj.ReceiveQty != 0) {
                    ValidChild = true;

                    obj.GateEntryNo = $("#GateEntryNo").val();
                    debugger;

                    var st = $("#txtStatus").val();
                    if (st == "Intra Transfer (Within State) Purchase") {
                        obj.Status = 'OSP';
                    } else if (st == "Inter Transfer (Other State) Purchase") {
                        obj.Status = 'ISP';
                    }
                    else {
                        obj.Status = null;
                    }

                    obj.StatusTypeId = $("#hdn_transfer_TransId").val();
                    obj.StatusTypeNo = $("#hdn_Transfer_No").val();
                    // UnitNo ItemGroupNo Added from controller
                    obj.SNo = $(this).attr('id');
                    console.log(obj.SN);
                    obj.ItemNo = $(this).children().eq(0).attr('it');
                    obj.ItemGroupNo = $(this).children().eq(0).attr('Igd');
                    obj.UnitNo = $(this).children().eq(0).attr('untid');
                    obj.Item = $(this).children().eq(2).text().trim();
                    obj.ItemGroup = $(this).children().eq(1).text().trim();
                    obj.Unit = $(this).children().eq(3).text().trim();

                    obj.StatusChildId = $(this).children().eq(0).attr('ipouid');
                    // obj.Vendor = $("td[class='Ven']").text().trim();
                    //  obj.VendorNo = $("td[class='Ven']").attr('vid');
                    debugger;
                    obj.Rate = $(this).children().eq(0).attr('rate');





                    otArr.push(obj);
                }
            }
        })
        return [ValidChild, otArr];
    }







    // Populate GRN On basis of Selected GRN Date.


    $("#Date").change(function () {

        $.ajax({
            type: 'POST',
            url: GetGRNNoDateWise, // Calling json method

            dataType: 'json',

            data: { ProjectNo: $("#hdn_PRJID").val(), GRN_Mid_Date: $("#Date").val() },


            success: function (obj) {
                obj = $.parseJSON(obj)



                $("#GateEntryNo").val(obj.GRN_NO);

            },
            complete: function () {
                // $('#loading').hide();
            },
            error: function (ex) {
                alert('Failed to retrieve Gate Entry Number Number.' + ex);
            }
        });

    });



});


function GetLoadChilddetail(StatusNo, Tpy) {
    $.ajax({
        type: 'GET',
        url: GetGrid,
        data: { TransferNo: StatusNo, Type: Tpy },
        // . $("#txtStatus").val()
        complete: function () {
            $('#dvLoading').hide();
        },
        success: function (result) {

            $('#formbody').html(result);
            // getchallan();
        },
        error: function (ex) {
            alert('Failed to retrieve Project Code.' + ex);
        }
    });
}

