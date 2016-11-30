<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckWarehouse.aspx.cs" Inherits="InventoryManange.Web.UI_InventoryManange.CheckWarehouse" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>盘库</title>
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

    <script type="text/javascript" src="js/page/CheckWarehouse.js" charset="utf-8"></script>
</head>
<body>
    <div id="cc" class="easyui-layout"data-options="fit:true,border:false" >    
         <div data-options="region:'west',split:true" style="width: 230px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
          <div id="toorBar" title="" style="height:56px;padding:10px;">
            <div>
                <table>
                    <tr>
                        <td>组织机构:</td>
                        <td >                               
                            <input id="organizationName" class="easyui-textbox" readonly="readonly"style="width:80px" />  
                            <input id="organizationId" readonly="readonly" style="display: none;" />             
                        </td>
                        <td>仓库选择</td>
                        <td style="width: 100px;">
                            <input id="comb_ProcessType" class="easyui-combobox" style="width: 130px;"data-options="panelHeight:'auto'" />
                        </td>                                      
                        <td>开始时间：</td>
                        <td>
                             <input id="startTime" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                        </td>
                           <td>结束时间：</td>
                         <td>
                             <input id="endTime" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                        </td>              
                         <td>
                            <a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query()">查询</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addFun()">添加</a>
                        </td>
                    </tr>
                </table>         
            </div>
	    </div> 
         <div data-options="region:'center'" style="padding:5px;background:#eee;">
             <table id="grid_Main"class="easyui-datagrid"></table>
         </div>
        <div id="AddandEditor" class="easyui-window" title="添加盘库" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width:900px;height:400px;padding:10px 60px 20px 60px"> 
    <div id="ccWindow" class="easyui-layout"data-options="fit:true,border:false" >    

            <div id="toorBarWindows" data-options="region:'north'" title="" style="height:56px;padding:10px;">
            <div>
                <table>
                    <tr>
                        <td>仓库选择</td>
                        <td style="width: 100px;">
                            <input id="comb_ProcessTypeWindow" class="easyui-combobox" style="width: 130px;"data-options="panelHeight:'auto'" />
                        </td>                                      
                        <td>开始时间：</td>
                        <td>
                             <input id="startTimeWindow" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                        </td>
                           <td>结束时间：</td>
                         <td>
                             <input id="endTimeWindow" type="text" class="easyui-datetimebox" style="width:150px;" required="required"/>
                        </td>              
                         <td>
                            <a id="btnWindow" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="QueryWindow()">查询</a>
                        </td>
                    </tr>
                </table>         
            </div>
	    </div> 
            <div data-options="region:'center'" style="padding:5px;">
             <table id="grid_MainWindow"class="easyui-datagrid"></table>
         </div>
        <div data-options="region:'south'" style="text-align:center;padding:5px;">
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="saveSectionType()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#AddandEditor').window('close');">关闭</a>
        </div>                          
            </div>
   </div>
                    <div id="editHouse" class="easyui-window" title="编辑盘库" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width:420px;height:auto;padding:10px 60px 20px 60px">
	    	    <table>
                     <tr>
	    			    <td>库存值：</td>
	    			    <td>
                          <input class="easyui-numberbox" id="productionName"  precision:2 style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		    </tr>
                    <tr>
	    			    <td>启用时间：</td> 
	    			    <td><input class="easyui-datetimebox"  id="editTime" style="width:160px" /></td>
	    		    </tr>     
	    	    </table>
	            <div style="text-align:center;padding:5px;margin-left:-18px;">
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="save()">保存</a>
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#editHouse').window('close');">取消</a>
	            </div>
            </div>
    </div>
</body>
</html>