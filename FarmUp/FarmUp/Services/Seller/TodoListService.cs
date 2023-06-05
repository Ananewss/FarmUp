using FarmUp.Dtos;
using FarmUp.Dtos.Seller;
using FarmUp.Dtos.Seller.Todolist;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Text;

namespace FarmUp.Services.Seller
{
    public class TodoListService
    {
        private readonly IConfiguration _configuration;
        MySqlConnection conn = new MySqlConnection();

        private readonly ILogger<TodoListService> _logger;

        public TodoListService(ILogger<TodoListService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<TodoListDto> SellerToDoListProblemList()
        {
            string strConn = _configuration.GetConnectionString($"onedurian");
            conn.ConnectionString = strConn;

            TodoListDto todoListDto = new TodoListDto();
            /*
               SELECT tdl_id,tdl_description 
               FROM ma_todolist
               WHERE tdl_type IN ('T')
               ORDER BY tdl_id
            */
            StringBuilder sbReadTodoList = new StringBuilder();
            sbReadTodoList.Clear();
            sbReadTodoList.Append($"SELECT tdl_id,tdl_description ");
            sbReadTodoList.Append($"FROM ma_todolist ");
            sbReadTodoList.Append($"WHERE tdl_type IN ('T') ");
            sbReadTodoList.Append($"ORDER BY tdl_id ");

            /*
                SELECT tdl_id,tdl_description
                FROM ma_todolist
                WHERE tdl_type IN ('P')
                ORDER BY tdl_id
             */
            StringBuilder sbReadProblem = new StringBuilder();
            sbReadProblem.Clear();
            sbReadProblem.AppendLine($"SELECT tdl_id,tdl_description ");
            sbReadProblem.AppendLine($"FROM ma_todolist ");
            sbReadProblem.AppendLine($"WHERE tdl_type IN ('P') ");
            sbReadProblem.AppendLine($"ORDER BY tdl_id ");

            try
            {
                using (MySqlCommand todoListcmd = new MySqlCommand(sbReadTodoList.ToString(), conn))
                {
                    await conn.OpenAsync();
                    _logger.LogInformation($"[TodoList][sellerToDoListProblemList][OpenSQLConnection]");
                    var readTotoListData = await todoListcmd.ExecuteReaderAsync();
                    while (await readTotoListData.ReadAsync())
                    {
                        SelectListItem todolistSelectListItem = new SelectListItem();

                        todolistSelectListItem.Text = readTotoListData["tdl_description"].ToString();
                        todolistSelectListItem.Value = Convert.ToInt32(readTotoListData["tdl_id"]).ToString();
                        todoListDto.DoList.Add(todolistSelectListItem);
                    }
                    await conn.CloseAsync();
                }
                using (MySqlCommand problemListcmd = new MySqlCommand(sbReadProblem.ToString(), conn))
                {
                    await conn.OpenAsync();
                    _logger.LogInformation($"[TodoList][sellerToDoListProblemList][OpenSQLConnection]");
                    var readProblemListData = await problemListcmd.ExecuteReaderAsync();
                    while (await readProblemListData.ReadAsync())
                    {
                        SelectListItem problemlistSelectListItem = new SelectListItem();

                        problemlistSelectListItem.Text = readProblemListData["tdl_description"].ToString();
                        problemlistSelectListItem.Value = Convert.ToInt32(readProblemListData["tdl_id"]).ToString();
                        todoListDto.ProblemList.Add(problemlistSelectListItem);
                    }
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"[TodoList][sellerToDoListProblemList][ex] = {ex.Message.ToString()}");
            }
            return await Task.FromResult(todoListDto);
        }

        //Use this for Save Activity Transection
        //27-11-65
        public async Task<ResponseMSG> AddActivityAndProblem(String userId,String ImagePath, String datatype)
        {
            ResponseMSG responseMSG = new ResponseMSG();
            string strConn = _configuration.GetConnectionString($"onedurian");

            StringBuilder sbAddActiviry = new StringBuilder();
            sbAddActiviry.Append($"INSERT INTO tr_activity(ActId,usr_id,ImgUrl,datatype,Created_by,updated_by) ");
            sbAddActiviry.Append($"VALUES(UNHEX(CONCAT(REPLACE(UUID(),'-',''))),@usr_id,@ImgUrl,@datatype,@Created_by,@updated_by) ");

            MySqlConnection mysqlConn = new MySqlConnection(strConn);
            await mysqlConn.OpenAsync();
            try
            {   using (MySqlCommand mysqlCmd = new MySqlCommand(sbAddActiviry.ToString(), mysqlConn))
                {
                    mysqlCmd.Parameters.Clear();
                    //mysqlCmd.Parameters.Add(new MySqlParameter("@ImgUrl", MySqlDbType.VarChar, 255)).Value = ActivityItem.ImgUrl;
                    mysqlCmd.Parameters.AddWithValue("@usr_id", userId);
                    mysqlCmd.Parameters.AddWithValue("@ImgUrl", ImagePath);
                    mysqlCmd.Parameters.AddWithValue("@datatype", datatype);
                    mysqlCmd.Parameters.AddWithValue("@Created_by", userId);
                    mysqlCmd.Parameters.AddWithValue("@updated_by", userId);
                    await mysqlCmd.ExecuteNonQueryAsync();
                    await mysqlCmd.DisposeAsync();
                }
                
                responseMSG.Status = 200;
                responseMSG.Result = $"Add data success";
            }
            catch (Exception ex)
            {
                responseMSG.Status = 400;
                responseMSG.Result = $"Add data fail. = {ex.Message.ToString()}";
            }
            finally
            {
                if (mysqlConn != null)
                {
                    if (mysqlConn.State == ConnectionState.Open)
                    {
                        await mysqlConn.CloseAsync();
                    }
                }
            }
            return responseMSG;
        }

        //Use this for save image to current location
        //27-11-65
        public async Task<ResponseMSG> SaveActivityImageToLocation(List<IFormFile> imgFiles)
        {
            ResponseMSG responseMSG = new ResponseMSG();

            //Read From appsettings.json
            //Set location for save Image file
            var readLocation = _configuration.GetValue<string>($"LocationImage:ActivityImage");

            return await Task.FromResult(responseMSG);
        }


        public async Task<SellerActivityResponseDto> AddActivityDataMaster(SellerActivityDto sellerActivityDto)
        {
            string strConn = _configuration.GetConnectionString($"onedurian");
            SellerActivityResponseDto sellerActivityResponseDto = new SellerActivityResponseDto();
            MySqlConnection Sqlconn = new MySqlConnection(strConn);

            /*
                INSERT INTO ma_activity(ActDesc,Created_by,updated_by)
                VALUES('ใบจุดสีดำ','id001','id001')
             */


            /*
                INSERT INTO tr_today_price (tdp_id)
                VALUE(UNHEX(CONCAT(REPLACE(UUID(), '-', '')))
             */

            StringBuilder sbActMaster = new StringBuilder();
            sbActMaster.Append($"INSERT INTO ma_activity(ActDesc,Created_by,updated_by) ");
            sbActMaster.Append($"VALUES(@ActDesc,@Created_by,@updated_by) ");

            try
            {
                await Sqlconn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(sbActMaster.ToString(), Sqlconn))
                {
                    cmd.Parameters.Add(new MySqlParameter("@ActDesc", MySqlDbType.VarChar)).Value = sellerActivityDto.Name;
                    cmd.Parameters.Add(new MySqlParameter("@Created_by", MySqlDbType.VarChar)).Value = sellerActivityDto.Name;
                    cmd.Parameters.Add(new MySqlParameter("@updated_by", MySqlDbType.VarChar)).Value = sellerActivityDto.Name;

                    await cmd.ExecuteNonQueryAsync();

                }

                sellerActivityResponseDto.ResponseMSG.Result = "Save data success.";
                sellerActivityResponseDto.ResponseMSG.Status = 200;
            }
            catch (Exception ex)
            {
                sellerActivityResponseDto.ResponseMSG.Result = $"Save data fail. = {ex.Message.ToString()}";
                sellerActivityResponseDto.ResponseMSG.Status = 400;
            }
            finally
            {
                if (Sqlconn != null)
                {
                    if (Sqlconn.State == System.Data.ConnectionState.Open)
                    {
                        await Sqlconn.CloseAsync();
                    }
                }
            }

            return sellerActivityResponseDto;
        }

