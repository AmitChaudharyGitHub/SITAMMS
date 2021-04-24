$(document).ready(function () {
    alert($("#hdnIntraTransferNo").val());

   

    debugger;
    $.getJSON(GetIntraProjectDetail, { IntraTransferNo: $("#hdnIntraTransferNo").val() }, function (data) {

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
            $('#hdn_IntraTrasferProjectId').val(item.ProjectId);
            $('#hdn_TransferProjectId').val(item.TRansferProjectID);
            $('#hdn_IntraTransferMasterDocumentId').val(item.IntraTransferMasterTransId);

            BindPIC(item.ProjectId);
        });
        // $("#dvLoading").hide(); 

     
        BindrecordThroughIntraTransferNo($('#hdnIntraTransferNo').val());


    });



    Calculation();

    ItemWiseFullCalculation();


    var timer;
    $(function () {
        $('#dateTime').html(getDateTime());
        timer = setTimeout(function () {
            update();
        }, 1000);


    });

    
    $(".DatePicker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: true,
        changeYear: true,
        value: "",
      //  minDate: 0
       // maxDate: new Date()
    });

    // for Save

    $('#btnsave').click(function (e) {
        debugger;
        var V = Valid();

        if (V == false)
            return;
        //debugger;
     //   var _griddata = gridToArray();
        debugger;
        var json = JSON.parse(gridToKuldeepjson());
                if (jQuery.isEmptyObject(json)) {
                    alert('Project and Indent no should not be empty');
                    return false;
                }
                var _griddata = gridToKuldeepjson();
                var _parentGrid = ParentKuldeeptoJson();

                debugger;


                console.log(_griddata);
                var url = testjsonadd;
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: { griddata: _griddata, Prntgrid: _parentGrid }
                }).done(function (data) {
                    debugger;
                    if (data.Status =="1")
                    {
                        alert("Data Saved Successfully");
                       window.location.href = RedUrl;
                        
                    }
                    else if (data.Status == "3") {
                                    alert("Administration Problem. Please Re-load Page.")
                                }
                    else if (data.Status == "4") {
                                    alert("Some Problem Occured. Please Contact Administration.")
                                }
                    else if (data.Status == "2") {
                                    alert("Some exception problem has occured.");
                                }
                    else if (data.Status == "5") {
                                    alert("Some exception problem has occured. Please Re-Load Page.");
                                }
                    else if (data.Status == "6") {
                                    alert("Data is already exist.");
                                }
                    else if (data.Status == "7")
                    {
                        alert("Bindling Problem.Kindly Re-load Page.");
                    }
                    else if (data.Status == "8")
                    {
                        alert("You are missing to press calculate button in some of item. Kindly press Calculate button item wise before final calculate and submit button. ");
                    }
                });


        //console.log(otArr); // 
        //var url = AddIntraTranferTaxable;
        //$.ajax({
        //    type: 'POST',
        //    url: url,
        //    data: JSON.stringify({ Grid: _griddata }),
        //    contentType: "application/json",
        //    dataType: "json",
        //    processdata: true,

        //    success: function (json) {
        //        debugger;

        //        if (json.Status == "1") {
        //            alert("Data Saved Successfully");

        //            url: json.redidtUrl,
        //            window.location.href = RedUrl;



        //        }

        //        else if (json.Status == "3") {
        //            alert("Administration Problem. Please Re-load Page.")
        //        }
        //        else if (json.Status == "4") {
        //            alert("Some Problem Occured. Please Contact Administration.")
        //        }
        //        else if (json.Status == "2") {
        //            alert("Some exception problem has occured.");
        //        }
        //        else if (json.Status == "5") {
        //            alert("Session has Expired. Please Re-login or Re-Load Page.");
        //        }
        //        else if (json.Status == "6") {
        //            alert("Data is already exist.");
        //        }

        //    },







        //    error: function (ex) {
        //        debugger;

        //        var errors = ex.responseJSON;

        //        console.log(errors);


        //        // alert('Failed to retrieve Project Code.' + ex);
        //        alert('Error in Submit Data' + ex);
        //        debugger;
        //    }
        //});
    });

    function gridTojson() {
        var json = '{';
        var otArr = [];
        var tbl2 = $('#grid tbody tr').each(function (i) {
            if ($(this)[0].rowIndex != 0) {
                x = $(this).children();
                var itArr = [];
                x.each(function () {
                    if ($(this).children('input').length > 0) {
                        itArr.push('"' + $(this).children('input').val() + '"');
                    }
                    else {
                        itArr.push('"' + $(this).text() + '"');
                    }
                });
                otArr.push('"' + i + '": [' + itArr.join(',') + ']');
            }
        })
        json += otArr.join(",") + '}'
        return json;
    }

    function gridToKuldeepjson() {
        debugger;
        var json = '{';
        var otArr = [];
        var tbl2 = $('div.single').find('div.second').each(function (i) {
    
            debugger;
            if ($(this)[0].rowIndex != 0) {
                debugger;
                x = $(this).children();
                var jh = jQuery(this).data('dhdn');
                var itArr = [];
                var currQty = $('td.CQty_' + jh).find('input').val();
                if (currQty != null && currQty != 0)
                {
                    var Item_Description = $('td.SrNo_' + jh).find('textarea#IDesc').val();
                    var Qty = $('td.CQty_' + jh).find('input').val();
                    var Rate = $('td.CRate_' + jh).find('#hdndiscountedRate_' + jh).val();
                    var NewRate = $('td.CRate_' + jh).find('input').val();
                    var Discount = $('td.CDiscount_' + jh).find('input').val();

                    var PackingChargesPercentage = $('td.PackingchargePercentage_' + jh).find('input').val();
                    var PackingChargesAmount = $('td.PackingchargePertageAmt_' + jh).find('#txtpackingChargePercentageAmount_' + jh).val();

                    var CartageTypeId = $('td.CartageType_' + jh).find('#CartageTypeddl_' + jh).find(':selected').val();
                    var Cartage_rate = $('td.CartagerateValue_' + jh).find('#CartageRate1_' + jh).val();
                    var CartageAmount = $('td.Cartageamt_' + jh).find('input').val();


                    var InsuranceRate = $('td.InsuranceRatePercentage_' + jh).find('#txtInsuranceRatePercentage_' + jh).val();
                    var InsuranceAmount = $('td.InsurancePercentageAmt_' + jh).find('#txtInsurancePercentageAmt_' + jh).val();

                    var TaxCode = $('td.GSTSlab_' + jh).find('#ddlGSTSlabs_' + jh).find(':selected').val();
                    var TaxRateType = $('td.GSTSlab_' + jh).find('#ddlGSTSlabs_' + jh).find(':selected').text();


                    var CGSTPercentage = $('td.CGSTPer_' + jh).find('#txtCGSTPerctg_' + jh).val();
                    var CGSTAmount = $('td.CGSTPerAmtValue_' + jh).find('#txtCGSTPerAmtValue_' + jh).val();


                    var SGSTPercentage = $('td.SGSTOrUTGSTPer_' + jh).find('#txtSGSTOrUTGSTPerctg_' + jh).val();
                    var SGSTAmount = $('td.SGSTOrUTGSTPerAmtValue_' + jh).find('#txtSGSTOrUTGSTPerAmtValue_' + jh).val();

                    var IGSTPercentage = $('td.IGSTPer_' + jh).find('#txtIGSTPerctg_' + jh).val();
                    var IGSTAmount = $('td.IGSTPerAmtValue_' + jh).find('#txtIGSTPerAmtValue_' + jh).val();

                    var TotalGSTAmount = $('td.TotalGSTAmtValue_' + jh).find('#txtTotalGSTAmtValue_' + jh).val();
                    var GrossAmount = $('td.TotalGrossAmtValue_' + jh).find('#txtTotalGrossAmtValue_' + jh).val();

                    var ItemId = $('#hdn_itemnoto_' + jh).val();
                    var IntraTRansferChildId = $('#hdn_indent_uid_' + jh).val();

                    var Total = $('#Ctotal_amount_' + jh).val();
                    var SubTotal1 = $('#total_SubTotal_Amount1_' + jh).val();
                    var SubTotal2 = $('#total_SubTotal_Amount2_' + jh).val();

                    itArr.push('"' +Item_Description+ '"');
                    itArr.push('"' +Qty+ '"');
                    itArr.push('"' +Rate+ '"');
                    itArr.push('"' +NewRate+ '"');
                    itArr.push('"' +Discount+ '"');
                    itArr.push('"' +PackingChargesPercentage+ '"');
                    itArr.push('"' +PackingChargesAmount+ '"');
                    itArr.push('"' +CartageTypeId+ '"');
                    itArr.push('"' +Cartage_rate+ '"');
                    itArr.push('"' +CartageAmount+ '"');
                    itArr.push('"' +InsuranceRate+ '"');
                    itArr.push('"' +InsuranceAmount+'"');
                    itArr.push('"' +TaxCode +'"');
                    itArr.push('"' +TaxRateType +'"');
                    itArr.push('"' +CGSTPercentage+ '"');
                    itArr.push('"' +CGSTAmount+ '"');
                    itArr.push('"' +SGSTPercentage+ '"');
                    itArr.push('"' +SGSTAmount+ '"');
                    itArr.push('"' +IGSTPercentage+ '"');
                    itArr.push('"' +IGSTAmount+ '"');
                    itArr.push('"' +TotalGSTAmount+ '"');
                    itArr.push('"' +GrossAmount+ '"');
                    itArr.push('"' +ItemId+ '"');
                    itArr.push('"' +IntraTRansferChildId+ '"');
                    itArr.push('"' +Total+ '"');
                    itArr.push('"' +SubTotal1+ '"');
                    itArr.push('"' +SubTotal2+ '"');
                   

                }

                debugger;
               
                otArr.push('"' + i + '": [' + itArr.join(',') + ']');
            }
        })
        json += otArr.join(",") + '}'
        return json;
    }

    function ParentKuldeeptoJson()
    {
        var MasterData ="MasterData";
        var json = '{';
        var otP = [];
        var itArP = [];

        itArP.push('"' + $("#txtDate").val() + '"');
        itArP.push('"' + $("#txtIntraTransferNo").val() + '"');
        itArP.push('"' + $("#hdn_IntraTrasferProjectId").val() + '"');
        itArP.push('"' + $("#hdn_TransferProjectId").val() + '"');
        itArP.push('"' + $("#hdn_IntraTransferMasterDocumentId").val() + '"');
        itArP.push('"' + $("#SumOfTotalVal").val() + '"');
        itArP.push('"' + $("#SumOfSubTotalAll1Amt").val() + '"');
        itArP.push('"' + $("#SumOf_total_Sub_2").val() + '"');
        itArP.push('"' + $("#SumOfAll_Gross_Total_amt").val() + '"');
        itArP.push('"' + $("#Employee option:selected").val() + '"');
        itArP.push('"' + $("#Employee option:selected").val() + '"');
        itArP.push('"' + $("#PORemarks").val() + '"');
        itArP.push('"' + $("#SumOfPackingCharge").val() + '"');
        itArP.push('"' + $("#SumOfCartageAll1Amt").val() + '"');
        itArP.push('"' + $("#SumOfInsuranceAmt").val() + '"');
        itArP.push('"' + $("#SumOfAll_GST_amt").val() + '"');
        itArP.push('"' + $("#Total_SumOf_CGST").val() + '"');
        itArP.push('"' + $("#SumOfSGSTAndUTGST_Amt").val() + '"');
        itArP.push('"' + $("#SumOf_IGST_Amt").val() + '"');
        itArP.push('"' + $("#Rupees").val() + '"');
        itArP.push('"' + $("#txtdateofdestination").val() + '"');
        otP.push('"' + MasterData + '": [' + itArP.join(',') + ']');
        json += otP.join(",") + '}'
        return json;
    }

    function gridToArray() {

        var otArr = [];


        var tbl2 = $('div.single').find('div.second').each(function (i) {
            debugger;
            var obj = new Object();

            var jh = jQuery(this).data('dhdn');
            var currQty = $('td.CQty_' + jh).find('input').val();


            if (currQty != null && currQty != 0) {
                debugger;
                obj.Item_Description = $('td.SrNo_' + jh).find('textarea#IDesc').val();
                obj.Qty = $('td.CQty_' + jh).find('input').val();

                //Two Type of rate. 1. Normal user entere rate 2. discounted rate.
                //normal rate
                //obj.Rate = $('td.CRate_' + jh).find('input').val();
                obj.Rate = $('td.CRate_' + jh).find('#hdndiscountedRate_' + jh).val();
                // discounted rate 
                obj.NewRate = $('td.CRate_' + jh).find('input').val();


                obj.Discount = $('td.CDiscount_' + jh).find('input').val();

                obj.PackingChargesPercentage = $('td.PackingchargePercentage_' + jh).find('input').val();
                obj.PackingChargesAmount = $('td.PackingchargePertageAmt_' + jh).find('#txtpackingChargePercentageAmount_' + jh).val();

                obj.CartageTypeId = $('td.CartageType_' + jh).find('#CartageTypeddl_' + jh).find(':selected').val();
                obj.Cartage_rate = $('td.CartagerateValue_' + jh).find('#CartageRate1_' + jh).val();
                obj.CartageAmount = $('td.Cartageamt_' + jh).find('input').val();


                obj.InsuranceRate = $('td.InsuranceRatePercentage_' + jh).find('#txtInsuranceRatePercentage_' + jh).val();
                obj.InsuranceAmount = $('td.InsurancePercentageAmt_' + jh).find('#txtInsurancePercentageAmt_' + jh).val();

                obj.TaxCode = $('td.GSTSlab_' + jh).find('#ddlGSTSlabs_' + jh).find(':selected').val();
                obj.TaxRateType = $('td.GSTSlab_' + jh).find('#ddlGSTSlabs_' + jh).find(':selected').text();


                obj.CGSTPercentage = $('td.CGSTPer_' + jh).find('#txtCGSTPerctg_' + jh).val();
                obj.CGSTAmount = $('td.CGSTPerAmtValue_' + jh).find('#txtCGSTPerAmtValue_' + jh).val();


                obj.SGSTPercentage = $('td.SGSTOrUTGSTPer_' + jh).find('#txtSGSTOrUTGSTPerctg_' + jh).val();
                obj.SGSTAmount = $('td.SGSTOrUTGSTPerAmtValue_' + jh).find('#txtSGSTOrUTGSTPerAmtValue_' + jh).val();

                obj.IGSTPercentage = $('td.IGSTPer_' + jh).find('#txtIGSTPerctg_' + jh).val();
                obj.IGSTAmount = $('td.IGSTPerAmtValue_' + jh).find('#txtIGSTPerAmtValue_' + jh).val();

                obj.TotalGSTAmount = $('td.TotalGSTAmtValue_' + jh).find('#txtTotalGSTAmtValue_' + jh).val();
                obj.GrossAmount = $('td.TotalGrossAmtValue_' + jh).find('#txtTotalGrossAmtValue_' + jh).val();

                obj.ItemId = $('#hdn_itemnoto_' + jh).val();
                obj.IntraTRansferChildId = $('#hdn_indent_uid_' + jh).val();
               // obj.RemOrApprovQty = $('#hdn_remaing_qty_' + jh).val();
              //  obj.PIQty = $('#hdn_item_approv_qty_' + jh).val();
                obj.Total = $('#Ctotal_amount_' + jh).val();
                obj.SubTotal1 = $('#total_SubTotal_Amount1_' + jh).val();
                obj.SubTotal2 = $('#total_SubTotal_Amount2_' + jh).val();
             
                // obj.Item_GrandTotal = $('#total_GrandTotal_Amount_' + jh).val();



                otArr.push(obj)
                // oatFirstBlock.push(obj);
            }



        });

        debugger;
        var obj1 = new Object();

        obj1.TransferDate = $("#txtDate").val();
        obj1.IntraTransferNumber = $("#txtIntraTransferNo").val();

        obj1.ProjectId = $("#hdn_IntraTrasferProjectId").val();
        obj1.TransferProjectId = $("#hdn_TransferProjectId").val();
        obj1.IntraTransferMasterDocumentId = $("#hdn_IntraTransferMasterDocumentId").val();


        obj1.Total = $("#SumOfTotalVal").val();
     
        obj1.SubTotal1 = $("#SumOfSubTotalAll1Amt").val();
        obj1.SubTotal2 = $("#SumOf_total_Sub_2").val();

        
        obj1.GrandTotal = $("#SumOfAll_Gross_Total_amt").val();
        obj1.SendTo = $("#Employee option:selected").val();
        obj1.PICApprovalId = $("#Employee option:selected").val();
        


        debugger;

       
        obj1.Remarks = $("#PORemarks").val();
        obj1.Total_PAndF = $("#SumOfPackingCharge").val();
        obj1.Total_Cartage = $("#SumOfCartageAll1Amt").val();
        obj1.Total_Insurance = $("#SumOfInsuranceAmt").val();
        obj1.Total_Taxable = $("#SumOfAll_GST_amt").val();
        obj1.Total_CGST = $("#Total_SumOf_CGST").val();
        obj1.Total_SGSTAndUTGST = $("#SumOfSGSTAndUTGST_Amt").val();
        obj1.Total_IGST = $("#SumOf_IGST_Amt").val();
        obj1.Total_NetAmountInWords = $("#Rupees").val();
        obj1.ReachingDateofDestination = $("#txtdateofdestination").val();



        

        


       

       



        var session = {
            'Child': otArr,
            'Master': obj1
        };
        return session;
    }

    function Valid() {

        var TransferDate = $("#txtDate").val();
        var IntraTransferNumber = $("#txtIntraTransferNo").val();

        var ProjectId = $("#hdn_IntraTrasferProjectId").val();
        var TransferProjectId = $("#hdn_TransferProjectId").val();
       


        var Total = $("#SumOfTotalVal").val();
     
       var SubTotal1 = $("#SumOfSubTotalAll1Amt").val();
        var SubTotal2 = $("#SumOf_total_Sub_2").val();

        
        var GrandTotal = $("#SumOfAll_Gross_Total_amt").val();
       var SendTo = $("#Employee option:selected").val();
        var PICApprovalId = $("#Employee option:selected").val();
        
        var Total_NetAmountInWords = $("#Rupees").val();

        debugger;

       
        var Remarks = $("#PORemarks").val();
        var Total_PAndF = $("#SumOfPackingCharge").val();

        var ReachingDateofDestination = $("#txtdateofdestination").val();
       

        var rr = true;

        if (TransferDate == "") {
            alert('Transfer Date field is empty');
            $('#txtDate').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#txtDate').css('border-color', '');
        }

        if (ReachingDateofDestination == "") {
            alert('Reach of Destination Date field is empty');
            $('#txtdateofdestination').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#txtdateofdestination').css('border-color', '');
        }


        if (IntraTransferNumber == "") {
            alert('Transfer Number field is empty');
            $('#txtIntraTransferNo').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#txtIntraTransferNo').css('border-color', '');

        }

        if (ProjectId == "") {
            alert('Project Id is missing');
          

            rr = false;
        }
        if (TransferProjectId == "") {
            alert('Transfer Project Id is missing');


            rr = false;
        }
        
         if (Remarks == "") {
            alert('Remarks can not be Empty.');
            $('#PORemarks').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#PORemarks').css('border-color', '');
        }

       

        if (Total_NetAmountInWords == "") {
            alert('Amount in Word field is empty');
            $('#Rupees').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Rupees').css('border-color', '');
        }


        if (SendTo == "0") {
            alert('Select Approval Person.');
            $('#Employee').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Employee').css('border-color', '');
        }

        if (Total_PAndF == "") {
            alert('Total P&F field is empty');
            $('#SumOfPackingCharge').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#SumOfPackingCharge').css('border-color', '');
        }

        if (Total_PAndF == "") {
            alert('Total P&F field is empty');
            $('#SumOfPackingCharge').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#SumOfPackingCharge').css('border-color', '');
        }






        if (rr == false) {


            return false;
        }
        else {

            return true;
        }
    }


   


});

