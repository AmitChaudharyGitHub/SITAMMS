$(document).ready(function () {
    debugger;
    if (transferType == '3') {
        $('#dvLoading').hide();
        $.get('/GetOut/GetDebitNoteDetails', { TransId:tId }, function (data) {
            $("#dvLoading").show();
            $('#formbody').show();
            $('#formbody').html(data);
            $("#dvLoading").hide();
           
          });
    }
    else {
         $.getJSON(GetIntraProjectDetail, { TransferNo: $("#hdnTransferNo").val() }, function (data) {
        data = $.parseJSON(data);
        debugger;
        $.each(data, function (i, item) {
            $('#lbl_project_name').text(item.ProjectAddress);
            $('#lbl_Project_address').text(item.ProjectName);
            $('#lbl_Project_State').text(item.ProjectState);
            $('#lbl_project_GSTIN').text(item.ProjectGSTIN);
            $('#lbl_Transferproject_name').text(item.TransferProjectName);
            $('#lbl_TransferProject_address').text(item.TransferProjectAddress);
            $('#lbl_TransferProject_State').text(item.TransferProjectState);
            $('#lbl_Transferproject_GSTIN').text(item.TarnsferProjectGSTIN);
            $('#lblTransportationMode').text(item.ModeOfTPT);
            $('#lblVehicleType').text(item.VehicleType);
            $('#lblVehicleNumber').text(item.VehicleNo);
            $('#lblEwayBillNo').text(item.EWayBillNo);
            $('#lblTaxpayableReverseCharges').text(item.TaxTapableStatus);
            $('#txtIntraTransferNo').val($("#hdnIntraTransferNo").val());
            $('#hdn_IntraProjectId').val(item.ProjectId);
            $('#hdn_TransferProjectId').val(item.TRansferProjectID);
            $('#hdn_IntraTransferMasterDocumentId').val(item.IntraTransferMasterTransId);
            $('#txtIntraTransferNo').val($('#hdnTransferNo').val());
            $('#lblLastDateofReachDestination').text(item.ReachingDateofDestination);
           // $('#txtGetoutDate').val($('#hdnTransferDate').val());
        });
        debugger;
       
        var k = $('#hdn_IntraProjectId').val();
        //alert($('#hdn_IntraProjectId').val())
        BindGrid($('#hdnTransferNo').val(), $('#hdn_IntraProjectId').val());

        debugger;
        $("#grid th:nth-child(2)").hide();
        $("#grid td:nth-child(2)").hide();

        $("#grid th:nth-child(4)").hide();
        $("#grid td:nth-child(4)").hide();

        $("#grid th:nth-child(6)").hide();
        $("#grid td:nth-child(6)").hide();
       
         });
    }


    // saving record

    $("#Submit").click(function (e) {

        if($('#txtGetOutNo').val()==''){
            alert('Please select Gate out date');
            return false;
        }
        if ($('#txtChalanNumber').val() == '') {
            alert('Please chalan number');
            return false;
        }

        if ($('#txtVehicleNumber').val() == '') {
            alert('Please enter Vehicle No.');
            return false;
        }



        var valid = true;
        if (valid) {
            var json = JSON.parse(gridTojson());
            if (jQuery.isEmptyObject(json)) {
                alert('Project and Indent no should not be empty');
                return false;
            }
            var _griddata = gridTojson();

            var url = Savegetout;
            if (transferType == '3') {
                url='/GetOut/SaveVendorDebitNoteGetOut';
            }
            $.ajax({
                url: url,
                type: 'POST',
                data: { griddata: _griddata }
            }).done(function (data) {
                if (data.Status == 3) {
                    alert(data.Msg);
                    return false;
                }
                if (data != "" && data != "1") {
                    $('#message').html(data);
                    //toastr.options.positionClass = "toast-bottom-right";
                    //toastr.options.closeButton = true;
                    //toastr.options.progressBar = true;
                    //  toastr.success('Issue Quantity Sent Successfully', 'Confirmation');
                    $("#myModal").modal('show');
                    window.location.href = redurl;
                }
                if (data == "1") {
                    //$('#message').html(data);
                    //toastr.options.positionClass = "toast-bottom-right";
                    //toastr.options.closeButton = true;
                    //toastr.options.progressBar = true;
                    //toastr.error('Issue Qty Can not be greater than Balance Qty ', 'Confirmation');
                    // $("#myModal").modal('show');
                } else if (data == "2") {
                    //toastr.options.positionClass = "toast-bottom-right";
                    //toastr.options.closeButton = true;
                    //toastr.options.progressBar = true;
                    //toastr.error('Issue Qty Can not be greater than Aval. Qty ', 'Confirmation');
                }
                else {
                    //toastr.error("Error");
                }
            });
        }
    });


    $(".DatePicker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: false,
        changeYear: false,
        value: "",
        maxDate: new Date()
    });
    $("#txtGetoutDate").change(function () {
        CreateGetoutNo($('#hdn_IntraProjectId').val(), $('#hdnTransferType').val(), $("#txtGetoutDate").val());
    });

    $(document).on('change', '#txtGetoutDate', function () {
        //alert(tDate);
        if (new Date($(this).val()) < new Date(tDate)) {
            alert('Gate out date can not be less than Transfer date');
            $(this).val('');
            return false;
        }
    });

});

