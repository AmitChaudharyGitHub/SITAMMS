$(document).ready(function () {

    $("#PurchaseIRDate").val('');

  $(".DatePicker").datepicker({
        dateFormat: 'dd M yy',
        changeMonth: true,
        changeYear: true,
        value: "",
        maxDate: new Date()
    });


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


                //$("#Project").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Projects.' + ex);
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



                },
                error: function (ex) {
                    alert('Failed to retrieve Purchase Type.' + ex);
                }
            });


        return false;
    };
  GetPrj();
})

$(document).ready(function () {
$("#Project").change(function () {
        $("#PurchaseIRNo").empty();
        if ($("#Project option:selected").val() !== 0) {
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetPurchaseIRNo, 

                dataType: 'json',

                data: { ProjectID: $("#Project").val(), OutPIType : $("#ddlPurchaseType").val() },
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
            return false;
        }
})

$("#ItemGroup").change(function () {
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

$("#ItemMaster").change(function () {

    $("#Unit").empty();
    if ($("#ItemMaster option:selected").val() !== 0) {
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


                $("#Unit").val(item.Unit);
                $("#Unit").attr("UNo", item.UnitNo);
                $("#LPRate").val(item.LPRate);
                $("#CRate").val(item.CRate);
            },
            error: function (ex) {
                alert('Failed to retrieve Unit' + ex);
            }
        });


        $.ajax({
            type: 'POST',
            url: GetCurrentStockData, 

            dataType: 'json',

            data: { PrjId: $("#Project option:selected").val(), IG: $("#ItemGroup option:selected").val(), itemid: $("#ItemMaster option:selected").val() },
            // Get Selected Country ID.

            success: function (balance1) {
                     $("#showBalanceData").text(balance1);
              
            },
            error: function (ex) {
                alert('Failed to retrieve Current Stock Value.' + ex);
            }
        });

     return false;
    }
})



function GetEmp1(data) {
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#SendTo").html(procemessage).show();
        var markup = "<option value='0'>Forward For Approval</option>";
        for (var x = 0; x < data.length; x++) {
           markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";}
        $("#SendTo").html(markup).show();
        $("#SendTo").prop('selectedIndex', 1);
}

$("#ddlPurchaseType").change(function ()
{
    $("#Project option:selected").prop("selected", false);
    $("#PurchaseIRNo").val('');
});

$("#PurchaseIRDate").change(function () {

    $("#PurchaseIRNo").empty();
    if ($("#Project option:selected").val() !== 0) {
        $('#dvLoading').show();
        $.ajax({
            type: 'POST',
            url: GetPurchaseIRNoOnly,

            dataType: 'json',

            data: { ProjectID: $("#Project").val(), OutPIType: $("#ddlPurchaseType").val(), Date: $("#PurchaseIRDate").val() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (obj) {
                obj = $.parseJSON(obj)
                $("#PurchaseIRNo").val(obj.P_No);
                

            },
            error: function (ex) {
                alert('Failed to retrieve PurchaseIR No.' + ex);
            }
        });
        return false;
    }


});

});



// adding record functionality .


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
        obj.Remarks = $("#Remark").val();


       

        if (obj.ItemGroupName === "Select Group") {
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

        debugger;
        gridList.push(obj);
        $("#grid").prepend("<tr class='danger _tempRow'><td>" + obj.ItemGroupName + "</td><td>" + obj.ItemName + "</td><td>" + obj.Unit + "</td><td>" + obj.DemandQty + "</td><td>" + obj.LastPurchaseRate + "</td><td>" + obj.CurrentRate + "</td><td>" + obj.CurrentValue + "</td><td><a href='#' id='removeItem' uniqueId='" + obj.Id + "'>Remove</a></td></tr>");
        debugger;


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


        return obj1;
    }


    $("#Submit").click(function () {


        if (gridList.length < 1) {
            alert("Add Any Item");
            return;
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







        var url = SaveOutPI;


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
                    
                    if (json === "2")
                    {
                        alert("Administration Problem.");
                    }
                    else if (json === "3") {
                        alert('Duplicate Purchase Indent can not be created. Kindly Re-load to create indent again.');
                    }
                    else if (json === "4") {
                        alert('Some Administrator Problem. Kindly Re-load Page. ')
                    }
                    else if (json == "-1") {
                        alert("Something went wrong!");
                    }
                    else
                    {
                        $("#trans_no").text(json.TransNo);
                        $("#myModal").modal('show');


                        $("#PurchaseIRNo").val('');
                        $("#PurchaseIRDate").datepicker("setDate", "");

                        $("#Project").prop('selectedIndex', 0);

                        $("#SendTo").prop('selectedIndex', 0);



                        //child = 0;
                        gridList = [];
                        $("#grid").empty();
                    }

                },
                error: function () {
                    alert('Error in Submit Data');
                }
                // When Service call fails
            })
    })

    $("body").on("click", "#removeItem", function () {
        debugger;
        var id = $(this).attr("uniqueId");

        for (var index = 0; index < gridList.length; index++) {
            if (gridList[index].Id == id) {

                gridList.splice(index, 1);
                $(this).parents("._tempRow").remove();
                debugger;
            }
        }

    })

    var uniqueId = null;
    getUniqueId = function () {
        if (!uniqueId) uniqueId = (new Date()).getTime();
        return uniqueId++;
    };




})

function ValidMaster() {

    var PurchaseRequisitionNo = $("#PurchaseIRNo").val();
    var Date = $("#PurchaseIRDate").val();

    var ProjectNo = $("#Project option:selected").val();
    var SendTo = $("#SendTo option:selected").val();
    var PurchaseType = $("#ddlPurchaseType option:selected").text();
    var Remarks = $("#txtRemarks").val();


    var rr = true;
    if (ProjectNo === "0") {
        $('#Project').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#Project').css('border-color', '');

    }
    if (PurchaseType === "Select Purchase Type") {
        $('#ddlPurchaseType').css('border-color', '#f0551b');
        rr = false;
    }
    else {
        $('#ddlPurchaseType').css('border-color', '');

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

// calculating value while on click event in rate.

$(document).on('keyup', "#Qty", function ()   // here change from #cartage to CartageRate
{
    GetValue();
});
$(document).on('keyup', "#CRate", function ()   // here change from #cartage to CartageRate
{
    GetValue();
});
function GetValue() {
    var Qt = 0;
    var currVal = 0;
    var cRt = 0;
    if ($("#Qty").val() != null)
        Qt = $("#Qty").val();
    if ($("#CRate").val() != null)
        cRt = $("#CRate").val();

    currVal = Qt * cRt;
    $("#CValue").val(currVal.toFixed(2));
}