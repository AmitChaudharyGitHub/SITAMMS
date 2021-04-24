$(document).ready(function () {

    var app = angular.module('myApp', []);
    app.controller('myCtrl', function ($scope) {
        $scope.Data = [];
        $scope.GetDisapprovedData = function () {
            var project=$('#DisapprovedProject').val();
            var purchaseType=$('#ddlDisapprovedPurchaseType').val();
            var vendorId=$('#ddlDisapprovedVendorlist').val();
            var poNo = $('#txtDisapprovedPONumber').val();
            if (project == '') {
                alert('Please select project.');
                return false;
            }
            $('#disapprovedProgress').show();

            $.get(getDisapprovedGRN, {ProjectId:project,PurchaseType:purchaseType,VendorId:vendorId,PONo:poNo}, function (response) {
                if (response.Status == 1) {
                    console.log(response.Data);
                    $scope.Data = response.Data;
                    $scope.DGPType = response.PType;
                }
                else {
                    $scope.Data = [];
                }
                $scope.$apply();
                $('#disapprovedProgress').hide();
            });
            
        }

    });

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
                $(".project").html(procemessage).show();
                var markup = "<option value='0'>Select Project</option>";
                $(".project").html(markup).show();
                result = $.parseJSON(result)

                var selectedDeviceModel = $('.project');
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
                // $("#Project").prop('selectedIndex', 2).trigger('change');
                // $("#ddlPurchaseType").prop('selectedIndex', 1).trigger('change');
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
        $(".ddlPurchaseType").html(procemessage).show();
        var markup = "<option value='0'>Select Purchase Type</option>";
        for (var x = 0; x < data.length; x++) {
            markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";

        }
        $(".ddlPurchaseType").html(markup).show();

    }

    $('.project').change(function () {
        if ($(this).val() == '0') {
            ClearDdl('.ddlVendorlist', 'Vendor Name');
        }
        else {
            BindDdl('.ddlVendorlist', getVendorList, { ProjectId: $(this).val() }, 'Vendor Name');
        }
    });

    $('#btnPendingSearch').click(SearchPending);
    $('#btnPartialSearch').click(SearchPartial);
    $('#btnUpdatedSearch').click(SearchUpdated);

});


function SearchPending() {
    $("#dvLoading").show();
    debugger;
    if ($('#ddlPendingPurchaseType option:selected').val() != 0) {
        if ($('#ddlPendingPurchaseType option:selected').val() == "5" || $('#ddlPendingPurchaseType option:selected').val() == "6")
        {
            $('#tbl-pending-grn').hide(); $('#tbl-partial-grn').hide();
            $('#tbl-tranpending-grn').show(); $('#tbl-tranpartial-grn').show();
            $('#Pend').hide(); $('#G_Pr').hide(); $('#Up_G').hide();
            $('#Pend_T').show(); $('#G_PrT').show(); $('#Up_TG').show();
            //Pending 
            var PendingList_TVM;
            $(function () {
                PendingList_TVM = {
                    dt2: null,
                    init: function () {
                        dt = $('#tbl-tranpending-grn').DataTable({

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
                                "url": GetPending_TransferGRN,
                                "type": "POST",
                                "datatype": "json",
                                "data": function (data) {

                                    data.ProjId = $('#PendingProject option:selected').val();
                                    data.PurchaseType = $('#ddlPendingPurchaseType option:selected').val();
                                    data.VendorId = $('#ddlPendingVendorlist option:selected').val();
                                    data.PONo = $('#txtPendingPONumber').val();
                                }
                            },
                            "columns": [
                                   { "title": "S No", "data": "Id", "searchable": true },
                                        { "title": "Site Name", "data": "ProjectName", "searchable": true },
                                        { "title": "Transfer Site Name ", "data": "TransferprojectName", "searchable": true },
                                        { "title": "Transfer No.", "data": "Transferno", "searchable": true },
                                        { "title": "Transfer Date", "data": "TransferDate" },
                                        //{ "title": "Transfer Date of Creation.", "data": "Transferdateofcreation" },
                                         { "title": "Gate Out No.", "data": "GetOutNo" },
                                          { "title": "Gate Out Date.", "data": "GetOutDate" },

                                        {
                                            "title": "Actions",
                                            "data": "Transferno",

                                            "searchable": false,
                                            "sortable": false,
                                            "render": function (data, type, full, meta) {
                                                return '<a href="/GateEntries/TCreateNew?TransferNo=' + data + '&TransferType=' + $('#ddlPendingPurchaseType option:selected').val() + '" class="editAsset" >Add GRN</a>';
                                            }
                                        }
                            ]
                        });
                    },
                    refresh: function () {
                        dt2.ajax.reload();
                    }
                }
                PendingList_TVM.init();
            });
        }
        else {
            $('#tbl-pending-grn').show(); $('#tbl-partial-grn').show();
            $('#tbl-tranpending-grn').hide(); $('#tbl-tranpartial-grn').hide();
            $('#Pend').show(); $('#G_Pr').show(); $('#Up_G').show();
            $('#Pend_T').hide(); $('#G_PrT').hide(); $('#Up_TG').hide();
            var PendingListVM;
            $(function () {
                PendingListVM = {
                    dt1: null,
                    init: function () {
                        dt1 = $('#tbl-pending-grn').DataTable({

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
                                "url": getPendingGRN,
                                "type": "POST",
                                "datatype": "json",
                                "data": function (data) {

                                    data.ProjId = $('#PendingProject option:selected').val();
                                    data.PurchaseType = $('#ddlPendingPurchaseType option:selected').val();
                                    data.VendorId = $('#ddlPendingVendorlist option:selected').val();
                                    data.PONo = $('#txtPendingPONumber').val();
                                }
                            },
                            "columns": [
                                   { "title": "S No", "data": "Id", "searchable": true },
                                        { "title": "Vendor Name", "data": "VendorName", "searchable": true },
                                        { "title": "Purchase Order No.", "data": "PurchaseOrderNo", "searchable": true },
                                        { "title": "Purchase Order Date", "data": "PODate", "searchable": true },
                                        { "title": "Purchase Indent Date", "data": "PIDate" },
                                        { "title": "Purchase Indent No.", "data": "PurReqNo" },

                                        {
                                            "title": "Actions",
                                            "data": "PurchaseOrderNo",
                                            "searchable": false,
                                            "sortable": false,
                                            "render": function (data, type, full, meta) {
                                                return '<a href="/GateEntries/CreateNew?PONo=' + data + '" class="editAsset" >Add GRN</a>';
                                            }
                                        }
                            ]
                        });
                    },
                    refresh: function () {
                        dt1.ajax.reload();
                    }
                }
                PendingListVM.init();
            });
        }
    }
}


