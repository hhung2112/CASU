using Microsoft.EntityFrameworkCore;
using CASU.Data; // Namespace chứa AppDbContext của bạn

var builder = WebApplication.CreateBuilder(args);

// Đăng ký AppDbContext sử dụng SQL Server với ConnectionString tên là "Default"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Bật trang hiển thị lỗi chi tiết (Developer Exception Page) khi chạy ở môi trường Development
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();