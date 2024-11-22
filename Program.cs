using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var configuration = app.Services.GetRequiredService<IConfiguration>();

string connectionString = configuration.GetConnectionString("Default")!;

app.MapGet("/", () =>
{
    var connection = new SqlConnection(connectionString);

    // if connection is property disposed, this should be logged
    connection.Disposed += (sender, a) => Console.WriteLine("Connection disposed");

    connection.Open();

    // select the current server date
    var command = new SqlCommand("SELECT GETDATE() AS CurrentDateTime;", connection);

    return command.ExecuteScalar();
});

app.Run();

