using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using InventoryManange.Service.InventoryManange;
using System.Data;
namespace InventoryManange.Web.UI_InventoryManange
{
    public partial class InventoryQuery : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_tsc_tsf", "zc_nxjc_znc_znf", "zc_nxjc_ychc_lsf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "0000";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "CenterControlRecord.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        [WebMethod]
        public static string GetWarehouseName(string myOrganizationId)
        {
            DataTable table = InventoryQueryService.GetProcessTypeInfo(myOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetInventoryHouseTime(string organizationID,string startTimeWindow,string endTimeWindow)
        {
            DataTable table = InventoryQueryService.GetInventoryTime(organizationID,startTimeWindow, endTimeWindow);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetInventory(string organizationID, string warehouseName, DateTime startTime,DateTime endTime)
        {
            DataTable table = InventoryQueryService.GetInventoryInformation(organizationID, warehouseName, startTime,endTime);
            return EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "FormulaLevelCode");
        }

    }
}