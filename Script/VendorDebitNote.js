$(document).ready(function () {

    

    var selectedData = [];
    $("#txtDebitNoteDate").datepicker(
        {
            dateFormat: 'dd M yy',
            maxDate: 0
        });
    $("#txtDebitNoteDate").change(OnDebitNoteDateChange);

    $(document).on('click', '#btnUpdate', UpdateDebitNote);

    $(document).on('keyup', "td[class^='InsuranceRatePercentage_']", function () {
        debugger;
        var C_Cartg = 0; var EPack = 0; var DTot = 0; var FSub1 = 0; var SUm = 0; var Insi_percent = 0; var Insi_Tot = 0; var insi_Sum = 0; var Sub_tot_2 = 0;
        //  var insi_id = $(this).find('input').val();
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

    $('#linkViewPODetails').click(function () {
        var poNo = $(this).attr('data');
        poNo = poNo.replace('/', '#');
        $.get('/View_Print_Slip_Approval/GetPO_details', { Id: poNo }, function (response) {
            $("#dialog-podetails").dialog({
                title: 'P.O Details',
                autoOpen: false,
                resizable: false,
                height: 450,
                width: 1100,
                show: { effect: 'drop', direction: "up" },
                modal: true,
                draggable: true,
                open: function (event, ui) {
                    $(this).html(response);
                }
            });
            $("#dialog-podetails").dialog('open');
        });
    });

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



        //var checktxt = $(this).parent().prev().children().find(":selected").text();
        //if (checktxt == 'Yes') {


        //$(".CartagerateValue_" + Ctg_ID).enabled;
        //$(".CartageValue_" + Ctg_ID).enabled;
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
        else if ($(this).val() == 2 || $(this).val() == 1) {
            $(".CartagerateValue_" + Ctg_ID).find('input').prop('disabled', true);
            $('td.Cartageamt_' + Ctg_ID).find('input').val(PutZeroVal); C1_lumAother();
        }



        //}
        //else if (checktxt == 'No') {

        //    $(".CartagerateValue_" + Ctg_ID).find('input').prop('disabled', true);
        //    $(".caramt_" + Ctg_ID).prop('disabled', true);
        //    $(".caramt_" + Ctg_ID).val(0);
        //}


    });

    $(document).on('change', '.gst-slab-ddl', function () {
        debugger;
        var gst_rowid = jQuery(this).data('gstval');
        var GST_code = $(this).val();
        var SubTot2 = $("#total_SubTotal_Amount2_" + gst_rowid).val();
        // var GetSplit_GST = BindGSTOnGRidItem;

        $.ajax({
            type: 'POST',
            url: '/IndentPurchaseOrders/GetAllGSTType',

            dataType: 'json',

            data: { TaxCode: GST_code },


            success: function (obj) {
                obj = $.parseJSON(obj)

                //$("#txtCGSTPerctg_" + gst_rowid).html(markup).show();
                $("#txtCGSTPerctg_" + gst_rowid).val(obj.tax_CGST);
                $("#txtSGSTOrUTGSTPerctg_" + gst_rowid).val(obj.tax_SGSTandUTGST);
                $("#txtIGSTPerctg_" + gst_rowid).val(obj.tax_IGST);
                CalculateAllGSTAmount(obj.tax_CGST, obj.tax_SGSTandUTGST, obj.tax_IGST, gst_rowid, SubTot2);
                $("#txtCGSTPerctg_" + gst_rowid).prop('disabled', true);
                $("#txtSGSTOrUTGSTPerctg_" + gst_rowid).prop('disabled', true);
                $("#txtIGSTPerctg_" + gst_rowid).prop('disabled', true);
                //$("#PurchaseOrderNo").val(obj.No);
                //$("#ProjectAddress").val(obj.Address);

            },
            error: function (ex) {
                alert('Failed to retrieve GST Code.' + ex);
                $("#txtCGSTPerctg_" + gst_rowid).val('');
                $("#txtSGSTOrUTGSTPerctg_" + gst_rowid).val('');
                $("#txtIGSTPerctg_" + gst_rowid).val('');
                $("#txtCGSTPerAmtValue_" + gst_rowid).val(''); $("#txtSGSTOrUTGSTPerAmtValue_" + gst_rowid).val('');
                $("#txtIGSTPerAmtValue_" + gst_rowid).val(''); $("#txtTotalGSTAmtValue_" + gst_rowid).val('');
                $("#txtTotalGrossAmtValue_" + gst_rowid).val('');

            }
        });
        return false;



    });
    $(document).on('change', '.checkbox', function () {
        //alert($(this).prop('checked'));
        if($(this).prop('checked'))
        {

            selectedData.push($(this).attr('id'));
        }
        else {
            selectedData = jQuery.grep(selectedData, function (value) {
                return value != $(this).attr('id');
            });
        }
    });

    $(document).on('click', '#btnCreateDebitNote', function () {
        //alert($('input:checkbox[name=chk]:checked').length);
        if ($('input:checkbox[name=chk]:checked').length > 0) {
            $('#hdnData').val(selectedData);
            $('#hdnProjectId').val($('#Projects option:selected').val());
            //alert($('#Projects option:selected').val());
            $('#frmCreateNote').submit();
        }
        else
            alert('Please select data to create debit note.');
    });



    $(document).on('click', '#btnCreate', CreateDebitNote);
 
   
    $("#Projects").change(function () {
        if ($("#Projects option:selected").val() != 0) {
            $('#dvLoading').show();
            $("#IssuedTo").empty();
            $.ajax({
                type: 'GET',
                url: 'GetVendors', // Calling json method
                dataType: 'json',
                data: { ProjectId: $("#Projects option:selected").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj);
                    $.each(obj, function (i, itname) {
                        $("#ddlVendors").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');
                    });
                    
                },
                error: function (ex) {
                     alert('Failed to retrieve Vendors.' + ex);
                }
            });
            return false;
        }
    });

    $("#ddlVendors").change(function () {
        if ($("#ddlVendors option:selected").val() != '') {
            $('#dvLoading').show();
            $("#ddlPONo").empty();
            $("#ddlPONo").append('<option value="">Select P.O Number</option>');
            $.ajax({
                type: 'GET',
                url: 'GetPOForVendors', // Calling json method
                dataType: 'json',
                data: { ProjectId: $("#Projects option:selected").val(),VendorId:$('#ddlVendors option:selected').val() },
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj);
                    $.each(obj, function (i, itname) {
                        $("#ddlPONo").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');
                    });
                },
                error: function (ex) {
                    alert('Failed to retrieve P.O Numbers.' + ex);
                }
            });
            return false;
        } else {
            $("#ddlPONo").empty();
            var markup = '<option value="">Select P.O Number</option>';

            $("#ddlPONo").html(markup).show();
        }
    });

    $("#ddlPONo").change(function () {
      
        if ($("#ddlPONo option:selected").val() != '') {
            $('#dvLoading').show();
            $("#ddlMRNNo").empty();
            $("#ddlMRNNo").append('<option value="">Select MRN Number</option>');
            $.ajax({
                type: 'GET',
                url: 'GeMRNForPONo', // Calling json method
                dataType: 'json',
                data: { ProjectId: $("#Projects option:selected").val(), VendorId: $('#ddlVendors option:selected').val(), PONumber: $('#ddlPONo option:selected').val() },
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                  
                    obj = $.parseJSON(obj);
                    console.log(obj);
                    //for (var i = 0; i < obj.length;i++) {
                    //    $("#ddlMRNNo").append('<option value="' + obj[i] + '">' +
                    //         obj[i] + '</option>');
                    //}
                    $.each(obj, function (i, itname) {
                     
                        $("#ddlMRNNo").append('<option value="' + itname.MRNCode + '">' +
                             itname.MRNCode + '</option>');
                    });
                 
                },
                error: function (ex) {
                    alert('Failed to retrieve MRN Numbers.' + ex);
                }
            });
            return false;
        } else {
            $("#ddlMRNNo").empty();
            var markup = '<option value="">Select MRN Number</option>';

            $("#ddlMRNNo").html(markup).show();
        }
    });


    $('#div_filter').hide();
    $('#formbody').hide();


    $('#btnView').click(function (e) {
        e.preventDefault();
       
        $('#loadingModal').modal();

        $.ajax({
            type: 'GET',
            url: 'GetSearchData', // Calling json method
            dataType: 'html',
            data: { ProjectId: $("#Projects option:selected").val(), VendorId: $('#ddlVendors option:selected').val(), PONumber: $('#ddlPONo option:selected').val(), MRNNumber: $('#ddlMRNNo option:selected').val() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                $('#formdata').html(result);
            },
            error: function (ex) {
                alert('Failed to retrieve search result' + ex);
            }
        });


        $('#loadingModal').modal('hide');
    });

    validteq();
    CalculateTotal();

    ItemWiseFullCalculation();


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
                var markup = "";
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

                $("#Projects").prop('selectedIndex', 0).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Project.' + ex);
            }
        });
        return false;
    };
    
    GetPrj();
    //$('#Projects').trigger('change');
});




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

