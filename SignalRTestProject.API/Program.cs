using Microsoft.EntityFrameworkCore;
using SignalRTestProject.API.Hubs;
using SignalRTestProject.API.Mapping;
using SignalRTestProject.API.Models;
using SignalRTestProject.API.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<MessageService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:7218", "http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Services.AddAutoMapper(typeof(MapProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConStr"]);
});



builder.Services.AddControllers();


builder.Services.AddSignalR();





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();


app.MapHub<MessageHub>("/home");
//app.MapHub<MessageHub>("/messages");


//app.UseEndpoints(endpoints =>
//{
//    //endpoints.MapControllers();
//});




app.MapControllers();

app.Run();
