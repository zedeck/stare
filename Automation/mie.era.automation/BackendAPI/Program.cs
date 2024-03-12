global using BackendAPI.Database;
using BackendAPI.Interfaces;
using BackendAPI.Services;
using Common.Authentication;
using Microsoft.OpenApi.Models;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin();
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                      });
});


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ERADBContext>();
builder.Services.AddHttpClient();

builder.Services.AddTransient<IQuestionnaire, QuestionnaireService>();
builder.Services.AddScoped<IDatabaseRepo, DatabaseService>();

builder.Services.AddTransient<IDashboard, DashboardService>();
builder.Services.AddTransient<ICandidate, CandidateService>();
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddTransient<CommsService>();
builder.Services.AddScoped<RefereesService>();
builder.Services.AddTransient<ReportingService>();
builder.Services.AddTransient<ReminderService>();
builder.Services.AddTransient<IAnswer, AnswerService>();
builder.Services.AddTransient<IRequests, RequestsService>();

var jwtSettingSection = builder.Configuration.GetSection("JwtIssuerOptions");
builder.Services.Configure<JwtIssuerOptions>(jwtSettingSection);
var jwtSettings = jwtSettingSection.Get<JwtIssuerOptions>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = jwtSettings.HeaderName,
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.ApiKey//SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.UseMiddleware<Common.CommonException.ExceptionHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