//function testSkill() {
//    debugger;
//    'use strict';
//    var junkVal = document.getElementById('SumOfAll_Gross_Total_amt').value;
//    junkVal = Math.floor(junkVal);
//    var obStr = junkVal.toString();
//    numReversed = obStr.split('');
//    actnumber = numReversed.reverse();

//    if (Number(junkVal) >= 0) {
//        //do nothing
//    } else {
//        window.alert('wrong Number cannot be converted');
//        return false;
//    }
//    if (Number(junkVal) === 0) {
//        document.getElementById('container').innerHTML = obStr + '' + 'Rupees Zero Only';
//        return false;
//    }
//    if (actnumber.length > 9) {
//        window.alert('Oops!!!! the Number is too big to covertes');
//        return false;
//    }



//    var iWordsLength = numReversed.length;
//    var finalWord = '';
//    j = 0;
//    for (i = 0; i < iWordsLength; i++) {
//        switch (i) {
//            case 0:
//                if (actnumber[i] === '0' || actnumber[i + 1] === '1') {
//                    inWords[j] = '';
//                } else {
//                    inWords[j] = iWords[actnumber[i]];
//                }
//                inWords[j] = inWords[j] + ' Only';
//                break;
//            case 1:
//                tensComplication();
//                break;
//            case 2:
//                if (actnumber[i] === '0') {
//                    inWords[j] = '';
//                } else if (actnumber[i - 1] !== '0' && actnumber[i - 2] !== '0') {
//                    inWords[j] = iWords[actnumber[i]] + ' Hundred and';
//                } else {
//                    inWords[j] = iWords[actnumber[i]] + ' Hundred';
//                }
//                break;
//            case 3:
//                if (actnumber[i] === '0' || actnumber[i + 1] === '1') {
//                    inWords[j] = '';
//                } else {
//                    inWords[j] = iWords[actnumber[i]];
//                }
//                if (actnumber[i + 1] !== '0' || actnumber[i] > '0') {
//                    inWords[j] = inWords[j] + ' Thousand';
//                }
//                break;
//            case 4:
//                tensComplication();
//                break;
//            case 5:
//                if (actnumber[i] === '0' || actnumber[i + 1] === '1') {
//                    inWords[j] = '';
//                } else {
//                    inWords[j] = iWords[actnumber[i]];
//                }
//                if (actnumber[i + 1] !== '0' || actnumber[i] > '0') {
//                    inWords[j] = inWords[j] + ' Lakh';
//                }
//                break;
//            case 6:
//                tensComplication();
//                break;
//            case 7:
//                if (actnumber[i] === '0' || actnumber[i + 1] === '1') {
//                    inWords[j] = '';
//                } else {
//                    inWords[j] = iWords[actnumber[i]];
//                }
//                inWords[j] = inWords[j] + ' Crore';
//                break;
//            case 8:
//                tensComplication();
//                break;
//            default:
//                break;
//        }
//        j++;
//    }


