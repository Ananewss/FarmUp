using FarmUp.Dtos.Seller;
using MySql.Data.MySqlClient;
using System.Text;
using MySql.Data;
using FarmUp.Dtos.Admin;

namespace FarmUp.Services.Admin
{
    public class BoardcastService
    {
        private readonly ILogger<BoardcastService> _logger;
        private readonly IConfiguration _config;
        public BoardcastService(ILogger<BoardcastService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<BoardcastDtoList> GetBoardcastUser()
        {
            BoardcastDtoList boardcastDtoList = new BoardcastDtoList();
            StringBuilder sbReadBoardcastUser = new StringBuilder();

            sbReadBoardcastUser.Append($"SELECT GROUP_CONCAT(usr.usr_line_id SEPARATOR ', ') lineid, CONCAT(slr.slr_district,', ',slr.slr_province) address ");
            sbReadBoardcastUser.Append($"FROM ma_seller slr ");
            sbReadBoardcastUser.Append($"LEFT JOIN ma_user usr ON slr.slr_usr_id = usr.usr_id ");
            sbReadBoardcastUser.Append($"GROUP BY slr.slr_district;");

            _logger.LogInformation($"[BoardcastService][GetBoardcastUser][sbReadBoardcastUser] = {sbReadBoardcastUser.ToString()}");
            string strConn = _config.GetConnectionString($"onedurian");

            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            try
            {

                MySqlCommand mCmd = new MySqlCommand(sbReadBoardcastUser.ToString(), mSqlConn);

                await mSqlConn.OpenAsync();
                var readData = await mCmd.ExecuteReaderAsync();
                while (await readData.ReadAsync())
                {
                    LineDto boardcastDto = new LineDto();
                    boardcastDto.address = readData["address"].ToString() ?? "";
                    boardcastDto.usr_line_id = readData["lineid"].ToString() ?? "";

                    boardcastDtoList.boardcastDtoList.Add(boardcastDto);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                await mSqlConn.CloseAsync();
            }

            return await Task.FromResult(boardcastDtoList);
        }
    }
}
