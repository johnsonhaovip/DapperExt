using System;
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
    public class PagedList<T> : List<T>, IPagedList<T> {
        /// <summary>
        /// 使用要分页的所有数据项、当前页索引和每页显示的记录数初始化PagedList对象
        /// </summary>
        /// <param name="allItems"></param>
        /// <param name="option"></param>
        public PagedList(IEnumerable<T> allItems, PageOption option) {
            PageSize = option.PageSize;
            var items = allItems as IList<T> ?? allItems.ToList();
            TotalItemCount = items.Count();
            CurrentPageIndex = option.PageIndex;
            AddRange(items.Skip(StartRecordIndex - 1).Take(option.PageSize));
        }

        /// <summary>
        /// 使用要分页的所有数据项、当前页索引和每页显示的记录数初始化PagedList对象
        /// </summary>
        /// <param name="allItems">要分页的所有数据项</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">每页显示的记录数</param>
        public PagedList(IEnumerable<T> allItems, int pageIndex, int pageSize) {
            PageSize = pageSize;
            var items = allItems as IList<T> ?? allItems.ToList();
            TotalItemCount = items.Count();
            CurrentPageIndex = pageIndex;
            AddRange(items.Skip(StartRecordIndex - 1).Take(pageSize));
        }
        /// <summary>
        /// 使用当前页数据项、当前页索引、每页显示记录数和要分页的总记录数初始化PagedList对象
        /// </summary>
        /// <param name="currentPageItems">总前页数据项</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="totalItemCount">要分页的总记录数</param>
        public PagedList(IEnumerable<T> currentPageItems, int pageIndex, int pageSize, int totalItemCount) {
            AddRange(currentPageItems);
            TotalItemCount = totalItemCount;
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
        }
        /// <summary>
        /// 使用要分页的所有数据项、当前页索引和每页显示的记录数初始化PagedList对象
        /// </summary>
        /// <param name="allItems">要分页的所有数据项</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">每页显示的记录数</param>
        public PagedList(IQueryable<T> allItems, int pageIndex, int pageSize) {
            int startIndex = (pageIndex - 1) * pageSize;
            AddRange(allItems.Skip(startIndex).Take(pageSize));
            TotalItemCount = allItems.Count();
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
        }
        /// <summary>
        /// 使用当前页数据项、当前页索引、每页显示记录数和要分页的总记录数初始化PagedList对象
        /// </summary>
        /// <param name="currentPageItems">总前页数据项</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="totalItemCount">要分页的总记录数</param>
        public PagedList(IQueryable<T> currentPageItems, int pageIndex, int pageSize, int totalItemCount) {
            AddRange(currentPageItems);
            TotalItemCount = totalItemCount;
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
        }
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalPageCount {
            get {
                var pageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
                if (pageCount < 1)
                    pageCount = 1;
                return pageCount;
            }
        }
        public int StartRecordIndex { get { return (CurrentPageIndex - 1) * PageSize + 1; } }
        public int EndRecordIndex { get { return TotalItemCount > CurrentPageIndex * PageSize ? CurrentPageIndex * PageSize : TotalItemCount; } }
    }
}