//    inWords.reverse();
//    for (i = 0; i < inWords.length; i++) {
//        finalWord += inWords[i];
//    }
//    document.getElementById('container').innerHTML = finalWord;
//    document.getElementById('Rupees').value = "Rupees" + finalWord;
//}

function UpdateDebitNote()
{

    if ($('#txtDebitNoteNo').val() == '') {
        alert('Please select debit note date');
        return false;
    }

    //debugger;
    var items = [];

    var tblRows = $('#tbl_topnm tr');

    tblRows.each(function () {
        var itemObj = {};
        var sn = $(this).attr('data-trautoincrm');
        var transId = $(this).attr('id');
        itemObj = {
            'SNo': sn,
            'TransId': transId,
            'ItemName': $('#itemName_' + sn).text(),
            'ItemNo': $('#itemName_' + sn).attr('data'),
            'UnitName': $('#unitName_' + sn).text(),
            'CTotalAmount': $('#Ctotal_amount_' + sn).val(),
            'TotalSubTotal': $('#total_SubTotal_Amount1_' + sn).val(),
            'TotalSubTotal2': $('#total_SubTotal_Amount2_' + sn).val(),
            'TotalGrandTotal': $('#total_GrandTotal_Amount_' + sn).val()
        };
        items.push(itemObj);
    });
    //console.log(items);

    //collapse panel data
    var colData = [];

    var tblDescription1 = $('.description1');
    tblDescription1.each(function () {
        tblRow = $(this).find('tr:eq(1)');
        tblRow.each(function () {
            var id = $(this).attr('id');
            var itemObj = {
                'SNo': id,
                'Description': $('#IDesc_' + id).val(),
                'TransferQty': $('.Qty_' + id).val(),
                'Qty':$('#hdn_item_approv_qty_' + id).val(),
                'CRate': $('.Rate_' + id).val(),
                'Discount': $('#txtDiscount_' + id).val(),
                'DiscountAmount': $('#txtDiscount_' + id).val(),
                'PackingChargePer': $('#txtpackingChargePercentage_' + id).val(),
                'PackingChargeAmt': $('#txtpackingChargePercentageAmount_' + id).val(),
                'CartageType': $('#CartageTypeddl_' + id).val(),
                'CartageRate': $('#CartageRate1_' + id).val(),
                'CartageAmt': $('.caramt_' + id).val(),
            };
            colData.push(itemObj);
        });
    });


    //console.log(colData);


    var colData1 = [];

    var tblDescription2 = $('.description2');
    tblDescription2.each(function () {
        tblRow = $(this).find('tr:eq(1)');
        tblRow.each(function () {
            var id = $(this).attr('id');
            var itemObj = {
                'SNo': id,
                'InsuranceRatePer': $('#txtInsuranceRatePercentage_' + id).val(),
                'InsurancePerAmt': $('#txtInsurancePercentageAmt_' + id).val(),
                'GSTSlab': $('#ddlGSTSlabs_' + id).val(),
                'CGSTPer': $('#txtCGSTPerctg_' + id).val(),
                'SGSTOrUGSTPer': $('#txtSGSTOrUTGSTPerctg_' + id).val(),
                'IGSTPer': $('#txtIGSTPerctg_' + id).val(),
                'CGSTPerAmt': $('#txtCGSTPerAmtValue_' + id).val(),
                'SGSTOrUTGSTPerAmt': $('#txtSGSTOrUTGSTPerAmtValue_' + id).val(),
                'IGSTPerAmt': $('#txtIGSTPerAmtValue_' + id).val(),
                'TotalGSTAmt': $('#txtTotalGSTAmtValue_' + id).val(),
                'TotalGrossAmt': $('#txtTotalGrossAmtValue_' + id).val()
            };
            colData1.push(itemObj);
        });
    });


    //console.log(colData1);

    //total calculation data

    var totalCalculation = {
        'TotalRate': $('#Total_Rate').val(),
        'SumOfPacking': $('#SumOfPackingCharge').val(),
        'SumOfCartageAmt': $('#SumOfCartageAll1Amt').val(),
        'SubTotal1': $('#SumOfSubTotalAll1Amt').val(),
        'SubTotal2': $('#SumOf_total_Sub_2').val(),
        'Total_Insurance': $('#SumOfInsuranceAmt').val(),
        
        'Total_CGST': $('#Total_SumOf_CGST').val(),
        'Total_SGSTAndUTGST': $('#SumOfSGSTAndUTGST_Amt').val(),
        'Total_IGST': $('#SumOf_IGST_Amt').val(),
        'Total_Taxable': $('#SumOfAll_GST_amt').val(),
        'Total': $('#SumOfAll_Gross_Total_amt').val(),
        'GrandTotal':$('#SumOfTotalVal').val()
        // 'TotalAmountInWords': $('#Rupees').val()
    };


    //console.log(totalCalculation);
    //alert($('#SendTo option:selected').val());
    $.ajax({
        url: '/VendorDebitNote/UpdateDebitNote',
        data: { Items: items, ItemDescription: colData, InsuranceGStDetails: colData1, TotalCalculation: totalCalculation, PONo: $('#PONo').text(), DebitNoteCode: $('#txtDebitNoteNo').val(), DebitNoteDate: $('#txtDebitNoteDate1').val(), SendTo: $('#SendTo option:selected').val() },
        type: 'POST',
        success: function (result) {
            if (result.Status == 1) {
                alert('Debit Note Updated');
                location.href = result.Url;
            }
            else if (result.Status == 2)
                alert('some error occur');
            else if (result.Status == 3)
                alert('Debit Note not updated');
        },
        error: function (ex) {
            alert('error' + ex);
        }
    });   
}


