$(document).ready(function () {

    $(window).load(function () {
        $.ajax({
            type: 'Get',
            url: GetItemGroup,
            success: function (data) {


                $.each(data, function (i, itname) {
                    $("#Category").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');



                });
            },
            error: function (ex) {
                alert('Failed to retrieve ItemGroup.' + ex);
            }
        });





    });


    $("#Category").change(function () {
        $("#SubCategory").empty();
        $("#SubCategory").append($("<option></option").val("").html("Select Sub Category"));
        if ($('#Category option:selected').val() != 0) {
            $.ajax({

                type: 'POST',
                url: GetItemDetail,
                dataType: 'json',
                data: { GrpId: $("#Category").val() },
                success: function (data) {

                    $.each(data, function (i, itname) {
                        $("#SubCategory").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');



                    });

                }

            })
        }
    });


    $('#btnsave').click(function (e) {
        debugger;

        var V = Valid();

        if (V == false)
            return;

        var _griddata = gridToArray();

        var url = SaveEuipment;
        $.ajax({
            type: 'POST',
            url: url,
            data: JSON.stringify(_griddata),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            processdata: true,

            success: function (json) {
                debugger;

                if (json.Status == "1") {



                    alert("Data Saved Successfully");
                    window.location.href = redUrllnk;



                }

            },

        error: function () {
                alert('Error in Submit Data');
            }
        });
    });

    function gridToArray() {
        var obj = new Object();
        obj.ItemGroupId = $("#Category option:selected").val();
        obj.ItemId = $("#SubCategory option:selected").val();
        obj.ProjectId = $("#Location option:selected").val();
        obj.Model = $("#Model option:selected").text();
        obj.EquipmentStatus = $("#Status option:selected").text();
        obj.FuelType = $("#FuelType option:selected").text();
        obj.EquipmentCondition = $("#Condition option:selected").text();

        obj.EquipmentName = $("#txtEquipmentName").val();
        obj.OwnerName = $("#txtOwner").val();
        obj.EquipmentRegNo = $("#txtEquipmentRegNo").val();
        obj.PurchaseDate = $("#txtDateOfPurchase").val();
        obj.ManufactureName = $("#txtManufacturer").val();
        obj.BodyColor = $("#txtBodyColor").val();
        obj.BrandName = $("#txtBrand").val();
       
        return obj;

    }

    function Valid() {
        var Category = $("#Category option:selected").val();
        var SubCategory= $("#SubCategory option:selected").val();
        var Location = $("#Location option:selected").val();
        var EquipmentRegNo = $("#txtEquipmentRegNo").val();
        var EquipName = $("#txtEquipmentName").val();
        var rr = true;
        if (Category =="")
        {
        alert('Select the Category.');
        $('#Category').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Category').css('border-color', '');
        }

        if (SubCategory == "") {
            alert('Select the Sub Category.');
            $('#SubCategory').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#SubCategory').css('border-color', '');
        }

        if (Location == "") {
            alert('Select the Location.');
            $('#Location').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#Location').css('border-color', '');
        }

        if (EquipmentRegNo == "") {
            alert('Enter the Equipment Registration No.');
            $('#txtEquipmentRegNo').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#txtEquipmentRegNo').css('border-color', '');
        }

        if (EquipName == "") {
            alert('Enter the Equipment Registration No.');
            $('#txtEquipmentName').css('border-color', '#f0551b');

            rr = false;
        }
        else {
            $('#txtEquipmentName').css('border-color', '');
        }


        if (rr == false) {


            return false;
        }
        else {
            return true;
        }
    }




});