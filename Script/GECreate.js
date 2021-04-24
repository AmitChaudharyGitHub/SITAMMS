                       //@*GetPrj*@
    $(document).ready(function () {

        

        window['GetPrj'] = function (dateString)
        { 
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetProjectByEmpId, // Calling json method

                dataType: 'json',
                data: { E: ss},
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
                         $("#Projects").prop('selectedIndex', 1);
                },
                complete: function () {
                    $('#dvLoading').hide();
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

//@*GateEntryNo && EMP*@
    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#Projects").change(function () {
            $("#GateEntryNo").empty();
            $("#Status").prop('selectedIndex', 0);
            $("#StatusTypeNo").empty();
            $('#formbody').html('');

            if($("#Projects option:selected").val()!=0)
            {
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetProjectGateEntryNo, // Calling json method

                    dataType: 'json',

                    data: { id: $("#Projects").val() },
                    // Get Selected Country ID.

                    success: function (obj) {
                        obj = $.parseJSON(obj)
                        //  var r = toArray(obj);


                        $("#GateEntryNo").val(obj.Message);
                        GetEmp1(obj.List);

                    },
                    complete: function () {
                        $('#dvLoading').hide();
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
        $("#Employee").html(procemessage).show();
        var markup = "<option value='0'>Select Employee</option>";
        for (var x = 0; x < data.length; x++) {


            //console.log(data[x].Value);

            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

        }
        $("#Employee").html(markup).show();

    }

    $(document).ready(function () {

        $(document).on('keyup', "td[class^='CDispatch_']", function () {
           // alert("A");
            var current = $(this).find('input').val();
            var str = $(this).attr('class');
            var a = str.substring(10);
            var Dispatch = $("#Dispatch_" + a).find('input').val();
            current = +current + +Dispatch
            var Qty = $("#Qty_" + a).find('input').val();
            if (+current > +Qty) {
                alert("Total Dispatch Quantity Can,t Greater than Qty");
                $(this).find('input').val('');
            }
          
          
        });


        $(document).on('keyup', "td[class^='CReceive_']", function () {
            var current = $(this).find('input').val();
            var str = $(this).attr('class');
            var a = str.substring(9);
            var Receive = $("#Receive_" + a).find('input').val();
            current = +current + +Receive
            var Dispatch = $("#Dispatch_" + a).find('input').val();
            if (+current > +Dispatch) {
                alert("Total Receive Quantity Can,t Greater than Dispatch Qty");
                $(this).find('input').val('');
            }


        });
    });

// @*StatusList*@
    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#Status").change(function () {
            $("#StatusTypeNo").empty();           
            $('#formbody').html('');

            if($("#Status option:selected").val()!=0)
            {
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetStatusTypeNo, // Calling json method

                    dataType: 'json',

                    data: { Type: $("#Status").val(),ProjectNo:  $("#Projects").val() },
                    // Get Selected Country ID.

                    success: function (obj) {
                        obj = $.parseJSON(obj)
                        //  var r = toArray(obj);


                  
                        GetStatusList(obj);

                    },
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    error: function (ex) {
                        // alert('Failed to retrieve Project Code.' + ex);
                    }
                });
                return false;}
        })
    });
    function   GetStatusList(data) {
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#StatusTypeNo").html(procemessage).show();
        var markup = "<option value='0'>Select No</option>";
        for (var x = 0; x < data.length; x++) {


            //console.log(data[x].Value);

            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

        }
        $("#StatusTypeNo").html(markup).show();

    }

//@*Get Grid*@
    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#StatusTypeNo").change(function () {
            if($("#StatusTypeNo option:selected").val()!=0)
            {
                $('#dvLoading').show();
                $.ajax({
                    type:  'GET',
                    url: Grid, // Calling json method

              

                    data: { Type: $("#Status").val(),PurchaseNo:  $("#StatusTypeNo").val() },
                    // Get Selected Country ID.
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    success: function (result) {
                 
                        $('#formbody').html(result);
                        getchallan();
                    },
                    error: function (ex) {
                        alert('Failed to retrieve Project Code.' + ex);
                    }
                });
                return false;}
        })

        function getchallan()
        {
           // alert("z");
            if ($("#Status").val() == "Dispatch" || $("#Status").val() == "Receive")
            {
                
                $("#ChallanNo").val($("div[class='Challan']").attr('challanno'));
                $("#VehicleNo").val($("div[class='Challan']").attr('vehicleno')) ;
                $("#ChallanDate").val($("div[class='Challan']").attr('challandate')) ;
            }
           
        }
    });









//@*Date*@

  $(document).ready(function () {
      $(".DatePicker").datepicker({
          dateFormat: 'dd M yy',
          changeMonth: true,
          changeYear: true,
          value:"",
      });

  });