function CreateDebitNote()
{

    if ($('#txtDebitNoteNo').val() == '') {
        alert('Please select debit note date');
        return false;
    }
    debugger;
    var fields = $('.required1').filter(function () { return this.value == ""; });

    if (fields.length > 0) {
        alert($(fields[0]).attr('field') + ' can not be empty.');
        return false;
    }
    //return false;
    //debugger;
    var items = [];

    var tblRows = $('#tbl_topnm tr');

    tblRows.each(function () {
        var itemObj = {};
        var sn = $(this).attr('data-trautoincrm');
        var transId = $(this).attr('id');
        itemObj = {
            'SNo': sn,
            'TransId': transId,
            'ItemName': $('#itemName_' + sn).text(),
            'ItemNo': $('#itemName_' + sn).attr('data'),
            'UnitName': $('#unitName_' + sn).text(),
            'CTotalAmount': $('#Ctotal_amount_' + sn).val(),
            'TotalSubTotal': $('#total_SubTotal_Amount1_' + sn).val(),
            'TotalSubTotal2': $('#total_SubTotal_Amount2_' + sn).val(),
            'TotalGrandTotal': $('#total_GrandTotal_Amount_' + sn).val()
        };
        items.push(itemObj);
    });
    //console.log(items);

    //collapse panel data
    var colData = [];

    var tblDescription1 = $('.description1');
    tblDescription1.each(function () {
        tblRow = $(this).find('tr:eq(1)');
        tblRow.each(function () {
            var id = $(this).attr('id');
            var itemObj = {
                'SNo': id,
                'Description': $('#IDesc_' + id).val(),
                'TransferQty': $('.Qty_' + id).val(),
                'Qty':$('#hdn_item_approv_qty_' + id).val(),
                'CRate': $('.Rate_' + id).val(),
                'Discount': $('#txtDiscount_' + id).val(),
                'DiscountAmount': $('#txtDiscount_' + id).val(),
                'PackingChargePer': $('#txtpackingChargePercentage_' + id).val(),
                'PackingChargeAmt': $('#txtpackingChargePercentageAmount_' + id).val(),
                'CartageType': $('#CartageTypeddl_' + id).val(),
                'CartageRate': $('#CartageRate1_' + id).val(),
                'CartageAmt': $('.caramt_' + id).val(),
            };
            colData.push(itemObj);
        });
    });


   // console.log(colData);


    var colData1 = [];

    var tblDescription2 = $('.description2');
    tblDescription2.each(function () {
        tblRow = $(this).find('tr:eq(1)');
        tblRow.each(function () {
            var id = $(this).attr('id');
            var itemObj = {
                'SNo': id,
                'InsuranceRatePer': $('#txtInsuranceRatePercentage_' + id).val(),
                'InsurancePerAmt': $('#txtInsurancePercentageAmt_' + id).val(),
                'GSTSlab': $('#ddlGSTSlabs_' + id).val(),
                'CGSTPer': $('#txtCGSTPerctg_' + id).val(),
                'SGSTOrUGSTPer': $('#txtSGSTOrUTGSTPerctg_' + id).val(),
                'IGSTPer': $('#txtIGSTPerctg_' + id).val(),
                'CGSTPerAmt': $('#txtCGSTPerAmtValue_' + id).val(),
                'SGSTOrUTGSTPerAmt': $('#txtSGSTOrUTGSTPerAmtValue_' + id).val(),
                'IGSTPerAmt': $('#txtIGSTPerAmtValue_' + id).val(),
                'TotalGSTAmt': $('#txtTotalGSTAmtValue_' + id).val(),
                'TotalGrossAmt': $('#txtTotalGrossAmtValue_' + id).val()
            };
            colData1.push(itemObj);
        });
    });


   // console.log(colData1);

    //total calculation data

    var totalCalculation = {
        'TotalRate': $('#Total_Rate').val(),
        'SumOfPacking': $('#SumOfPackingCharge').val(),
        'SumOfCartageAmt': $('#SumOfCartageAll1Amt').val(),
        'SubTotal1': $('#SumOfSubTotalAll1Amt').val(),
        'SubTotal2': $('#SumOf_total_Sub_2').val(),
        'Total_Insurance': $('#SumOfInsuranceAmt').val(),
        
        'Total_CGST': $('#Total_SumOf_CGST').val(),
        'Total_SGSTAndUTGST': $('#SumOfSGSTAndUTGST_Amt').val(),
        'Total_IGST': $('#SumOf_IGST_Amt').val(),
        'Total_Taxable': $('#SumOfAll_GST_amt').val(),
        'Total': $('#SumOfAll_Gross_Total_amt').val(),
        'GrandTotal':$('#SumOfTotalVal').val()
       // 'TotalAmountInWords': $('#Rupees').val()
    };


    //console.log(totalCalculation);
    //alert($('#SendTo option:selected').val());
    $.ajax({
        url: '/VendorDebitNote/SaveDebitNote',
        data: { Items: items, ItemDescription: colData, InsuranceGStDetails: colData1, TotalCalculation: totalCalculation, PONo: $('#PONo').text(), DebitNoteCode: $('#txtDebitNoteNo').val(), DebitNoteDate: $('#txtDebitNoteDate').val(), SendTo: $('#SendTo option:selected').val(), Reason: $('#txtReason').val()},
        type: 'POST',
        success: function (result) {
            if (result.Status == 1) {
                alert('Debit Note Created');
                location.href = result.Url;
            }
            else if (result.Status == 2)
                alert('Debit note not created');
            else if (result.Status == 3)
                alert(result.Msg);
        },
        error: function (ex) {
            alert('error' + ex);
        }
    });   
}



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