function BindPIC(ProjId)
{
    $.ajax({
        type: 'POST',
        url: GetBIndPIC, // Calling json method

        dataType: 'json',

        data: { projectId: ProjId },

        complete: function () {
            $('#dvLoading').hide();
        },
        success: function (obj) {
            obj = $.parseJSON(obj)

            GetEmp1(obj.Lst);

        },
        error: function (ex) {
            alert('Failed to retrieve Vendor Code.' + ex);
        }
    });
}
function GetEmp1(data) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#Employee").html(procemessage).show();
    var markup = "<option value='0'>Forward For Approval</option>";
    for (var x = 0; x < data.length; x++) {


        //console.log(data[x].Value);

        markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

    }
    $("#Employee").html(markup).show();

}

function BindrecordThroughIntraTransferNo(xret) {
    if (xret != null) {
        $('#dvLoading').show();

        $.ajax({
            type: 'GET',
            url: GetGrids,
            data: { IntraTransferNo: xret },
            
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {

                $('#formbody').html(result);
               // ClearValues();
            },
            error: function (ex) {
                alert('Failed to retrieve Project Code.' + ex);
            }
        });
        //return false;
    }
    else { $('#formbody').html(''); }
}


function Calculation()
{
    $(document).on('keyup', "td[class^='CDiscount_']", function () {

        var Disc = 0; var temp1 = 0; var Temp2 = 0; var Tolt = 0;
        var Disc = $(this).find('input').val();
        var str = $(this).attr('class');
        var a = str.substring(9);

        var Rate = $(".CRate" + a).find('input').val();
        var Qty = $(".CQty" + a).find('input').val();

        if (Disc != null) {

            if (Rate != null && Qty != null)
            { temp1 = Qty * Rate; }
            if (temp1 != null && Disc != null)
            { Temp2 = temp1 * (Disc / 100); }
            if (temp1 != null && Temp2 != null)
            { Tolt = (temp1 - Temp2); }
            if (Tolt != null) {


                $("#Ctotal_amount" + a).val(Tolt.toFixed(3));

            }
        }

    });


    $(document).on('keyup', "td[class^='PackingchargePercentage_']", function () {
        debugger;
        var Disc = 0; var temp1 = 0; var Temp2 = 0; var Tolt = 0; var PandFPercentg = 0; var PandFPercentgAmt = 0;
        var PandFPercentg = $(this).find('input').val();
        var str = $(this).attr('class');
        var a = str.substring(23);
        var Rate = $(".CRate" + a).find('input').val();
        var Qty = $(".CQty" + a).find('input').val();
        var Disc = $(".CDiscount" + a).find('input').val();

        if (Disc != null) {

            if (Rate != null && Qty != null)
            { temp1 = Qty * Rate; }
            if (temp1 != null && Disc != null)
            { Temp2 = temp1 * (Disc / 100); }
            if (temp1 != null && Temp2 != null)
            { Tolt = (temp1 - Temp2); }
        }

        if (PandFPercentg != null) {
            debugger;
            PandFPercentgAmt = Tolt * (PandFPercentg / 100);
        }

        if (PandFPercentgAmt != null) {
            debugger;
            $("#txtpackingChargePercentageAmount" + a).val(PandFPercentgAmt.toFixed(3));
        }

        debugger;

    });

    $(document).on('change', '.car1-type1', function () {
        debugger; var PutZeroVal = 0;
        var Ctg_ID = jQuery(this).data('cartage1');

        if ($(this).val() == 4) {

            $(".CartagerateValue_" + Ctg_ID).find('input').prop('disabled', false);


            var qty = $(".CQty_" + Ctg_ID).find('input').val();
            var Cartagw_rate = $(".CartagerateValue_" + Ctg_ID).find('input').val();

            if (qty != null && Cartagw_rate != null)
            { Cl(); }

        }
        else if ($(this).val() == 3) {


            $(".CartagerateValue_" + Ctg_ID).find('input').prop('disabled', true);

            $(".caramt_" + Ctg_ID).prop('disabled', false); C1_lum();
        }

        else if ($(this).val() == "-1") {
            $(".CartagerateValue_" + Ctg_ID).find('input').val('');
            $(".Cartageamt_" + Ctg_ID).find('input').val('');
        }
        else if ($(this).val() == 2 || $(this).val() == 1 || $(this).val() == 5) {
            $(".CartagerateValue_" + Ctg_ID).find('input').prop('disabled', true);
            $('td.Cartageamt_' + Ctg_ID).find('input').val(PutZeroVal); C1_lumAother();
        }



        


    });

    $(document).on('keyup', "td[class^='InsuranceRatePercentage_']", function () {
        debugger;
        var C_Cartg = 0; var EPack = 0; var DTot = 0; var FSub1 = 0; var SUm = 0; var Insi_percent = 0; var Insi_Tot = 0; var insi_Sum = 0; var Sub_tot_2 = 0;
     
        var str = $(this).attr('class');
        var insi_id = str.substring(23);
        Insi_percent = $(this).find('input').val();

        DTot = $("#Ctotal_amount" + insi_id).val(); if (isNaN(DTot) || DTot == '') { DTot = 0; } else { DTot = DTot; }
        EPack = $(".PackingchargePertageAmt" + insi_id).find('input').val(); if (isNaN(EPack) || EPack == '') { EPack = 0; } else { EPack = EPack; }
        C_Cartg = $(".Cartageamt" + insi_id).find('input').val(); if (isNaN(C_Cartg) || C_Cartg == '') { C_Cartg = 0; } else { C_Cartg = C_Cartg; }

        SUm = parseFloat(DTot) + parseFloat(EPack) + parseFloat(C_Cartg);
        debugger;
        if (SUm != null) {
            Insi_Tot = SUm * (Insi_percent / 100);
        }

        if (Insi_Tot != null) {
            debugger;
            $("#txtInsurancePercentageAmt" + insi_id).val(Insi_Tot.toFixed(3));
        }


        insi_Sum = $(".InsurancePercentageAmt" + insi_id).find('input').val(); if (isNaN(insi_Sum) || insi_Sum == '') { insi_Sum = 0; } else { insi_Sum = insi_Sum; }

        if (insi_Sum != null) {
            Sub_tot_2 = parseFloat(SUm) + parseFloat(insi_Sum);
            $("#total_SubTotal_Amount2" + insi_id).val('');
            $("#total_SubTotal_Amount2" + insi_id).val(Sub_tot_2.toFixed(3));
        }

    });

    $(document).on('change', '.gst-slab-ddl', function () {
        debugger;
        var gst_rowid = jQuery(this).data('gstval');
        var GST_code = $(this).val();
        var SubTot2 = $("#total_SubTotal_Amount2_" + gst_rowid).val();
        

        $.ajax({
            type: 'POST',
            url: BindGSTOnGRidItem,

            dataType: 'json',

            data: { TaxCode: GST_code },


            success: function (obj) {
                obj = $.parseJSON(obj)

              
                $("#txtCGSTPerctg_" + gst_rowid).val(obj.tax_CGST);
                $("#txtSGSTOrUTGSTPerctg_" + gst_rowid).val(obj.tax_SGSTandUTGST);
                $("#txtIGSTPerctg_" + gst_rowid).val(obj.tax_IGST);
                CalculateAllGSTAmount(obj.tax_CGST, obj.tax_SGSTandUTGST, obj.tax_IGST, gst_rowid, SubTot2);


               

            },
            error: function (ex) {
                alert('Failed to retrieve GST Code.' + ex);
            }
        });
        return false;



    });

}




