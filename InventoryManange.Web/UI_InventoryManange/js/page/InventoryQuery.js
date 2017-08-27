var targetTimeDateBoxName;
var m_organizationId;
$(document).ready(function () {
    initPageAuthority();
    InitDateTime();
    InitSearchBox();
    LoadDataGrid("first");
    LoadDataGridWindow("first", { "rows": [], "total": 0 });
});
   
//搜索框初始化
function InitSearchBox() {
    $("#sbox_QueryStartDate").searchbox({
        prompt: '',
        searcher: function () {
            if (m_organizationId == "") {
                $.messager.alert('警告', '请选择生产线');
                return;
            };
            targetTimeDateBoxName = "sbox_QueryStartDate";
            LoadTime();
        }
    })
    $("#sbox_QueryEndDate").searchbox({
        prompt: '',
        searcher: function () {
            if (m_organizationId == "") {
                $.messager.alert('警告', '请选择生产线');
                return;
            };
            targetTimeDateBoxName = "sbox_QueryEndDate";
            LoadTime();
        }
    })
}
function InitDateTime() {
    var myDate = new Date();
    var nowDateString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds();
    var beforeDataString = (myDate.getFullYear() - 1) + '-' + (myDate.getMonth() + 1) + '-' + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds();

    $("#startTimeF").datetimebox('setValue', beforeDataString);//开始日期框初值
    $("#EndTimeF").datetimebox('setValue', nowDateString);//结束日期框初值
}
//弹窗时间加载函数，共用
function LoadTime() { 
    $('#TimeSelector').window('open');
    var m_StartTime = $("#startTimeF").datetimebox('getValue');
    var m_EndTime = $("#EndTimeF").datetimebox('getValue');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "InventoryQuery.aspx/GetInventoryHouseTime",
        data: '{organizationID: "' + m_organizationId + '",startTimeWindow:"' + m_StartTime + '",endTimeWindow:"' + m_EndTime + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.rows.length == 0) {
                $.messager.alert('提示', '没有查到相关时间信息！');
            }
            else {
                LoadDataGridWindow("last", m_MsgData);//基准时间弹窗与盘库时间弹窗LoadDataGridWindow不同
            }

        },
        error: function handleError() {
            $.messager.progress('close');
            $.messager.alert('失败', '获取数据失败');
        }
    });

}
//查询函数
function SearchCheckInventoryTimeFun() {
    LoadTime();
}
//搜索框弹窗加载
function LoadDataGridWindow(type, myData) {
    if (type == "first") {
        $('#grid_DateTimeSelector').datagrid({
            title: '',
            data: myData,
            dataType: "json",
            fit: true,
            idField: 'TimeStamp',
            striped: true,
            rownumbers: true,
            singleSelect: true,
            toolbar: '#toolBar_DateTimeSelector',
            columns: [[
                { field: 'TimeStamp', title: '盘库时间', width: 400, align: "center" }
            ]],
            onDblClickRow: function (rowIndex, rowData) {
                $("#" + targetTimeDateBoxName).searchbox({
                    value:rowData.TimeStamp
                }),
                $("#TimeSelector").window('close')
            }
        });
    }
    else {
        $('#grid_DateTimeSelector').datagrid("loadData", myData);
    }

}

//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            var authArray = msg.d;

            if (authArray[2] == '0') {
                $("#id_save").linkbutton('disable');
            }
        }
    });
}
//单击事件
function onOrganisationTreeClick(myNode) {
    m_organizationId = myNode.OrganizationId;
    if (myNode.OrganizationType == "分公司") {
        $.messager.alert('提示', '请选择到分厂！');
        return
    }
    $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    $('#TextBox_OrganizationType').textbox('setText', myNode.OrganizationType)
}
function LoadDataGrid(type, myData) {
    if (type == "first") {
        $('#gridMain_ReportTemplate').treegrid({
            title: '',
            data: [],
            dataType: "json",
            striped: true,
            rownumbers: true,
            singleSelect: true,
            idField: "id",
            treeField: "WarehouseName",
            toolbar: '#toolBar_ReportTemplate',
            columns: [[
                    { field: 'WarehouseName', title: '仓库名称', width: 160 },
                    { field: 'benchmarksTime', title: '基准时间', width: 120, align: "center",hidden:true},
                    { field: 'benchmarksValue', title: '上次盘库库存', width: 100, align: "right" },
                    { field: 'InputWarehouse', title: '入库量', width: 80, align: "right" },
                    { field: 'OutputWarehouse', title: '出库量', width: 80, align: "right" },
                    {
                        field: 'CurrentInventory', title: '动态库存', width: 100, align: "right",
                        formatter: function FormatInventory(value, row) {
                            var AlarmEnabled = row.AlarmEnabled;   //只有设定报警有效才报警
                            var H = row.highLimit;
                            var L = row.lowLimit;
                            if (AlarmEnabled == "1" && value > parseFloat(H)) {
                                var s = '<div style="width:100%;background:#cc0000">' + value + '</div>'
                                return s
                            }
                            else if (AlarmEnabled == "1" && value < parseFloat(L)) {
                                var s = '<div style="width:100%;background:yellow">' + value + '</div>'
                                return s
                            }
                            else {
                                return value
                            }
                        }
                    },
                    { field: 'panKuValue', title: '本次盘库库存', width: 100, align: "right" },
                    { field: 'offSet', title: '偏差比(动态/盘库)', width: 110, align: "right" },
                    { field: 'highLimit', title: '最高', width: 80, align: "right", hidden: true },
                    { field: 'lowLimit', title: '最低', width: 80, align: "right", hidden: true }

            ]]

        });
    }
    else {
        $('#gridMain_ReportTemplate').treegrid('loadData', myData);
    }
}

//单击查询加载仓库信息报表
function QueryReportFun() {
    editIndex = undefined;
    var startTime = $('#sbox_QueryStartDate').searchbox('getValue');//基准时间
    var endTime = $('#sbox_QueryEndDate').searchbox('getValue');//库存时间
    if (m_organizationId == "" || startTime == "" || endTime=="") {
        $.messager.alert('警告', '请选择生产线或时间');
        return;
    }
    if (startTime > endTime) {
        $.messager.alert('警告', '请选择合适的基准时间和盘库时间');
        return;
    }
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "InventoryQuery.aspx/GetInventory",
        data: '{organizationID: "' + m_organizationId + '", warehouseName: "全部", startTime: "' + startTime + '",endTime:"'+endTime+'"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (XMLHttpRequest) {

            win;
        },
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.length == 0) {
                $.messager.alert('提示', '没有查到相关库存信息！');
            }
            else {
                LoadDataGrid("last", m_MsgData);
            }

        },
        error: function handleError() {
            $.messager.progress('close');
            $.messager.alert('失败', '获取数据失败');
        }
    });
}


