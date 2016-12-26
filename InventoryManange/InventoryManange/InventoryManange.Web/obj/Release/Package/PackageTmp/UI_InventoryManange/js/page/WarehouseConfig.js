var mOrganizationId = "";
var IsAdd = true;
var mWarehousingtype = "";
//var mName = "";
//var mWarehousename = "";
var mVariableid = "";
var mSpecies = "";
var mDatabasename = "";
var mDatatablename = "";
var mMultiple = "";
var mOffset = "";
var mEditor = "";
var mRemark = "";
var mItemId = "";//标识符
//var mUserId = "";
var mWarehousename="";
//var mWarehouseid = "";


$(document).ready(function () {
    LoadMainDataGrid("first");
    //LoadWareHouseName("mOrganizationId");
    //LoadEditWareHouseName("mOrganizationId");
    //Query();
});
//$(function () {
//    LoadMainDataGrid("first");
//});
function onOrganisationTreeClick(node) {
    $('#organizationname').textbox('setText', node.text);
    mOrganizationId = node.OrganizationId;
    //LoadStaffInfo(mOrganizationId);
    LoadWareHouseName(mOrganizationId);
    LoadEditWareHouseName(mOrganizationId);
}



function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
               //{ field: 'WarehouseId', title: '库房ID ', width: '10%' },
               { field: 'Name', title: '库房名', width: '10%' },
                { field: 'VariableId', title: '标签名', width: '10%' },
                { field: 'Specs', title: '类型', width: '10%' },
                { field: 'DataBaseName', title: '数据库名', width: '6%' },
                { field: 'DataTableName', title: '数据表名', width: '15%' },
                { field: 'WarehousingType', title: '出入库类型', width: '6%' },
                { field: 'Multiple', title: '乘积因子', width: '6%' },
                { field: 'Offset', title: '偏移量因子', width: '6%' },
                { field: 'Editor', title: '编辑人', width: '6%' },
                { field: 'EditTime', title: '编辑时间', width: '10%' },
                { field: 'Remark', title: '备注', width: '10%' },

                {
                    field: 'edit', title: '编辑', width: '8%', formatter: function (value, row, index) {
                        var str = "";
                        str = '<a href="#" onclick="editWorkingDefine(true,\'' + row.ItemId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editWorkingDefine(true,\'' + row.ItemId + '\')"/>编辑</a>';
                        // str = str + '<a href="#" onclick="deleteWorkingDefine(\'' + row.ItemId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteWorkingDefine(\'' + row.ItemId + '\')"/>删除</a>';
                        //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面" onclick="DeletePageFun(\'' + row.id + '\');"/>删除';
                        return str;
                    }
                }
            ]],
            fit: true,
            toolbar: "#toorBar",
            idField: 'ItemId',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#grid_Main').datagrid('loadData', myData);  //loadData:加载本地数据 旧的数据会被替代
    }
}

function LoadWareHouseName(mOrganizationId) {  //括号内应该为空
    $.ajax({
        type: "POST",
        url: "WarehouseConfigIndication.aspx/GetWareHouseName",
        data: "{mOrganizationId:'"+ mOrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#warehousename').combobox({
                valueField: 'Id',
                textField: 'Name',
                panelHeight: 'auto',
                data: myData.rows,
                onLoadSuccess: function () {
                    var data = $('#warehousename').combobox('getData');
                    $("#warehousename ").combobox('select', data[0].id);
                }
            });
        },
        error: function () {
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '下拉菜单加载失败！');
        }
    });
}

function LoadEditWareHouseName(mOrganizationId) {
    $.ajax({
        type: "POST",
        url: "WarehouseConfigIndication.aspx/GetWareHouseName",
        data: "{mOrganizationId:'" + mOrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#name').combobox({
                valueField: 'Id',
                textField: 'Name',
                panelHeight: 'auto',
                data: myData.rows,
                onLoadSuccess: function () {
                    var data = $('#name').combobox('getData');
                    $("#name ").combobox('select', data[0].Id);

                    //mWarehousename = record.Name;
                }
            });
        },
        error: function () {
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    });
}


