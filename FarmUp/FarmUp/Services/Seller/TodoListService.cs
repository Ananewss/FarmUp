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
        public async Task<ResponseMSG> AddActivityAndProblem(SellerActivityDto sellerActivityDto, List<IFormFile> imgFiles)
        {
            ResponseMSG responseMSG = new ResponseMSG();
            string strConn = _configuration.GetConnectionString($"onedurian");

            /*
                INSERT INTO tr_activity(ActId,ImgUrl,ActDesc,ActTopic,Created_by,updated_by)
                VALUES(UNHEX(CONCAT(REPLACE(UUID(), '-', ''))),'yello2.png','โคนต้นเน่า','โรค|แมลง','1','1')
             */

            StringBuilder sbAddActiviry = new StringBuilder();
            sbAddActiviry.Append($"INSERT INTO tr_activity(ActId,ImgUrl,ActDesc,ActTopic,Created_by,updated_by) ");
            sbAddActiviry.Append($"VALUES(UNHEX(CONCAT(REPLACE(UUID(),'-',''))),@ImgUrl,@ActDesc,@ActTopic,@Created_by,@updated_by) ");

            MySqlConnection mysqlConn = new MySqlConnection(strConn);
            await mysqlConn.OpenAsync();
            MySqlTransaction tr = mysqlConn.BeginTransaction();
            try
            {
                //foreach (var ActivityItem in sellerActivityDto.sellerActivityTrObjs)
                //{
                //    using (MySqlCommand mysqlCmd = new MySqlCommand(sbAddActiviry.ToString(), mysqlConn))
                //    {
                //        mysqlCmd.Parameters.Clear();
                //        //mysqlCmd.Parameters.Add(new MySqlParameter("@ImgUrl", MySqlDbType.VarChar, 255)).Value = ActivityItem.ImgUrl;
                //        mysqlCmd.Parameters.Add(new MySqlParameter("@ActDesc", MySqlDbType.VarChar, 500)).Value = ActivityItem.ActDesc;
                //        mysqlCmd.Parameters.Add(new MySqlParameter("@ActTopic", MySqlDbType.VarChar, 500)).Value = ActivityItem.ActTopic;
                //        mysqlCmd.Parameters.Add(new MySqlParameter("@Created_by", MySqlDbType.VarChar, 50)).Value = sellerActivityDto.UserId;
                //        mysqlCmd.Parameters.Add(new MySqlParameter("@updated_by", MySqlDbType.VarChar, 50)).Value = sellerActivityDto.UserId;
                //        await mysqlCmd.ExecuteNonQueryAsync();
                //        await mysqlCmd.DisposeAsync();
                //    }
                //}

                for (int i =0;i< sellerActivityDto.ActDesc.Length;i++)
                {
                    using (MySqlCommand mysqlCmd = new MySqlCommand(sbAddActiviry.ToString(), mysqlConn))
                    {
                        mysqlCmd.Parameters.Clear();
                        //mysqlCmd.Parameters.Add(new MySqlParameter("@ImgUrl", MySqlDbType.VarChar, 255)).Value = ActivityItem.ImgUrl;
                        mysqlCmd.Parameters.Add(new MySqlParameter("@ImgUrl", MySqlDbType.VarChar, 255)).Value = $"yello{i.ToString()}.png";
                        mysqlCmd.Parameters.Add(new MySqlParameter("@ActDesc", MySqlDbType.VarChar, 500)).Value = sellerActivityDto.ActDesc[i];
                        mysqlCmd.Parameters.Add(new MySqlParameter("@ActTopic", MySqlDbType.VarChar, 500)).Value = sellerActivityDto.ActTopic[i];
                        mysqlCmd.Parameters.Add(new MySqlParameter("@Created_by", MySqlDbType.VarChar, 50)).Value = sellerActivityDto.UserId;
                        mysqlCmd.Parameters.Add(new MySqlParameter("@updated_by", MySqlDbType.VarChar, 50)).Value = sellerActivityDto.UserId;
                        await mysqlCmd.ExecuteNonQueryAsync();
                        await mysqlCmd.DisposeAsync();
                    }
                }
                responseMSG.Status = 200;
                responseMSG.Result = $"Add data success";
                await tr.CommitAsync();
            }
            catch (Exception ex)
            {
                responseMSG.Status = 400;
                responseMSG.Result = $"Add data fail. = {ex.Message.ToString()}";
                await tr.RollbackAsync();
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
    }
}