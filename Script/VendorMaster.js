
$(document).ready(function () {



});

function submitForm() {
    debugger;
    var model = new Object();
    model.TransID = "";
    model.PRJID = $('#PRJID').val();
    model.RegistrationType = $('#RegistrationType').val();
    model.Name = $("#ContactPerson").val();
    model.Address = $("#Address").val();
    model.Country = $("#ddlCountry option:selected").val();
    model.State = $("#ddlState option:selected").val();
    model.City = $("#ddlCity option:selected").val();
    model.MobileNo = $("#MobileNo").val();
    model.Email = $("#Email").val();
    model.Status = $("#Status").val();
    model.PanNo = $("#PanNo").val(); 
    model.TinNo = $("#TinNo").val();
    model.PinCode = $("#PinCode").val();
    model.PhoneNo = $("#PhoneNo").val(); 
    model.BlackListStatus = $("[name='BlackListStatus']:checked").val();

    if (model.PRJID == null) {
        $('#PRJID').css('border-color', '#f0551b');
        alert('Kindly Select Project');
        return false;
    }
    else {
        $('#PRJID').css('border-color', '');
    }
    
    if (model.RegistrationType == null) {
        $('#RegistrationType').css('border-color', '#f0551b');
        alert('Kindly Select Registration Type');
        //return false;
    }
    else {
        $('#RegistrationType').css('border-color', '');
    }

    //if (model.Name.trim() == "") {
    //    $('#ContactPerson').css('border-color', '#f0551b');
    //    alert('Kindly Enter Contact Person Name');
    //    //return false;
    //}
    //else {
    //    $('#ContactPerson').css('border-color', '');
    //}

    //if (model.TinNo.trim() == "") {
    //    $('#TinNo').css('border-color', '#f0551b');
    //    alert('Kindly Enter Tin No.');
    //    //return false;
    //}
    //else {
    //    $('#TinNo').css('border-color', '');
    //}

    //if (model.Address.trim() == "") {
    //    $('#Address').css('border-color', '#f0551b');
    //    alert('Kindly Enter Address.');
    //    //return false;
    //}
    //else {
    //    $('#Address').css('border-color', '');
    //}


    //if (model.State == null) {
    //    $('#ddlState').css('border-color', '#f0551b');
    //    alert('Kindly Select State');
    //    //return false;
    //}
    //else {
    //    $('#ddlState').css('border-color', '');
    //}






    window['GetVend'] = function (dateString) {
        debugger;

        $.ajax({
            type: 'POST',
            url: GetVendorForDetail, // Calling json method
            dataType: 'json',
            data: JSON.stringify({ tblVendorMaster: model }),
            success: function (data) {
                alert(data);
            },
            failure: function (errMsg) {
                alert(errMsg);
            }
         });
        return false;

    };

    


}