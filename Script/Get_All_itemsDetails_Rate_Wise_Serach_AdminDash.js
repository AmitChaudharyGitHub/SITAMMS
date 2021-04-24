//@* For Rate Wise Search *@

        $(document).ready(function () {
            $("#RateCheck1").change(function (event) {
                if ($(this).is(':checked')) {
                    $("#imgLoading").show();
                    var theId = $(this).attr('id'); // The id of the checkbox
                    var theValue = $(this).val(); // The value field of the checkbox
                }

                else {
                    $("#imgLoading").show();
                    $("#tblEmpoyees tbody tr").remove();
                    $.ajax({
                        type: 'POST',
                        url: GetAll_Items_Details,
                        dataType: 'json',
                        data: {},
                        success: function (data) {
                            $("#imgLoading").hide();

                            var items = '';
                            var rows = "<tr>"
                                + "<th align='left' class='empTableTH'>Project Name</th> <th align='left' class='empTableTH'>Date</th> <th align='left' class='empTableTH'>Item Name</th> <th align='left' class='empTableTH'>PO Quantity</th> <th align='left' class='empTableTH'>Rate</th>"
                                + "</tr>";
                            $('#tblEmpoyees tbody').append(rows);

                            $.each(data, function (i, item) {
                                var rows = "<tr class='success'>"
                                        + "<td class='empTableTD'>" + item.ProjectName + "</td>"
                                        + "<td class='empTableTD'>" + item.PurchaseOrderDate + "</td>"
                                        //+ "<td class='empTableTD'>" + item.ItemNo + "</td>"
                                        + "<td class='empTableTD'>" + item.ItemName + "</td>"
                                        + "<td class='empTableTD'>" + item.Qty + "</td>"
                                        + "<td class='empTableTD'>" + item.Rate + "</td>"
                                        + "</tr>";
                                $('#tblEmpoyees tbody').append(rows);
                            });
                        },
                        error: function (ex) {
                            var r = jQuery.parseJSON(response.responseText);
                            alert("Message: " + r.Message);
                        }
                    });
                    return false;

                }
           
                $("#tblEmpoyees tbody tr").remove();
                $.ajax({
                    type: 'POST',
                    url: GetRateWise_Items_Details,
                    dataType: 'json',
                    data: { rates: theValue },

                    success: function (data) {
                        $("#imgLoading").hide();
                        var items = '';
                        var rows = "<tr>"
                    + "<th align='left' class='empTableTH'>Project Name</th> <th align='left' class='empTableTH'>Date</th> <th align='left' class='empTableTH'>Item Name</th> <th align='left' class='empTableTH'>PO Quantity</th> <th align='left' class='empTableTH'>Rate</th>"
                    + "</tr>";
                        $('#tblEmpoyees tbody').append(rows);


                        $.each(data, function (i, item) {
                            var rows = "<tr>"
                            //+ "<td class='empTableTD'>" + item.ProjectNo + "</td>"
                            + "<td class='empTableTD'>" + item.ProjectName + "</td>"
                            + "<td class='empTableTD'>" + item.PurchaseOrderDate + "</td>"
                            //+ "<td class='empTableTD'>" + item.ItemNo + "</td>"
                            + "<td class='empTableTD'>" + item.ItemName + "</td>"
                            + "<td class='empTableTD'>" + item.Qty + "</td>"
                            + "<td class='empTableTD'>" + item.Rate + "</td>"
                            + "</tr>";
                            $('#tblEmpoyees tbody').append(rows);
                        });

                    },
                    error: function (ex) {
                        var r = jQuery.parseJSON(response.responseText);
                        alert("Message: " + r.Message);
                    }
                });
                return false;
            })
        });


