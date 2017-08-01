<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryMaterialContrast.aspx.cs" Inherits="InventoryManange.Web.UI_InventoryManange.InventoryMaterialContrast" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
       <title>物料库定义</title>
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

    <script type="text/javascript" src="js/page/InventoryMaterialContrast.js" charset="utf-8""></script>
    <style type="text/css">
        .auto-style1 {
            width: 132px;
        }
    </style>
</head>
<body>
   <div class="easyui-layout" data-options="fit:true,border:false">
       <div id="toolbar_Material">
           <table>
               <tr>
                 <td>物料编码:</td>
                 <td class="auto-style1"><input id="materialID" class="easyui-textbox"/></td>
                 <td>物料对照变量:</td>
                 <td><input id="variableID" class="easyui-textbox"/></td>
                 <td>对照名称:</td>
                 <td>
                 <input id="name" class="easyui-textbox"/>
                 </td>
                 <td>
                 <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="Query()">查询</a> 
                 </td>
                 </tr>
                 <tr>
                 <td >
                      <a id="add" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addFun()">添加</a>
                      </td>
                     <td style="text-align:center;padding:0px" class="auto-style1" >
                         <div class="datagrid-btn-separator" style="padding:1px">
                         <a id="bt_WeightMaterial" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="DialogOpen(true)">过磅物料</a>
                         </div>
                     </td>
               </tr>
           </table>
       </div>
 
        <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:true, split:false">
            <table id="gridMain_Material" class="easyui-datagrid"></table>
        </div>
          
   </div><%--第一个div--%>
    <!--过磅物料-->
    <div id="wd_WeightNYGL" class="easyui-window" title="过磅物料" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,plain:true" style="height:500px;width:780px;margin-bottom:5px;top:90px;left:300px">
    <div class="easyui-layout" data-options="fit:true,border:false">
                 <div id="wd-toolbar" class="easyui-panel" style="padding:5px,5px,5px,5px" data-options="region:'north', border:true, collapsible:true, split:false">
                     <table data-options="border-spacing:0px">
                         <tr>
                             <td style="font-size:12px">开始时间:</td>
                             <td><input id="db_startTime" type="text" class="easyui-datetimebox" style="width:150px;"/></td>
                             <td style="font-size:12px">结束时间:</td>
                             <td><input id="db_endTime" type="text" class="easyui-datetimebox" style="width:150px;"/></td>
                             <td style="font-size:12px">物料名称:</td>
                             <td><input id="tb_materiablName" type="text" class="easyui-textbox"/></td>
                             <td style="text-align:right;padding-right:2px">
					            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="LoadWB_WeightNYGL()" data-options="iconCls:'icon-search',plain:true" >查询</a>
				             </td>
                         </tr>
                       </table>
                   </div>
           <div id="weightNYGLTable" data-options="region:'center', border:true, collapsible:true, split:false" style="padding:5px">
                  <table id="gridWeightNYGL" class="easyui-datagrid"></table>
           </div>
  </div>
        </div>
     

<!-- 弹窗-->
     <div id="AddandEditor" class="easyui-window" title="物料对照表" data-options="draggable:true,modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,plain:true" style="width:450px;height:320px;padding:10px 25px 10px 25px;top:90px;left:30px"> 
     <div id="ccWindow" class="easyui-layout" data-options="fit:true,border:false" >    
            <div id="toorBarWindows" data-options="region:'center'" title="" style="padding:10px;text-align:left">        
               <div>  
                <table style="border-spacing:0px;margin:auto">
                    <tr>
                        <td>物料编码:</td>
                        <td><input id="tb_MaterialID" type="text" class="easyui-textbox" style="width:150px;"/><span style="color:red">*</span></td>
                        <td style="text-align:right"><a id="bt_Weight" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="DialogOpen(false)">过磅物料</a> </td>
                    </tr>
                
                    <tr>
                        <td>物料对照变量:</td>
                        <td><input id="tb_VariableId" type="text" class="easyui-textbox" style="width:150px;"/><span style="color:red">*</span></td>
 
                    </tr>
                    <tr>
                        <td>对照名称:</td>
                        <td><input id="tb_Name" type="text" class="easyui-textbox" style="width:150px;"/><span style="color:red">*</span></td>
                      
                    </tr>
                    <tr>
                        <td>品种:</td>
                        <td><input id="tb_Specs" type="text" class="easyui-textbox" style="width:150px;"/></td>
                    </tr>
                    <tr>
                        <td>对照品种:</td>
                        <td><input id="tb_VariableSpecs" type="text" class="easyui-textbox" style="width:150px;"/></td>
                        
                    </tr>
                    <tr>
                        <td>数据类型:</td>
                        <td><input id="tb_StatisticalType" type="text" class="easyui-textbox" style="width:150px;"/></td>
                    </tr>
                    <tr>
                        <td>编辑时间:</td>
                        <td><input id="tb_EditTime" type="text" class="easyui-datetimebox" style="width:150px;"/><span style="color:red">*</span></td>
               
                    </tr>
                    <tr>
                        <td>备注:</td>
                        <td><input id="tb_Remark" type="text" class="easyui-textbox" style="width:200px;"/></td>
                    </tr>
                </table>    
                   </div>                                                                                        
             </div>
        <div data-options="region:'south'" style="text-align:center;padding:5px;">
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="save()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#AddandEditor').window('close');">关闭</a>
        </div>                          
     </div>
   </div>    

</body>
</html>


