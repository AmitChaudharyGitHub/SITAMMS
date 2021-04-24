

$(document).ready(function () {
   
    function valimas()
    {
        debugger;

        if ($('#hdnTypeSelect').val() == "Mang") {
          
            var remrks = $('textarea#txtPICRemarks').val();
            if (remrks == "") {
                $('textarea#txtPICRemarks').css('border-color', '#f0551b');
                return false;
            }
            else {
                $('textarea#txtPICRemarks').css('border-color', '');
            }
        }
        else if ($('#hdnTypeSelect').val() == "PIC") {

              var Limtval = $("#lblprojectValue").text();
              var Totalval = $("#lblTotalValue").text();
             if (parseFloat(parseFloat(Totalval).toFixed(2)) >= parseFloat(parseFloat(Limtval).toFixed(2))) {

                 var ForwardToMang = $('#ddlForwardTo option:selected').text();
                 var remrks = $('textarea#txtPICRemarks').val();
                 if (remrks == "") {
                     $('textarea#txtPICRemarks').css('border-color', '#f0551b');
                     return false;
                 }
                 else {
                     $('textarea#txtPICRemarks').css('border-color', '');
                 }

                 if (ForwardToMang == "Select Forward To") {
                     $('#ddlForwardTo').css('border-color', '#f0551b');
                     return false;
                 }
                 else {
                     $('#ddlForwardTo').css('border-color', '');

                 }
             }


            

        }

      

       
    }


    function getMaster() {
        

        var obj1 = new Object();

        obj1.UId = $(".MasterId").attr('muid');
        obj1.ForwardToMang = $('#ddlForwardTo option:selected').val();
        obj1.PICRemarks = $('textarea#txtPICRemarks').val();
        obj1.PurchasePIC_Type = $('#ddlPurchaseType option:selected').val();
        obj1.DeliveryDate = $("#PurchaseIRDeliveryDate").val();
        obj1.Comp = $("#ddlPurchasedecisionType option:selected").val();
        return obj1;
    }

    function LPToArray() {
       
        var otArr = [];
        var ValidChild = false;
        var tbl2 = $('#gridD tbody tr').each(function (i) {
            // alert("A");
            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {
                x = $(this).children();
                obj.ApprovedQty = $(this).children().eq(7).find('input').val();
                if (obj.ApprovedQty != null) {
                    ValidChild = true;
                    obj.UId = $(this).children().eq(0).attr('uid'); 
                    //obj.ApprovedQty = $(this).children().eq(5).find('input').val();
                    //obj.ApprovedQty = +(obj.ApprovedQty) + +(obj.CurrentQty);
                    obj.ItemGroupNo = $(this).children().eq(1).find('input').val();
                    obj.ItemNo = $(this).children().eq(2).find('input').val();
                    obj.Remarks = $(this).children().eq(3).find('textarea#Remarks').val();
                    obj.UnitNo = $(this).children().eq(4).find('input').val();
                    obj.CurrentRate = $(this).children().eq(8).find('input').val();
                    var piccurntval = $(this).children().eq(9).find('input').val();
                    if (piccurntval >= 0.00) {  obj.PICCurrentValue = $(this).children().eq(9).find('input').val(); } else { $(this).children().eq(9).find('input').css('border-color', '#f0551b'); ValidChild = false; }
                    obj.PICStatus = "Approved";
                    obj.MangStatus = "Approved";
                    obj.ApprovedQtyPIC = obj.ApprovedQty;
                    obj.ApprovedQty = obj.ApprovedQty;
                    obj.ApprovedQtyMang = obj.ApprovedQty;
                    



                    otArr.push(obj);
                    if (valimas() == true) { 
                    var app = 0;
                    $(this).children().eq(7).find('input').val('');
                    app = ($(this).children().eq(6).find('input').val());
                    var c = +app + +obj.ApprovedQty;
                    $(this).children().eq(6).find('input').val(c);
                    }


                }
            }
        })
        return [ValidChild, otArr];

    }
    $("#SubmitA").click( function (e) {
        //alert("A");
        //$("#Submit").click(function () {
      



        var otArr = [];
        var values = LPToArray();

        Valid = values[0];
        
        console.log(Valid);

       // alert(otArr);
        

        var validout = valimas();
        if (Valid == false || validout == false)
            return;
         otArr = values[1];
         var obj1 = getMaster();
        
        
       // alert(obj1);
        var session =
            {
                'Childk': otArr,
                'Masterk': obj1
            };

       // alert(session);

        var url = UpdateList2;
      //  alert(url);
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
                    if (json == "1") {
                        //alert("Save Successfully");
                        // $("#dialog-view").empty();
                        // $("#dialog-view").dialog('close');
                       // $(this).dialog('close');
                        $("#myModal").modal('show');
                        window.location.href = Redurl;

                     
                    }

                },
                error: function () {
                    alert('Error in Submit Data');
                }
                // When Service call fails
            })
    })

   

    $(".DatePicker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: true,
        changeYear: true,
        value: "",
        maxDate: new Date()
    });


    $(".Date_Picker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: true,
        changeYear: true,
        value: ""

    });


})



