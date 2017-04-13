<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryQuery.aspx.cs" Inherits="InventoryManange.Web.UI_InventoryManange.InventoryQuery" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="ucl" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
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
            <div class="easyui-layout" data-options="fit:true,border:false">
               <div  class="easyui-panel queryPanel" data-options="region:'north', border:true, collapsible:false, split:false" style="height: 45px;">
          <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>分厂名称</td>
                                <td style="width: 100px;">
                                    <input id="TextBox_OrganizationText" class="easyui-textbox" data-options="editable:false, readonly:true" style="width: 100px;" />
                                </td>
                                  <%--<td>仓库名称</td>
                                <td style="width: 100px;">
                                    <input id="comb_ProcessType" class="easyui-combobox" style="width: 100px;"data-options="panelHeight:'auto'" />
                                </td> --%>                          
                                <td>&nbsp;</td>
                                  <td>查询日期</td>
                                <td style="width: 100px;">
                                    <input id="dbox_QueryDate" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                                 </td>                            
                                <td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                        onclick="QueryReportFun();">查询</a>
                                </td>
                                <td>
                                    <input id="TextBox_OrganizationId" style="width: 10px; visibility: hidden;" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center'"title="库存信息">
               <table id="gridMain_ReportTemplate" class="easyui-treegrid" data-options="idField:'id',treeField:'WarehouseName',rownumbers:true,singleSelect:true,fit:true">
                <thead>
                    <tr>
                    <th data-options="field:'WarehouseName',width:160">仓库名称</th>
                    <th data-options="field:'benchmarksTime',width:120,align:'center'">基准时间</th>
                    <th data-options="field:'benchmarksValue',width:80,align:'right'">基准库存</th>
                     <th data-options="field:'InputWarehouse',width:80,align:'right'">入库量</th>
                         <th data-options="field:'OutputWarehouse',width:80,align:'right'">出库量</th>
				        <th data-options="field:'CurrentInventory',width:80,align:'right'">当前库存</th>
                        </tr>
                    </thead>
            </table>
        </div>
                </div>
            </div>
        <!-- 图表结束 -->
    </div>
    <form id="form_Main" runat="server"></form>
</body>
</html>