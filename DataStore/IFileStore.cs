namespace DataBase
{
    public interface IFileStore
    {
        bool IsRunning { get; set; }

        /// <summary>
        /// Insert json 
        /// </summary>
        /// <param name="json">This entity</param>
        /// <returns>true if inserted</returns>
        Task<bool> InsertAsync(string json);

        /// <summary>
        /// Read json
        /// </summary>
        /// <param name="json">Read this entity</param>
        /// <returns>Json as dynamic</returns>
        Task<dynamic?> ReadAsync(string json);

        /// <summary>
        /// Read json
        /// </summary>
        /// <param name="plantId">Search for PlantId</param>
        /// <returns>Json as dynamic</returns>
        Task<dynamic?> ReadByPlantIdAsync(string plantId);

        /// <summary>
        /// Update json
        /// </summary>
        /// <param name="json">This entity</param>
        /// <returns>true id updated</returns>
        Task<bool> UpdateAsync(string json);

        /// <summary>
        /// Delete json
        /// </summary>
        /// <param name="json">This entity</param>
        /// <returns>true if deleted</returns>
        Task<bool> DeleteAsync(string json);

        /// <summary>
        /// Delete json
        /// </summary>
        /// <param name="plantId">Search for plantId</param>
        /// <returns>true if deleted</returns>
        Task<bool> DeleteByPlantIdAsync(string plantId);

        /// <summary>
        /// Dispose file store
        /// </summary>
        void Dispose();
    }
}
