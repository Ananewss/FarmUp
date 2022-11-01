using FarmUp.Dtos.Seller;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text;

namespace FarmUp.Services.Seller
{
    public class TodoList
    {
        private readonly IConfiguration _configuration;
        MySqlConnection conn = new MySqlConnection();

        private readonly ILogger<TodoList> _logger;

        public TodoList(ILogger<TodoList> logger, IConfiguration configuration)
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
                using(MySqlCommand todoListcmd = new MySqlCommand(sbReadTodoList.ToString(),conn))
                {
                    await conn.OpenAsync();
                    _logger.LogInformation($"[TodoList][sellerToDoListProblemList][OpenSQLConnection]");
                    var readTotoListData = await todoListcmd.ExecuteReaderAsync();
                    while(await readTotoListData.ReadAsync())
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

        public async Task AddTodoListAndProblem(TodoListFormDto todoListFormDto,List<IFormFile> imgFiles)
        {
            await Task.Delay(1);
        }
    }
}