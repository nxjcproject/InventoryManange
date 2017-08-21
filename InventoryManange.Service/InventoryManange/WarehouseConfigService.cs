
using InventoryManange.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using EasyUIJsonParser;
using System.Threading.Tasks;

namespace InventoryManange.Service.InventoryManange
{
    public class WarehouseConfigService
    {
        public static DataTable GetWarehousenameList(string mOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT [Id] 
                            ,[Name] 
                             ,[OrganizationID] 
  FROM [inventory_Warehouse] where OrganizationID=@mOrganizationId order by levelcode asc";

            SqlParameter para = new SqlParameter("@mOrganizationId", mOrganizationId);
            DataTable dt = factory.Query(mySql, para);//
            return dt;
        }








        public static DataTable GetQueryDataTable(string mVariableid, string mWarehousename, string mOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
 
        string   mySql = "";
        if (mVariableid == "" && mWarehousename=="")
            {
       mySql = @"SELECT
      A.[ItemId] 
      ,B.[Name] as Name
      ,A.[WarehouseId]
      ,A.[VariableId]
      ,A.[Specs]
      ,A.[DataBaseName]
      ,A.[DataTableName]
      ,A.[WarehousingType]
      ,A.[Multiple]
      ,A.[Offset]
      ,A.[Editor]
      ,A.[EditTime]
      ,A.[Remark]
  FROM [inventory_WareHousingContrast] A,[inventory_Warehouse] B where A.WarehouseId=B.Id and B.OrganizationID=@mOrganizationId  order by B.LevelCode,A.WarehousingType,A.DataBaseName";
            }
        else if (mVariableid == ""|| mWarehousename == "")
             {
                 mySql = @"SELECT
      A.[ItemId] 
      ,B.[Name] as Name
      ,A.[WarehouseId]
      ,A.[VariableId]
      ,A.[Specs]
      ,A.[DataBaseName]
      ,A.[DataTableName]
      ,A.[WarehousingType]
      ,A.[Multiple]
      ,A.[Offset]
      ,A.[Editor]
      ,A.[EditTime]
      ,A.[Remark]
  FROM [inventory_WareHousingContrast] A,[inventory_Warehouse] B  where A.WarehouseId=B.Id  and B.OrganizationID=@mOrganizationId and  (A.VariableId=@mVariableid or B.Name=@mWarehousename) order by B.LevelCode,A.WarehousingType,A.DataBaseName";
             }
        else if (mVariableid != "" && mWarehousename != "")
        {
       mySql = @"SELECT 
      A.[ItemId]
      ,B.[Name] as Name
      ,A.[WarehouseId]
      ,A.[VariableId]
      ,A.[Specs]
      ,A.[DataBaseName]
      ,A.[DataTableName]
      ,A.[WarehousingType]
      ,A.[Multiple]
      ,A.[Offset]
      ,A.[Editor]
      ,A.[EditTime]
      ,A.[Remark]
  FROM [inventory_WareHousingContrast] A,[inventory_Warehouse] B  where A.WarehouseId=B.Id and A.VariableId=@mVariableid and B.Name=@mWarehousename and B.OrganizationID=@mOrganizationId order by B.LevelCode,A.WarehousingType,A.DataBaseName";
      
      
      }
SqlParameter [] para = {
                           new SqlParameter("@mVariableid", mVariableid),
                           new SqlParameter("@mWarehousename", mWarehousename),
                           new SqlParameter("@mOrganizationId", mOrganizationId)
                       };
            DataTable DT = factory.Query(mySql, para);
            return DT;
        }

        public static int InsertWarehousing(string mWarehousingtype, string mWarehousenameId, string mVariableid, string mSpecies, string mDatabasename, string mDatatablename, string mMultiple,
            string mOffset, string mUserId, string mRemark) //
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"INSERT INTO [inventory_WareHousingContrast]   
                            ([ItemId]
                            ,[WarehouseId]
                            ,[VariableId]
                            ,[Specs]
                            ,[DataBaseName]
                            ,[DataTableName]
                            ,[WarehousingType]
                            ,[Multiple]
                            ,[Offset]
                            ,[Editor]
                            ,[Remark]
                            ,[EditTime]
                                        )
                           
                        VALUES
                        (    @mItemId
                            , @mWarehousenameId
                            , @mVariableid
                            , @mSpecies
                            , @mDatabasename
                            , @mDatatablename
                            , @mWarehousingtype
                            , @mMultiple
                            , @mOffset
                            , @mUserId 
                            , @mRemark
                            , @mEditTime
                         )";
            SqlParameter[] para = { new SqlParameter("@mItemId",System.Guid.NewGuid().ToString()),  ////)  //  
                                    new SqlParameter("@mWarehousenameId", mWarehousenameId),
                                    new SqlParameter("@mVariableid", mVariableid),
                                    new SqlParameter("@mSpecies", mSpecies),
                                    new SqlParameter("@mDatabasename", mDatabasename),
                                    new SqlParameter("@mDatatablename", mDatatablename),
                                    new SqlParameter("@mWarehousingtype", mWarehousingtype),
                                    new SqlParameter("@mMultiple", mMultiple),
                                    new SqlParameter("@mOffset", mOffset),
                                    new SqlParameter("@mUserId",mUserId),
                                    new SqlParameter("@mEditTime", DateTime.Now.ToString()),
                                    new SqlParameter("@mRemark", mRemark)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }

  public static int EditWarehousing(string mWarehousingtype, string mWarehousenameId, string mVariableid, string mSpecies, string mDatabasename, string mDatatablename, string mMultiple,
            string mOffset, string mUserId, string mRemark, string mItemId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"UPDATE [inventory_WareHousingContrast] 
                               SET [WarehouseId]=@mWarehousenameId
                            ,[VariableId]=@mVariableid
                            ,[Specs]=@mSpecies
                            ,[DataBaseName]=@mDatabasename
                            ,[DataTableName]=@mDatatablename
                            ,[WarehousingType]=@mWarehousingtype
                            ,[Multiple]=@mMultiple
                            ,[Offset]=@mOffset
                            ,[Editor]=@mUserId
                            ,[Remark]=@mRemark
                            ,[EditTime]=@mEditTime
                            WHERE [ItemId] =@mItemId  ";
            SqlParameter[] para = {
                                    new SqlParameter("@mWarehousenameId", mWarehousenameId),// 
                                    new SqlParameter("@mVariableid", mVariableid),
                                    new SqlParameter("@mSpecies", mSpecies),
                                    new SqlParameter("@mDatabasename", mDatabasename),
                                    new SqlParameter("@mDatatablename", mDatatablename),
                                    new SqlParameter("@mWarehousingtype", mWarehousingtype),
                                    new SqlParameter("@mMultiple", mMultiple),
                                    new SqlParameter("@mOffset", mOffset),
                                    new SqlParameter("@mUserId",mUserId),
                                    new SqlParameter("@mEditTime", DateTime.Now.ToString()),
                                    new SqlParameter("@mRemark", mRemark), 
                                    new SqlParameter("@mItemId", mItemId)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;

        }
        public static int deleteWarehousing(string mItemId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [inventory_WareHousingContrast]
                         WHERE ItemId =@mItemId";
            SqlParameter para = new SqlParameter("@mItemId", mItemId);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }





    }
}
