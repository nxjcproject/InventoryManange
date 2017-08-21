using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.UI.WebControls;
using InventoryManange.Service.InventoryManange;


namespace InventoryManange.Web.UI_InventoryManange
{
    public partial class WeighingDetail : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_ychc_lsf", "zc_nxjc_tsc_tsf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "WeighingDetail.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;

                ///以下是接收js脚本中post过来的参数
                string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
                string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
                string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
                if (m_FunctionName == "ExcelStream")
                {
                    //ExportFile("xls", "导出报表1.xls");
                    string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                    m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                    m_ExportTable = m_ExportTable.Replace("&nbsp", "  ");
                    WeighingDetailService.ExportExcelFile("xls", m_Parameter2 + "过磅明细.xls", m_ExportTable);
                }
            }
        }
        [WebMethod]
        public static string GetWeighingInfo(string mOrganizationId, string mAterialName, string mStartTime, string mEndTime, string mSelectTime)
        {
            DataTable table = WeighingDetailService.GetWeighingInfoTable(mOrganizationId, mAterialName, mStartTime, mEndTime, mSelectTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetWeighingDetail(string mOrganizationId, string mMaterialId, string mStartTime, string mEndTime, string mSelectTime)
        {
            DataTable table = WeighingDetailService.GetWeighingDetailTable(mOrganizationId, mMaterialId, mStartTime, mEndTime, mSelectTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }    
        [WebMethod]
        public static string GetWeighingCountDay(string mOrganizationId, string mdayMaterialId, string mdaystartTime, string mdayendTime, string mSelectTime)
        {
            DataTable table = WeighingDetailService.GetWeighingCountDayTable(mOrganizationId, mdayMaterialId, mdaystartTime, mdayendTime, mSelectTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}