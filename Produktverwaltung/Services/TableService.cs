using Azure.Data.Tables;
using Azure.Data.Tables.Models;

namespace Produktverwaltung.Controllers
{
    public class TableService
    {
        private readonly IConfiguration Configuration;
        string connectionString;
        TableServiceClient tableServiceClient;

        public TableService(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = Configuration["StorageConnectionString"];
            tableServiceClient = new TableServiceClient(connectionString);
        }

        public void CreateTable(string tableName)
        {
            tableServiceClient.CreateTableIfNotExists(tableName);
        }

        public void InsertTableEntry(string tableName, TableEntity tableEntry)
        {
            try
            {
                TableClient tableClient = new TableClient(connectionString, tableName);
                tableClient.Create();
                tableClient.AddEntity(tableEntry);
            }
            catch (Exception e)
            {
            }
        }
    }
}