        public async Task<ActivitylistSelectObjList> ReadActivitylistSelectListObj()
        {
            string strConn = _configuration.GetConnectionString($"onedurian");

            ActivitylistSelectObjList todolistSelectObjList = new ActivitylistSelectObjList();
            StringBuilder sbReadActivity = new StringBuilder();

            /*
                SELECT ActId,ActDesc
                FROM ma_activity
                WHERE isActive = 'Y'
             */

            sbReadActivity.Append($"SELECT ActId,ActDesc ");
            sbReadActivity.Append($"FROM ma_activity ");
            sbReadActivity.Append($"WHERE isActive = 'Y' ");

            MySqlConnection mysqlConnReadActList = new MySqlConnection(strConn);

            try
            {
                await mysqlConnReadActList.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(sbReadActivity.ToString(), mysqlConnReadActList))
                {
                    var readData = await cmd.ExecuteReaderAsync();
                    while (await readData.ReadAsync())
                    {
                        ActivitylistSelectObj activitylistSelectObj = new ActivitylistSelectObj();
                        activitylistSelectObj.ActId = readData["ActId"].ToString() ?? "";
                        activitylistSelectObj.ActDesc = readData["ActDesc"].ToString() ?? "";

                        todolistSelectObjList.activitylistSelectObjs.Add(activitylistSelectObj);
                    }
                }
                todolistSelectObjList.ResponseMSG.Result = "Read data success.";
                todolistSelectObjList.ResponseMSG.Status = 200;
            }
            catch (Exception ex)
            {
                todolistSelectObjList.ResponseMSG.Result = $"Read data fail. = {ex.Message.ToString()}";
                todolistSelectObjList.ResponseMSG.Status = 400;
            }
            finally
            {
                if (mysqlConnReadActList != null)
                {
                    if (mysqlConnReadActList.State == System.Data.ConnectionState.Open)
                    {
                        await mysqlConnReadActList.CloseAsync();
                    }
                }
            }


