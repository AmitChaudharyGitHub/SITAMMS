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
   
    BindDdl('#Projects', GetProjectByEmpId, { E: ss }, "Project Name");

    BindDdl('#ItemGroup', GetGroup, {}, "Item Group");

    $("#ItemGroup").change(function () {
        $("#ItemMaster").empty();
        BindDdl('#ItemMaster', GetItemByGroup, { id: $("#ItemGroup").val() }, "Item Name");
    });

    $(document).on('click', '#btnSearch', function () {
        var projectId = $('#Projects option:selected').val();
        var itemGroupId = $('#ItemGroup option:selected').val();
        var itemCode = $('#ItemMaster option:selected').val();
        var financialYear = '';

        if (Validate(projectId, itemGroupId, itemCode)) {
            $.get(GetBudgetsForItem, { ProjectId: projectId, ItemGroupId: itemGroupId, ItemCode: itemCode, FinancialYear: financialYear }, function (response) {
                $('#formdata').empty();
                $('#formdata').html(response);
            });
        }
    });
    $('#Projects').change(function () {
        if ($('#Projects option:selected').val() != '') {
            BindDdl('#ddlFinancialYear', GetFinancialYears, { ProjectId: $('#Projects option:selected').val() },'Financial Year')
        }
    });

    $(document).on('click', '.lnkDetail', function (e) {
        e.preventDefault();
        alert($(this).attr('data'));
        $.get(GetBudgetDetails, { UId: $(this).attr('data') }, function (result) {
            $('#itemBudgetDetailBody').empty();
            $('#itemBudgetDetailBody').html(result);
            $('#myModal').modal('show');
        });
    });

    $('#btnCreateNew').click(function () {
        var projectId = $('#Projects option:selected').val();
        var itemGroupId = $('#ItemGroup option:selected').val();
        var itemCode = $('#ItemMaster option:selected').val();
        var financialYear = '';
        if (Validate(projectId, itemGroupId)) {
            projectId = projectId.replace('AHL', '111');
            location.href = CreateNewBudget + '?Pid=' + projectId + '&ICode=' + itemCode;
        }
    });

    $(document).on('click', '.update', function () {
        var data = $(this).attr('data');
        var projectId = $('#Projects option:selected').val();
        var fY = $('#fy_'+data).text();
        var itemCode = $('#itemCode_' + data).text();
        var totalQty = $('#TotalQty_' + data).val();
        var rate = $('#Rate_' + data).val();
        var amount = $('#Amt_' + data).val();
        var excessPer = $('#ExcessPer_' + data).val();
        var excessQty = $('#ExcessQty_' + data).val();
       
        if (totalQty == '') {
            alert('Total Qty can not be empty');
            return false;
        }
        if (rate == '') {
            alert('Rate can not be empty');
            return false;
        }

        if (amount == '') {
            alert('Amount can not be empty');
            return false;
        }
        if (excessPer == '') {
            alert('Excess Percentage can not be empty');
            return false;
        }
        if (excessQty == '') {
            alert('Excess Qty can not be empty');
            return false;
        }
        
        if (projectId == '') {
            alert('Please select Project');
            return false;
        }

        $('.qty_' + data).each(function () {
            if($(this).val()=='')
            {
                alert('Month qty can not be empty');
                return false;
            }
        });

        var obj = [];
        obj.push(fY);
        obj.push(itemCode);
        obj.push(totalQty);
        obj.push(rate);
        obj.push(amount);
        obj.push(excessPer);
        obj.push(excessQty);
        obj.push(projectId);
        
        $('.qty_' + data).each(function () {
            obj.push($(this).val());
        });

        $.post(UpdateBudget, { PostData: obj }, function (response) {
            if (response.Status!='') {
                alert(response.Message);
            }
        });
        
    });

    $(document).on('change', '[id^=TotalQty_],[id^=Rate_]', function () {
        var data = $(this).attr('id').split('_')[1];

        var totalQty = $('#TotalQty_' + data).val();
        var rate = $('#Rate_' + data).val();
        var excessPer = $('#ExcessPer_' + data).val();

        if (totalQty != '') {
            var monthQty = totalQty / 12;
            $('.qty_' + data).val(monthQty.toFixed(3));
        }
        else {
            $('.qty_' + data).val('');
        }

        if (totalQty != '' && rate != '') {
            $('#Amt_' + data).val((totalQty * rate).toFixed(2));
        }
        else {
            $('#Amt_' + data).val('');
        }

        if (excessPer != '' && totalQty != '') {
            $('#ExcessQty_'+data).val(((totalQty * excessPer) / 100).toFixed(3));
        }
        else {
            $('#ExcessQty_'+data).val('');
        }
       

       
    });

    $(document).on('change','[id^=ExcessPer_]', function () {
        var data = $(this).attr('id').split('_')[1];
        var excessPer = $(this).val();
        var totalQty = $('#TotalQty_' + data).val();
        if (excessPer != '' && totalQty != '') {
            $('#ExcessQty_' + data).val(((totalQty * excessPer) / 100).toFixed(3))
        }
        else {
            $('#ExcessQty_' + data).val('');
        }
    });


    $(document).on('change', '[class*="qty_"]', function () {
        
        var data = $(this).attr('class').split('_')[1];
        var totalQty = $('#TotalQty_' + data).val();
        var tQty = 0;
        $('.qty_'+data).each(function () {
            tQty = tQty + parseFloat($(this).val() == '' ? 0 : $(this).val());
        });
       
        if (tQty > totalQty) {
            alert('Total month qty can not be greter than total qty.');
            $(this).val('');
        }
    });

});



function Validate(projectId, itemGroupId) {
    if (projectId == '') {
        alert('Please select Project');
        return false;
    }
    if (itemGroupId == '') {
        alert('Please select Item Group');
        return false;
    }
    
    return true;
}