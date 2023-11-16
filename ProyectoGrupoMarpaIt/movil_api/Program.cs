var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nombre de tu API V1");
    // Puedes ajustar la URL del endpoint Swagger seg�n tu configuraci�n
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
