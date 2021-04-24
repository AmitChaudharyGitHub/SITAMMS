$(document).ready(function () {
    GetProject();
    GetPendingDebitNote();
    GetApprovedDebitNote();
    // $(document).on('click', '.edit', EditDebitNote);
    $(document).on('click', '.approve', ApproveDebitNote);
    $(document).on('click', '#btnFinalApprove', FinalApproveDebitNote);
    $(document).on('change', '#Projects', function () {
        GetPendingDebitNote();
        GetApprovedDebitNote();
    });


    $(document).on('click', '.viewDetails', ViewVendorDebitNoteDetails);
});


function GetProject() {
    $('#dvLoading').show();
    $.ajax({
        type: 'POST',
        url: '/VendorWiseReport/GetProjectByEmpId', // Calling json method
        async: false,
        dataType: 'json',
        data: { E: ss },
        complete: function () {
            $('#dvLoading').hide();
        },
        success: function (result) {
            var procemessage = "<option value='0'> Please wait...</option>";
            $("#Projects").html(procemessage).show();
            var markup = "";
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

            $("#Projects").prop('selectedIndex', 0).trigger('change');
        },
        error: function (ex) {
            alert('Failed to retrieve Project.' + ex);
        }
    });
    return false;
};


function GetPendingDebitNote() {
    $.ajax({
        url: '/VendorDebitNote/GetPendingDebitNote',
        type: 'GET',
        dataType: 'html',
        data: { ProjectId: $("#Projects option:selected").val() },
        success: function (response) {
            $('#pendingDebitNotes').html(response);
        }
    });
}


function GetApprovedDebitNote() {
    $.ajax({
        url: '/VendorDebitNote/GetApprovedDebitNote',
        type: 'GET',
        dataType: 'html',
        data: { ProjectId: $("#Projects option:selected").val() },
        success: function (response) {
            $('#approvedDebitNotes').html(response);
        }
    });
}

function ApproveDebitNote() {
    //alert($(this).attr('id'));
    //$("#dialog-pending-debit-Note").attr('data', $(this).attr('id'));
    $("#dialog-pending-debit-Note").attr('data1', $(this).attr('data1'));

    var id = $(this).attr('data1');
    $.get('/VendorDebitNote/GetPO_details', { Id: id }, function (response) {
        $("#dialog-pending-debit-Note").dialog({
            title: 'Debit Note Details',
            autoOpen: false,
            resizable: false,
            height:450,
            width: 1100,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            open: function (event, ui) {
                $(this).html(response);
            }
        });
        //console.log(response);
        $("#dialog-pending-debit-Note").dialog('open');
    });
}

function FinalApproveDebitNote() {
    $("#dialog-pending-debit-Note").dialog('close');
    $.get('/VendorDebitNote/ApproveDebitNote', { TransId: $(this).attr('data') }, function (response) {
        if (response.Status == '1') {
            alert('Debit Note Approved.');
            location.href = response.Url;
        }
        else if (response.Status == '2')
            alert('Some error occur.');
        else {
            alert('Debit Note not Approved.');
        }
    });

}




function ViewVendorDebitNoteDetails() {
   
    $("#dialog-pending-debit-Note1").attr('data1', $(this).attr('data1'));

    var id = $(this).attr('data1');
    $.get('/VendorDebitNote/GetPO_details', { Id: id,ViewOnly:true }, function (response) {
        $("#dialog-pending-debit-Note").dialog({
            title: 'Debit Note Details',
            autoOpen: false,
            resizable: false,
            height: 450,
            width: 1100,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            open: function (event, ui) {
                $(this).html(response);
            }
        });
        //console.log(response);
        $("#dialog-pending-debit-Note").dialog('open');
    });
}