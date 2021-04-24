

    
    $(document).ready(function () {
    
        window['lol'] = function() { alert('lol');   };

        window['GetPrj'] = function (dateString)
        { 
            // var ss = @Html.Raw(Json.Encode(ViewBag.EmpId));
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetProjectByEmpId, // Calling json method

                dataType: 'json',
                data: { E: ss},
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
                    //$("#Projects").prop('selectedIndex', 1);$("#prjtid1")
                    $("#Projects").prop('selectedIndex', 1).trigger('change');
                    $("#prjtid1").prop('selectedIndex', 1).trigger('change');
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
        
    // alert("Proj Change");
    //Country Dropdown Selectedchange event
    $("#Projects").change(function () {
        if($("#Projects option:selected").val()!=0)
        {
            $("#PurchaseOrderNo").empty();
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetProjectPurchaseOrderNo, // Calling json method

                dataType: 'json',

                data: { PRJID: $("#Projects").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)
                    //  var r = toArray(obj);ProjectAddress


                    $("#PurchaseOrderNo").val(obj.No);
                    $("#ProjectAddress").val(obj.Address);
                    GetEmp1(obj.List);
                    GetInd1(obj.List1);
                },
                error: function (ex) {
                    // alert('Failed to retrieve Project Code.' + ex);
                }
            });
            return false;}else
        { var markup = "<option value='0'>Send To</option>";
               
            $("#Employee").html(markup).show();
        }
    })
});
function GetEmp1(data) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#Employee").html(procemessage).show();
    var markup = "<option value='0'>Send To</option>";
    for (var x = 0; x < data.length; x++) {


        //console.log(data[x].Value);

        markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

    }
    $("#Employee").html(markup).show();

}
function GetInd1(data) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#Indents").html(procemessage).show();
    var markup = "<option value='0'>Select Indent</option>";
    for (var x = 0; x < data.length; x++) {


      

        markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

    }
    $("#Indents").html(markup).show();

}
$(document).ready(function () {
    //Country Dropdown Selectedchange event

    $("#VGID").change(function ()
    { if($("#VGID option:selected").val()!=0)
    {
        $('#dvLoading').show();
        $("#VNAME").empty();
        $.ajax({
            type: 'POST',
            url: GetVendorByGroup, // Calling json method

            dataType: 'json',

            data: { Vid: $("#VGID").val(), Pid: $("#Projects").val() },
            // Get Selected Country ID.
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (obj) {
                obj = $.parseJSON(obj)



                // console.log(obj)
                GetVendor(obj);

            },
            error: function (ex) {
                // alert('Failed to retrieve Project Code.' + ex);
            }
        });
        return false;
    }else
    { var markup = "<option value='0'>Select Vendor</option>";
               
        $("#VNAME").html(markup).show();
    }
    })
});
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
$(document).ready(function () {
    //Country Dropdown Selectedchange event
    $("#VNAME").change(function () {
        if($("#VNAME option:selected").val()!=0)
        {
            $("#SupplierAddress").val('');
            $("#SupplierTinNo").val('');
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetVendorDetail, // Calling json method

                dataType: 'json',

                data: { ID: $("#VNAME").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)
                    //  var r = toArray(obj);ProjectAddress


                    $("#SupplierAddress").val(obj.Address);
                    $("#SupplierTinNo").val(obj.TinNo);
                    

                },
                error: function (ex) {
                    // alert('Failed to retrieve Project Code.' + ex);
                }
            });
            return false;
        }
        else{$("#SupplierAddress").val('');
            $("#SupplierTinNo").val('');}
    })
});



