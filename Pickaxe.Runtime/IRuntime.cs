using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime
{
    public interface IRuntime
    {
        IHttpRequestFactory RequestFactory { get; }
        int TotalOperations { get; set; }
        void Call(int line);
        void OnProgress();
        bool IsRunning { get; }
    }
}
