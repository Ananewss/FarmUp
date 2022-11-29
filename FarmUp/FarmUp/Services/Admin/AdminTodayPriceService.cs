using FarmUp.Dtos.Seller;
using MySql.Data.MySqlClient;
using System.Text;
using MySql.Data;
using FarmUp.Dtos.Admin;
using FruitProject.Models;
using System.Security.Policy;

namespace FarmUp.Services.Admin
{
    public class AdminTodayPriceService
    {
        private readonly ILogger<AdminTodayPriceService> _logger;
        private readonly IConfiguration _config;
        public AdminTodayPriceService(ILogger<AdminTodayPriceService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<ProductDtoList> GetProductList()
        {
            ProductDtoList productDtoList = new ProductDtoList();

            string strConn = _config.GetConnectionString($"onedurian");

            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            try
            {
                await mSqlConn.OpenAsync();
                {
                    String sql = @"SELECT * FROM ma_producttype;";

                    MySqlCommand mCmd = new MySqlCommand(sql, mSqlConn);

                    var readData = await mCmd.ExecuteReaderAsync();
                    while (await readData.ReadAsync())
                    {
                        ProductTypeDto pdt = new ProductTypeDto();
                        pdt.pdt_id = readData["pdt_id"].ToString() ?? "";
                        pdt.pdt_description = readData["pdt_description"].ToString() ?? "";

                        productDtoList.productTypeDtoList.Add(pdt);
                    }
                    readData.Close();
                }
                {
                    String sql = @"SELECT * FROM ma_productgrade;";

                    MySqlCommand mCmd = new MySqlCommand(sql, mSqlConn);

                    var readData = await mCmd.ExecuteReaderAsync();
                    while (await readData.ReadAsync())
                    {
                        ProductGradeDto pdg = new ProductGradeDto();
                        pdg.pdg_id = readData["pdg_id"].ToString() ?? "";
                        pdg.pdg_description = readData["pdg_description"].ToString() ?? "";

                        productDtoList.productGradeDtoList.Add(pdg);
                    }
                    readData.Close();
                }

                {
                    String sql = @"SELECT tdp.*, pdt.pdt_description, pdg.pdg_description
                        FROM tr_today_price tdp
                        LEFT JOIN ma_producttype pdt ON tdp_pdt_id = pdt.pdt_id
                        LEFT JOIN ma_productgrade pdg ON tdp_pdg_id = pdg.pdg_id
                        WHERE tdp_date = DATE_FORMAT(NOW(), '%Y-%m-%d')
                        ORDER BY tdp_pdt_id,tdp_pdg_id,tdp_buyer_name;";

                    MySqlCommand mCmd = new MySqlCommand(sql, mSqlConn);

                    var readData = await mCmd.ExecuteReaderAsync();
                    while (await readData.ReadAsync())
                    {
                        AdminTodayPriceDto tdp = new AdminTodayPriceDto();
                        tdp.pdt_description = readData["pdt_description"].ToString() ?? "";
                        tdp.pdg_description = readData["pdg_description"].ToString() ?? "";
                        tdp.price = readData["tdp_price"].ToString() ?? "";
                        tdp.buyer_name = readData["tdp_buyer_name"].ToString() ?? "";

                        productDtoList.adminTodayPriceDtoList.Add(tdp);
                    }
                    readData.Close();
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                await mSqlConn.CloseAsync();
            }

            return await Task.FromResult(productDtoList);
        }

        internal async Task<bool> InsertTodayPriceAsync(Dictionary<string, string>? dict)
        {

            string strConn = _config.GetConnectionString($"onedurian");
            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            try
            {
                await mSqlConn.OpenAsync();

                string sqlCommand = @"INSERT INTO tr_today_price (tdp_id,tdp_buyer_name,tdp_date,tdp_price,tdp_pdg_id,tdp_pdt_id,updated_by)
                        VALUE(UNHEX(CONCAT(REPLACE(UUID(), '-', ''))),@tdp_buyer_name,DATE_FORMAT(NOW(), '%Y-%m-%d'),@tdp_price,@tdp_pdg_id,@tdp_pdt_id,@updated_by);";

                MySqlCommand cmd = new MySqlCommand(sqlCommand, mSqlConn);
                cmd.Parameters.AddWithValue("@tdp_buyer_name", dict["buyername"].ToString());
                cmd.Parameters.AddWithValue("@tdp_price", dict["price"].ToString());
                cmd.Parameters.AddWithValue("@tdp_pdg_id", dict["pdg"].ToString());
                cmd.Parameters.AddWithValue("@tdp_pdt_id", dict["pdt"].ToString());
                cmd.Parameters.AddWithValue("@updated_by", "Admin");

                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                return false;
            }

            return true; 
        }
    }
}