function Cl() {
    var CartageTotal = 0; var qty = 0;

    $(document).on('keyup', '.cartage1-Inputrate', function () {

        var d = jQuery(this).data('itval');
        var j = $(this).val();
        var qty = $(".CQty_" + d).find('input').val();

        CartageTotal = qty * j;
        $(".Cartageamt_" + d).find('input').val(CartageTotal.toFixed(2))

        F2(d, j)

    });
}

function F2(Idd, cartage_rate) {
    debugger;
    var E = 0; var D = 0; var G = 0; var H = 0; var Tolt = 0;
    E = $(".PackingchargePertageAmt_" + Idd).find('input').val(); if (isNaN(E) || E == '') { E = 0; } else { E = E; }
    D = $("#Ctotal_amount" + '_' + Idd).val();

    var sum = parseFloat(E) + parseFloat(D);


    if (cartage_rate == 'lupsum') {
        H = $(".Cartageamt_" + Idd).find('input').val();
    }
    else if (cartage_rate == 'F.O.R. Site') {
        H = $(".Cartageamt_" + Idd).find('input').val();
    } else if (cartage_rate == 'Cash Paid at site') {
        H = $(".Cartageamt_" + Idd).find('input').val();
    }

    else {
        var qty = $(".CQty_" + Idd).find('input').val();
        H = qty * cartage_rate;
    }


    Tolt = parseFloat(D) + parseFloat(E) + parseFloat(H);


    $("#total_SubTotal_Amount1_" + Idd).val('');
    $("#total_SubTotal_Amount1_" + Idd).val(Tolt.toFixed(2));
}

