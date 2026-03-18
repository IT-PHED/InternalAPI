using INTERNAL.Data;
using INTERNAL.Services.Implementations;
using INTERNAL.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<OracleConnectionFactory>();
builder.Services.AddScoped<ICapturedMeterService, CapturedMeterService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IReportProcessingService, ReportProcessingService>();
builder.Services.AddScoped<IAgentLowBalanceService, AgentLowBalanceService>();
builder.Services.AddScoped<IAgentLowBalanceProcessingService, AgentLowBalanceProcessingService>();
builder.Services.AddHostedService<ReportScheduler>();
builder.Services.AddHostedService<AgentBalanceScheduler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
