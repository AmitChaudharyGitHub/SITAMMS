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
    var junkVal = document.getElementById('SumOfAll_Gross_Total_amt').value;
    junkVal = Math.floor(junkVal);
    var obStr = junkVal.toString();
    numReversed = obStr.split('');
    actnumber = numReversed.reverse();

    if (Number(junkVal) >= 0) {
        //do nothing
    } else {
        window.alert('wrong Number cannot be converted');
        return false;
    }
    if (Number(junkVal) === 0) {
        document.getElementById('container').innerHTML = obStr + '' + 'Rupees Zero Only';
        return false;
    }
    if (actnumber.length > 9) {
        window.alert('Oops!!!! the Number is too big to covertes');
        return false;
    }


    var finalWord = withDecimal(document.getElementById('SumOfAll_Gross_Total_amt').value);
    document.getElementById('container').innerHTML =   finalWord;
    document.getElementById('Rupees').value =  "Rupees " + finalWord +' Only';  
}


$(document).ready(function () { 

   //$(".DatePicker").datepicker({
   //     dateFormat: 'dd M yy',
   //     changeMonth: true,
   //     changeYear: true,
   //     value: "",
   //     maxDate: new Date()
   // });

    $('.continue').click(function () {

        $('.nav-tabs > .active').next('li').find('a').trigger('click');
    });
    $('.back').click(function () {
        $('.nav-tabs > .active').prev('li').find('a').trigger('click');
    });


    $("#VNAME").change(function () {
        if ($("#VNAME option:selected").val() != 0) {
            debugger
            $("#pnl_toggel_vendor-detail").show();
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetVendorDetail, // Calling json method
                async: false,
                dataType: 'json',

                data: { ID: $("#VNAME").val() },

                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)

                    BindRowFieldWithValueGST(); // getting item wise value after load of Item in grid check later.
                    $("#lbl_vendor_address").text(obj.Address);
                    $("#lbl_vendorCountry").text(obj.VCountry);
                    $("#lbl_vendor_state").text(obj.VState);
                    $("#lbl_vendor_City").text(obj.VCity);
                    $("#lbl_vendor_PinNo").text(obj.V_PIN);
                    $("#lbl_vendor_TinNO").text(obj.TinNo);
                    $("#lbl_vendor_AadhaarNo").text(obj.vAdhar);
                    $("#lbl_Vendor_CP_name").text(obj.CpName);
                    $("#lbl_Vendor_CP_Mobile").text(obj.CpMobile);
                    $("#lbl_vendor_CP_Phone").text(obj.CpPhone);
                    $("#lbl_vendor_CP_Email").text(obj.CpEmail);

                    $(".gst-slab-ddl option").addClass("hide");
                    $(".gst-slab-ddl option").attr('disabled', 'disabled');
                    prjState = $('#POStateID').val();

                    if (prjState == obj.StateID) {
                        $(".gst-slab-ddl option").filter(function () {
                            if ($(this).text().substr(0, 3) == 'GST') {
                                $(this).removeClass("hide");
                                $(this).removeAttr('disabled');
                                $('.GST_Col').removeClass("hide");
                                $('.IGST_Col').addClass("hide");
                                return this;
                            }
                        });
                    }
                    else {
                        $(".gst-slab-ddl option").filter(function () {
                            if ($(this).text().substr(0, 4) == 'IGST') {
                                $(this).removeClass("hide");
                                $(this).removeAttr('disabled');
                                $('.IGST_Col').removeClass("hide");
                                $('.GST_Col').addClass("hide");
                                return this;
                            }
                        });
                    }


                },
                error: function (ex) {
                    // alert('Failed to retrieve Project Code.' + ex);
                }
            });
            return false;
        }
        else {
            $("#pnl_toggel_vendor-detail").hide();
            $("#SupplierAddress").val('');
            $("#SupplierTinNo").val('');
        }
    });

    $("#prjtid1").change(function () {
        $("#Delivery_Address1").empty();
        $("#Billing_Address1").empty();
        $("#contact_Person_Info1").empty();
        $("#contact_Person_Mobile1").empty();

        $.ajax({
            type: 'POST',
            url: GetPODeleveryDetailAddr,

            dataType: 'json',

            data: { Uid: $('#hdn_Purchase_Indent_detail').val() },


            success: function (partes) {
                debugger;
                $("#Delivery_Address1").val(partes.DeliveryAddr);
                $("#Billing_Address1").val(partes.BillingAddr);
               // $("#contact_Person_Info1").val(partes.PName);
                //$("#contact_Person_Mobile1").val(partes.ContMobile);
                if (partes.ContType != null) {
                    $('#ddl_Division').val(partes.ContType).change();
                    $('#ddl_ContactPerson').val(partes.PersonID);
                    $("#txt_Contact").val(partes.ContMobile);
                }
                else {
                    $('#ddl_Division option:eq(0)').attr('selected', 'selected');
                }

                $("#Delivery_Schedule1").val(partes.DeliverySchedule);

            },
            error: function (ex) {
               
            }
        });








        return false;
    });

    $("#ddlPurchaseEmployePerson").change(function () {
        $("#contact_Person_Mobile1").empty();
       
        $.ajax({
            type: 'POST',
            url: GetEmployeeContatOnGST, // Calling json method

            dataType: 'json',

            data: { id: $("#ddlPurchaseEmployePerson option:selected").val() },
            success: function (partes) {
                $("#contact_Person_Mobile1").val(partes);
            },
            error: function (ex) {
                alert('Failed to retrieve Mobile No.' + ex);
            }
        });
        return false;
    })
   



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


    var GetPu_OnPodetail = EditPODetail;
    debugger;
    $.getJSON(GetPu_OnPodetail, { PONo: $("#hdn_PoEditNo").val() }, function (data) {

        data = $.parseJSON(data);



        $.each(data, function (i, item) {
            $('#lblprojctnmWithaddd').text(item.ProjectName + ' ,' + item.Location);
            $('#PurchaseOrderNo').val(item.PONumber);
            $('#lblPurch_indentno').text(item.PurRequisitionNo);
            $('#lbl_pur_indentdate').text(item.Date);
            $('#lbl_purIndent_QuotrefNo').text(item.QuotationRefNo);
            $('#hdn_Purchase_Indent_detail').val(item.UId);
            $('#hdn_purchase_project_Id').val(item.ProjectNo);
            $('#hdn_purch_indent_no_ber').val(item.PurRequisitionNo);
            $('#Reference1').val(item.Reference);
            //draft use
            $('#pUId').val(item.UId);
            $('#PurchaseOrderID').val(item.PurchaseOrderId)
            ////
            EditVennameId = item.SupplierNo;
            $("#PMCEmployee").val(item.Send_To);

            indentDate = new Date(item.Date);
            $(".DatePicker").datepicker({
                dateFormat: 'dd M yy',
                changeMonth: true,
                changeYear: true,
                value: "",
                maxDate: new Date(),
                minDate: new Date(item.Date)
            });
        });

        BindrecordThroughPurchaseIndentReq($('#hdn_Purchase_Indent_detail').val());
        debugger;



        $("#prjtid1").prop('selectedIndex', 1).trigger('change');


        GetFloor($('#hdn_Purchase_Indent_detail').val());
        GetSi($('#hdn_Purchase_Indent_detail').val());
        GetSTC($('#hdn_Purchase_Indent_detail').val());



        // $("#VNAME").val(EditVennameId).trigger('change');
        // $("#VGID").val(EditVendrpId);
        //  $("#VGID").val('TID0000002').trigger('change');
        $("#VNAME").trigger('change');

    });

    $("#VGID").change(function () {

        if ($("#VGID option:selected").val() != 0) {
            $('#dvLoading').show();
            $("#VNAME").empty();
            $.ajax({
                type: 'POST',
                url: GetVendorByGroup, // Calling json method

                dataType: 'json',

                data: { Vid: $("#VGID").val(), Pid: $('#hdn_purchase_project_Id').val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj);

                    // console.log(obj)
                    GetVendor(obj);
                    //BindRowFieldWithValue();


                },
                error: function (ex) {
                    // alert('Failed to retrieve Project Code.' + ex);
                }
            });
            return false;
        } else {
            var markup = "<option value='0'>Select Vendor</option>";

            $("#VNAME").html(markup).show();
        }
    })

    // for Save

    $('#btnsave').click(function (e) {
        debugger;
        var V = Valid();

        if (V == false)
            return;

        if ($('#po_purinlimit').css('display') != "none" && $('#PMCEmployee option:selected').val() == '') {
            alert('Please select Forward for approval.');
            return false;
        }


        if ($('#po_pmcoutlimit').css('display') != "none" && $('#Employee option:selected').val() == '') {
            alert('Please select Forward for approval.');
            return false;
        }

        //debugger;
        var _griddata = gridToArray();
       // console.log(JSON.stringify(_griddata));
        //console.log(otArr);
        var url = UpdateEditPO;
        $.ajax({
            type: 'POST',
            url: url,
            data: { Grid: _griddata },
           // contentType: "application/json",
            dataType: "json",
            processdata: true,

            success: function (json) {
                debugger;

                if (json.Status == "1") {
                    alert("Data Successfully Updated.");

                    //  url: json.redidtUrl,
                    window.location.href = Redurl;



                }

                else if (json.Status == "3") {
                    alert("PO status is NOT Approved , so can not be edited..")
                }

                else if (json.Status == "2") {
                    alert("Some exception problem has occured.");
                }
                else if (json.Status == "4") {
                    alert("Something wrong in Purchase Order. Please Re-login or Re-Load Page.");
                }
                else if (json.status = "9") {
                    alert(json.Message);
                }

            },
            error: function (ex) {
                debugger;

                
                // alert('Failed to retrieve Project Code.' + ex);
                alert('Error in Submit Data' + ex);
                debugger;
            }
        });
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

    function gridToArray() {

        var otArr = [];
        var i = 0;

        var tbl2 = $('div.single').find('div.second').each(function (i) {
            debugger;
            var obj = new Object();

            var jh = jQuery(this).data('dhdn');
            var currQty = $('td.CQty_' + jh).find('input').val();
            i = i + 1;

            if (currQty != null) {
                debugger;
                 obj.UId = $('#hdn_pur_req_id_'+jh).val();
                 obj.Item_Description = $('td.SrNo_' + jh).find('#IDesc_' + jh).val();
                     //$('td.SrNo_' + jh).find('textarea#IDesc_' + jh).val();
                 obj.Qty = $('td.CQty_' + jh).find('input').val();

                
                //Two Type of rate. 1. Normal user entere rate 2. discounted rate.
                //normal rate
                //obj.Rate = $('td.CRate_' + jh).find('input').val();
                obj.Rate = $('td.CRate_' + jh).find('#hdndiscountedRate_' + jh).val();
                // discounted rate 
                obj.NewRate = $('td.CRate_' + jh).find('input').val();

                obj.MUid = i;

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

                obj.ItemNo = $('#hdn_itemnoto_' + jh).val();
                obj.IndentId = $('#hdn_indent_uid_' + jh).val();
                obj.RemOrApprovQty = $('#hdn_remaing_qty_' + jh).val();
                obj.PIQty = $('#hdn_item_approv_qty_' + jh).val();
                obj.Total = $('#Ctotal_amount_' + jh).val();
                obj.SubTotal1 = $('#total_SubTotal_Amount1_' + jh).val();
                obj.TCS_Rate = $(this).find('select.dll_TCS').val();
                obj.TotalTCSAmount = $(this).find('input.tcs_amt').val();
                obj.SubTotal2 = $('#total_SubTotal_Amount2_' + jh).val();
                obj.TotalGrosssAmount = $(this).find('input.gross_amt').val();
                // obj.Item_GrandTotal = $('#total_GrandTotal_Amount_' + jh).val();



                otArr.push(obj)
                // oatFirstBlock.push(obj);
            }



        });

        debugger;
        var obj1 = new Object();

        obj1.PurchaseOrderDate = $("#Date").val();
        obj1.PurchaseOrderNo = $("#PurchaseOrderNo").val();

        obj1.ProjectNo = $("#hdn_purchase_project_Id").val();
        obj1.Vendor_Group_Id = $("#VGID option:selected").val();

        obj1.SupplierNo = $("#VNAME option:selected").val();

        obj1.PODescription = $('#txtPODescription').val();
     
        obj1.Reference = $("#Reference1").val();

        obj1.IndentRefNo = $("#hdn_purch_indent_no_ber").val();
    

        obj1.Total = $("#SumOfTotalVal").val();
     

        obj1.SubTotal1 = $("#SumOfSubTotalAll1Amt").val();
        obj1.SubTotal2 = $("#SumOf_total_Sub_2").val();

        
        obj1.GrandTotal = $("#SumOfAll_Gross_Total_amt").val();
      
        if (parseFloat($("#hdn_poLimitval").val()) >= parseFloat($("#SumOfAll_Gross_Total_amt").val())) {
            obj1.SendTo = $("#PMCEmployee option:selected").val();
            obj1.CheckedBeyondPOLimit = 1;
            obj1.Purchase_Id = $("#PMCEmployee option:selected").val();

        }
        else {
            obj1.SendTo = $("#Employee option:selected").val();
            obj1.CheckedBeyondPOLimit = 0;
            obj1.PMC_Id = $("#PMCEmployee option:selected").val();

        }


        debugger;

    
        obj1.QuotationRefNo = $("#Quot_ion_refId").val();
        obj1.Remarks = $("#PORemarks").val();
        obj1.Total_PAndF = $("#SumOfPackingCharge").val();
        obj1.Total_Cartage = $("#SumOfCartageAll1Amt").val();
        obj1.Total_Insurance = $("#SumOfInsuranceAmt").val();
        obj1.Total_Taxable = $("#SumOfAll_GST_amt").val();
        obj1.Total_CGST = $("#Total_SumOf_CGST").val();
        obj1.Total_SGSTAndUTGST = $("#SumOfSGSTAndUTGST_Amt").val();
        obj1.Total_IGST = $("#SumOf_IGST_Amt").val();
        obj1.Total_NetAmountInWords = $("#Rupees").val();
        obj1.Total_TCS = $("#SumOftotal_TCSAmt").val();
        //draft use
        obj1.UID = $('#pUId').val();
        obj1.PurchaseOrderId = $('#PurchaseOrderID').val();
       

        var otArr2 = [];
        var tbl2 = $('#grid tbody tr').each(function (i) {


            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {

                x = $(this).children().eq(0).find('input');
               // var c = x.is(':checked');
                var c = true;
                if (c == true) {
                    obj.STC_Master_ID = x.val();
                    obj.Purchase_Order_No = $("#PurchaseOrderNo").val();

                    obj.ProjectId = $("#hdn_purchase_project_Id").val();

                    //obj.IndentId = $(this).children().eq(0).text().trim();        
                    obj.Id = $(this).children().eq(1).find('input').val().trim();
                    obj.HeaderTitle = $(this).children().eq(2).find('input').val().trim();

                    obj.STC_Description = $(this).children().eq(3).find('textarea').val().trim();
                    obj.Status = x.is(':checked') ? "Checked" : "UnChecked";

                    otArr2.push(obj);
                }

            }
        })

        var otArr3 = [];
        var tbl2 = $('#grid2 tbody tr').each(function (i) {


            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {

                x = $(this).children().eq(0).find('input');
                //  var c = x.is(':checked');
                var c = true;
                if (c == true) {
                    obj.SI_Master_ID = x.val();
                    obj.Purchase_Order_No = $("#PurchaseOrderNo").val();

                    obj.ProjectId = $("#hdn_purchase_project_Id").val();

                    //obj.IndentId = $(this).children().eq(0).text().trim();  
                    obj.Id = $(this).children().eq(1).find('input').val().trim();
                    obj.Header_Title = $(this).children().eq(2).find('input').val().trim();
                    obj.SI_Description = $(this).children().eq(3).find('textarea').val().trim();
                    obj.Status = x.is(':checked') ? "Checked" : "UnChecked";

                    otArr3.push(obj);
                }

            }
        })

        var otArr4 = [];
        var tbl2 = $('#grid_gtcs tbody tr').each(function (i) {


            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {

                x = $(this).children().eq(0).find('input');
                // var c = x.is(':checked');
                var c = true;
                if (c == true) {
                    obj.GTC_Master_ID = x.val();
                    obj.Purchase_Order_No = $("#PurchaseOrderNo").val();

                    obj.ProjectId = $("#hdn_purchase_project_Id").val();
                    obj.Id = $(this).children().eq(1).find('input').val().trim();

                    //obj.IndentId = $(this).children().eq(0).text().trim();                       
                    obj.Header_Title = $(this).children().eq(2).find('input').val().trim();
                    obj.GTC_Description = $(this).children().eq(3).find('textarea').val().trim();
                    obj.Status = x.is(':checked') ? "Checked" : "UnChecked";
                    otArr4.push(obj);
                }

            }
        })

        debugger;
        var obj5 = new Object();

        obj5.ProjectId = $("#prjtid1 option:selected").val();
        obj5.Purchase_order_No = $("#PurchaseOrderNo").val();
        obj5.Delivery_Schedule = $("#Delivery_Schedule1").val();
        obj5.Delivery_Address = $("#Delivery_Address1").val();
        obj5.Billing_Address = $("#Billing_Address1").val();
      
        //obj5.Contact_Person_Name = $("#ddlPurchaseEmployePerson option:selected").text();
        //obj5.Contact_Person_Mobile = $("#contact_Person_Mobile1").val();

        obj5.Contact_Person_Name = $("#ddl_ContactPerson option:selected").text();
        obj5.EPContactPerID = $('#ddl_ContactPerson').val();
        obj5.Contact_Person_Mobile = $("#txt_Contact").val();
        obj5.ContactDivType = $('#ddl_Division').val();



        var session = {
            'Child': otArr,
            'Child2': otArr2,
            'Child3': otArr3,
            'Child4': otArr4,
            'Child5': obj5,
            'Master': obj1
        };
        return session;
    }

    function Valid() {

        var PurchaseOrderDate = $("#Date").val();
        var PODescription = $("#txtPODescription").val();
        var SupplierNo = $("#VNAME option:selected").val();
        var Vendor_Group_Id = $("#VGID option:selected").text();
        var Reference = $("#Reference1").val();

    


        var Remarks = $("#PORemarks").val();

        var Delivery_Schedules = $("#Delivery_Schedule1").val();

        //var DeliveryContactPerson = $("#ddlPurchaseEmployePerson option:selected").val();
        //var DeliveryContactpersonMobile = $("#contact_Person_Mobile1").val();

        var DeliveryContactPerson = $("#ddl_ContactPerson option:selected").val();
        var DeliveryContactpersonMobile = $("#txt_Contact").val();

        var Rupees = $("#Rupees").val();

        var SendTo = $("#Employee option:selected").text();

        var rr = true;

        if (PurchaseOrderDate == "") {
            alert('OrderDate field is empty');
            $('#Date').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Date').css('border-color', '');
        }


        if (Vendor_Group_Id == "Select Vendor Group") {
            alert('Vendor Group field is empty');
            $('#VGID').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#VGID').css('border-color', '');

        }

        if (SupplierNo == "") {
            alert('Vendor Name field is empty');
            $('#VNAME').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#VNAME').css('border-color', '');

        }


        if (Reference == "") {
            alert('Reference can not be Empty.');
            $('#Reference1').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Reference1').css('border-color', '');
        }

        if (PODescription == "") {
            alert('PODescription can not be Empty..')
            $('#txtPODescription').css('border-color', '#f0551b')
            rr = false;
        }
        else {
            $('#txtPODescription').css('border-color', '');
        }


        if (Remarks == "") {
            alert('Remarks can not be Empty.');
            $('#PORemarks').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#PORemarks').css('border-color', '');
        }

        if (Delivery_Schedules == "") {
            alert('Delivery Schedule field is empty');
            $('#Delivery_Schedule1').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Delivery_Schedule1').css('border-color', '');
        }

        if (Rupees == "") {
            alert('Amount in Word field is empty');
            $('#Rupees').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Rupees').css('border-color', '');
        }

        if (DeliveryContactPerson == "0") {
            alert('Select Delivery Contact Person Name.');
            $('#ddlPurchaseEmployePerson').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#ddlPurchaseEmployePerson').css('border-color', '');
        }

        if (DeliveryContactpersonMobile == "") {
            alert('Contact Person Mobile can not be empity.');
            $('#contact_Person_Mobile1').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#contact_Person_Mobile1').css('border-color', '');
        }








        if (rr == false) {


            return false;
        }
        else {

            return true;
        }
    }






    $("#prjtid1").prop('selectedIndex', 1).trigger('change');
    validteq();
    CalculateTotal();
    ItemWiseFullCalculation();

});

