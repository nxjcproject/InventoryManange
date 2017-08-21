var mOrganizationID = "";
var IsAdd = "";
var mUserId = "";
$(function () {
    loadMaterialDataGrid("first");
    initPageAuthority();
    GetmUserId();
    GetLoginUser();   
});
function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    mOrganizationID = node.OrganizationId;
}
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "MaterialHouseDefinition.aspx/AuthorityControl",
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
function GetmUserId() {
    $.ajax({
        type: "POST",
        url: "MaterialHouseDefinition.aspx/GetmUserId",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            mUserId = msg.d;
        }
    });
}
function loadMaterialDataGrid(type, myData) {
    if (type == "first") {
        $('#gridMain_Material').datagrid({
            columns: [[
                    { field: 'Id', title: '标识列', width: 80, hidden: true },
                    { field: 'Name', title: '物料库名称', width: 100 },                 
                    {
                        field: 'Type', title: '类别', width: 50,
                        formatter: function (value, row) {
                            if (row.Type == 'Virtual') {
                                return "虚拟库";
                            }
                            //if (row.Type == "") {
                            //    return "实物库";
                            //}
                        }
                    },             
                    { field: 'Cubage', title: '容积', width: 100},
                    { field: 'Length', title: '长', width: 50},
                    { field: 'Width', title: '宽', width: 50},
                    { field: 'Height', title: '高', width: 50},                  
                    { field: 'Value', title: '当前库存量', width: 100},
                    { field: 'HighLimit', title: '报警上限', width: 70},
                    { field: 'LowLimit', title: '报警下限', width: 70},
                    {
                        field: 'AlarmEnabled', title: '是否报警', width: 60, align: "center",
                        formatter: function (value, row) {
                            if (row.AlarmEnabled == 'True') {
                                return "是";
                            }
                            if (row.AlarmEnabled == "False") {
                                return "否";
                            }
                        }
                    },
                    { field: 'UserName', title: '编辑人员', width: 80 },
                    { field: 'Remark', title: '备注', width: 100},
                    {
                        field: 'edit', title: '编辑', width: 100, formatter: function (value, row, index) {
                          var str = "";
                          str = '<a href="#" onclick="editFun(true,\'' + row.Id + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png"/>编辑</a>';
                          str = str + '<a href="#" onclick="deleteFun(\'' + row.Id + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png"/>删除</a>';
                          return str;
                       }
                    }
            ]],
            fit: true,
            toolbar: "#toolbar_Material",
            idField: 'Id',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
            //idField: "LevelCode",
            //treeField: "Name"
        })
    }
    else {
        $('#gridMain_Material').datagrid("loadData", myData);
    }
}
function Query() { 
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
        url: "MaterialHouseDefinition.aspx/GetWarehouseInfo",
        data: "{mOrganizationID:'" + mOrganizationID + "',mloginUser:'" + mloginUser + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.length == 0) {
                $('#gridMain_Material').datagrid('loadData', []);
                $.messager.alert('提示', '没有相关数据！');
            }
            else {
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

function addFun() {
    editFun(false);
}
var mId = "";
var mMessage = "";
var mloginUser = "";
function editFun(Isedit, editId) {
    if (Isedit) {
        IsAdd = false;
        $('#gridMain_Material').datagrid('selectRecord', editId);
        var data = $('#gridMain_Material').datagrid('getSelected');  
        $("#wareHouseName").textbox('setText', data.Name);
        $("#materialId").textbox('setText', data.MaterialId)
        $("#type").textbox('setText', data.Type);
        $("#levelCode").textbox('setText', data.LevelCode)
        $("#cubage").textbox('setText', data.Cubage);
        $("#length").textbox('setText', data.Length);
        $("#width").textbox('setText', data.Width);
        $("#height").textbox('setText', data.Height);
        $("#highLimit").textbox('setText', data.HighLimit);
        $("#lowLimit").textbox('setText', data.LowLimit);
        if (data.AlarmEnabled == 'True') {
            mMessage = "1";
        }
        if (data.AlarmEnabled == 'False') {
            mMessage = "0";
        }
        $("#alarmEnable").combobox('setValue', mMessage);
        $("#remark").textbox('setText', data.Remark);
        mId = data.Id;
    }
    else {
        if (mOrganizationID == "") {
            $.messager.alert('警告', '请选择组织机构');
            return;
        }
        else {
            IsAdd = true;
            $("#wareHouseName").textbox('clear');
            $("#materialId").textbox('clear');
            $("#type").textbox('clear');
            $("#levelCode").textbox('clear');
            $("#cubage").textbox('clear');
            $("#length").textbox('clear');
            $("#width").textbox('clear');
            $("#height").textbox('clear');
            $("#highLimit").textbox('clear');
            $("#lowLimit").textbox('clear');
            $("#alarmEnable").combobox('setValue', "0");
            $("#remark").textbox('clear');
        }      
    }
    $('#AddandEditor').window('open');
}
var mcubage = "";
var mlength = "";
var mwidth = "";
var mheight = "";
var mhighLimit = "";
var mlowLimit = "";
function save() {
    var mwareHouseName = $("#wareHouseName").textbox('getText');
    var mmaterialId = $("#materialId").textbox('getText');
    var mtype = $("#type").textbox('getText');
    var mlevelCode = $("#levelCode").textbox('getText');
    var mcubage = $("#cubage").numberbox('getValue');
    var mlength = $("#length").numberbox('getValue');
    var mwidth = $("#width").numberbox('getValue');
    var mheight = $("#height").numberbox('getValue');
    var mhighLimit = $("#highLimit").numberbox('getValue');
    var mlowLimit = $("#lowLimit").numberbox('getValue');
    if (mcubage == "") {
        mcubage = 0;
    }
    if (mlength == "") {
        mlength = 0;
    }
    if (mwidth == "") {
        mwidth = 0;
    }
    if (mheight == "") {
        mheight = 0;
    }
    if (mhighLimit == "") {
        mhighLimit = 0;
    }
    if (mlowLimit == "") {
        mlowLimit = 0;
    }
    var malarmEnable = $("#alarmEnable").combobox('getValue');
    var mremark = $("#remark").textbox('getText');  
    if (mwareHouseName == "" || mmaterialId == "" || mtype == "" || mlevelCode == "") {
        $.messager.alert('提示', '请填写必填项！')
    }
    else {
        var murl = "";
        var mdata = "";
        if (IsAdd) {
            murl = "MaterialHouseDefinition.aspx/AddWarehouseInfo";
            mdata = "{mWareHouseName:'" + mwareHouseName + "',mMaterialId:'" + mmaterialId + "',mType:'" + mtype + "',mLevelCode:'" + mlevelCode + "',mCubage:'" + mcubage + "',mLength:'" + mlength + "',mWidth:'" + mwidth + "',mHeight:'" + mheight + "',mHighLimit:'" + mhighLimit + "',mLowLimit:'" + mlowLimit + "',mUserId:'" + mUserId + "',mAlarmEnable:'" + malarmEnable + "',mRemark:'" + mremark + "',mOrganizationID:'" + mOrganizationID + "'}";
        }
        else {
            murl = "MaterialHouseDefinition.aspx/EditWarehouseInfo";
            mdata = "{mId:'" + mId + "',mWareHouseName:'" + mwareHouseName + "',mMaterialId:'" + mmaterialId + "',mType:'" + mtype + "',mLevelCode:'" + mlevelCode + "',mCubage:'" + mcubage + "',mLength:'" + mlength + "',mWidth:'" + mwidth + "',mHeight:'" + mheight + "',mHighLimit:'" + mhighLimit + "',mLowLimit:'" + mlowLimit + "',mUserId:'" + mUserId + "',mAlarmEnable:'" + malarmEnable + "',mRemark:'" + mremark + "',mOrganizationID:'" + mOrganizationID + "'}";
        }
        $.ajax({
            type: "POST",
            url: murl,
            data: mdata,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = msg.d;
                if (myData == 1) {
                    $.messager.alert('提示', '操作成功！');
                    $('#AddandEditor').window('close');
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
function refresh() {
    Query();
}
function deleteFun(deleteId) {
    $.messager.confirm('提示', '确定要删除吗？', function (r) {
        if (r) {
            $('#gridMain_Material').datagrid('selectRecord', deleteId);
            var data = $('#gridMain_Material').datagrid('getSelected');
            mId = data.Id;
            $.ajax({
                type: "POST",
                url: "MaterialHouseDefinition.aspx/DeleteWarehouseInfo",
                data: "{mId:'" + mId + "'}",
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
function GetLoginUser() {
    $.ajax({
        type: "POST",
        url: "MaterialHouseDefinition.aspx/GetLoginUser",
        data: "{mUserId:'" + mUserId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mloginUser = msg.d;
        }
    });
}