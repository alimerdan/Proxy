namespace GatewayService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddSwaggerGen(setup =>
            //{
            //    // Include 'SecurityScheme' to use JWT Authentication
            //    var jwtSecurityScheme = new OpenApiSecurityScheme
            //    {
            //        BearerFormat = "JWT",
            //        Name = "JWT Authentication",
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.Http,
            //        Scheme = JwtBearerDefaults.AuthenticationScheme,
            //        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

            //        Reference = new OpenApiReference
            //        {
            //            Id = JwtBearerDefaults.AuthenticationScheme,
            //            Type = ReferenceType.SecurityScheme
            //        }
            //    };

            //    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            //    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        { jwtSecurityScheme, Array.Empty<string>() }
            //    });
            //});

            // Adding OKTA Integration
            builder.Services.Configure<GatewayService.Okta.OktaJwtVerificationOptions>(
builder.Configuration.GetSection("Okta"));
            builder.Services.AddTransient<GatewayService.Okta.IJwtValidator, GatewayService.Okta.OktaJwtValidation>();

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
        }
    }
}