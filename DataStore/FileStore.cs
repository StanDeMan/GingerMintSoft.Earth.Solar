using System.Diagnostics.CodeAnalysis;
using Serilog;
using Newtonsoft.Json.Linq;
using JsonFlatFileDataStore;

namespace DataBase;

public class FileStore : IDisposable, IFileStore
{
    private const string DbName = "database";
    private const string CollectionName = "plants";

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")] 
    public bool IsRunning { get; set; }

    private string? DataBaseName { get; } = DbName;

    private DataStore? DataStore { get; }

    /// <summary>
    /// Flat Json file data store
    /// </summary>
    /// <param name="dataBaseName">Name of file to create</param>
    public FileStore(string? dataBaseName = null)
    {
        try
        {
            // Generate store with upper camel case Json file
            // and reload before reading collection
            DataStore = dataBaseName == null 
                ? new DataStore($@"{DataBaseName}.json", false, null, true) 
                : new DataStore($@"{dataBaseName}.json", false, null, true);

            DataBaseName = dataBaseName ?? DataBaseName;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.File(@$"logs\{DataBaseName}.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            IsRunning = true;
        }
        catch (Exception e)
        {
            Log.Error($"DataBase.File initializing: {e}");

            IsRunning = false;
        }
    }

    /// <summary>
    /// Insert json 
    /// </summary>
    /// <param name="json">This entity</param>
    /// <returns>true if inserted</returns>
    public async Task<bool> InsertAsync(string json)
    {
        try
        {
            var plant = JToken.Parse(json);
            var collection = DataStore?.GetCollection(CollectionName);

            // check if entry stored
            var entry = await Task.Run(() => collection?
                .AsQueryable()
                .Count(d => d.PlantId == plant.Value<string>("PlantId")));

            // only one entry allowed
            return entry == 0 && await collection?.InsertOneAsync(plant)!;
        }
        catch (Exception e)
        {
            Log.Error($"DataBase.File.InsertAsync: {e}");
                
            return false;
        }
    }

    /// <summary>
    /// Read json
    /// </summary>
    /// <param name="json">Read this entity</param>
    /// <returns>Json as dynamic</returns>
    public async Task<dynamic?> ReadAsync(string json)
    {
        try
        {
            var plant = JToken.Parse(json);
            var collection = DataStore?.GetCollection(CollectionName);
                
            return await Task.Run(() => collection?
                .AsQueryable()
                .FirstOrDefault(d => d.PlantId == plant.Value<string>("PlantId")));
        }
        catch (Exception e)
        {
            Log.Error($"DataBase.File.ReadAsync: {e}");

            return null;
        }
    }

    /// <summary>
    /// Read json
    /// </summary>
    /// <param name="plantId">Search for PlantId</param>
    /// <returns>Json as dynamic</returns>
    public async Task<dynamic?> ReadByPlantIdAsync(string plantId)
    {
        try
        {
            var collection = DataStore?.GetCollection(CollectionName);
                
            return await Task.Run(() => collection?
                .AsQueryable()
                .FirstOrDefault(d => d.PlantId == plantId));
        }
        catch (Exception e)
        {
            Log.Error($"DataBase.File.ReadByPlantIdAsync: {e}");

            return null;
        }
    }

    /// <summary>
    /// Update json
    /// </summary>
    /// <param name="json">This entity</param>
    /// <returns>true id updated</returns>
    public async Task<bool> UpdateAsync(string json)
    {
        try
        {
            var plant = JToken.Parse(json);
            var collection = DataStore?.GetCollection(CollectionName);

            return await collection?
                .UpdateOneAsync(d => d.PlantId == plant.Value<string>("PlantId"), plant)!;
        }
        catch (Exception e)
        {
            Log.Error($"DataBase.File.UpdateAsync: {e}");
                
            return false;
        }
    }

    /// <summary>
    /// Delete json
    /// </summary>
    /// <param name="json">This entity</param>
    /// <returns>true if deleted</returns>
    public async Task<bool> DeleteAsync(string json)
    {
        try
        {
            var plant = JToken.Parse(json);
            var collection = DataStore?.GetCollection(CollectionName);

            return await collection?
                .DeleteOneAsync(d => d.PlantId == plant.Value<string>("PlantId"))!;
        }
        catch (Exception e)
        {
            Log.Error($"DataBase.File.DeleteAsync: {e}");
                
            return false;
        }
    }

    /// <summary>
    /// Delete json
    /// </summary>
    /// <param name="plantId">Search for plantId</param>
    /// <returns>true if deleted</returns>
    public async Task<bool> DeleteByPlantIdAsync(string plantId)
    {
        try
        {
            var collection = DataStore?.GetCollection(CollectionName);

            return await collection?
                .DeleteOneAsync(d => d.PlantId == plantId)!;
        }
        catch (Exception e)
        {
            Log.Error($"DataBase.File.DeleteByPlantIdAsync: {e}");
                
            return false;
        }
    }

    /// <summary>
    /// Dispose file store
    /// </summary>
    public void Dispose()
    {
        DataStore?.Dispose();
    }
}