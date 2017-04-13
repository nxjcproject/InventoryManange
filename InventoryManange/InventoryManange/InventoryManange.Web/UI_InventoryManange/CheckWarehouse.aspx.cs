using InventoryManange.Service.InventoryManange;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InventoryManange.Web.UI_InventoryManange
{
    public partial class CheckWarehouse : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx_tys", "zc_nxjc_klqc_klqf", "zc_nxjc_znc_znf"
                ,"zc_nxjc_ychc_yfcf","zc_nxjc_tsc_tsf","zc_nxjc_qtx_efc","zc_nxjc_ychc_ndf","zc_nxjc_szsc_szsf","zc_nxjc_ychc_lsf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "StaffAssessmentRanking.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
            //string userId=mUserId;
            //string userName = mUserName;
            //string time =DateTime.Now.ToString();            
        }
        [WebMethod]
        public static string GetInventoryName(string myOrganizationId)
        {
            //mUserId
            DataTable table = CheckWarehouseService.GetInventoryNameTable(myOrganizationId);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table,"LevelCode");
            return json;
        }
        //[WebMethod]
        //public static string GetInventory(string mOrganizationID,string beginTime, string endTime, string wareHouseId)
        //{
        //    DataTable table = CheckWarehouseService.InventoryWarehouseDataTable(mOrganizationID,beginTime, endTime, wareHouseId);
        //    string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
        //    return json;
        //}
        [WebMethod]
        public static string GetInventoryAll(string mOrganizationID, string beginTime)
        {
            DataTable table = CheckWarehouseService.InventoryWarehouseDataTableAll(mOrganizationID, beginTime);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;
        }
        [WebMethod]
        public static int DeleteWarehouse(string mId)
        {
            int result = CheckWarehouseService.DeleteWarehouseSection(mId);
            return result;
        }
        [WebMethod]
        public static int SaveWarehouse(string saveId,string saveValue,string saveTimeStamp)
        {
            string myName = mUserName;
            int result = CheckWarehouseService.SaveWarehouseSection(saveId,saveValue,saveTimeStamp,myName);
            return result;
        }
        //[WebMethod]
        //public static int AddWarehouse(string windowId, string windowValue, string windowTimeStamp)
        //{
        //    string myName = mUserName;
        //    int result = CheckWarehouseService.AddWarehouseSection(windowId, windowValue, windowTimeStamp, myName);
        //    return result;
        //}
        [WebMethod]
        public static string AddWarehouse(string json)
        {
            
            string myName = mUserName;
            CheckWarehouseService.AddWarehouseSection(json,myName);
            return "success";
        }
        //[WebMethod]
        //public static string WindowWarehouse(string mOrganizationID, string beginTime, string endTime, string wareHouseWindowId)
        //{
        //    DataTable table = CheckWarehouseService.WindowWarehouseDataTable(mOrganizationID, beginTime, endTime,wareHouseWindowId);
        //    string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
        //    return json;
        //}
        [WebMethod]
        public static string WindowWarehouseAll(string mOrganizationID, string beginTime)
        {
            DataTable table = CheckWarehouseService.WindowWarehouseDataTableAll(mOrganizationID, beginTime);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;
        }
    }
}