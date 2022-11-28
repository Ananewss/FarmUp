using FarmUp.Dtos.Seller.Todolist;
using MySql.Data.MySqlClient;
using System.Text;

namespace FarmUp.Services.Seller.Todolist
{
    public class SellerActivityService
    {
        private readonly IConfiguration _conf;
        private readonly ILogger<SellerActivityService> _logger;
        
        public SellerActivityService(IConfiguration conf,ILogger<SellerActivityService> logger)
        {
            _conf = conf;
            _logger = logger;
        }
        
    }
}
