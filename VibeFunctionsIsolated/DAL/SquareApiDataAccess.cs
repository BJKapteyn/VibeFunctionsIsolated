using Microsoft.Extensions.Logging;
using Square;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeFunctionsIsolated.DAL
{
    public class SquareApiDataAccess : SquareDataAcess
    {
        #region Private Members

        private readonly ILogger<SquareSdkDataAccess> logger;
        private SquareClient squareClient { get; }
        #endregion
        public SquareApiDataAccess(ILogger<SquareSdkDataAccess> logger)
        {
            this.logger = logger;
            squareClient = InitializeClient();
        }
    }
}
