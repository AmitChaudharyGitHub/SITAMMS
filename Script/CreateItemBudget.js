$(document).ready(function () {
  
    $.ajaxSetup({
        url: '',
        global:true,
        beforeSend: function (xhr) {
            $('#loadingModal').modal('show');
        },
        complete: function (xhr, status) {
            $('#loadingModal').modal('hide');
        }
    });


    $('#Rate,#TotalQuantity').change(function () {
        var rate = $('#Rate').val();
        var quantity = $('#TotalQuantity').val();
        var percentage = $('#ExcessPercentage').val();

        if (quantity != '') {
            var monthQty = quantity / 12;
            $('.qty_'+data).val(monthQty.toFixed(3));
        }
        else {
            $('.qty_'+data).val('');
        }

        if (percentage != ''&& quantity!='') {
            $('#ExcessQuantity').val(((quantity * percentage) / 100).toFixed(3));
        }
        else {
            $('#ExcessQuantity').val('');
        }
        if(rate!='' && quantity!=''){
            $('#Amount').val((rate * quantity).toFixed(2));
        }
        else {
            $('#Amount').val('');
        }
    });

    $('#ExcessPercentage').change(function () {
        var percentage = $('#ExcessPercentage').val();
        var quantity = $('#TotalQuantity').val();
        if (percentage != '' && quantity)
            $('#ExcessQuantity').val(((quantity*percentage)/100).toFixed(3));
        else
            $('#ExcessQuantity').val('');
    });

    $('#btnSubmit').click(function (e) {
        SubmitData();
    });

    $('.qty').change(function () {
        var quantity = $('#TotalQuantity').val();
        var totalQty = 0;
        $('.qty').each(function () {
            totalQty = totalQty+parseFloat($(this).val() == '' ? 0 : $(this).val());
        });
        //alert(totalQty);
        if (totalQty > quantity) {
            alert('Total month qty can not be greter than total qty.');
            $('.qty').val('');
        }
    });

   
});

function SubmitData() {
    e.preventDefault();
    if ($('#TotalQuantity').val() == '') {
        alert('Please enter Total Quantity');
        return false;
    }
    if ($('#Rate').val() == '') {
        alert('Please enter rate');
        return false;
    }
    if ($('#Amount').val() == '') {
        alert('Amount can not be empty');
        return false;
    }
    if ($('#FinancialYear').val() == '') {
        alert('Financial Year can not be empty');
        return false;
    }
    if ($('#ExcessPercentage').val() == '') {
        alert('Please enter excess percentage');
        return false;
    }
    if ($('#ExcessQuantity').val() == '') {
        alert('Excess Quantity can not be empty');
        return false;
    }
    $('.qty').each(function () {
        if ($(this).val() == '') {
            alert('Month Qty can not be empty');
            return false;
        }
    });


    $('#FinancialYear').removeAttr('disabled');
    $('#Amount').removeAttr('disabled');
    $('#ExcessQuantity').removeAttr('disabled');
    $('#frm').submit();

}

function Validate(projectId, itemGroupId, itemCode) {
    if (projectId == '') {
        alert('Please select Project');
        return false;
    }
    if (itemGroupId == '') {
        alert('Please select Item Group');
        return false;
    }
    if (itemCode == '') {
        alert('Please select Item');
        return false;
    }
    return true;
}