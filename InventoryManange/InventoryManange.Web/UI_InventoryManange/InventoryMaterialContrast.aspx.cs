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
    public partial class InventoryMaterialContrast : WebStyleBaseForEnergy.webStyleBase
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
                mPageOpPermission = "1111";
#elif RELEASE
#endif
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
        //获取当前登陆用户User_ID
        public static string GetmUserId()
        {
            return mUserId;
        }
        [WebMethod]
        public static string GetMaterialContrast(string materialID, string variableID, string name)
        {
            DataTable table = InventoryManange.Service.InventoryManange.InventoryMaterialContrast.MateriableContrast(materialID, variableID, name);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }

        //添加
        [WebMethod]
        public static int AddMaterialContrastInfo(string mMaterialID, string mVariableId, string mName, string mSpecs, string mVariableSpecs, string mStatisticalType, string mEditTime, string mRemark, string mUserId)
        {
            int result = InventoryManange.Service.InventoryManange.InventoryMaterialContrast.AddMaterialContrast(mMaterialID, mVariableId, mName, mSpecs, mVariableSpecs, mStatisticalType, mEditTime, mRemark, mUserId);
            return result;
        }
        //编辑
        [WebMethod]
        public static int EditMaterialContrastInfo(string mId, string mMaterialID, string mVariableId, string mName, string mSpecs, string mVariableSpecs, string mStatisticalType, string mEditTime, string mRemark)
        {
            int result = InventoryManange.Service.InventoryManange.InventoryMaterialContrast.EditMaterialContrast(mId, mMaterialID, mVariableId, mName, mSpecs, mVariableSpecs, mStatisticalType, mEditTime, mRemark);
            return result;
        }
        //删除
        [WebMethod]
        public static int DeleteMaterialContrastInfo(string mMaterialID)
        {
            int result = InventoryManange.Service.InventoryManange.InventoryMaterialContrast.DeleteMaterialContrast(mMaterialID);
            return result;
        }
        [WebMethod]
        public static string getWeightTable(string mMaterialName, string startTime, string endTime)
        {
            DataTable result = InventoryManange.Service.InventoryManange.InventoryMaterialContrast.WB_WeightNYGLTable(mMaterialName, startTime, endTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(result);
            return json;

        }
        [WebMethod]
        public static string GetMaterialIdLength()
        {
            string result = InventoryManange.Service.InventoryManange.InventoryMaterialContrast.MaterialIdLength();
            return result;
        }
    }
}