$(function () {
    initPageAuthority();
    loadMaterialDataGrid("first");
    GetmUserId();
    GetMaterialLength();
});
function addFun() {
    editFun(false);
};
var mUserId = "";//存后台传来的User_ID
var mId = "";//村修改信息的MateriablID
var IsAdd = "";
var pagePermission = "";
var addCode = '';
var deleteCode = '';
var materialLength = '';
var myDate = new Date();
var nowDateString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds();
var beforeDataString = myDate.getFullYear() + '-' + myDate.getMonth() + '-' + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds();
function initDate() {
    $('#db_startTime').datetimebox('setValue', beforeDataString);
    $('#db_endTime').datetimebox('setValue', nowDateString);
}
function GetMaterialLength() {
    $.ajax({
        type: "POST",
        url: "InventoryMaterialContrast.aspx/GetMaterialIdLength",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            materialLength = msg.d
        }
    })
}
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "InventoryMaterialContrast.aspx/AuthorityControl",
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
            if (PageOpPermission[2] == '0') {
                addCode = '0';
            }
            else {
                addCode = '1';
            }

            if (PageOpPermission[3] == '0') {
                deleteCode = '0';
            }
            else {
                deleteCode = '1';
            }
        }
    });
}
//加载添加窗口过磅物料对话框
function loadDialogDataGrid(type, myData, IsMain) {
    if (type == "first") {
        $("#gridWeightNYGL").datagrid({
            columns: [[
                { field: 'Material', title: '物料编码', width: 150 },
                { field: 'MaterialName', title: '物料名称', width: 100 },
                { field: 'ggxh', title: '物料品种', width: 150 },
                { field: 'VariableId', title: '对照变量', width: 80 },
                { field: 'Name', title: '对照名称', width: 100 },
                { field: 'VariableSpecs', title: '对照品种', width: 100 }
            ]],
            fit: true,
            toolbar: "#wd-toolbar",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
            onDblClickRow: function (rowIndex, rowData) {
                if (!isMain) {
                    $('#wd_WeightNYGL').window('close');
                    $('#tb_MaterialID').textbox('setValue', rowData.Material);
                    $('#tb_Specs').textbox('setValue', rowData.ggxh);
                }
            }
        })
    }
    else {
        $("#gridWeightNYGL").datagrid('loadData', myData);
    }
}


