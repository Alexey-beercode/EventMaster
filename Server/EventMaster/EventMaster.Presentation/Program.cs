using EventMaster.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAuthentication();
builder.AddCache();
builder.AddDatabase();
builder.AddMapping();
builder.AddServices();
builder.AddValidation();
builder.AddSwaggerDocumentation();
builder.Services.AddControllers(); 
var app = builder.Build();

app.AddApplicationMiddleware();
app.AddSwagger();
app.MapControllers();
app.Run();