            return await Task.FromResult(todolistSelectObjList);
        }

        internal async Task<SellerTodoListDtoList> GetTodoListByUser(string? userId)
        {
            SellerTodoListDtoList sellerTodaoListDtoList = new SellerTodoListDtoList();
            string strConn = _configuration.GetConnectionString($"onedurian");

            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            mSqlConn.Open();
            try
            {
                {
                    MySqlCommand cmd = new MySqlCommand(@"SELECT *
                        FROM tr_activity
                        WHERE usr_id = @usr_id
                        AND DATE(created_at) = CURDATE()
                        AND deleted_at IS NULL
                        ORDER BY created_at DESC", mSqlConn);
                    cmd.Parameters.AddWithValue("@usr_id", userId);


                    var readData = cmd.ExecuteReader();
                    while (readData.Read())
                    {
                        SellerTodoListDto todoList = new SellerTodoListDto();
                        todoList.ImageUrl = readData["ImgUrl"].ToString();
                        todoList.ActDesc = (readData["ActDesc"] != null) ? readData["ActDesc"].ToString() : "";
                        todoList.ActTopic = (readData["ActTopic"] != null) ? readData["ActTopic"].ToString() : "";

                        if (readData["datatype"].ToString().Equals("normalAct"))
                            sellerTodaoListDtoList.todayListDtosList.Add(todoList);
                        else
                            sellerTodaoListDtoList.todayAlarmDtosList.Add(todoList);
                    }
                    readData.Close();
                    readData.Dispose();
                }

                {
                    MySqlCommand cmd = new MySqlCommand(@"SELECT ActDesc
                        FROM ma_activity
                        WHERE deleted_at IS NULL
                        ORDER BY ActDesc", mSqlConn);


                    var readData = cmd.ExecuteReader();
                    while (readData.Read())
                    {
                        sellerTodaoListDtoList.titleActivity.Add(readData["ActDesc"].ToString());
                    }
                    readData.Close();
                    readData.Dispose();
                }

            }
            catch (Exception ex) { }
            mSqlConn.Close();

            return await Task.FromResult(sellerTodaoListDtoList);
        }

        internal ResponseMSG UpdateActivity(string title, string desc, string imgUrl)
        {
            ResponseMSG responseMSG = new ResponseMSG();
            string strConn = _configuration.GetConnectionString($"onedurian");

            MySqlConnection mysqlConn = new MySqlConnection(strConn);
            mysqlConn.Open();
            try
            {
                using (MySqlCommand mysqlCmd = new MySqlCommand(@"UPDATE tr_activity SET ActDesc=@ActDesc, ActTopic=@ActTopic, updated_at = current_timestamp() WHERE ImgUrl=@ImgUrl", mysqlConn))
                {
                    mysqlCmd.Parameters.Clear();
                    //mysqlCmd.Parameters.Add(new MySqlParameter("@ImgUrl", MySqlDbType.VarChar, 255)).Value = ActivityItem.ImgUrl;
                    mysqlCmd.Parameters.AddWithValue("@ActTopic", System.Net.WebUtility.HtmlDecode(title));
                    mysqlCmd.Parameters.AddWithValue("@ActDesc", System.Net.WebUtility.HtmlDecode(desc));
                    mysqlCmd.Parameters.AddWithValue("@ImgUrl", imgUrl);
                    mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Dispose();
                }

                responseMSG.Status = 200;
                responseMSG.Result = $"Add data success";
            }
            catch (Exception ex)
            {
                responseMSG.Status = 400;
                responseMSG.Result = $"Add data fail. = {ex.Message.ToString()}";
            }
            finally
            {
                if (mysqlConn != null)
                {
                    if (mysqlConn.State == ConnectionState.Open)
                    {
                        mysqlConn.Close();
                    }
                }
            }
            return responseMSG;
        }
    }
}