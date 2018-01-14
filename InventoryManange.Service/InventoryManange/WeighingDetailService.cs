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
using UpDownLoadFiles;

namespace InventoryManange.Service.InventoryManange
{
    public class WeighingDetailService
    {
        public static DataTable GetWeighingInfoTable(string mOrganizationId, string mAterialName, string mStartTime, string mEndTime, string mSelectTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            DataTable table = new DataTable();
            if (mSelectTime == "firstWeight")
            {
                //因物料编号导出时的格式问题，故在物料编号前加个"A"，导致后面的过磅明细和日统计的查询中要先截取A后边的字符串
                string mySql = @"select ('A'+A.[Material]) as [Material],A.[MaterialName],A.[ggxh]
                                ,sum(case when A.[Type]= 0 then A.[Suttle] 
                                          when A.[Type]= 5 and A.[reservationchar7]= 11 then A.[Suttle] end) as inputValue
                                ,sum(case when A.[Type]= 3 and A.[sales_gblx]= 'DE' then A.[Suttle]
                                          when A.[Type]= 3 and A.[sales_gblx]= 'RD' then -A.[Suttle]
                                          when A.[Type]= 5 and A.[reservationchar7]= 10 then A.[Suttle] end) as outputValue
                                ,count(A.[Suttle]) as vehicleValue
                               from [extern_interface].[dbo].[WB_WeightNYGL] A,
                                    [dbo].[inventory_MaterialContrast] B
                              where A.[OrganizationID]=@mOrganizationId
                                and A.[MaterialName] like '%' + @mAterialName + '%'
                                and (case when A.[weightdate]<A.[lightdate] then A.[weightdate] else A.[lightdate] end)>@mStartTime
                                and (case when A.[weightdate]<A.[lightdate] then A.[weightdate] else A.[lightdate] end)<@mEndTime
                                and A.[Material]=B.[MaterialID]
                                group by A.[Material],A.[MaterialName],A.[ggxh]
                                order by A.[Material]";
                SqlParameter[] sqlParameter = { new SqlParameter("@mOrganizationId", mOrganizationId),
                                                new SqlParameter("@mAterialName", mAterialName),
                                                new SqlParameter("@mStartTime", mStartTime),
                                                new SqlParameter("@mEndTime", mEndTime)};
                table = dataFactory.Query(mySql, sqlParameter);
            }
            else if (mSelectTime == "endWeight")
            {
                string mySql = @"select ('A'+A.[Material]) as [Material],A.[MaterialName],A.[ggxh]
                                ,sum(case when A.[Type]= 0 then [Suttle] 
                                          when A.[Type]= 5 and A.[reservationchar7]= 11 then A.[Suttle] end) as inputValue
                                ,sum(case when A.[Type]= 3 and A.[sales_gblx]= 'DE' then A.[Suttle]
                                          when A.[Type]= 3 and A.[sales_gblx]= 'RD' then -A.[Suttle]
                                          when A.[Type]= 5 and A.[reservationchar7]= 10 then A.[Suttle] end) as outputValue
                                ,count(A.[Suttle]) as vehicleValue
                               from [extern_interface].[dbo].[WB_WeightNYGL] A,
                                    [dbo].[inventory_MaterialContrast] B
                              where A.[OrganizationID]=@mOrganizationId
                                and A.[MaterialName] like '%' + @mAterialName + '%'
                                and (case when A.[weightdate]>A.[lightdate] then A.[weightdate] else A.[lightdate] end)>@mStartTime
                                and (case when A.[weightdate]>A.[lightdate] then A.[weightdate] else A.[lightdate] end)<@mEndTime
                                and A.[Material]=B.[MaterialID]
                                group by A.[Material],A.[MaterialName],A.[ggxh]
                                order by A.[Material]";
                SqlParameter[] sqlParameter = { new SqlParameter("@mOrganizationId", mOrganizationId),
                                                new SqlParameter("@mAterialName", mAterialName),
                                                new SqlParameter("@mStartTime", mStartTime),
                                                new SqlParameter("@mEndTime", mEndTime)};
                table = dataFactory.Query(mySql, sqlParameter);
            }
            return table;
        }
        //过磅明细
        public static DataTable GetWeighingDetailTable(string mOrganizationId, string mMaterialId, string mStartTime, string mEndTime, string mSelectTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mMaterialID = mMaterialId.Substring(1);
            DataTable table = new DataTable();
            if (mSelectTime == "firstWeight")
            {
                string mySql = @"select  
                                     [BillNO]
                                    ,[Gross]
                                    ,[Tare]
                                    ,[Suttle]
                                    ,[Cars]
                                    ,[carsName]
                                    ,('A'+[Material]) [Material]
                                    ,[MaterialName]
                                    ,[ggxh]
                                    ,(case when [Type]=0 then '采购' when [Type]=3 then '销售' when [Type]=5 then '调拨' end) [Type]
                                    ,[companyid]
                                    ,[TORORGID]
                                    ,[TORORGNAME]
                                    ,[FROMORGID]
                                    ,[FROMORGNAME]
                                    ,(case when [reservationchar7]=10 then '调出' when [reservationchar7]=11 then '调入' end) [reservationchar7]
                                    ,[weightdate]
                                    ,[lightdate]
                                    ,('A'+[lydh1]) [lydh1]
                                    ,('A'+[sales_lydd1]) [sales_lydd1]
                                    ,('A'+[lydh2]) [lydh2]
                                    ,('A'+[sales_lydd2]) [sales_lydd2]
                                    ,(case when [sales_gblx]='RD' then '退货' when [sales_gblx]='DE' then '普通销售' end) [sales_gblx]
                               from [extern_interface].[dbo].[WB_WeightNYGL]
                              where [OrganizationID]=@mOrganizationId
                                and [Material]=@mMaterialId 
                                and (case when [weightdate]<[lightdate] then [weightdate] else [lightdate] end)>@mStartTime
                                and (case when [weightdate]<[lightdate] then [weightdate] else [lightdate] end)<@mEndTime
                                order by [weightdate]";
                SqlParameter[] sqlParameter = { new SqlParameter("@mOrganizationId", mOrganizationId),
                                                new SqlParameter("@mMaterialId", mMaterialID),
                                                new SqlParameter("@mStartTime", mStartTime),
                                                new SqlParameter("@mEndTime", mEndTime)};
                table = dataFactory.Query(mySql, sqlParameter);
            }
            else if (mSelectTime == "endWeight")
            {
                string mySql = @"select  
                                     [BillNO]
                                    ,[Gross]
                                    ,[Tare]
                                    ,[Suttle]
                                    ,[Cars]
                                    ,[carsName]
                                    ,('A'+[Material]) [Material]
                                    ,[MaterialName]
                                    ,[ggxh]
                                    ,(case when [Type]=0 then '采购' when [Type]=3 then '销售' when [Type]=5 then '调拨' end) [Type]
                                    ,[companyid]
                                    ,[TORORGID]
                                    ,[TORORGNAME]
                                    ,[FROMORGID]
                                    ,[FROMORGNAME]
                                    ,(case when [reservationchar7]=10 then '调出' when [reservationchar7]=11 then '调入' end) [reservationchar7]
                                    ,[weightdate]
                                    ,[lightdate]
                                    ,('A'+[lydh1]) [lydh1]
                                    ,('A'+[sales_lydd1]) [sales_lydd1]
                                    ,('A'+[lydh2]) [lydh2]
                                    ,('A'+[sales_lydd2]) [sales_lydd2]
                                    ,(case when [sales_gblx]='RD' then '退货' when [sales_gblx]='DE' then '普通销售' end) [sales_gblx]
                               from [extern_interface].[dbo].[WB_WeightNYGL]
                              where [OrganizationID]=@mOrganizationId
                                and [Material]=@mMaterialID 
                                and (case when [weightdate]>[lightdate] then [weightdate] else [lightdate] end)>@mStartTime
                                and (case when [weightdate]>[lightdate] then [weightdate] else [lightdate] end)<@mEndTime
                                order by [weightdate]";
                SqlParameter[] sqlParameter = { new SqlParameter("@mOrganizationId", mOrganizationId),
                                                new SqlParameter("@mMaterialID", mMaterialID),
                                                new SqlParameter("@mStartTime", mStartTime),
                                                new SqlParameter("@mEndTime", mEndTime)};
                table = dataFactory.Query(mySql, sqlParameter);
            }
            return table;
        }      
        //日统计
        public static DataTable GetWeighingCountDayTable(string mOrganizationId, string mdayMaterialId, string mdaystartTime, string mdayendTime, string mSelectTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mDayMaterialId = mdayMaterialId.Substring(1);
            DataTable table = new DataTable();
            if (mSelectTime == "firstWeight")
            {
                string mySql = @"select convert(varchar(10),[weightdate],120) as StatisticsTime,('A'+[Material]) as [Material],[MaterialName],[ggxh]
                                ,sum(case when [Type]= 0 then [Suttle] 
                                          when [Type]= 5 and [reservationchar7]= 11 then [Suttle] end) as inputValue
                                ,sum(case when [Type]= 3 and [sales_gblx]= 'DE' then [Suttle]
                                          when [Type]= 3 and [sales_gblx]= 'RD' then -[Suttle]
                                          when [Type]= 5 and [reservationchar7]= 10 then [Suttle] end) as outputValue
                                ,count([Suttle]) as vehicleValue
                               from [extern_interface].[dbo].[WB_WeightNYGL]
                              where [OrganizationID]=@mOrganizationId
                                and [Material]=@mdayMaterialId
                                and (case when [weightdate]<[lightdate] then [weightdate] else [lightdate] end)>@mdaystartTime
                                and (case when [weightdate]<[lightdate] then [weightdate] else [lightdate] end)<@mdayendTime
                                group by convert(varchar(10),[weightdate],120),[Material],[MaterialName],[ggxh]
                                order by convert(varchar(10),[weightdate],120)";
                SqlParameter[] sqlParameter = { new SqlParameter("@mOrganizationId", mOrganizationId),
                                                new SqlParameter("@mdayMaterialId", mDayMaterialId),
                                                new SqlParameter("@mdaystartTime", mdaystartTime),
                                                new SqlParameter("@mdayendTime", mdayendTime)};
                table = dataFactory.Query(mySql, sqlParameter);
            }
            else if (mSelectTime == "endWeight")
            {
                string mySql = @"select convert(varchar(10),[weightdate],120) as StatisticsTime,('A'+[Material]) as [Material],[MaterialName],[ggxh]
                                ,sum(case when [Type]= 0 then [Suttle] 
                                          when [Type]= 5 and [reservationchar7]= 11 then [Suttle] end) as inputValue
                                ,sum(case when [Type]= 3 and [sales_gblx]= 'DE' then [Suttle]
                                          when [Type]= 3 and [sales_gblx]= 'RD' then -[Suttle]
                                          when [Type]= 5 and [reservationchar7]= 10 then [Suttle] end) as outputValue
                                ,count([Suttle]) as vehicleValue
                               from [extern_interface].[dbo].[WB_WeightNYGL]
                              where [OrganizationID]=@mOrganizationId
                                and [Material]=@mdayMaterialId
                                and (case when [weightdate]>[lightdate] then [weightdate] else [lightdate] end)>@mdaystartTime
                                and (case when [weightdate]>[lightdate] then [weightdate] else [lightdate] end)<@mdayendTime
                                group by convert(varchar(10),[weightdate],120),[Material],[MaterialName],[ggxh]
                                order by convert(varchar(10),[weightdate],120)";
                SqlParameter[] sqlParameter = { new SqlParameter("@mOrganizationId", mOrganizationId),
                                                new SqlParameter("@mdayMaterialId", mDayMaterialId),
                                                new SqlParameter("@mdaystartTime", mdaystartTime),
                                                new SqlParameter("@mdayendTime", mdayendTime)};
                table = dataFactory.Query(mySql, sqlParameter);
            }
            return table;
        }
        public static void ExportExcelFile(string myFileType, string myFileName, string myData)
        {
            if (myFileType == "xls")
            {
                UpDownLoadFiles.DownloadFile.ExportExcelFile(myFileName, myData);
            }
        }
    }
}
