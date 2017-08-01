$(function () {
    InitialDate();
    LoadDataGrid("first");
    LoadTimeStamp("first");
    //mLoadDataGrid("first");
    initPageAuthority();
});
function InitialDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getDate() - 30);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate()  + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    $('#startTime').datetimebox('setValue', beforeString);
    $('#endTime').datetimebox('setValue', nowString);
    $('#startTimeWindow').datetimebox('setValue', nowString);
    //$('#endTimeWindow').datetimebox('setValue', nowString);
}
function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    mOrganizationId = node.OrganizationId;
    //LoadStaffInfo(mOrganizationId);
    //PrcessTypeItem(mOrganizationId);
    //PrcessTypeItemWindow(mOrganizationId)
}
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            PageOpPermission = msg.d;
            //增加
            if (PageOpPermission[1] == '0') {
                $("#add").linkbutton('disable');
            }
            //修改
            //if (authArray[2] == '0') {
            //    $("#edit").linkbutton('disable');
            //}
            //删除
            //if (PageOpPermission[3] == '0') {
            //    $("#id_deleteAll").linkbutton('disable');
            //}
        }
    });
}
var nodeTimeStamp = '';
function LoadTimeStamp(type, myData) {
    if (type == "first") {
        $('#grid_Stamp').datagrid({
            columns: [[
                    { field: 'TimeStamp', title: '盘库时间', width: 130 },               
            ]],
            fit: true,      
            rownumbers: true,         
            singleSelect: true,
            striped: true,
            data: [],
            onDblClickRow: function (rowIndex, rowData) {
                QueryDetailedInfo(rowData.TimeStamp);
                nodeTimeStamp = rowData.TimeStamp; 
            }
        });
    }
    else {
        $('#grid_Stamp').datagrid('loadData', myData);
    }
}
function LoadDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').treegrid({
            columns: [[
                    { field: 'Name', title: '仓库类型', width: 150 },                
                    {
                        field: 'Value', title: '库存值', width: 80, align: "right",
                    },
                    {
                        field: 'TimeStamp', title: '盘库时间', width: 130, align: "left",
                    },
                    { field: 'Editor', title: '编辑人', width: 60, align: "left" },
                    { field: 'EditTime', title: '编辑时间', width: 130, align: "left" },
                    {
                        field: 'edit', title: '编辑', width: 100, formatter: function (value, row, index) {
                            var str = "";
                            str = '<a href="#" onclick="editWarehouse(\'' + row.LevelCode + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面"/>编辑</a>';
                            str = str + '<a href="#" onclick="deleteWarehouse(\'' + row.LevelCode + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"/>删除</a>';
                            return str;
                        }
                    }
            ]],
            fit: true,
            toolbar: "#toorBar",
            treeField: "Name",
            rownumbers: true,
            idField:'LevelCode',
            singleSelect: true,
            striped: true,
            data: [],
        });
    }
    else {
        $('#grid_Main').treegrid('loadData', myData);
    }
}