function SearchPartial() {
    $("#dvLoading").show();

    if ($('#ddlPartialPurchaseType option:selected').val() != 0) {

        if ($('#ddlPartialPurchaseType option:selected').val() == "5" || $('#ddlPartialPurchaseType option:selected').val() == "6") {
            //Partial
            var PartialList_TVM;
            $(function () {
                PartialList_TVM = {
                    dt3: null,
                    init: function () {
                        dt3 = $('#tbl-tranpartial-grn').DataTable({

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
                                "url": GetPartial_TransferGRN,
                                "type": "POST",
                                "datatype": "json",
                                "data": function (data) {

                                    data.ProjId = $('#PartialProject option:selected').val();
                                    data.PurchaseType = $('#ddlPartialPurchaseType option:selected').val();
                                    data.VendorId = $('#ddlPartialVendorlist option:selected').val();
                                    data.PONo = $('#txtPartialPONumber').val();
                                }
                            },
                            "columns": [
                                   { "title": "S No", "data": "Id", "searchable": true },
                                        { "title": "Site Name", "data": "ProjectName", "searchable": true },
                                        { "title": "Transfer Site Name ", "data": "TransferprojectName", "searchable": true },
                                        { "title": "Transfer No.", "data": "Transferno", "searchable": true },
                                        { "title": "Transfer Date", "data": "TransferDate" },
                                       // { "title": "Transfer Date of Creation.", "data": "Transferdateofcreation" },
                                         { "title": "Gate Out No.", "data": "GetOutNo" },
                                          { "title": "Gate Out Date.", "data": "GetOutDate" },

                                        {
                                            "title": "Actions",
                                            "data": "Transferno",

                                            "searchable": false,
                                            "sortable": false,
                                            "render": function (data, type, full, meta) {
                                                return '<a href="/GateEntries/TCreateNew?TransferNo=' + data + '&TransferType=' + $('#ddlPartialPurchaseType option:selected').val() + '" class="editAsset" >Add GRN</a>';
                                            }
                                        }
                            ]
                        });
                    },
                    refresh: function () {
                        dt3.ajax.reload();
                    }
                }
                PartialList_TVM.init();
            });
        }
        else {
            var PartialListVM;
            $(function () {
                PartialListVM = {
                    dt: null,
                    init: function () {
                        dt = $('#tbl-partial-grn').DataTable({

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
                                "url": getPartialGRN,
                                "type": "POST",
                                "datatype": "json",
                                "data": function (data) {

                                    data.ProjId = $('#PartialProject option:selected').val();
                                    data.PurchaseType = $('#ddlPartialPurchaseType option:selected').val();
                                    data.VendorId = $('#ddlPartialVendorlist option:selected').val();
                                    data.PONo = $('#txtPartialPONumber').val();
                                }
                            },
                            "columns": [
                                   { "title": "S No", "data": "Id", "searchable": true },
                                        { "title": "Vendor Name", "data": "VendorName", "searchable": true },
                                        { "title": "Purchase Order No.", "data": "PurchaseOrderNo", "searchable": true },
                                        { "title": "Purchase Order Date", "data": "PODate", "searchable": true },
                                        { "title": "Purchase Indent Date", "data": "PIDate" },
                                        { "title": "Purchase Indent No.", "data": "PurReqNo" },
                                       {
                                           "title": "Actions",
                                           "data": "PurchaseOrderNo",
                                           "searchable": false,
                                           "sortable": false,
                                           "render": function (data, type, full, meta) {
                                               return '<a href="/GateEntries/CreateNew?PONo=' + data + '" class="editAsset" >Add GRN</a>';
                                           }
                                       }
                            ]
                        });
                    },
                    refresh: function () {
                        dt.ajax.reload();
                    }
                }
                PartialListVM.init();
            })

        }
    }
}


