using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineEdu.Business.Configurations;
using OnlineEdu.Business.Validators;
using OnlineEdu.DataAccess.Context;
using OnlineEdu.Entity.Entities;
using OnlineEduAPI.Extensions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using MassTransit;
using OnlineEduAPI.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddServiceExtensions(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SendEmailConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });
        cfg.ReceiveEndpoint("send-email-queue", e =>
        {
            e.ConfigureConsumer<SendEmailConsumer>(context);
        });
    });
});


builder.Services.AddDbContext<OnlineEduContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"),
        sql => sql.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null));
    options.UseLazyLoadingProxies();
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "redis:6379";
    options.InstanceName = "OnlineEdu_";
});

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Giriş e-postayla yapıldığı için her e-posta tek bir hesaba ait olmalı
    options.User.RequireUniqueEmail = true;
    // Kullanıcı adında Türkçe karakterlere izin ver
    options.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoöpqrsştuüvwxyzABCÇDEFGĞHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._";
}).AddEntityFrameworkStores<OnlineEduContext>().AddErrorDescriber<CustomErrorDescribe>();

var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<JwtTokenOptions>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.Key)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name
    };
});


builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.WriteIndented = true; // opsiyonel
    });
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();




using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OnlineEduContext>();
    db.Database.Migrate();
    
    // Seed sample data
    OnlineEduAPI.DbSeeder.Seed(db);

    var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<OnlineEdu.Entity.Entities.AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<OnlineEdu.Entity.Entities.AppRole>>();
    
    foreach (var roleName in new[] { "Admin", "Teacher", "Student" })
    {
        if (!roleManager.Roles.Any(r => r.Name == roleName))
        {
            roleManager.CreateAsync(new OnlineEdu.Entity.Entities.AppRole { Name = roleName }).Wait();
        }
    }

    var adminUser = userManager.FindByEmailAsync("admin@admin.com").Result;
    if (adminUser == null)
    {
        adminUser = new OnlineEdu.Entity.Entities.AppUser { UserName = "admin", Email = "admin@admin.com", FirstName = "Admin", LastName = "User" };
        userManager.CreateAsync(adminUser, "Admin123!").Wait();
    }
    
    if (!userManager.IsInRoleAsync(adminUser, "Admin").Result)
    {
        userManager.AddToRoleAsync(adminUser, "Admin").Wait();
    }
}

app.Run();
