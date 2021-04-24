$(document).ready(function () {


    window['GetPrj'] = function (dateString) {

        $.ajax({
            type: 'POST',
            url: GetProjectByEmpId, // Calling json method

            dataType: 'json',
            data: { E: ss },
            complete: function () {

            },
            success: function (result) {
                var procemessage = "<option value='0'> Please wait...</option>";
                $("#Project").html(procemessage).show();
                var markup = "<option value='0'>Select Project</option>";
                $("#Project").html(markup).show();
                result = $.parseJSON(result)

                var selectedDeviceModel = $('#Project');
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
                alert('Failed to retrieve Project.' + ex);
            }
        });

        $.ajax({
            type: 'POST',
            url: getPurchaseTypeForLoggedEmployee,

            dataType: 'json',

            data: {},


            success: function (obj) {
                obj = $.parseJSON(obj)




                GetPurchaseTypeList(obj);
                // $("#Project").prop('selectedIndex', 1).trigger('change');
                //$("#ddlPurchaseType").prop('selectedIndex', 1).trigger('change');
            },
            complete: function () {
                $('#loading').hide();
            },
            error: function (ex) {
                // alert('Failed to retrieve Project Code.' + ex);
            }
        });


        return false;
    };
    GetPrj();
    function GetPurchaseTypeList(data) {
        debugger;
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#ddlPurchaseType").html(procemessage).show();
        var markup = "<option value='0'>Select Purchase Type</option>";
        for (var x = 0; x < data.length; x++) {
            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

        }
        $("#ddlPurchaseType").html(markup).show();

    }

    //$("#ddlPurchaseType").change(function () {
    //    $("#dvLoading").show();

    //    if ($('#ddlPurchaseType option:selected').val() != 0) {
    //        $.get(getpendingMRN,
    //            {
    //                ProjId: $('#Project option:selected').val(),
    //                PurchaseType: $('#ddlPurchaseType option:selected').val()
    //            },
    //            function (result) {
    //                $('#formbody').show();
    //                $('#formbody').html(result);
    //                $("#dvLoading").hide();
    //            });




    //        //$.get(getUpdatedMRN,
    //        //    {
    //        //        ProjId: $('#Project option:selected').val(),
    //        //        PurchaseType : $('#ddlPurchaseType option:selected').val()
    //        //    },
    //        //    function (result) {
    //        //        $('#formbodyUpdatedMRN').show();
    //        //        $('#formbodyUpdatedMRN').html(result);
    //        //        $("#dvLoading").hide();
    //        //    });


    //    }
    //});


    $("#Project").change(function () {
        if ($('#Project option:selected').val() != 0) {
            $.ajax({
                type: 'POST',
                url: getVendorList, // Calling json method

                dataType: 'json',
                data: { ProjectId: $('#Project option:selected').val() },
                complete: function () {

                },
                success: function (result) {
                    var procemessage = "<option value='0'> Please wait...</option>";
                    $("#ddlVendorlist").html(procemessage).show();
                    var markup = "<option value='0'>Select Vendor Name</option>";
                    $("#ddlVendorlist").html(markup).show();
                    result = $.parseJSON(result)

                    var selectedDeviceModel = $('#ddlVendorlist');
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
                    alert('Failed to retrieve Vendor Name.' + ex);
                }
            });
        }

    });

    $("#frmid_drp").hide();

    $("#ddlPurchaseType").change(function () {

        if ($('#ddlPurchaseType option:selected').val() != 0) {
            if ($('#ddlPurchaseType option:selected').val() == 5 || $('#ddlPurchaseType option:selected').val() == 6) {
                $("#vndr_drp").hide(); $("#frmid_drp").show();

             
                if ($('#ddlPurchaseType option:selected').val() != 0) {
                    $.ajax({
                        type: 'POST',
                        url: BindFromSiteList, // Calling json method

                        dataType: 'json',
                        data: { PurchaseTypeId: $('#ddlPurchaseType option:selected').val(), ProjectId: $('#Project option:selected').val() },
                        complete: function () {

                        },
                        success: function (result) {
                            var procemessage = "<option value='0'> Please wait...</option>";
                            $("#ddlFromSiteName").html(procemessage).show();
                            var markup = "<option value='0'>Select Site Name</option>";
                            $("#ddlFromSiteName").html(markup).show();
                            result = $.parseJSON(result)

                            var selectedDeviceModel = $('#ddlFromSiteName');
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
                            alert('Failed to retrieve Site Name.' + ex);
                        }
                    });
                }

            }
            else {
                $("#vndr_drp").show(); $("#frmid_drp").hide();


            }

        }
       




      

    });





  



})