//GetFloor();
function GetFloor(Po_id) {

    $('#progress').show();
    $.ajax({
        url: UserGeneralTCPOWise,
        type: 'GET',
        data: { id: Po_id },
        complete: function () {
            $('#progress').hide();
        },
        success: function (result) {

            $('#Gtc_DatasLoad').html(result);
        }
    });
    return false;
    
}
//  GetSi();
function GetSi(Po_id) {

    $('#progress').show();
    $.ajax({
        url: UserSpecificInstruction,
        type: 'GET',
        data: { id: Po_id },
        complete: function () {
            $('#progress').hide();
        },
        success: function (result) {

            $('#Si_DatasLoad').html(result);
        }
    });
    return false;
    
}

//  GetSTC();
function GetSTC(Po_id) {

    $('#progress').show();
    $.ajax({
        url: userSpeceficTermsAndCondition,
        type: 'GET',
        data: { id: Po_id },
        complete: function () {
            $('#progress').hide();
        },
        success: function (result) {

            $('#STC_DatasLoad').html(result);
        }
    });
    return false;
    
}


function BindrecordThroughPurchaseIndentReq(xret) {
    if (xret != null) {
        $('#dvLoading').show(); debugger;

        $.ajax({
            type: 'GET',
            url: EditGridGST, // Calling json method
            data: { Uid: xret },
            async: false,
            // Get Selected Country ID.
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {

                $('#formbody').html(result);
                ClearValues();
            },
            error: function (ex) {
                alert('Failed to retrieve Project Code.' + ex);
            }
        });
        //return false;
    }
    else { $('#formbody').html(''); }
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

function GetVendor(data) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#VNAME").html(procemessage).show();
    var markup = "<option value='0'>Select Vendor</option>";
    for (var x = 0; x < data.length; x++) {


        //console.log(data[x].Value);
        if (data[x].Value != null)
            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

    }
    $("#VNAME").html(markup).show();

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

function BindRowFieldWithValueGST() {
    debugger;


    var ss = Child;
    
    debugger;
  


    $('div.single').find('div.second').each(function (i) {
        debugger;
        var s = ss[i];

        var jh = jQuery(this).data('dhdn');
        debugger;
        var selctval = null; 
         

      //  $('td.SrNo_' + jh).find('#IDesc_' + jh).val(s.Item_description);
        $('td.ctot_' + jh).find('#Ctotal_amount_' + jh).val(s.Total);
        $('td.tot_sub1_' + jh).find('#total_SubTotal_Amount1_' + jh).val(s.SubTotal1);
        $('td.tot_sub2_' + jh).find('#total_SubTotal_Amount2_' + jh).val(s.SubTotal2);
        $('td.tot_grnd_' + jh).find('#total_GrandTotal_Amount_' + jh).val(s.GrossAmount);
        $('td.CartageType_' + jh).find('#CartageTypeddl_' + jh).find('option[value="' + s.CartageTypeId + '"]').attr('selected', 'selected');

        $('td.CartageType_' + jh).find('#CartageTypeddl_' + jh).trigger('change');
        
        $('td.CartagerateValue_' + jh).find('#CartageRate1_' + jh).val(s.Cartage_rate);
        $('td.Cartageamt_' + jh).find('#CartageAmount1').val(s.CartageAmount);

        $('td.GSTSlab_' + jh).find('#ddlGSTSlabs_' + jh).find('option[value="' + s.TaxCode + '"]').attr('selected', 'selected');
        $('td.GSTSlab_' + jh).find('#ddlGSTSlabs_' + jh).trigger('change');

        $('td.CGSTPer_' + jh).find('#txtCGSTPerctg_' + jh).val(s.CGSTPercentage);
        $('td.SGSTOrUTGSTPer_' + jh).find('#txtSGSTOrUTGSTPerctg_' + jh).val(s.SGSTPercentage);
        $('td.IGSTPer_' + jh).find('#txtIGSTPerctg_' + jh).val(s.IGSTPercentage);

        $('td.CGSTPerAmtValue_' + jh).find('#txtCGSTPerAmtValue_' + jh).val(s.CGSTAmount);
        $('td.SGSTOrUTGSTPerAmtValue_' + jh).find('#txtSGSTOrUTGSTPerAmtValue_' + jh).val(s.SGSTAmount);
        $('td.IGSTPerAmtValue_' + jh).find('#txtIGSTPerAmtValue_' + jh).val(s.IGSTAmount);

        $('td.TotalGSTAmtValue_' + jh).find('#txtTotalGSTAmtValue_' + jh).val(s.TotalGSTAmount);
        $('td.TotalGrossAmtValue_' + jh).find('#txtTotalGrossAmtValue_' + jh).val(s.GrossAmount);

        // Binding all dropdownlist for yes or No by  above selecting value.

        





        //$('td.IsExcise_' + jh).find('#Isexcisedd-' + jh).trigger('change');
        //$('td.Insurance1_' + jh).find('#Insuranceddl1_' + jh).trigger('change');
        //$('td.IsCartageType_' + jh).find('#IsCar_tageddl_' + jh).trigger('change');
        //$('td.Insurance2_' + jh).find('#Insuranceddl2_' + jh).trigger('change');
        //$('td.IsTax_' + jh).find('#Isexciseddl_' + jh).trigger('change');
        //$('td.Insurance3_' + jh).find('#Insuranceddl3_' + jh).trigger('change');
        //$('td.Insurance4_' + jh).find('#Insuranceddl4_' + jh).trigger('change');

        // end here.


        // bind remaing textbox here

        debugger;

        //$('td.ExciseDutyValue_' + jh).find('#ExciseDutyValue_' + jh).val(s.ExciseDutyType);

        //$('td.ExciseAmtUpto_' + jh).find('#ExciseDutyAmtUptoValue_' + jh).val(s.ExcisepercentageAmt);

        //$('td.InsuranceValue1_' + jh).find('#Insurancerate1_' + jh).val(s.Insurance1Rate);
        //$('td.InsuranceAmtUpto1_' + jh).find('#InsuranceAmtUptoValue1_' + jh).val(s.Insurance1Amount);

        //$('td.CartagerateValue_' + jh).find('#CartageRate1_' + jh).val(s.Cartage_rate);
        //$('td.Cartageamt_' + jh).find('#CartageAmount1_' + jh).val(s.CartageAmount);

        //$('td.InsuranceValue2_' + jh).find('#Insurancerate2_' + jh).val(s.Insurancer_rate2);
        //$('td.InsuranceAmtUpto2_' + jh).find('#InsuranceAmtUptoValue2_' + jh).val(s.Insurance2_Amount);

        //$('td.taxValue_' + jh).find('#txtamtval_' + jh).val(s.TaxPercentage);
        //$('td.taxAmtUptoValue_' + jh).find('#txtamtvalupto_' + jh).val(s.Tax_Amount);


        //$('td.InsuranceValue3_' + jh).find('#Insurancerate3_' + jh).val(s.Insurancer_rate3);
        //$('td.InsuranceAmtUpto3_' + jh).find('#InsuranceAmtUptoValue3_' + jh).val(s.Insurance3_Amount);


        //$('td.CartagerateValue2_' + jh).find('#CartageRate2_' + jh).val(s.Cartage2_rate);
        //$('td.Cartageamt2_' + jh).find('#CartageAmount2_' + jh).val(s.CartageAmount2);



        //$('td.InsuranceTypeSel4_' + jh).find('#Insurancerate4_' + jh).val(s.Insurancer_rate4);
        //$('td.InsuranceAmtUptoTypeSel4_' + jh).find('#Insurancerate4amtUptoVal_' + jh).val(s.Insurance4_Amount);

 })

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

            $('td.CartagerateValue_' + Ctg_ID).find('#CartageRate1_' + Ctg_ID).val('');
            $('td.Cartageamt_' + Ctg_ID).find('#CartageAmount1').val(0);
            $(".caramt_" + Ctg_ID).prop('disabled', false); C1_lum();
        }

        else if ($(this).val() == "-1") {
            $(".CartagerateValue_" + Ctg_ID).find('input').val('');
            $(".Cartageamt_" + Ctg_ID).find('input').val('');
        }
        else if ($(this).val() == 2 || $(this).val() == 1 || $(this).val() == 1) {
            $(".CartagerateValue_" + Ctg_ID).find('input').prop('disabled', true);
            $('td.Cartageamt_' + Ctg_ID).find('input').val(PutZeroVal);
            $('td.CartagerateValue_' + Ctg_ID).find('#CartageRate1_' + Ctg_ID).val('');
            $('td.Cartageamt_' + Ctg_ID).find('#CartageAmount1').val(0);
            C1_lumAother();
        }



        //}
        //else if (checktxt == 'No') {

        //    $(".CartagerateValue_" + Ctg_ID).find('input').prop('disabled', true);
        //    $(".caramt_" + Ctg_ID).prop('disabled', true);
        //    $(".caramt_" + Ctg_ID).val(0);
        //}


    });


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

    $(document).on('change', '.gst-slab-ddl', function () {
        debugger;
        
        var gst_rowid = jQuery(this).data('gstval');
        var GST_code = $(this).val();
        var SubTot2 = $("#total_SubTotal_Amount2_" + gst_rowid).val();
        // var GetSplit_GST = BindGSTOnGRidItem;

        $.ajax({
            type: 'POST',
            url: BindGSTOnGRidItem,

            dataType: 'json',

            data: { TaxCode: GST_code },


            success: function (obj) {
                obj = $.parseJSON(obj)

                //$("#txtCGSTPerctg_" + gst_rowid).html(markup).show();
                $("#txtCGSTPerctg_" + gst_rowid).val(obj.tax_CGST);
                $("#txtSGSTOrUTGSTPerctg_" + gst_rowid).val(obj.tax_SGSTandUTGST);
                $("#txtIGSTPerctg_" + gst_rowid).val(obj.tax_IGST);
                CalculateAllGSTAmount(obj.tax_CGST, obj.tax_SGSTandUTGST, obj.tax_IGST, gst_rowid, SubTot2);


                //$("#PurchaseOrderNo").val(obj.No);
                //$("#ProjectAddress").val(obj.Address);

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

function RateSum() {
    debugger;
    var SumRt = 0; SumTotal = 0; SumTotpackingCharge = 0; SumTotCart1 = 0;
    var SumTotal1 = 0; SumOfInsurance = 0; SumSubTott2Sum = 0; SUMOfCGST = 0; SumOfSGSTAndUTGST = 0;
    SumOfIGST = 0; SumOfTotalGST = 0; SumofGrossAmount = 0, sumofTCSamt=0;

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

            //var S_ToGrossAmt = $('td.TotalGrossAmtValue_' + Id).find('#txtTotalGrossAmtValue_' + Id).val();
            //if (!isNaN(S_ToGrossAmt) && S_ToGrossAmt != null) {
            //    S_ToGrossAmt = S_ToGrossAmt;
            //}
            //else {
            //    S_ToGrossAmt = 0;
            //}

            var S_ToGrossAmt = $('td.TotalGrossAmtValue_' + Id).parents('tr').find('input.gross_amt').val();
            if (!isNaN(S_ToGrossAmt) && S_ToGrossAmt != null) {
                S_ToGrossAmt = S_ToGrossAmt;
            }
            else {
                S_ToGrossAmt = 0;
            }

             var s_ToTCS = $('td.TotalGSTAmtValue_' + Id).parents('tr').find('input.tcs_amt').val();
            if (!isNaN(s_ToTCS) && s_ToTCS != null) {
                s_ToTCS = s_ToTCS;
            }
            else {
                s_ToTCS = 0;
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

              sumofTCSamt = parseFloat(sumofTCSamt) + parseFloat(s_ToTCS);

            // SumTotpackingChargegolbal = SumTotpackingCharge +SumTotpackingChargegolbal ;


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

            $("#SumOftotal_TCSAmt").val(sumofTCSamt.toFixed(2))
        }

    });

    testSkill();
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
    //alert(DiscountedRate)
    $('td.CRate_' + Id).find('#hdndiscountedRate_' + Id).val(DiscountedRate.toFixed(3));


 
    x = $('td.PackingchargePercentage_' + Id).find('#txtpackingChargePercentage_' + Id).val(); if (isNaN(x) || x == '') { x = 0; } else { x = x; }
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
    var tcsrate = $('td.TotalGrossAmtValue_' + Id).parents('tr').find('select.dll_TCS').val();
    var tcsamt = parseFloat((R * tcsrate) / 100);
    $('td.TotalGrossAmtValue_' + Id).parents('tr').find('input.tcs_amt').val(tcsamt.toFixed(3));
    R += tcsamt;
    $('td.TotalGrossAmtValue_' + Id).parents('tr').find('input.gross_amt').val(R.toFixed(3));
    $("#total_GrandTotal_Amount_" + Id).val('');
    $("#total_GrandTotal_Amount_" + Id).val(R.toFixed(2));


}


////////TCS Slab On Gross Amount/////////////////////

$('body').on('change', '.dll_TCS', function () {
    var rate = $(this).val();
    var gamtwotcs = $(this).parents('tr').find('input.gamtwotcs').val();
    var tcsrateamt = (gamtwotcs * rate) / 100;
    var grossamttcs = parseFloat(gamtwotcs) + tcsrateamt;
    $(this).parents('tr').find('input.tcs_amt').val(tcsrateamt.toFixed(2));
    $(this).parents('tr').find('input.gross_amt').val(parseFloat(grossamttcs).toFixed(2));
});

///////////////end///////////////////////////////