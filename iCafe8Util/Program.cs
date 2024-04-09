using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.json", true, true);

var configuration = builder.Build();

try
{
    var barserverLocation = configuration.GetConnectionString("BarServer");

    using var connection = new SqliteConnection($"Data Source={barserverLocation}");

    connection.Open();

    var gameDiskDrive = configuration["Defaults:GameDiskDrive"];

    using var command = connection.CreateCommand();

    command.CommandText = @"UPDATE tbl_IcafeServerDiskCfg SET ClientSymbol = $gameDiskDrive WHERE ServerSymbolPurpose = 1";

    command.Parameters.AddWithValue("$gameDiskDrive", gameDiskDrive);

    await command.ExecuteNonQueryAsync();

    var groupName = configuration["Defaults:GroupName"];

    command.CommandText = @"UPDATE tbl_GroupName SET Name = $groupName WHERE GroupId = 1";

    command.Parameters.AddWithValue("$groupName", groupName);

    await command.ExecuteNonQueryAsync();

}
catch (Exception)
{
    Console.WriteLine("It's likely that your bar server is running, or perhaps this app is not running in an elevated mode (run as admin)");
    
    Console.WriteLine("Please also check if the file path of barserver is correct in your current setup.");
}

