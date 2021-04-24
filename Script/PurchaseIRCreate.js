$(document).ready(function () {
    $('#ItemCode').select2();
    $('#ItemMaster').select2();
    $('#ItemGroup').select2();
    $("#PurchaseIRDate").val('');

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
                $("#Project").html(procemessage).show();

                var markup1 = "<option value='0'>Select Project</option>";
                $("#Project").html(markup1).show();
                result = $.parseJSON(result);
                var List1 = $.parseJSON(result.List1);
                debugger;

                var selectedDeviceModel1 = $('#Project');
                $.each(List1, function (index, item) {
                    // alert(item.Value);
                    selectedDeviceModel1.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });

                //$("#Project").prop('selectedIndex', 1);

                $("#Project").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Projects.' + ex);
            }
        });
        return false;
    };
    GetPrj();

    $('#ItemMaster').change(function () {
        //$('#ItemCode').prop('selectedIndex', $(this).prop('selectedIndex'));
        //$('#select2-ItemCode-container').attr('title', $('#ItemCode option:selected').text());
        //$('#select2-ItemCode-container').text($('#ItemCode option:selected').text());

        $("#ItemCode option[value = " + $('#ItemMaster').select2('data')[0].id + "]").prop("selected", true);
        $('#select2-ItemCode-container').text($('#ItemCode').select2('data')[0].text);
        $('#select2-ItemCode-container').attr('title',$('#ItemCode').select2('data')[0].text);
        
    });
    $('#ItemCode').change(function () {
        //$('#ItemMaster').prop('selectedIndex', $(this).prop('selectedIndex'));
        //$('#select2-ItemMaster-container').attr('title', $('#ItemMaster option:selected').text());
        //$('#select2-ItemMaster-container').text($('#ItemMaster option:selected').text());

        $("#ItemMaster option[value = " + $('#ItemCode').select2('data')[0].id + "]").prop("selected", true);
        $('#select2-ItemMaster-container').text($('#ItemMaster').select2('data')[0].text);
        $('#select2-ItemMaster-container').attr('title',$('#ItemMaster').select2('data')[0].text);
    });
});

$(document).ready(function () {

    


    //Country Dropdown Selectedchange event
    $("#Project").change(function () {
        $("#PurchaseIRNo").empty();
        if ($("#Project option:selected").val() !== 0) {
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetPurchaseIRNo, // Calling json method

                dataType: 'json',

                data: { ProjectID: $("#Project").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)





                  //  $("#PurchaseIRNo").val(obj.Message);
                    GetEmp1(obj.List);




                },
                error: function (ex) {
                    alert('Failed to retrieve PurchaseIR No.' + ex);
                }
            });


        $.getJSON(GetPiType, { Projectid: $("#Project option:selected").val() }, function (data) {
        data = $.parseJSON(data);

        $.each(data, function (i, item) {
            
            $('#hdn_sitepurchasevalue').val(item.EmengencyPurchaseType);
            $('#hdn_hopurchasevalue').val(item.NormalPurchaseType);
            $('#hdn_sitepurchaseLimitValue').val(item.EmengencyLimitValue);
            $('#hdn_HOepurchaseLimitValue').val(item.NormalLimitValue);

           
        });


    });



            return false;
        }
    })


    function GetEmp1(data) {
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#SendTo").html(procemessage).show();
        var markup = "<option value='0'>Forward For Approval</option>";
        for (var x = 0; x < data.length; x++) {


            //console.log(data[x].Value);

            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

        }
        $("#SendTo").html(markup).show();

    }

    $("#PurchaseIRDate").change(function () {
        $("#PurchaseIRNo").empty();
        if ($("#Project option:selected").val() !== 0) {
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetPurDate, // Calling json method

                dataType: 'json',

                data: { ProjectId: $("#Project").val(), PurDate: $("#PurchaseIRDate").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (obj) {
                    obj = $.parseJSON(obj)

                    $("#PurchaseIRNo").val(obj.PrNO);
                    $('#txtLastPODate').text(obj.Last_PODate);
                    $('#txtLastPONo').text(obj.Last_PONo);




                },
                error: function (ex) {
                    alert('Failed to retrieve PurchaseIR No.' + ex);
                }
            });
        }

    });


});

