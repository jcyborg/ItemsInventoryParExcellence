using ItemsInventoryParExcellence.DataLayer.ApplicationUsers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#if DEBUG
// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
#else
// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
        policy.WithOrigins("invenntoryapp-b6azfbbjbmanaggj.eastus2-01.azurewebsites.net")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
#endif
builder.Services.AddDbContext<AppUsersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsersConnString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__UsersConnString")));
builder.Services.AddScoped<AppUsersContext>();

var app = builder.Build();
app.UseCors("AllowAngularApp");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