function SearchUpdated() {
    $("#dvLoading").show();

    if ($('#ddlUpdatedPurchaseType option:selected').val() != 0) {

        // for Pending GRNList

        if ($('#ddlUpdatedPurchaseType option:selected').val() == "5" || $('#ddlUpdatedPurchaseType option:selected').val() == "6") {
            var UpdatedList_TVM;
            $(function () {
                UpdatedList_TVM = {
                    dt4: null,
                    init: function () {
                        dt4 = $('#tbl-transfupdate-grn').DataTable({

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
                                "url": GetUpdated_TransferGRN,
                                "type": "POST",
                                "datatype": "json",
                                "data": function (data) {

                                    data.ProjId = $('#UpdatedProject option:selected').val();
                                    data.PurchaseType = $('#ddlUpdatedPurchaseType option:selected').val();
                                    data.VendorId = $('#ddlUpdatedVendorlist option:selected').val();
                                    data.PONo = $('#txtUpdatedPONumber').val();
                                }
                            },
                            "columns": [
                                   //{ "title": "S No", "data": "Id", "searchable": true },
                                        { "title": "Site Name", "data": "ProjectName", "searchable": true },
                                        { "title": "Transfer Site Name ", "data": "TransferprojectName", "searchable": true },
                                        { "title": "Transfer No.", "data": "Transferno", "searchable": true },
                                        { "title": "Transfer Date", "data": "TransferDate" },
                                       // { "title": "Transfer Date of Creation.", "data": "Transferdateofcreation" },
                                         { "title": "Gate Out No.", "data": "GetOutNo" },
                                          { "title": "Gate Out Date.", "data": "GetOutDate" },
                                           { "title": "GRN No.", "data": "GRNNo" },
                                           { "title": "GRN Created Date", "data": "GRNCreatedDate" },
                                           { "title": "GRN Time", "data": "GRNTime" }

                            ]
                        });
                    },
                    refresh: function () {
                        dt4.ajax.reload();
                    }
                }
                UpdatedList_TVM.init();
            })
        }
        else {
            var UpdatedListVM;
            $(function () {
                UpdatedListVM = {
                    dt: null,
                    init: function () {
                        dt = $('#tbl-update-grn').DataTable({

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
                                "url": getUpdatedGRN,
                                "type": "POST",
                                "datatype": "json",
                                "data": function (data) {

                                    data.ProjId = $('#UpdatedProject option:selected').val();
                                    data.PurchaseType = $('#ddlUpdatedPurchaseType option:selected').val();
                                    data.VendorId = $('#ddlUpdatedVendorlist option:selected').val();
                                    data.PONo = $('#txtUpdatedPONumber').val();
                                }
                            },
                            "columns": [
                                   { "title": "S No", "data": "Id", "searchable": true },
                                        { "title": "Vendor Name", "data": "VendorName", "searchable": true },
                                        { "title": "Purchase Order No.", "data": "PurchaseOrderNo", "searchable": true },
                                        { "title": "Purchase Order Date", "data": "PODate", "searchable": true },
                                        { "title": "Purchase Indent Date", "data": "PIDate" },
                                        { "title": "Purchase Indent No.", "data": "PurReqNo" },
                                        { "title": "GRN No.", "data": "GRNNo" },
                                        { "title": "GRN Created Date", "data": "GRNDate" },
                                        { "title": "GRN Time", "data": "GRNTime" }

                            ]
                        });
                    },
                    refresh: function () {
                        dt.ajax.reload();
                    }
                }
                UpdatedListVM.init();
            })
        }
    }
}

