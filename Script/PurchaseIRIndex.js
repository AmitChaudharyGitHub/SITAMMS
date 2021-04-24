    //$(document).ready(function () {
    //    $('input:radio[name=f]').change(function () {
    //        //if ($(this).val() == 'All') {
    //        //    $("#L1").text("Local Purchase List For Admin");
             
    //        //}
    //        if ($(this).val() == 'C') {
    //            $("#L1").text("  Local Purchase ");
           
    //        }
    //        if ($(this).val() == 'A') {
    //            $("#L1").text(" Local Purchase  For Approval ");
               
    //        }
    //        // GetPrj();
    //    });
    //});
  

    $(document).ready(function () {
        $("#empName1").change(function () {
            // alert("A");
            GetPrj();
        });

        $("#Prj").change(function () {
            // alert("A");
            getblock1();
        });
    });
    $(document).ready(function ()
    {
    
        window['lol'] = function() { alert('lol');   };
       
        window['GetPrj'] =    function (dateString) 
        {
            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetProjectByEmpId, // Calling json method

                dataType: 'json',               
                data: { E: $("#empName1 option:selected").val() },
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (result) {
                   // alert(result);
                    var procemessage = "<option value='0'> Please wait...</option>";
                    $("#Prj").html(procemessage).show();
                    var markup = "<option value='0'>Select Project</option>";
                    $("#Prj").html(markup).show();
                    result = $.parseJSON(result)
                    var List1 = $.parseJSON(result.List1);
                    var selectedDeviceModel = $('#Prj');
                    $.each(List1, function (index, item)
                    {
                        selectedDeviceModel.append(
                            $('<option/>', {
                                value: item.Value,
                                text: item.Text
                            })
                        );
                    });
                    $("#Prj").prop('selectedIndex', 1);
                    //$.each(partes, function (i, state)
                    //{                       
                    //markup += "<option value=" + state.Value + ">" + state.Text + "</option>";
                    //});
                    //$("#Prj").html(markup).show();
                },
                error: function (ex) {
                    alert('Failed to retrieve Project.' + ex);
                }
            });
            return false;
        };
        GetPrj();
        window['getblock1'] = function () {

            //alert("A");
            //alert($("#Comp").val());
            if ($("#empName1 option:selected").val() == null || $("#empName1 option:selected").val() == "0" || $("#empName1 option:selected").val() == "") {
                alert("Select Employee");
                return;
            }
            if ($("#Prj option:selected").val() == null || $("#Prj option:selected").val() == "0")
            {
                alert("Select Project");
                return;
            }
            var url = Grid2;

            $('#dvLoading').show();
        
            $.ajax({

                url: url,
                type: 'GET',
                data: { Search: "Last", PrjId: $("#Prj option:selected").val(), f: $("input[name='f']:checked").val(), E: $("#empName1 option:selected").val(), Comp: $("input[name='Comp']:checked").val() },
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (result) {
                    $('#formbody').html(result);
                }
            });
            return false;

        }
        getblock1();
    }
        )
    $(document).ready(function () {
        $(".DatePicker").datepicker({
            dateFormat: 'dd M yy',
            changeMonth: true,
            changeYear: true,
        }).val('');
    });
        
function getblock()
{
    //  alert("A");
    if ($("#empName1 option:selected").val() == null || $("#empName1 option:selected").val() == "0" || $("#empName1 option:selected").val() == "") {
        alert("Select Employee");
        return;
    }
    if ($("#Prj option:selected").val() == null || $("#Prj option:selected").val() == "0") {
        alert("Select Project");
        return;
    }


    var url = Grid2;

    $('#dvLoading').show();
    $.ajax({

        url: url,
        type: 'GET',
        data: { Search: "Date", fromDate: $('#fromDate').val(), toDate: $('#toDate').val(), PrjId: $("#Prj option:selected").val(), f: $("input[name='f']:checked").val(), E: $("#empName1 option:selected").val(), Comp: $("input[name='Comp']:checked").val() },
        complete: function () {
            $('#dvLoading').hide();
        },
        success: function (result) {
            $('#formbody').html(result);
        }
    });
    return false;

}
   
