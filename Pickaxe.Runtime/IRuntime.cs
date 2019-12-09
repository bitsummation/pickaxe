using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pickaxe.Runtime
{
    public interface IRuntime
    {
        IHttpRequestFactory RequestFactory { get; }
        int TotalOperations { get; set; }
        void Call(int line);
        void OnProgress();

        Thread ExecutingThread { get; set; }
        IList<Thread> DownloadThreads { get; set;}
    }
}
