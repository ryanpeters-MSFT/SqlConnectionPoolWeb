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

    Console.WriteLine($"ClientConnectionId: {connection.ClientConnectionId}");
    Console.WriteLine($"ServerProcessId: {connection.ServerProcessId}");

    var serverCommand = new SqlCommand("SELECT @@SERVERNAME;", connection);
    var versionCommand = new SqlCommand("SELECT @@VERSION;", connection);
    var timeCommand = new SqlCommand("SELECT GETDATE();", connection);

    return new 
    {
        server = serverCommand.ExecuteScalar() as string,
        version = versionCommand.ExecuteScalar() as string,
        date = timeCommand.ExecuteScalar(),
        connection.ClientConnectionId,
        connection.ServerProcessId,
        connection.WorkstationId
    };
});

app.Run();

