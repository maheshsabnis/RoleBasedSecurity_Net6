using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoleBasedSecurity_Net6.AppBuilder;
using RoleBasedSecurity_Net6.Models;
using RoleBasedSecurity_Net6.Repositories;
using SharedModels.Models;
using SharedModels.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("RBSAppDbContextConnection"));
});

builder.Services.AddDbContext<RBSAuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("RBSAuthDbContextConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<RBSAuthDbContext>()
                .AddDefaultTokenProviders();




// The registration of the custom service
builder.Services.AddScoped<AuthSecurityService>();
builder.Services.AddScoped<IRepository<Orders, int>, OrdersService>();

builder.Services.AddControllers();



builder.Services.AddCors(options =>
{
    options.AddPolicy("corspolicy", (policy) =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// define policies

// set authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", (policy) =>
    {
        policy.RequireRole("Administrator");
    });

    options.AddPolicy("AdminManagerPolicy", (policy) =>
    {
        policy.RequireRole("Administrator", "Manager");
    });

    options.AddPolicy("AdminManagerClerkPolicy", (policy) =>
    {
        policy.RequireRole("Administrator", "Manager", "Clerk");
    });
});


// The Token Validation
byte[] secretKey = Convert.FromBase64String(builder.Configuration["JWTCoreSettings:SecretKey"]);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    x.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Authentication-Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
}).AddCookie(options =>
{
    options.Events.OnRedirectToAccessDenied =
    options.Events.OnRedirectToLogin = c =>
    {
        c.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.FromResult<object>(null);
    };
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
app.UseCors("corspolicy");
app.UseAuthentication();
app.UseAuthorization();
// Create DefaultAdminsistrator User
IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
await GlobalOps.CreateApplicationAdministrator(serviceProvider);

app.MapControllers();

app.Run();