function C1_lum() {
    $(document).on('keyup', '.car1amttkval', function () {
        var idss = jQuery(this).data('cart1amtvalsum');

        F2(idss, 'lupsum');
    });
}

function C1_lumAother() {
    $(document).on('keyup', '.car1amttkval', function () {

        var idss = jQuery(this).data('cart1amtvalsum');

        var chktxt = $('td.CartageType_' + idss).find('#CartageTypeddl_' + idss).find(':selected').text();

        F2(idss, chktxt);
    });
}

function CalculateAllGSTAmount(CGST_val, SGSTAndUTGST_val, UGST_val, ID, SubTotal2Val) {
    var CG_Rate = 0; var SGandUTG_Rate = 0; var UT_Rate = 0; var CG_SUM = 0; var SUM_SGAndUTG = 0; var UT_Sum = 0
    var TOTAL_GST_SUM = 0; var TotalGROSS_Amount = 0;

    if (CGST_val != null) {
        CG_Rate = CGST_val;
        CG_SUM = SubTotal2Val * (CG_Rate / 100);

        $("#txtCGSTPerAmtValue_" + ID).val('');
        $("#txtCGSTPerAmtValue_" + ID).val(CG_SUM.toFixed(3));
    }
    if (SGSTAndUTGST_val != null) {
        SGandUTG_Rate = SGSTAndUTGST_val;
        SUM_SGAndUTG = SubTotal2Val * (SGandUTG_Rate / 100);

        $("#txtSGSTOrUTGSTPerAmtValue_" + ID).val('');
        $("#txtSGSTOrUTGSTPerAmtValue_" + ID).val(SUM_SGAndUTG.toFixed(3));
    }
    if (UGST_val != null) {
        UT_Rate = UGST_val;
        UT_Sum = SubTotal2Val * (UT_Rate / 100);

        $("#txtIGSTPerAmtValue_" + ID).val('');
        $("#txtIGSTPerAmtValue_" + ID).val(UT_Sum.toFixed(3));
    }
    TOTAL_GST_SUM = parseFloat(CG_SUM) + parseFloat(SUM_SGAndUTG) + parseFloat(UT_Sum);

    if (TOTAL_GST_SUM != null) {
        $("#txtTotalGSTAmtValue_" + ID).val('');
        $("#txtTotalGSTAmtValue_" + ID).val(TOTAL_GST_SUM.toFixed(3));
    }

    TotalGROSS_Amount = parseFloat(TOTAL_GST_SUM) + parseFloat(SubTotal2Val);
    if (TotalGROSS_Amount != null) {
        $("#txtTotalGrossAmtValue_" + ID).val('');
        $("#txtTotalGrossAmtValue_" + ID).val(TotalGROSS_Amount.toFixed(3));
    }


}