function Query() {
    //组织机构ID在函数onOrganisationTreeClick中已经获取
    mVariableid = $('#headvariableid').textbox('getText');
    mWarehousename = $('#warehousename').combobox('getText');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "WarehouseConfigIndication.aspx/GetQueryData",
        data: " {mVariableid:'" + mVariableid + "',mWarehousename:'" + mWarehousename + "',mOrganizationId:'"+mOrganizationId+ "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            //mger.window('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                LoadMainDataGrid("last", []);
                $.messager.alert('提示', "没有查询的数据");
            } else {
                LoadMainDataGrid("last", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function addFun() {     //编辑

    editWorkingDefine(false);

}
function refresh() {     //刷新
    Query();
}
function deleteFun() {     //删除
    deleteItem();
}

function save() {   //窗口的添加的保存

   
    mWarehousingtype = $('#warehousingtype').combobox('getValue');
    //mWarehouseid = $('#warehouseid').textbox('getText');

    mWarehousename = $('#name').combobox('getValue');
    mVariableid = $('#variableid').textbox('getText');
    mSpecies = $('#species').textbox('getText');
    mDatabasename = $('#databasename').textbox('getText');
    mDatatablename = $('#datatablename').textbox('getText');
    mMultiple = $('#multiple').textbox('getText');
    mOffset = $('#offset').textbox('getText');
    //mEditor = $('#editor').textbox('getText');
    mRemark = $('#remark').textbox('getText');
  
    if (mWarehousename == "" || mVariableid == "" )  //|| mCreator == ""
        $.messager.alert('提示', '请选择分厂或填写未填项!');
    else {
        var mUrl = "";
        var mdata = "";
        if (IsAdd == true) {
            mUrl = "WarehouseConfigIndication.aspx/AddContent";
            mdata = "{mWarehousingtype:'" + mWarehousingtype +"',mWarehousename:'" + mWarehousename + "',mVariableid:'" + mVariableid + "',mSpecies:'" + mSpecies + "',mDatabasename:'" + mDatabasename + "',mDatatablename:'" + mDatatablename + "',mMultiple:'" + mMultiple +
            "',mOffset:'" + mOffset + "',mRemark:'" + mRemark + "'}"; // "',mEditor:'" + mEditor +   "',mUserId:'" + mUserId + 
        }

        else {
            mUrl = "WarehouseConfigIndication.aspx/EditContent";
            mdata = "{mWarehousingtype:'" + mWarehousingtype + "',mWarehousename:'" + mWarehousename + "',mVariableid:'" + mVariableid + "',mSpecies:'" + mSpecies + "',mDatabasename:'" + mDatabasename + "',mDatatablename:'" + mDatatablename + "',mMultiple:'" + mMultiple +
           "',mOffset:'" + mOffset + "',mRemark:'" + mRemark + "', mItemId:'" + mItemId + "'}";// + "',mEditor:'" + mEditor     + "',mUserId:'" + mUserId 在remark 之后
        }

        $.ajax({
            type: "POST",
            url: mUrl,
            data: mdata,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = msg.d;
                if (myData == 1) {
                    $.messager.alert('提示', '操作成功！');
                    $('#AddandEditor').window('close');
                    Query();
                }
                else {
                    $.messager.alert('提示', '添加失败！');
                    Query();
                }
            }
        });
    }
}
function editWorkingDefine(IsEdit, editContrastId) {
  
    if (IsEdit) {
        IsAdd = false;
        $('#grid_Main').datagrid('selectRecord', editContrastId);
        var data = $('#grid_Main').datagrid('getSelected');
        //$('#warehouseid').textbox('setText', data.WarehouseId);
        //mWarehouseid = data.WarehouseId;


        $('#name').textbox('setText', data.Name);
        mWarehousename = data.WarehouseId;
        $('#variableid').textbox('setText',data.VariableId);
        mVariableid = data.VariableId;
        $('#species').textbox('setText', data.Specs);
        mSpecies = data.Specs;
        $('#databasename').textbox('setText', data.DataBaseName);
        mDatabasename = data.DataBaseName;
        $('#datatablename').textbox('setText', data.DataTableName);
        mDatatablename = data.DataTableName;
        $('#warehousingtype').combobox('setValue', data.WarehousingType);
        mWarehousingtype = data.WarehousingType;
        $('#multiple').textbox('setText', data.Multiple);
        mMultiple = data.Multiple;
        $('#offset').textbox('setText', data.Offset);
        mOffset = data.Offset;
        //$('#editor').textbox('setText', data.Editor);
        //mEditor = data.Editor;
        $('#remark').textbox('setText', data.Remark);
        mRemark = data.Remark;

        mItemId = data.ItemId;
    }
    else {
        IsAdd = true;
        $('#name').textbox('setValue', '');
        $('#variableid').textbox('setValue', '');
        $('#species').textbox('setValue', '');
        $('#databasename').textbox('setValue', '');
        $('#datatablename').textbox('setValue', '');
        $('#warehousingtype').combobox('setValue', '');
        $('#multiple').textbox('setValue', '1.00');
        $('#offset').textbox('setValue', '0.00');
        //$('#editor').textbox('setValue', '');
        $('#remark').textbox('setValue', '');

    }
    $('#AddandEditor').window('open');
}

function deleteItem() {
    var row = $("#grid_Main").datagrid('getSelected');
    if (row == null) {
        alert('请选中一行数据！');
    }
    else {
        var index = $("#grid_Main").datagrid('getRowIndex', row);
        //$.messager.defaults = { ok: "是", cancel: "否" };
        $.messager.confirm('提示', '确定要删除选中行？', function (r) {
            if (r) {
                $('#grid_Main').datagrid('deleteRow', index);
                deleteWorkingDefine(row['ItemId']);
            }
        });
    }
}

function deleteWorkingDefine(mItemId) {

    $.ajax({
        type: "POST",
        url: "WarehouseConfigIndication.aspx/deleteContent",
        data: "{mItemId:'" + mItemId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (msg) {
            var myData = msg.d;
            if (msg.d == '1') {
                alert("删除成功！");
            }
            else {
                alert("删除失败！");
            }
        }
    });

}
