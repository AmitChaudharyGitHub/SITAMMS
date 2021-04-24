$(document).ready(function () {

    var app = angular.module('myApp', []);
    app.controller('myCtrl', function ($scope) {
        $scope.Progress1 = false;
        $scope.Progress2 = false;
        $scope.Progress3 = false;
        $scope.Progress4 = false;
        $scope.Progress5 = false;
        $scope.Progress6 = false;
        $scope.Progress7 = false;
        $scope.Progress8 = false;
        $scope.Progress9 = false;
        $scope.Progress10 = false;
        $scope.Progress11 = false;

        $scope.RoleId = 0;
        $scope.IsPMC = false;
        $scope.IsPIC = false;
        $scope.PendingPIAtPIC = [];
        $scope.PendingPIAtCluster = [];

        $scope.PendingPOAtHO = [];
        $scope.PendingPOAtSite = [];

        $scope.PendingPOAtPIC = [];
        $scope.PendingPOAtCluster = [];
        $scope.PendingPOAtPMC = [];
        $scope.DisapprovedPOAtPurchaser = [];
        $scope.DisapprovedPOAtHO = [];
        $scope.PendingReleasePO = [];

        $scope.PendingTransferAtPIC = [];

        $scope.GetPendingPIAtPIC = function () {
            $scope.Progress1 = true;
            $.get(GetPI, { Status: 'Pending', Stage: 'PIC' }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingPIAtPIC = response.Data;
                }
                $scope.Progress1 = false;
                $scope.$apply();
            });
        };

        $scope.GetPendingPOAtHO = function () {
            $scope.Progress9 = true;
            $.get(GetPI, { Status: 'HO', Stage: 'Pending', PIType:1 }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingPOAtHO = response.Data;
                }
                $scope.Progress9 = false;
                $scope.$apply();
            });
        };

        $scope.GetPendingPOAtSite = function () {
            $scope.Progress10 = true;
            $.get(GetPI, { Status: 'Site', Stage: 'Peding', PIType: 1 }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingPOAtSite = response.Data;
                }
                $scope.Progress10 = false;
                $scope.$apply();
            });
        };

        $scope.GetPendingPIAtCluster = function () {
            $scope.Progress2 = true;
            $.get(GetPI, { Status: 'Pending', Stage: 'Cluster' }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingPIAtCluster = response.Data;
                   
                }
                $scope.Progress2 = false;
                $scope.$apply();
            });
        }
        $scope.GetPendingPOAtPIC = function () {
            $scope.Progress3 = true;
            $.get("GetPOPICPending", { Status: 'Pending', Stage: 'PIC' }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingPOAtPIC = response.Data;
                    $scope.IsPIC = response.IsPIC;
                    console.log($scope.IsPIC);
                }
                $scope.Progress3 = false;
                $scope.$apply();
            });
        };
        $scope.GetPendingPOAtCluster = function () {
            $scope.Progress4 = true;
            $.get("GetPOClusterPending", { Status: 'Pending', Stage: 'Cluster' }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingPOAtCluster = response.Data;
                    //console.log(response.Data);
                }
                $scope.Progress4 = false;
                $scope.$apply();
            });
        }
        $scope.GetPendingPOAtPMC = function () {
            $scope.Progress5 = true;
            $.get("GetPOPMCPending", { Status: 'Pending', Stage: 'PMC' }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingPOAtPMC = response.Data;
                    $scope.IsPMC = response.IsPMC;
                    $scope.RoleId = response.Role;
                }
                $scope.Progress5 = false;
                $scope.$apply();
            });
        }


        $scope.GetDisapprovedPOAtPurchaser = function () {
            $scope.Progress6 = true;
            $.get("GetPurchaserDisaprove", { Type: 'PO', Status: 'Disapproved', Stage: 'Purchaser' }, function (response) {
                if (response.Status == 1) {
                    $scope.DisapprovedPOAtPurchaser = response.Data;
                }
                $scope.Progress6 = false;
                $scope.$apply();
            });
        }

        //////////////////////////
        $scope.GetDisapprovedPOAtHO = function () {
            $scope.Progress6 = true;
            $.get(GetPO, { Type: 'PO', Status: 'Disapproved', Stage: 'HO' }, function (response) {
                if (response.Status == 1) {
                    $scope.DisapprovedPOAtHO = response.Data;
                }
                $scope.Progress6 = false;
                $scope.$apply();
            });
        }

        $scope.GetPendingReleasePO = function () {
            $scope.Progress7 = true;
            $.get("GetPONoRelease", {Status: 'NoRelease', Stage: '' }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingReleasePO = response.Data;
                }
                $scope.Progress7 = false;
                $scope.$apply();
            });
        }


        $scope.GetPendingTransferAtPIC = function () {
            $scope.Progress8 = true;
            $.get(GetPO, {Type:'Transfer', Status: 'Pending', Stage: 'PIC' }, function (response) {
                if (response.Status == 1) {
                    $scope.PendingTransferAtPIC = response.Data;
                }
                $scope.Progress8 = false;
                $scope.$apply();
            });
        }



        $scope.GetPendingPIAtPIC();
        $scope.GetPendingPIAtCluster();

        $scope.GetPendingPOAtHO();
        $scope.GetPendingPOAtSite();
        //po
        $scope.GetPendingPOAtPIC();
        $scope.GetPendingPOAtCluster();
        $scope.GetPendingPOAtPMC();
        $scope.GetDisapprovedPOAtPurchaser();
        $scope.GetDisapprovedPOAtHO();
        $scope.GetPendingReleasePO();
        $scope.GetPendingTransferAtPIC();


    });
   

});



