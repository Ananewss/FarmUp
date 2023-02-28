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

namespace FarmUp.Services.MasterService
{
    public class MasterService
    {
        private readonly ILogger<MasterService> _logger;
        private readonly IConfiguration _config;
        string _strConn;
        public MasterService(ILogger<MasterService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _strConn = _config.GetConnectionString($"onedurian");
        }

        public async Task<List<vwUserDBModel>> GetvwUserDtoAsync(string UserType)
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT * FROM vw_user  WHERE 1=1";
                    if (!string.IsNullOrEmpty(UserType))
                    {
                        sQuery += " AND usertype = @UserType";
                        ParamList.Add("UserType", UserType);
                    }


                    var Results = (await conn.QueryAsync<vwUserDBModel>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetvwUserDtoAsync() :: Error ", ex);
                }
            }
        }

        public async Task<List<MaBuyerDBModel>> GetMaBuyerDtoAsync(int? buy_usr_id)
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT * FROM ma_buyer  WHERE 1=1";
                    if (buy_usr_id.HasValue)
                    {
                        sQuery += " AND buy_usr_id = @buy_usr_id";
                        ParamList.Add("buy_usr_id", buy_usr_id);
                    }


                    var Results = (await conn.QueryAsync<MaBuyerDBModel>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetMaBuyerDtoAsync() :: Error ", ex);
                }
            }
        }


        public async Task<List<ProductTypeDBModel>> GetProductTypeDtoAsync()
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT * FROM ma_producttype  WHERE deleted_at IS NULL";


                    var Results = (await conn.QueryAsync<ProductTypeDBModel>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetProductTypeDtoAsync() :: Error ", ex);
                }
            }
        }

        public async Task<List<ProductGradeDBModel>> GetProductGradeDtoAsync()
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT * FROM ma_productgrade  WHERE deleted_at IS NULL";


                    var Results = (await conn.QueryAsync<ProductGradeDBModel>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetProductGradeDtoAsync() :: Error ", ex);
                }
            }
        }

        public async Task<List<string>> GetDistrictBySellerDtoAsync()
        {
            using (IDbConnection conn = new MySqlConnection(_strConn))
            {
                try
                {
                    conn.Open();
                    var ParamList = new DynamicParameters();
                    string sQuery = @"SELECT S.slr_district_th AS District FROM `ma_seller` S 
                                        WHERE S.deleted_at IS NULL AND S.slr_district_th IS NOT NULL
                                        GROUP BY S.slr_district_th";


                    var Results = (await conn.QueryAsync<string>(sQuery, ParamList)).ToList();

                    return Results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetDistrictBySellerDtoAsync() :: Error ", ex);
                }
            }
        }


    }
}