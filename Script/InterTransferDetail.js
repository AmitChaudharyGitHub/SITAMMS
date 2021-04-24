$(document).ready(function () {

    $.ajax({
        type: 'Post',
        url: GetForwardTo, // Calling json method
        dataType: 'json',
        data: { TransferNo: $("#hdnTransferTransID").val() },
        success: function (data) {

            var markup = "<option value='0'>Select Forward To </option>";
            $("#ddlForwardTo").html(markup).show();


            data = $.parseJSON(data)


            var selectedDeviceModel = $('#ddlForwardTo');
            $.each(data, function (index, item) {
                selectedDeviceModel.append(
                    $('<option/>', {
                        value: item.Value,
                        text: item.Text
                    })
                );
            });


            //$.each(data, function (i, itname) {
            //    $("#ddlForwardTo").append('<option value="' + itname.Value + '">' +
            //         itname.Text + '</option>');



            //});



        },
        error: function (ex) {
            alert('Failed to retrieve User Type.' + ex);
        }
    });
    GetTotalValue();

    HideandDisplay();


    $("#SubmitA").click(function (e) {
        //alert("A");
        //$("#Submit").click(function () {

        if ($('#ddlForwardTo option:selected').val() == '0') {
            alert('Please select Forward to');
            return false;
        }

        if ($('#txtPICRemarks').val() == '') {
            alert('Please enter remarks');
            return false;
        }
        
        var otArr = [];
        var values = LPToArray();

        Valid = values[0];


        // alert(otArr);


        //var validout = valimas();
        //if (Valid == false || validout == false)
        //    return;
        otArr = values[1];
        var obj1 = getMaster();


        // alert(obj1);
        var session =
            {
                'Child': otArr,
                'Master': obj1
            };





        // alert(session);

        var url = UpdateTransferList;
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
                        CleardValue();
                        $("#myModal").modal('show');
                        window.location.href = redurl;


                    }

                },
                error: function () {
                    alert('Error in Submit Data');
                }
                // When Service call fails
            })
    })



});


$(document).on('keyup', "td[class^='DeliveryQty_']", function () {

    var EstimatedAmt = 0; var BQt = 0;
    var delivryQty = $(this).find('input').val();
    var str = $(this).attr('class');
    var a = str.substring(12);

    var StockQty = $(".StockQty_" + a).find('input').val();

    var DeliveryRate = $(".DeliveryRate_" + a).find('input').val() ? $(".DeliveryRate_" + a).find('input').val() : 0;


    if (+delivryQty <= +StockQty) {

        BQt = parseFloat(parseFloat(StockQty) - parseFloat(delivryQty));
        EstimatedAmt = parseFloat(parseFloat(delivryQty) * parseFloat(DeliveryRate));

        $(".BalanceQty_" + a).find('input').val(BQt);
        $(".EstimatedValue_" + a).find('input').val(EstimatedAmt.toFixed(2));



    }
    else {
        alert("Delivery Qty can not greater than Stock Qty.")
        $(".DeliveryQty_" + a).find('input').val('');
    }

    GetTotalValue();
});

$(document).on('keyup', "td[class^='DeliveryRate_']", function () {

    var EstimatedAmt = 0;
    var delivryRate = $(this).find('input').val();
    var str = $(this).attr('class');
    var a = str.substring(13);
    var StockQty = $(".StockQty_" + a).find('input').val();

    var delivryQty = $(".DeliveryQty_" + a).find('input').val() ? $(".DeliveryQty_" + a).find('input').val() : 0;

    if (+delivryQty <= +StockQty) {
        EstimatedAmt = parseFloat(parseFloat(delivryQty) * parseFloat(delivryRate));

        $(".EstimatedValue_" + a).find('input').val(EstimatedAmt.toFixed(2));


    }
    else {
        alert("Delivery Qty can not greater than Stock Qty.")
        $(".DeliveryQty_" + a).find('input').val('');
    }



    GetTotalValue();








});


function GetTotalValue() {

    var T = 0;
    $("td[class^='EstimatedValue_").each(function () {

        if ($(this).find('input').val() != null) {
            var str = $(this).attr('class');
            var a = str.substring(15);

            var A = $('td.EstimatedValue_' + a).find('input').val();

            // var A = $(this).find('input').val()

            T = +T + +A;
        }
    });

    $("#lblTotalValue").text(T.toFixed(2));


}




function getMaster() {


    var obj1 = new Object();

    obj1.TransId = $("#hdnTransID").val();
    obj1.ForwardToStore = $('#ddlForwardTo option:selected').val();
    obj1.PICRemarks = $('textarea#txtPICRemarks').val();
    obj1.PICApprovalStatus = "Approve";
    obj1.InterTransferNumber = $("#hdnTransferTransID").val();


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
            obj.DeliveryQty = $(this).children().eq(7).find('input').val();
            if (obj.DeliveryQty != null && obj.DeliveryQty != 0) {
                ValidChild = true;
                obj.TransId = $(this).children().eq(0).attr('uid');

                //obj.ApprovedQty = $(this).children().eq(5).find('input').val();
                //obj.ApprovedQty = +(obj.ApprovedQty) + +(obj.CurrentQty);
                obj.ItemGroupNo = $(this).children().eq(1).find('input').val();
                obj.ItemNo = $(this).children().eq(2).find('input').val();

                obj.PICApprovalQty = $(this).children().eq(7).find('input').val();
                obj.PICBalancedQty = $(this).children().eq(8).find('input').val();
                obj.PICApprovalRate = $(this).children().eq(10).find('input').val();
                obj.PICEstimatedValue = $(this).children().eq(11).find('input').val();



                otArr.push(obj);



            }
        }
    })
    return [ValidChild, otArr];

}

function HideandDisplay() {
    $("td[class^='DeliveryRate_").each(function () {
        debugger;
        var delivryRate = $(this).find('input').val();
        var str = $(this).attr('class');
        var a = str.substring(13);

        var EmpType = $("#hdnLoggedType").val();
        if (EmpType == "Emp") {

            $(this).find('input').prop("disabled", true);
            $(".DeliveryQty_" + a).find('input').prop("disabled", true);
            $("#iddropdown").hide();
            $("#Rmrks").hide();
            $("#SubmitA").hide();

        }
        else if (EmpType == "PIC") {

            $(this).find('input').prop("disabled", false);
            $("#iddropdown").show();
            $("#Rmrks").show();
            $("#SubmitA").show();

        }

        else {

            $(this).find('input').prop("disabled", true);
            $(".DeliveryQty_" + a).find('input').prop("disabled", true);
            $("#iddropdown").hide();
            $("#Rmrks").hide();
            $("#SubmitA").hide();

        }


    });




}

function CleardValue() {

    $('#ddlForwardTo option:selected').prop("selected", false);
    $('textarea#txtPICRemarks').val('');


}


