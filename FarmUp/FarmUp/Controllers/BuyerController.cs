using FarmUp.Services.Admin;
using FruitProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FarmUp.Services.Buyer;
using static Org.BouncyCastle.Math.EC.ECCurve;
using FarmUp.Models.Response.Product;
using FarmUp.Models.Response;
using FarmUp.Services.MasterService;
using FarmUp.Dtos;
using MySql.Data.MySqlClient;
using FarmUp.Dtos.Product;
using System;
using FarmUp.Services.LineBot;
using FarmUp.Models.Request.Product;
using Org.BouncyCastle.Ocsp;

namespace FruitProject.Controllers
{
   
    public class BuyerController : Controller
    {
        private IConfiguration _config;
        private readonly ILogger<AdminController> _logger;
        private readonly BuyerService _buyerService;
        private readonly MasterService _masterService;
        private readonly LineBotService _lineBotService;

        public BuyerController(IConfiguration config, ILogger<AdminController> logger, BuyerService buyerService, MasterService masterService, LineBotService lineBotService)
        {
            _logger = logger;
            _buyerService = buyerService;
            _masterService = masterService;
            _lineBotService = lineBotService;
            _config = config;
        }
         
        public IActionResult Index()
        {
             
                return View();
        }

        public async Task<IActionResult> BuyerItem()
        {
            var Resp = new BuyItemResponse();

            var userId = HttpContext.Session.GetString("userId");
            //ProductBuyer
            var TodayPrice = await _buyerService.GetvwTodayPriceAsync(Convert.ToInt32(userId),null);
            var TodayPriceResp = TodayPrice
                .Where(o => o.created_at.GetValueOrDefault().Date == DateTime.Now.Date)
                .OrderByDescending(o => o.created_at)
                .Select(o => new TodayPriceResp {  
                    tdp_id = new Guid(o.tdp_id) ,
                    tdp_buy_usr_id = o.tdp_buy_usr_id,
                    tdp_date = o.tdp_date,
                    buy_name = o.buy_name,
                    buy_location = o.buy_location, 
                    pdt_id = o.pdt_id,
                    pdt_description = o.pdt_description,
                    pdg_id = o.pdg_id,
                    pdg_description = o.pdg_description, 
                    tdp_price = o.tdp_price,
                    created_at = o.created_at,
                    updated_at = o.updated_at,
                    deleted_at = o.deleted_at,
                    updated_by = o.updated_by,
            }).ToList(); 

            //GetProductTypeDtoAsync
            var ProductTypeDto = await _masterService.GetProductTypeDtoAsync();
            Resp.ProductTypeResp = ProductTypeDto.Select(o => new ProductTypeResp { 
                pdt_id = o.pdt_id,
                pdt_description = o.pdt_description
            }).ToList();
            //GetProductGradeDtoAsync
            var ProductGradeDto = await _masterService.GetProductGradeDtoAsync();
            Resp.ProductGradeResp = ProductGradeDto.Select(o => new ProductGradeResp
            {
                pdg_id = o.pdg_id,
                pdg_description = o.pdg_description
            }).ToList();
            Resp.TodayPriceResp = TodayPriceResp;
            

            return View(Resp);
        }

