﻿using Square.Models;
using VibeCollectiveFunctions.Models;

namespace VibeCollectiveFunctions.Utility
{
    internal interface ISquareUtility
    {
        public Task<T?> DeserializeStream<T>(Stream body);
        public IEnumerable<SquareItem>? MapSquareProductItems(SearchCatalogObjectsResponse response, string type);

    }
}