$(document).ready(function () {

  

    var element = document.getElementById('ddlPurchaseType');
    var type = $("#purchadeid").val();
    var selType = $("#purchaseDeceion-id").val();

    var elementdescision = document.getElementById('ddlPurchasedecisionType');

    

    $(window).load(function () {

        $.ajax({
            type: 'Get',
            url: GetForwardTo, // Calling json method
            success: function (data) {


                $.each(data, function (i, itname) {
                    $("#ddlForwardTo").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');



                });



            },
            error: function (ex) {
                alert('Failed to retrieve User Type.' + ex);
            }
        });

        $.ajax({
            type: 'Get',

            url: GetPurchasRequestionType, // Calling json method
            success: function (data) {


                $.each(data, function (i, itname) {
                    $("#ddlPurchaseType").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');



                });
                element.value = type;

            },
            error: function (ex) {
                alert('Failed to retrieve Purchase Type.' + ex);
            }
        });

        $.ajax({
            type: 'Get',
            url: GetPurchasdecesionType, // Calling json method
            success: function (data) {


                $.each(data, function (i, itname) {
                    $("#ddlPurchasedecisionType").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');



                });

                elementdescision.value = selType;

            },
            error: function (ex) {
                alert('Failed to retrieve Purchase Type.' + ex);
            }
        });

        //GetLoadFixType();


    });




    //$("#ddlForwardTo").change(function () {


    if ($('#hdnTypeSelect').val() == "Mang") {
        $("#ddlForwardTo").hide();
        $("#SubmitA").show();
        //  $("#kkPurchagetypehide").hide();
        $("#Rmrks").show();
        var elem = document.getElementById("SubmitA");
        elem.value = "Approve"


    }
    else if ($('#hdnTypeSelect').val() == "PIC") {
        $("#ddlForwardTo").show();
        $("#SubmitA").show();
        $("#kkPurchagetypehide").show();
        $("#Rmrks").show();
    } else if ($('#hdnTypeSelect').val() == "User") {
        $("#kkPurchagetypehide").show();
        $("#ddlPurchaseType").prop('disabled', true);
        $("#PurchaseIRDeliveryDate").prop('disabled', true);
        $("#ddlForwardTo").hide();
        $("#SubmitA").hide();
        $("#Rmrks").hide();
    }
    //});

    GetTotalValue(); $("#kknote").hide(); $("#kkhide").hide(); enabledisable();

   

    $(document).on('keyup', "td[class^='Current_']", function () {

        var ApprovQty = $(this).find('input').val();
        var str = $(this).attr('class');
        var a = str.substring(7);
        //   var kk = $(".Rate" + a);

        var Rate = $(".Rate" + a).find('input').val();
        if (Rate != null && ApprovQty != null) {
            var temp = ApprovQty * Rate;
            var Amount = temp.toFixed(2);
            $(".Amount" + a).find('input').val(temp.toFixed(2));

        }

        GetTotalValue();
        CompareProjectLimitValWithTotalValue();
        //$("td[class^='Current_']").trigger('keyup');

    });

    function GetTotalValue() {

        var T = 0;
        $("td[class^='Amount_").each(function () {
        
            if ($(this).find('input').val() != null) {
                var str = $(this).attr('class');
                var a = str.substring(7);
                var A = $('td.Amount_' + a).find('input').val();
                // var A = $(this).find('input').val()
                T = +T + +A;
            }
        });

        $("#lblTotalValue").text(T.toFixed(2));
      
    }

    function CompareProjectLimitValWithTotalValue() {
        debugger;
        var Limtval = $("#lblprojectValue").text();
        var Totalval = $("#lblTotalValue").text();


        if ($('#hdnTypeSelect').val() == "PIC") {
            if (parseFloat(parseFloat(Totalval).toFixed(2)) >= parseFloat(parseFloat(Limtval).toFixed(2))) {

                $("#kkhide").show();
                $("#kknote").show();

                var elemll = document.getElementById("SubmitA");
                elemll.value = "Approve and Forward"

            }
            else {
                $("#kkhide").hide();
                $("#kknote").hide();

                var elemll = document.getElementById("SubmitA");
                elemll.value = "Approve"
            }



        }
        else if ($('#hdnTypeSelect').val() == "Mang") {
            $("#kkhide").hide();
            $("#kknote").hide();

            var elemll = document.getElementById("SubmitA");
            elemll.value = "Approve"

        }




    }

    function enabledisable() {
        $("td[class^='Current_").each(function () {
            debugger;
            // var CurrentQty = $(this).find('input').val();
            var str = $(this).attr('class');
            var a = str.substring(7);
            var kk = $("ApprovedQty" + a);
            var approvqty = $("#ApprovedQty" + a).find('input').val();

            $(this).find('input').val(approvqty);

            var CurrentQty = $(this).find('input').val();

            var Rate = $(".Rate" + a).find('input').val();
            if (Rate != null && CurrentQty != null) {
                var temp = CurrentQty * Rate;
                var Amount = temp.toFixed(2);
                $(".Amount" + a).find('input').val(temp.toFixed(2));

            }




            if ($('#hdnTypeSelect').val() == "User") {

                $(this).find('input').prop("disabled", true);
            }
            else if ($('#hdnTypeSelect').val() == "PIC") {
                $(this).find('input').prop("disabled", false);
            }
            else if ($('#hdnTypeSelect').val() == "Mang") {
                $(this).find('input').prop("disabled", false);
            }
        });
    }



    FixType();



});



