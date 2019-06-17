using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.UI.Paging {
    internal class PagerItem {
        public PagerItem(string text, int pageIndex, bool disabled, PagerItemType type) {
            Text = text;
            PageIndex = pageIndex;
            Disabled = disabled;
            Type = type;
        }
        internal string Text { get; set; }
        internal int PageIndex { get; set; }
        internal bool Disabled { get; set; }
        internal PagerItemType Type { get; set; }
    }
    internal enum PagerItemType : byte {
        FirstPage,
        NextPage,
        PrevPage,
        LastPage,
        MorePage,
        NumericPage
    }
}
