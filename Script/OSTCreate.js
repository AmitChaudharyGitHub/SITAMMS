    $(document).ready(function () {

       

        window['GetPrj'] = function (dateString)
        { 
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
                    $("#DispatchSite").html(procemessage).show();
                    $("#ReceiveSite").html(procemessage).show();
                    var markup1 = "<option value='0'>Select Dispatch Site</option>";
                    $("#DispatchSite").html(markup1).show();
               
                    var markup2 = "<option value='0'>Select Receive Site</option>";
                    $("#ReceiveSite").html(markup2).show();
                    result = $.parseJSON(result);
                    var List1 = $.parseJSON(result.List1);
                    var List2 = $.parseJSON(result.List2);
                   
                    var selectedDeviceModel1 = $('#DispatchSite');
                    $.each(List1, function (index, item) {
                        //alert("A");
                        selectedDeviceModel1.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });
                    var selectedDeviceModel2 = $('#ReceiveSite');
                    $.each(List2, function (index, item) {
                        selectedDeviceModel2.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });
                    //$("#DispatchSite").prop('selectedIndex', 1);
                    $("#DispatchSite").prop('selectedIndex', 1).trigger('change');
                },
                error: function (ex) {
                    alert('Failed to retrieve Projects.' + ex);
                }
            });
            return false;
        };
        GetPrj();
       
    }
        )

    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#DispatchSite").change(function () {
            $("#TransferDocumentNo").empty();
            if ($("#DispatchSite option:selected").val() != 0)
            {
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetOstNo, // Calling json method

                    dataType: 'json',

                    data: { ProjectID: $("#DispatchSite").val() },
                    // Get Selected Country ID.
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    success: function (obj) {
                        obj = $.parseJSON(obj)
                      


                        $("#TransferDocumentNo").val(obj);
                     

                    },
                    error: function (ex) {
                         alert('Failed to retrieve Transfer Document No.' + ex);
                    }
                });
                return false;}
        })
    });
   
    $(document).ready(function () {
       
        $("#ItemGroup").change(function () {
            $("#ItemMaster").empty();
            $("#ItemMaster").append($("<option></option").val("").html("Select Item Name"));
            if ($("#ItemGroup option:selected").val() != 0) {
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
    });

    $(document).ready(function () {
     
        $("#ItemMaster").change(function () {
          
            $("#Unit").empty();
            if ($("#ItemMaster option:selected").val() != 0) {
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetItemDetail, 

                    dataType: 'json',

                    data: { id: $("#ItemMaster").val(), PrjId: $("#DispatchSite").val() },
            
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    success: function (item) {
                        item = $.parseJSON(item)
                       
                        $("#Rate").val(item.Rate);
                        $("#Unit").val(item.Unit);
                        $("#Unit").attr("UNo", item.UnitNo);
                    },
                    error: function (ex) {
                        alert('Failed to retrieve Unit' + ex);
                    }
                });
                return false;
            }
        })
    });
   
    $(document).ready(function () {
        //Show tax percentage
        $("#TaxType").change(function ()
        {
            var a = $("#TaxType option:selected").val();


            $("#TaxPer").val(a);

        })
    });

    $(document).ready(function () {

       function getAmount()
       {
           if ($("#Rate").val() != null && $("#Qty").val() != null) {
               var a = $("#Rate").val() * $("#Qty").val();
                
               $("#Amount").val(a.toFixed(2));
            }
            else
            {
               $("#Amount").val(0);
            }
        }
        //Country Dropdown Selectedchange event
       $("#Rate").keyup(function () {                      
            getAmount();
        })
        $("#Qty").keyup(function () {
            getAmount();

        })
    });
    function Getgt() {
   
        if ($("#AmountOfFreight").val() != null)
            f = $("#AmountOfFreight").val();
      
        GT = +pf + +Tax + +Tdl + +f ;
       
        $("#GrandTotal").val(GT.toFixed(2));
    }
  
    $(document).ready(function () {
       
        $("#AmountOfFreight").keyup(function () {
            
            Getgt();


        })
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

    $(document).ready(function () {

        $("#addNew").click(function () {
   
            var obj = new Object();
            obj.Id = getUniqueId();


            obj.ItemGroup = $("#ItemGroup option:selected").text();
            obj.ItemGroupNo = $("#ItemGroup option:selected").val();
            obj.Itemname = $("#ItemMaster option:selected").text();
            obj.ItemNo = $("#ItemMaster option:selected").val();
            obj.Unit = $("#Unit").val();
            obj.UnitNo = $("#Unit").attr('UNo');
            obj.Qty = $("#Qty").val();
            obj.Rate = $("#Rate").val();
            obj.DeliveryCharges = $("#DeliveryCharges").val();
        
            if ($("#TaxType option:selected").val() != 0)
                obj.TaxType = $("#TaxType option:selected").text();
            else
                obj.TaxType = '';
          
            obj.Vat = $("#TaxPer").val();
            obj.Amount = $("#Amount").val();
          
            
           
            
        

            if (obj.Qty == "")
            {
                $('#Qty').css('border-color', '#f0551b');
                return;
             }
           if (obj.Rate == "")
            {
                $('#Rate').css('border-color', '#f0551b');
            }
           
                gridList.push(obj);
                $("#grid").prepend("<tr class='danger _tempRow'><td>" + obj.ItemGroup + "</td><td>" + obj.Itemname + "</td><td>" + obj.Unit + "</td><td>" + obj.Qty + "</td><td>" + obj.Rate + "</td><td>" + obj.DeliveryCharges + "</td><td>" + obj.TaxType + "</td><td>" + obj.Vat + "</td><td>" + obj.Amount + "</td><td><a href='#' id='removeItem' uniqueId='" + obj.Id + "'>Remove</a></td></tr>");
                GetPF();
                //child = 1;
                $("#Qty").val('');
                $("#Rate").val('');
                $("#Amount").val('');
                $("#DeliveryCharges").val('');
                $('#Qty').css('border-color', '');
                $('#Rate').css('border-color', '');
              
          

        })

        function getMaster()
        {

            var obj1 = new Object();

            obj1.TransferDocumentNo = $("#TransferDocumentNo").val();
            obj1.TransferDocumentDate = $("#TransferDocumentDate").val();

            obj1.DispatchSiteNo = $("#DispatchSite option:selected").val();
            obj1.ReceiveSiteNo = $("#ReceiveSite option:selected").val();

            obj1.ChallanNo = $("#ChallanNo").val();
            obj1.ChallanDate = $("#ChallanDate").val();
            obj1.VehicleNo = $("#VehicleNo").val();
            obj1.P_F = $("#pf").val();
            obj1.Vat = $("#Vat").val();
            obj1.DeliveryCharges = $("#TDeliveryCharges").val();
            obj1.AmountOfFreight = $("#AmountOfFreight").val();
            obj1.GrandTotal = $("#GrandTotal").val();

            return obj1;
        }


        $("#Submit").click(function () {

            if (gridList.length < 1) {
                alert("Add Any Item");
                return;
            }

            var Valid = ValidMaster();
            if (Valid == false)
                return;
            var obj1 = getMaster();
               
  
            var session =
                {
                'Child': gridList,
                'Master': obj1
                 };                     

         

          



            var url = SendList1;
            $('#dvLoading').show();
            $.ajax(
                {
              type: "Post",	           
              url: url,  
              data: JSON.stringify(session),
              contentType: "application/json; charset=utf-8",		
              dataType: "json", 	//Expected data format from server
              processdata: true, 	//True or False
              complete: function ()
              {
                    $('#dvLoading').hide();
              },
              success: function (json)
              {
                  if (json == "1")
                  {
   
                      $("#myModal").modal('show');


                       $("#TransferDocumentNo").val('');
                       $("#TransferDocumentDate").datepicker("setDate", "");

                      $("#DispatchSite").prop('selectedIndex', 0);
                      $("#ReceiveSite").prop('selectedIndex', 0);
                      
                      //$("#DispatchSite").val(0);
                      //$("#ReceiveSite").val(0);
                      $("#ChallanNo").val('');
                      $("#ChallanDate").datepicker("setDate", "");
                      $("#VehicleNo").val('');
                      $("#pf").val('');
                      $("#Vat").val('');
                      $("#DeliveryCharges").val('');
                      $("#AmountOfFreight").val('');
                      $("#GrandTotal").val('');

                        //child = 0;
                        gridList = [];
                        $("#grid").empty();
}

},
    error: function ()
{
                    alert('Error in Submit Data');
}
    // When Service call fails
})
        })

        $("body").on("click", "#removeItem", function ()
        {
            var id = $(this).attr("uniqueId");

            for (var index = 0; index < gridList.length; index++)
            {
                if (gridList[index].Id == id) {

                    gridList.splice(index, 1);
                    $(this).parents("._tempRow").remove();
}
}
            GetPF();
})

        var uniqueId = null;
        getUniqueId = function () {
            if (!uniqueId) uniqueId = (new Date()).getTime();
            return uniqueId++;
};

        function GetPF() {
            pf = 0;
            Tdl = 0;
            Tax = 0;
            GT = 0;
            if ($("#AmountOfFreight").val() != null)
                f = $("#AmountOfFreight").val();
           
            for (var index = 0; index < gridList.length; index++)
            {
                pf = +pf + +gridList[index].Amount;
   
                $("#pf").val(pf.toFixed(2));
                Tdl = +Tdl + +(gridList[index].Qty * gridList[index].DeliveryCharges);                 
                $("#TDeliveryCharges").val(Tdl.toFixed(2));


                Tax = +Tax + +(gridList[index].Amount * gridList[index].Vat / 100);
    
                $("#Vat").val(Tax.toFixed(2));

}
            GT = +pf + +Tax + +Tdl +  +f ;

            $("#GrandTotal").val(GT.toFixed(2));
}


})

    function ValidMaster()
    {
        var TransferDocumentNo = $("#TransferDocumentNo").val();
        var TransferDocumentDate = $("#TransferDocumentDate").val();

        var DispatchSiteNo = $("#DispatchSite option:selected").val();
        var ReceiveSiteNo = $("#ReceiveSite option:selected").val();

        //var ChallanNo = $("#ChallanNo").val();
        //var ChallanDate = $("#ChallanDate").val();
        //var VehicleNo = $("#VehicleNo").val();

        var P_F = $("#pf").val();
       
        var GrandTotal = $("#GrandTotal").val();

       
        var rr = true;
        if (DispatchSiteNo == "0") {
            $('#DispatchSite').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#DispatchSite').css('border-color', '');

        }
        if (ReceiveSiteNo == "0") {
            $('#ReceiveSite').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#ReceiveSite').css('border-color', '');

        }

        if (TransferDocumentNo == "") {
            $('#TransferDocumentNo').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#TransferDocumentNo').css('border-color', '');
        }

        if (TransferDocumentDate == "") {
            $('#TransferDocumentDate').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#TransferDocumentDate').css('border-color', '');
        }


        if (P_F == "") {
            $('#pf').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#pf').css('border-color', '');
        }

        if (GrandTotal == "") {
            $('#GrandTotal').css('border-color', '#f0551b');
            rr = false;
        }
        else {
            $('#GrandTotal').css('border-color', '');
        }

        return rr;

    }


      $(document).ready(function () {
          $(".DatePicker").datepicker({
              dateFormat: 'dd M yy',
              changeMonth: true,
              changeYear: true,
              value:"",
          }).val('');

      });








