$(document).ready(function ()
{


    $(window).load(function () {
        $.ajax({
            type: 'Get',
            url: GetUniteData, // Calling json method
            success: function (data) {
               

                $.each(data, function (i, itname) {
                    $("#UniteTo").append('<option value="' + itname.Value + '">' +
                         itname.Text + '</option>');

                    $("#UniteConersersation").append('<option value="' + itname.Value + '">' +
                       itname.Text + '</option>');

                });


                // $("#Projects").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Unit.' + ex);
            }
        });
    });


    //window['GetItemGroup'] = function () {
    //    debugger;
    //    //$('#dvLoading').show();
       
    //    return false;
    //};

    //GetItemGroup();


    //$("#ItemGroups").change(function () {
    //    $("#Items").empty();
       
    //    $("#Items").append($("<option></option").val("").html("Select Item"));
    //    if ($('#ItemGroups option:selected').val() != 0) {

          
    //        $.ajax({
    //            type: 'POST',
    //            url: GetItemData, // Calling json method

    //            dataType: 'json',

    //            data: { GroupId: $("#ItemGroups").val() },
              
              
    //            success: function (names) {
                    
    //                 $.each(names, function (i, itnm) {
    //                    $("#Items").append('<option value="' + itnm.Value + '">' +
    //                         itnm.Text + '</option>');

    //                });


    //            },
    //            error: function (ex) {
    //                alert('Failed to retrieve Item Name.' + ex);
    //            }
    //        });
    //        return false;
    //    }
    //});





})