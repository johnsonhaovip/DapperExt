using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.PagedList {
    /*
     ASP.NET MvcPager 分页组件
     Copyright:2009-2013 陕西省延安市吴起县 杨涛\Webdiyer (http://www.webdiyer.com)
     Source code released under Ms-PL license
     */
    public interface IPagedList : IEnumerable {
        int CurrentPageIndex { get; set; }
        int PageSize { get; set; }
        int TotalItemCount { get; set; }
        int TotalPageCount { get; }
    }
    public interface IPagedList<T> : IEnumerable<T>, IPagedList { }
}