//$(document).ready(function () {

//    $("#ItemGroup").change(function () {
//        $("#ItemMaster").empty();
//        $("#ItemMaster").append($("<option></option").val("").html("Select Item Name"));

//        if ($("#ItemGroup option:selected").val() != 0) {

//            $('#dvLoading').show();
//            $.ajax({
//                type: 'POST',
//                url: GetItemByGroup2,

//                dataType: 'json',


//                data: { Gid: $("#ItemGroup").val(), Pid: $("#Project").val() },

//                complete: function () {
//                    $('#dvLoading').hide();
//                },
//                success: function (names) {
//                    names = $.parseJSON(names)


//                    $.each(names, function (i, itname) {
//                        $("#ItemMaster").append('<option value="' + itname.Value + '">' +
//                             itname.Text + '</option>');

//                    });
//                },
//                error: function (ex) {
//                    alert('Failed to retrieve Item Name.' + ex);
//                }
//            });
//            return false;
//        }
//    })
//});
$(document).ready(function () {

    $("#ItemGroup").change(function () {

        if (gridList.length > 0) {
            if (gridList[0].ItemGroupNo != $("#ItemGroup option:selected").val()) {
                alert('You cant add items from different Item Group ');
                $("#ItemGroup").val(gridList[0].ItemGroupNo);
                return false;
            }
        }


        $("#ItemMaster").empty();
        $("#ItemMaster").append($("<option></option").val("").html("Select Item Name"));

        if ($("#ItemGroup option:selected").val() !== 0) {

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
                    names = $.parseJSON(names);


                    $.each(names, function (i, itname) {
                        $("#ItemMaster").append('<option value="' + itname.Value + '">' +
                            itname.Text + '</option>');

                    });
                    BindDdl('#ItemCode', GetItemCodes, { id: $("#ItemGroup").val() }, 'Item Code');
                   
                },
                error: function (ex) {
                    alert('Failed to retrieve Item Name.' + ex);
                }
            });
            return false;
        }
    });
});
$(document).ready(function () {

    $("#ItemMaster, #ItemCode").change(function () {

        $("#Unit").empty();
        if ($("#ItemMaster option:selected").val() !== 0 || $("#ItemCode option:selected").val() !== 0) {
            var sel_item=$(this).val();
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemDetail,

                dataType: 'json',
                //$("#ItemMaster").val()
                data: { id: sel_item, PrjId: $("#DispatchSite").val() },

                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (item) {
                    item = $.parseJSON(item)


                    $("#Unit").val(item.Unit);
                    $("#Unit").attr("UNo", item.UnitNo);
                    $("#LPRate").val(item.LPRate);
                    $("#CRate").val(item.CRate);
                    $("#CTotRecv").val(item.TotRecv);
                    $("#CTotBalncd").val(item.TotBalncd);
                },
                error: function (ex) {
                    alert('Failed to retrieve Unit' + ex);
                }
            });


             $.ajax({
                type: 'POST',
                url: GetCurrentStockData, // Calling json method

                dataType: 'json',

                data: { PrjId: $("#Project option:selected").val(), IG: $("#ItemGroup option:selected").val(), itemid: $("#ItemMaster option:selected").val() },
                // Get Selected Country ID.

                success: function (balance1) {

                    //    $.each(states, function (i, state) {
                    //alert(state.Value)
                    $("#showBalanceData").text(balance1);
                    // });
                },
                error: function (ex) {
                    alert('Failed to retrieve Current Stock Value.' + ex);
                }
            });






            return false;
        }
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
      var chkData= $("#ItemMaster option:selected").val();

        var obj = new Object();
        obj.Id = getUniqueId();


        obj.ItemGroupName = $("#ItemGroup option:selected").text();
        obj.ItemGroupNo = $("#ItemGroup option:selected").val();
        obj.ItemName = $("#ItemMaster option:selected").text();
        obj.ItemNo = $("#ItemMaster option:selected").val();
        obj.Unit = $("#Unit").val();
        obj.UnitNo = $("#Unit").attr('UNo');
        obj.DemandQty = $("#Qty").val();
        obj.LastPurchaseRate = $("#LPRate").val();
        obj.CurrentRate = $("#CRate").val();
        obj.CurrentValue = $("#CValue").val();
        obj.ReceiveUpto = $("#CTotRecv").val();
        obj.Balanced = $("#CTotBalncd").val();

        obj.Remarks = $("#Remark").val();


        //obj.PurRequisitionNo = $("#PurchaseIRNo").val();        
        //obj.Date = $("#PurchaseIRDate").val();
        //obj.ProjectNo = $("#Project option:selected").val();
        //obj.ProjectName = $("#Project option:selected").text();

        if (obj.ItemGroupName === "Select Group")
        {
            $('#ItemGroup').css('border-color', '#f0551b');
            return;
        }
        if (obj.ItemName === "Select Item Name") {
            $('#ItemMaster').css('border-color', '#f0551b');
            return;
        }


        if (obj.DemandQty === "") {
            $('#Qty').css('border-color', '#f0551b');
            return;
        }
        if (obj.CurrentRate === "") {
            $('#CRate').css('border-color', '#f0551b');
            return;
        }
        if (obj.CurrentRate <= 0) {
            alert('Rate should be greater than zero. ')
            $('#CRate').css('border-color', '#f0551b');
            $("#CRate").val('')
            return;
        }

        if (gridList.length > 0) {
            var i = gridList.findIndex(x => x.ItemNo === chkData);
            if (i >= 0) {
                alert('You can not add same item two time. You can remove and change its quantity.');
                $("#Qty").val('');
                $('#Qty').css('border-color', '');

                $("#LPRate").val('');
                $('#LPRate').css('border-color', '');

                $("#CRate").val('');
                $('#CRate').css('border-color', '');

                $("#CValue").val('');
                $('#CValue').css('border-color', '');

                $("#Remark").val('');
                $('#Remark').css('border-color', '');

                $("#CTotRecv").val('');
                $('#CTotRecv').css('border-color', '');

                $("#CTotBalncd").val('');
                $('#CTotBalncd').css('border-color', '');

                $('#Unit').val('');
                return false;
            }
        }
        

        gridList.push(obj);

        $("#grid").prepend("<tr class='danger _tempRow'><td>" + obj.ItemGroupName + "</td><td>" + obj.ItemName + "</td><td>" + obj.Remarks + "</td><td>" + obj.Unit + "</td><td>" + obj.DemandQty + "</td><td>" + obj.LastPurchaseRate + "</td><td>" + obj.CurrentRate + "</td><td>" + obj.CurrentValue + "</td><td>" + obj.ReceiveUpto + "</td><td style='display:none;'>" + obj.Balanced + "</td><td><a href='#' id='removeItem' uniqueId='" + obj.Id + "'>Remove</a></td></tr>");
        //$("#<tr class='danger _tempRow'><td></td><td></td><td></td><td></td><td></td><td>Total PI Value</td><td></td><td></td><td></td></tr>").insertAfter("#grid")


        $("#Qty").val('');
        $('#Qty').css('border-color', '');

        $("#LPRate").val('');
        $('#LPRate').css('border-color', '');

        $("#CRate").val('');
        $('#CRate').css('border-color', '');

        $("#CValue").val('');
        $('#CValue').css('border-color', '');

        $("#Remark").val('');
        $('#Remark').css('border-color', '');

        $("#CTotRecv").val('');
        $('#CTotRecv').css('border-color', '');

        $("#CTotBalncd").val('');
        $('#CTotBalncd').css('border-color', '');

        $('#Unit').val('');
        $("#ItemGroup").change();

    })

    function getMaster() {

        var obj1 = new Object();

        obj1.PurRequisitionNo = $("#PurchaseIRNo").val();
        obj1.Date = $("#PurchaseIRDate").val();

        obj1.ProjectNo = $("#Project option:selected").val();
        obj1.ProjectName = $("#Project option:selected").text();

        obj1.SendTo = $("#SendTo  option:selected").val();
        obj1.SendToName = $("#SendTo  option:selected").text();
        obj1.PurchasePI_Type = $("#ddlPurchaseType option:selected").val();
        obj1.Remarks = $("#txtRemarks").val();
        obj1.DeliveryDate = $("#PurchaseIRDeliveryDate").val();
        obj1.Comp = $("#ddlPurchasedecisionType option:selected").val();

        

        return obj1;
    }


    $("#Submit").click(function () {
               

        if (gridList.length < 1) {
            alert("Add Any Item");
            return;
        }

        if($('#SendTo option:selected').val()=='0'){
            alert("Please select forward to");
            return false;
        }

        var Valid = ValidMaster();

        if (Valid === false)
            return;
        var obj = getMaster();


        var session =
            {
                'ChildNew': gridList,
                'MasterNew': obj
            };


        var formData = new FormData();
        var upldfiles = 0;
        var filesave = true;
        var fileObj = {};
        for(i=0; i<$("#b_FileUpload tr").length; i++) { 
            if($("#b_FileUpload tr").eq(i).find("input[name='AttachFile']").val()!='' && $("#b_FileUpload tr").eq(i).find("input[name='FileName']").val().trim()!='')
            {
                if($("#b_FileUpload tr").eq(i).find("input[name='AttachFile']").val()=='')
                {
                    alert("Upload File in entered File Name Row!");
                    return;
                }
                if ($("#b_FileUpload tr").eq(i).find("input[name='FileName']").val().trim() == '') {
                    alert("Enter File Name in uploaded file row!");
                    return;
                }
                var upldFile=$("#b_FileUpload tr").eq(i).find("input[name='AttachFile']").get(0);
                var filename=$("#b_FileUpload tr").eq(i).find("input[name='FileName']").val().trim();
                formData.append("PIAttachFiles[" + i + "].AttachFile", upldFile.files[0]);
                formData.append("PIAttachFiles[" + i + "].FileName", filename);
                upldfiles = i + 1;
            }
        };

        $('#dvLoading').show();

        if (upldfiles > 0)
        {
            $.ajax({
                async: false,
                type:"Post",
                url: "Upload_PIAttachment",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.Error != "") {
                        alert(response.Error);
                        filesave=false;
                    }
                    else {
                        fileObj = response.Data;
                    }
                },
                error: function () {
                    $('#dvLoading').hide();
                    alert("File not uploaded. Try Again!");
                    filesave = false;
                }
            })
        }

        if (!filesave) {
            $('#dvLoading').hide();
            return;
        }

        var url = SendList1;

        $.ajax(
            {
                type: "Post",
                url: url,
                data: JSON.stringify({ Grid: session, AttachModel: fileObj }),
                contentType: "application/json; charset=utf-8",
                dataType: "json", 	//Expected data format from server
                processdata: true, 	//True or False
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (json) {
                 if (json === "3")
                    {
                        alert('Duplicate Purchase Indent can not be created. Kindly Re-load to create indent again.');
                    }
                    else if(json === "4")
                    {
                        alert('Some Administrator Problem. Kindly Re-load Page. ')
                    }
                    else if (json === "5") {
                        alert('You can not add same item two time. You can remove and change its quantity.');
                    }
                    else if (json === "6") {
                        alert('Please fill all data.');
                    }
                    else if (json === "222") {
                        alert('You can not add items form multiple Item Groups.');
                    }
                    else if (json === "2") {
                        alert('Something Went Wrong');
                    }
                    else if (json.LastPIDate != undefined) {
                        alert('PI date must be greater than last PI date ' + json.LastPIDate);
                    }
                    else {
                        {
                            $("#trans_no").text(json.TransNo);
                            $("#myModal").modal('show');
                            $("#PurchaseIRNo").val('');
                            $("#PurchaseIRDate").datepicker("setDate", "");
                            $("#Project").prop('selectedIndex', 0);
                            $("#SendTo").prop('selectedIndex', 0);

                            $("#b_FileUpload tr:not(:first)").remove();
                            $("#b_FileUpload tr").find("input").val("");
                            //child = 0;
                            gridList = [];
                            $("#grid").empty();
                        }
                    }

                },
                error: function () {
                    alert('Error in Submit Data');
                }
                // When Service call fails
            })
    })

    $("body").on("click", "#removeItem", function () {
        var id = $(this).attr("uniqueId");

        for (var index = 0; index < gridList.length; index++) {
            if (gridList[index].Id == id) {

                gridList.splice(index, 1);
                $(this).parents("._tempRow").remove();
            }
        }

    })

    var uniqueId = null;
    getUniqueId = function () {
        if (!uniqueId) uniqueId = (new Date()).getTime();
        return uniqueId++;
    };

    BindDecisionDrpDown();
    FixType();

})

