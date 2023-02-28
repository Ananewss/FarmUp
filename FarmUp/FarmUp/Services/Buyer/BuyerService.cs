using FarmUp.Dtos.Seller;
using MySql.Data.MySqlClient;
using System.Text;
using MySql.Data;
using FarmUp.Dtos.Admin;
using FruitProject.Models;
using System.Security.Policy;
using Dapper;
using System.Data;
using FarmUp.Dtos.Buyer;
using FarmUp.Dtos.Product;
using Dapper.Contrib.Extensions;
using System.Data.Common;

namespace FarmUp.Services.Buyer
{
    public class BuyerService
    {
        private readonly ILogger<BuyerService> _logger;
        private readonly IConfiguration _config;
        string _strConn;
        public BuyerService(ILogger<BuyerService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _strConn = _config.GetConnectionString($"onedurian");
        }

        public async Task<List<vwTodayPriceDBModel>> GetvwTodayPriceAsync(int? buyerID, byte[]? tdpID)
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT * FROM vw_today_price WHERE 1=1 ";
                    if (buyerID.HasValue)
                    {
                        sQuery += " AND tdp_buy_usr_id = @buyerID";
                        ParamList.Add("buyerID", buyerID);
                    }
                    if (tdpID != null)
                    {
                        sQuery += " AND tdp_id = @ProductBuyerID";
                        ParamList.Add("ProductBuyerID", tdpID);
                    }


                    var Results = (await conn.QueryAsync<vwTodayPriceDBModel>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetvwProductBuyerAsync() :: Error ", ex);
                }
            } 
        }

        public async Task<List<trTodayPriceDBModel>> GetTodayPriceAsync(byte[] tdpID)
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT * FROM tr_today_price WHERE 1=1 ";
                    sQuery += " AND tdp_id = @tdpID";
                    ParamList.Add("tdpID", tdpID);

                      
                    var Results = (await conn.QueryAsync<trTodayPriceDBModel>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetTodayPriceAsync() :: Error ", ex);
                }
            }
        }

        public async Task<bool> UpdateTodayPriceAsync(trTodayPriceDBModel data)
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();

                    var resp = await conn.UpdateAsync(data);
                      
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"UpdateTodayPriceAsync() :: Error ", ex);
                }
            }
        }

        public async Task<bool> InsertTrTodayPriceAsync(trTodayPriceDBModel Data)
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                conn.Open();
                using (IDbTransaction tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        await conn.InsertAsync(Data, tran);
                        tran.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception($"InsertTrTodayPriceAsync :: Error ", ex);
                    }
                }
            }
        }

        public async Task<List<vwTrProductDBModel>> GetvwTrProductAsync(int? sellerID, byte[]? prdID, int? pdt_id, int? pdg_id, DateTime? harvesttime, string District)
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT * FROM vw_tr_product WHERE 1=1 ";
                    if (sellerID.HasValue)
                    {
                        sQuery += " AND prd_usr_id = @sellerID";
                        ParamList.Add("sellerID", sellerID);
                    }
                    if (prdID != null)
                    {
                        sQuery += " AND prd_id = @prdID";
                        ParamList.Add("prdID", prdID);
                    }
                    if (pdt_id != null)
                    {
                        sQuery += " AND pdt_id = @pdt_id";
                        ParamList.Add("pdt_id", pdt_id);
                    }
                    if (pdg_id != null)
                    {
                        sQuery += " AND pdg_id = @pdg_id";
                        ParamList.Add("pdg_id", pdg_id);
                    }
                    if (harvesttime != null)
                    {
                        sQuery += " AND prd_harvest_time = @harvesttime";
                        ParamList.Add("harvesttime", harvesttime.Value.ToString("yyyy-MM-dd"));
                        string sss = harvesttime.Value.ToString("yyyy-MM-dd");
                    }
                    if (District != null)
                    {
                        sQuery += " AND slr_district_th = @District";
                        ParamList.Add("District", District);
                    }


                    var Results = (await conn.QueryAsync<vwTrProductDBModel>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetvwProductBuyerAsync() :: Error ", ex);
                }
            }
        }


    }
}