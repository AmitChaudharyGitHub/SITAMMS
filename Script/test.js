﻿function GetEmp1(t) {
    var e = "<option value='0'> Please wait...</option>";
    $("#Employee").html(e).show();
    for (var a = "<option value='0'>Select Employee</option>", o = 0; o < t.length; o++) a += "<option value=" + t[o].Value + ">" + t[o].Text + "</option>";
    $("#Employee").html(a).show()
}

function GetStatusList(t) {
    var e = "<option value='0'> Please wait...</option>";
    $("#StatusTypeNo").html(e).show();
    for (var a = "<option value='0'>Select No</option>", o = 0; o < t.length; o++) a += "<option value=" + t[o].Value + ">" + t[o].Text + "</option>";
    $("#StatusTypeNo").html(a).show()
}
$(document).ready(function() {
    window.GetPrj = function(t) {
        return $("#dvLoading").show(), $.ajax({
            type: "POST",
            url: GetProjectByEmpId,
            dataType: "json",
            data: {
                E: ss
            },
            success: function(t) {
                var e = "<option value='0'> Please wait...</option>";
                $("#Projects").html(e).show();
                var a = "<option value='0'>Select Project</option>";
                $("#Projects").html(a).show(), t = $.parseJSON(t);
                var o = $("#Projects");
                $.each(t, function(t, e) {
                    o.append($("<option/>", {
                        value: e.Value,
                        text: e.Text
                    }))
                }), $("#Projects").prop("selectedIndex", 1)
            },
            complete: function() {
                $("#dvLoading").hide()
            },
            error: function(t) {
                alert("Failed to retrieve Project." + t)
            }
        }), !1
    }, GetPrj()
}), $(document).ready(function() {
    $("#Projects").change(function() {
        return $("#GateEntryNo").empty(), $("#Status").prop("selectedIndex", 0), $("#StatusTypeNo").empty(), $("#formbody").html(""), 0 != $("#Projects option:selected").val() ? ($("#dvLoading").show(), $.ajax({
            type: "POST",
            url: GetProjectGateEntryNo,
            dataType: "json",
            data: {
                id: $("#Projects").val()
            },
            success: function(t) {
                t = $.parseJSON(t), $("#GateEntryNo").val(t.Message), GetEmp1(t.List)
            },
            complete: function() {
                $("#dvLoading").hide()
            },
            error: function(t) {}
        }), !1) : void 0
    })
}), $(document).ready(function() {
    $(document).on("keyup", "td[class^='CDispatch_']", function() {
        var t = $(this).find("input").val(),
            e = $(this).attr("class"),
            a = e.substring(10),
            o = $("#Dispatch_" + a).find("input").val();
        t = +t + +o;
        var i = $("#Qty_" + a).find("input").val(); + t > +i && (alert("Total Dispatch Quantity Can,t Greater than Qty"), $(this).find("input").val(""))
    }), $(document).on("keyup", "td[class^='CReceive_']", function() {
        var t = $(this).find("input").val(),
            e = $(this).attr("class"),
            a = e.substring(9),
            o = $("#Receive_" + a).find("input").val();
        t = +t + +o;
        var i = $("#Dispatch_" + a).find("input").val(); + t > +i && (alert("Total Receive Quantity Can,t Greater than Dispatch Qty"), $(this).find("input").val(""))
    })
}), $(document).ready(function() {
    $("#Status").change(function() {
        return $("#StatusTypeNo").empty(), $("#formbody").html(""), 0 != $("#Status option:selected").val() ? ($("#dvLoading").show(), $.ajax({
            type: "POST",
            url: GetStatusTypeNo,
            dataType: "json",
            data: {
                Type: $("#Status").val(),
                ProjectNo: $("#Projects").val()
            },
            success: function(t) {
                t = $.parseJSON(t), GetStatusList(t)
            },
            complete: function() {
                $("#dvLoading").hide()
            },
            error: function(t) {}
        }), !1) : void 0
    })
}), $(document).ready(function() {
    function t() {
        ("Dispatch" == $("#Status").val() || "Receive" == $("#Status").val()) && ($("#ChallanNo").val($("div[class='Challan']").attr("challanno")), $("#VehicleNo").val($("div[class='Challan']").attr("vehicleno")), $("#ChallanDate").val($("div[class='Challan']").attr("challandate")))
    }
    $("#StatusTypeNo").change(function() {
        return 0 != $("#StatusTypeNo option:selected").val() ? ($("#dvLoading").show(), $.ajax({
            type: "GET",
            url: Grid,
            data: {
                Type: $("#Status").val(),
                PurchaseNo: $("#StatusTypeNo").val()
            },
            complete: function() {
                $("#dvLoading").hide()
            },
            success: function(e) {
                $("#formbody").html(e), t()
            },
            error: function(t) {
                alert("Failed to retrieve Project Code." + t)
            }
        }), !1) : void 0
    })
}), $(document).ready(function() {
    $(".DatePicker").datepicker({
        dateFormat: "dd M yy",
        changeMonth: !0,
        changeYear: !0,
        value: ""
    })
}), $(document).ready(function() {
    function t() {
        var t = [],
            e = !1;
        $("#grid tbody tr").each(function(a) {
            var o = new Object;
            0 != $(this)[0].rowIndex && 1 != $(this)[0].rowIndex && (x = $(this).children(), o.ReceiveQty = $(this).children().eq(8).find("input").val(), null != o.ReceiveQty && 0 != o.ReceiveQty && (e = !0, o.GateEntryNo = $("#GateEntryNo").val(), o.Status = $("#Status option:selected").val(), o.StatusTypeId = $("#StatusTypeNo option:selected").val(), o.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim(), o.ItemNo = $(this).children().eq(0).attr("it"), o.Item = $(this).children().eq(2).text().trim(), o.ItemGroup = $(this).children().eq(1).text().trim(), o.Unit = $(this).children().eq(3).text().trim(), o.StatusChildId = $(this).children().eq(0).attr("lpuid"), o.Vendor = $("td[class='Ven']").text().trim(), o.VendorNo = $("td[class='Ven']").attr("vid"), o.Rate = $(this).children().eq(0).attr("rate"), t.push(o)))
        });
        return [e, t]
    }

    function e() {
        var t = [],
            e = !1;
        $("#grid tbody tr").each(function(a) {
            var o = new Object;
            0 != $(this)[0].rowIndex && 1 != $(this)[0].rowIndex && (x = $(this).children(), o.ReceiveQty = $(this).children().eq(9).find("input").val(), null != o.ReceiveQty && 0 != o.ReceiveQty && (e = !0, o.GateEntryNo = $("#GateEntryNo").val(), o.Status = $("#Status option:selected").val(), o.StatusTypeId = $("#StatusTypeNo option:selected").val(), o.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim(), o.ItemNo = $(this).children().eq(0).attr("it"), o.Item = $(this).children().eq(2).text().trim(), o.ItemGroup = $(this).children().eq(1).text().trim(), o.Unit = $(this).children().eq(3).text().trim(), o.StatusChildId = $(this).children().eq(0).attr("ipouid"), o.Vendor = $("td[class='Ven']").text().trim(), o.VendorNo = $("td[class='Ven']").attr("vid"), o.Rate = $(this).children().eq(0).attr("rate"), t.push(o)))
        });
        return [e, t]
    }

    function a() {
        var t = [],
            e = !1;
        $("#grid tbody tr").each(function(a) {
            var o = new Object;
            0 != $(this)[0].rowIndex && 1 != $(this)[0].rowIndex && 2 != $(this)[0].rowIndex && (x = $(this).children(), o.ReceiveQty = $(this).children().eq(7).find("input").val(), null != o.ReceiveQty && 0 != o.ReceiveQty && (e = !0, o.GateEntryNo = $("#GateEntryNo").val(), o.Status = $("#Status option:selected").val(), o.StatusTypeId = $("#StatusTypeNo option:selected").val(), o.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim(), o.ItemNo = $(this).children().eq(0).attr("it"), o.ItemGroupNo = $(this).children().eq(0).attr("ig"), o.Item = $(this).children().eq(2).text().trim(), o.ItemGroup = $(this).children().eq(1).text().trim(), o.Unit = $(this).children().eq(3).text().trim(), o.UnitNo = $(this).children().eq(0).attr("unitno"), o.StatusChildId = $(this).children().eq(0).attr("ostuid"), o.Vendor = $("td[class='RVen']").attr("receivename"), o.VendorNo = $("td[class='RVen']").attr("receiveno"), o.Rate = $(this).children().eq(0).attr("rate"), o.TaxPer = $(this).children().eq(0).attr("taxper"), o.DeliveryUnitCharge = $(this).children().eq(0).attr("dperunit"), t.push(o)))
        });
        return [e, t]
    }

    function o() {
        var t = [],
            e = !1;
        $("#grid tbody tr").each(function(a) {
            var o = new Object;
            0 != $(this)[0].rowIndex && 1 != $(this)[0].rowIndex && 2 != $(this)[0].rowIndex && (x = $(this).children(), o.ReceiveQty = $(this).children().eq(7).find("input").val(), null != o.ReceiveQty && 0 != o.ReceiveQty && (e = !0, o.GateEntryNo = $("#GateEntryNo").val(), o.Status = $("#Status option:selected").val(), o.StatusTypeId = $("#StatusTypeNo option:selected").val(), o.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim(), o.ItemNo = $(this).children().eq(0).attr("it"), o.ItemGroupNo = $(this).children().eq(0).attr("ig"), o.Item = $(this).children().eq(2).text().trim(), o.ItemGroup = $(this).children().eq(1).text().trim(), o.Unit = $(this).children().eq(3).text().trim(), o.UnitNo = $(this).children().eq(0).attr("unitno"), o.StatusChildId = $(this).children().eq(0).attr("ostuid"), o.Vendor = $("td[class='DVen']").attr("dispatchname"), o.VendorNo = $("td[class='DVen']").attr("dispatchno"), o.Rate = $(this).children().eq(0).attr("rate"), o.TaxPer = $(this).children().eq(0).attr("taxper"), o.DeliveryUnitCharge = $(this).children().eq(0).attr("dperunit"), t.push(o)))
        });
        return [e, t]
    }

    function i() {
        var t = new Object;
        return t.Date = $("#Date").val(), t.Time = $("#Time").val(), t.ProjectNo = $("#Projects option:selected").val(), t.ProjectName = $("#Projects option:selected").text(), t.GateEntryNo = $("#GateEntryNo").val(), t.EmpId = $("#Employee option:selected").val(), 0 != t.EmpId && (t.EmpName = $("#Employee option:selected").text()), t.ChallanNo = $("#ChallanNo").val(), t.ChallanDate = $("#ChallanDate").val(), t.VehicleNo = $("#VehicleNo").val(), t.Status = $("#Status option:selected").val(), t.StatusTypeId = $("#StatusTypeNo option:selected").val(), t.StatusTypeNo = $("#StatusTypeNo option:selected").text().trim(), t.EmpId = $("#Employee option:selected").val(), t.EmpName = $("#Employee option:selected").text(), t
    }

    function n() {
        var t = $("#Projects option:selected").val(),
            e = $("#GateEntryNo").val(),
            a = $("#Status option:selected").val(),
            o = $("#StatusTypeNo option:selected").val(),
            i = $("#Date").val(),
            n = !0;
        return "0" == t ? ($("#Projects").css("border-color", "#f0551b"), n = !1) : $("#Projects").css("border-color", ""), "" == e ? ($("#GateEntryNo").css("border-color", "#f0551b"), n = !1) : $("#GateEntryNo").css("border-color", ""), "" == a ? ($("#Status").css("border-color", "#f0551b"), n = !1) : $("#Status").css("border-color", ""), "" == o ? ($("#StatusTypeNo").css("border-color", "#f0551b"), n = !1) : $("#StatusTypeNo").css("border-color", ""), "" == i ? ($("#Date").css("border-color", "#f0551b"), n = !1) : $("#Date").css("border-color", ""), n
    }
    $("#btnsave").click(function(r) {
        var s = n();
        if (0 != s) {
            var l = [],
                c = AddM_UpC;
            if ("LP" == $("#Status").val()) {
                var d = t();
                if (s = d[0], 0 == s) return;
                l = d[1]
            } else if ("IPO" == $("#Status").val()) {
                var d = e();
                if (s = d[0], 0 == s) return;
                l = d[1]
            } else if ("Dispatch" == $("#Status").val()) {
                var d = a();
                if (s = d[0], 0 == s) return;
                l = d[1]
            } else if ("Receive" == $("#Status").val()) {
                var d = o();
                if (s = d[0], 0 == s) return;
                l = d[1]
            }
            var u = i(),
                h = {
                    Child: l,
                    Master: u
                };
            $("#dvLoading").show(), $.ajax({
                type: "POST",
                url: c,
                data: JSON.stringify(h),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processdata: !0,
                success: function(t) {
                    "1" == t && ($("#myModal").modal("show"), $("#Time").prop("selectedIndex", 0), $("#Projects").prop("selectedIndex", 0), $("#Employee").prop("selectedIndex", 0), $("#Status").prop("selectedIndex", 0), $("#StatusTypeNo").prop("selectedIndex", 0), $("#formbody").html(""), $("#GateEntryNo").val(""), $("#ChallanNo").val(""), $("#ChallanDate").datepicker("setDate", ""), $("#VehicleNo").val(""))
                },
                complete: function() {
                    $("#dvLoading").hide()
                },
                error: function() {
                    alert("Error in Submit Data")
                }
            })
        }
    })
}), $(document).ready(function() {
    $(document).on("keyup", "td[class^='Receive_']", function() {
        var t = $(this).find("input").val(),
            e = $(this).attr("class"),
            a = e.substring(8),
            o = $("#Qty_" + a).find("input").val(),
            i = $("#PReceive_" + a).find("input").val(),
            n = +t + +i; + n > +o && (alert("Total Greater than  Purchase Qty"), $(this).find("input").val(""))
    })
}), $(document).ready(function() {
    function t(t, e) {
        var a = t.which ? t.which : event.keyCode;
        return 45 == a && -1 == $(e).val().indexOf("-") || 46 == a && -1 == $(e).val().indexOf(".") || !(48 > a || a > 57) ? !0 : !1
    }
    $(document).on("keypress", "input[id='Receive']", function(e) {
        return t(e, this)
    })
});