function ValidMaster() {

    var PurchaseRequisitionNo = $("#PurchaseIRNo").val();
    var Date = $("#PurchaseIRDate").val();

    var ProjectNo = $("#Project option:selected").val();
    var SendTo = $("#SendTo option:selected").val();
    var PurchaseType = $("#ddlPurchaseType option:selected").text();
    var Remarks = $("#txtRemarks").val();
    var PuchaseDelDate = $("#PurchaseIRDeliveryDate").val();
    var purchasedesecion = $("#ddlPurchasedecisionType").val();



    var rr = true;
    if (ProjectNo === "0") {
        $('#Project').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#Project').css('border-color', '');

    }
    if (PurchaseType === "Select Purchase Type" || PurchaseType =='' ) {
        $('#ddlPurchaseType').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlPurchaseType').css('border-color', '');

    }

    if (purchasedesecion === "Select Type" || purchasedesecion == '') {
        $('#ddlPurchasedecisionType').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlPurchasedecisionType').css('border-color', '');

    }


    if (SendTo === "0") {
        $('#SendTo').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#SendTo').css('border-color', '');

    }

    if (Date === "") {
        $('#PurchaseIRDate').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#PurchaseIRDate').css('border-color', '');
    }

    if (PuchaseDelDate === "") {
        $('#PurchaseIRDeliveryDate').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#PurchaseIRDeliveryDate').css('border-color', '');
    }

    if (PurchaseRequisitionNo === "") {
        $('#PurchaseIRNo').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#PurchaseIRNo').css('border-color', '');
    }

    if (Remarks === "") {
        $('#txtRemarks').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#txtRemarks').css('border-color', '');
    }



    return rr;

}