var isMain;
function DialogOpen(isMainWeight) {
    isMain = isMainWeight;
    initDate();
    loadDialogDataGrid("first");
    $("#wd_WeightNYGL").dialog('open');
}
function LoadWB_WeightNYGL() {
    mMaterialName = $('#tb_materiablName').textbox('getValue');
    startTime = $('#db_startTime').datetimebox('getValue');
    endTime = $('#db_endTime').datetimebox('getValue');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    var dataToServer = {
        mMaterialName: mMaterialName,
        startTime: startTime,
        endTime: endTime
    }
    $.ajax({
        type: "POST",
        url: "InventoryMaterialContrast.aspx/getWeightTable",
        data: JSON.stringify(dataToServer),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.rows.length == 0) {
                $('#gridDialog').datagrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                for (var i = 0; i < m_MsgData.rows.length; i++) {
                    if (m_MsgData.rows[i].ggxh != '') {
                        m_MsgData.rows[i].ggxh = m_MsgData.rows[i].ggxh.replace(/_/g, "\\");
                        //    m_MsgData.rows[i].VariableSpecs = m_MsgData.rows[i].VariableSpecs.replace(/_/g, "\\");
                    }
                }
                loadDialogDataGrid("last", m_MsgData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function handleError() {
            $.messager.progress('close');
            $('#gridDialog').datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function loadMaterialDataGrid(type, myData) {
    if (type == "first") {
        $('#gridMain_Material').datagrid({
            columns: [[
                { field: 'MaterialID', title: '物料编码', width: 160 },
                { field: 'VariableId', title: '物料对照变量', width: 80 },
                { field: 'Name', title: '对照名称', width: 90 },
                { field: 'Specs', title: '品种', width: 200 },
                { field: 'VariableSpecs', title: '对照品种', width: 100 },
                { field: 'StatisticalType', title: '数据类型', width: 80 },
                { field: 'USER_NAME', title: '编辑人', width: 60 },
                { field: 'EditTime', title: '编辑时间', width: 140 },
                { field: 'Remark', title: '备注', width: 100 },
                {
                    field: 'edit', title: '编辑', width: 100, formatter: function (value, row, index) {
                        var str = "";
                        pagePermission = addCode + deleteCode;
                        if (pagePermission == '01') {
                            str = '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png"/><font color="gray">编辑</font>';
                            str = str + '<a  href="#"  id="delete" class="easyui-linkbutton" onclick="deleteFun(\'' + row.MaterialID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png"/>删除</a>';
                        }
                        else if (pagePermission == '10') {
                            str = '<a href="#" id="editt" class="easyui-linkbutton" onclick="editFun(true,\'' + row.MaterialID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png"/>编辑</a>';
                            str = str + ' <img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png"/><font color="gray">删除</font>';
                        }
                        else if (pagePermission == '00') {
                            str = '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png"/><font color="gray">编辑</font>';
                            str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png"/><font color="gray">删除</font>';
                        }
                        else {
                            str = '<a href="#" id="editt" class="easyui-linkbutton" onclick="editFun(true,\'' + row.MaterialID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png"/>编辑</a>';
                            str = str + '<a  href="#"  id="delete" class="easyui-linkbutton" onclick="deleteFun(\'' + row.MaterialID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png"/>删除</a>';
                        }
                        return str;
                    }
                }
            ]],
            fit: true,
            toolbar: "#toolbar_Material",
            idField: 'MaterialID',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
        })
    }
    else {
        $('#gridMain_Material').datagrid("loadData", myData);

    }
}

function Query() {
    var materialID = $('#materialID').textbox('getValue');
    var variableID = $('#variableID').textbox('getValue');
    var name = $('#name').textbox('getValue');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });

    $.ajax({
        type: "POST",
        url: "InventoryMaterialContrast.aspx/GetMaterialContrast",
        data: "{materialID:'" + materialID + "',variableID:'" + variableID + "',name:'" + name + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.rows.length == 0) {
                $('#gridMain_Material').datagrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
                for (var i = 0; i < m_MsgData.rows.length; i++) {
                    if (m_MsgData.rows[i].Specs != '' || m_MsgData.rows[i].VariableSpecs != '') {
                        m_MsgData.rows[i].Specs = m_MsgData.rows[i].Specs.replace(/_/g, "\\");
                        m_MsgData.rows[i].VariableSpecs = m_MsgData.rows[i].VariableSpecs.replace(/_/g, "\\");
                    }
                }
                loadMaterialDataGrid("last", m_MsgData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function handleError() {
            $.messager.progress('close');
            $('#gridMain_Material').datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}

function save() {

    mMaterialID = $("#tb_MaterialID").textbox('getText');
    mVariableId = $("#tb_VariableId").textbox('getText');
    mName = $("#tb_Name").textbox('getText');
    mSpecs = $("#tb_Specs").textbox('getText');
    mVariableSpecs = $("#tb_VariableSpecs").textbox('getValue');
    mStatisticalType = $("#tb_StatisticalType").textbox('getValue');
    mEditTime = $("#tb_EditTime").datetimebox('getValue');
    mRemark = $("#tb_Remark").textbox('getValue');
    if (mMaterialID == "" || mVariableId == "" || mName == "" || mEditTime == "") {
        $.messager.alert('提示', '请填写必填项！');

    }


    else {
        if (mMaterialID.length != materialLength) {
            $.messager.alert('提示', '物料编码位数有误！');
            return;
        }
        var mUrl = "";
        var mData = "";
        if (IsAdd) {
            mUrl = "InventoryMaterialContrast.aspx/AddMaterialContrastInfo";
            mData = "{mMaterialID:'" + mMaterialID + "',mVariableId:'" + mVariableId + "',mName:'" + mName + "',mSpecs:'" + mSpecs +
                    "',mVariableSpecs:'" + mVariableSpecs + "',mStatisticalType:'" + mStatisticalType +
                    "',mEditTime:'" + mEditTime + "',mRemark:'" + mRemark + "',mUserId:'" + mUserId + "'}";
            //    mData=JSON.stringify(dataToServer);
        }
        else {
            mUrl = "InventoryMaterialContrast.aspx/EditMaterialContrastInfo";
            mData = "{mId:'" + mId + "',mMaterialID:'" + mMaterialID + "',mVariableId:'" + mVariableId + "',mName:'" + mName + "',mSpecs:'" + mSpecs +
                   "',mVariableSpecs:'" + mVariableSpecs + "',mStatisticalType:'" + mStatisticalType +
                  "',mEditTime:'" + mEditTime + "',mRemark:'" + mRemark + "'}";
        }
        $.ajax({
            type: "POST",
            url: mUrl,
            data: mData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = msg.d;
                if (myData == 1) {
                    $('#AddandEditor').window('close');
                    $.messager.alert('提示', '操作成功！');
                    refresh();
                }
                else if (myData == 2) {
                    $.messager.alert('提示', '物料编码已存在，请重新输入！');
                    refresh();
                }
                else {
                    $.messager.alert('提示', '操作失败！');
                    refresh();
                }
            },
            error: function () {
                $.messager.alert('提示', '操作失败！');
                refresh();
            }
        });
    }
}

function editFun(Isedit, editId) {
    if (Isedit) {
        IsAdd = false;
        $('#gridMain_Material').datagrid('selectRecord', editId);//通过传递 id 的值做参数选中一行。用此方法才能将当前编辑行加载出来
        var data = $('#gridMain_Material').datagrid('getSelected');//返回第一个选中的行或者 null
        $("#tb_MaterialID").textbox('setText', data.MaterialID);
        $("#tb_VariableId").textbox('setText', data.VariableId)
        $("#tb_Name").textbox('setText', data.Name);
        $("#tb_Specs").textbox('setText', data.Specs)
        $("#tb_VariableSpecs").textbox('setText', data.VariableSpecs);
        $("#tb_StatisticalType").textbox('setText', data.StatisticalType);
        $("#tb_UserName").textbox('setText', data.USER_NAME);
        $("#tb_EditTime").datetimebox('setValue', data.EditTime);
        $("#tb_Remark").textbox('setText', data.Remark);
        mId = data.MaterialID;
        // $('#bt_Weight').linkbutton('disable');
    }
    else {
        IsAdd = true;
        $("#tb_MaterialID").textbox('clear');
        $("#tb_VariableId").textbox('clear');
        $("#tb_Name").textbox('clear');
        $("#tb_Specs").textbox('clear');
        $("#tb_VariableSpecs").textbox('clear');
        $("#tb_StatisticalType").textbox('clear');
        $("#tb_UserName").textbox('clear');
        $("#tb_EditTime").combobox('clear');
        $("#tb_Remark").textbox('clear');
        $('#AddandEditor').window('open');
        // $('#bt_Weight').linkbutton('enable');
    }
    $('#AddandEditor').window('open');
}


function deleteFun(deleteId) {
    $.messager.confirm('提示', '确定要删除吗？', function (r) {
        if (r) {
            $('#gridMain_Material').datagrid('selectRecord', deleteId);
            var data = $('#gridMain_Material').datagrid('getSelected');
            mId = data.MaterialID;
            $.ajax({
                type: "POST",
                url: "InventoryMaterialContrast.aspx/DeleteMaterialContrastInfo",
                data: "{mMaterialID:'" + mId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData == 1) {
                        $.messager.alert('提示', '删除成功！');
                        refresh();
                    }
                    else {
                        $.messager.alert('提示', '操作失败！');
                        refresh();
                    }
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                    refresh();
                }
            });
        }
    })
}
function refresh() {
    Query();
}
//从后台获取UserID
function GetmUserId() {
    $.ajax({
        type: "POST",
        url: "InventoryMaterialContrast.aspx/GetmUserId",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            mUserId = msg.d;
        }
    });
}
