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
    public partial class MaterialHouseDefinition : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif              
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "MaterialHouseDefinition.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        /// <summary>
        /// 增删改查权限控制
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static char[] AuthorityControl()
        {
            return mPageOpPermission.ToArray();
        }
        [WebMethod]
        public static string GetmUserId()
        {
            return mUserId;
        }
        [WebMethod]
        public static string GetWarehouseInfo(string mOrganizationID, string mloginUser)
        {
            DataTable table = MaterialHouseDefinitionService.GetWarehouseInfoTable(mOrganizationID, mloginUser);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        //public static string GetWarehouseInfo(string mOrganizationID)
        //{
        //    //mUserId
        //    DataTable table = MaterialHouseDefinitionService.GetWarehouseInfoTable(mOrganizationID);
        //    string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
        //    return json;
        //}
        [WebMethod]
        public static string UserId()
        {
            return mUserId;
        }       
        [WebMethod]
        public static int AddWarehouseInfo(string mWareHouseName, string mMaterialId, string mType, string mLevelCode, string mCubage, string mLength, string mWidth, string mHeight, string mHighLimit, string mLowLimit, string mUserId, string mAlarmEnable, string mRemark, string mOrganizationID)
        {
            int result = MaterialHouseDefinitionService.AddWarehouseInfomation(mWareHouseName, mMaterialId, mType, mLevelCode, mCubage, mLength, mWidth, mHeight, mHighLimit, mLowLimit, mUserId, mAlarmEnable, mRemark, mOrganizationID);
            return result;
        }
        [WebMethod]
        public static int EditWarehouseInfo(string mId, string mWareHouseName, string mMaterialId, string mType, string mLevelCode, string mCubage, string mLength, string mWidth, string mHeight, string mHighLimit, string mLowLimit, string mUserId, string mAlarmEnable, string mRemark, string mOrganizationID)
        {
            int result = MaterialHouseDefinitionService.EditWarehouseInfomation(mId, mWareHouseName, mMaterialId, mType, mLevelCode, mCubage, mLength, mWidth, mHeight, mHighLimit, mLowLimit, mUserId, mAlarmEnable, mRemark, mOrganizationID);
            return result;
        }
        [WebMethod]
        public static int DeleteWarehouseInfo(string mId)
        {
            int result = MaterialHouseDefinitionService.DeleteWarehouseInfomation(mId);
            return result;
        }
        [WebMethod]
        public static string GetLoginUser(string mUserId)
        {
            string result = MaterialHouseDefinitionService.GetLoginUser(mUserId);
            return result;
        }
    }
}