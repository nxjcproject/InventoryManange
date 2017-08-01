
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InventoryManange.Infrastructure.Configuration;
using SqlServerDataAdapter;
using EasyUIJsonParser;
using System.Data.SqlClient;
using System.Transactions;
using System.Text.RegularExpressions;

namespace InventoryManange.Service.InventoryManange
{
    public class InventoryMaterialContrast
    {
        public static DataTable MateriableContrast(string materialID, string variableID, string name)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql;
            SqlParameter[] myParameter = { new SqlParameter("@materialID", materialID), new SqlParameter("@variableID", variableID), new SqlParameter("@name", name) };
            if (materialID == "" && variableID == "" && name == "")
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs
                        ,B.USER_NAME from inventory_MaterialContrast A
                               left join [IndustryEnergy_SH].[dbo].[users] B 
                               on A.[Editor]=B.[USER_ID] order by A.[VariableId] asc,A.[MaterialID] asc";

            }
            else if (materialID == "" && variableID == "" && name != "")
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs
                        ,B.USER_NAME from inventory_MaterialContrast AS A
                               left join [IndustryEnergy_SH].[dbo].[users] AS B 
                               on A.[Editor]=B.[USER_ID] where A.Name=@name order by A.[VariableId] asc,A.[MaterialID] asc";
            }
            else if (materialID == "" && variableID != "" && name == "")
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs,B.USER_NAME from inventory_MaterialContrast A
                               left join [IndustryEnergy_SH].[dbo].[users] B 
                               on A.[Editor]=B.[USER_ID] where VariableID=@variableID order by A.[VariableId] asc,A.[MaterialID] asc";
            }
            else if (materialID != "" && variableID == "" && name == "")
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs,B.USER_NAME from inventory_MaterialContrast A
                               left join [IndustryEnergy_SH].[dbo].[users] B 
                               on A.[Editor]=B.[USER_ID] where MaterialID=@materialID order by A.[VariableId] asc,A.[MaterialID] asc";
            }
            else if (materialID != "" && variableID != "" && name == "")
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs,B.USER_NAME from inventory_MaterialContrast A
                               left join [IndustryEnergy_SH].[dbo].[users] B 
                               on A.[Editor]=B.[USER_ID] where MaterialID=@materialID and VariableID=@variableID order by A.[VariableId] asc,A.[MaterialID] asc";
            }
            else if (materialID == "" && variableID != "" && name != "")
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs,B.USER_NAME from inventory_MaterialContrast A
                               left join [IndustryEnergy_SH].[dbo].[users] B 
                               on A.[Editor]=B.[USER_ID] where VariableID=@variableID and Name=@name order by A.[VariableId] asc,A.[MaterialID] asc";
            }
            else if (materialID != "" && variableID == "" && name != "")
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs,B.USER_NAME from inventory_MaterialContrast A
                               left join [IndustryEnergy_SH].[dbo].[users] B 
                               on A.[Editor]=B.[USER_ID] where MaterialID=@materialID and Name=@name order by A.[VariableId] asc,A.[MaterialID] asc";
            }
            else
            {
                mySql = @"select A.[MaterialID]
                        ,A.[VariableId]
                        ,A.[Name]
                        ,replace(A.VariableSpecs,'\','_') AS VariableSpecs
                        ,A.[StatisticalType]
                        ,A.[Editor]
                        ,A.[EditTime]
                        ,A.[Remark]
                        ,replace(A.Specs,'\','_') AS Specs,B.USER_NAME from inventory_MaterialContrast A
                               left join [IndustryEnergy_SH].[dbo].[users] B 
                               on A.[Editor]=B.[USER_ID] where MaterialID=@materialID and Name=@name and VariableID=@variableID order by A.[VariableId] asc,A.[MaterialID] asc";
            }
            DataTable table = dataFactory.Query(mySql, myParameter);
            return table;

        }

        //materiableID的唯一性 mId为编辑之前的MateriablID
        public static bool IsExistMaterialID(string mMaterialID, string mId, bool isAdd)
        {
            bool r = false;
            string mySql;
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            if (isAdd)
            {
                mySql = @"select MaterialID from inventory_MaterialContrast where MaterialID=@mMaterialID";
            }
            else
            {
                if (mMaterialID == mId)
                {
                    return r;
                }
                else
                {
                    mySql = @"select MaterialID from inventory_MaterialContrast where MaterialID=@mMaterialID";
                }
            }
            SqlParameter[] para = { new SqlParameter("@mMaterialID", mMaterialID), new SqlParameter("@mID", mId) };
            DataTable table = dataFactory.Query(mySql, para);
            if (table.Rows.Count != 0)
            {
                r = true;
            }
            return r;
        }
        //根据用户名获得用户ID
        //public static string GetUserID(string mUserName)
        //{
        //    string connectionString = ConnectionStringFactory.NXJCConnectionString;
        //    ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
        //    string mySql = @"select A.USER_ID FROM [IndustryEnergy_SH].[dbo].[users] AS A WHERE A.USER_NAME=@mUserName";
        //    SqlParameter para = new SqlParameter("@mUserName", mUserName);
        //    DataTable table = dataFactory.Query(mySql, para);
        //    string UserID="";
        //    if (table.Rows.Count != 0)
        //    {
        //       UserID=table.Rows[0]["USER_ID"].ToString();
        //    }
        //    return UserID;
        //}

        //添加信息
        public static int AddMaterialContrast(string mMaterialID, string mVariableId, string mName, string mSpecs, string mVariableSpecs, string mStatisticalType, string mEditTime, string mRemark, string mUserId)
        {
            int dt;
            if (IsExistMaterialID(mMaterialID, "", true))
            {
                dt = 2;//若为2则存在重复的mMaterialID
            }
            else
            {
                string connectionString = ConnectionStringFactory.NXJCConnectionString;
                ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

                string mySql = @"INSERT INTO [dbo].[inventory_MaterialContrast]
                                 ([MaterialID]
                                 ,[VariableId]
                                 ,[Name]
                                 ,[Specs]
                                 ,[VariableSpecs]
                                 ,[StatisticalType]
                                 ,[Editor]
                                 ,[EditTime]
                                 ,[Remark]) VALUES (@mMaterialID,@mVariableId,@mName,@mSpecs,@mVariableSpecs,@mStatisticalType,@mEditor,@mEditTime,@mRemark)";
                SqlParameter[] paras = { new SqlParameter("@mMaterialID",mMaterialID), new SqlParameter("@mVariableId",mVariableId), new SqlParameter("@mName", mName),
                                new SqlParameter("@mSpecs", mSpecs), new SqlParameter("@mVariableSpecs",  mVariableSpecs),new SqlParameter("@mStatisticalType", mStatisticalType),                                  
                                new SqlParameter("@mEditor",mUserId),new SqlParameter("@mEditTime", mEditTime), new SqlParameter("@mRemark", mRemark)};
                dt = factory.ExecuteSQL(mySql, paras);
            }
            return dt;
        }
        //编辑信息
        public static int EditMaterialContrast(string mId, string mMaterialID, string mVariableId, string mName, string mSpecs, string mVariableSpecs, string mStatisticalType, string mEditTime, string mRemark)
        {
            int dt;
            if (IsExistMaterialID(mMaterialID, mId, false))
            {
                dt = 2;
            }
            else
            {
                string connectionString = ConnectionStringFactory.NXJCConnectionString;
                ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
                // string mUserID = GetUserID(mUserName);
                string mySql = @"UPDATE [dbo].[inventory_MaterialContrast] SET
                                  [MaterialID]=@mMaterialID
                                 ,[VariableId]=@mVariableId
                                 ,[Name]=@mName
                                 ,[Specs]=@mSpecs
                                 ,[VariableSpecs]=@mVariableSpecs
                                 ,[StatisticalType]=@mStatisticalType
                                 
                                 ,[EditTime]=@mEditTime
                                 ,[Remark]=@mRemark where [MaterialID]=@mId";
                SqlParameter[] paras = { new SqlParameter("@mMaterialID",mMaterialID), new SqlParameter("@mVariableId",mVariableId), new SqlParameter("@mName", mName),
                                new SqlParameter("@mSpecs", mSpecs), new SqlParameter("@mVariableSpecs",  mVariableSpecs),new SqlParameter("@mStatisticalType", mStatisticalType),                                  
                                new SqlParameter("@mEditTime", mEditTime), new SqlParameter("@mRemark", mRemark),
                                   new SqlParameter("@mId",mId)};
                dt = factory.ExecuteSQL(mySql, paras);
            }
            return dt;

        }
        //删除信息
        public static int DeleteMaterialContrast(string mMaterialID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [dbo].[inventory_MaterialContrast]
                         WHERE [MaterialID] =@mMaterialID";
            SqlParameter para = new SqlParameter("@mMaterialID", mMaterialID);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }
        public static string GetLoginUser(string mUserId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select USER_NAME        
                                    from [IndustryEnergy_SH].[dbo].[users]
                                    where [USER_ID]=@mUserId";
            SqlParameter sqlParameter = new SqlParameter("@mUserId", mUserId);
            DataTable table = dataFactory.Query(mySql, sqlParameter);
            string loginUser = table.Rows[0]["USER_NAME"].ToString();
            return loginUser;
        }
        public static DataTable WB_WeightNYGLTable(string mMaterialName, string startTime, string endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            SqlParameter[] paras = { new SqlParameter("@mMaterialName", mMaterialName), new SqlParameter("@startTime", startTime), new SqlParameter("@endTime", endTime) };
            string mySql;
            if (mMaterialName == "")
            {
                mySql = @"select distinct A.Material,A.MaterialName,replace(A.ggxh,'\','_') AS ggxh,B.VariableId,B.Name,B.VariableSpecs from extern_interface.dbo.WB_WeightNYGL A 
                          left join inventory_MaterialContrast B on A.Material = B.MaterialId where A.weightdate>@startTime and A.weightdate<@endTime";
            }
            else
            {
                mySql = @"select distinct A.Material,A.MaterialName,replace(A.ggxh,'\','_') AS ggxh,B.VariableId,B.Name,B.VariableSpecs from extern_interface.dbo.WB_WeightNYGL A 
                          left join inventory_MaterialContrast B on A.Material = B.MaterialId where A.weightdate>@startTime and A.weightdate<@endTime and A.MaterialName like '%'+@mMaterialName+'%'";
            }
            DataTable table = dataFactory.Query(mySql, paras);
            return table;
        }
        public static string MaterialIdLength()
        {
            string mySql = @"select col_length('inventory_MaterialContrast','materialID')";
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            DataTable dt = dataFactory.Query(mySql);
            string materialLength = dt.Rows[0][0].ToString();
            return materialLength;
        }
    }
}