$("#btn_Search").click(function (e) {

    debugger;
    if ($('#Project option:selected').val() != 0) {
        if ($('#ddlPurchaseType option:selected').val() != 0) {

            if ($("#ddlPurchaseType option:selected").val() != '5' && $("#ddlPurchaseType option:selected").val() != '6') {
                debugger;
                $("#nottr_mrn").show(); $("#tr_mrn").hide();
                $('#trtbleid').hide(); $('#assets-data-table').show();

                $.get(getpendingMRN,
                   {
                       ProjId: $('#Project option:selected').val(),
                       PurchaseType: $('#ddlPurchaseType option:selected').val(),
                       VendorId: $('#ddlVendorlist option:selected').val(),
                       PurchaseOrderNo: $("#txtPurchaseOrderNo").val()
                   },
                   function (result) {
                       $('#formbody').show();
                       $('#formbody').html(result);
                       $("#dvLoading").hide();
                   });

                // for Update MRN list

                var assetListVM;
                $(function () {
                    assetListVM = {
                        dt: null,
                        init: function () {
                            dt = $('#assets-data-table').DataTable({

                                "paging": true,
                                "bInfo": false,
                                "bLengthChange": true,
                                "destroy": true,
                                "processing": true, // for show processing bar
                                "serverSide": true, // for process on server side
                                //  "filter": true,
                                "orderMulti": false, // for disable multi column order
                                "searching": true,

                                "dom": '<"top"i>rt<"bottom"lp><"clear">', // for hide default global search box
                                "ajax": {
                                    "url": "/MRNListClassification/UpdatedMRN_NewFormat",
                                    "type": "POST",
                                    "datatype": "json",
                                    "data": function (data) {

                                        data.ProjId = $('#Project option:selected').val();
                                        data.PurchaseType = $('#ddlPurchaseType option:selected').val();
                                        data.VendorId = $('#ddlVendorlist option:selected').val();
                                        data.PurchaseOrderNo = $("#txtPurchaseOrderNo").val();

                                    }
                                },
                                "columns": [
                                       { "title": "S No", "data": "Sno", "searchable": true },
                                            { "title": "Vendor Name", "data": "Name", "searchable": true },
                                            { "title": "Purchase Order No.", "data": "PurchaseOrderNo", "searchable": true },
                                            { "title": "Purchase Indent No.", "data": "IndentRefNo", "searchable": true },
                                            { "title": "Purchase Indent Date", "data": "PIDate" },
                                            { "title": "Gate Entry Date", "data": "GateEntryMidDate" },
                                            { "title": "Gate Entry No.", "data": "GateEntryMidNo" },
                                            { "title": "MRN Date.", "data": "MRNDate" },
                                            { "title": "MRN No.", "data": "MRN_No_New" },
                                              { "name": "", "data": "ProjectNo", "visible": false },
                                                { "name": "", "data": "PurchasePIC_Type", "visible": false },
                                             {
                                                 "title": "Actions",
                                                 "data": "MRN_No_New",
                                                 "searchable": false,
                                                 "sortable": false,
                                                 "render": function (data, type, full, meta) {
                                                     return '<a href="/MRNListClassification/GetMRSDetails?MRN_NO=' + data + '&PurTypeId=' + $('#ddlPurchaseType option:selected').val() + '" class="editAsset" target="_blank">Create MRS</a>';
                                                 }
                                             }
                                ]
                            });
                        },
                        refresh: function () {
                            dt.ajax.reload();
                        }
                    }
                    assetListVM.init();
                })


                // end here update mrn list
            }
            else if ($("#ddlPurchaseType option:selected").val() == '5' || $("#ddlPurchaseType option:selected").val() == '6') {
                $("#nottr_mrn").hide(); $("#tr_mrn").show(); debugger;
                $('#trtbleid').show(); $('#assets-data-table').hide();

                $.get(getpendingMRNTRN,
              {
                  ProjId: $('#Project option:selected').val(),
                  PurchaseType: $('#ddlPurchaseType option:selected').val(),
                  fromSiteId: $('#ddlFromSiteName option:selected').val(),
                  PurchaseOrderNo: $("#txtPurchaseOrderNo").val()
              },
              function (result) {
                  $('#formbody').show();
                  $('#formbody').html(result);
                  $("#dvLoading").hide();
              });

                // for update MRN Transfer List

                var uptrnsList_tVM;
                $(function () {
                    uptrnsList_tVM = {
                        dt2: null,
                        init: function () {
                            debugger;
                            dt2 = $('#trtbleid').DataTable({

                                "paging": true,
                                "bInfo": false,
                                "bLengthChange": true,
                                "destroy": true,
                                "processing": true, // for show processing bar
                                "serverSide": true, // for process on server side
                                //  "filter": true,
                                "orderMulti": false, // for disable multi column order
                                "searching": true,

                                "dom": '<"top"i>rt<"bottom"lp><"clear">', // for hide default global search box
                                "ajax": {
                                    "url": getUpdatedMRNTRN,
                                    "type": "POST",
                                    "datatype": "json",
                                    "data": function (data) {

                                        data.ProjId = $('#Project option:selected').val();
                                        data.PurchaseType = $('#ddlPurchaseType option:selected').val();
                                        data.fromSiteId = $('#ddlFromSiteName option:selected').val();
                                        data.PurchaseOrderNo = $("#txtPurchaseOrderNo").val();

                                    }
                                },
                                "columns": [
                                           //{ "title": "S No", "data": "Sno", "searchable": true },
                                            { "title": "From Site Name", "data": "ProjectName", "searchable": true },
                                            { "title": "Transfer Order No", "data": "IntraTransferNumber", "searchable": true },
                                            { "title": "Transfer Date", "data": "TransferDate" },
                                            { "title": "Gate Entry Date", "data": "GateEntryMidDate" },
                                            { "title": "Gate Entry No.", "data": "GateEntryMidNo" },
                                            { "title": "MRN Date.", "data": "MRNDate" },
                                            { "title": "MRN No.", "data": "MRN_No_New" },
                                            { "name": "", "data": "ProjectId", "visible": false },

                                             {
                                                 "title": "Actions",
                                                 "data": "MRN_No_New",
                                                 "searchable": false,
                                                 "sortable": false,
                                                 "render": function (data, type, full, meta) {
                                                     return '<a href="/MRNListClassification/GetMRSDetails?MRN_NO=' + data + '&PurTypeId=' + $('#ddlPurchaseType option:selected').val() + '" class="editAsset" target="_blank">Create MRS</a>';
                                                 }
                                             }
                                ]
                            });
                        },
                        refresh: function () {
                            dt2.ajax.reload();
                        }
                    }
                    uptrnsList_tVM.init();
                })


                // End here.


            }

        }
        else {
            alert('Select Purchase Type.');
        }

    }
    else {
        alert('Select project.');
    }


});