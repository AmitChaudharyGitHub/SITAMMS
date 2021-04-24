    $(document).ready(function () {

        window['lol'] = function() { alert('lol');   };

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
                    $("#prjtid1").html(procemessage).show();
                    var markup = "<option value='0'>Select Project</option>";
                    $("#prjtid1").html(markup).show();
                    result = $.parseJSON(result)
                    var selectedDeviceModel = $('#prjtid1');
                    $.each(result, function (index, item) {
                        selectedDeviceModel.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });
                    //$("#prjtid1").prop('selectedIndex', 1);
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
        //Country Dropdown Selectedchange event
        $("#prjtid1").change(function () {
            $("#name").empty();
            $("#VNAME").empty();
            $('select[name^="VGId"] option:selected').attr("selected", null);
            if($("#prjtid1 option:selected").val()!=0)
            {
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: Getprojectcodes, // Calling json method

                    dataType: 'json',

                    data: { id: $("#prjtid1").val() },
                    // Get Selected Country ID.
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    success: function (obj) {
                        obj = $.parseJSON(obj)
                        //  var r = toArray(obj);


                        $("#name").val(obj.Message);
                        GetEmp1(obj.List);

                    },
                    error: function (ex) {
                        // alert('Failed to retrieve Project Code.' + ex);
                    }
                });
                return false;}
        })
    });
    function GetEmp1(data) {
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#empName1").html(procemessage).show();
        var markup = "<option value='0'>Select Employee</option>";
        for (var x = 0; x < data.length; x++) {


            //console.log(data[x].Value);

            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

        }
        $("#empName1").html(markup).show();

    }

    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        
        $("#VGID1").change(function ()
        { $("#VNAME").empty();
            if($("#VGID1 option:selected").val()!=0)
            {
                $('#dvLoading').show();
                $("#VNAME").empty();
                $.ajax({
                    type: 'POST',
                    url: GetVendorByGroup, // Calling json method

                    dataType: 'json',

                    data: { Vid: $("#VGID1").val(), Pid: $("#prjtid1").val() },
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
        //Show tax percentage
        $("#taxtype1").change(function ()
        {
            var a = $("#taxtype1 option:selected").val();


            $("#taxper1").val(a);

        })
    });

    $(document).ready(function () {

       function getAmount()
       {
           if ($("#rate1").val() != null && $("#quantity1").val() != null) {
                var a = $("#rate1").val() * $("#quantity1").val();
                // console.log(a);
                $("#amount1").val(a.toFixed(2));
            }
            else
            {
                $("#amount1").val(0);
            }
        }
        //Country Dropdown Selectedchange event
       $("#rate1").keyup(function () {                      
            getAmount();
        })
        $("#quantity1").keyup(function () {
            getAmount();

        })
    });
    function Getgt() {
        //alert("getgt");
        if ($("#roundoff").val() != null)
            r = $("#roundoff").val();
        if ($("#freight").val() != null)
            f = $("#freight").val();
        if ($("#exciseduty").val() != null)
            ex = $("#exciseduty").val();
        GT = +pf + +Tax - +Tdis + +r + +f + +ex;
        //console.log(GT);


        //console.log(pf);
        //console.log(Tax);
        //console.log(Tdis);
        //console.log(r);
        //console.log(f);
        //console.log(ex);
        //console.log(GT);
        $("#granttotal").val(GT.toFixed(2));
    }
    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#roundoff").keyup(function ()
        {
            //  alert("Round");
            Getgt();


        })
    });
    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#freight").keyup(function () {
            // alert("Round");
            Getgt();


        })
    });
    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#exciseduty").keyup(function () {
            //alert("Round");
            Getgt();


        })
    });
    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#quantity1").keyup(function () {

            if ($("#rate1").val() !=0 && $("#quantity1").val() !=0)
            {
                var a = $("#rate1").val() * $("#quantity1").val();
                //console.log(a);
                $("#amount1").val(a);
            }


        })
    });

    $(document).ready(function () {
        //Item Dropdown Selectedchange event
        $("#tblItemMaster").change(function () {
            $("#make1").empty();
            $("#partno1").empty();
            $("#unit1").empty();
            if($("#tblItemMaster option:selected").val()!=0)
            {
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetItemDetail, // Calling json method

                    dataType: 'json',

                    data: { id: $("#tblItemMaster").val() },
                    // Get Selected Country ID.
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    success: function (item) {
                        item = $.parseJSON(item)
                        //console.log(item);

                        //console.log(item.Make);
                        //console.log(item.PartNo);
                        $("#make1").val(item.Make);
                        $("#partno1").val(item.PartNo);
                        $("#unit1").val(item.Unit);
                        // });
                    },
                    error: function (ex) {
                        alert('Failed to retrieve Make.' + ex);
                    }
                });
                return false;}
        })
    });







