$(document).ready(function () {
    var ss = '@Html.Raw(Json.Encode(ViewBag.EmpId))';
    $("#fromdate").datepicker();
    $("#todate").datepicker();

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

                $("#Projects").prop('selectedIndex', 1).trigger('change');
            },
            error: function (ex) {
                alert('Failed to retrieve Project.' + ex);
            }
        });
        return false;
    };


    GetPrj();
    $('#btn_Receipt_Search').click(function (e) {
       
        e.preventDefault();
        if ($('#Projects option:selected').val() == '0') {
            alert('Please select project');
            return false;
        }       
        $.get(GetReceiptVsPO,
            { ProjectId: $('#Projects option:selected').val(), ItemGroupId: $('#ItemGroupId option:selected').val(), ItemId: $('#item_names option:selected').val(), VendorId: $('#Vendor_Names option:selected').val(), PONo: $('#PONos option:selected').text(), FromDate: $('#fromdate').val(), ToDate: $('#todate').val() },
            function (result) {
                $('#DIV_Receipt_Report').html(result);
            });
    });

    $('#ItemGroupId').change(function () {
        $('#dvLoading').show();
        $.ajax({
            type: 'Get',
            url: '/PuchaseOrder_ReceiptReport/GetGroupItems', // Calling json method
            dataType: 'json',
            data: { GroupId: $('#ItemGroupId option:selected').val() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                var procemessage = "<option value=''> Please wait...</option>";
                $("#item_names").html(procemessage).show();
                var markup = "<option value=''>Select Item Name</option>";
                $("#item_names").html(markup).show();
                result = $.parseJSON(result)

                var selectedDeviceModel = $('#item_names');
                $.each(result, function (index, item) {
                    selectedDeviceModel.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Item Name.' + ex);
            }
        });
    });

    $('#Projects').change(function () {
        $('#dvLoading').show();
        $.ajax({
            type: 'Get',
            url: GetPONumber, // Calling json method
            dataType: 'json',
            data: { ProjectId: $('#Projects option:selected').val() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                var procemessage = "<option value=''> Please wait...</option>";
                $("#PONos").html(procemessage).show();
                var markup = "<option value=''>Select PO Number</option>";
                $("#PONos").html(markup).show();
                result = $.parseJSON(result)

                var selectedDeviceModel = $('#PONos');
                $.each(result, function (index, item) {
                    selectedDeviceModel.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });
            },
            error: function (ex) {
                alert('Failed to retrieve PO number.' + ex);
            }
        });

        $('#dvLoading').show();
        $.ajax({
            type: 'Get',
            url: '/PuchaseOrder_ReceiptReport/GetPOVendors', // Calling json method
            dataType: 'json',
            data: { ProjectId: $('#Projects option:selected').val() },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                var procemessage = "<option value=''> Please wait...</option>";
                $("#Vendor_Names").html(procemessage).show();
                var markup = "<option value=''>Select Vendor Name</option>";
                $("#Vendor_Names").html(markup).show();
                result = $.parseJSON(result)

                var selectedDeviceModel = $('#Vendor_Names');
                $.each(result, function (index, item) {
                    selectedDeviceModel.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });
            },
            error: function (ex) {
                alert('Failed to retrieve vendor name.' + ex);
            }
        });

        $('#dvLoading').show();
        $.ajax({
            type: 'Get',
            url: '/PuchaseOrder_ReceiptReport/GetItemGroups', // Calling json method
            dataType: 'json',
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {
                var procemessage = "<option value=''> Please wait...</option>";
                $("#ItemGroupId").html(procemessage).show();
                var markup = "<option value=''>Select Item Group</option>";
                $("#ItemGroupId").html(markup).show();
                result = $.parseJSON(result)

                var selectedDeviceModel = $('#ItemGroupId');
                $.each(result, function (index, item) {
                    selectedDeviceModel.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Item Group Name.' + ex);
            }
        });
        return false;
    });

});