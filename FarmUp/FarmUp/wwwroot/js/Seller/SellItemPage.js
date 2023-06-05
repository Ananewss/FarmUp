

var baseUrl = window.location.origin;
  

var SellItemPage = SellItemPage || {
    init: function () {

        var self = this;
        self.initEvent(); 
    },
     
    btnEditModal: function (id) {
        console.log(id);
        $("#" + id).modal("show");
    }, 

    btnDelete: function (prd_id) {
        console.log(prd_id);
        $.ajax({
            url: baseUrl + '/Seller/DetailSellItem?prd_id=' + prd_id,
            type: 'get',
            success: function (Response) {
                console.log(Response);
                 
                Swal.fire({
                    title: 'ยืนยันลบ ?',
                    text: 'สายพันธุ์ ' + Response.prd_pdt_desc + " (เกรด " + Response.prd_pdg_desc + ")",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#848b93',
                    confirmButtonText: 'ยืนยัน',
                    cancelButtonText: 'ยกเลิก'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $.ajax({
                            url: baseUrl + '/Seller/DeleteSellItem?prd_id=' + prd_id,
                            type: 'delete',
                            success: function (response) {
                                if (response.status == 200) {
                                    Swal.fire(
                                        'ลบข้อมูลสำเร็จ',
                                        '',
                                        'success'
                                    ).then(() => {
                                        location.reload();
                                    });

                                }
                            }
                        });


                    }
                })
            }

        });


        
    },

    dateFomate(dateInput) {
        console.log(dateInput);
        const month = (dateInput.getMonth() + 1) < 10 ? '0' + (dateInput.getMonth() + 1) : (dateInput.getMonth() + 1);
        const date = dateInput.getDate() < 10 ? '0' + dateInput.getDate() : dateInput.getDate();
        return dateInput.getFullYear() + '-' + month + '-' + date;
    },


    BuyItem: function() {

        var productype = $("[name=productype]").val();
        var productgrade = $("[name=productgrade]").val(); 
        var price = $("[name=price]").val(); 
        var dt = $("[name=dtpicker]").jqxDateTimeInput('getDate');
        console.log(dt);
        var dtpicker = BuyerItemPage.dateFomate(dt);

        console.log(dtpicker);
        var pas = true;
        if (productype == "") { pas = false; alert("กรุณากรอกชื่อสายพันธุ์"); }
        if (price == "") { pas = false; alert("กรุณากรอกราคา"); } 

        if (!pas)
            return;

        var fd = new FormData();
        fd.append('productype', productype);
        fd.append('productgrade', productgrade);
        fd.append('price', price);
        fd.append('dtpicker', dtpicker);
        $.ajax({
            url: baseUrl + '/Buyer/AddBuyItem',
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
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

    GetDetails: function (prd_id) { 
        $.ajax({
            url: baseUrl + '/Seller/DetailSellItem?prd_id=' + prd_id,
            type: 'get', 
            success: function (data) {
                debugger
                console.log(data);
                $("[name=Editid]").val(prd_id);
                $("[name=Editproductype]").val(data.prd_pdt_desc);
                $("[name=Editproductgrade]").val(data.prd_pdg_id);
                $("[name=Editprice]").val(data.prd_price_per_unit);
                $("[name=Editamount]").val(data.prd_amount);
                
                $("#Editmodal").modal("show");
            }
        
        });
    },

    btnEdit: function () {
        var id = $("[name=Editid]").val();
        var productype = $("[name=Editproductype]").val();
        var productgrade = $("[name=Editproductgrade]").val();
        var price = $("[name=Editprice]").val();
        var amount = $("[name=Editamount]").val();
        //var dt = $("[name=Editdtpicker]").jqxDateTimeInput('getDate');
        //var dtpicker = BuyerItemPage.dateFomate(dt);

        //console.log(dtpicker);
        var pas = true;
        if (productype == "") { pas = false; alert("กรุณากรอกชื่อสายพันธุ์"); }
        if (price == "") { pas = false; alert("กรุณากรอกราคา"); }
        if (amount == "") { pas = false; alert("กรุณากรอกจำนวน"); }

        if (!pas)
            return;

        var data = {
            prd_id: id,
            productype: productype,
            productgrade: productgrade,
            price: price,
            amount: amount
        } 
         

        $.ajax({
            url: baseUrl + '/Seller/EditSellItem',
            type: 'put',
            data: data,  
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
    

     getSearchParams(k) {
        var p = {};
        location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (s, k, v) { p[k] = v })
        return k ? p[k] : p;
    }, 

    initEvent: function () {
        var self = this;
         

        document.getElementById("btnBuyItem").onclick = function () { BuyerItemPage.BuyItem() };

       
        $(document).ready(function () {
            
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