function ItemWiseFullCalculation() {

    $(document).on('click', '.itemwise_cal', function () {
        debugger;
        var find_id = jQuery(this).data('btnautoidbind');
        var escise = $('td.Exciseduty_' + find_id).find('#ExciseDutyType_' + find_id).find(':selected').val();

        var cartage1 = $('td.CartageType_' + find_id).find('#CartageTypeddl_' + find_id).find(':selected').text();
        if (cartage1 == '' || cartage1 == 'Select') {
            $('td.Cartageamt_' + find_id).find('input').val(0);
        }





        var packingcharge = $('td.PackingchargePercentage_' + find_id).find('input').val();
        if (isNaN(packingcharge) || packingcharge == '') {
            $('td.PackingchargePertageAmt_' + find_id).find('input').val(0)
        }



        Setof_ItemWiseFullCalculation(find_id);




    });
}


function Setof_ItemWiseFullCalculation(Id) {

    var Packge = 0; var Total = 0; var exciseVal = 0; var cartage1 = 0; var Tolt = 0;
    var insiamt = 0; var tatxamt = 0; var Cart2amt = 0; var Insi2amt = 0; var Insi3amt = 0; var Insi4amt = 0; var Subtotal2 = 0; var grnadtotl = 0;
    var A = 0; var B = 0; var C = 0; var D = 0; var E = 0; var F = 0; var I = 0; var J_i1 = 0; var K = 0; var H = 0; var J = 0;
    var L = 0; var M = 0; var N = 0; var O = 0; var P = 0; var Q = 0; var R = 0; var S = 0; var T = 0; var U = 0; var V = 0; var W = 0;
    var DiscountedRate = 0;
    debugger;




    A = $("td.CQty_" + Id).find('input').val(); if (isNaN(A) || A == '') { A = 0; } else { A = A; }

    B = $(".CRate_" + Id).find('input').val(); if (isNaN(B) || B == '') { B = 0; } else { B = B; }
    C = $(".CDiscount_" + Id).find('input').val(); if (isNaN(C) || C == '') { C = 0; } else { C = C; }

    D = parseFloat(A * B) - parseFloat(parseFloat(A * B) * (C / 100)); if (isNaN(D) || D == '') { D = 0; } else { D = D; }
    $("#Ctotal_amount_" + Id).val('');
    $("#Ctotal_amount_" + Id).val(D.toFixed(3));

    // Here, To calculate Discounted Rate. Note : It has must be noted that In database On the Rate column , now discounted rate value will gone
    // insteed of previously rate value. And the The entered rate has gone to new rate column and in previous rate column of database, then new
    // discounted rate will has to gone.

    DiscountedRate = parseFloat((D / A)); if (isNaN(DiscountedRate) || DiscountedRate == '') { DiscountedRate = 0; } else { DiscountedRate = DiscountedRate; }
    alert(DiscountedRate)
    $('td.CRate_' + Id).find('#hdndiscountedRate_' + Id).val(DiscountedRate.toFixed(3));




    x = $('td.PackingchargePercentage_' + Id).find('input').val(); if (isNaN(x) || x == '') { x = 0; } else { x = x; }
    E = parseFloat((x / 100) * D);
    $('td.PackingchargePertageAmt_' + Id).find('#txtpackingChargePercentageAmount_' + Id).val(E.toFixed(3));

    F = $(".Cartageamt_" + Id).find('input').val(); if (isNaN(F) || F == '') { F = 0; } else { F = F; }
    G = parseFloat(D) + parseFloat(E) + parseFloat(F);
    $("#total_SubTotal_Amount1_" + Id).val('');
    $("#total_SubTotal_Amount1_" + Id).val(G.toFixed(2));

    H = $('td.InsuranceRatePercentage_' + Id).find('#txtInsuranceRatePercentage_' + Id).val(); if (isNaN(H) || H == '') { H = 0; } else { H = H; }
    I = parseFloat(G * (H / 100));
    $('td.InsurancePercentageAmt_' + Id).find('#txtInsurancePercentageAmt_' + Id).val(I.toFixed(3));
    J = parseFloat(G) + parseFloat(I);
    $("#total_SubTotal_Amount2_" + Id).val('');
    $("#total_SubTotal_Amount2_" + Id).val(J.toFixed(2));
    K = $('td.CGSTPer_' + Id).find('#txtCGSTPerctg_' + Id).val(); if (isNaN(K) || K == '') { K = 0; } else { K = K; }
    M = $('td.SGSTOrUTGSTPer_' + Id).find('#txtSGSTOrUTGSTPerctg_' + Id).val(); if (isNaN(M) || M == '') { M = 0; } else { M = M; }
    O = $('td.IGSTPer_' + Id).find('#txtIGSTPerctg_' + Id).val(); if (isNaN(O) || O == '') { O = 0; } else { O = O; }
    L = parseFloat((K / 100) * J);
    $('td.CGSTPerAmtValue_' + Id).find('#txtCGSTPerAmtValue_' + Id).val(L.toFixed(3));
    N = parseFloat((M / 100) * J);
    $('td.SGSTOrUTGSTPerAmtValue_' + Id).find('#txtSGSTOrUTGSTPerAmtValue_' + Id).val(N.toFixed(3));
    P = parseFloat((O / 100) * J);
    $('td.IGSTPerAmtValue_' + Id).find('#txtIGSTPerAmtValue_' + Id).val(P.toFixed(3));
    Q = parseFloat(L) + parseFloat(N) + parseFloat(P);
    $('td.TotalGSTAmtValue_' + Id).find('#txtTotalGSTAmtValue_' + Id).val(Q.toFixed(3));
    R = parseFloat(Q) + parseFloat(J);
    $('td.TotalGrossAmtValue_' + Id).find('#txtTotalGrossAmtValue_' + Id).val(R.toFixed(3));
    $("#total_GrandTotal_Amount_" + Id).val('');
    $("#total_GrandTotal_Amount_" + Id).val(R.toFixed(2));

  
}

