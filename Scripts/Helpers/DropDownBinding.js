/// <reference path="C:\Users\SC\Desktop\MMS_APP_13072018\acilmms.sitaapp.com\Scripts/jquery-1.10.2.js" />

function BindDdl(Id,Url,Data,Name) {
    var selectedDeviceModel = $(Id);

   
     selectedDeviceModel.empty();
    $.ajax({
        type: 'Get',
        url: Url, // Calling json method
        dataType: 'json',
        global:true,
        data: Data,
        complete: function () {
        },
        success: function (result) {
            var procemessage = "<option value=''> Please wait...</option>";
            selectedDeviceModel.html(procemessage).show();
            var markup = "<option value=''>Select "+Name+"</option>";
            selectedDeviceModel.html(markup).show();
            result = $.parseJSON(result)
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
            $('#loadingModal').modal('hide');
            selectedDeviceModel.append("<option value=''>Select " + Name + "</option>");
            alert("Failed to retrieve " + Name);
            
        }
    });
}

function BindDdlWithoutLabel(Id, Url, Data) {
    var selectedDeviceModel = $(Id);

    console.log(Data);

    selectedDeviceModel.empty();
    $.ajax({
        type: 'Get',
        url: Url, // Calling json method
        dataType: 'json',
        global: true,
        data: Data,
        complete: function () {
        },
        success: function (result) {
            var procemessage = "<option value=''> Please wait...</option>";
            selectedDeviceModel.html(procemessage).show();
            var markup = "";
            selectedDeviceModel.html(markup).show();
            result = $.parseJSON(result)
            $.each(result, function (index, item) {
                selectedDeviceModel.append(
                    $('<option/>', {
                        value: item.Value,
                        text: item.Text
                    })
                );
            });
            //$(selectedDeviceModel).select2();
           

        },
        error: function (ex) {
            alert("Failed to retrieve Data");

        }
    });
}


function BindDdlWithFun(Id, Url, Data, Name,fun) {
    var selectedDeviceModel = $(Id);


    selectedDeviceModel.empty();
    $.ajax({
        type: 'Get',
        url: Url, // Calling json method
        dataType: 'json',
        global: true,
        data: Data,
        complete: function () {
        },
        success: function (result) {
            var procemessage = "<option value=''> Please wait...</option>";
            selectedDeviceModel.html(procemessage).show();
            var markup = "<option value=''>Select " + Name + "</option>";
            selectedDeviceModel.html(markup).show();
            result = $.parseJSON(result)
            $.each(result, function (index, item) {
                selectedDeviceModel.append(
                    $('<option/>', {
                        value: item.Value,
                        text: item.Text
                    })
                );
            });
            fun();
        },
        error: function (ex) {
            selectedDeviceModel.append("<option value=''>Select " + Name + "</option>");
            alert("Failed to retrieve " + Name);

        }
    });
}

function ClearDdl(Id,Name) {
    var selectedDeviceModel = $(Id);
    selectedDeviceModel.empty();
    selectedDeviceModel.append("<option value=''>Select " + Name + "</option>");
}

