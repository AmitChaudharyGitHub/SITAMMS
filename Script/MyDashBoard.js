$(document).ready(function () {



    window['GetPrj'] = function (dateString) {

        $('#dvLoading').show();
        $.ajax({
            type: 'POST',
            url: GetProjectByEmpId, 
            dataType: 'json',
            data: { E: ss },
            complete: function () {
                $('#dvLoading').hide();
            },
            success: function (result) {

                var procemessage = "<option value='0'> Please wait...</option>";
                
                $("#ProjectHomeId").html(procemessage).show();

               

                var markup1 = "<option value='0'>Select Project</option>";
                $("#ProjectHomeId").html(markup1).show();

              

                result = $.parseJSON(result);
              
               
                var List0 = $.parseJSON(result.List1);
               

              

                var selectedDeviceModel0 = $('#ProjectHomeId');

                $.each(List0, function(index, item) {

                    selectedDeviceModel0.append(
                        $('<option/>', {
                            value: item.Value,
                            text: item.Text
                        })
                    );
                });

                $("#ProjectHomeId").prop('selectedIndex', 1).trigger('change');

              

                $("#ItemGroup2").prop('selectedIndex', 1).trigger('change');
                $("#ItemGroup3").prop('selectedIndex', 1).trigger('change');
                $("#ItemGroup").prop('selectedIndex', 4).trigger('change');
                $("#ItemGroup4").prop('selectedIndex', 4).trigger('change');
                $("#Status").prop('selectedIndex', 1).trigger('change');
       
            },
            error: function (ex) {
                alert('Failed to retrieve Projects.' + ex);
            }
        });
        return false;
    };
    GetPrj();
    $('#pipendingform').hide();


    $("#ProjectHomeId").change(function () {

        $("#dvLoading").show();
        $('#pipendingform').hide(); $('#tblformPoTable').hide();


        if ($('#ProjectHomeId option:selected').val() != 0) {

            // PI section 

            $.get(GetPIPendingdetail,
            { ProjectId: $('#ProjectHomeId option:selected').val() },
           function (result) {
               $('#pipendingform').show();
               $('#pipendingform').html(result);
               $("#dvLoading").hide();
           });

            // end PI section

            // PO section 

            $.get(GEtPOPendingDetail,
             { ProjectId: $('#ProjectHomeId option:selected').val() },
             function (result) {
              $('#tblformPoTable').show();
              $('#tblformPoTable').html(result);
              $("#dvLoading1").hide();
          });


            // End PO section



            $.getJSON(GetSumOFallRecvAndIssueofAllAlocatedProject, { ProjectId: $('#ProjectHomeId option:selected').val() }, function (data) {

                data = $.parseJSON(data);


                $.each(data, function (i, item) {
                    $('#lbltxttotalRecvtilldate').text(item.TotalRecvofAllProject);

                    $('#lbltxttotalIssueTillDate').text(item.TotalIssueAmtOfAllProject);


                });



            });





        }


    });










    $("#ItemGroup2").change(function () {
        $("#ItemMaster").empty();
        $("#ItemMaster").append($("<option></option").val("").html("Select Item Name"));

        if ($("#ItemGroup2 option:selected").val() !== 0) {

            $('#dvLoading').show();
            $.ajax({
                type: 'POST',
                url: GetItemByGroup,

                dataType: 'json',


                data: { id: $("#ItemGroup2").val() },

                complete: function () {
                    $('#dvLoading').hide();
                },
                success: function (names) {
                    names = $.parseJSON(names)


                    $.each(names, function (i, itname) {
                        $("#ItemMaster").append('<option value="' + itname.Value + '">' +
                             itname.Text + '</option>');

                    });


                    $("#ItemMaster").prop('selectedIndex', 1).trigger('change');

                },
                error: function (ex) {
                    alert('Failed to retrieve Item Name.' + ex);
                }
            });

           

            return false;

        }

       
    })

    $("#ItemMaster").change(function () {

        if ($("#ItemMaster option:selected").val() !== 0) {
            $("#dvLoading").show();
            $("#columnchart").hide();
            var param1 = $("#ProjectHomeId option:selected").val();
            var param2 = $("#ItemGroup2 option:selected").val();
            var param3 = $("#ItemMaster option:selected").val();
            // alert(param1)
            var List_1 = [];
            var List_Recv = [];
            var List_Issu = [];

            function empty() {
                //empty your array
                List_1.length = 0;
                List_Recv.length = 0;
                List_Issu.length = 0;
            }
            empty();

            $.getJSON(GetPurchaseAndRecevgraphdetail, { ProjectId: param1, ItemGrpId: param2, ItemId: param3 }, function (data) {
                data = $.parseJSON(data);
                console.log(data);
                
                $.each(data, function (i, item) {
                    
                    List_1.push(item.Month);
                    List_Recv.push(item.ReceiveQty);
                    List_Issu.push(item.IssueQty);
                    

                });
               // alert(List_1);
                console.log(List_1);
                DrawColumnChartGraph(List_1, List_Recv, List_Issu);
                $("#dvLoading").hide();
                $("#columnchart").show();
                
            });
            

        }




    });


    $("#Status").change(function () {
        if ($("#Status option:selected").val() !== 0)
        {
            $('#dvLoadingAging').show();
           
            var url = GetAgingOnHome;
            $.get(url,
                { PID: $('#ProjectHomeId option:selected').val(), GID: $('#ItemGroup3 option:selected').val(), STA: $('#Status option:selected').text() },
                function (result) {
                    $('#agingdivId').show();
                    $('#agingdivId').html(result);
                    $("#dvLoadingAging").hide();
                });

            $.getJSON(GetageingHomeGraph, { ProjectId: $('#ProjectHomeId option:selected').val(), ItemgrpId: $('#ItemGroup3 option:selected').val(), Status: $('#Status option:selected').text() }, function (data) {
                data = $.parseJSON(data);
                 
                var DLst = [];


                $.each(data, function (i, item) {

                    DLst.push(item.MovingStatus, item.StockPercentage);


                });


                DrawPieHighChartGraph(DLst);
                console.log(DLst);


            });




            // draw pie highchart here.
        

        }

    });



  
   

    // setting default selected value for Purchase Vs Issue column stacked chart . Start
  

    // End

});

function DrawColumnChartGraph(CatMonth ,recvData , IssueData)
{
    debugger;
    
    Highcharts.chart('columnchart', {
        chart: {
            type: 'column',
            options3d: {
                enabled: true,
                alpha: 0,
                beta: 0,
                viewDistance:85,
                depth: 50
            }
        },

 
        xAxis: {
            categories: CatMonth,
      
        
        },

        yAxis: {
            allowDecimals: false,
            min: 1,
            title: {
                text: 'Quantity'
            }
        },


        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y}'
        },

        plotOptions: {
            column: {
                stacking: 'normal',
                depth: 10
            }
        },

        series: [{
            name: 'Purchase',
            data: recvData,
            stack: 'male'
        }, {
            name: 'Issue',
            data: IssueData,
            stack: 'female'
        }]
    });
    debugger;
}

function DrawPieHighChartGraph(ListDTa)
{
    Highcharts.chart('con', {
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            }
        },

        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    enabled: true,
                    format: '{point.name}'
                }
            }
        },
        series: [{
            type: 'pie',
            name: 'Ageing Status (%) ',
            data: [ListDTa]
        }]
    });
}