        [HttpPost]
        public async Task<ResponseMSG> AddBuyItem()
        {

            var userId = HttpContext.Session.GetString("userId");

            var productype = Request.Form["productype"].ToString();
            var productgrade = Request.Form["productgrade"].ToString(); 
            var price = Request.Form["price"].ToString(); 
            var dtpicker = Request.Form["dtpicker"].ToString();

            var BuyerData = (await _masterService.GetMaBuyerDtoAsync(Convert.ToInt32(userId))).FirstOrDefault();
            
            var tdpId = Guid.NewGuid();
            var Data = new trTodayPriceDBModel() {
                tdp_id = tdpId.ToByteArray(),
                tdp_pdt_id = Convert.ToInt32(productype),
                tdp_pdg_id = Convert.ToInt32(productgrade),
                tdp_buy_usr_id = Convert.ToInt32(userId),
                tdp_buyer_name = BuyerData.buy_name,
                tdp_price = Convert.ToDecimal(price) , 
                tdp_date = Convert.ToDateTime(dtpicker),
                updated_by = userId,
            };
             
            var Resp = await _buyerService.InsertTrTodayPriceAsync(Data);
            
            var TodayPrice = (await _buyerService.GetvwTodayPriceAsync(null, Data.tdp_id)).FirstOrDefault();

            var FlexData = new Dictionary<string, string> {
                    { "CreateDate",TodayPrice.created_at?.ToString("dd/MM/yyyy HH:mm")},
                    { "TdpDate",TodayPrice.tdp_date?.ToString("dd/MM/yyyy")},
                    { "ProductType", TodayPrice.pdt_description},
                    { "ProductGrade", TodayPrice.pdg_description},
                    { "PricePerUnit", TodayPrice.tdp_price.ToString()}, 
                    { "location",  TodayPrice.buy_location}
                    };

            var sellerData = await _masterService.GetvwUserDtoAsync("buyer");
            foreach (var seller in sellerData)
            {
                try
                {
                    var flex = await _lineBotService.LineFlexMessage(seller.usr_line_id, 1, "buyer", FlexData);
                }
                catch (Exception)
                {
                     
                }
            }

            ResponseMSG responseMSG = new ResponseMSG();
            responseMSG.Status = 200;
            responseMSG.Result = $"Success";

            return responseMSG;
        }

        [HttpDelete]
        public async Task<ResponseMSG> DeleteBuyItem(Guid tdp_id)
        {

            var userId = HttpContext.Session.GetString("userId");

            var bin_tdpId = tdp_id.ToByteArray();
            var TodayPrice = (await _buyerService.GetTodayPriceAsync(bin_tdpId)).FirstOrDefault();
            if (TodayPrice != null)
            {
                TodayPrice.deleted_at = DateTime.Now;
                TodayPrice.updated_by = userId;

                await _buyerService.UpdateTodayPriceAsync(TodayPrice);
            }
              
            ResponseMSG responseMSG = new ResponseMSG();
            responseMSG.Status = 200;
            responseMSG.Result = $"Success";

            return responseMSG;
        }

        [HttpPut]
        public async Task<ResponseMSG> EditBuyItem(UpdateBuyItemReq req)
        {

            var userId = HttpContext.Session.GetString("userId");

            var bin_tdpId = req.tdp_id.ToByteArray();
            var TodayPrice = (await _buyerService.GetTodayPriceAsync(bin_tdpId)).FirstOrDefault();
            if (TodayPrice != null)
            {
                TodayPrice.tdp_date = req.dtpicker ?? TodayPrice.tdp_date;
                TodayPrice.tdp_price = req.price ?? TodayPrice.tdp_price;
                TodayPrice.tdp_pdt_id = req.productype ?? TodayPrice.tdp_pdt_id;
                TodayPrice.tdp_pdg_id = req.productgrade ?? TodayPrice.tdp_pdg_id; 
                TodayPrice.updated_by = userId;

                await _buyerService.UpdateTodayPriceAsync(TodayPrice);
            }

            ResponseMSG responseMSG = new ResponseMSG();
            responseMSG.Status = 200;
            responseMSG.Result = $"Success";

            return responseMSG;
        }

        [HttpGet]
        public async Task<TodayPriceResp> DetailBuyItem(Guid tdp_id)
        {

            var userId = HttpContext.Session.GetString("userId");

            var bin_tdpId = tdp_id.ToByteArray();

            var TodayPrice = (await _buyerService.GetvwTodayPriceAsync(null, bin_tdpId)).FirstOrDefault();
            var Resp = new TodayPriceResp
            {
                tdp_id = new Guid(TodayPrice.tdp_id),
                tdp_buy_usr_id = TodayPrice.tdp_buy_usr_id,
                tdp_date = TodayPrice.tdp_date,
                buy_name = TodayPrice.buy_name,
                buy_location = TodayPrice.buy_location,
                pdt_id = TodayPrice.pdt_id,
                pdt_description = TodayPrice.pdt_description,
                pdg_id = TodayPrice.pdg_id,
                pdg_description = TodayPrice.pdg_description,
                tdp_price = TodayPrice.tdp_price,
                created_at = TodayPrice.created_at,
                updated_at = TodayPrice.updated_at,
                deleted_at = TodayPrice.deleted_at,
                updated_by = TodayPrice.updated_by,
            };
             

            return Resp;
        }