function RateSum() {
    debugger;
    var SumRt = 0; SumTotal = 0; SumTotpackingCharge = 0; SumTotCart1 = 0;
    var SumTotal1 = 0; SumOfInsurance = 0; SumSubTott2Sum = 0; SUMOfCGST = 0; SumOfSGSTAndUTGST = 0;
    SumOfIGST = 0; SumOfTotalGST = 0; SumofGrossAmount = 0;

    $('div.single').find('div.second').each(function (i) {
        var Id = jQuery(this).data('dhdn');
        var currentQty = $('td.CQty_' + Id).find('input').val();
        var MyCurrRate = $('td.CRate_' + Id).find('input').val();
        var MyGSTselction = $('td.GSTSlab_' + Id).find('#ddlGSTSlabs_' + Id).val();
        var MyCartagesel = $('td.CartageType_' + Id).find('#CartageTypeddl_' + Id).find(':selected').text();
      
        if (currentQty == "" || currentQty == null) {
            alert('Qty. can not left empty.'); false;
        } else if (MyCurrRate == "" || MyCurrRate == null) {
            alert('Rate. can not left empty.'); false;
        }
        else if (MyCartagesel == '' || MyCartagesel == "" || MyCartagesel == "Select") {
            alert('Cartage must be selected.'); false;
        }
        else if (MyGSTselction == '' || MyGSTselction == "") {
            alert('GST must be selected.'); false;
        }



        if (currentQty != null && currentQty != 0) {
            debugger;
            var d = 0; Dtotal = 0, dPacktot = 0, dcartg1tot = 0;
            d = $('td.CRate_' + Id).find('input').val();
            SumRt = parseFloat(SumRt) + parseFloat(d);
            Dtotal = $('#Ctotal_amount_' + Id).val();

            dPacktot = $('td.PackingchargePertageAmt_' + Id).find('input').val();

            if (!isNaN(dPacktot) && dPacktot != "") {
                dPacktot = dPacktot;
            } else {
                dPacktot = 0;
            }


            dcartg1tot = $('td.Cartageamt_' + Id).find('input').val();
            if (!isNaN(dcartg1tot) && dcartg1tot != "") {
                dcartg1tot = dcartg1tot
            }
            else {
                dcartg1tot = 0;
            }

            var SubTotl1 = $('#total_SubTotal_Amount1_' + Id).val();
            if (!isNaN(SubTotl1) && SubTotl1 != null) {
                SubTotl1 = SubTotl1;
            } else {
                SubTotl1 = 0;
            }


            var Insuamt1 = $('td.InsurancePercentageAmt_' + Id).find('#txtInsurancePercentageAmt_' + Id).val();
            if (!isNaN(Insuamt1) && Insuamt1 != null) {
                Insuamt1 = Insuamt1;
            }
            else {
                Insuamt1 = 0;
            }


            var SumSubTotal2 = $('#total_SubTotal_Amount2_' + Id).val();
            if (!isNaN(SumSubTotal2) && SumSubTotal2 != null) {
                SumSubTotal2 = SumSubTotal2;
            }
            else {
                SumSubTotal2 = 0;
            }


            var S_CGST = $('td.CGSTPerAmtValue_' + Id).find('#txtCGSTPerAmtValue_' + Id).val();
            if (!isNaN(S_CGST) && S_CGST != null) {
                S_CGST = S_CGST;
            }
            else {
                S_CGST = 0;
            }

            var S_SGSTandUTGST = $('td.SGSTOrUTGSTPerAmtValue_' + Id).find('#txtSGSTOrUTGSTPerAmtValue_' + Id).val();
            if (!isNaN(S_SGSTandUTGST) && S_SGSTandUTGST != null) {
                S_SGSTandUTGST = S_SGSTandUTGST;
            }
            else {
                S_SGSTandUTGST = 0;
            }

            var S_IGST = $('td.IGSTPerAmtValue_' + Id).find('#txtIGSTPerAmtValue_' + Id).val();
            if (!isNaN(S_IGST) && S_IGST != null) {
                S_IGST = S_IGST;
            }
            else {
                S_IGST = 0;
            }

            var S_ToGST = $('td.TotalGSTAmtValue_' + Id).find('#txtTotalGSTAmtValue_' + Id).val();
            if (!isNaN(S_ToGST) && S_ToGST != null) {
                S_ToGST = S_ToGST;
            }
            else {
                S_ToGST = 0;
            }

            var S_ToGrossAmt = $('td.TotalGrossAmtValue_' + Id).find('#txtTotalGrossAmtValue_' + Id).val();
            if (!isNaN(S_ToGrossAmt) && S_ToGrossAmt != null) {
                S_ToGrossAmt = S_ToGrossAmt;
            }
            else {
                S_ToGrossAmt = 0;
            }


            SumTotal = parseFloat(SumTotal) + parseFloat(Dtotal);
            SumTotpackingCharge = parseFloat(SumTotpackingCharge) + parseFloat(dPacktot);
            SumTotCart1 = parseFloat(SumTotCart1) + parseFloat(dcartg1tot);
            SumTotal1 = parseFloat(SumTotal1) + parseFloat(SubTotl1);
            SumOfInsurance = parseFloat(SumOfInsurance) + parseFloat(Insuamt1);

            SumSubTott2Sum = parseFloat(SumSubTott2Sum) + parseFloat(SumSubTotal2);

            SUMOfCGST = parseFloat(SUMOfCGST) + parseFloat(S_CGST);

            SumOfSGSTAndUTGST = parseFloat(SumOfSGSTAndUTGST) + parseFloat(S_SGSTandUTGST);

            SumOfIGST = parseFloat(SumOfIGST) + parseFloat(S_IGST);

            SumOfTotalGST = parseFloat(SumOfTotalGST) + parseFloat(S_ToGST);
            SumofGrossAmount = parseFloat(SumofGrossAmount) + parseFloat(S_ToGrossAmt);

          


            $("#Total_Rate").val(SumRt.toFixed(2))
            $("#SumOfTotalVal").val(SumTotal.toFixed(2))
            $("#SumOfPackingCharge").val(SumTotpackingCharge.toFixed(2))
            $("#SumOfCartageAll1Amt").val(SumTotCart1.toFixed(2))

            $("#SumOfSubTotalAll1Amt").val(SumTotal1.toFixed(2))

            $("#SumOfInsuranceAmt").val(SumOfInsurance.toFixed(2))

            $("#SumOf_total_Sub_2").val(SumSubTott2Sum.toFixed(2))
            $("#Total_SumOf_CGST").val(SUMOfCGST.toFixed(2))
            $("#SumOfSGSTAndUTGST_Amt").val(SumOfSGSTAndUTGST.toFixed(2))

            $("#SumOf_IGST_Amt").val(SumOfIGST.toFixed(2))
            $("#SumOfAll_GST_amt").val(SumOfTotalGST.toFixed(2))

            $("#SumOfAll_Gross_Total_amt").val(SumofGrossAmount.toFixed(2))
        }

    });

    testSkill();
   
}

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