//var mger = Object;
function Query() {
    var mOrganizationID = $('#organizationId').val();
    var beginTime = $('#startTime').datebox('getValue');
    var endTime = $('#endTime').datebox('getValue');
    if (mOrganizationID == "") {
        $.messager.alert('警告', '请选择组织机构');
        return;
    }
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/GetInventoryTime",
        data: "{mOrganizationID:'" + mOrganizationID + "',beginTime:'" + beginTime + "',endTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.length == 0) {
                $('#grid_Stamp').datagrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                LoadTimeStamp("last", m_MsgData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function handleError() {
            $.messager.progress('close');
            $('#grid_Stamp').datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function QueryDetailedInfo(mtimeStamp) {
    var mOrganizationID = $('#organizationId').val();
    //var beginTime = $('#startTime').datebox('getValue');
    //var endTime = $('#endTime').datebox('getValue');
    
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/GetInventoryAll",
        data: "{mOrganizationID:'" + mOrganizationID + "',mtimeStamp:'" + mtimeStamp + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.length == 0) {
                $('#grid_Main').treegrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                LoadDataGrid("last", m_MsgData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function handleError() {
            $.messager.progress('close');
            $('#grid_Main').treegrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
var saveId = '';
var level = '';
var m_parent = '';
var mainId = '';
var m_data = '';
var saveMainId = '';
var firstMainValue = '';
var firstValue = '';
var warehouseId = '';
function editWarehouse(editContrastId) {
    $('#grid_Main').treegrid('select', editContrastId);
    var data = $('#grid_Main').treegrid('getSelected');
    level = $('#grid_Main').treegrid('getLevel', editContrastId);
    if (level==3) {
        m_parent = $('#grid_Main').treegrid('getParent', editContrastId);
        mainId = m_parent.LevelCode;
        saveMainId = m_parent.ItemId;
        firstMainValue = m_parent.Value;
    }   
    $('#productionName').numberbox('setValue', data.Value);
    $('#editTime').datetimebox('setValue', data.TimeStamp);
    saveId = data.ItemId;
    firstValue = data.Value;
    warehouseId = data.Id;
    $('#editHouse').window('open');
}
function save() {
    var saveValue = $('#productionName').numberbox('getValue');
    var saveTimeStamp = $('#editTime').datetimebox('getValue');
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/SaveWarehouse",
        data: "{saveId:'" + saveId + "',saveValue:'" + saveValue + "',saveTimeStamp:'" + saveTimeStamp + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = msg.d;
            if (myData == 1) {
                $.messager.alert('提示', '保存成功！');
                $('#editHouse').window('close');
                QueryDetailedInfo(nodeTimeStamp);
            }
        }
    });
    var mainValue = 0;
    var m_value = 0;
    if (level == 3) {
        m_data = $('#grid_Main').treegrid('getChildren', mainId);
        //var length = m_data.length;
        //for (var i = 0; i < length; i++) {
        //    //if (m_data[i].ItemId != saveId) {
        //        var sumValue = m_data[i].Value;
        //    //sumValue = Number((a + b).toFixed(2));

        //        mainValue = (Number(mainValue) + Number(sumValue)).toFixed(2);
        //    //}         
        //}
        m_value = (Number(firstMainValue) - Number(firstValue) + Number(saveValue)).toFixed(2);
        //m_value = (Number(saveValue) + Number(mainValue)).toFixed(2);
        $.ajax({
            type: "POST",
            url: "CheckWarehouse.aspx/SaveWarehouse",
            data: "{saveId:'" + saveMainId + "',saveValue:'" + m_value + "',saveTimeStamp:'" + saveTimeStamp + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = msg.d;
                if (myData == 1) {
                    //$.messager.alert('提示', '保存成功！');
                    //$('#editHouse').window('close');
                    QueryDetailedInfo(nodeTimeStamp);
                }
            }
        });
    }
}
function deleteWarehouse(deleteContrastId) {

    $('#grid_Main').treegrid('select', deleteContrastId);
    var data = $('#grid_Main').treegrid('getSelected');

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
                        QueryDetailedInfo(nodeTimeStamp);
                    }
                    else {
                        $.messager.alert('提示', '操作失败！');
                        QueryDetailedInfo(nodeTimeStamp);
                    }
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                    //$('#AddandEditor').window('close');
                    QueryDetailedInfo(nodeTimeStamp);
                }
            });
        }
    });
}
function QueryWindow() {
    var mOrganizationID = $('#organizationId').val();
    var beginTime = $('#startTimeWindow').datebox('getValue');
    if (mOrganizationID == "") {
        $.messager.alert('警告', '请选择组织机构');
        return;
    }
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/WindowWarehouseAll",
        data: '{ mOrganizationID: "' + mOrganizationID + '", beginTime: "' + beginTime + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0) {
                $('#grid_MainWindow').treegrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                mLoadDataGrid("last", m_MsgData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function handleError() {
            $('#grid_MainWindow').treegrid('loadData', []);
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
        $('#grid_MainWindow').treegrid({
            columns: [[
                    { field: 'Name', title: '仓库类型', width: 130 },
                    {
                        field: 'Value', title: '库存值', width: 80, align: "left",
                        editor: { type: 'numberbox', options: { precision: 2 } }
                    }
                    //{
                    //    field: 'TimeStamp', title: '启用时间', width: 160, align: "left",
                    //    editor: { type: 'datetimebox' }
                    //},
                    //{ field: 'CurrentInventory', title: '盘库基准数', width: 130 }
            ]],
            fit: true,
            toolbar: "toorBarWindows",
            treeField: "Name",
            idField: "LevelCode",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            onContextMenu: onContextMenu,
            onClickRow: onClickRow,
            data: []
        });
    }
    else {
        $('#grid_MainWindow').treegrid('loadData', myData);
    }
}
function onContextMenu(e, row) {
    e.preventDefault();
    $(this).treegrid('select', row.id);
    $('#MenuId').menu('show', {
        left: e.pageX,
        top: e.pageY
    });
}
var editingId;
function onClickRow(clickRow) {
    if (editingId != undefined) {
        var t = $('#grid_MainWindow');
        t.treegrid('endEdit', editingId);
        editingId = clickRow.id;
        $('#grid_MainWindow').treegrid('beginEdit', editingId);
        return;
    }
    var row = $('#grid_MainWindow').treegrid('getSelected');
    if (row) {
        editingId = row.id
        $('#grid_MainWindow').treegrid('beginEdit', editingId);
    }
}
function endEditing() {
    var t = $('#grid_MainWindow');
    if (editingId == undefined) { return true; }
    if ($("#grid_MainWindow").treegrid('validateRow', editingId)) {
        $("#grid_MainWindow").treegrid('endEdit', editingId);
        editingId = undefined;
        return 'true';
    } else {
        return 'false';
    }
}
function saveSectionType() {
    var mOrganizationID = $('#organizationId').val();
    var mStampTime = $('#startTimeWindow').datebox('getValue');
    $.ajax({
        type: "POST",
        url: "CheckWarehouse.aspx/JudgeTodayStamp",
        data: "{mOrganizationID:'" + mOrganizationID + "',mStampTime:'" + mStampTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var mcolumns = msg.d;
            if (mcolumns > 0) {
                $.messager.alert('提示', '当天盘库内容已存在！');
            }
            else {
                endEditing();
                var windowData = $("#grid_MainWindow").treegrid('getData');
                var mainData = $('#grid_Main').treegrid('getData');
                var s_time = $('#startTimeWindow').datetimebox('getValue');
                var count = windowData.length;
                for (var i = 0; i < count; i++) {
                    var windowLength = windowData[i].children.length
                    var m_windowValue = 0;
                    for (var j = 0; j < windowLength; j++) {
                        var windowValue = windowData[i].children[j].Value;
                        var windowCurrent = windowData[i].children[j].CurrentInventory;
                        var windowId = windowData[i].Id;
                        if (windowValue != '') {
                            m_windowValue = (Number(m_windowValue) + Number(windowValue)).toFixed(2);
                            windowData[i].Value = (Number(m_windowValue)).toFixed(2);
                            windowData[i].children[j].TimeStamp = s_time;
                        }
                        if (windowValue == '') {
                            m_windowValue = (Number(m_windowValue) + Number(windowCurrent)).toFixed(2);
                        }
                    }
                    windowData[i].TimeStamp = s_time;
                }
                var str = JSON.stringify(windowData);
                $.ajax({
                    type: "POST",
                    url: "CheckWarehouse.aspx/AddWarehouse",
                    data: '{json:\'' + str + '\'}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        if (msg.d == "success") {
                            $.messager.alert('提示', '保存成功！');
                        }
                        $('#AddandEditor').window('close');
                    }
                });
            }
        }
    })
}



