

    $(document).ready(function ()
    {



        window['GetPrj'] =    function (dateString)
        {  
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
                    $.each(result, function (index, item)
                    {
                        selectedDeviceModel.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });

                    $("#Projects").prop('selectedIndex', 1);
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
        $("#ItemGroups").change(function () {
            $("#Items").empty();
            $("#Items").append($("<option></option").val("").html("Select Item Name"));
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemByGroup, // Calling json method

                dataType: 'json',

                data: { id: $("#ItemGroups").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (names) {
                    names = $.parseJSON(names)


                    $.each(names, function (i, itname) {
                        $("#Items").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');

                    });
                  
                    FindAllGroupLastOpeningInSite();
                },
                error: function (ex) {
                    alert('Failed to retrieve Item Name.' + ex);
                }
            });
            return false;
        });
        $("#Projects").change(function () 
        {
            $("#grid").empty();
        });
    });



    $(document).ready(function () {
        //Item Dropdown Selectedchange event
        $("#Items").change(function () {
            //alert("A");
            $("#Unit").empty();
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemDetail, // Calling json method

                dataType: 'json',

                data: { id: $("#Items").val() },
                // Get Selected Country ID.
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (item) {
                    item = $.parseJSON(item)
                   
                 
                    $("#Unit").val(item.Unit);
                    $("#Unit").attr('uid',item.UnitNo);
                },
                error: function (ex) {
                    alert('Failed to retrieve Make.' + ex);
                }
            });
            return false;
        })
    });

  

    function SaveStock()
    {
        // alert("A");



        var url = SaveOpening;
        if(  $('#Projects option:selected').val()!=0)
        {
            if($('#Items option:selected').val()!=0 )
            {
                if( $("#Opening").val()!=0 &&   $("#Opening").val()!="")
                { 
                    if($("#Rate").val()==null)
                        $("#Rate").val()=0.00;
                    $('#dvLoading').show();
                    $.ajax({
                       
                        url: url,
                        type: 'GET',
                        data: { Project: $('#Projects option:selected').val(), ItemGroup: $('#ItemGroups option:selected').val(), Item: $('#Items option:selected').val(), Unit: $("#Unit").attr('uid'),Opening: $("#Opening").val(),Rate: $("#Rate").val() },
                        complete: function () {
                            $('#dvLoading').hide();
                        },
                        success: function (result) {
                            if (result == "1") {
                    
                                $("#myModal").modal('show');
                                $("#Opening").val('');
                                FindOneItemLastOpeningInSite();
                            }
                        }
                    });
                    return false;
                }
                else
                {
                    alert("Give Quantity");
                }
            }
            else
            {
                alert("Enter Item");
            }
        }
        else
        {
            alert("Enter Project");
        }
    }
window['FindAllGroupLastOpeningInSite'] =    function ()
{
  

    var url = GetAllGroupLastOpeningInSite;
    if($('#Projects option:selected').val()!=0 && $('#ItemGroups option:selected').val()!=0)
    { 
        $('#dvLoading').show();
        $.ajax({

            url: url,
            type: 'GET',
            data: { Project: $('#Projects option:selected').val(), ItemGroup: $('#ItemGroups option:selected').val() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                $("#grid").empty();
                item = $.parseJSON(result)
                $.each(item, function (i, itname) {
                        
                    $("#grid").prepend("<tr class='danger _tempRow'><td>" + itname.ProjectName + "</td><td>" + itname.ItemGroupName + "</td><td>" + itname.ItemName + "</td><td>" + itname.tblItemMasterStock.Opening + "</td><td>" + itname.tblItemMasterStock.Rate + "</td></tr>");
                })
            }
        });
        return false;
    }

}
window['FindOneItemLastOpeningInSite'] =    function ()
{
        
    
    var url = GetOneItemLastOpeningInSite;
    if($('#Projects option:selected').val()!=0 && $('#Items option:selected').val()!=0)
    {
        $('#dvLoading').show();
        $.ajax
            ({

                url: url,
                type: 'GET',
                data: { Project: $('#Projects option:selected').val(), Item: $('#Items option:selected').val() },
                complete: function ()
                {
                    $('#dvLoading').hide();
                },
                success: function (result) 
                {   $("#grid").empty();
                    item = $.parseJSON(result)
                    $.each(item, function (i, itname) {
                       
                        $("#grid").prepend("<tr class='danger _tempRow'><td>" + itname.ProjectName + "</td><td>" + itname.ItemGroupName + "</td><td>" + itname.ItemName + "</td><td>" + itname.tblItemMasterStock.Opening + "</td><td>" + itname.tblItemMasterStock.Rate + "</td></tr>");
                    })
                }
            });
        return false;
    }

}


    $(document).ready(function () {
        $(".DatePicker").datepicker({
            dateFormat: 'dd M yy',
            changeMonth: true,
            changeYear: true,
        }).val('');
    });
        
        
