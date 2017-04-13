using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManange.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System.Data.SqlClient;
using System.Data;

namespace InventoryManange.Service.InventoryManange
{
    public class InventoryQueryService
    {
        public static DataTable GetProcessTypeInfo(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select Name as text,LevelCode AS FormulaLevelCode,Id from [dbo].[inventory_Warehouse] 
                                       where OrganizationID=@organizationId
                                      ";
            SqlParameter sqlParameter = new SqlParameter("@organizationId", organizationId);
            DataTable table = dataFactory.Query(mySql, sqlParameter);
            return table;
        }
        public static DataTable GetInventoryInformation(string organizationID, string warehouseName, DateTime startTime)
        
        { 
            string organizationIdNew=organizationID;
            string warehouseNameNew=warehouseName;
            string FormulaLevelCode="";
            string Id = "";
            DataTable idTable = GetProcessTypeInfo(organizationIdNew);
            DataTable table = new DataTable();
            DataTable getWarehouseTable;
            DataTable getBenchmarksInformationTable;
            DateTime startTimeNew = startTime;
            Decimal benchmarksValue;
            DateTime benchmarksTime;
            DataTable getInputWarehouseTable;
            Decimal inputQuantity;
            DataTable getOutputWarehouseTable;
            Decimal outputQuantity;
            Decimal currentInventory;
            table.Columns.Add("WarehouseName", Type.GetType("System.String"));
            table.Columns.Add("OriginasitionID", Type.GetType("System.String"));
            table.Columns.Add("Id", Type.GetType("System.String"));
            table.Columns.Add("FormulaLevelCode", Type.GetType("System.String"));
            table.Columns.Add("benchmarksValue", Type.GetType("System.String"));
            table.Columns.Add("benchmarksTime", Type.GetType("System.DateTime"));
            table.Columns.Add("InputWarehouse", Type.GetType("System.Decimal"));
            table.Columns.Add("OutputWarehouse", Type.GetType("System.Decimal"));
            table.Columns.Add("CurrentInventory", Type.GetType("System.Decimal"));
            if (warehouseNameNew == "全部")
            {
                for (int i = 0; i < idTable.Rows.Count; i++)
                {
                    warehouseNameNew = Convert.ToString(idTable.Rows[i][0]);
                    FormulaLevelCode = Convert.ToString(idTable.Rows[i][1]);
                    Id = Convert.ToString(idTable.Rows[i][2]);
                     getWarehouseTable = GetWarehouseId(organizationIdNew, warehouseNameNew);//仓库ID信息
                     getBenchmarksInformationTable = GetBenchmarksInformation(startTimeNew, getWarehouseTable);//盘库信息
                     try
                     {
                         benchmarksValue = Convert.ToDecimal(getBenchmarksInformationTable.Rows[0][0]);//基准库存
                     }
                     catch
                     {
                         continue; 
                     }
                     benchmarksTime = Convert.ToDateTime(getBenchmarksInformationTable.Rows[0][1]);//基准库存时间
                     getInputWarehouseTable = GetInputWarehouse(getBenchmarksInformationTable, startTimeNew, organizationIdNew);//入库信息
                     inputQuantity = GetInputQuantity(startTimeNew, getBenchmarksInformationTable, organizationIdNew, getInputWarehouseTable);//入库量
                     getOutputWarehouseTable = GetOutputWarehouse(getBenchmarksInformationTable, startTimeNew, organizationIdNew);//出库信息
                     outputQuantity = GetOutputQuantity(startTimeNew, getBenchmarksInformationTable, organizationIdNew, getOutputWarehouseTable);//出库量
                    currentInventory = benchmarksValue + inputQuantity - outputQuantity;
                    DataRow dr = table.NewRow();
                    dr["WarehouseName"] = warehouseNameNew;
                    dr["OriginasitionID"] = organizationIdNew;
                    dr["Id"] = Id;
                    dr["FormulaLevelCode"] = FormulaLevelCode;
                    dr["benchmarksValue"] = benchmarksValue;
                    dr["benchmarksTime"] = benchmarksTime;
                    dr["InputWarehouse"] = inputQuantity;
                    dr["OutputWarehouse"] = outputQuantity;
                    dr["CurrentInventory"] = currentInventory;
                    table.Rows.Add(dr);
                }
            }
            return table;
        }
        //获取仓库ID
        private static DataTable GetWarehouseId(string organizationID, string warehouseName)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select ID  from [dbo].[inventory_Warehouse] 
                                       where OrganizationID=@organizationID and Name=@warehouseName
                                      ";
            SqlParameter[] myParameter = { new SqlParameter("@organizationID", organizationID), new SqlParameter("@warehouseName", warehouseName) };
            DataTable table = dataFactory.Query(mySql, myParameter);
            return table;
        }
        //获取盘库基准信息
        private static DataTable GetBenchmarksInformation(DateTime startTime, DataTable IdInformation)
        {
            string ID = Convert.ToString(IdInformation.Rows[0][0]);
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select Value,TimeStamp as TimeStamp,WarehouseId from [dbo].[inventory_CheckWarehouse] 
                                       where TimeStamp<@startTime and WarehouseId=@IdInformation
                                      order by TimeStamp desc
                                      ";
            SqlParameter[] myParameter = { new SqlParameter("@startTime", startTime), new SqlParameter("@IdInformation", ID) };
            DataTable table = dataFactory.Query(mySql, myParameter);
            return table;
        }
        //获取入库总信息
        private static DataTable GetInputWarehouse(DataTable BenchmarksInformation, DateTime startTime, string organizationID)
        {

            DateTime BenchmarksTime = Convert.ToDateTime(BenchmarksInformation.Rows[0][1]);//基准库存时间
            string BenchmarksId = Convert.ToString(BenchmarksInformation.Rows[0][2]);//仓库ID
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select VariableId from [dbo].[inventory_WareHousingContrast] 
                                       where WarehouseId=@BenchmarksId and WarehousingType='Input'and @BenchmarksTime<EditTime and EditTime<@startTime
                                      ";
            SqlParameter[] myParameter = { new SqlParameter("@BenchmarksId", BenchmarksId), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@startTime", startTime) };
            DataTable table = dataFactory.Query(mySql, myParameter);

            return table;
        }
        //计算入库量
        private static Decimal GetInputQuantity(DateTime startTime, DataTable BenchmarksInformation, string organizationID, DataTable InputWarehouse)
        {
            int tableRowsCount = InputWarehouse.Rows.Count;
            Decimal inputProduction = 0;
            Decimal inputNygl = 0;
            Decimal inputTotal = 0;
            Decimal inputSigleProduction = 0;
            Decimal inputSigleNygl = 0;
            Decimal BenchmarksValue = Convert.ToDecimal(BenchmarksInformation.Rows[0][0]);//基准库存值
            DateTime BenchmarksTime = Convert.ToDateTime(BenchmarksInformation.Rows[0][1]);//基准库存时间
            for (int i = 0; i < tableRowsCount; i++)
            {
                string variableIdName = Convert.ToString(InputWarehouse.Rows[i][0]);
                if (variableIdName.Length < 5)
                {
                    string connectionString = ConnectionStringFactory.NXJCConnectionString;
                    ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
                    string mySql = @"select sum(Value) as InputSigle from [dbo].[VWB_WeightProduction] 
                                       where @BenchmarksTime<StartTime and StartTime<@startTime  and OrganizationID=@OrganizationID and VariableId=@VariableId
                                      ";
                    SqlParameter[] myParameter = { new SqlParameter("@startTime", startTime), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@OrganizationID", organizationID), new SqlParameter("@VariableId", variableIdName) };
                    DataTable table = dataFactory.Query(mySql, myParameter);
                    try
                    {
                        inputSigleProduction = Convert.ToDecimal(table.Rows[0][0]);
                    }
                    catch
                    {
                        inputSigleProduction = 0;
                    }
                    inputProduction = inputProduction + inputSigleProduction;
                }
                else
                {
                    string connectionString = ConnectionStringFactory.NXJCConnectionString;
                    ISqlServerDataFactory dataFactoryNew = new SqlServerDataFactory(connectionString);
                    string mySql = @"select sum(Value) as InputSigle from [dbo].[VWB_WeightNYGL] 
                                       where @BenchmarksTime<StartTime and StartTime<@startTime and OrganizationID=@OrganizationID and VariableId=@VariableId
                                      ";
                    SqlParameter[] myParameter = { new SqlParameter("@startTime", startTime), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@OrganizationID", organizationID), new SqlParameter("@VariableId", variableIdName) };
                    DataTable table = dataFactoryNew.Query(mySql, myParameter);
                    try
                    {
                        inputSigleNygl = Convert.ToDecimal(table.Rows[0][0]);
                    }
                    catch
                    {
                        inputSigleNygl = 0;
                    }
                    inputNygl = inputNygl + inputSigleNygl;
                }
                inputTotal = inputProduction + inputNygl;
            }
            return inputTotal;
        }
        //获取出库总信息
        private static DataTable GetOutputWarehouse(DataTable BenchmarksInformation, DateTime startTime, string organizationID)
        {

            DateTime BenchmarksTime = Convert.ToDateTime(BenchmarksInformation.Rows[0][1]);//基准库存时间
            string BenchmarksId = Convert.ToString(BenchmarksInformation.Rows[0][2]);//仓库ID
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select VariableId from [dbo].[inventory_WareHousingContrast] 
                                       where WarehouseId=@BenchmarksId and WarehousingType='Output'and @BenchmarksTime<EditTime and @startTime>EditTime
                                      ";
            SqlParameter[] myParameter = { new SqlParameter("@BenchmarksId", BenchmarksId), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@startTime", startTime) };
            DataTable table = dataFactory.Query(mySql, myParameter);
            return table;
        }
        //计算出库量
        private static Decimal GetOutputQuantity(DateTime startTime, DataTable BenchmarksInformation, string organizationID, DataTable OutputWarehouse)
        {
            int tableRowsCount = OutputWarehouse.Rows.Count;
            Decimal outputProduction = 0;
            Decimal outputNygl = 0;
            Decimal outputTotal = 0;
            Decimal outputSigleProduction = 0;
            Decimal outputSigleNygl = 0;
            Decimal BenchmarksValue = Convert.ToDecimal(BenchmarksInformation.Rows[0][0]);//基准库存值
            DateTime BenchmarksTime = Convert.ToDateTime(BenchmarksInformation.Rows[0][1]);//基准库存时间
            for (int i = 0; i < tableRowsCount; i++)
            {
                string variableIdName = Convert.ToString(OutputWarehouse.Rows[i][0]);
                if (variableIdName.Length < 5)
                {
                    string connectionString = ConnectionStringFactory.NXJCConnectionString;
                    ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
                    string mySql = @"select sum(Value) as InputSigle from [dbo].[VWB_WeightProduction] 
                                       where @BenchmarksTime<StartTime and StartTime<@startTime and OrganizationID=@OrganizationID and VariableId=@VariableId
                                      ";
                    SqlParameter[] myParameter = { new SqlParameter("@startTime", startTime), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@OrganizationID", organizationID), new SqlParameter("@VariableId", variableIdName) };
                    DataTable table = dataFactory.Query(mySql, myParameter);
                   
                    try
                    {
                        outputSigleProduction = Convert.ToDecimal(table.Rows[0][0]);
                    }
                    catch
                    {
                        outputSigleProduction = 0;
                    }
                    outputProduction = outputProduction + outputSigleProduction;
                }
                else
                {
                   
                    string connectionString = ConnectionStringFactory.NXJCConnectionString;
                    ISqlServerDataFactory dataFactoryNew = new SqlServerDataFactory(connectionString);
                    string mySql = @"select sum(Value) as InputSigle from [dbo].[VWB_WeightNYGL] 
                                       where @BenchmarksTime<StartTime and StartTime<@startTime and OrganizationID=@OrganizationID and VariableId=@VariableId
                                      ";
                    SqlParameter[] myParameter = { new SqlParameter("@startTime", startTime), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@OrganizationID", organizationID), new SqlParameter("@VariableId", variableIdName) };
                    DataTable table = dataFactoryNew.Query(mySql, myParameter);
                    
                    try
                    {
                        outputSigleNygl = Convert.ToDecimal(table.Rows[0][0]);
                    }
                    catch
                    {
                        outputSigleNygl = 0;
                    }
                    outputNygl = outputNygl + outputSigleNygl;
                }
                outputTotal = outputProduction + outputNygl;
            }
            return outputTotal;
        }
    }
}
