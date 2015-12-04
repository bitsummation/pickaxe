using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    internal class RetryHttpRequest : HttpRequest
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int RetryCount = 5;

        private int _errorCount;

        public RetryHttpRequest(string url)
            : base(url)
        {
            _errorCount = 0;
        }

        protected override bool OnError(DownloadError error)
        {
            if (_errorCount == 0)
                Log.InfoFormat("Failed download, Url = {0}, Message = {1}", Url, error.Message);
            else
                Log.InfoFormat("Retry failed download, Url = {0}, Message = {1}", Url, error.Message);
            
            _errorCount++;
            return (_errorCount < RetryCount);
        }
    }
}
