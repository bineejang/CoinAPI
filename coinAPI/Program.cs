using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddPolicy("corsapp",builder=>{
    builder.WithOrigins().AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("corsapp");
app.UseAuthorization();

app.MapControllers();

app.Run();

