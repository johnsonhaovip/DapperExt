using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNYFrame_Test.PerformanceTest
{
    public interface IPerformanceTest
    {
        string TestName { get; }

        int InsertData(int num);

        int GetData(int num = 0);

        bool BulkCopy();
    }
}
