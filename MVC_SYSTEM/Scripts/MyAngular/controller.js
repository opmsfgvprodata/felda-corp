// <reference path="../angular.js" />     
/// <reference path="../angular.min.js" />      
/// <reference path="../angular-animate.js" />      
/// <reference path="../angular-animate.min.js" />     
var app;

(function () {
    app = angular.module("DispositionReportModule", ['ngAnimate']);
})();

app.controller("DispositionReportController", ['$scope', '$q', '$http', '$anchorScroll', '$filter', '$location', function ($scope, $q, $http, $anchorScroll, $filter, $location) {
    $scope.date = new Date();
    $scope.MyName = "ashahri";
    $scope.exportbtn = true;

    // 2) Item List Arrays.This arrays will be used to display .
    $scope.items = [];
    $scope.fldDate = [];

    // To get all details from Database
    //selectDispositionEffectiveDetails($scope.MainBatch, $scope.StartDate, $scope.EndDate, $scope.UserID);

    //alert($location.path());

    GetBatchList();

    $scope.gotoBottom = function () {
        // set the location.hash to the id of
        // the element you wish to scroll to.
        $location.hash('bottom');

        // call $anchorScroll()
        $anchorScroll();
    };

    $scope.Date = function (arg) {
        //var date = new Date(arg);
        return $filter('date')(new Date(arg), 'MM/dd/yyyy');
    };
    
    function getDispoSummary(UserID) {
        $http.get('GetDispoSummary/', { params: { UserID: UserID } })
        .success(function (data) {
            $scope.DispoSummary = data;
            if ($scope.DispoSummary.length > 0) {

            }
        })
        .error(function () {
            alert('error');
        });
    }

    function GetBatchList()
    {
        $http.get('GetBatchList/')
        .success(function(data) {
            $scope.BatchLeadsList = data;
        })
        .error(function () {
            alert('error');
        });
    }

    function selectDispositionEffectiveDetails(MainBatch, StartDate, EndDate, UserID) {
        $scope.loading = true;
        $http.get('/api/DispositionEffectiveReport/', { params: { MainBatch: MainBatch, StartDate: StartDate, EndDate: EndDate, UserID: UserID } })
        .success(function (data) {
            $scope.DispositionDetails = data;
            if ($scope.DispositionDetails.length > 0) {
            
                var uniquefldDate = {}, uniquefldSalesStatus = {}, i;

                for (i = 0; i < $scope.DispositionDetails.length; i += 1) {
                    // For Column wise fldDate add
                    uniquefldDate[$scope.DispositionDetails[i].fldDate] = $scope.DispositionDetails[i];
                }
                
                // For Column wise fldDate add
                for (i in uniquefldDate) {
                    $scope.fldDate.push(uniquefldDate[i]);
                }
                // To disply the  fldDate wise Pivot result
                $scope.getDateDetails();
                //alert('test');
                getDispoSummary($scope.UserID);
                //getTotalEffInEffCount(UserID);
                $scope.exportbtn = false;

            }

        })
        .error(function () {
            $scope.error = "An Error has occured while loading posts!";
        })

        .finally(function () {
            // Hide loading spinner whether our call succeeded or failed.
            $scope.hide = true;
            $scope.loading = false;
        });

    }

    //Search
    $scope.getReport = function () {
        // 1) Item List Arrays.This arrays will be used to display .
        $scope.hide = false;
        $scope.items = [];
        $scope.fldDate = [];
        if ($scope.startdate != null && $scope.enddate != null && $scope.MainBatch != null)
        {
            //alert($scope.UserID);
            selectDispositionEffectiveDetails($scope.MainBatch, $scope.startdate, $scope.enddate, $scope.UserID);
        }
        else
        {
            $.simplyToast('Please select all selection', 'warning');
        }
    }
    //end 1

    // To Display dispo Details as fldDate wise Pivot
    $scope.getDateDetails = function () {
        var UniqueItem = {}, i
        for (i = 0; i < $scope.DispositionDetails.length; i += 1) {
            UniqueItem[$scope.DispositionDetails[i].fldSalesStatus] = $scope.DispositionDetails[i];
        }
        for (i in UniqueItem) {

            var ItmDetails = {
                fldSalesStatus: UniqueItem[i].fldSalesStatus,
                fldCallDisposition: UniqueItem[i].fldCallDisposition,
                fldDate: UniqueItem[i].fldDate,
                fldSorting: UniqueItem[i].fldSorting
            };
            $scope.items.push(ItmDetails);
        }
    }

    // To Display dispo Details as fldDate wise Pivot fldCount Sum calculate 
    $scope.showfldDateItemDetails = function (fldSalesStatus, fldDate) {
        $scope.getItemCount = 0;
        $scope.dateCount = 0;
        for (i = 0; i < $scope.DispositionDetails.length; i++) {
            if (fldSalesStatus == $scope.DispositionDetails[i].fldSalesStatus) {
                if (fldDate == $scope.DispositionDetails[i].fldDate) {

                    if ($scope.dateCount > 0) {
                        $scope.getItemCount = parseInt($scope.getItemCount) + parseInt($scope.DispositionDetails[i].fldCount);
                    }
                    else {
                        $scope.getItemCount = parseInt($scope.DispositionDetails[i].fldCount);
                    }
                    $scope.dateCount = $scope.dateCount + 1;
                }
            }
        }
        return $scope.getItemCount;
    }

    // To Display dispo Details as fldDate wise Pivot Column wise Total
    $scope.showfldDateColumnGrandTotal = function (fldSalesStatus, fldDate) {

        $scope.getColumTot = 0;
        $scope.dateCount = 0;
        for (i = 0; i < $scope.DispositionDetails.length; i++) {
            if (fldSalesStatus == $scope.DispositionDetails[i].fldSalesStatus) {
                $scope.getColumTot = parseInt($scope.getColumTot) + parseInt($scope.DispositionDetails[i].fldCount);
            }
        }
        return $scope.getColumTot;
    }

    // To Display dispo Details as fldDate wise Pivot Row & Column Grand Total
    $scope.showfldDateGrandTotals = function (fldSalesStatus, fldDate, fldCallDisposition) {

        $scope.getGrandTotal = 0;
        if ($scope.DispositionDetails && $scope.DispositionDetails.length) {
            for (i = 0; i < $scope.DispositionDetails.length; i++) {
                if (fldCallDisposition == $scope.DispositionDetails[i].fldCallDisposition) {
                    $scope.getGrandTotal = parseInt($scope.getGrandTotal) + parseInt($scope.DispositionDetails[i].fldCount);
                }
            }
        }
        return $scope.getGrandTotal;
    }

    // To Display dispo Details as fldDate wise Pivot Row wise Total
    $scope.showfldDateRowTotal = function (fldSalesStatus, fldDate, fldCallDisposition) {

        $scope.getrowTotal = 0;

        for (i = 0; i < $scope.DispositionDetails.length; i++) {

            if (fldDate == $scope.DispositionDetails[i].fldDate) {
                if (fldCallDisposition == $scope.DispositionDetails[i].fldCallDisposition) {
                    $scope.getrowTotal = parseInt($scope.getrowTotal) + parseInt($scope.DispositionDetails[i].fldCount);
                }
            }
        }
        return $scope.getrowTotal;
    }

    $scope.showfldDateColumnPercent = function (Total1, Total2, Total3) {
        var totalcount = parseFloat(Total2) + parseFloat(Total3);
        if (Total1 != 0) {
            var totalafterall = ((parseFloat(Total1) / parseFloat(totalcount)) * 100);
        }
        else {
            totalafterall = 0;
        }
        if (totalafterall == 100 || totalafterall == 0) {
            totalafterall = totalafterall
        }
        else {
            totalafterall = totalafterall.toFixed(2)
        }

        return totalafterall;
    }

    // To Display %
    $scope.showGrandTotalPercent = function (count) {
        var totalcount = parseFloat($scope.TotalEffectiveCount) + parseFloat($scope.TotalIneffectiveCount);
        if (count != 0)
        {
            var totalafterall = ((parseFloat(count.toString().replace(",", "")) / parseFloat(totalcount)) * 100);
        }
        else
        {
            totalafterall = 0;
        }
        if (totalafterall == 100 || totalafterall == 0)
        {
            totalafterall = totalafterall
        }
        else
        {
            totalafterall = totalafterall.toFixed(2)
        }

        return totalafterall;
    }
    $scope.showContactRatePercent = function () {
        $scope.ContactRatePercent = 0;
        var totaleffective
        var totalclosed
        if ($scope.DispoSummary && $scope.DispoSummary.length) {
            for (i = 0; i < $scope.DispoSummary.length; i++) {

                if (3 == $scope.DispoSummary[i].fldSumDescriptionID) {
                    totaleffective = $scope.DispoSummary[i].fldCount;
                }

                if (5 == $scope.DispoSummary[i].fldSumDescriptionID) {
                    totalclosed = $scope.DispoSummary[i].fldCount;
                }
            }
            $scope.ContactRatePercent = (parseFloat(totaleffective) / parseFloat(totalclosed)) * 100;

            if ($scope.ContactRatePercent == 100 || $scope.ContactRatePercent == 0) {
                $scope.ContactRatePercent = $scope.ContactRatePercent
            }
            else {
                $scope.ContactRatePercent = $scope.ContactRatePercent.toFixed(2)
            }
        }

        return $scope.ContactRatePercent;
    }

    $scope.showConversionRatePercent = function () {
        $scope.ConversionRatePercent = 0;
        var totalsale
        var totalclosed
        if ($scope.DispoSummary && $scope.DispoSummary.length) {
            for (i = 0; i < $scope.DispoSummary.length; i++) {

                if (6 == $scope.DispoSummary[i].fldSumDescriptionID) {
                    totalsale = $scope.DispoSummary[i].fldCount;
                }

                if (5 == $scope.DispoSummary[i].fldSumDescriptionID) {
                    totalclosed = $scope.DispoSummary[i].fldCount;
                }
            }
            $scope.ConversionRatePercent = (parseFloat(totalsale) / parseFloat(totalclosed)) * 100;

            if ($scope.ConversionRatePercent == 100 || $scope.ConversionRatePercent == 0) {
                $scope.ConversionRatePercent = $scope.ConversionRatePercent
            }
            else {
                $scope.ConversionRatePercent = $scope.ConversionRatePercent.toFixed(2)
            }
        }
        return $scope.ConversionRatePercent;
    }

    $scope.exportData = function (filename) {
        var blob = new Blob([document.getElementById('Disposition').innerHTML], {
            type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
        })
        saveAs(blob, filename + ".xls");
    }
   
}]);