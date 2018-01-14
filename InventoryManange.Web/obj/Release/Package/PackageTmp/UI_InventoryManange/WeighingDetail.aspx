<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WeighingDetail.aspx.cs" Inherits="InventoryManange.Web.UI_InventoryManange.WeighingDetail" %>
<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>过磅明细</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <script type="text/javascript" src="js/page/WeighingDetail.js"charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <%--图表开始--%>
        <div id="toolbar_Weighing" style="display:none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td style="width:60px; text-align: right;">组织机构</td>
                                <td>
                                    <input id="organizationName" class="easyui-textbox" style="width: 100px;" readonly="readonly"/>
                                    <input id="organizationId" readonly="readonly" style="display: none;"/>
                                </td>                                                                                        
                                <td style="width:60px; text-align: right;">开始时间</td>
                                <td>
                                    <input id="startTime" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                                </td>
                                <td style="width:60px; text-align: right;">查询时间</td>
                                <td>
                                    <select class="easyui-combobox" id="selectTime" name="delay" style="width:130px" data-options="editable:false,panelHeight:'auto'">
                                       <option value="firstWeight">第一次过磅时间</option>
                                       <option value="endWeight">最后一次过磅时间</option>             
                                    </select>                          
                                </td>
                                <td>
                                    <a id="selectBtn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query()">查询</a>
                                </td>           
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>    
                             <tr>                              
                                <td style="width:60px; text-align: right;">物料名称</td>
                                <td>
                                    <input id="MaterialName" class="easyui-textbox" style="width: 100px;"/>
                                </td>
                                <td style="width:60px; text-align: right;">结束时间</td>
                                <td>
                                    <input id="endTime" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                                </td>
                                <td style="width:194px;"></td>
                                <td>
                                    <a id="exportedFile" href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun()">导出</a>
                                </td>                                                                                                                                        
                            </tr>                       
                        </table>
                    </td>
                </tr>
            </table>          
        </div>
        <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false">
            <table id="gridMain_Weighing" class="easyui-datagrid"></table>
        </div>
        <%--图表结束--%>
    </div>
    <%--过磅明细--%>
    <div id="WeighingWindow" class="easyui-window" title="过磅明细" data-options="modal:true,closed:true,iconCls:'icon-save',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width:1000px;height:400px;padding:5px 5px 5px 5px"> 
      <div class="easyui-layout" data-options="fit:true,border:false" ">              
          <div id="weighTable" class="easyui-panel" data-options="fit:true,border:false, collapsible:false, split:false">
            <table id="grid_WeighingDetail" class="easyui-datagrid"></table>
          </div>          
      </div>
        <div style="position: absolute;top:1px; left: 850px;" >
            <a id="exportedInfo" href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileDetail()">导出</a>
        </div>             
   </div>
    <%--日统计--%>
    <div id="WeighingCountday" class="easyui-window" title="日统计" data-options="modal:true,closed:true,iconCls:'icon-save',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width:810px;height:400px;padding:10px 10px 10px 10px"> 
      <div id="Weighingday" class="easyui-layout"data-options="fit:true,border:false" ">    
           <div id="toorBarCountday" data-options="region:'north'" title="" style="height:45px;padding:5px 0px 0px 0px;">
            <div>
                <table>
                    <tr>                    
                        <td style="width:60px; text-align: right;">开始时间</td>
                        <td>
                             <input id="StartTimeWindow" type="text" class="easyui-datetimebox" style="width:150px;"/>
                        </td>    
                        <td style="width:60px; text-align: right;">结束时间</td>
                         <td>
                             <input id="EndTimeWindow" type="text" class="easyui-datetimebox" style="width:150px;"/>
                        </td>            
                        <td>
                            <a id="SearchBtn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="SearchWeightday()">查询</a>
                        </td>
                        <td style="width:100px"></td>
                        <td>
                            <a id="exportedBtn" href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFunDay()">导出</a>
                        </td>  
                    </tr>
                </table>         
            </div>
	    </div> 
        <div data-options="region:'center'" style="padding:5px;">
            <table id="grid_WeighingCountday" class="easyui-datagrid"></table>
        </div>                               
     </div>
   </div>
    <form id="form1" runat="server"></form>
</body>
</html>    