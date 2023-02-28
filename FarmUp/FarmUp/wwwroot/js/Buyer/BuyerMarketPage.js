

var baseUrl = window.location.origin;
  

var BuyerMarketPage = BuyerMarketPage || {
    init: function () {

        var self = this;
        self.initEvent(); 
    }, 

     

    dateFomate(dateInput) {
        console.log(dateInput);
        const month = (dateInput.getMonth() + 1) < 10 ? '0' + (dateInput.getMonth() + 1) : (dateInput.getMonth() + 1);
        const date = dateInput.getDate() < 10 ? '0' + dateInput.getDate() : dateInput.getDate();
        return dateInput.getFullYear() + '-' + month + '-' + date;
    }, 

    

    BuyerMarketSearch: function () {
        //var id = $("[name=searchid]").val();
        var productype = $("[name=searchproductype]").val();
        var productgrade = $("[name=searchproductgrade]").val();
        var price = $("[name=searchprice]").val();
        var dt = $("[name=searchdtpicker]").jqxDateTimeInput('getDate');
        console.log(productype);
        console.log(dt);
        var dtpicker = BuyerMarketPage.dateFomate(dt);

        console.log(dtpicker);
        var pas = true;
        //if (productype == "") { pas = false; alert("กรุณากรอกชื่อสายพันธุ์"); }
        //if (price == "") { pas = false; alert("กรุณากรอกราคา"); }

        if (!pas)
            return;

        var data = { 
            productype: productype,
            productgrade: productgrade,
            price: price,
            dtpicker: dtpicker
        } 
         

        $.ajax({
            url: baseUrl + '/Buyer/BuyerMarketSearch',
            type: 'get',
            data: data,  
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                if (response.status == 200) {
                    Swal.fire(
                        'บันทึกสำเร็จ',
                        '',
                        'success'
                    ).then(() => {
                        location.reload();
                    }); 
                }
            }
        });
    },

    alertAdmin(id) {
        var data = {
            prdid: id
        }; 

        $.ajax({
            url: baseUrl + '/Buyer/AlertAdmin',
            type: 'get',
            data: data,
            success: function (response) {
                if (response.status == 200) {
                    Swal.fire(
                        'แจ้งแอดมินเรียบร้อย',
                        '',
                        'success'
                    ).then(() => {
                        location.reload();
                    });
                }
            }
        }); 
    },

    OpenSearchModal() {
        console.log("show");
        $("#searchmodal").modal("show");
    },
    

     getSearchParams(k) {
        var p = {};
        location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (s, k, v) { p[k] = v })
        return k ? p[k] : p;
    }, 

    initEvent: function () {
        var self = this;
         

        //document.getElementById("btnBuyItem").onclick = function () { BuyerItemPage.BuyItem() };

        

       
        $(document).ready(function () {
             
            $("[name=harvesttime]").jqxDateTimeInput({ formatString: 'yyyy-MM-dd', value: null, culture: 'th-TH', width: '100%', height: '38px' });
            //$('#harvesttime').on('valueChanged', function (event) { 
            //    var date = event.args.date;
            //    console.log(date);
            //    $('#harvesttime').value = date;
            //});
        });
        
    },
    dispose: function () {

    }


}



//window.onload = function () {
//    if (location.href.indexOf("localhost") > 0) {
//            var lineUserId = "U715e8a962480fce471439e813bc2bc0a";
//        var ref = getSearchParams("ref");
//        {
//            $.post('@Url.Action("GetLIFF", "Blank")', JSON.stringify({
//                ref: ref,
//                lineUserId: lineUserId
//            })).done(function (result) {
//                if (result != null && result != "")
//                    window.location = result;
//            });
//        }
//    } else {
//        const defaultLiffId = '1657681416-QJr0wxdE';
//        liff.init({ liffId: defaultLiffId, withLoginOnExternalBrowser: true }).then(() => {
//            liff.getProfile().then(function (profile) {
//                console.log(profile);
//                var lineUserId = profile.userId;
//                console.log(lineUserId);
//                var ref = getSearchParams("ref");

//                //https://localhost:44335/?ref=Seller-WeatherForecast
//                {
//                    $.post('@Url.Action("GetLIFF", "Blank")', JSON.stringify({
//                        ref: ref,
//                        lineUserId: lineUserId
//                    })).done(function (result) {
//                        if (result != null && result != "")
//                            window.location = result;
//                    });
//                }
//            });
//        }).catch((err) => {
//            console.log(err.code, err.message);
//        })
//    }
//}