//@*Save*@
    $(document).ready(function ()
    {
        $('#btnsave').click(function (e)
        {
            //debugger;
            var Valid=ValidMaster();
            if(Valid==false)
                return;
            var otArr = [];
            var url = AddM_UpC;
            if($("#Status").val()=="LP")
            { 
                var values = LPToArray();
               
                Valid=values[0];
                if(Valid==false)
                    return;
                otArr=values[1];
                
            }
            else if($("#Status").val()=="IPO")
            {  
                var values = IPOToArray();
                Valid=values[0];
                if(Valid==false)
                    return;
                otArr=values[1];
            }
            else if ($("#Status").val() == "Dispatch") {
                var values = DispatchToArray();
                Valid = values[0];
                if (Valid == false)
                    return;
                otArr = values[1];
            }
            else if ($("#Status").val() == "Receive") {
                var values = ReceiveToArray();
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
                dataType: "json" ,
                processdata: true,

                success: function (json)
                {  if (json == "1")
                {
                    $("#myModal").modal('show');
                    //$("#Date").datepicker("setDate", "");
                    $("#Time").prop('selectedIndex', 0); 
                    $("#Projects").prop('selectedIndex', 0); 
                    $("#Employee").prop('selectedIndex', 0); 
                    $("#Status").prop('selectedIndex', 0); 
                    $("#StatusTypeNo").prop('selectedIndex', 0);   
                     
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
        function LPToArray()
        {
            
            var otArr = [];
            var ValidChild=false;
            var tbl2 = $('#grid tbody tr').each(function (i)
            {
                var obj = new Object();
                if ($(this)[0].rowIndex != 0 && $(this)[0].rowIndex != 1)
                {
                    x = $(this).children();
                    obj.ReceiveQty = $(this).children().eq(8).find('input').val();
                    if (obj.ReceiveQty != null && obj.ReceiveQty != 0) {
                        ValidChild = true;
                        obj.GateEntryNo = $("#GateEntryNo").val();

                        obj.Status = $("#Status option:selected").val();
                        obj.StatusTypeId = $("#StatusTypeNo option:selected").val();
                        obj.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim();

                        // UnitNo ItemGroupNo Added from controller
                        obj.ItemNo = $(this).children().eq(0).attr('it');
                        obj.Item = $(this).children().eq(2).text().trim();
                        obj.ItemGroup = $(this).children().eq(1).text().trim();
                        obj.Unit = $(this).children().eq(3).text().trim();

                        obj.StatusChildId = $(this).children().eq(0).attr('lpuid');
                        obj.Vendor = $("td[class='Ven']").text().trim();
                        obj.VendorNo = $("td[class='Ven']").attr('vid');

                        obj.Rate = $(this).children().eq(0).attr('rate');


                        otArr.push(obj);
                    }
                }
            })
            return [ValidChild, otArr];
           
        }
        function IPOToArray()
        {
            
            var otArr = [];
            var ValidChild=false;
            var tbl2 = $('#grid tbody tr').each(function (i)
            {
                var obj = new Object();
                if ($(this)[0].rowIndex != 0 && $(this)[0].rowIndex != 1)
                {
                    x = $(this).children();
                    obj.ReceiveQty = $(this).children().eq(9).find('input').val();
                    if (obj.ReceiveQty != null && obj.ReceiveQty != 0) {
                        ValidChild = true;
                        obj.GateEntryNo = $("#GateEntryNo").val();

                        obj.Status = $("#Status option:selected").val();
                        obj.StatusTypeId = $("#StatusTypeNo option:selected").val();
                        obj.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim();
                        // UnitNo ItemGroupNo Added from controller
                        obj.ItemNo = $(this).children().eq(0).attr('it');
                        obj.Item = $(this).children().eq(2).text().trim();
                        obj.ItemGroup = $(this).children().eq(1).text().trim();
                        obj.Unit = $(this).children().eq(3).text().trim();

                        obj.StatusChildId = $(this).children().eq(0).attr('ipouid');
                        obj.Vendor = $("td[class='Ven']").text().trim();
                        obj.VendorNo = $("td[class='Ven']").attr('vid');

                        obj.Rate = $(this).children().eq(0).attr('rate');


                        otArr.push(obj);
                    }
                }
            })
            return [ValidChild, otArr];
        }
        function DispatchToArray() {
          //  alert("A");
            var otArr = [];
            var ValidChild = false;
            var tbl2 = $('#grid tbody tr').each(function (i) {
                var obj = new Object();
                if ($(this)[0].rowIndex != 0 && $(this)[0].rowIndex != 1 && $(this)[0].rowIndex != 2) {
                    x = $(this).children();
                    obj.ReceiveQty = $(this).children().eq(7).find('input').val();
                    if (obj.ReceiveQty != null && obj.ReceiveQty != 0) {
                        ValidChild = true;
                        obj.GateEntryNo = $("#GateEntryNo").val();

                        obj.Status = $("#Status option:selected").val();
                        obj.StatusTypeId = $("#StatusTypeNo option:selected").val();
                        obj.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim();

                        obj.ItemNo = $(this).children().eq(0).attr('it');
                        obj.ItemGroupNo = $(this).children().eq(0).attr('ig');
                        obj.Item = $(this).children().eq(2).text().trim();
                        obj.ItemGroup = $(this).children().eq(1).text().trim();
                        obj.Unit = $(this).children().eq(3).text().trim();
                        obj.UnitNo = $(this).children().eq(0).attr('unitno');

                        obj.StatusChildId = $(this).children().eq(0).attr('ostuid');
                        //This Vendor use Here For Receiver Site
                        obj.Vendor = $("td[class='RVen']").attr('receivename');
                        obj.VendorNo = $("td[class='RVen']").attr('receiveno');

                        obj.Rate = $(this).children().eq(0).attr('rate');

                        obj.TaxPer = $(this).children().eq(0).attr('taxper');
                        obj.DeliveryUnitCharge = $(this).children().eq(0).attr('dperunit');
                        //This ReceiveQty use Here For Dispatch Qty

                        otArr.push(obj);
                    }
                }
            })
            return [ValidChild, otArr];
        }
        function ReceiveToArray() {

            var otArr = [];
            var ValidChild = false;
            var tbl2 = $('#grid tbody tr').each(function (i) {
                var obj = new Object();
                if ($(this)[0].rowIndex != 0 && $(this)[0].rowIndex != 1 && $(this)[0].rowIndex != 2) {
                    x = $(this).children();
                    obj.ReceiveQty = $(this).children().eq(7).find('input').val();
                    if (obj.ReceiveQty != null && obj.ReceiveQty != 0) {
                        ValidChild = true;
                        obj.GateEntryNo = $("#GateEntryNo").val();

                        obj.Status = $("#Status option:selected").val();
                        obj.StatusTypeId = $("#StatusTypeNo option:selected").val();
                        obj.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim();

                        obj.ItemNo = $(this).children().eq(0).attr('it');
                        obj.ItemGroupNo = $(this).children().eq(0).attr('ig');
                        obj.Item = $(this).children().eq(2).text().trim();
                        obj.ItemGroup = $(this).children().eq(1).text().trim();
                        obj.Unit = $(this).children().eq(3).text().trim();
                        obj.UnitNo = $(this).children().eq(0).attr('unitno');

                        obj.StatusChildId = $(this).children().eq(0).attr('ostuid');
                        //This Vendor use Here For Dispach Site
                        obj.Vendor = $("td[class='DVen']").attr('dispatchname');
                        obj.VendorNo = $("td[class='DVen']").attr('dispatchno');

                        obj.Rate = $(this).children().eq(0).attr('rate');
                        obj.TaxPer = $(this).children().eq(0).attr('taxper');
                        obj.DeliveryUnitCharge = $(this).children().eq(0).attr('dperunit');
                        //This ReceiveQty use Here For Receive Qty

                        otArr.push(obj);
                    }
                }
            })
            return [ValidChild, otArr];
        }
        function getMaster() {



            var obj1 = new Object();

            obj1.Date = $("#Date").val();
            obj1.Time = $("#Time").val();

            obj1.ProjectNo = $("#Projects option:selected").val();
            obj1.ProjectName = $("#Projects option:selected").text();

            obj1.GateEntryNo = $("#GateEntryNo").val();

            obj1.EmpId = $("#Employee option:selected").val();
            if (obj1.EmpId!=0)
            obj1.EmpName = $("#Employee option:selected").text();

            obj1.ChallanNo = $("#ChallanNo").val();
            obj1.ChallanDate = $("#ChallanDate").val();
            obj1.VehicleNo = $("#VehicleNo").val();

            obj1.Status = $("#Status option:selected").val();
           
            obj1.StatusTypeId = $("#StatusTypeNo option:selected").val();
            obj1.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim();   

         
            obj1.EmpId = $("#Employee option:selected").val();
            obj1.EmpName = $("#Employee option:selected").text();



            return obj1;
        }


        function ValidMaster()
        {
            var Projects = $("#Projects option:selected").val();
            var GateEntryNo = $("#GateEntryNo").val();
            var Status = $("#Status option:selected").val();
            var StatusTypeNo = $("#StatusTypeNo option:selected").val();
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

            if (StatusTypeNo == "") {
                $('#StatusTypeNo').css('border-color', '#f0551b');
                rr = false;
            }
            else {
                $('#StatusTypeNo').css('border-color', '');
            }

            if (Date == "") {
                $('#Date').css('border-color', '#f0551b');
                rr = false;
            }
            else {
                $('#Date').css('border-color', '');
            }        
                                           
            return rr;
            
        }

        function ValidChild()
        {
            
            
        }
    });





    $(document).ready(function ()
    {
          
        $(document).on('keyup', "td[class^='Receive_']", function () {
            var current=$(this).find('input').val();
            var str=$(this).attr('class');
            var a=str.substring(8);
            var Qty = $("#Qty_" + a).find('input').val();
            var PReceive = $("#PReceive_" + a).find('input').val();
            var bal = +current + +PReceive;
            if ( +bal > +Qty)
            { 
                alert("Total Greater than  Purchase Qty");
                $(this).find('input').val('');
            }
           
          
        });
    })


    $(document).ready(function () {
        //  alert('testing');
      

        $(document).on('keypress',"input[id='Receive']",function(event){


            return isNumber(event, this)
        });
      
        function isNumber(evt, element) {

            var charCode = (evt.which) ? evt.which : event.keyCode

            if (
                (charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
                (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
                (charCode < 48 || charCode > 57))
                return false;

            return true;
        }

    });
