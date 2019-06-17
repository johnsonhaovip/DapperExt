using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.PagedList {
    public class PageOption {
        int pageIndex = 1;

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex {
            get { return pageIndex; }
            set { pageIndex = value; }
        }
        int pageSize = 10;

        /// <summary>
        /// 分页大小。默认10条每页
        /// </summary>
        public int PageSize {
            get { return pageSize; }
            set { pageSize = value; }
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 升序
        /// </summary>
        public bool Ascending { get; set; }
    }
}
