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
        <div data-options="region:'west',border:false " style="width: 150px;">
            <ucl:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
       
         <div id="toolbar_ReportTemplate" style="display: none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>分厂名称</td>
                                <td style="width: 100px;">
                                    <input id="TextBox_OrganizationText" class="easyui-textbox" data-options="editable:false, readonly:true" style="width: 100px;" />
                                </td>
                                  <td>仓库名称</td>
                                <td style="width: 100px;">
                                    <input id="comb_ProcessType" class="easyui-combobox" style="width: 100px;"data-options="panelHeight:'auto'" />
                                </td>                           
                                <td>&nbsp;</td>
                                  <td>查询日期</td>
                                <td style="width: 100px;">
                                    <input id="dbox_QueryDate" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                                 </td>                            
                                <td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                        onclick="QueryReportFun();">查询</a>
                                </td>
                           <%--     <td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                        onclick="GetCenterControlReportTagInfoFun();">标签查询</a>
                                </td>--%>
                                <td>
                                    <input id="TextBox_OrganizationId" style="width: 10px; visibility: hidden;" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
         </div>
       <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:true, split:false">
            <table id="gridMain_ReportTemplate"></table>
        </div>
       </div>
    
    <%-- <div id="toolbar_formulaDatagrid" style="display: normal;height:35px;">              
                        <table style="padding-left:20px">
                            <tr>      
                                <td style="padding-left:350px; font-size:18px">库  存  信  息  报  表（<input id="reportTime" class="easyui-textbox" required="required"readonly="readonly" style="width:80px"/>）</td>                                                                               
                                <td style="padding-left:32px">
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td> 
                                 <td style="padding-left:40px">
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                </td>                                                              
                            </tr>
                        </table>                                                                         
                 </div>    --%>
     
    <%--<script>
        function onOrganisationTreeClick(myNode) {
            //alert(myNode.text);
            m_organizationId = myNode.OrganizationId;
            $('#TextBox_OrganizationId').attr('value', m_organizationId);  //textbox('setText', myNode.OrganizationId);
            $('#TextBox_OrganizationText').textbox('setText', myNode.text);
            //$('#TextBox_OrganizationType').textbox('setText', myNode.OrganizationType);
            PrcessTypeItem(m_organizationId);
        }
        </script>--%>
    
    <form id="form1" runat="server">
    <div>    
    </div>
    </form>
</body>
</html>