//@* FOR ITEM NAME BINDING HERE *@

    //$(document).ready(function () {
    //    //Country Dropdown Selectedchange event
    //    $("#itemgroupname1").change(function () {
    //        $("#tblItemMaster").empty();
    //        $("#tblItemMaster").append($("<option></option").val("").html("Select Item Name"));
    //        if($("#itemgroupname1 option:selected").val()!=0)
    //        {
    //            debugger;
    //            $('#dvLoading').show();
    //            $.ajax({
    //                type: 'POST',
    //                url:GetItemByGroup2, // Calling json method

    //                dataType: 'json',

    //                data: { Gid: $("#itemgroupname1").val(), Pid: $("#prjtid1").val() },
    //                // Get Selected Country ID.
    //                complete: function () {
    //                    $('#dvLoading').hide();
    //                },
    //                success: function (names) {
    //                    names = $.parseJSON(names)


    //                    $.each(names, function (i, itname) {
    //                        $("#tblItemMaster").append('<option value="' + itname.Value + '">' +
    //                             itname.Text + '</option>');

    //                    });
    //                },
    //                error: function (ex) {
    //                    alert('Failed to retrieve Item Name.' + ex);
    //                }
    //            });
    //            return false;}
    //    })
    //});

    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#itemgroupname1").change(function () {
            $("#tblItemMaster").empty();
            $("#unit1").val('');
            $("#make1").val('');
            $("#partno1").val('');
            $("#tblItemMaster").append($("<option></option").val("").html("Select Item Name"));
            if ($("#itemgroupname1 option:selected").val() != 0) {
                debugger;
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetItemByGroup, // Calling json method

                    dataType: 'json',

                    data: { id: $("#itemgroupname1").val() },
                    // Get Selected Country ID.
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    success: function (names) {
                        names = $.parseJSON(names)


                        $.each(names, function (i, itname) {
                            $("#tblItemMaster").append('<option value="' + itname.Value + '">' +
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

//@* Here for dynamic add webgrid  *@
    //java script array;
    var gridList = [];
    var pf = 0;
    var Tdis = 0;
    var Tax = 0;
    var GT = 0;

    var r = 0;
    var f = 0;
    var ex = 0;
    //var child = 0;
    var nowTemp = new Date();
  //  @*var nowTemp = @ViewBag.OrderDate;*@
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);

    $(document).ready(function () {

        $("#addNew").click(function () {
  
            var obj = new Object();
            obj.Id = getUniqueId();


            obj.ItemGroup = $("#itemgroupname1 option:selected").text();       
            obj.ItemId = $("#tblItemMaster option:selected").val();
            obj.Unit = $("#unit1").val();
           
            

            obj.ItemGroupID = $("#itemgroupname1 option:selected").val();
            obj.Itemname = $("#tblItemMaster option:selected").text();
          
            obj.Makename = $("#make1").val();
            obj.Partname = $("#partno1").val();
            obj.PurchaseQty = $("#quantity1").val();
            obj.Rate = $("#rate1").val();
            obj.Discount = $("#disper1").val();
            if($("#taxtype1 option:selected").val()!=0)
                obj.TaxType = $("#taxtype1 option:selected").text();
else
                obj.TaxType ='';
            obj.Vat = $("#taxper1").val();
            obj.Amount = $("#amount1").val();
        

            if (obj.PurchaseQty == "") {
                $('#quantity1').css('border-color', '#f0551b');
}
             if (obj.Rate == "") {
                $('#rate1').css('border-color', '#f0551b');
}
else {
                 gridList.push(obj);
                 //child =child+ 1;
                $("#grid").prepend("<tr class='danger _tempRow'><td>" + obj.ItemGroup + "</td><td>" + obj.Itemname + "</td><td>" + obj.Unit + "</td><td>" + obj.Makename + "</td><td>" + obj.Partname + "</td><td>" + obj.PurchaseQty + "</td><td>" + obj.Rate + "</td><td>" + obj.Discount + "</td><td>" + obj.TaxType + "</td><td>" + obj.Vat + "</td><td>" + obj.Amount + "</td><td><a href='#' id='removeItem' uniqueId='" + obj.Id + "'>Remove</a></td></tr>");
                GetPF();
    //$("#unit1").val('');
    //$("#make1").val('');
    //$("#partno1").val('');
                $("#quantity1").val('');
                $("#rate1").val('');
                $("#amount1").val('');
    //for removing border color here
                $('#quantity1').css('border-color', '');
                $('#rate1').css('border-color', '');
                $('#tblItemMaster').css('border-color', '');
                $("#disper1").val('');
}

})

        $("#Submit").click(function () {

            if (gridList.length < 1)
        {
            alert("Add Any Item");
            return;
        }

    //  alert("Helo");

         
            var obj1 = new Object();

            obj1.CashPurchaseNo = $("#name").val();
            obj1.Date = $("#OrderDate").val();
            obj1.VendorId = $("#VNAME option:selected").val();
            obj1.VendorName = $("#VNAME option:selected").text();
            obj1.RefNo = $("#RefNo").val();
            obj1.RefDate = $("#RefDate").val();
            obj1.ProjectId = $("#prjtid1 option:selected").val();
            obj1.EmpId = $("#empName1 option:selected").val();
            obj1.ProjectName = $("#prjtid1 option:selected").text();
            obj1.EmpName = $("#empName1 option:selected").text();
            if (obj1.EmpId != 0)
                obj1.EmpName = $("#empName1 option:selected").text();
            obj1.Delivery_Address = $("#deliveryaddress").val();
            obj1.Delivery_Date = $("#deliverydate").val();
            obj1.P_F = $("#pf").val();
            obj1.ExciseDuty = $("#exciseduty").val();
            obj1.Freight = $("#freight").val();
            obj1.D_Charges = $("#dcharges").val();
            obj1.Vat = $("#vat").val();
            obj1.RondOff = $("#roundoff").val();
            obj1.GrandTotal = $("#granttotal").val();
    //console.log(obj1);
    //console.log(gridList);
            var session = {
                'Child': gridList,
                'Master': obj1
};
    //console.log(session);
    //var prj = $("#prjtid1 option:selected").val();
            var prj = $("#prjtid1 option:selected").val();
            var gpname = $("#itemgroupname1 option:selected").val();
            var empnames = $("#empName1 option:selected").val();
            var itmnamess = $("#tblItemMaster option:selected").val();
            var ordrdate = $("#OrderDate").val();
            var vendrid = $("#VGID1 option:selected").val();
            var endrname = $("#VNAME option:selected").val();
            var rr = true;
            if (prj == "0" || prj == "" || prj == null) {
                $('#prjtid1').css('border-color', '#f0551b');
                rr = false;
}
else {
                $('#prjtid1').css('border-color', '');

}
            if (gpname == "") {
                $('#itemgroupname1').css('border-color', '#f0551b');
                rr = false;
}
else {
                $('#itemgroupname1').css('border-color', '');
}

            if (empnames == "" || empnames == "0" || empnames == null) {
                $('#empName1').css('border-color', '#f0551b');
                rr = false;
}
else {
                $('#empName1').css('border-color', '');
}

            if (itmnamess == "") {
                $('#tblItemMaster').css('border-color', '#f0551b');
                rr = false;
}
else {
                $('#tblItemMaster').css('border-color', '');
}

            if (ordrdate == "") {
                $('#OrderDate').css('border-color', '#f0551b');
                rr = false;
}
else {
                $('#OrderDate').css('border-color', '');
}

            if (vendrid == "") {
                $('#VGID1').css('border-color', '#f0551b');
                rr = false;
}
else {
                $('#VGID1').css('border-color', '');
}

            if (endrname == "" || endrname == "0" || endrname == null) {
                $('#VNAME').css('border-color', '#f0551b');
                rr = false;
}
else {
                $('#VNAME').css('border-color', '');
}

            if (rr == false)
{
    //console.log(rr);
             
                return;
}
            var url = SendList1;
            $('#dvLoading').show();
            $.ajax({
    type: "Post", 		//GET or POST or PUT or DELETE verb
             //   @*url:@Url.Action("SendList1","CashPurchaseChilds") ,*@ 		// Location of the service
    url: url,
    //data: { Grid:k }, 		//Data sent to server
    data: JSON.stringify(session),
    contentType: "application/json; charset=utf-8",		// content type sent to server
    dataType: "json", 	//Expected data format from server
    processdata: true, 	//True or False
    complete: function () {
                    $('#dvLoading').hide();
},
    success: function (json) {//On Successful service call
                    if (json == "1") {
    // alert("Data Saved Successfully.");
                        $("#myModal").modal('show');
                        $("#name").val('');

                        //  $("#prjtid1").val('');
                        $('select[name^="prjtid"] option[value="0"]').attr("selected", "selected");
                       // $('select[name^="prjtid"] option:selected').attr("selected", null);
                       // $("#VGID1").val('');
                       // $('select[name^="VGId"] option[value="0"]').attr("selected", "selected");
                       $('select[name^="VGId"] option:selected').attr("selected", null);

                        $("#empName1").val('');
                        $("#itemgroupname1").val('');
                        $("#tblItemMaster").val('');
                        $("#RefNo").val('');

                        $("#OrderDate").datepicker("setDate", "")
                        $("#RefDate").datepicker("setDate", "")

                        $("#deliveryaddress").val('');

                        $("#deliverydate").datepicker("setDate", now)
                        $("#pf").val('');
                        $("#exciseduty").val('');
                        $("#freight").val('');
                        $("#dcharges").val('');
                        $("#vat").val('');
                        $("#roundoff").val('');
                        $("#granttotal").val('');
                        gridList = [];
                        child = 0;
                        $("#grid").empty();
                        $("#VNAME").empty();
                        $("#empName1").empty();
}

},
    error: function ()
{
                    alert('Error in Submit Data');
}
    // When Service call fails
})
})
        $("body").on("click", "#removeItem", function () {
            var id = $(this).attr("uniqueId");

            for (var index = 0; index < gridList.length; index++) {
                if (gridList[index].Id == id) {
                    //child = child - 1;
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
            Tdis = 0;
            Tax = 0;
            GT = 0;
            if ($("#roundoff").val() != null)
                r = $("#roundoff").val();
            if ($("#freight").val() != null)
                f = $("#freight").val();
            if ($("#exciseduty").val() != null)
                ex = $("#exciseduty").val();
            for (var index = 0; index < gridList.length; index++) {
                pf = +pf + +gridList[index].Amount;
    // console.log(pf);
                $("#pf").val(pf.toFixed(2));
                Tdis = +Tdis + +(gridList[index].Amount * gridList[index].Discount / 100);
    // console.log(Tdis);
                $("#dcharges").val(Tdis.toFixed(2));
                Tax = +Tax + +(gridList[index].Amount * gridList[index].Vat / 100);
    // console.log(Tax);
                $("#vat").val(Tax.toFixed(2));

}
            GT = +pf + +Tax - +Tdis + +r + +f + +ex;

            $("#granttotal").val(GT.toFixed(2));
}


})


      $(document).ready(function () {
          $(".DatePicker").datepicker({
              dateFormat: 'dd M yy',
              changeMonth: true,
              changeYear: true,
              value:"",
          }).val('');

      });








