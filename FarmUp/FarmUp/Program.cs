using FarmUp.Services.Admin;
using FarmUp.Services.Buyer;
using FarmUp.Services.LineBot;
using FarmUp.Services.MasterService;
using FarmUp.Services.Seller;
using FarmUp.Services.Seller.Todolist;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TodoListService>();
builder.Services.AddScoped<SellerActivityService>();
builder.Services.AddScoped<WeatherForecastService>();
builder.Services.AddScoped<TodayPriceService>();
builder.Services.AddScoped<BoardcastService>();
builder.Services.AddScoped<AdminTodayPriceService>();
builder.Services.AddScoped<BuyerService>();
builder.Services.AddScoped<MasterService>();
builder.Services.AddScoped<LineBotService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blank}/{action=Index}/{id?}");

app.Run();
