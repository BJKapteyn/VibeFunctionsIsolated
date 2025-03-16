namespace VibeFunctionsIsolated.Utility.UtilityInterfaces
{
    public interface IApplicationUtility
    {
        /// <summary>
        /// Deserialize into api model
        /// </summary>
        /// <typeparam name="T">The type of model to deserialize into</typeparam>
        /// <param name="body">Body of the response as a stream</param>
        /// <returns>Deserialized object</returns>
        public Task<T?> DeserializeStream<T>(Stream body);
    }
}