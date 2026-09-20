using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ILibraryService>(sp =>
    new LibraryService(
        new SqlBookRepository(new LibraryDbContext()),
        new SqlReservationRepository(new LibraryDbContext())
    ));

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();