function gridTojson() {
    var json = '{';
    var otArr = [];
    debugger;
    var tbl2 = $('#grid tbody tr').each(function (i) {
        if ($(this)[0].rowIndex != 0) {
            x = $(this).children();
            var itArr = [];
            x.each(function () {
                if ($(this).children('input').length > 0) {
                    itArr.push('"' + $(this).children('input').val() + '"');
                }
                else {
                    var x = $(this).text();
                    itArr.push('"' + $.trim(x.replace('"', "").replace('"', '_')) + '"');
                }
            });

            itArr.push('"' + $("#txtGetOutNo").val() + '"');
            itArr.push('"' + $("#hdn_IntraProjectId").val() + '"');
            itArr.push('"' + $("#hdn_TransferProjectId").val() + '"');
            itArr.push('"' + $("#txtIntraTransferNo").val() + '"');
            itArr.push('"' + $("#txtGetoutDate").val() + '"');
            itArr.push('"' + $("#txtChalanNumber").val() + '"');
            itArr.push('"' + $("#txtChalanDate").val() + '"');
            itArr.push('"' + $("#txtVehicleNumber").val() + '"');
            itArr.push('"' + $("#hdnTransferType").val() + '"');
            itArr.push('"' + $("#txtEWayBill").val() + '"');
            if (transferType == '3') {
                itArr.push('"' + tId + '"');
            }
            otArr.push('"' + i + '": [' + itArr.join(',') + ']');
        }
    })
    json += otArr.join(",") + '}'
    //debugger;
    //alert(json)
    return json;
}

function BindGrid(TransferNo, ProjectId)
{
    if ((TransferNo != null || TransferNo != '') && (ProjectId != null || ProjectId != ''))
    {

        $.get(GetGrid,
               { TransferNo: TransferNo, ProjectId: ProjectId },
               function (result) {
                   $('#formbody').show();
                   $('#formbody').html(result);
                   $("#dvLoading").hide();
               });
    }
}

function CreateGetoutNo(ProjectId, GetOutType, GateDate)
{
    $.ajax({
        type: 'POST',
        url: GetOutCodeOnly, // Calling json method

        dataType: 'json',

        data: { ProjectID: ProjectId, GetOutType: GetOutType , GateOutDate: GateDate },

        complete: function () {
            $('#dvLoading').hide();
        },
        success: function (obj1) {
            obj1 = $.parseJSON(obj1)
            $("#txtGetOutNo").val(obj1.getoutCode);


        },
        error: function (ex) {
            alert('Failed to retrieve Project Number.' + ex);
        }
    });

}