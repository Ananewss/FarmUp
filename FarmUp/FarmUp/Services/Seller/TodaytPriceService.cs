using FarmUp.Dtos.Seller;
using MySql.Data.MySqlClient;
using System.Text;
using MySql.Data;

namespace FarmUp.Services.Seller
{
    public class TodayPriceService
    {
        private readonly ILogger<TodayPriceService> _logger;
        private readonly IConfiguration _config;
        public TodayPriceService(ILogger<TodayPriceService> logger,
                                             IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<TodayPriceDtoList> GetTodayPrice()
        {
            TodayPriceDtoList todayPriceDtoList = new TodayPriceDtoList();
            StringBuilder sbReadTodayPrice = new StringBuilder();

            sbReadTodayPrice.Append($"SELECT t1.tdp_date,ROUND(AVG(t1.tdp_price),2) as average,t1.tdp_pdg_id, t1.tdp_pdt_id, pdt_description, pdg_description ");
            sbReadTodayPrice.Append($",(SELECT t.tdp_price FROM tr_today_price t WHERE t.tdp_date = CURDATE() AND t.tdp_pdg_id = t1.tdp_pdg_id AND t.tdp_pdt_id = t1.tdp_pdt_id ORDER BY t.tdp_price DESC LIMIT 0,1) max_val ");
            sbReadTodayPrice.Append($",(SELECT t.tdp_buyer_name FROM tr_today_price t WHERE t.tdp_date = CURDATE() AND t.tdp_pdg_id = t1.tdp_pdg_id AND t.tdp_pdt_id = t1.tdp_pdt_id ORDER BY t.tdp_price DESC LIMIT 0,1) max_name ");
            sbReadTodayPrice.Append($",(SELECT t.tdp_price FROM tr_today_price t WHERE t.tdp_date = CURDATE() AND t.tdp_pdg_id = t1.tdp_pdg_id AND t.tdp_pdt_id = t1.tdp_pdt_id ORDER BY t.tdp_price ASC LIMIT 0,1) min_val ");
            sbReadTodayPrice.Append($",(SELECT t.tdp_buyer_name FROM tr_today_price t WHERE t.tdp_date = CURDATE() AND t.tdp_pdg_id = t1.tdp_pdg_id AND t.tdp_pdt_id = t1.tdp_pdt_id ORDER BY t.tdp_price ASC LIMIT 0,1) min_name ");
            sbReadTodayPrice.Append($"FROM tr_today_price t1 ");
            sbReadTodayPrice.Append($"LEFT JOIN ma_productgrade ON t1.tdp_pdg_id = pdg_id ");
            sbReadTodayPrice.Append($"LEFT JOIN ma_producttype ON t1.tdp_pdt_id = pdt_id ");
            sbReadTodayPrice.Append($"WHERE t1.tdp_date = CURDATE() ");
            sbReadTodayPrice.Append($"GROUP BY t1.tdp_pdt_id, t1.tdp_pdg_id ");
            sbReadTodayPrice.Append($"ORDER BY t1.tdp_pdt_id, t1.tdp_pdg_id;");

            _logger.LogInformation($"[TodayPriceService][GetTodayPrice][sbReadTodayPrice] = {sbReadTodayPrice.ToString()}");
            string strConn = _config.GetConnectionString($"onedurian");

            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            try
            {

                MySqlCommand mCmd = new MySqlCommand(sbReadTodayPrice.ToString(), mSqlConn);

                await mSqlConn.OpenAsync();
                var readData = await mCmd.ExecuteReaderAsync();
                while (await readData.ReadAsync())
                {
                    TodayPriceDto todayPriceDto = new TodayPriceDto();
                    todayPriceDto.tdp_id = "--";
                    todayPriceDto.tdp_date = (DateTime)readData["tdp_date"];
                    todayPriceDto.tdp_price = readData["average"].ToString() ?? "";
                    todayPriceDto.tdp_pdg_id = (int)readData["tdp_pdg_id"];
                    todayPriceDto.tdp_pdt_id = (int)readData["tdp_pdt_id"];
                    todayPriceDto.pdg_description = readData["pdg_description"].ToString() ?? "";
                    todayPriceDto.pdt_description = readData["pdt_description"].ToString() ?? "";
                    todayPriceDto.tdp_price_max = readData["max_val"].ToString() ?? "";
                    todayPriceDto.tdp_price_min = readData["min_val"].ToString() ?? "";
                    todayPriceDto.tdp_max_seller_name = readData["max_name"].ToString() ?? "";
                    todayPriceDto.tdp_min_seller_name = readData["min_name"].ToString() ?? "";
                    todayPriceDto.additionalCss = String.Format("bg{0}-light", todayPriceDto.tdp_pdt_id);

                    todayPriceDtoList.todayPriceDtosList.Add(todayPriceDto);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                await mSqlConn.CloseAsync();
            }

            return await Task.FromResult(todayPriceDtoList);
        }
    }
}