function ClearValues() {
    $("#Total").val('');
    $("#Vat").val('');
    $("#VatAmount").val('');
    $("#SubTotal").val('');
    $("#SurCharge").val('');
    $("#Cartage").val('');
    $("#GrandTotal").val('');
    $("#Rupees").val('');
    $("ExciseDutyRate").val();
    $("ExciseDuty").val();
}
function validteq() {

    $(document).on('keyup', "td[class^='CQty_']", function () {

        var q = $(this).find('input').val();
        var str = $(this).attr('class');
        var a = str.substring(4);
        var remq = $("#hdn_remaing_qty" + a).val();

        if (+q > +remq) {

            alert("Quantity Can't Greater than Remaining");
            $(this).find('input').val('');
        }

    });
}


function OnDebitNoteDateChange () {
    if ($("#txtDebitNoteDate").val() == '') {
        alert('Please select debit note date');
        $('#txtDebitNoteNo').val('');
        return false;
    }
    var url = '/VendorDebitNote/GetDebitCode';
    $.get(url, {debitDate: $("#txtDebitNoteDate").val()},
        function (result) {
            $('#txtDebitNoteNo').val(result);
        });
}



function CalculateTotal() {

    $(document).on('keyup', "td[class^='CRate_']", function () {

        var temp1 = 0; var Temp2 = 0; var Tolt = 0;
        var Rate = $(this).find('input').val();
        var str = $(this).attr('class');
        var a = str.substring(5);

        var cmrts = $("#RatingsCompares").val();


        var Qty = $(".CQty" + a).find('input').val();
        //  Disc = $(".CDiscount" +a).find('input').val();

        if (Rate != null && Qty != null)
        { temp1 = Qty * Rate; }

        if (temp1 != null)
        { $("#Ctotal_amount" + a).val(temp1.toFixed(3)); }

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

    //if (isNaN(Ex_val) || Ex_val == '') {
    //    Ex_val = 0;
    //}
    //else {
    //    Ex_val = $(".ExciseDutyValue_" + Idd).find('input').val();
    //}

    //G = sum * (Ex_val / 100);

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

    //if ($('td.InsuranceTypeSel1_' + Idd).find('#Insurance_Type_1_' + Idd).find(':selected').val() != "" && $('td.InsuranceTypeSel1_' + Idd).find('#Insurance_Type_1_' + Idd).find(':selected').val() != "-1") {
    //    var insi = $('td.InsuranceTypeSel1_' + Idd).find('#Insurance_Type_1_' + Idd).find(':selected').text();
    //    if (insi == 'lupsum') {
    //        Ins = 0;
    //    }
    //    else {
    //        Ins = $('td.InsuranceAmtUpto1_' + Idd).find('#InsuranceAmtUptoValue1_' + Idd).val();
    //    }
    //}

    //var qty = $(".CQty_" +Idd).find('input').val();
    //H = qty * cartage_rate;
    // Tolt = parseFloat(D) + parseFloat(E) + parseFloat(G) + parseFloat(H) + parseFloat(Ins);

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
    $("#txtCGSTPerAmtValue_" + ID).prop('disabled', true); $("#txtSGSTOrUTGSTPerAmtValue_" + ID).prop('disabled', true);
    $("#txtIGSTPerAmtValue_" + ID).prop('disabled', true); $("#txtTotalGSTAmtValue_" + ID).prop('disabled', true);
    $("#txtTotalGrossAmtValue_" + ID).prop('disabled', true);

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
        //alert(MyCartagesel)
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

            // SumTotpackingChargegolbal = SumTotpackingCharge +SumTotpackingChargegolbal ;

           // alert(SumTotal);

            $("#Total_Rate").val(SumRt.toFixed(3))
            $("#SumOfTotalVal").val(SumTotal.toFixed(3))
            $("#SumOfPackingCharge").val(SumTotpackingCharge.toFixed(3))
            $("#SumOfCartageAll1Amt").val(SumTotCart1.toFixed(3))

            $("#SumOfSubTotalAll1Amt").val(SumTotal1.toFixed(3))

            $("#SumOfInsuranceAmt").val(SumOfInsurance.toFixed(3))

            $("#SumOf_total_Sub_2").val(SumSubTott2Sum.toFixed(3))
            $("#Total_SumOf_CGST").val(SUMOfCGST.toFixed(3))
            $("#SumOfSGSTAndUTGST_Amt").val(SumOfSGSTAndUTGST.toFixed(3))

            $("#SumOf_IGST_Amt").val(SumOfIGST.toFixed(3))
            $("#SumOfAll_GST_amt").val(SumOfTotalGST.toFixed(3))

            $("#SumOfAll_Gross_Total_amt").val(SumofGrossAmount.toFixed(3))
        }

    });

    //testSkill();
    BindDropdownChangeOnPoValue()
}


function BindDropdownChangeOnPoValue() {
    debugger;
    var poval = $("#hdn_poLimitval").val();
    var Gdtot = $("#SumOfAll_Gross_Total_amt").val();
    if (parseFloat(poval) >= parseFloat(Gdtot)) {
        $("#po_purinlimit").show();
        $("#po_pmcoutlimit").hide();
    }
    else {

        $("#po_purinlimit").hide();
        $("#po_pmcoutlimit").show();
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
   // alert(DiscountedRate)
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


//function CreateDebitNote() {
//    $('#frmDebitNoteData').submit();
//}