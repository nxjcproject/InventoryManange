<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryQuery.aspx.cs" Inherits="InventoryManange.Web.UI_InventoryManange.InventoryQuery" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="ucl" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>库存信息查询</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/editCell.js" charset="utf-8"></script>

    <script type="text/javascript" src="js/page/InventoryQuery.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 180px;">
            <ucl:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <!-- 图表开始 -->
        <div data-options="region:'center',border:false">
            <div id="toolBar_ReportTemplate" style ="display: none;">
                <table>
                    <tr>
                        <td style ="height:25px;">分厂名称</td>
                        <td style="width: 100px;">
                            <input id="TextBox_OrganizationText" class="easyui-textbox" data-options="editable:false, readonly:true" style="width: 100px;" />
                        </td>
                        <td>&nbsp;</td>
                        <td>基准时间</td>
                        <td style="width: 100px;">
                            <input id="sbox_QueryStartDate" type="text" class="easyui-searchbox" style="width: 150px;" />
                        </td>
                        <td>盘库时间</td>
                        <td style="width: 100px;">
                            <input id="sbox_QueryEndDate" type="text" class="easyui-searchbox" style="width: 150px;" />
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                onclick="QueryReportFun();">查询</a>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="gridMain_ReportTemplate" class="easyui-treegrid" data-options="idField:'id',treeField:'WarehouseName',rownumbers:true,singleSelect:true,fit:true"></table>
        </div>
        <!-- 图表结束 -->
    </div>
    <%--   modal定义窗口是不是模态（modal）窗口。minimizable定义是否显示最大化按钮。collapsible定义是否显示折叠按钮。resizable定义窗口是否可调整尺寸--%>
    <div id="TimeSelector" class="easyui-window" title="基准时间" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width: 510px; height: 300px;">
        <div id="toolBar_DateTimeSelector" style ="display: none;">
            <table>
                <tr>
                    <td>开始时间</td>
                    <td>
                        <input id="startTimeF" type="text" class="easyui-datetimebox" style="width: 150px;" required="required" />
                    </td>
                    <td>结束时间</td>
                    <td>
                        <input id="EndTimeF" type="text" class="easyui-datetimebox" style="width: 150px;" required="required" />
                    </td>
                    <td>
                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="SearchCheckInventoryTimeFun()">查询</a>
                    </td>
                </tr>
            </table>
        </div>
        <table id="grid_DateTimeSelector" class="easyui-datagrid"></table>
    </div>
    <form id="form_Main" runat="server"></form>
</body>
</html>
