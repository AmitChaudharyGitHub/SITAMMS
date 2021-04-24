$(document).ready(function () {
    BindDdl('#ddlProjects', projectUrl, {}, 'Project', true);
  
    var app = angular.module('myApp', []);
    app.controller('myCtrl', function ($scope) {
        $scope.Progress = false;
        $scope.Data = [];           
        $scope.GetData = function () {
           
             var projectId = $('#ddlProjects option:selected').val();
             var PONo = $('#txtPONo').val();
           
            if (projectId == '') {
                alert('Please select Project');
                return false;
            }
            $scope.Progress = true;
            $.get(GetPOData, { ProjectId: projectId,PONO:PONo }, function (response) {
                if (response.Status == 1) {
                    $scope.Data = response.Data;
                    
                }
                else if (response.Status == 3) {
                    alert('Some error occur ' + response.Error);
                }
                $scope.Progress = false;
                $scope.$apply();
            });
        }
        $scope.PODetail = [];
        $scope.GetPPDetails = function (PurchaseOrderNo) {
            //$.get(GetPoDetails, { PONO: PurchaseOrderNo}, function (responce) {
            //    var PONo = $('#txtPONo').val();
            //    if(response.Status==1){
            //        $scope.PODetail = response.PODetail;
            //        $scope.$apply();  
            //    }
            //    else if (response.Status == 3) {
            //        alert('Some error occur ' + response.Error);
            //    }
            //});

            location.href = GetPoDetails + '?PONO=' + PurchaseOrderNo;

        }

    });
});
