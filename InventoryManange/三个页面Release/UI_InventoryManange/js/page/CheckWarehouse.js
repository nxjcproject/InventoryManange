$(function () {
    InitialDate();
    LoadDataGrid("first");
    //mLoadDataGrid("first");
});
function InitialDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getDate() - 30);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate()  + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate()  + " 00:00:00";
    $('#startTime').datetimebox('setValue', beforeString);
    $('#endTime').datetimebox('setValue', nowString);
    $('#startTimeWindow').datetimebox('setValue', beforeString);
    $('#endTimeWindow').datetimebox('setValue', nowString);
}
function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    mOrganizationId = node.OrganizationId;
    //LoadStaffInfo(mOrganizationId);
    PrcessTypeItem(mOrganizationId);
    //PrcessTypeItemWindow(mOrganizationId)
}
var wareHouseId = '';
function PrcessTypeItem(mOrganizationId) {
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/GetInventoryName",
        data: "{myOrganizationId:'" + mOrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg){
            var m_MsgData = jQuery.parseJSON(msg.d);
            var comboboxData = new Array();
            comboboxData[0] = { "Id": "All", "text": "全部" };
            for (i = 1; i < m_MsgData.rows.length + 1; i++) {
                comboboxData[i] = m_MsgData.rows[i - 1];
            }
            if (m_MsgData.total == 0) {
                $.messager.alert('提示', '未查询到仓库', 'info');
            }
            $('#comb_ProcessType').combobox({
                data: comboboxData,
                valueField: 'Id',
                textField: 'text',
                onSelect: function (param) {
                    wareHouseId = param.Id;
                }
            });
            $('#comb_ProcessTypeWindow').combobox({
                data: comboboxData,
                valueField: 'Id',
                textField: 'text',
                onSelect: function (param) {
                    wareHouseWindowId = param.Id;
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '仓库名加载失败！');
        }
    });
}
function LoadDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
                    { field: 'Name', title: '仓库类型', width: 100 },                
                    {
                        field: 'Value', title: '库存值', width: 80, align: "right",
                    },
                    {
                        field: 'TimeStamp', title: '启用时间', width: 160, align: "left",
                    },
                    { field: 'Editor', title: '编辑人', width: 80, align: "left" },
                    { field: 'EditTime', title: '编辑时间', width: 160, align: "left" },
                    {
                        field: 'edit', title: '编辑', width: 100, formatter: function (value, row, index) {
                            var str = "";
                            str = '<a href="#" onclick="editWarehouse(\'' + row.ItemId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editWarehouse(\'' + index+ '\')"/>编辑</a>';
                            str = str + '<a href="#" onclick="deleteWarehouse(\'' + row.ItemId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteWarehouse(\'' + row.Id + '\')"/>删除</a>';                           
                            return str;
                        }
                    }
            ]],
            fit: true,
            toolbar: "#toorBar",
            rownumbers: true,
            idField:'ItemId',
            singleSelect: true,
            striped: true,
            data: [],
        });
    }
    else {
        $('#grid_Main').datagrid('loadData', myData);
    }
}

//var mger = Object;
function Query() {
    var mOrganizationID = $('#organizationId').val();
    var beginTime = $('#startTime').datebox('getValue');
    var endTime = $('#endTime').datebox('getValue');
    var type = $('#comb_ProcessType').combobox('getValue');
    if (mOrganizationID == "") {
        $.messager.alert('警告', '请选择组织机构');
        return;
    }
    var mUrl = "";
    var mdata = "";
    if (type=='All') {
        mUrl = "CheckWarehouse.aspx/GetInventoryAll";
        mdata = "{mOrganizationID:'" + mOrganizationID + "',beginTime:'" + beginTime + "',endTime:'" + endTime + "'}";
    } else  {
        mUrl = "CheckWarehouse.aspx/GetInventory";
        mdata = "{mOrganizationID:'" + mOrganizationID + "',beginTime:'" + beginTime + "',endTime:'" + endTime + "',wareHouseId:'" + wareHouseId + "'}";
    }
    $.ajax({
        type: "POST",
        url: mUrl,
        data: mdata,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //mger.window('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0) {
                $('#grid_Main').datagrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                LoadDataGrid("last", m_MsgData);
            }
        },
        //beforeSend: function (XMLHttpRequest) {
        //    //alert('远程调用开始...');
        //    mger = $.messager.alert('提示', "加载中...");
        //},
        error: function handleError() {
            $('#grid_Main').datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function refresh() {
    Query();
    QueryWindow();
}

var saveId = '';
function editWarehouse(editContrastId) {
    $('#grid_Main').datagrid('selectRecord', editContrastId);
    var data = $('#grid_Main').datagrid('getSelected');
    $('#productionName').numberbox('setValue', data.Value);
    $('#editTime').datetimebox('setValue', data.TimeStamp);
    saveId = data.ItemId;
    $('#editHouse').window('open');
}

function save() {
    var saveValue = $('#productionName').numberbox('getValue');
    var saveTimeStamp = $('#editTime').datetimebox('getValue');
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/SaveWarehouse",
        data: "{saveId:'" + saveId + "',saveValue:'"+saveValue+"',saveTimeStamp:'"+saveTimeStamp+"'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = msg.d;
            if (myData == 1) {
                $.messager.alert('提示', '保存成功成功！');
                $('#editHouse').window('close');
                refresh();
            }
        }
        });
}
function deleteWarehouse(deleteContrastId) {

    $('#grid_Main').datagrid('selectRecord', deleteContrastId);
    var data = $('#grid_Main').datagrid('getSelected');

    var mId = data.ItemId;
    $.messager.confirm('提示', '确定要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: "CheckWarehouse.aspx/DeleteWarehouse",
                data: "{mId:'" + mId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData == 1) {
                        $.messager.alert('提示', '删除成功！');
                        //$('#AddandEditor').window('close');
                        refresh();
                    }
                    else {
                        $.messager.alert('提示', '操作失败！');
                        refresh();
                    }
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                    //$('#AddandEditor').window('close');
                    refresh();
                }
            });
        }
    });
}

