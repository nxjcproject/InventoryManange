var m_KeyID = "";
var m_DatabaseID = "";
var m_Count = 0;
var m_TableList = [];
var m_organizationId = "";
var m_warehouseName = "";
var startStartTimeWindow = "";
var startEndTimeWindow = "";
var startTimeWindow = "";
var endTimeWindow = "";
var myDate = new Date();
var nowDateString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds();
var beforeDataString=(myDate.getFullYear()-1)+'-' + (myDate.getMonth() + 1) + '-' + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds();
$(document).ready(function () {
    LoadDataGrid("first");
    initPageAuthority();
    InitSearchBox();
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
            $("#startStartTimeWindow").datetimebox('setValue', beforeDataString);//开始日期框初值
            $("#startEndTimeWindow").datetimebox('setValue', nowDateString);//结束日期框初值
            $('#startTimeSelector').window('open');
            LoadStartDataGridWindow("first");//初始化数据表格
            LoadTime(LoadStartDataGridWindow, beforeDataString, nowDateString);
        }
    })
    $("#sbox_QueryEndDate").searchbox({
        prompt: '',
        searcher: function () {
            if (m_organizationId == "") {
                $.messager.alert('警告', '请选择生产线');
                return;
            };
            $("#endStartTimeWindow").datetimebox('setValue', beforeDataString);//开始日期框初值
            $("#endEndTimeWindow").datetimebox('setValue', nowDateString);//结束日期框初值
            $("#endTimeSelector").window('open');
            LoadEndDataGridWindow("first");
            LoadTime(LoadEndDataGridWindow, beforeDataString, nowDateString);
        }
    })
}
//弹窗时间加载函数，共用
function LoadTime(LoadDataGridWindow, startTimeWindow, endTimeWindow) {
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "InventoryQuery.aspx/GetInventoryHouseTime",
        data: '{organizationID: "' + m_organizationId + '",startTimeWindow:"' + startTimeWindow + '",endTimeWindow:"' + endTimeWindow + '"}',
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
function SearchStartTimeFun() {
    startTimeWindow = $("#startStartTimeWindow").datetimebox('getValue');
    endTimeWindow = $("#startEndTimeWindow").datetimebox('getValue');
    LoadTime(LoadStartDataGridWindow,startTimeWindow,endTimeWindow);
}
//点击查询函数
function SearchEndTimeFun() {
    startTimeWindow = $("#endStartTimeWindow").datetimebox('getValue');
    endTimeWindow = $("#endEndTimeWindow").datetimebox('getValue');
    LoadTime(LoadEndDataGridWindow, startTimeWindow, endTimeWindow);
}
//开始搜索框弹窗加载
function LoadStartDataGridWindow(type, myData) {
    if (type == "first") {
        $('#grid_StartMainWindow').datagrid({
            title: '',
            data: [],
            dataType: "json",
            striped: true,
            rownumbers: true,
            singleSelect: true,
           // toolbar: '#startToorBarWindows',
            columns: [[
                    { field: 'WarehouseName', title: '仓库名称', width: 160, hidden: true },
                    { field: 'TimeStamp', title: '基准时间', width: 400, align: "center" },
                    { field: 'benchmarksValue', title: '基准库存', width: 80, align: "right", hidden: true },
            ]],
            onDblClickRow: function (rowIndex, rowData) {
                $("#sbox_QueryStartDate").searchbox({
                    value:rowData.TimeStamp
                }),
                $("#startTimeSelector").window('close')
            }
        });
    }
    else {
        $('#grid_StartMainWindow').datagrid('loadData', myData);
    }

}
//结束搜索框弹窗加载
function LoadEndDataGridWindow(type, myData) {
    if (type == "first") {
        $('#grid_EndMainWindow').datagrid({
            title: '',
            data: [],
            dataType: "json",
            striped: true,
            rownumbers: true,
            singleSelect: true,
            // toolbar: '#toorBarWindows',
            columns: [[
                    { field: 'WarehouseName', title: '仓库名称', width: 160, hidden: true },
                    { field: 'TimeStamp', title: '盘库时间', width: 400, align: "center" },
                    { field: 'benchmarksValue', title: '基准库存', width: 80, align: "right", hidden: true },
            ]],
            onDblClickRow: function (rowIndex, rowData) {
                $("#sbox_QueryEndDate").searchbox({
                    value: rowData.TimeStamp
                }),
                $("#endTimeSelector").window('close')
            }
        });
    }
    else {
        $('#grid_EndMainWindow').datagrid('loadData', myData);
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
    $('#TextBox_OrganizationId').attr('value', m_organizationId);  //textbox('setText', myNode.OrganizationId);
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
            toolbar: '#toolbar_ReportTemplate',
            columns: [[
                    { field: 'WarehouseName', title: '仓库名称', width: 160 },
                    { field: 'benchmarksTime', title: '基准时间', width: 120, align: "center",hidden:true},
                    { field: 'benchmarksValue', title: '上次盘库库存', width: 100, align: "right" },
                    { field: 'InputWarehouse', title: '入库量', width: 80, align: "right" },
                    { field: 'OutputWarehouse', title: '出库量', width: 80, align: "right" },
                    {
                        field: 'CurrentInventory', title: '动态库存', width: 100, align: "right",
                        formatter: function FormatInventory(value, row) {
                            var H = row.highLimit;
                            var L = row.lowLimit;
                            if (value > parseFloat(H)) {
                                var s = '<div style="width:100%;background:#cc0000">' + value + '</div>'
                                return s
                            }
                            else if (value < parseFloat(L)) {
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
                    { field: 'lowLimit', title: '最低', width: 80, align: "right", hidden: true },

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
function RefreshRecordDataFun()
{ QueryCenterControlReportInfoFun(); }
function GetCenterControlReportTagInfoFun() {

    var OrganizationId = m_organizationId;
    var KeyID = m_KeyID;
    var DatabaseID = m_DatabaseID;
    var m_Time = $('#dbox_QueryDate').datebox('getValue');
    var m_SumCount = m_Count;

    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetTagDataJson",
        data: "{KeyID:'" + KeyID + "',DatabaseID:'" + DatabaseID + "',OrganizationId:'" + OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#datagrid_Tag').datagrid('loadData', m_MsgData);
        },
        error: function () {
            $.messager.alert('提示', '数据加载错误！');
        }
    })

    $('#dialog_Tag').dialog('open');
}



