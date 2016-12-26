var m_KeyID = "";
var m_DatabaseID = "";
var m_Count = 0;
var m_TableList = [];
var m_organizationId = "";
var m_warehouseName = "";
$(document).ready(function () {
    //LoadProductionType('first');
    //loadOrganisationTree('first');
    loadDataGrid("first");
    //$('#TextBox_OrganizationId').textbox('hide');
    InitDate();
    //LoadEnergyConsumptionData('first');
    initPageAuthority();
    //LoadHtml(g_templateURL);

    ///// 测试
    //t_url = "";
    //g_templateURL = "/UI_CenterControl/ReportTemplete/" + t_url;
    //$("#contain").load(g_templateURL);
    //loadDataGrid("first");
});

function InitDate() {
    var myDate = new Date();
    var DateString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds();
    $('#dbox_QueryDate').datetimebox('setValue', DateString);
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
            //增加
            //if (authArray[1] == '0') {
            //    $("#add").linkbutton('disable');
            //}
            //修改
            if (authArray[2] == '0') {
                $("#id_save").linkbutton('disable');
            }
            //删除
            //if (authArray[3] == '0') {
            //    $("#delete").linkbutton('disable');
            //}
        }
    });
}
//单击事件
function onOrganisationTreeClick(myNode) {
    //alert(myNode.text);
    m_organizationId = myNode.OrganizationId;
    if (myNode.OrganizationType== "分公司")
    {
        $.messager.alert('提示', '请选择到分厂！');
        return
    }
    $('#TextBox_OrganizationId').attr('value', m_organizationId);  //textbox('setText', myNode.OrganizationId);
    $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    //$('#TextBox_OrganizationType').textbox('setText', myNode.OrganizationType)
    PrcessTypeItem(m_organizationId);
}
//仓库选择
function PrcessTypeItem(m_OrganizationId) {
    $.ajax({
        type: "POST",
        url: "InventoryQuery.aspx/GetWarehouseName",
        data: "{myOrganizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            var comboboxData = new Array();
            comboboxData[0] = { "text": "全部" };
            for (i = 1; i < m_MsgData.rows.length + 1; i++) {
                comboboxData[i] = m_MsgData.rows[i - 1];
            }

            if (m_MsgData.total == 0) {
                $.messager.alert('提示', '未查询到仓库', 'info');
            }
            //InitializeEnergyConsumptionGrid(m_GridCommonName, m_MsgData);
            $('#comb_ProcessType').combobox({
                data: comboboxData,
                valueField: 'text',
                //textField: 'text',
                onSelect: function (param) {
                    m_warehouseName = param.text;
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '仓库加载失败！');
        }
    });
}
//function RecordNameItem(OrganizationId, ProductionprocessId) {
//    var m_OrganizationID = OrganizationId;
//    $.ajax({
//        type: "POST",
//        url: "CenterControlRecord.aspx/GetRecordNameItem",
//        data: "{myOrganizationId:'" + OrganizationId + "',ProductionprocessId:'" + ProductionprocessId + "'}",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            var m_MsgData = jQuery.parseJSON(msg.d);
//            if (m_MsgData.total == 0) {
//                $.messager.alert('提示', '未查询到该类别下的产线！')
//            }
//            $('#comb_LineType').combobox({
//                data: m_MsgData.rows,
//                valueField: 'id',
//                textField: 'text',
//                onSelect: function (param) {
//                    m_KeyID = param.KeyID;
//                    m_DatabaseID = param.DatabaseID;
//                    QueryTableCount(m_KeyID);
//                    LoadHtml(m_KeyID);

//                }
//            });
//        },
//        error: function () {
//            $.messager.alert('提示', '操作记录类型加载失败！');
//        }
//    });
//}
//加载报表模板
//function LoadHtml(KeyId) {
//    $.ajax({
//        type: "POST",
//        url: "CenterControlRecord.aspx/GetHtmlTemplete",
//        data: "{KeyID:'" + KeyId + "'}",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            var m_MsgData = jQuery.parseJSON(msg.d);
//            if (m_MsgData.rows[0]["TemplateUrl"] == "") {
//                $.messager.alert('提示', '未查询到该记录的模板！');
//            }
//            t_url = m_MsgData.rows[0]["TemplateUrl"];
//            g_templateURL = "/UI_CenterControl/ReportTemplete/" + t_url;
//            //$("#contain").empty();
//            $("#contain").load(g_templateURL);
//        },
//        error: function () {
//            $.messager.alert('提示', '记录模板加载失败！');
//        }
//    })

//}

//function QueryTableCount(KeyId) {
//    $.ajax({
//        type: "POST",
//        url: "CenterControlRecord.aspx/GetSumCount",
//        data: "{KeyID:'" + KeyId + "'}",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            var m_MsgData = jQuery.parseJSON(msg.d);
//            m_Count = m_MsgData;
//        },
//        error: function () {
//            $.messager.alert('提示', '未能加载字段个数！');
//        }
//    })
//}
//报表框架
function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#gridMain_ReportTemplate').datagrid({
            columns: [[
                    { field: 'WarehouseName', title: '仓库名称', width: 100 },
                    //{ field: 'OriginasitionID', title: '组织机构', width: 100 },
                    //{ field: 'Count', title: '运行次数', width: 80 },
               
                    { field: 'benchmarksValue', title: '基准库存', width: 80, align: 'right' },
                    { field: 'benchmarksTime', title: '基准时间', width: 75 },
                    { field: 'InputWarehouse', title: '入库量', width: 80 ,align:'right'},
                    { field: 'OutputWarehouse', title: '出库量', width: 80,align:'right' },
                    { field: 'CurrentInventory', title: '当前库存', width: 80, align: 'right' }
            ]],
            fit: true,
            toolbar: "#toolbar_ReportTemplate",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#gridMain_ReportTemplate').datagrid({ data: [] });
        $('#gridMain_ReportTemplate').datagrid('loadData', myData);
        // $('#gridMain_ReportTemplate').datagrid("collapseAll");
    }
}
//单击查询加载仓库信息报表
function QueryReportFun() {
    editIndex = undefined;
    var startTime = $('#dbox_QueryDate').datetimebox('getValue');//开始时间
    //var endTime = $('#endDate').datetimebox('getValue');//结束时间
    if (m_organizationId == "" || startTime == "" || m_warehouseName == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    //if (startDate > endDate) {
    //    $.messager.alert('警告', '结束时间不能大于开始时间！');
    //    return;
    //}
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "InventoryQuery.aspx/GetInventory",
        data: '{organizationID: "' + m_organizationId + '", warehouseName: "' + m_warehouseName + '", startTime: "' + startTime + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (XMLHttpRequest) {
              
            win;
            },
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0) {
                $('#gridMain_ReportTemplate').datagrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                loadDataGrid("last", m_MsgData);
            }
        },
        error: function handleError() {
            $.messager.progress('close');
            $('#gridMain_ReportTemplate').datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function RefreshRecordDataFun()
{ QueryCenterControlReportInfoFun(); }

//function LoadTagDatagrid() {
//    $('#datagrid_Tag').datagrid({
//        columns: [[
//            { field: 'DisplayIndex', title: '标签号', width: 50 },
//            { field: 'VariableDescription', title: '名称', width: 150 },
//            { field: 'ContrastID', title: '标签名', width: 100, align: 'right' },
//            { field: 'Enabled', title: '是否可见', width: 80, align: 'right' },
//            { field: 'DatabaseID', title: 'DCS数据库', width: 140 },
//            { field: 'DCSTableName', title: 'DCS表名', width: 120 }
//        ]]
//    });

//}

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