$(document).ready(function () {
    //Country Dropdown Selectedchange event
    $("#Indents").change(function () {
        if($("#Indents option:selected").val()!=0)
        {
            $('#dvLoading').show();
            $.ajax({
                type:  'GET',
                url: Grid, // Calling json method

              

                data: { IndentNo: $("#Indents").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (result) {
                 
                    $('#formbody').html(result);
                    ClearValues() ;
                },
                error: function (ex) {
                    alert('Failed to retrieve Project Code.' + ex);
                }
            });
            return false;}else{ $('#formbody').html('');}
    })





    function ClearValues() 
    {
        $("#Total").val('');
        $("#Vat").val('');
        $("#VatAmount").val('');
        $("#SubTotal").val('');
        $("#SurCharge").val('');
        $("#Cartage").val('');
        $("#GrandTotal").val('');
        $("#Rupees").val('');
    }
});



    
$(document).ready(function () {
    //  alert('testing');
    $(document).on('click','span.checkVal',function(){
        // alert($(this).attr('id'));
        //var getQty = $(this).data('qtyid'); 
        // alert(  $(this).find('input').val());
        $(this).find('input').val();

        var str=$(this).attr('id');
        //alert( str.substring(8));
        var a= str.substring(8);
        // alert(   $(".Qty_"+a).find('input').val());
    });
        
    $(document).on('click',"td[class^='Item_']",function(){
        alert($(this).attr('class'));
        alert( $(this).text()); 
        alert( $(this).val()); 
        // alert(  $(this).find('input').val());
        $(this).find('input').val();

        var str=$(this).attr('id');
        //alert( str.substring(8));
        var a= str.substring(8);
        // alert(   $(".Qty_"+a).find('input').val());
    });
    //alert(   $(".Qty_"+a).find('input').val());  "input[name^='news']" "td[class^='Qty_']" $('*[class^="text"]') 
        
    //Country Dropdown Selectedchange event
    // document.getElementById("#Qty_").change(function () {
    //    alert("Round");
    // Getgt();


});

$(document).ready(function () {
    var mytst =0;
    $(document).on('keyup', "td[class^='Qty_']", function () {

        var qty=$(this).find('input').val();
        var str=$(this).attr('class');
        var a = str.substring(4);
            
        var Balance= $("#Balance_"+a).find('input').val();
        var Rate=$(".Rate_"+a).find('input').val();
        if(+qty>+Balance)
        { 
             
            alert("Quantity Can't Greater than Remaining");
            $(this).find('input').val('');
        }
        // alert(qty + "" + Rate);
        if(Rate!=null&&qty!=null)
        { var temp1=qty * Rate;
            var Amount = temp1.toFixed(2);
            $(".Amount_"+a).find('input').val(temp1.toFixed(2));
        }
        GetTotal()  ;
        GetSubTotal()  ;
        GetGrandTotal()  ;
        //alert(   $(".Qty_"+a).find('input').val());  "input[name^='news']" "td[class^='Qty_']" $('*[class^="text"]')
    });


    $(document).on('keyup', "td[class^='Rate_']", function () {
           
        var Rate=$(this).find('input').val();
        var str=$(this).attr('class');
        var a = str.substring(5);
            
        var cmrts = $("#RatingsCompares").val();
            

        var Qty= $(".Qty_"+a).find('input').val();
        // alert(Qty + "" + Rate);
        if(Rate!=null&&Qty!=null)
        { var temp1=Qty * Rate;
            var Amount = temp1.toFixed(2);
            $(".Amount_"+a).find('input').val(temp1.toFixed(2));
        }
        if (+cmrts < +Rate)
        {               
            alert("Rate Can't Greater than Fixed Rate");
            $(this).find('input').val(cmrts);
        }

        GetTotal()  ;
        GetSubTotal()  ;
        GetGrandTotal()  ;
              
    });
    $(document).on('keyup',"td[class^='Discount_']",function(){
           
                         
        var Discount = $(this).find('input').val();
           
        var str=$(this).attr('class');
        var a=str.substring(9);
        var Qty= $(".Qty_"+a).find('input').val();
        var Rate= $(".Rate_"+a).find('input').val();
        if(Rate!=null&&Qty!=null&&Discount!=null)
        { var temp1=Qty * Rate;
            var Amount = temp1.toFixed(2);
            Amount = Amount - (Amount * Discount/100);
            $(".Amount_"+a).find('input').val(Amount.toFixed(2));
        }
        GetTotal()  ;
        GetSubTotal()  ;
        GetGrandTotal()  ;
              
    });
    $(document).on('keyup', "td[class^='Vat_']", function () {
            
            
        var Vat = $(this).find('input').val();
        var str = $(this).attr('class');
        var a = str.substring(4);
        var Amount = $(".Amount_" + a).find('input').val();
        //var Rate = $(".Rate_" + a).find('input').val();
        if (Amount != "" && Vat != "") {

            var temp1 = (Amount * Vat) / 100;
            var vat = temp1.toFixed(2);
            var SubAmount = +Amount + +vat;
            $(".SubTotal_" + a).find('input').val(SubAmount.toFixed(2));
        }
        else {
            alert('Vat field is required');
        }
        GetTotal();
        GetSubTotal();
        GetGrandTotal();

    });


    $(document).on('change',"td[class^='Amount_']",function()
    {
        GetTotal()  ;
              
              
              
         
          
              
    });

    $(document).on('click',"#Total" ,function()
    {
             
              
              
        GetTotal()  ;
         
          
              
    });
    $(document).on('keyup',"#SurCharge" ,function()
    {
             
              
              
        GetGrandTotal()  ;
         
          
              
    });
    $(document).on('keyup',"#cartage" ,function()   // here change from #cartage to CartageRate
    {
             
              
              
        GetGrandTotal()   ;
         
          
              
    });
    $('#btnCalc').click(function (e)
    {//alert("A");
        GetTotal() ;
        GetSubTotal() ;
        GetGrandTotal() ;
    });
    function GetTotal() 
    {
        var T = 0; Topi = 0;
             
        $("td[class^='SubTotal_").each(function () {
                
            if($(this).find('input').val()!=null)
            { 
                var A= $(this).find('input').val()
                T=+T + +A;
            }
        });
        
        $("#Total").val(T.toFixed(2));
        //alert(T.toFixed(2))
        //"td[class^='Discount_']"

        $("td[class^='Qty_']").each(function () {
             
            if ($(this).find('input').val() != null) {
                var Aop = $(this).find('input').val()
                Topi = +Topi + +Aop;
            }
        });  

        $("#Hidden1").val(Topi);
        // document.getElementById('Hidden1').value = Topi.toFixed(2);
        var hv = $('#Hidden1').val();
        //alert(hv);
        // alert(Topi.toFixed(2))

    }






    function GetSubTotal() 
    {
        //alert("GetSubTotal");
        //var VA=0;
        //var ST=0;      
        //var T=$("#Total").val();
        //var V=$("#Vat").val();

        //VA= +(T * V/100);
        //ST=+T + +VA;
        //$("#VatAmount").val(VA.toFixed(2));
        //$("#SubTotal").val(ST.toFixed(2));
    }
    function GetGrandTotal() 
    {
        //alert("GetSubTotal");
        var Td =0;
        var currVal =0;
        var GT=0;
        var T=0; 
        var S=0;
        var C=0;
        if($("#Total").val()!=null)
            T=$("#Total").val();
        if($("#SurCharge").val()!=null)
            S=$("#SurCharge").val();

        if($("#Cartage").val()!=null) // here change from #Cartage to CartageRate
            C=$("#Cartage").val(); // here change from #Cartage to CartageRate

        GT=+T + +S + +C; 
        // GT=+T + +S + +Td; 
        $("#GrandTotal").val(GT.toFixed(2));
    }

    $("#ddlCart").change(function () {
        //mytst
        var currVal=0;
        var Tid =0;
        if($("#CartageRate").val() !=null)
        {
            if($("#ddlCart").val() == "1")
            {
                $("#CartageRate").attr('readonly',false);
                if($('#Hidden1').val() !=null)
                    currVal =$('#Hidden1').val();
                Tid = currVal * $("#CartageRate").val();
                    
                   

            }
            else if($("#ddlCart").val() == "2")
            {
                $("#CartageRate").attr('readonly',false);
                Tid = $("#CartageRate").val();
            }
            else if($("#ddlCart").val() == "3")
            {
                $("#CartageRate").attr("readonly", true);
                Tid = 0;
                $("#CartageRate").val(Tid);
            }
            else{
                $("#CartageRate").attr('readonly',false);
            }
        }
        //  document.getElementById('<%=Cartage.ClientID%>').value = Tid;
        $("#Cartage").val(Tid) ;
        mytst = Tid;GetGrandTotal() ;
        // alert($('#Hidden1').val())

        //alert($("#CartageRate").val())
    });
    //$(document).on('keyup',"#Vat",function()
    //{
    //    GetSubTotal() ;
              
    //});


});







$(document).ready(function () {
    $(".DatePicker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: true,
        changeYear: true,
        value:"",
    }).val('');

});


