<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialHouseDefinition.aspx.cs" Inherits="InventoryManange.Web.UI_InventoryManange.MaterialHouseDefinition" %>

<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
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

    <script type="text/javascript" src="js/page/MaterialHouseDefinition.js"charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <!-- 图表开始 -->
        <div id="toolbar_Material" style="display: none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td style="width:60px"> 组织机构：</td>
                                <td>
                                    <input id="organizationName" class="easyui-textbox" style="width: 100px;" readonly="readonly"/>
                                    <input id="organizationId" readonly="readonly" style="display: none;" />
                                </td>                                                                                        
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="Query();">查询</a>
                                </td>               
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>    
                             <tr>                              
                                <td>
                                    <a id="add" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addFun()">添加</a>
                                </td>                                                                                                                        
                            </tr>                       
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:true, split:false">
            <table id="gridMain_Material" class="easyui-datagrid"></table>
        </div>
        <!-- 图表结束 -->
    </div>
    
    
    <div id="AddandEditor" class="easyui-window" title="仓库信息配置" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,plain:true" style="width:450px;height:400px;padding:8px 25px 8px 25px"> 
     <div id="ccWindow" class="easyui-layout"data-options="fit:true,border:false" >    
            <div id="toorBarWindows" data-options="region:'center'" title="" style="padding:10px;">          
                <table>
                    <tr>            
                        <td>物料库名称：</td>
                        <td>
                             <input id="wareHouseName" type="text" class="easyui-textbox" style="width:100px;"/>
                        </td>
                        <td><span style="color:red">*</span></td>                                                                                                
                    </tr>
                    <tr>
                        <td>物料ID：</td>
                        <td>
                             <input id="materialId" type="text" class="easyui-textbox" style="width:100px;"/>
                        </td>
                        <td><span style="color:red">*</span></td>                     
                    </tr>
                    <tr>
                        <td>类别：</td>
                        <td>
                             <input id="type" type="text" class="easyui-textbox" style="width:100px;"/>
                        </td>
                        <td><span style="color:red">*</span></td>
                    </tr>
                    <tr>
                        <td>级别码：</td>
                        <td>
                             <input id="levelCode" type="text" class="easyui-textbox" style="width:100px;"/>
                        </td>
                        <td><span style="color:red">*</span></td>
                    </tr>
                    <tr>
                        <td>容积：</td>
                        <td>
                             <input id="cubage" type="text" class="easyui-numberbox" style="width:100px;"/>
                        </td>              
                    </tr>
                    </table>
                    <table>
                        <tr>
                            <td>长：</td>
                            <td style="width:43px"></td>
                        <td>
                             <input id="length" type="text" class="easyui-numberbox" style="width:100px;"/>
                        </td>                       
                        <td style="width:5px"></td>                 
                        <td>宽：</td>
                        <td>
                             <input id="width" type="text" class="easyui-numberbox" style="width:100px;"/>
                        </td>
                        </tr>
                        <tr>                            
                        <td>高：</td>
                        <td style="width:43px"></td>
                        <td>
                             <input id="height" type="text" class="easyui-numberbox" style="width:100px;"/>
                        </td>                      
                        </tr>
                    </table>                                   
                    <table>
                        <tr>
                            <td>报警上限：</td>
                            <td style="width:7px"></td>                                                            
                            <td>
                                <input id="highLimit" type="text" class="easyui-numberbox" style="width:100px;"/>
                            </td>                          
                        </tr>
                        <tr>
                            <td>报警下限：</td>
                            <td style="width:7px"></td>
                            <td>
                                <input id="lowLimit" type="text" class="easyui-numberbox" style="width:100px;"/>
                            </td>                                                    
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>是否报警：</td>
                            <td style="width:7px"></td>
                            <td>
                                <select class="easyui-combobox" id="alarmEnable" name="delay" style="width:50px" data-options="editable:false,panelHeight: 'auto'">
                                   <option value="1">是</option>
                                   <option value="0">否</option>             
                               </select>                               
                            </td>              
                        </tr>
                    </table>
                    <table>                      
                        <tr>
                            <td>备注：</td>
                                <td style="width:30px"></td>
                                <td>
                                   <input id="remark" type="text" class="easyui-textbox" style="width:100px;"/>
                                </td>              
                        </tr>
                    </table>                                                                                           
         </div>
        <div data-options="region:'south'" style="text-align:center;padding:5px;">
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="save()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#AddandEditor').window('close');">关闭</a>
        </div>                          
     </div>
   </div>    
    <form runat="server"></form>
</body>
</html>