        [HttpGet]
        public async Task<bool> AlertAdmin(Guid prdid)
        {

            var userId = HttpContext.Session.GetString("userId");

            var bin_prdId = prdid.ToByteArray();
            var TrProduct = (await _buyerService.GetvwTrProductAsync(null, bin_prdId, null, null,null,null)).FirstOrDefault();
            string xx = "user: "  + userId + "\nสนใจ ประกาศ Id:";
            xx += new Guid(TrProduct.prd_id).ToString() ;
            var data = new Dictionary<string, string> {
                        { "customText", xx},
                         
                        };
           
            await _lineBotService.LineFlexMessage("U715e8a962480fce471439e813bc2bc0a", 32, "buyer", data);


            return true;
        }
        public async Task<IActionResult> BuyerMarket(int? productype, int? productgrade,DateTime? harvesttime, string District)
        {
            var Resp = new BuyerMarketResponse();

            var userId = HttpContext.Session.GetString("userId");

              
            //byte[] prdId = req.prd_id == null ? null : req.prd_id.ToByteArray();

            var TrProduct = await _buyerService.GetvwTrProductAsync(null, null, productype, productgrade, harvesttime, District);
            var TrProductResp = TrProduct
                //.Where(o => o.created_at.GetValueOrDefault().Date == DateTime.Now.Date)
                .OrderByDescending(o => o.created_at)
                .Select(o => new TrProductResp
                {
                    prd_id = new Guid(o.prd_id),
                    prd_usr_id = o.prd_usr_id,
                    slr_farm_name = o.slr_farm_name,
                    slr_farm_location = o.slr_farm_location,
                    slr_district_th = o.slr_district_th,
                    prd_datetime = o.prd_datetime,
                    prd_harvest_time = o.prd_harvest_time,
                    pdt_id = o.pdt_id,
                    pdt_description = o.pdt_description,
                    pdg_id = o.pdg_id,
                    pdg_description = o.pdg_description,
                    prd_amount = o.prd_amount,
                    prd_price_per_unit = o.prd_price_per_unit,
                    created_at = o.created_at,
                    updated_at = o.updated_at,
                    deleted_at = o.deleted_at,
                    updated_by = o.updated_by,
                }).ToList();

            var DistrictDto = await _masterService.GetDistrictBySellerDtoAsync();
            Resp.DistrictResp = DistrictDto;

            //GetProductTypeDtoAsync
            var ProductTypeDto = await _masterService.GetProductTypeDtoAsync();
            Resp.ProductTypeResp = ProductTypeDto.Select(o => new ProductTypeResp
            {
                pdt_id = o.pdt_id,
                pdt_description = o.pdt_description
            }).ToList();
            //GetProductGradeDtoAsync
            var ProductGradeDto = await _masterService.GetProductGradeDtoAsync();
            Resp.ProductGradeResp = ProductGradeDto.Select(o => new ProductGradeResp
            {
                pdg_id = o.pdg_id,
                pdg_description = o.pdg_description
            }).ToList();

            Resp.TrProductResp = TrProductResp;


            //set text filter
            string filter = string.Empty;
            if (productype != null)
            {
                filter = ProductTypeDto.Where(o => o.pdt_id == productype).Select(o => String.Join(",", "สายพันธุ์:" + o.pdt_description)).FirstOrDefault();
            }
            if (productgrade != null)
            {
                if (!String.IsNullOrEmpty(filter))
                {
                    filter += "|";
                }
                filter += ProductGradeDto.Where(o => o.pdg_id == productgrade).Select(o => String.Join(",", "เกรด:" + o.pdg_description)).FirstOrDefault();
            }
            if (harvesttime != null)
            {
                if (!String.IsNullOrEmpty(filter))
                {
                    filter += "|";
                }
                filter += "วันที่เก็บเกี่ยว:" + harvesttime.GetValueOrDefault().ToString("dd/MM/yyyy") ;
            }
            if (District != null)
            {
                if (!String.IsNullOrEmpty(filter))
                {
                    filter += "|";
                }
                filter += "จังหวัด:" + District;
            }
            ViewBag.Filter = filter;
       
            return View(Resp);
        }
    }
}