var iWords = ['Zero', ' One', ' Two', ' Three', ' Four', ' Five', ' Six', ' Seven', ' Eight', ' Nine'];
var ePlace = ['Ten', ' Eleven', ' Twelve', ' Thirteen', ' Fourteen', ' Fifteen', ' Sixteen', ' Seventeen', ' Eighteen', ' Nineteen'];
var tensPlace = ['', ' Ten', ' Twenty', ' Thirty', ' Forty', ' Fifty', ' Sixty', ' Seventy', ' Eighty', ' Ninety'];
var inWords = [];

var numReversed, inWords, actnumber, i, j;

function tensComplication() {
    'use strict';
    if (actnumber[i] === 0) {
        inWords[j] = '';
    } else if (actnumber[i] === 1) {
        inWords[j] = ePlace[actnumber[i - 1]];
    } else {
        inWords[j] = tensPlace[actnumber[i]];
    }
}

function testSkill() {
    debugger;
    'use strict';

   //document.getElementById('container').innerHTML = finalWord;
    //document.getElementById('Rupees').value = "Rupees" + finalWord;

    'use strict';
    var finalWord = withDecimal(document.getElementById('SumOfAll_Gross_Total_amt').value);
    //document.getElementById('container').innerHTML = finalWord;
    document.getElementById('Rupees').value = "Rupees " + finalWord + ' Only';
}