$(document).ready(function ()
{
        
    $('#btnsave').click(function (e)
    {                      
            
        var V=   Valid();
            
        if(V==false)
            return;
        //debugger;
        var _griddata = gridToArray();
        //console.log(_griddata);
        //console.log(otArr);
        var url = AddM_AddC;
        $.ajax({
            type: 'POST',
            url: url,           
            data: JSON.stringify(_griddata),
            contentType: "application/json; charset=utf-8",	
            dataType: "json" ,
            processdata: true,
      
            success: function (json) 
            {
                    
                    
                if (json.Status == "1")
                {
                        
                        

                    alert("Data Saved Successfully");
                    // $("#myModal").modal('show');
                    url: json.redidtUrl,


                    $("#Date").datepicker("setDate", "");
                    $("#PurchaseOrderNo").val('');
                    $("#SupplierAddress").val('');
                    $("#SupplierTinNo").val('');
                    $("#Projects").prop('selectedIndex', 0);   
                    $("#VGID").prop('selectedIndex', 0);   
                    $("#VNAME").prop('selectedIndex', 0);    
                    
                    $("#Indents").prop('selectedIndex', 0);       
                    $('#formbody').html('');

                    $("#Total").val('');
                    $("#Vat").val('');
                    $("#VatAmount").val('');

                    $("#SubTotal").val('');
                    $("#SurCharge").val('');
                    $("#Cartage").val('');
                    $("#GrandTotal").val('');
                    $("#Rupees").val(''); 
                    $("#Employee").prop('selectedIndex', 0);

                       
                    window.location.href = logoutUrl;

                    // location.href = "/View_PO_Details_ByUser_Created/Index";


                    //window.location.href = '@Url.Action("Index", "View_PO_Details_ByUser_Created")';
                    // location.href = "/Print_PO_Slip/Index?IndentNo=" + json.IndentNo;
                   
                }
                   
            },
               
                
               
              


            
            error: function ()
            {
                alert('Error in Submit Data');
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
        var tbl2 = $('#webgrid tbody tr').each(function (i)
        {
           
                
            var obj = new Object();
            if ($(this)[0].rowIndex != 0)
            {  var SubTotal=0;
                x = $(this).children();
                var SubTotal = $(this).children().eq(11).find('input').val()              

                if (SubTotal != 0 && SubTotal != null)
                {
                        
                    obj.IndentId=  $(this).children().eq(0).text().trim()  ;
                    obj.ItemNo = $(this).children().eq(1).text().trim();                     
                    obj.Qty = $(this).children().eq(6).find('input').val();
                    obj.Rate = $(this).children().eq(7).find('input').val();
                    obj.Discount = $(this).children().eq(8).find('input').val();
                    obj.Amount = $(this).children().eq(9).find('input').val();
                    obj.Vat = $(this).children().eq(10).find('input').val();
                    obj.Remark = $(this).children().eq(12).find('textarea').val();
                    obj.SubTotal = SubTotal;
                    otArr.push(obj);
                }
            }
        })

            
        var obj1 = new Object();
           
        obj1.PurchaseOrderDate = $("#Date").val();
        obj1.PurchaseOrderNo = $("#PurchaseOrderNo").val();

        obj1.ProjectNo = $("#Projects option:selected").val();
        obj1.Vendor_Group_Id = $("#VGID option:selected").val();
            
        obj1.SupplierNo = $("#VNAME option:selected").val();

        //for code here refereces
        obj1.Reference = $("#Reference1").val();

        obj1.IndentRefNo = $("#Indents option:selected").text();       
        obj1.AcilTinNo = $("#AcilTinNo").val();

        obj1.Total = $("#Total").val();
        //obj1.Vat = $("#Vat").val();
        //obj1.VatAmount = $("#VatAmount").val();

        //obj1.SubTotal = $("#SubTotal").val();
        obj1.SurCharge = $("#SurCharge").val();
        obj1.Cartage = $("#Cartage").val();
        obj1.GrandTotal = $("#GrandTotal").val();
        obj1.Rupees = $("#Rupees").val(); 
        obj1.SendTo = $("#Employee option:selected").val();
        obj1.Cartage_Rate = $("#CartageRate").val();
        obj1.Cartage_Type = $("#ddlCart option:selected").text();
        
        var otArr2 = [];
        var tbl2 = $('#grid tbody tr').each(function (i) {

                
            var obj = new Object();
            if ($(this)[0].rowIndex != 0)
            {
                    
                x = $(this).children().eq(0).find('input');
                var c = x.is(':checked');
                // var c = x.attr("checked");
                //if (x.checked == "checked")
                if (c == true)
                {
                    obj.STC_Master_ID = x.val();
                    obj.Purchase_Order_No = $("#PurchaseOrderNo").val();

                    obj.ProjectId = $("#Projects option:selected").val();

                    //obj.IndentId = $(this).children().eq(0).text().trim();                       
                    obj.HeaderTitle = $(this).children().eq(2).find('input').val().trim();
                        
                    obj.STC_Description = $(this).children().eq(3).find('textarea').val().trim();
                      
                    otArr2.push(obj);
                }
                   
            }
        })
       
        var otArr3 = [];
        var tbl2 = $('#grid2 tbody tr').each(function (i) {

               
            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {

                x = $(this).children().eq(0).find('input');
                var c = x.is(':checked');
                // var c = x.attr("checked");
                //if (x.checked == "checked")
                if (c == true) {
                    obj.SI_Master_ID = x.val();
                    obj.Purchase_Order_No = $("#PurchaseOrderNo").val();

                    obj.ProjectId = $("#Projects option:selected").val();

                    //obj.IndentId = $(this).children().eq(0).text().trim();                       
                    obj.Header_Title = $(this).children().eq(2).find('input').val().trim();
                    obj.SI_Description = $(this).children().eq(3).find('textarea').val().trim();

                    otArr3.push(obj);
                }

            }
        })

        var otArr4 = [];
        var tbl2 = $('#grid_gtcs tbody tr').each(function (i) {

                
            var obj = new Object();
            if ($(this)[0].rowIndex != 0) {

                x = $(this).children().eq(0).find('input');
                var c = x.is(':checked');
                // var c = x.attr("checked");
                //if (x.checked == "checked")
                if (c == true) {
                    obj.GTC_Master_ID = x.val();
                    obj.Purchase_Order_No = $("#PurchaseOrderNo").val();

                    obj.ProjectId = $("#Projects option:selected").val();

                    //obj.IndentId = $(this).children().eq(0).text().trim();                       
                    obj.Header_Title = $(this).children().eq(2).find('input').val().trim();
                    obj.GTC_Description = $(this).children().eq(3).find('textarea').val().trim();

                    otArr4.push(obj);
                }

            }
        })

           
        var obj5 = new Object();

        obj5.ProjectId = $("#Projects option:selected").val();
        obj5.Purchase_order_No = $("#PurchaseOrderNo").val();
        obj5.Delivery_Schedule = $("#Delivery_Schedule1").val();
        obj5.Delivery_Address = $("#Delivery_Address1").val();
        obj5.Billing_Address = $("#Billing_Address1").val();
        obj5.Contact_Person_Name = $("#contact_Person_Info1").val();
        obj5.Contact_Person_Mobile = $("#contact_Person_Mobile1").val();



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
        var PurchaseOrderNo = $("#PurchaseOrderNo").val();

        var ProjectNo = $("#Projects option:selected").val();     
        var SupplierNo = $("#VNAME option:selected").val();
        var Vendor_Group_Id = $("#VGID option:selected").val();
        

        var IndentRefNo = $("#Indents option:selected").val();       
        var AcilTinNo = $("#AcilTinNo").val();

        var Total = $("#Total").val();
        //var Vat = $("#Vat").val();
        //var VatAmount = $("#VatAmount").val();

        //var SubTotal = $("#SubTotal").val();
        var SurCharge = $("#SurCharge").val();
        var Cartage = $("#Cartage").val();
        var GrandTotal = $("#GrandTotal").val();

        var Delivery_Schedules = $("#Delivery_Schedule1").val();
        var Rupees = $("#Rupees").val();
        var SendTo = $("#Employee option:selected").val();           
        var rr = true;
        if (PurchaseOrderDate == "") {
            alert('OrderDate field is empty');
            $('#Date').css('border-color', '#f0551b');
                
            rr = false;
        }
        else {
            $('#Date').css('border-color', '');
        }                    

        if (Rupees == "") {
            alert('Amount in Word field is empty');
            $('#Rupees').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Rupees').css('border-color', '');
        }
        if (Delivery_Schedules == "") {
            alert('Delivery Schedule field is empty');
            $('#Delivery_Schedule1').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Delivery_Schedule1').css('border-color', '');
        }


        if (PurchaseOrderNo == "")
        {
            alert('Purchase OrderNo field is empty');
            $('#PurchaseOrderNo').css('border-color', '#f0551b');
                
            rr = false;
        }
        else {
            $('#PurchaseOrderNo').css('border-color', '');

        }

        if (ProjectNo == "") {
            $('#Projects').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#Projects').css('border-color', '');
        }
        if (SendTo == "0") {
            alert('Send to field is empty');
            $('#Employee').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#Employee').css('border-color', '');
        }

        if (SupplierNo == "")
        {
            alert('Vendor Name field is empty');
            $('#VNAME').css('border-color', '#f0551b');
                
            rr = false;
        }
        else {
            $('#VNAME').css('border-color', '');

        }



        if (IndentRefNo == "")
        {
            $('#Indents').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#Indents').css('border-color', '');

        }
        if (Total == "")
        {
            $('#Total').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#Total').css('border-color', '');

        } 
        if (SubTotal == "")
        {
            $('#SubTotal').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#SubTotal').css('border-color', '');

        }
        if (GrandTotal == "")
        {
            $('#GrandTotal').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#GrandTotal').css('border-color', '');

        }
        
        if (rr == false)
        {
            
             
            return false;
        }
        else
        {
            var T=0;
             
            $( "td[class^='Amount_" ).each(function() {
                
                if($(this).find('input').val()!=null)
                { 
                    var A= $(this).find('input').val()
                    T=+T + +A;
                }
            });
            if(T==0)
            { alert("Items Grid Not Fill Correctly ");
                return false;
            }
            return true;
        }
    }
});