//var mger1 = Object;
function QueryWindow() {
    var mOrganizationID = $('#organizationId').val();
    var beginTime = $('#startTimeWindow').datebox('getValue');
    var endTime = $('#endTimeWindow').datebox('getValue');
    var type = $('#comb_ProcessTypeWindow').combobox('getValue');
    if (mOrganizationID == "") {
        $.messager.alert('警告', '请选择组织机构');
        return;
    }
    var amUrl = "";
    var amdata = "";
    if (type == 'All') {
        amUrl = "CheckWarehouse.aspx/WindowWarehouseAll";
        amdata = "{mOrganizationID:'" + mOrganizationID + "',beginTime:'" + beginTime + "',endTime:'" + endTime + "'}";
    } else {
        amUrl = "CheckWarehouse.aspx/WindowWarehouse";
       amdata = "{mOrganizationID:'" + mOrganizationID + "',beginTime:'" + beginTime + "',endTime:'" + endTime + "',wareHouseWindowId:'" + wareHouseWindowId + "'}";
    }
    $.ajax({
        type: "POST",
        url: amUrl,
        data: amdata,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //mger1.window('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0) {
                $('#grid_MainWindow').datagrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                mLoadDataGrid("last", m_MsgData);
            }
        },
        //beforeSend: function (XMLHttpRequest) {
        //    //alert('远程调用开始...');
        //    mger1 = $.messager.alert('提示', "加载中...");
        //},
        error: function handleError() {
            $('#grid_MainWindow').datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function addFun() {
    $('#AddandEditor').window('open');
    mLoadDataGrid("first");
}
function mLoadDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_MainWindow').datagrid({
            columns: [[
                    { field: 'Name', title: '仓库类型', width: 100 },
                    {
                        field: 'Value', title: '库存值', width: 80, align: "left",
                        editor: { type: 'numberbox', options: { precision: 2 } }
                    },
                    {
                        field: 'TimeStamp', title: '启用时间', width: 160, align: "left",
                        editor: { type: 'datetimebox' }
                    }
            ]],
            fit: true,
            toolbar: "toorBarWindows",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
            onClickCell: onClickCella
        });
    }
    else {
        $('#grid_MainWindow').datagrid('loadData', myData);
    }
}
$.extend($.fn.datagrid.methods, {
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }
            $(this).datagrid('beginEdit', param.index);
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});
var editIndexa = undefined;      //重置编辑索引行
function endEditinga() {
    if (editIndexa == undefined) { return 'true' }     //返回真允许编辑
    if ($("#grid_MainWindow").datagrid('validateRow', editIndexa)) {
        $("#grid_MainWindow").datagrid('endEdit', editIndexa);
        editIndexa = undefined;
        return 'true';
    } else {
        return 'false';
    }

}
function onClickCella(index, field) {
    if (endEditinga()) {
        $("#grid_MainWindow").datagrid('selectRow', index)
                .datagrid('editCell', { index: index, field: field });
        editIndexa = index;
    }

}
function saveSectionType() {
    endEditinga();
    var windowData = $("#grid_MainWindow").datagrid('getData');

   
    //var jsonData = windowData.toJSONString();
    //var str = obj.toJSONString();
    var str = JSON.stringify(windowData);
    //var length = windowData.length;
    //for (var i = 0; i < length; i++) {
    //    var windowId = windowData[i].Id;
    //    var windowValue = windowData[i].Value;
    //    var windowTimeStamp = windowData[i].TimeStamp;
        $.ajax({
            type: "POST",
            url: "CheckWarehouse.aspx/AddWarehouse",
            data: '{json:\''+str+'\'}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == "success"){
                    $.messager.alert('提示', '保存成功！');
                }
                
                $('#AddandEditor').window('close');
                //refresh();
            }
        });       
    }
    //refresh()



