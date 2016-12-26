<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WarehouseConfigIndication.aspx.cs" Inherits="InventoryManange.Web.UI_InventoryManange.WarehouseConfigIndication" %>
<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>出入库配置</title>
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

    <script src="js/page/WarehouseConfig.js"></script>
   
</head>
<body>
   <div id="cc" class="easyui-layout"data-options="fit:true,border:false" >    
         <div data-options="region:'west',split:true" style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
          <div id="toorBar" title="" style="height:60px;padding:10px;">
    <div>
         <table>
                    <tr>
                        <td>组织机构:</td>
<%--                        <td><input id="productLineName" class="easyui-textbox" style="width: 80px;" readonly="readonly" /></td>--%>
                        <td >                               
                            <input id="organizationname" class="easyui-textbox" readonly="readonly"style="width:80px" />（*必选） </td>              
                       

                         <td>标签名:</td>
                        <td >                               
                            <input id="headvariableid"  class="easyui-textbox"  style="width:100px" data-options="panelHeight: 'auto'"/ />（*选填）</td>                

                         
                  </tr>

                </table> 
                <table>
                    <tr>
                        <td>仓库名:</td> 
                        <td ><input id="warehousename" class="easyui-combobox" editable="false" style="width:100px" />（*选填） </td>                
                                      
                           <td>
                            <a "javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query()">查询</a>
                        </td>
                        <td style="width:20px"></td>
                       <td>
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addFun()">添加</a>
                        </td>
                        <td>
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="deleteFun()">删除</a>
                        </td>
                        <td>
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="refresh()">刷新</a>
                        </td>
                    </tr>
                </table>         
            </div>
	    </div> 
         <div data-options="region:'center'" style="padding:5px;background:#eee;">
            
              <table id="grid_Main"class="easyui-datagrid"></table>
         
         </div>

           <!-- 编辑窗口开始 -->
            <div id="AddandEditor" class="easyui-window" title="录入信息" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width:420px;height:auto;padding:10px 60px 20px 60px">
	    	    
                <table>

<%--                    <tr>
	    			    <td>仓库房ID：</td>
	    			    <td>
                          <input class="easyui-combobox" id="warehouseid"  style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>--%>



                  <tr>
	    			    <td>仓库名：</td>
	    			    <td>
                          <input class="easyui-combobox" id="name"  style="width:160px"editable="false" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>

                    <tr>
	    			    <td>标签名：</td>
	    			    <td>
                          <input class="easyui-textbox" id="variableid"  style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>

                    <tr>
	    			    <td>类型：</td>
	    			    <td>
                          <input class="easyui-textbox" id="species"  style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>


                    <tr>
	    			    <td>数据库名：</td>
	    			    <td>
                          <input class="easyui-textbox" id="databasename"  style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>


                    <tr>
	    			    <td>数据表名：</td>
	    			    <td>
                          <input class="easyui-textbox" id="datatablename"  style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>

                    <tr>
	    			<td>出入库类型：</td>
	    			<td> 
                         <select id="warehousingtype" class="easyui-combobox" editable="false"  style="width:120px"     >
                                        <option value="Output">出库</option>
                                        <option value="Input">入库</option>
                                       <%-- <option value="年">年</option>--%>
                                       
                    </select>
                    </td>
	    			 </tr>

                    <tr>
	    			    <td>乘积因子：</td>
	    			    <td>
                          <input class="easyui-textbox" id="multiple"  style="width:160px" data-options="panelHeight: 'auto'"/>
                       </td>
	    		  </tr>

                    <tr>
	    			    <td>偏移量因子：</td>
	    			    <td>
                          <input class="easyui-textbox" id="offset"  style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>

                   <%-- <tr>
	    			    <td>编辑人：</td>
	    			    <td>
                          <input class="easyui-textbox" id="editor"  style="width:160px" data-options="panelHeight: 'auto'"/>           
	    			    </td>
	    		  </tr>--%>
                  
                   <%-- <tr>--%>
	    			   <%-- <td>创建人：</td> 
	    			    <td><input class="easyui-textbox" type="text" id="creator" style="width:120px" />(*必填)</td>
	    		    </tr>--%>
	    		    
                     <%--<tr>
	    			    <td>创建时间：</td> 
	    			    <td><input class="easyui-textbox" type="text" id="creattime" style="width:120px" /></td>
	    		    </tr>    --%>   
                     <tr>
	    			    <td>备注：</td> 
	    			    <td>
                          <input id="remark" class="easyui-textbox" data-options="multiline:true" style="width:160px;height:50px"/>
	    			    </td>
	    		    </tr>
	    	    </table>
	            <div style="text-align:center;padding:5px;margin-left:-18px;">
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="save()">保存</a>
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#AddandEditor').window('close');">取消</a>
	            </div>
            </div>
          
    </div>

   
</body>
</html>
