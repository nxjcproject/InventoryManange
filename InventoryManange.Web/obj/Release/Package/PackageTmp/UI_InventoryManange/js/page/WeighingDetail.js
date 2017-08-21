$(function () {
    InitDate();
    loadDataGrid("first");
    loadWeighingDetailDataGrid("first");
    loadCountdayDataGrid("first");
    $("#WeighingWindow").window({
        onClose: function () {
            loadWeighingDetailDataGrid("first");
        }
    })
    $("#WeighingCountday").window({
        onClose: function () {
            loadCountdayDataGrid("first");
        }
    })
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getDate() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate() + " 00:00:00";
    $('#startTime').datetimebox('setValue', beforeString);
    $('#endTime').datetimebox('setValue', nowString);
    $('#StartTimeWindow').datetimebox('setValue', beforeString);
    $('#EndTimeWindow').datetimebox('setValue', nowString);
}
var mOrganizationId = '';
function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    mOrganizationId = node.OrganizationId;
}
function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#gridMain_Weighing').datagrid({
            columns: [[
                    { field: 'Material', title: '物料编码', width: 160 },
                    { field: 'MaterialName', title: '名称', width: 140 },
                    { field: 'ggxh', title: '品种', width: 160 },
                    { field: 'inputValue', title: '进厂量', width: 90 },
                    { field: 'outputValue', title: '出厂量', width: 80 },
                    { field: 'vehicleValue', title: '车数', width: 80 },
                    {
                        field: 'edit', title: '查询', width: 150, formatter: function (value, row, index) {
                            var str = "";
                            str = '<a href="#" onclick="weightFun(\'' + row.Material + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note.png"/>过磅明细</a>';
                            str = str + '<a href="#" onclick="dayCountFun(\'' + row.Material + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note.png"/>日统计</a>';
                            return str;
                        }
                    }
            ]],
            fit: true,
            toolbar: "#toolbar_Weighing",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
        })
    }
    else {
        $('#gridMain_Weighing').datagrid("loadData", myData);
    }
}
var startTime = '';
var endTime = '';
var selectTime = '';
function Query() {
    startTime = $('#startTime').datetimebox('getValue');//开始时间
    endTime = $('#endTime').datetimebox('getValue');//结束时间
    var materialName = $('#MaterialName').textbox('getText');
    selectTime = $('#selectTime').combobox('getValue');
    SelectOrganizationName = $('#organizationName').textbox('getText');
    SelectDatetime = startTime + ' 至 ' + endTime;
    if (mOrganizationId == "") {
        $.messager.alert('警告', '请选择组织机构');
        return;
    }
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "WeighingDetail.aspx/GetWeighingInfo",
        data: "{mOrganizationId:'" + mOrganizationId + "',mAterialName:'" + materialName + "',mStartTime:'" + startTime + "',mEndTime:'" + endTime + "',mSelectTime:'" + selectTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData != undefined && myData.length == 0) {
                loadDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                loadDataGrid("last", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#Windows_Report").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    })
}
function loadWeighingDetailDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_WeighingDetail').datagrid({
            columns: [[
                    { field: 'BillNO', title: '过磅单编号', width: 140 },
                    { field: 'Gross', title: '毛重', width: 70 },
                    { field: 'Tare', title: '皮重', width: 70 },
                    { field: 'Suttle', title: '净重', width: 70 },
                    { field: 'Cars', title: '车辆编号', width: 90 },
                    { field: 'carsName', title: '车牌号', width: 70 },
                    { field: 'Material', title: '物料编号', width: 160 },
                    { field: 'MaterialName', title: '物料名称', width: 140 },
                    { field: 'ggxh', title: '品种', width: 160 },
                    {
                        field: 'Type', title: '类型', width: 50,
                        //formatter: function (value, row) {
                        //    if (row.Type == '0') {
                        //        return "采购";
                        //    }
                        //    if (row.Type == '3') {
                        //        return "销售";
                        //    }
                        //    if (row.Type == '5') {
                        //        return "调拨";
                        //    }
                        //}
                    },
                    { field: 'companyid', title: '公司编号', width: 70 },
                    { field: 'TORORGID', title: '移入厂区pk', width: 100 },
                    { field: 'TORORGNAME', title: '移入厂区名称', width: 100 },
                    { field: 'FROMORGID', title: '移出厂区PK', width: 100 },
                    { field: 'FROMORGNAME', title: '移出厂区名称', width: 100 },
                    {
                        field: 'reservationchar7', title: '调拨', width: 70,
                        //formatter: function (value, row) {
                        //    if (row.reservationchar7 == '10') {
                        //        return "调出";
                        //    }
                        //    if (row.reservationchar7 == '11') {
                        //        return "调入";
                        //    }
                        //}
                    },
                    { field: 'weightdate', title: '重车时间', width: 130 },
                    { field: 'lightdate', title: '轻车时间', width: 130 },
                    { field: 'lydh1', title: '来源单据编号1', width: 120 },
                    { field: 'sales_lydd1', title: '来源订单1id', width: 165 },
                    { field: 'lydh2', title: '来源单据编号2', width: 120 },
                    { field: 'sales_lydd2', title: '来源订单2id', width: 165 },
                    {
                        field: 'sales_gblx', title: '销售类型', width: 70,
                        //formatter: function (value, row) {
                        //    if (row.sales_gblx == 'RD') {
                        //        return "退货";
                        //    }
                        //    if (row.sales_gblx == 'DE') {
                        //        return "普通销售";
                        //    }
                        //}
                    },
            ]],
            fit: true,
            toolbar: "",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
        })
    }
    else {
        $('#grid_WeighingDetail').datagrid("loadData", myData);
    }
}
function weightFun(MaterialId) {
    $("#WeighingWindow").window('open');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "WeighingDetail.aspx/GetWeighingDetail",
        data: "{mOrganizationId:'" + mOrganizationId + "',mMaterialId:'" + MaterialId + "',mStartTime:'" + startTime + "',mEndTime:'" + endTime + "',mSelectTime:'" + selectTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData != undefined && myData.length == 0) {
                loadWeighingDetailDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                loadWeighingDetailDataGrid("last", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_WeighingDetail").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    })
}
//追加行数据
//function loadMoreInfo() {
//    var loadValue = $("#loadRows").textbox('getValue');
//    var index = $("#grid_WeighingDetail").datagrid('getRows').length-1;
//    var row = $('#grid_WeighingDetail').datagrid('getData').rows[index];
//    var footerRowsDate = row.weightdate;
//    var loadMaterialId = row.Material;
//    var win = $.messager.progress({
//        title: '请稍后',
//        msg: '数据载入中...'
//    });
//    $.ajax({
//        type: "POST",
//        url: "WeighingDetail.aspx/GetWeighingDetailLoad",
//        data: "{mOrganizationId:'" + mOrganizationId + "',mloadMaterialId:'" + loadMaterialId + "',mfooterRowsDate:'" + footerRowsDate + "',mEndTime:'" + endTime + "',mSelectTime:'" + selectTime + "',mloadValue:'" + loadValue + "'}",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            $.messager.progress('close');
//            var myData = jQuery.parseJSON(msg.d);
//            for (var i = 0; i < myData.total; i++) {
//                $("#grid_WeighingDetail").datagrid("appendRow", myData.rows[i]);
//            }         
//        },
//        beforeSend: function (XMLHttpRequest) {         
//            win;
//        },
//        error: function () {
//            $.messager.progress('close');
//            $.messager.alert('失败', '加载失败！');
//        }
//    })
//}
function loadCountdayDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_WeighingCountday').datagrid({
            columns: [[
                    { field: 'StatisticsTime', title: '日期', width: 90 },
                    { field: 'Material', title: '物料编码', width: 160 },
                    { field: 'MaterialName', title: '名称', width: 110 },
                    { field: 'ggxh', title: '品种', width: 160 },
                    { field: 'inputValue', title: '进厂量', width: 80 },
                    { field: 'outputValue', title: '出厂量', width: 80 },
                    { field: 'vehicleValue', title: '车数', width: 50 }
            ]],
            fit: true,
            toolbar: "#toorBarCountday",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
        })
    }
    else {
        $('#grid_WeighingCountday').datagrid("loadData", myData);
    }
}
var dayMaterialId = '';
function dayCountFun(MaterialId) {
    $("#WeighingCountday").window('open');
    dayMaterialId = MaterialId;
}
var daystartTime = '';
var dayendTime = '';
function SearchWeightday() {
    daystartTime = $('#StartTimeWindow').datetimebox('getValue');//开始时间
    dayendTime = $('#EndTimeWindow').datetimebox('getValue');//结束时间
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "WeighingDetail.aspx/GetWeighingCountDay",
        data: "{mOrganizationId:'" + mOrganizationId + "',mdayMaterialId:'" + dayMaterialId + "',mdaystartTime:'" + daystartTime + "',mdayendTime:'" + dayendTime + "',mSelectTime:'" + selectTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData != undefined && myData.length == 0) {
                loadCountdayDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                loadCountdayDataGrid("last", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_WeighingCountday").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    })
}
var SelectOrganizationName = '';
var SelectDatetime = '';
function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetDataGridTableHtml("gridMain_Weighing", "过磅明细", SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "WeighingDetail.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
}
function ExportFileFunDay() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetDataGridTableHtml("grid_WeighingCountday", "过磅日统计", SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "WeighingDetail.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
}
function ExportFileDetail() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetDataGridTableHtml("grid_WeighingDetail", "过磅明细", SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "WeighingDetail.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
}