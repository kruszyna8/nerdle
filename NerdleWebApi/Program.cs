using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IExpressionValidator, ExpressionValidator>();
builder.Services.AddScoped<IUserExpressionGuessing, UserExpressionGuessing>();
builder.Services.AddSingleton<IUniqueUserExpressionGuessing, UniqueUserExpressionGuessing>();
builder.Services.AddSingleton<IUniqueComputerExpressionGuessing, UniqueComputerExpressionGuessing>();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.Console().
WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
