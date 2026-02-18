using GZone.API;
using GZone.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Using package DotNetEnv to load .env file (must be first priority at here)
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// Add services to the container.
builder.Services.RegisterServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GZone API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "GZone API V2");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
