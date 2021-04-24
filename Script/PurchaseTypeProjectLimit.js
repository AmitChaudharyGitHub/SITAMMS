$(document).ready(function () {

    var siteelement = document.getElementById('ddlSitePurchaseType');
    var hoelement = document.getElementById('ddlHOPurchaseType');
    $("#ddlSitePurchaseType").prop('disabled', true);
    $("#ddlHOPurchaseType").prop('disabled', true);

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

                // $("#Projects").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Project.' + ex);
            }
        });
        return false;
    };
    GetPrj();



    $(window).load(function () {

        $.ajax({
            type: 'Get',
            url: GetPurchasRequestionType, // Calling json method
            success: function (data) {


                $.each(data, function (i, itname) {
                    $("#ddlSitePurchaseType").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');



                });

                siteelement.value = 2;

            },
            error: function (ex) {
                alert('Failed to retrieve Purchase Type.' + ex);
            }
        });

        $.ajax({
            type: 'Get',
            url: GetPurchasRequestionType, // Calling json method
            success: function (data) {


                $.each(data, function (i, itname) {
                    $("#ddlHOPurchaseType").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');



                });
                
                hoelement.value = 1;

            },
            error: function (ex) {
                alert('Failed to retrieve Purchase Type.' + ex);
            }
        });

    })


    $('#txtHOPurchaseLimitValue').change(function () {
        debugger;
        var Sitelimitval = $('#txtSitePurchaseLimitValue').val();
        var HOlimitval = $('#txtHOPurchaseLimitValue').val();
        var maxlmt = 365;

        if (parseFloat(HOlimitval) > parseFloat(Sitelimitval)) {

            if ((parseFloat(HOlimitval) > parseFloat(Sitelimitval)) && (parseFloat(HOlimitval) > parseFloat(maxlmt)))
            {
                alert("HO Limit value must be less than or equal to 365 days respectively.");
                $('#txtHOPurchaseLimitValue').val('');
                return false;
            }

            return true;

        }
        else {

            alert("HO Limit value must be greater than SITE Limit value.");
            $('#txtHOPurchaseLimitValue').val('');
            return false;
        }

    });


    $('#btnsave').click(function (e) {
        debugger;
        var V = Valid();

        if (V == false)
            return;
        
        var _griddata = gridToArray();
        console.log(_griddata);
     
        var url = SaveLimit;
        $.ajax({
            type: 'POST',
            url: url,
            data: JSON.stringify(_griddata),
            contentType: "application/json",
            dataType: "json",
            processdata: true,

            success: function (json) {
                debugger;

                if (json.Status == "1") {
                    alert("Data Saved Successfully");

                    //url: json.redidtUrl,
                    window.location.href = Reload;



                }
                else if (json.Status == "2")
                {
                    alert("Some Exception has occured.");
                }
                else if (json.Status == "3") {
                    alert("Project is already exist.");
                }
                else { }

                

            },







            error: function (ex) {
                debugger;

                var errors = ex.responseJSON;

                console.log(errors);


                // alert('Failed to retrieve Project Code.' + ex);
                alert('Error in Submit Data' + ex);
                debugger;
            }
        });
    });

    

    function gridToArray() {

        debugger;
        var obj1 = new Object();

        obj1.ProjectId = $("#Projects option:selected").val();
        obj1.EmengencyPurchaseType = $("#ddlSitePurchaseType option:selected").val();

        obj1.EmengencyLimitValue = $("#txtSitePurchaseLimitValue").val();
        obj1.NormalPurchaseType = $("#ddlHOPurchaseType option:selected").val();

        obj1.NormalLimitValue = $("#txtHOPurchaseLimitValue").val();

       

        
        return obj1;
    }

    function Valid() {

       var ProjectId = $("#Projects option:selected").val();
       var SitePurchaseType = $("#ddlSitePurchaseType option:selected").val();

       var SiteLimitValue = $("#txtSitePurchaseLimitValue").val();
       var HOPurchaseType = $("#ddlHOPurchaseType option:selected").val();

       var HOLimitValue = $("#txtHOPurchaseLimitValue").val();

        var rr = true;

        if (ProjectId == "" || ProjectId =="0") {
            alert('Select Project');
            $('#Projects').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Projects').css('border-color', '');
        }


        if (SiteLimitValue == "") {
            alert('Enter Site Purchase Limit Value.');
            $('#txtSitePurchaseLimitValue').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#txtSitePurchaseLimitValue').css('border-color', '');

        }

        if (HOLimitValue == "") {
            alert('Enter HO purchase Limit Value.');
            $('#txtHOPurchaseLimitValue').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#txtHOPurchaseLimitValue').css('border-color', '');

        }



        if (rr == false) {


            return false;
        }
        else {

            return true;
        }
    }

    $("#Projects").change(function ()
    {

        $.get(GetData, {
            ProjectId: $('#Projects option:selected').val()
        },
                           function (result) {
                               $('#formbody').show();
                               $('#formbody').html(result);

                           });
    });

   


});


