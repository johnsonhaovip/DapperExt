using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Infrastructure {

    /// <summary>
    /// 依赖注入接口
    /// </summary>
    public interface IDependencyRegistrar {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);
        int Order { get; }
    }
}
