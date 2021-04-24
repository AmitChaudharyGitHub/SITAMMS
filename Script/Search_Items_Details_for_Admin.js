
    $(document).ready(function ()
    {



        window['GetPrj'] =    function (dateString)
        {  
            //$('#dvLoading').show();
            //$.ajax({
            //    type: 'POST',
            //    url: GetProjectByEmpId, // Calling json method

            //    dataType: 'json',
            //    data: { E: ss},
            //    complete: function () {
            //        $('#dvLoading').hide();
            //    },
            //    success: function (result) {
            //        var procemessage = "<option value='0'> Please wait...</option>";
            //        $("#Projects").html(procemessage).show();
            //        var markup = "<option value='0'>Select Project</option>";
            //        $("#Projects").html(markup).show();
            //        result = $.parseJSON(result)

            //        var selectedDeviceModel = $('#Projects');
            //        $.each(result, function (index, item)
            //        {
            //            selectedDeviceModel.append(
            //                $('<option/>', {
            //                    value: item.Value,
            //                    text: item.Text
            //                })
            //            );
            //        });

            //        $("#Projects").prop('selectedIndex', 1).trigger('change');
            //    },
            //    error: function (ex) {
            //        alert('Failed to retrieve Project.' + ex);
            //    }
            //});
            //return false;
        };
        GetPrj();


    }
        )

    $(document).ready(function () {
        //Country Dropdown Selectedchange event
        $("#ItemGroups").change(function () {
            $("#Items").empty();
            $("#grid").empty();
            $("#Items").append($("<option></option").val("").html("For All Items "));
            if($('#ItemGroups option:selected').val()!=0 )
            {
              
                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetItemByGroup, // Calling json method

                    dataType: 'json',

                    data: { Pid: $("#Projects").val(), Gid: $("#ItemGroups").val() },
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

                  
                    },
                    error: function (ex) {
                        alert('Failed to retrieve Item Name.' + ex);
                    }
                });
                return false;
            }
        });
        $("#Projects").change(function () {
           // alert("vx");
            $("#ItemGroups").empty();
            $("#grid").empty();
            $("#ItemGroups").append($("<option></option").val("").html("For All Item Groups"));
            if ($('#Projects option:selected').val() != 0) {

                $('#dvLoading').show();
                $.ajax({
                    type: 'POST',
                    url: GetItemGroup, // Calling json method

                    dataType: 'json',

                    data: { Pid: $("#Projects option:selected").val() },
                    // Get Selected Country ID.
                    complete: function () {
                        $('#dvLoading').hide();
                    },
                    success: function (names) {
                        names = $.parseJSON(names)


                        $.each(names, function (i, itname) {
                            $("#ItemGroups").append('<option value="' + itname.Value + '">' +
                                 itname.Text + '</option>');

                        });


                    },
                    error: function (ex) {
                        alert('Failed to retrieve Item Groups.' + ex);
                    }
                });
                return false;
            }
        });
        $("#Items").change(function () {
            $("#grid").empty();
        });
        //$("#Items").autocomplete({
        //    source: SearchItemByGroup,
        //    minLength: 2
        //})
    });

  


    function getblock()
    {
        //  alert("A");


        if ($("#Projects option:selected").val() != 0) {
            var url = StockGrid;

            $('#dvLoading').show();
            $.ajax({

                url: url,
                type: 'GET',
                data: {  PrjId: $("#Projects option:selected").val(), IG: $("#ItemGroups option:selected").val(), IT: $("#Items option:selected").val() },
                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (result) {
                   
                    $('#formbody').html(result);
                }
            });
            return false;
        }
        else
        {
            alert("Select Project");
        }

    }

