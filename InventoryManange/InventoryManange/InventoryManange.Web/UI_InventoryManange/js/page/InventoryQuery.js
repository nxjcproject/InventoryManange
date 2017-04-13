var m_KeyID = "";
var m_DatabaseID = "";
var m_Count = 0;
var m_TableList = [];
var m_organizationId = "";
var m_warehouseName = "";
$(document).ready(function () {  
    InitDate();
    initPageAuthority();
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
            
            if (authArray[2] == '0') {
                $("#id_save").linkbutton('disable');
            }
        }
    });
}
单击事件
function onOrganisationTreeClick(myNode) {
    alert(myNode.text);
    m_organizationId = myNode.OrganizationId;
    if (myNode.OrganizationType== "分公司")
    {
        $.messager.alert('提示', '请选择到分厂！');
        return
    }
    $('#TextBox_OrganizationId').attr('value', m_organizationId);  //textbox('setText', myNode.OrganizationId);
    $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    $('#TextBox_OrganizationType').textbox('setText', myNode.OrganizationType)
}
//报表框架
function loadDataGrid(myData) {
    $('#gridMain_ReportTemplate').treegrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        rownumbers: true,
        singleSelect: true,

        idField: "id",
        treeField: "WarehouseName",

        toolbar: '#toolbar_ReportTemplate'
    });
}
//单击查询加载仓库信息报表
function QueryReportFun() {
    editIndex = undefined;
    var startTime = $('#dbox_QueryDate').datetimebox('getValue');//开始时间
    if (m_organizationId == "" || startTime == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "InventoryQuery.aspx/GetInventory",
        data: '{organizationID: "' + m_organizationId + '", warehouseName: "全部", startTime: "' + startTime + '"}',
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
                loadDataGrid(m_MsgData);
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


