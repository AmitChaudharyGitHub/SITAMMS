$(document).ready(function () {
    BindDdl('#ddlProjects', projectUrl, {}, 'Project', true);
    $(document).on('change', '#ddlProjects', function () {
        BindDdl('#ddlVendor', BindVendor, { ProjectId: $('#ddlProjects option:selected').val() }, 'Vendor', true);
    })

    $('.datepicker').datepicker({
        format: 'dd-mm-yyyy',
        autoclose: true,
        todayHighlight: true
    });
    $('.datepicker1').datepicker({
        format: 'dd-mm-yyyy',
        autoclose: true,
        todayHighlight: true
    });

    var app = angular.module('myApp', []);
    app.controller('myCtrl', function ($scope) {
        $scope.Data = [];
        $scope.Progress = false;
        $scope.GetData = function () {
            debugger;
            var projectId = $('#ddlProjects option:selected').val();
            var vendorid = $('#ddlVendor option:selected').val();
            var pono = $('#txtPONo').val();
            var fromDate = $('#txtFromDate').val();
            var toDate = $('#txtToDate').val();
            var reciv = $('#ddlReceived').val();

            if (projectId == '') {
                alert('Please select Project');
                return false;
            }
            $scope.Progress = true;
            $.get(GetPOData, { ProjectId: projectId, VendorId: vendorid, PONO: pono, FromDate: fromDate, ToDate: toDate, Received: reciv }, function (response) {
                if (response.Status == 1) {
                    $scope.Data = response.Data;
                    BindDdl('.ddlPOCloser', GetResonCloser, {}, 'Reason', true);
                }
                else if (response.Status == 3) {
                    alert('Some error occur ' + response.Error);
                }
                $scope.Progress = false;
                $scope.$apply();
            });
        }
        $scope.PODetail = [];
        $scope.GetPODetail = function (UId) {

            $.get(GetVar, { UId: UId }, function (response) {

                location.href = GetPoDetails + '?q=' + response;
            });


        }

    });
});
