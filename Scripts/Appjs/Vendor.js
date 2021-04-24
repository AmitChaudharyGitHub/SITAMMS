/// <reference path="Item.js" />

    // Fill Grid At Load Time
    $(document).ready(function () {
        getJob();
        function getJob() {
            $.post('@Url.Content("~/tblVendorsMasters/Grid/")', function (data) {
                $('#formbody').html("");
                $('#formbody').html(data);
            });
        }
       
    });
// Fill Grid for Paging
$(document).on("click", "#myPager a", function () {
    //var url = "/tblItemMasters/Grid/";
    $.ajax({
            
        url: $(this).attr("href"),
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#formbody').html(result);
        }
    });
    return false;
});


function getDetails() {
    this.event.preventDefault();
    var url = "/tblVendorsMasters/Details/";
    $('#progress').show();
    $.ajax({
        url: url,
        type: 'GET',
           
        complete: function () {
            $('#progress').hide();
        },
        success: function (result) {
            $('#formbody').html(result);
        }
    });
    return false;
}
// Fill Grid for List Selection
function getblock(_siteid) {
    var url = "/tblVendorsMasters/Grid/";
    $('#progress').show();
    $.ajax({
        url: url,
        type: 'GET',
        data: { ItemGroupId: $('#ddlItemG').val() },
        complete: function () {
            $('#progress').hide();
        },
        success: function (result) {
            $('#formbody').html(result);
        }
    });
    return false;
}
