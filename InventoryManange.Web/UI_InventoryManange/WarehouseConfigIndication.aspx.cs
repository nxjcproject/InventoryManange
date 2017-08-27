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
    public partial class WarehouseConfigIndication : WebStyleBaseForEnergy.webStyleBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_tsc", "zc_nxjc_qtx_tys", "zc_nxjc_qtx_efc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                this.OrganisationTree_ProductionLine.LeveDepth =5;
                mPageOpPermission = "1111";
#elif RELEASE
#endif
                 this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization"); //向web用户控件传递数据授权参数
                 this.OrganisationTree_ProductionLine.PageName = "WarehouseConfigIndication.aspx";   //向web用户控件传递当前调用的页面名称
                 this.OrganisationTree_ProductionLine.LeveDepth = 5;               
            }

        }
        [WebMethod]
        public static char[] AuthorityControl()
        {
            return mPageOpPermission.ToArray();
        }
        [WebMethod]
        public static string GetWareHouseName(string mOrganizationId)
        {
            DataTable table = WarehouseConfigService.GetWarehousenameList(mOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }      
        [WebMethod]
        public static string GetQueryData(string mVariableid, string mWarehousename, string mOrganizationId)
        {
            DataTable table = WarehouseConfigService.GetQueryDataTable(mVariableid, mWarehousename, mOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static int AddContent(string mWarehousingtype, string mWarehousenameId, string mVariableid, string mSpecies, string mDatabasename, string mDatatablename, string mMultiple, string mOffset, string mRemark) 
        {
            int result = WarehouseConfigService.InsertWarehousing(mWarehousingtype, mWarehousenameId, mVariableid, mSpecies, mDatabasename, mDatatablename, mMultiple, mOffset, mUserId, mRemark);
            return result;
        }

        [WebMethod]
        public static int EditContent(string mWarehousingtype, string mWarehousenameId, string mVariableid, string mSpecies, string mDatabasename, string mDatatablename, string mMultiple, string mOffset, string mRemark, string mItemId) 
        {
            int result = WarehouseConfigService.EditWarehousing(mWarehousingtype, mWarehousenameId, mVariableid, mSpecies, mDatabasename, mDatatablename, mMultiple, mOffset, mUserId, mRemark, mItemId);
            return result;
        }
        [WebMethod]
        public static int deleteContent(string mItemId)
        {
            int result = WarehouseConfigService.deleteWarehousing(mItemId);
            return result;
        }
    }
   
}