function FixType() {

    $("#ddlPurchasedecisionType").prop('disabled', true);
    $(document).on('change', '.set_delvry', function () {


        debugger;
        var delvdate = $("#PurchaseIRDeliveryDate").val();
        var Pidate = $("#PurchaseIRDate").val();
        var startdt = GetDate(Pidate);
        var enddt = GetDate(delvdate);


        var startdt_new = new Date(startdt.split('/')[2], startdt.split('/')[1] - 1, startdt.split('/')[0]);


        var enddt_new = new Date(enddt.split('/')[2], enddt.split('/')[1] - 1, enddt.split('/')[0]);

        var timeDiff = Math.abs(enddt_new.getTime() - startdt_new.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        // alert(diffDays);


        if (enddt_new.getTime() >= startdt_new.getTime()) {
            debugger;

            //  $("#ddlPurchaseType").prop('disabled', false);
            var SiteLimitVal = $('#hdn_sitepurchaseLimitValue').val();
            var SiteID = $('#hdn_sitepurchasevalue').val();
            var HOLimitValue = $('#hdn_HOepurchaseLimitValue').val();
            var HOId = $('#hdn_hopurchasevalue').val();
            BindDecisionDrpDown();
            // clearSelected();
            if (diffDays <= SiteLimitVal) {

                $("#ddlPurchasedecisionType").find('option[value="' + SiteID + '"]').attr('selected', true);

            }
            else if (diffDays > SiteLimitVal && diffDays <= HOLimitValue) {


                $("#ddlPurchasedecisionType").find('option[value="' + HOId + '"]').attr('selected', true);


            }


            return false;

        }


        else {
            clearSelected();
            alert("Delivery Date should be always greater than Puchase Indent date.");
            $("#PurchaseIRDeliveryDate").val('');
            $("#ddlPurchasedecisionType").prop('disabled', true);
            return false;
        }


    });
    


}

function GetDate(str) {
    
    var arr = str.split(' ');
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    var i = 1; var j = 1;
    for (i; i <= months.length; i++) {
        if (months[i] == arr[1]) {
            j = i + 1;
            break;
        }
    }

    var formatddate = arr[0] + '/' + j + '/' + arr[2];
    return formatddate;
}

function clearSelected() {
    
    var elements = document.getElementById("ddlPurchasedecisionType").options;

    for (var i = 0; i < elements.length; i++) {
        if (elements[i].selected)
            elements[i].selected = false;
    }
}

function BindDecisionDrpDown() {
    $.ajax({
        type: 'Get',
        url: GetPurchasdecesionType, // Calling json method
        success: function (data) {


            $.each(data, function (i, itname) {
                $("#ddlPurchasedecisionType").append('<option value="' + itname.Value + '">' +
                     itname.Text + '</option>');



            });



        },
        error: function (ex) {
            alert('Failed to retrieve Purchase Type.' + ex);
        }
    });
}








