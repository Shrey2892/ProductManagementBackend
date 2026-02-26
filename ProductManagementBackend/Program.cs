using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductManagementBackend.Data;
using ProductManagementBackend.Services;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//////////////////////////////////////////////////////
// 1️⃣ ADD SERVICES
//////////////////////////////////////////////////////

// Controllers
builder.Services.AddControllers();


// -------------------- MySQL (EF Core) --------------------
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))
    );
});


// -------------------- JWT --------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            ),

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


// -------------------- CORS --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://productmanagement-fl4qk05a2-shreyanair2892-5334s-projects.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// -------------------- Swagger --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// -------------------- App Services --------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();


//////////////////////////////////////////////////////
// 2️⃣ BUILD APP
//////////////////////////////////////////////////////
var app = builder.Build();

//////////////////////////////////////////////////////
// 3️⃣ MIDDLEWARE PIPELINE
//////////////////////////////////////////////////////

// Swagger (OK to keep always-on for now)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AngularPolicy");

// Authentication BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//////////////////////////////////////////////////////
// 4️⃣ MIGRATIONS + RUN
//////////////////////////////////////////////////////

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();