

var baseUrl = window.location.origin;
  

var BuyerItemPage = BuyerItemPage || {
    init: function () {

        var self = this;
        self.initEvent(); 
    },
     
    btnEditModal: function (id) {
        console.log(id);
        $("#" + id).modal("show");
    }, 

    btnDelete: function (id) {
        console.log(id);

        $.ajax({
            url: baseUrl + '/Buyer/DetailBuyItem?tdp_id=' + id,
            type: 'get',
            success: function (Response) {
                console.log(Response);
                 
                Swal.fire({
                    title: 'ยืนยันลบประกาศ ?',
                    text: 'สายพันธุ์ ' + Response.pdt_description + " (เกรด " + Response.pdg_description + ")",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#848b93',
                    confirmButtonText: 'ยืนยัน',
                    cancelButtonText: 'ยกเลิก'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $.ajax({
                            url: baseUrl + '/Buyer/DeleteBuyItem?tdp_id=' + id,
                            type: 'delete',
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

    GetDetails: function(id) { 
        $.ajax({
            url: baseUrl + '/Buyer/DetailBuyItem?tdp_id=' + id,
            type: 'get', 
            success: function (data) {
                console.log(data);
                $("[name=Editid]").val(data.tdp_id);
                $("[name=Editproductype]").val(data.pdt_id);
                $("[name=Editproductgrade]").val(data.pdg_id);
                $("[name=Editprice]").val(data.tdp_price);
                $("[name=Editdtpicker]").val(data.tdp_date);
                $("#Editmodal").modal("show");

            }
        
        });
    },

    btnEdit: function () {
        var id = $("[name=Editid]").val();
        var productype = $("[name=Editproductype]").val();
        var productgrade = $("[name=Editproductgrade]").val();
        var price = $("[name=Editprice]").val();
        var dt = $("[name=Editdtpicker]").jqxDateTimeInput('getDate');
        console.log(productype);
        console.log(dt);
        var dtpicker = BuyerItemPage.dateFomate(dt);

        console.log(dtpicker);
        var pas = true;
        if (productype == "") { pas = false; alert("กรุณากรอกชื่อสายพันธุ์"); }
        if (price == "") { pas = false; alert("กรุณากรอกราคา"); }

        if (!pas)
            return;

        var data = {
            tdp_id: id,
            productype: productype,
            productgrade: productgrade,
            price: price,
            dtpicker: dtpicker
        } 
         

        $.ajax({
            url: baseUrl + '/Buyer/EditBuyItem',
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
            $("[name=dtpicker]").jqxDateTimeInput({ culture: 'th-TH', width: '100%', height: '38px' });
            $("[name=Editdtpicker]").jqxDateTimeInput({ culture: 'th-TH', width: '100%', height: '38px' });
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