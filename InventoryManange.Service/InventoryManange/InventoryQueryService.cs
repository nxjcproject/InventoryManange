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
            string mySql = @"select Name as text,LevelCode AS FormulaLevelCode,Id,HighLimit,LowLimit from [dbo].[inventory_Warehouse] 
                                       where OrganizationID=@organizationId
                                      ";
            SqlParameter sqlParameter = new SqlParameter("@organizationId", organizationId);
            DataTable table = dataFactory.Query(mySql, sqlParameter);
            return table;
        }
        public static DataTable GetInventoryTime(string organizationID,string startTimeWindow,string endTimeWindow)
        {
            string ID;
            DataTable table, idInformation;
             idInformation = GetProcessTypeInfo(organizationID);//获取组织机构对应的各个库的id信息
            if (idInformation.Rows.Count == 0)
            {
                table=null;
                return table;
            }
            else
            {
            ID = Convert.ToString(idInformation.Rows[0][2]);//取其中一个库的id
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select distinct TimeStamp  from [dbo].[inventory_CheckWarehouse] where TimeStamp>@startTimeWindow and TimeStamp<@endTimeWindow and WarehouseId=@wareHouseId order by timestamp desc";
            SqlParameter[] myParameter = { new SqlParameter("@startTimeWindow", startTimeWindow), new SqlParameter("@endTimeWindow", endTimeWindow),new SqlParameter("@wareHouseId",ID)};
             table = dataFactory.Query(mySql, myParameter);
            return table;
            }
        }
        public static DataTable GetInventoryInformation(string organizationID, string warehouseName, DateTime startTime,DateTime endTime)
        {
            string organizationIdNew = organizationID;
            string warehouseNameNew = warehouseName;
            Decimal highLimit = 0;
            Decimal lowLimit = 0;

            string FormulaLevelCode = "";
            string Id = "";
            DataTable idTable = GetProcessTypeInfo(organizationIdNew);
            DataTable table = new DataTable();
            DataTable getWarehouseTable;
            DataTable getBenchmarksInformationTable;
            DataTable getPankuInformationTable;
            DateTime startTimeNew = startTime;
            DateTime endTimeNew = endTime;
            Decimal benchmarksValue;
            Decimal panKuValue;
            DateTime benchmarksTime;
            DataTable getInputWarehouseTable;
            Decimal inputQuantity;
            DataTable getOutputWarehouseTable;
            Decimal outputQuantity;
            Decimal currentInventory;
            Decimal offSet;
            table.Columns.Add("WarehouseName", Type.GetType("System.String"));
            table.Columns.Add("OriginasitionID", Type.GetType("System.String"));
            table.Columns.Add("Id", Type.GetType("System.String"));
            table.Columns.Add("FormulaLevelCode", Type.GetType("System.String"));
            table.Columns.Add("benchmarksValue", Type.GetType("System.Decimal"));
            table.Columns.Add("benchmarksTime", Type.GetType("System.DateTime"));
            table.Columns.Add("InputWarehouse", Type.GetType("System.Decimal"));
            table.Columns.Add("OutputWarehouse", Type.GetType("System.Decimal"));
            table.Columns.Add("CurrentInventory", Type.GetType("System.Decimal"));
            table.Columns.Add("panKuValue", Type.GetType("System.Decimal"));
            table.Columns.Add("highLimit", Type.GetType("System.Decimal"));
            table.Columns.Add("lowLimit", Type.GetType("System.Decimal"));
            table.Columns.Add("offSet", Type.GetType("System.Decimal"));
            if (warehouseNameNew == "全部")
            {
                for (int i = 0; i < idTable.Rows.Count; i++)
                {
                    warehouseNameNew = Convert.ToString(idTable.Rows[i][0]);
                    FormulaLevelCode = Convert.ToString(idTable.Rows[i][1]);
                    Id = Convert.ToString(idTable.Rows[i][2]);

                    getWarehouseTable = GetWarehouseId(organizationIdNew, warehouseNameNew);//仓库ID信息
                    getBenchmarksInformationTable = GetBenchmarksInformation(startTimeNew, getWarehouseTable);//盘库信息(基准)
                    getPankuInformationTable = GetBenchmarksInformation(endTimeNew, getWarehouseTable);//盘库信息(盘库)
                    try
                    {
                        benchmarksValue = Convert.ToDecimal(getBenchmarksInformationTable.Rows[0][0]);//基准库存
                        panKuValue = Convert.ToDecimal(getPankuInformationTable.Rows[0][0]);//盘库库存
                    }
                    catch
                    {
                        continue;
                    }
                    benchmarksTime = Convert.ToDateTime(getBenchmarksInformationTable.Rows[0][1]);//基准库存时间
                   
                    getInputWarehouseTable = GetInputWarehouse(getBenchmarksInformationTable, endTimeNew, organizationIdNew);//入库信息
                    inputQuantity = GetInputQuantity(endTimeNew, getBenchmarksInformationTable, organizationIdNew, getInputWarehouseTable);//入库量
                    getOutputWarehouseTable = GetOutputWarehouse(getBenchmarksInformationTable, endTimeNew, organizationIdNew);//出库信息
                    outputQuantity = GetInputQuantity(endTimeNew, getBenchmarksInformationTable, organizationIdNew, getOutputWarehouseTable);//出库量
                    currentInventory = benchmarksValue + inputQuantity - outputQuantity;//当前库存
                    object obHighLimit = idTable.Rows[i][3];
                    object obLowLimit = idTable.Rows[i][4];
                       try
                        {
                            offSet = currentInventory/panKuValue;//偏差量
                        }
                        catch
                        {
                            offSet=0;
                        }
                    if (obHighLimit is DBNull || obLowLimit is DBNull)
                    {
                        if (obHighLimit is DBNull && !(obLowLimit is DBNull))
                        {
                            highLimit = currentInventory;
                            lowLimit = Convert.ToDecimal(obLowLimit);
                        }
                        else if (obLowLimit is DBNull && !(obHighLimit is DBNull))
                        {
                            lowLimit = currentInventory;
                            highLimit = Convert.ToDecimal(obHighLimit);
                        }
                        else
                        {
                            lowLimit = currentInventory;
                            highLimit = currentInventory;
                        }
                    }
                    else
                    {
                        highLimit = Convert.ToDecimal(obHighLimit);
                        lowLimit = Convert.ToDecimal(obLowLimit);
                    }
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
                    dr["panKuValue"] = panKuValue;
                    dr["highLimit"] = highLimit;
                    dr["lowLimit"] = lowLimit;
                    dr["offSet"] = offSet;
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
                                       where TimeStamp=@startTime and WarehouseId=@IdInformation
                                      order by TimeStamp desc
                                      ";
            SqlParameter[] myParameter = { new SqlParameter("@startTime", startTime), new SqlParameter("@IdInformation", ID) };
            DataTable table = dataFactory.Query(mySql, myParameter);
            return table;
        }
        //获取入库总信息
        private static DataTable GetInputWarehouse(DataTable BenchmarksInformation, DateTime endTime, string organizationID)
        {

            DateTime BenchmarksTime = Convert.ToDateTime(BenchmarksInformation.Rows[0][1]);//基准库存时间
            string BenchmarksId = Convert.ToString(BenchmarksInformation.Rows[0][2]);//仓库ID
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select VariableId,specs,DataTableName,WarehousingType,Multiple,Offset from [dbo].[inventory_WareHousingContrast] 
                                       where WarehouseId=@BenchmarksId and WarehousingType='Input' 
                                      ";
            SqlParameter[] myParameter = { new SqlParameter("@BenchmarksId", BenchmarksId), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@endTime", endTime) };
            DataTable table = dataFactory.Query(mySql, myParameter);

            return table;
        }
        //计算出入库量
        private static Decimal GetInputQuantity(DateTime endTime, DataTable BenchmarksInformation, string organizationID, DataTable InputWarehouse)
        {
            int tableRowsCount = InputWarehouse.Rows.Count;
            Decimal inputNygl = 0;
            Decimal inputSigleNygl = 0;
            Decimal BenchmarksValue = Convert.ToDecimal(BenchmarksInformation.Rows[0][0]);//基准库存值
            DateTime BenchmarksTime = Convert.ToDateTime(BenchmarksInformation.Rows[0][1]);//基准库存时间
            for (int i = 0; i < tableRowsCount; i++)
            {
                string variableIdName = Convert.ToString(InputWarehouse.Rows[i][0]);
                string specsName = Convert.ToString(InputWarehouse.Rows[i][1]);
                string dataTableName = Convert.ToString(InputWarehouse.Rows[i][2]);
                string multiple = Convert.ToString(InputWarehouse.Rows[i][4]);
                string offset = Convert.ToString(InputWarehouse.Rows[i][5]);
                if (dataTableName == "material_MaterialChangeContrast")
                {

                    DataTable table = GetMaterialChangeDataTable(organizationID, "cementmill", BenchmarksTime, endTime);
                    foreach (DataRow dr in table.Rows)
                    {
                        if (dr["VariableId"].ToString() == variableIdName)
                        {
                            try
                            {
                                inputSigleNygl = Convert.ToDecimal(dr["Production"]) * Convert.ToDecimal(multiple) + Convert.ToDecimal(offset);
                            }
                            catch
                            {
                                inputSigleNygl = 0;
                            }
                        }
                        else
                        {
                            continue;
                        }
                        inputNygl = inputNygl + inputSigleNygl;
                    }
                }
                else if (dataTableName == "VWB_WeightNYGL")
                {
                    if (specsName == "")
                    {
                        string connectionString = ConnectionStringFactory.NXJCConnectionString;
                        ISqlServerDataFactory dataFactoryNew = new SqlServerDataFactory(connectionString);
                        string mySql = @"select sum(Value) as InputSigle from [dbo].[VWB_WeightNYGL] 
                                       where @BenchmarksTime<[StatisticalTime] and [StatisticalTime]<@endTime and OrganizationID=@OrganizationID and VariableId=@VariableId
                                      ";
                        SqlParameter[] myParameter = { new SqlParameter("@endTime", endTime), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@OrganizationID", organizationID), new SqlParameter("@VariableId", variableIdName) };
                        DataTable table = dataFactoryNew.Query(mySql, myParameter);
                        try
                        {
                            inputSigleNygl = Convert.ToDecimal(table.Rows[0][0]) * Convert.ToDecimal(multiple) + Convert.ToDecimal(offset);
                        }
                        catch
                        {
                            inputSigleNygl = 0;
                        }
                        inputNygl = inputNygl + inputSigleNygl;
                    }
                    else
                    {
                        string connectionString = ConnectionStringFactory.NXJCConnectionString;
                        ISqlServerDataFactory dataFactoryNew = new SqlServerDataFactory(connectionString);
                        string mySql = @"select sum(Value) as InputSigle from [dbo].[VWB_WeightNYGL] 
                                       where @BenchmarksTime<[StatisticalTime] and [StatisticalTime]<@endTime and OrganizationID=@OrganizationID and VariableId=@VariableId and [VariableSpecs]=@specsName
                                      ";
                        SqlParameter[] myParameter = { new SqlParameter("@endTime", endTime), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@OrganizationID", organizationID), new SqlParameter("@VariableId", variableIdName), new SqlParameter("@specsName", specsName) };
                        DataTable table = dataFactoryNew.Query(mySql, myParameter);
                        try
                        {
                            inputSigleNygl = Convert.ToDecimal(table.Rows[0][0]) * Convert.ToDecimal(multiple) + Convert.ToDecimal(offset);
                        }
                        catch
                        {
                            inputSigleNygl = 0;
                        }
                        inputNygl = inputNygl + inputSigleNygl;
                    }

                }
                else if (dataTableName == "HistoryDCSIncrement")
                {
                    string connectionString = ConnectionStringFactory.NXJCConnectionString;
                    ISqlServerDataFactory dataFactoryNew = new SqlServerDataFactory(connectionString);
                    string mySql = @"select sum({0}) as InputSigle from {1}.[dbo].[HistoryDCSIncrement]
                                       where @BenchmarksTime<[vDate] and [vDate]<@endTime";
                    SqlParameter[] myParameter = { new SqlParameter("@endTime", endTime), new SqlParameter("@BenchmarksTime", BenchmarksTime) };
                    DataTable table = dataFactoryNew.Query(string.Format(mySql, variableIdName, organizationID), myParameter);
                    try
                    {
                        inputSigleNygl = Convert.ToDecimal(table.Rows[0][0]) * Convert.ToDecimal(multiple) + Convert.ToDecimal(offset);
                    }
                    catch
                    {
                        inputSigleNygl = 0;
                    }
                    inputNygl = inputNygl + inputSigleNygl;
                }
            }
            return inputNygl;
        }
        //获取出库总信息
        private static DataTable GetOutputWarehouse(DataTable BenchmarksInformation, DateTime endTime, string organizationID)
        {

            DateTime BenchmarksTime = Convert.ToDateTime(BenchmarksInformation.Rows[0][1]);//基准库存时间
            string BenchmarksId = Convert.ToString(BenchmarksInformation.Rows[0][2]);//仓库ID
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select VariableId,specs,DataTableName,WarehousingType,Multiple,Offset from [dbo].[inventory_WareHousingContrast] 
                                       where WarehouseId=@BenchmarksId and WarehousingType='Output'
                                      ";
            SqlParameter[] myParameter = { new SqlParameter("@BenchmarksId", BenchmarksId), new SqlParameter("@BenchmarksTime", BenchmarksTime), new SqlParameter("@endTime", endTime) };
            DataTable table = dataFactory.Query(mySql, myParameter);
            return table;
        }
        //分品种库存入库（调用了徐一帅的算法有多余数据）
        private static DataTable GetMaterialChangeDataTable(string mOrganizationId, string productionLine, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            DataTable table = new DataTable();
            if (productionLine == "cementmill")
            {
                string Allsql = @"(SELECT 
                                     A.[OrganizationID]
                                    ,C.[Name]
		                            ,B.[MaterialColumn]
		                            ,A.[ChangeStartTime]
		                            ,A.[ChangeEndTime]
		                            ,B.[VariableId]
		                            ,B.[MaterialDataBaseName]
		                            ,B.[MaterialDataTableName]
                                    ,'' as [LevelCode]
		                            ,'leafnode' as [NodeType]
	                            FROM [NXJC].[dbo].[material_MaterialChangeLog] A,[NXJC].[dbo].[material_MaterialChangeContrast] B,[NXJC].[dbo].[system_Organization] C
	                            where A.OrganizationID like @mOrganizationId + '%'
                                    and A.OrganizationID=C.OrganizationID
	                                and B.[ContrastID]=A.[ContrastID]
		                            and A.[VariableType]='Cement'
		                            and LOWER(A.EventValue) = LOWER(B.Valid)
                                    and B.[VariableId] != '自产/外购熟料'
		                            and ((A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@startTime) or
                                    (A.[ChangeStartTime]>=@startTime
                                    and A.[ChangeEndTime]<=@endTime) or (A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@endTime)
	                                or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime]>=@endTime)
                                    or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime] is NULL))
                                )
	                            union all
	                            (SELECT  A.[OrganizationID],C.[Name],'' as [MaterialColumn],'' as [ChangeStartTime],'' as [ChangeEndTime],'' as [VariableId]
	                            ,'' as [MaterialDataBaseName],'' as [MaterialDataTableName],'' as [LevelCode], 'node' as [NodeType]
	                             from [NXJC].[dbo].[material_MaterialChangeLog] A,[NXJC].[dbo].[material_MaterialChangeContrast] B,[NXJC].[dbo].[system_Organization] C
	                            where A.OrganizationID like @mOrganizationId + '%'
                                    and A.OrganizationID=C.OrganizationID
	                                and B.[ContrastID]=A.[ContrastID]
		                            and A.[VariableType]='Cement'
		                            and LOWER(A.EventValue) = LOWER(B.Valid)
                                    and B.[VariableId] != '自产/外购熟料'
		                            and ((A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@startTime) or
                                    (A.[ChangeStartTime]>=@startTime
                                    and A.[ChangeEndTime]<=@endTime) or (A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@endTime)
	                                or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime]>=@endTime)
                                    or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime] is NULL))
		                            --order by A.[OrganizationID]
		                            group by A.[OrganizationID],C.[Name])
		                            order by A.[OrganizationID],A.[ChangeStartTime]";
                SqlParameter[] Allparameter ={
                                          new SqlParameter("mOrganizationId", mOrganizationId),
                                          new SqlParameter("startTime", startTime),
                                          new SqlParameter("endTime", endTime)
                                       };
                table = dataFactory.Query(Allsql, Allparameter);
            }
            else
            {
                string sql = @"(SELECT  
                                   A.[OrganizationID]
                                  ,C.[Name]
		                          ,B.[MaterialColumn]
		                          ,A.[ChangeStartTime]
		                          ,A.[ChangeEndTime]
		                          ,B.[VariableId]
		                          ,B.[MaterialDataBaseName]
		                          ,B.[MaterialDataTableName]
                                  ,'' as [LevelCode]
                                  ,'leafnode' as [NodeType]
	                          FROM [NXJC].[dbo].[material_MaterialChangeLog] A,[NXJC].[dbo].[material_MaterialChangeContrast] B,[NXJC].[dbo].[system_Organization] C
	                          where A.OrganizationID=@productionLine
                                    and A.OrganizationID=C.OrganizationID
	                                and B.[ContrastID]=A.[ContrastID]
			                        and A.[VariableType]='Cement'
			                        and LOWER(A.EventValue) = LOWER(B.Valid)
                                    and B.[VariableId] != '自产/外购熟料'
			                        and ((A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@startTime) or
                                    (A.[ChangeStartTime]>=@startTime
                                    and A.[ChangeEndTime]<=@endTime) or (A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@endTime)
	                                or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime]>=@endTime)
                                    or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime] is NULL))
                              union all
                              (SELECT  
                                   A.[OrganizationID]
                                  ,C.[Name]
		                          ,'' as [MaterialColumn]
		                          ,'' as [ChangeStartTime]
		                          ,'' as [ChangeEndTime]
		                          ,'' as [VariableId]
		                          ,'' as [MaterialDataBaseName]
		                          ,'' as [MaterialDataTableName]
                                  ,'' as [LevelCode]
                                  ,'node' as [NodeType]
	                          FROM [NXJC].[dbo].[material_MaterialChangeLog] A,[NXJC].[dbo].[material_MaterialChangeContrast] B,[NXJC].[dbo].[system_Organization] C
	                          where A.OrganizationID=@productionLine
                                    and A.OrganizationID=C.OrganizationID
	                                and B.[ContrastID]=A.[ContrastID]
			                        and A.[VariableType]='Cement'
			                        and LOWER(A.EventValue) = LOWER(B.Valid)
                                    and B.[VariableId] != '自产/外购熟料'
			                        and ((A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@startTime) or
                                    (A.[ChangeStartTime]>=@startTime
                                    and A.[ChangeEndTime]<=@endTime) or (A.[ChangeStartTime]<=@startTime and A.[ChangeEndTime]>=@endTime)
	                                or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime]>=@endTime)
                                    or (A.[ChangeStartTime]<=@endTime and A.[ChangeEndTime] is NULL))
                                    group by A.[OrganizationID],C.[Name]))
                              order by A.[ChangeStartTime]";
                SqlParameter[] parameter ={
                                          new SqlParameter("productionLine", productionLine),
                                          new SqlParameter("startTime", startTime),
                                          new SqlParameter("endTime", endTime)
                                       };
                table = dataFactory.Query(sql, parameter);
                DataRow row;
                row = table.NewRow();
                row["OrganizationID"] = productionLine;
                row["LevelCode"] = "M01";
            }
            table.Columns.Add("Production");
            table.Columns.Add("Formula");
            table.Columns.Add("Consumption");
            int count = table.Rows.Count;
            for (int j = 0; j < count; j++)
            {
                if (table.Rows[j]["ChangeEndTime"].ToString() == "")
                {
                    table.Rows[j]["ChangeEndTime"] = endTime;
                }
                DateTime m_startTime = Convert.ToDateTime(table.Rows[j]["ChangeStartTime"].ToString().Trim());
                DateTime m_endTime = Convert.ToDateTime(table.Rows[j]["ChangeEndTime"].ToString().Trim());
                if (DateTime.Compare(m_startTime, Convert.ToDateTime(startTime)) == -1)
                {
                    table.Rows[j]["ChangeStartTime"] = startTime;
                }
                if (DateTime.Compare(m_endTime, Convert.ToDateTime(endTime)) == 1)
                {
                    table.Rows[j]["ChangeEndTime"] = endTime;
                }
            }
            for (int i = 0; i < count; i++)
            {
                string nodeType = table.Rows[i]["NodeType"].ToString().Trim();
                if (nodeType == "leafnode")
                {
                    string materialDataBaseName = table.Rows[i]["MaterialDataBaseName"].ToString().Trim();
                    string materialDataTableName = table.Rows[i]["MaterialDataTableName"].ToString().Trim();
                    string changeStartTime = table.Rows[i]["ChangeStartTime"].ToString().Trim();
                    string changeEndTime = table.Rows[i]["ChangeEndTime"].ToString().Trim();
                    string materialColumn = table.Rows[i]["MaterialColumn"].ToString().Trim();
                    string m_productionLine = table.Rows[i]["OrganizationID"].ToString().Trim();
                    //string mProductionLine = table.Rows[i]["OrganizationID"].ToString().Trim();
                    //                string mSql = @"select cast(sum(A.{0}) as decimal(18,2)) as [MaterialProduction]
                    //                                      ,cast(sum(B.[FormulaValue]) as decimal(18,2)) as [Formula]
                    //                                from {1}.[dbo].{2} A,{1}.[dbo].[HistoryFormulaValue] B
                    //                                where A.[vDate]>=@changeStartTime
                    //                                      and A.[vDate]<=@changeEndTime
                    //                                      and B.[OrganizationID]=@productionLine
                    //                                      and B.[vDate]>=@changeStartTime
                    //                                      and B.[vDate]<=@changeEndTime";
                    string mSql = @"select cast(sum([FormulaValue]) as decimal(18,2)) as [Formula] from {0}.[dbo].[HistoryFormulaValue]
                                where vDate>=@changeStartTime
                                        and vDate<=@changeEndTime
                                        and variableId = 'cementPreparation'
	                                    and [OrganizationID]=@m_productionLine";
                    SqlParameter[] para ={
                                        new SqlParameter("m_productionLine", m_productionLine),
                                        new SqlParameter("changeStartTime", changeStartTime),
                                        new SqlParameter("changeEndTime", changeEndTime)
                                     };
                    DataTable passTable = dataFactory.Query(string.Format(mSql, materialDataBaseName), para);
                    string mFormula = passTable.Rows[0]["Formula"].ToString().Trim();
                    string mSsql = @"select cast(sum({0}) as decimal(18,2)) as [MaterialProduction] from {1}.[dbo].{2}
                                where vDate>=@changeStartTime
                                      and vDate<=@changeEndTime";
                    SqlParameter[] paras ={
                                        new SqlParameter("changeStartTime", changeStartTime),

                                        new SqlParameter("changeEndTime", changeEndTime)
                                     };
                    DataTable resultTable = dataFactory.Query(string.Format(mSsql, materialColumn, materialDataBaseName, materialDataTableName), paras);
                    string mProduction = resultTable.Rows[0]["MaterialProduction"].ToString().Trim();
                    table.Rows[i]["Production"] = mProduction;
                    table.Rows[i]["Formula"] = mFormula;
                }
            }
            //增加层次码
            int mcode = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string id = table.Rows[i]["NodeType"].ToString();
                if (id == "node")
                {
                    string nodeCode = "M01" + (++mcode).ToString("00");
                    table.Rows[i]["LevelCode"] = nodeCode;
                    int mleafcode = 0;
                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        if (table.Rows[j]["OrganizationID"].ToString().Trim() == table.Rows[i]["OrganizationID"].ToString().Trim() && table.Rows[j]["NodeType"].ToString().Equals("leafnode"))
                        {
                            table.Rows[j]["LevelCode"] = nodeCode + (++mleafcode).ToString("00");
                        }
                    }
                }
            }
            DataColumn stateColumn = new DataColumn("state", typeof(string));
            table.Columns.Add(stateColumn);
            //此处代码是控制树开与闭的
            //foreach (DataRow dr in table.Rows)
            //{
            //    if (dr["NodeType"].ToString() == "node")
            //    {
            //        dr["state"] = "closed";                           
            //    }
            //    else
            //    {
            //        dr["state"] = "open";
            //    }
            //}
            //计算电耗和产线总计
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i]["Production"].ToString().Trim() != "0.00" && table.Rows[i]["Production"].ToString().Trim() != "")
                {
                    string mFormula = table.Rows[i]["Formula"].ToString().Trim();
                    if (mFormula == "")
                    {
                        mFormula = "0";
                    }
                    double lastFormula = Convert.ToDouble(mFormula);
                    string mProduction = table.Rows[i]["Production"].ToString().Trim();
                    //if (mProduction == "")
                    //{
                    //    mProduction = "0";
                    //}
                    double lastProduction = Convert.ToDouble(mProduction);
                    double mConsumption = Convert.ToDouble((lastFormula / lastProduction).ToString("0.00"));
                    //string lastConsumption = Convert.ToString(mConsumption);
                    table.Rows[i]["Consumption"] = mConsumption;
                }
                if (table.Rows[i]["NodeType"].ToString() == "leafnode" && (table.Rows[i]["Production"].ToString().Trim() == "0.00" || table.Rows[i]["Production"].ToString().Trim() == ""))
                {
                    string mConsumption = "";
                    table.Rows[i]["Consumption"] = mConsumption;
                }
                //string firstName = table.Rows[i]["Name"].ToString().Trim();
                //string secondName = table.Rows[i + 1]["Name"].ToString().Trim();                
            }
            for (int i = 0; i < table.Rows.Count; )
            {
                string m_Name = table.Rows[i]["Name"].ToString();
                DataRow[] m_SubRoot = table.Select("Name = '" + m_Name + "'");
                int length = m_SubRoot.Length;
                double sumProduction = 0;
                double sumFormula = 0;
                double sumConsumption = 0;
                for (int j = 0; j < length; j++)
                {
                    string mmProduction = m_SubRoot[j]["Production"].ToString().Trim();
                    if (mmProduction == "")
                    {
                        mmProduction = "0";
                    }
                    double m_Prodcution = Convert.ToDouble(mmProduction);
                    sumProduction = sumProduction + m_Prodcution;
                    string mmFormula = m_SubRoot[j]["Formula"].ToString().Trim();
                    if (mmFormula == "")
                    {
                        mmFormula = "0";
                    }
                    double m_formula = Convert.ToDouble(mmFormula);
                    sumFormula = sumFormula + m_formula;
                    string mmConsumption = m_SubRoot[j]["Consumption"].ToString().Trim();
                    if (mmConsumption == "")
                    {
                        mmConsumption = "0";
                    }
                    double m_consumption = Convert.ToDouble(mmConsumption);
                    sumConsumption = sumConsumption + m_consumption;
                }
                table.Rows[i]["Production"] = sumProduction;
                table.Rows[i]["Formula"] = sumFormula;
                table.Rows[i]["Consumption"] = Convert.ToDouble((sumFormula / sumProduction).ToString("0.00"));
                i = i + length;
            }
            return table;
        }
    }
}
