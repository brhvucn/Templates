//small repository implementation to work with table storage
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public abstract class BaseRepository<T> where T : TableEntity, new()
{
    private readonly CloudTableClient tableClient;
    private readonly string connectionString;
    private readonly CloudStorageAccount storageAccount;
    protected readonly string tableName;
    private readonly CloudTable table;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;    

    public BaseRepository(IConfiguration config, ILogger logger)
    {

        this._logger = logger;
        this._configuration = config;
        this.connectionString = this._configuration["AzureWebJobsStorage"].ToString();
        storageAccount = CloudStorageAccount.Parse(connectionString);
        tableClient = storageAccount.CreateCloudTableClient();
        this.tableName = getTableName();
        table = tableClient.GetTableReference(tableName);
        table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
    }

    protected abstract string getTableName();

    public async Task Update(T entity)
    {
        try
        {
            if (table != null)
            {
                await table.ExecuteAsync(TableOperation.Replace(entity));
            }
        }
        catch (Exception ex)
        {
            this._logger.LogCritical(ex.Message);
        }
    }

    public async Task Delete(T entity)
    {
        if (table != null)
        {
            await table.ExecuteAsync(TableOperation.Delete(entity));
        }
    }

    public async Task Insert(T entity)
    {
        try
        {
            //await table.CreateIfNotExistsAsync();
            await table.ExecuteAsync(TableOperation.Insert(entity));
        }
        catch (Exception ex)
        {
            this._logger.LogCritical(ex.Message);
        }
    }

    public async Task<T> Get(string partitionkey, string rowkey)
    {
        if (table != null)
        {
            TableQuery<T> query = new TableQuery<T>().Where($"PartitionKey eq '{partitionkey}' and RowKey eq '{rowkey}'");
            var result = await table.ExecuteQuerySegmentedAsync(query, null);
            return result.FirstOrDefault();
        }
        return default(T);
    }   

    public async Task<List<T>> GetAll()
    {
        if (table != null)
        {
            TableQuery<T> query = new TableQuery<T>();
            var result = await table.ExecuteQuerySegmentedAsync(query, null);
            return result.Results;
        }
        return new List<T>();
    }

    public async Task<List<T>> GetAll(string partitionKey)
    {
        if (table != null)
        {
            TableQuery<T> query = new TableQuery<T>().Where($"PartitionKey eq '{partitionKey}'");
            var result = await table.ExecuteQuerySegmentedAsync(query, null);
            return result.Results;
        }
        return new List<T>();
    }

    public async Task<List<T>> GetAllValuesWithFilterAsync(string whereStatement)
    {
        TableQuery<T> query = new TableQuery<T>().Where(whereStatement);
        var entities = new List<T>();
        TableContinuationToken token = null;

        do
        {
            TableQuerySegment<T> resultSegment = await table.ExecuteQuerySegmentedAsync(query, token);
            entities.AddRange(resultSegment.Results);
            token = resultSegment.ContinuationToken;
        } while (token != null);

        return entities;
    } 

    public static I ConvertDynamicEntityToType<I>(DynamicTableEntity dynamicEntity) where I : ITableEntity, new()
    {
        var customEntity = new I
        {
            PartitionKey = dynamicEntity.PartitionKey,
            RowKey = dynamicEntity.RowKey,
            Timestamp = dynamicEntity.Timestamp,
            ETag = dynamicEntity.ETag
        };

        // Map dynamic properties to custom entity properties
        foreach (var property in dynamicEntity.Properties)
        {
            // Use reflection to set the property value
            var propertyInfo = typeof(T).GetProperty(property.Key);
            if (propertyInfo != null)
            {
                object value = GetPropertyTypeValue(property.Value.PropertyAsObject, propertyInfo.PropertyType);
                propertyInfo.SetValue(customEntity, value);
            }
        }

        return customEntity;
    }

    // Helper method to convert property value to the correct type
    private static object GetPropertyTypeValue(object value, Type targetType)
    {
        if (value == null || targetType.IsAssignableFrom(value.GetType()))
        {
            return value;
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value.ToString());
        }

        // Add more type conversions as needed...

        // Default case: use Convert.ChangeType
        return Convert.ChangeType(value, targetType);
    }

}
