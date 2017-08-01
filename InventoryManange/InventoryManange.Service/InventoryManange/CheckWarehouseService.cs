using InventoryManange.Infrastructure.Configuration;
using EasyUIJsonParser;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Text.RegularExpressions;

namespace InventoryManange.Service.InventoryManange
{
    public class CheckWarehouseService
    {
////////        public static DataTable GetInventoryNameTable(string organizationId)
////////        {
////////            string connectionString = ConnectionStringFactory.NXJCConnectionString;
////////            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
////////            string mySql = @"select Id ,Name as text,[LevelCode]
////////                                       from [dbo].[inventory_Warehouse] 
////////                                       where OrganizationID=@organizationId
////////                                       and CheckInventory=1
////////                                       group by Id,Name,Code,[LevelCode]
////////                                       order by Id";
////////            SqlParameter sqlParameter = new SqlParameter("@organizationId", organizationId);
////////            DataTable table = dataFactory.Query(mySql, sqlParameter);
////////            DataRow newrow;
////////            newrow = table.NewRow();
////////            newrow["Id"] = "All";
////////            newrow["text"] = "全部";
////////            newrow["LevelCode"] = "W00";
////////            table.Rows.InsertAt(newrow, 0);
////////            return table;
////////        }
//        public static DataTable InventoryWarehouseDataTable(string mOrganizationID,string beginTime, string endTime, string wareHouseId)
//        {
//            string connectionString = ConnectionStringFactory.NXJCConnectionString;
//            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
//            string mySql = @"SELECT  A.LevelCode,A.Name,A.Id,A.Type,A.MaterialId
//                          ,B.[Value]
//                          ,CONVERT(varchar(64),B.TimeStamp, 120) as TimeStamp
//                          ,B.[Editor]
//                          ,CONVERT(varchar(64),B.EditTime, 120) as EditTime
//                          ,B.[ItemId]
//                      FROM [dbo].[inventory_Warehouse] A,[dbo].[inventory_CheckWarehouse] B
//                      where WarehouseId=@wareHouseId
//                            and A.Id=@wareHouseId
//                            and A.OrganizationID=@mOrganizationID
//                            and not exists(select 1 from [dbo].[inventory_CheckWarehouse] C 
//					        where C.WarehouseId=B.WarehouseId and C.TimeStamp>B.TimeStamp)
//                    order by [TimeStamp],LevelCode";
//            SqlParameter [] sqlParameters = {
//                                                new SqlParameter("@wareHouseId", wareHouseId),
//                                                new SqlParameter("@mOrganizationID", mOrganizationID)
//                                            };
//            DataTable table = dataFactory.Query(mySql, sqlParameters);
//            return table;
//        }
        public static DataTable GetInventoryTimeTable(string mOrganizationID, string beginTime, string endTime)
        {
            string connectionStr = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionStr);
            string mysql = @"select distinct B.[TimeStamp]
                           from [dbo].[inventory_Warehouse] A,[dbo].[inventory_CheckWarehouse] B
                           where A.[OrganizationID]=@mOrganizationID
                             and A.[Id]=B.[WarehouseId]
                             and B.[TimeStamp]>=@beginTime
                             and B.[TimeStamp]<=@endTime
                             order by B.[TimeStamp] desc";
            SqlParameter[] mysqlpara ={
                                     new SqlParameter("@mOrganizationID",mOrganizationID),
                                     new SqlParameter("@beginTime",beginTime),
                                     new SqlParameter("@endTime",endTime)
                                     };
            DataTable mytable = dataFactory.Query(mysql, mysqlpara);
            return mytable;
        }
        public static DataTable InventoryWarehouseDataTableAll(string mOrganizationID, string timeStamp)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            //            string mySql = @"(SELECT  A.LevelCode as it,A.Name,A.Id,A.Type,A.MaterialId
//                          ,B.[Value]
//                          ,CONVERT(varchar(64),B.TimeStamp, 120) as TimeStamp
//                          ,B.[Editor]
//                          ,CONVERT(varchar(64),B.EditTime, 120) as EditTime
//                          ,B.[ItemId]
//                          ,'' as [LevelCode]
//						  ,'leafnode' as [NodeType]
//                      FROM [dbo].[inventory_Warehouse] A,[dbo].[inventory_CheckWarehouse] B
//                      where WarehouseId=A.Id
//                            and B.TimeStamp>@beginTime
//                            and B.TimeStamp<@endTime
//                            and A.OrganizationID=@mOrganizationID)
//					union all
//                        (SELECT  A.LevelCode as it,A.Name,A.Id,A.Type,A.MaterialId
//                          ,B.[Value]
//                          ,'' as TimeStamp
//                          ,''[Editor]
//                          ,'' as EditTime
//                          ,B.[ItemId]
//                          ,'' as [LevelCode]
//						  ,'node' as [NodeType]
//                      FROM [dbo].[inventory_Warehouse] A,[dbo].[inventory_CheckWarehouse] B
//                      where WarehouseId=A.Id
//                            and B.TimeStamp>@beginTime
//                            and B.TimeStamp<@endTime
//                            and A.OrganizationID=@mOrganizationID
//							group by A.LevelCode,A.Name,A.Id,A.Type,A.MaterialId,B.Value,B.ItemId)
//							order by A.LevelCode";
//            string mySql = @"SELECT  A.LevelCode,A.Name,A.Id,A.Type,A.MaterialId
//                          ,B.[Value]
//                          ,CONVERT(varchar(64),B.TimeStamp, 120) as TimeStamp
//                          ,B.[Editor]
//                          ,CONVERT(varchar(64),B.EditTime, 120) as EditTime
//                          ,B.[ItemId]
//                      FROM [dbo].[inventory_Warehouse] A,[dbo].[inventory_CheckWarehouse] B
//                      where WarehouseId=A.Id
//                            and B.TimeStamp>@beginTime
//                            and B.TimeStamp<@endTime
//                            and A.OrganizationID=@mOrganizationID                    
//                            order by A.LevelCode";
//            string mySql = @"SELECT  A.LevelCode,A.Name,A.Id,A.Type,A.MaterialId
//                          ,B.[Value]
//                          ,CONVERT(varchar(64),B.TimeStamp, 120) as TimeStamp
//                          ,B.[Editor]
//                          ,CONVERT(varchar(64),B.EditTime, 120) as EditTime
//                          ,B.[ItemId]
//                      FROM [dbo].[inventory_Warehouse] A,[dbo].[inventory_CheckWarehouse] B
//                      where 
//					  WarehouseId=A.Id
//                            and A.OrganizationID=@mOrganizationID
//					  and not exists(select 1 from [dbo].[inventory_CheckWarehouse] C 
//					  where C.WarehouseId=B.WarehouseId and C.TimeStamp>B.TimeStamp)					  					  
            //                    order by [TimeStamp],LevelCode";
            string mySql = @"SELECT  A.LevelCode,A.Name,A.Id,A.Type,A.MaterialId
                          ,convert(varchar(64),convert(decimal(18,2),B.Value)) as [Value]
                          ,CONVERT(varchar(64),B.TimeStamp, 120) as TimeStamp
                          ,B.[Editor]
                          ,CONVERT(varchar(64),B.EditTime, 120) as EditTime
                          ,B.[ItemId]
                      FROM [dbo].[inventory_Warehouse] A,[dbo].[inventory_CheckWarehouse] B
                      where WarehouseId=A.Id
                            and B.TimeStamp=@timeStamp
                            and A.OrganizationID=@mOrganizationID           
                            order by A.LevelCode";	
            SqlParameter[] sqlParameters = {
                                                new SqlParameter("@mOrganizationID", mOrganizationID),
                                                new SqlParameter("@timeStamp", timeStamp),                                              
                                            };
            DataTable table = dataFactory.Query(mySql, sqlParameters);
            //int mcode = 0;
            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    string id = table.Rows[i]["NodeType"].ToString();
            //    if (id == "node")
            //    {
            //        string nodeCode = "M" + (++mcode).ToString("00");
            //        table.Rows[i]["LevelCode"] = nodeCode;
            //        int mleafcode = 0;
            //        for (int j = 0; j < table.Rows.Count; j++)
            //        {
            //            if (table.Rows[j]["Name"].ToString().Trim() == table.Rows[i]["Name"].ToString().Trim() && table.Rows[j]["NodeType"].ToString().Equals("leafnode"))
            //            {
            //                table.Rows[j]["LevelCode"] = nodeCode + (++mleafcode).ToString("00");
            //            }
            //        }
            //    }
            //}
            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    string m_id = table.Rows[i]["NodeType"].ToString();
            //    if (m_id == "leafnode")
            //    {
            //        int thirdCode = 0;
            //        for (int j = 0; j < table.Rows.Count; j++)
            //        {
            //            if (table.Rows[j]["TimeStamp"].ToString().Trim() == table.Rows[i]["TimeStamp"].ToString().Trim() && table.Rows[j]["NodeType"].ToString().Equals("thirdnode"))
            //            {
            //                table.Rows[j]["LevelCode"] = table.Rows[i]["LevelCode"].ToString().Trim() + (++thirdCode).ToString("00");
            //            }
            //        }
            //    }
            //}
            return table;
        }
        public static int DeleteWarehouseSection(string mId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [dbo].[inventory_CheckWarehouse]
                         WHERE [ItemId]=@mId";
            SqlParameter para = new SqlParameter("@mId", mId);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }
        public static int JudgeTodayWhetherStamp(string mOrganizationID, string mStampTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.[Id],A.[OrganizationID], 
                                    B.[WarehouseId],B.[TimeStamp]  
                               from [dbo].[inventory_Warehouse] A,
                                    [dbo].[inventory_CheckWarehouse] B                       
                              WHERE A.[Id]=B.[WarehouseId] 
                                and A.[OrganizationID]=@morganizationID      
                                and convert(varchar,[TimeStamp],120) like @time+'%'";
            SqlParameter[] paras = {
                                       new SqlParameter("@morganizationID", mOrganizationID),                                                                             
                                       new SqlParameter("@time", DateTime.Parse(mStampTime).ToString("yyyy-MM-dd"))
                                   };
            DataTable table = factory.Query(mySql, paras);
            int dt = table.Rows.Count;
            return dt;
        }
        public static int SaveWarehouseSection(string saveId,string saveValue,string saveTimeStamp,string myName)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"update [inventory_CheckWarehouse]
                         set Value=@saveValue,TimeStamp=@saveTimeStam,Editor=@myName,EditTime=@time
                         WHERE [ItemId]=@saveId";
            SqlParameter[] paras = {
                                                new SqlParameter("@saveId", saveId),
                                                new SqlParameter("@saveValue", saveValue),
                                                new SqlParameter("@saveTimeStam", DateTime.Parse(saveTimeStamp).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@myName", myName),
                                                new SqlParameter("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
            int dt = factory.ExecuteSQL(mySql, paras);
            return dt;
        }
//        public static int AddWarehouseSection(string windowId, string windowValue, string windowTimeStamp, string myName)
//        {
//            string connectionString = ConnectionStringFactory.NXJCConnectionString;
//            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

//            string mySql = @"insert into [inventory_CheckWarehouse]
//                            (WarehouseId,Value,TimeStamp,Editor,EditTime) values (@windowId,@windowValue,@windowTimeStam,@myName,@time)";
//            SqlParameter[] paras = {
//                                                new SqlParameter("@windowId", windowId),
//                                                new SqlParameter("@windowValue", windowValue),
//                                                new SqlParameter("@windowTimeStam", DateTime.Parse(windowTimeStamp).ToString("yyyy-MM-dd HH:mm:ss")),
//                                                new SqlParameter("@myName", myName),
//                                                new SqlParameter("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
//                                            };
//            int dt = factory.ExecuteSQL(mySql, paras);
//            return dt;
//        }
        public static void AddWarehouseSection(string json,string myName)
        {
            //string[] detailJsons = json.JsonPickArray("rows");
            //string mId=detailJsons.JsonPick("")
            string[] detailJsons = Regex.Split(json, "\"id\":", RegexOptions.IgnoreCase);
            

            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (TransactionScope tsCope = new TransactionScope())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = connection.CreateCommand();

                    command.CommandText = @"insert into [inventory_CheckWarehouse]
                            (WarehouseId,Value,TimeStamp,Editor,EditTime) values (@windowId,@windowValue,@windowTimeStamp,@myName,@time)";

                    connection.Open();

                    foreach (string detail in detailJsons)
                    {
                        if(detail.JsonPick("Value").Length!=0 && detail.JsonPick("TimeStamp").Length!=0){
                        string windowId = detail.JsonPick("myId");
                        string windowValue = detail.JsonPick("Value");
                        string windowTimeStamp = detail.JsonPick("TimeStamp");
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("windowId", windowId));
                        command.Parameters.Add(new SqlParameter("windowValue", windowValue));
                        command.Parameters.Add(new SqlParameter("windowTimeStamp", DateTime.Parse(windowTimeStamp).ToString("yyyy-MM-dd HH:mm:ss")));
                        command.Parameters.Add(new SqlParameter("myName", myName));
                        command.Parameters.Add(new SqlParameter("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                        command.ExecuteNonQuery();
                        };
                    }
                }

                tsCope.Complete();
            }
        }
//        public static DataTable WindowWarehouseDataTable(string mOrganizationID, string beginTime, string endTime, string wareHouseWindowId)
//        {
//            string connectionString = ConnectionStringFactory.NXJCConnectionString;
//            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
//            string mySql = @"SELECT  LevelCode,Name,Id,Type,MaterialId
//                          ,'' as [Value]
//                          ,'' as [TimeStamp]
//                          ,'' as [Editor]
//                          ,'' as [EditTime]
//                      FROM [dbo].[inventory_Warehouse] 
//                      where Id=@wareHouseId
//                            and OrganizationID=@mOrganizationID
//                    order by [TimeStamp],LevelCode";
//            SqlParameter[] sqlParameters = {
//                                                new SqlParameter("@beginTime", beginTime),
//                                                new SqlParameter("@endTime", endTime),
//                                                new SqlParameter("@wareHouseId", wareHouseWindowId),
//                                                new SqlParameter("@mOrganizationID", mOrganizationID)
//                                            };
//            DataTable table = dataFactory.Query(mySql, sqlParameters);
//            return table;
//        }
        public static DataTable WindowWarehouseDataTableAll(string mOrganizationID, string beginTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT  LevelCode,Name,Id as myId,Type,MaterialId
                          ,'' as [Value]
                          ,'' as [TimeStamp]
                          ,'' as [Editor]
                          ,'' as [CheckEditTime]
                      FROM [dbo].[inventory_Warehouse] 
                      where 
                             OrganizationID=@mOrganizationID
                    order by [TimeStamp],LevelCode";
            SqlParameter[] sqlParameters = {
                                                new SqlParameter("@beginTime", beginTime),
                                                new SqlParameter("@mOrganizationID", mOrganizationID)
                                            };
            DataTable table = dataFactory.Query(mySql, sqlParameters);
            //DataTable tb = InventoryWarehouseDataTableAll(mOrganizationID, beginTime);
            //if (tb.Rows.Count != 0)
            //{
            //    DataTable resultTable = InventoryQueryService.GetInventoryInformation(mOrganizationID, "全部", Convert.ToDateTime(beginTime));
            //    table.Columns.Add("CurrentInventory", Type.GetType("System.Decimal"));
            //    //table = resultTable.DefaultView.ToTable(true, "CurrentInventory");
            //    for (int i = 0; i < resultTable.Rows.Count; i++)
            //    {
            //        for (int j = 0; j < table.Rows.Count; j++)
            //        {
            //            if (table.Rows[j]["Id"].ToString() == resultTable.Rows[i]["Id"].ToString())
            //            {
            //                table.Rows[j]["CurrentInventory"] = resultTable.Rows[i]["CurrentInventory"];
            //            }
            //        }
            //        //table.Rows[i]["CurrentInventory"] = resultTable.Rows[i]["CurrentInventory"];
            //    }
            //}
            DateTime date = DateTime.Now;//GetInventoryInformation方法有4个参数 date在本项目中没用
            DataTable resultTable = InventoryQueryService.GetInventoryInformation(mOrganizationID, "全部", Convert.ToDateTime(beginTime),date);
            table.Columns.Add("CurrentInventory", Type.GetType("System.Decimal"));
            //table = resultTable.DefaultView.ToTable(true, "CurrentInventory");
            for (int i = 0; i < resultTable.Rows.Count; i++)
            {
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    if (table.Rows[j]["myId"].ToString() == resultTable.Rows[i]["Id"].ToString())
                    {
                        table.Rows[j]["CurrentInventory"] = resultTable.Rows[i]["CurrentInventory"];
                    }
                }
                //table.Rows[i]["CurrentInventory"] = resultTable.Rows[i]["CurrentInventory"];
            }
            return table;

        }
    }
}
