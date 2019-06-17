using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    [Serializable]
    public class Entity
    {
        public object As(object key, string values)
        {

            return values;
        }
    }
}
