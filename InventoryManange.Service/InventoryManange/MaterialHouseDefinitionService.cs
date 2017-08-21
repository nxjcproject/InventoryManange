using System;
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
    public class MaterialHouseDefinitionService
    {
        public static DataTable GetWarehouseInfoTable(string morganizationId, string mloginUser)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.*
                                   ,B.USER_NAME
                                   ,(case when B.USER_NAME is null then @mloginUser else B.USER_NAME end) as UserName
                                   from (select * from [dbo].[inventory_Warehouse] where OrganizationID=@morganizationId and [Enabled]=1) A 
                                   left join
                                   [IndustryEnergy_SH].[dbo].[users] B 
                                   on A.[Editor]=B.[USER_ID]
                                   order by LevelCode";
            SqlParameter[] sqlParameter = {
                                            new SqlParameter("@morganizationId", morganizationId),
                                            new SqlParameter("@mloginUser", mloginUser)
                                          };
            DataTable table = dataFactory.Query(mySql, sqlParameter);
            return table;
        }
        public static int AddWarehouseInfomation(string mWareHouseName, string mMaterialId, string mType, string mLevelCode, string mCubage, string mLength, string mWidth, string mHeight, string mHighLimit, string mLowLimit, string mUserId, string mAlarmEnable, string mRemark, string mOrganizationID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"INSERT INTO [dbo].[inventory_Warehouse]
                                    ([Name]
                                    ,[MaterialId]
                                    ,[Type]
                                    ,[LevelCode]
                                    ,[Cubage]
                                    ,[Length]
                                    ,[Width]
                                    ,[Height]
                                    ,[Value]
                                    ,[HighLimit] 
                                    ,[LowLimit]
                                    ,[AlarmEnabled]
                                    ,[Remark]
                                    ,[OrganizationID]                                                              
                                    ,[CheckInventory]
                                    ,[Editor]
                                    ,[EditTime]
                                    ,[Enabled])
                             VALUES
                                   (@mWareHouseName
                                   ,@mMaterialId
                                   ,@mType
                                   ,@mLevelCode
                                   ,@mCubage
                                   ,@mLength
                                   ,@mWidth
                                   ,@mHeight
                                   ,0
                                   ,@mHighLimit
                                   ,@mLowLimit
                                   ,@mAlarmEnable
                                   ,@mRemark
                                   ,@mOrganizationID
                                   ,1
                                   ,@mUserId
                                   ,getdate()
                                   ,1)";
            SqlParameter[] para = { new SqlParameter("@mWareHouseName",mWareHouseName),
                                    new SqlParameter("@mType",mType),
                                    new SqlParameter("@mCubage", mCubage),
                                    new SqlParameter("@mLength", mLength),
                                    new SqlParameter("@mWidth",  mWidth),
                                    new SqlParameter("@mHeight", mHeight),                                  
                                    new SqlParameter("@mHighLimit",mHighLimit),
                                    new SqlParameter("@mLowLimit", mLowLimit),
                                    new SqlParameter("@mAlarmEnable", mAlarmEnable),
                                    new SqlParameter("@mRemark",  mRemark),
                                    new SqlParameter("@mOrganizationID",  mOrganizationID),
                                    new SqlParameter("@mMaterialId",  mMaterialId),
                                    new SqlParameter("@mUserId",  mUserId),
                                    new SqlParameter("@mLevelCode",  mLevelCode)};
            int dt = factory.ExecuteSQL(mySql, para);
            string msql = @"select [Id] from [dbo].[inventory_Warehouse] 
                                      where Name=@mWareHouseName";
            SqlParameter sqlPara = new SqlParameter("@mWareHouseName", mWareHouseName);
            DataTable table = factory.Query(msql, sqlPara);
            string mcode = table.Rows[0]["Id"].ToString();
            string mSql = @"UPDATE [dbo].[inventory_Warehouse]
                                    SET [Code]=@mcode
                                    where Name=@mWareHousename";
            SqlParameter[] Para = { new SqlParameter("@mcode", mcode), 
                                    new SqlParameter("@mWareHousename", mWareHouseName)
                                  };
            int dt1 = factory.ExecuteSQL(mSql, Para);
            return dt1;
        }
        public static int EditWarehouseInfomation(string mId, string mWareHouseName, string mMaterialId, string mType, string mLevelCode, string mCubage, string mLength, string mWidth, string mHeight, string mHighLimit, string mLowLimit, string mUserId, string mAlarmEnable, string mRemark, string mOrganizationID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"UPDATE [dbo].[inventory_Warehouse]
                                SET  [Name]=@mWareHouseName
                                    ,[MaterialId]=@mMaterialId
                                    ,[Type]=@mType
                                    ,[LevelCode]=@mLevelCode
                                    ,[Cubage]=@mCubage
                                    ,[Length]=@mLength
                                    ,[Width]=@mWidth
                                    ,[Height]=@mHeight                                   
                                    ,[HighLimit]=@mHighLimit
                                    ,[LowLimit]=@mLowLimit
                                    ,[AlarmEnabled]=@mAlarmEnable
                                    ,[Remark]=@mRemark
                                    ,[CheckInventory]=1
                                    ,[Editor]=@mUserId
                                    ,[EditTime]=getdate()
                                    ,[Enabled]=1
                              WHERE  [Id]=@mId";

            SqlParameter[] para = { new SqlParameter("@mId",mId),
                                    new SqlParameter("@mWareHouseName",mWareHouseName),
                                    new SqlParameter("@mMaterialId",mMaterialId),
                                    new SqlParameter("@mType",mType),
                                    new SqlParameter("@mLevelCode",mLevelCode),
                                    new SqlParameter("@mCubage", mCubage),
                                    new SqlParameter("@mLength", mLength),
                                    new SqlParameter("@mWidth",  mWidth),
                                    new SqlParameter("@mHeight", mHeight),                                   
                                    new SqlParameter("@mHighLimit",mHighLimit),
                                    new SqlParameter("@mLowLimit", mLowLimit),
                                    new SqlParameter("@mAlarmEnable", mAlarmEnable),
                                    new SqlParameter("@mUserId",  mUserId),
                                    new SqlParameter("@mRemark",  mRemark)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }
        public static int DeleteWarehouseInfomation(string mId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [dbo].[inventory_Warehouse]
                         WHERE [Id] =@mId";
            SqlParameter para = new SqlParameter("@mId", mId);
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
    }
}