$(document).ready(function () {
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

});

function FixType()
{

     $("#ddlPurchasedecisionType").prop('disabled', true);

    $(document).on('change', '.setdelvry', function () {
        //clearSelected();

        var delvdate = $("#PurchaseIRDeliveryDate").val();
        var Pidate = $("#PurchaseIRDate").val();
        var startdt = GetDate(Pidate);
        var enddt = GetDate(delvdate);

        
        var startdt_new = new Date(startdt.split('/')[2], startdt.split('/')[1] - 1, startdt.split('/')[0]);

        
        var enddt_new = new Date(enddt.split('/')[2], enddt.split('/')[1] - 1, enddt.split('/')[0]);

        var timeDiff = Math.abs(enddt_new.getTime() - startdt_new.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
       // alert(diffDays);
        BindDecisionDrpDown();
        
        if (enddt_new.getTime() >= startdt_new.getTime())
        {
            debugger;
            
           // $("#ddlPurchaseType").prop('disabled', false);
            var SiteLimitVal = $('#hdn_sitepurchaseLimitValue').val();
            var SiteID = $('#hdn_sitepurchasevalue').val();
            var HOLimitValue = $('#hdn_HOepurchaseLimitValue').val();
            var HOId = $('#hdn_hopurchasevalue').val();

            if (diffDays <= SiteLimitVal) {
                $("#ddlPurchasedecisionType").find('option[value="' + SiteID + '"]').attr('selected', 'selected');
                
            }
            else if (diffDays > SiteLimitVal && diffDays <= HOLimitValue) {

                $("#ddlPurchasedecisionType").find('option[value="' + HOId + '"]').attr('selected', 'selected');

            }

             return true
                
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


function BindDecisionDrpDown()
{
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



function GetDate(str) {
   
    var arr = str.split(' ');
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    var i = 1; var j = 1;
    for (i; i <= months.length; i++) {
        if (months[i] == arr[1]) {
            j = i + 1 ;
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




