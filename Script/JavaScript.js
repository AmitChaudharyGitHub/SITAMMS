function GetEmp1(t) {
    var e = "<option value='0'> Please wait...</option>";
    $("#Employee").html(e).show();
    for (var a = "<option value='0'>Select Employee</option>", n = 0; n < t.length; n++) a += "<option value=" + t[n].Value + ">" + t[n].Text + "</option>";
    $("#Employee").html(a).show()
}

function GetInd1(t) {
    var e = "<option value='0'> Please wait...</option>";
    $("#Indents").html(e).show();
    for (var a = "<option value='0'>Select Indent</option>", n = 0; n < t.length; n++) a += "<option value=" + t[n].Value + ">" + t[n].Text + "</option>";
    $("#Indents").html(a).show()
}

function GetVendor(t) {
    var e = "<option value='0'> Please wait...</option>";
    $("#VNAME").html(e).show();
    for (var a = "<option value='0'>Select Vendor</option>", n = 0; n < t.length; n++) null != t[n].Value && (a += "<option value=" + t[n].Value + ">" + t[n].Text + "</option>");
    $("#VNAME").html(a).show()
}
$(document).ready(function () {
    window.lol = function () {
        alert("lol")
    }, window.GetPrj = function (t) {
        return $("#dvLoading").show(), $.ajax({
            type: "POST",
            url: GetProjectByEmpId,
            dataType: "json",
            data: {
                E: ss
            },
            complete: function () {
                $("#dvLoading").hide()
            },
            success: function (t) {
                var e = "<option value='0'> Please wait...</option>";
                $("#Projects").html(e).show();
                var a = "<option value='0'>Select Project</option>";
                $("#Projects").html(a).show(), t = $.parseJSON(t);
                var n = $("#Projects");
                $.each(t, function (t, e) {
                    n.append($("<option/>", {
                        value: e.Value,
                        text: e.Text
                    }))
                }), 1 == Trig1 && ($("#Projects").val(Master.ProjectNo), $("#Projects").trigger("change"), $("#VGID").val(VG), $("#VGID").trigger("change"), Trig1 = !1)
            },
            error: function (t) {
                alert("Failed to retrieve Project." + t)
            }
        }), !1
    }, GetPrj()
}), $(document).ready(function () {
    $("#Projects").change(function () {
        if (0 != $("#Projects option:selected").val()) return $("#PurchaseOrderNo").empty(), $("#dvLoading").show(), $.ajax({
            type: "POST",
            url: GetProjectPurchaseOrderNo,
            dataType: "json",
            data: {
                PRJID: $("#Projects").val()
            },
            complete: function () {
                $("#dvLoading").hide()
            },
            success: function (t) {
                t = $.parseJSON(t), $("#PurchaseOrderNo").val(t.No), $("#ProjectAddress").val(t.Address), GetEmp1(t.List), GetInd1(t.List1), 1 == Trig2 && ($("#Employee").val(Master.SendTo), $("#Indents").val(Master.IndentRefNo), $("#Indents").trigger("change"), Trig2 = !1)
            },
            error: function (t) { }
        }), !1;
        var t = "<option value='0'>Select Employee</option>";
        $("#Employee").html(t).show()
    })
}), $(document).ready(function () {
    $("#VGID").change(function () {
        if (0 != $("#VGID option:selected").val()) return $("#dvLoading").show(), $("#VNAME").empty(), $.ajax({
            type: "POST",
            url: GetVendorByGroup,
            dataType: "json",
            data: {
                Vid: $("#VGID").val(),
                Pid: $("#Projects").val()
            },
            complete: function () {
                $("#dvLoading").hide()
            },
            success: function (t) {
                t = $.parseJSON(t), GetVendor(t), 1 == Trig3 && ($("#VNAME").val(Master.SupplierNo), $("#VNAME").trigger("change"), Trig3 = !1)
            },
            error: function (t) { }
        }), !1;
        var t = "<option value='0'>Select Vendor</option>";
        $("#VNAME").html(t).show()
    })
}), $(document).ready(function () {
    $("#VNAME").change(function () {
        return 0 != $("#VNAME option:selected").val() ? ($("#SupplierAddress").val(""), $("#SupplierTinNo").val(""), $("#dvLoading").show(), $.ajax({
            type: "POST",
            url: GetVendorDetail,
            dataType: "json",
            data: {
                ID: $("#VNAME").val()
            },
            complete: function () {
                $("#dvLoading").hide()
            },
            success: function (t) {
                t = $.parseJSON(t), $("#SupplierAddress").val(t.Address), $("#SupplierTinNo").val(t.TinNo)
            },
            error: function (t) { }
        }), !1) : ($("#SupplierAddress").val(""), void $("#SupplierTinNo").val(""))
    })
}), $(document).ready(function () {
    function t() {
        $("#Total").val(""), $("#Vat").val(""), $("#VatAmount").val(""), $("#SubTotal").val(""), $("#SurCharge").val(""), $("#Cartage").val(""), $("#GrandTotal").val(""), $("#Rupees").val("")
    }
    $("#Indents").change(function () {
        return 0 != $("#Indents option:selected").val() ? ($("#dvLoading").show(), $.ajax({
            type: "GET",
            url: Grid,
            data: {
                IndentNo: $("#Indents").val()
            },
            complete: function () {
                $("#dvLoading").hide()
            },
            success: function (e) {
                $("#formbody").html(e), 1 == Trig4 ? (GetChild(), GetMaster(), Trig4 = !1) : t()
            },
            error: function (t) {
                alert("Failed to retrieve Project Code." + t)
            }
        }), !1) : void $("#formbody").html("")
    })
}), $(document).ready(function () {
    $(document).on("click", "span.checkVal", function () {
        $(this).find("input").val();
        var t = $(this).attr("id");
        t.substring(8)
    }), $(document).on("click", "td[class^='Item_']", function () {
        $(this).find("input").val();
        var t = $(this).attr("id");
        t.substring(8)
    })
}), $(document).ready(function () {
    function t() {
        var t = 0;
        $("td[class^='Amount_").each(function () {
            if (null != $(this).find("input").val()) {
                var e = $(this).find("input").val();
                t = +t + +e
            }
        }), $("#Total").val(t.toFixed(2))
    }

    function e() {
        var t = 0,
            e = 0,
            a = $("#Total").val(),
            n = $("#Vat").val();
        t = +(a * n / 100), e = +a + +t, $("#VatAmount").val(t.toFixed(2)), $("#SubTotal").val(e.toFixed(2))
    }

    function a() {
        var t = 0,
            e = 0,
            a = 0,
            n = 0;
        null != $("#SubTotal").val() && (e = $("#SubTotal").val()), null != $("#SurCharge").val() && (a = $("#SurCharge").val()), null != $("#Cartage").val() && (n = $("#Cartage").val()), t = +e + +a + +n, $("#GrandTotal").val(t.toFixed(2))
    }
    $(document).on("keyup", "td[class^='Qty_']", function () {
        var n = $(this).find("input").val(),
            o = $(this).attr("class"),
            r = o.substring(4),
            l = $("#Balance_" + r).find("input").val(),
            i = $(".Rate_" + r).find("input").val();
        if (+n > +l && (alert("Quantity Can,t Greater than Remaining"), $(this).find("input").val("")), null != i && null != n) {
            var d = n * i;
            d.toFixed(2);
            $(".Amount_" + r).find("input").val(d.toFixed(2))
        }
        t(), e(), a()
    }), $(document).on("keyup", "td[class^='Rate_']", function () {
        var n = $(this).find("input").val(),
            o = $(this).attr("class"),
            r = o.substring(5),
            l = $(".Qty_" + r).find("input").val();
        if (null != n && null != l) {
            var i = l * n;
            i.toFixed(2);
            $(".Amount_" + r).find("input").val(i.toFixed(2))
        }
        t(), e(), a()
    }), $(document).on("keyup", "td[class^='Discount_']", function () {
        var n = $(this).find("input").val(),
            o = $(this).attr("class"),
            r = o.substring(9),
            l = $(".Qty_" + r).find("input").val(),
            i = $(".Rate_" + r).find("input").val();
        if (null != i && null != l && null != n) {
            var d = l * i,
                c = d.toFixed(2);
            c -= c * n / 100, $(".Amount_" + r).find("input").val(c.toFixed(2))
        }
        t(), e(), a()
    }), $(document).on("change", "td[class^='Amount_']", function () {
        t()
    }), $(document).on("click", "#Total", function () {
        t()
    }), $(document).on("keyup", "#SurCharge", function () {
        a()
    }), $(document).on("keyup", "#Cartage", function () {
        a()
    }), $("#btnCalc").click(function (n) {
        t(), e(), a()
    }), $(document).on("keyup", "#Vat", function () {
        e()
    })
}), $(document).ready(function () {
    $(".DatePicker").datepicker({
        dateFormat: "dd M yy",
        changeMonth: !0,
        changeYear: !0,
        value: ""
    })
}), $(document).ready(function () {
    function t() {
        var t = [],
            e = ($("#webgrid tbody tr").each(function (e) {
                var a = new Object;
                if (0 != $(this)[0].rowIndex) {
                    var n = 0;
                    x = $(this).children();
                    var n = $(this).children().eq(9).find("input").val();
                    0 != n && null != n && (a.IndentId = $(this).children().eq(0).text().trim(), a.ItemNo = $(this).children().eq(1).text().trim(), a.Qty = $(this).children().eq(6).find("input").val(), a.Rate = $(this).children().eq(7).find("input").val(), a.Discount = $(this).children().eq(8).find("input").val(), a.Amount = n, a.UId = $(this).children().eq(0).attr("UId"), a.MUId = $(this).children().eq(0).attr("MUId"), a.SrNo = $(this).children().eq(0).attr("SrNo"), t.push(a))
                }
            }), new Object);
        e.PurchaseOrderDate = $("#Date").val(), e.PurchaseOrderNo = $("#PurchaseOrderNo").val(), e.ProjectNo = $("#Projects option:selected").val(), e.SupplierNo = $("#VNAME option:selected").val(), e.IndentRefNo = $("#Indents option:selected").text(), e.AcilTinNo = $("#AcilTinNo").val(), e.Total = $("#Total").val(), e.Vat = $("#Vat").val(), e.VatAmount = $("#VatAmount").val(), e.SubTotal = $("#SubTotal").val(), e.SurCharge = $("#SurCharge").val(), e.Cartage = $("#Cartage").val(), e.GrandTotal = $("#GrandTotal").val(), e.Rupees = $("#Rupees").val(), e.SendTo = $("#Employee option:selected").val(), e.UId = Master.UId, e.SessionId = Master.SessionId, e.CreatedBy = Master.CreatedBy, e.PurchaseOrderId = Master.PurchaseOrderId;
        var a = {
            Child: t,
            Master: e
        };
        return a
    }

    function e() {
        var t = $("#Date").val(),
            e = $("#PurchaseOrderNo").val(),
            a = $("#Projects option:selected").val(),
            n = $("#VNAME option:selected").val(),
            o = $("#Indents option:selected").val(),
            r = ($("#AcilTinNo").val(), $("#Total").val()),
            l = ($("#Vat").val(), $("#VatAmount").val(), $("#SubTotal").val()),
            i = ($("#SurCharge").val(), $("#Cartage").val(), $("#GrandTotal").val()),
            d = ($("#Rupees").val(), $("#Employee option:selected").val(), !0);
        if ("" == t ? ($("#Date").css("border-color", "#f0551b"), d = !1) : $("#Date").css("border-color", ""), "" == e ? ($("#PurchaseOrderNo").css("border-color", "#f0551b"), d = !1) : $("#PurchaseOrderNo").css("border-color", ""), "" == a ? ($("#Projects").css("border-color", "#f0551b"), d = !1) : $("#Projects").css("border-color", ""), "" == n ? ($("#VNAME").css("border-color", "#f0551b"), d = !1) : $("#VNAME").css("border-color", ""), "" == o ? ($("#Indents").css("border-color", "#f0551b"), d = !1) : $("#Indents").css("border-color", ""), "" == r ? ($("#Total").css("border-color", "#f0551b"), d = !1) : $("#Total").css("border-color", ""), "" == l ? ($("#SubTotal").css("border-color", "#f0551b"), d = !1) : $("#SubTotal").css("border-color", ""), "" == i ? ($("#GrandTotal").css("border-color", "#f0551b"), d = !1) : $("#GrandTotal").css("border-color", ""), 0 == d) return !1;
        var c = 0;
        return $("td[class^='Amount_").each(function () {
            if (null != $(this).find("input").val()) {
                var t = $(this).find("input").val();
                c = +c + +t
            }
        }), 0 == c ? (alert("Items Grid Not Fill Correctly "), !1) : !0
    }
    $("#btnsave").click(function (a) {
        var n = e();
        if (0 != n) {
            var o = t(),
                r = UpM_UpC;
            $.ajax({
                type: "POST",
                url: r,
                data: JSON.stringify(o),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processdata: !0,
                success: function (t) {
                    "1" == t && ($("#myModal").modal("show"), $("#Date").datepicker("setDate", ""), $("#PurchaseOrderNo").val(""), $("#SupplierAddress").val(""), $("#SupplierTinNo").val(""), $("#Projects").prop("selectedIndex", 0), $("#VGID").prop("selectedIndex", 0), $("#VNAME").prop("selectedIndex", 0), $("#Indents").prop("selectedIndex", 0), $("#formbody").html(""), $("#Total").val(""), $("#Vat").val(""), $("#VatAmount").val(""), $("#SubTotal").val(""), $("#SurCharge").val(""), $("#Cartage").val(""), $("#GrandTotal").val(""), $("#Rupees").val(""), $("#Employee").prop("selectedIndex", 0))
                },
                error: function () {
                    alert("Error in Submit Data")
                }
            })
        }
    })
}), $(document).ready(function () {
    window.Master = Master, window.Trig1 = !0, window.Trig2 = !0, window.Trig3 = !0, window.Trig4 = !0, window.GetMaster = function () {
        $("#Total").val(Master.Total), $("#Vat").val(Master.Vat), $("#VatAmount").val(Master.VatAmount), $("#SubTotal").val(Master.SubTotal), $("#SurCharge").val(Master.SurCharge), $("#Cartage").val(Master.Cartage), $("#GrandTotal").val(Master.GrandTotal), $("#AcilTinNo").val(Master.AcilTinNo), $("#PurchaseOrderNo").val(Master.PurchaseOrderNo)
    }, window.GetChild = function () {
        for (var t = Child, e = 0; e < t.length; e++) {
            var a = t[e];
            $("#webgrid tbody tr").each(function (t) {
                if (0 != $(this)[0].rowIndex) {
                    var e = $(this).children().eq(0).text().trim();
                    if (e == a.IndentId) {
                        var n = $(this).children().eq(5).find("input").val();
                        null == n && (n = 0), $(this).children().eq(5).find("input").val(+n + +a.Qty), $(this).children().eq(6).find("input").val(a.Qty), $(this).children().eq(7).find("input").val(a.Rate), $(this).children().eq(8).find("input").val(a.Discount), $(this).children().eq(9).find("input").val(a.Amount), $(this).children().eq(0).attr("UId", a.UId), $(this).children().eq(0).attr("MUId", a.MUId), $(this).children().eq(0).attr("SrNo", a.SrNo)
                    }
                }
            })
        }
    }
});