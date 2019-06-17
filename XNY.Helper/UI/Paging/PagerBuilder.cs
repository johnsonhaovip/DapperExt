using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace XNY.Helper.UI.Paging {
    internal class PagerBuilder {
        private readonly HtmlHelper _html;
        private readonly AjaxHelper _ajax;
        private readonly string _actionName;
        private readonly string _controllerName;
        private readonly int _totalPageCount = 1;
        private readonly int _pageIndex;
        private readonly PagerOptions _pagerOptions;
        private readonly RouteValueDictionary _routeValues;
        private readonly string _routeName;
        private readonly int _startPageIndex = 1;
        private readonly int _endPageIndex = 1;
        private readonly bool _ajaxPagingEnabled;
        private readonly MvcAjaxOptions _ajaxOptions;
        private IDictionary<string, object> _htmlAttributes;
        private const string CopyrightText = "\r\n<!--MvcPager2.0-->\r\n";
        /// <summary>
        /// 适用于PagedList为null时
        /// </summary>
        internal PagerBuilder(HtmlHelper html, AjaxHelper ajax, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes) {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            _html = html;
            _ajax = ajax;
            _pagerOptions = pagerOptions;
            _htmlAttributes = htmlAttributes;
        }
        internal PagerBuilder(HtmlHelper html, AjaxHelper ajax, string actionName, string controllerName,
            int totalPageCount, int pageIndex, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues,
            MvcAjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes) {
            _ajaxPagingEnabled = (ajax != null);
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            _html = html;
            _ajax = ajax;
            _actionName = actionName;
            _controllerName = controllerName;
            if (pagerOptions.MaxPageIndex == 0 || pagerOptions.MaxPageIndex > totalPageCount)
                _totalPageCount = totalPageCount;
            else
                _totalPageCount = pagerOptions.MaxPageIndex;
            _pageIndex = pageIndex;
            _pagerOptions = pagerOptions;
            _routeName = routeName;
            _routeValues = routeValues;
            _ajaxOptions = ajaxOptions;
            _htmlAttributes = htmlAttributes;
            // start page index
            _startPageIndex = pageIndex - (pagerOptions.NumericPagerItemCount / 2);
            if (_startPageIndex + pagerOptions.NumericPagerItemCount > _totalPageCount)
                _startPageIndex = _totalPageCount + 1 - pagerOptions.NumericPagerItemCount;
            if (_startPageIndex < 1)
                _startPageIndex = 1;
            // end page index
            _endPageIndex = _startPageIndex + _pagerOptions.NumericPagerItemCount - 1;
            if (_endPageIndex > _totalPageCount)
                _endPageIndex = _totalPageCount;
        }
        //non Ajax pager builder
        internal PagerBuilder(HtmlHelper helper, string actionName, string controllerName, int totalPageCount,
            int pageIndex, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes)
            : this(helper, null, actionName, controllerName,
                totalPageCount, pageIndex, pagerOptions, routeName, routeValues, null, htmlAttributes) { }
        //Ajax pager builder
        internal PagerBuilder(AjaxHelper helper, string actionName, string controllerName, int totalPageCount,
            int pageIndex, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues,
            MvcAjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
            : this(null, helper, actionName,
                controllerName, totalPageCount, pageIndex, pagerOptions, routeName, routeValues, ajaxOptions, htmlAttributes) { }

        private void AddPrevious(ICollection<PagerItem> results) {
            var item = new PagerItem(_pagerOptions.PrevPageText, _pageIndex - 1, _pageIndex == 1, PagerItemType.PrevPage);
            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }
        private void AddFirst(ICollection<PagerItem> results) {
            var item = new PagerItem(_pagerOptions.FirstPageText, 1, _pageIndex == 1, PagerItemType.FirstPage);
            //只有导航按钮未被禁用，或导航按钮被禁用但ShowDisabledPagerItems=true时才添加
            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }
        private void AddMoreBefore(ICollection<PagerItem> results) {
            if (_startPageIndex > 1 && _pagerOptions.ShowMorePagerItems) {
                var index = _startPageIndex - 1;
                if (index < 1) index = 1;
                var item = new PagerItem(_pagerOptions.MorePageText, index, false, PagerItemType.MorePage);
                results.Add(item);
            }
        }
        private void AddPageNumbers(ICollection<PagerItem> results) {
            for (var pageIndex = _startPageIndex; pageIndex <= _endPageIndex; pageIndex++) {
                var text = pageIndex.ToString(CultureInfo.InvariantCulture);
                if (pageIndex == _pageIndex && !string.IsNullOrEmpty(_pagerOptions.CurrentPageNumberFormatString))
                    text = string.Format(_pagerOptions.CurrentPageNumberFormatString, text);
                else if (!string.IsNullOrEmpty(_pagerOptions.PageNumberFormatString))
                    text = string.Format(_pagerOptions.PageNumberFormatString, text);
                var item = new PagerItem(text, pageIndex, false, PagerItemType.NumericPage);
                results.Add(item);
            }
        }
        private void AddMoreAfter(ICollection<PagerItem> results) {
            if (_endPageIndex < _totalPageCount) {
                var index = _startPageIndex + _pagerOptions.NumericPagerItemCount;
                if (index > _totalPageCount) { index = _totalPageCount; }
                var item = new PagerItem(_pagerOptions.MorePageText, index, false, PagerItemType.MorePage);
                results.Add(item);
            }
        }
        private void AddNext(ICollection<PagerItem> results) {
            var item = new PagerItem(_pagerOptions.NextPageText, _pageIndex + 1, _pageIndex >= _totalPageCount, PagerItemType.NextPage);
            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }
        private void AddLast(ICollection<PagerItem> results) {
            var item = new PagerItem(_pagerOptions.LastPageText, _totalPageCount, _pageIndex >= _totalPageCount, PagerItemType.LastPage);
            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }
        /// <summary>
        /// 根据页索引生成分页导航Url
        /// </summary>
        /// <param name="pageIndex">要生成导航链接的页索引</param>
        /// <returns>分页导航链接Url</returns>
        private string GenerateUrl(int pageIndex) {
            ViewContext viewContext = _ajax == null ? _html.ViewContext : _ajax.ViewContext;
            //若要生成url的页索引小于1或大于总页数或等于当前页索引时，无需生成分页导航链接
            if (pageIndex > _totalPageCount || pageIndex == _pageIndex)
                return null;
            var routeValues = new RouteValueDictionary(viewContext.RouteData.Values);
            AddQueryStringToRouteValues(routeValues, viewContext);
            if (_routeValues != null && _routeValues.Count > 0) {
                foreach (var de in _routeValues) {
                    if (!routeValues.ContainsKey(de.Key)) {
                        routeValues.Add(de.Key, de.Value);
                    } else {
                        routeValues[de.Key] = de.Value; //手动添加的RouteValues具有高优先级
                    }
                }
            }
            var pageValue = viewContext.RouteData.Values[_pagerOptions.PageIndexParameterName];
            string routeName = _routeName;
            // 设置Route Value中的分页导航Url参数值，pageIndex为0时生成适用于脚本的导航链接
            if (pageIndex == 0)
                routeValues[_pagerOptions.PageIndexParameterName] = "__" + _pagerOptions.PageIndexParameterName + "__";
            else {
                if (pageIndex == 1) {
                    if (!string.IsNullOrWhiteSpace(_pagerOptions.FirstPageRouteName))
                    //如果显式指定了FirstPageRouteName，则使用此Route
                    {
                        routeName = _pagerOptions.FirstPageRouteName;
                        routeValues.Remove(_pagerOptions.PageIndexParameterName); //去除页索引参数
                        viewContext.RouteData.Values.Remove(_pagerOptions.PageIndexParameterName);
                    } else {
                        var curRoute = viewContext.RouteData.Route as Route;
                        //判断当前Route是否为Route类型，如果是，则判断该Route中页索引参数默认值是否为UrlParameter.Optional，或页索引参数是否存在于该Route Url的参数列表中，如果参数默认值为UrlParameter.Optional或分页参数名不存在于Route Url参数中，则从当前的RouteValue列表中去除该参数
                        if (curRoute != null &&
                            (curRoute.Defaults[_pagerOptions.PageIndexParameterName] == UrlParameter.Optional ||
                             !curRoute.Url.Contains("{" + _pagerOptions.PageIndexParameterName + "}"))) {
                            routeValues.Remove(_pagerOptions.PageIndexParameterName); //去除页索引参数
                            viewContext.RouteData.Values.Remove(_pagerOptions.PageIndexParameterName);
                        } else {
                            routeValues[_pagerOptions.PageIndexParameterName] = pageIndex;
                        }
                    }
                } else {
                    routeValues[_pagerOptions.PageIndexParameterName] = pageIndex;
                }
            }
            var routes = _ajax == null ? _html.RouteCollection : _ajax.RouteCollection;
            string url;
            if (!string.IsNullOrEmpty(routeName))
                url = UrlHelper.GenerateUrl(routeName, _actionName, _controllerName, routeValues, routes,
                                             viewContext.RequestContext, false);
            else
                url = UrlHelper.GenerateUrl(null, _actionName, _controllerName, routeValues, routes,
                                             viewContext.RequestContext, false);
            if (pageValue != null)
                viewContext.RouteData.Values[_pagerOptions.PageIndexParameterName] = pageValue;
            return url;
        }
        /// <summary>
        /// 生成最终的分页Html代码
        /// </summary>
        /// <returns></returns>
        internal MvcHtmlString RenderPager() {
            //return null if total page count less than or equal to 1
            if (_totalPageCount <= 1 && _pagerOptions.AutoHide)
                return MvcHtmlString.Create(CopyrightText);
            //Display error message if pageIndex out of range
            if ((_pageIndex > _totalPageCount && _totalPageCount > 0) || _pageIndex < 1) {
                return
                    MvcHtmlString.Create(string.Format("{0}<div style=\"color:red;font-weight:bold\">{1}</div>{0}",
                                         CopyrightText, _pagerOptions.PageIndexOutOfRangeErrorMessage));
            }

            var pagerItems = new List<PagerItem>();
            //First page
            if (_pagerOptions.ShowFirstLast)
                AddFirst(pagerItems);
            // Prev page
            if (_pagerOptions.ShowPrevNext)
                AddPrevious(pagerItems);
            if (_pagerOptions.ShowNumericPagerItems) {
                if (_pagerOptions.AlwaysShowFirstLastPageNumber && _startPageIndex > 1)
                    pagerItems.Add(new PagerItem("1", 1, false, PagerItemType.NumericPage));
                // more page before numeric page buttons
                if (_pagerOptions.ShowMorePagerItems && ((!_pagerOptions.AlwaysShowFirstLastPageNumber && _startPageIndex > 1) || (_pagerOptions.AlwaysShowFirstLastPageNumber && _startPageIndex > 2)))
                    AddMoreBefore(pagerItems);
                // numeric page
                AddPageNumbers(pagerItems);
                // more page after numeric page buttons
                if (_pagerOptions.ShowMorePagerItems && ((!_pagerOptions.AlwaysShowFirstLastPageNumber && _endPageIndex < _totalPageCount) || (_pagerOptions.AlwaysShowFirstLastPageNumber && _totalPageCount > _endPageIndex + 1)))
                    AddMoreAfter(pagerItems);
                if (_pagerOptions.AlwaysShowFirstLastPageNumber && _endPageIndex < _totalPageCount)
                    pagerItems.Add(new PagerItem(_totalPageCount.ToString(CultureInfo.InvariantCulture), _totalPageCount, false,
                                                 PagerItemType.NumericPage));
            }
            // Next page
            if (_pagerOptions.ShowPrevNext)
                AddNext(pagerItems);
            //Last page
            if (_pagerOptions.ShowFirstLast)
                AddLast(pagerItems);
            var sb = new StringBuilder();
            if (_ajaxPagingEnabled) {
                foreach (PagerItem item in pagerItems) {
                    sb.Append(GenerateAjaxPagerElement(item));
                }
            } else {
                foreach (PagerItem item in pagerItems) {
                    sb.Append(GeneratePagerElement(item));
                }
            }
            var tb = new TagBuilder(_pagerOptions.ContainerTagName);
            if (!string.IsNullOrEmpty(_pagerOptions.Id))
                tb.GenerateId(_pagerOptions.Id);
            if (!string.IsNullOrEmpty(_pagerOptions.CssClass))
                tb.AddCssClass(_pagerOptions.CssClass);
            if (!string.IsNullOrEmpty(_pagerOptions.HorizontalAlign)) {
                string strAlign = "text-align:" + _pagerOptions.HorizontalAlign.ToLower();
                if (_htmlAttributes == null)
                    _htmlAttributes = new RouteValueDictionary { { "style", strAlign } };
                else {
                    if (_htmlAttributes.Keys.Contains("style"))
                        _htmlAttributes["style"] += ";" + strAlign;
                }
            }
            tb.MergeAttributes(_htmlAttributes, true);
            if (_ajaxPagingEnabled) {
                var attrs = _ajaxOptions.ToUnobtrusiveHtmlAttributes();
                attrs.Remove("data-ajax-url");
                attrs.Remove("data-ajax-mode");
                if (_ajaxOptions.EnablePartialLoading)
                    attrs.Add("data-ajax-partialloading", "true");
                if (_pageIndex > 1)
                    attrs.Add("data-ajax-currentpage", _pageIndex);
                if (!string.IsNullOrWhiteSpace(_ajaxOptions.DataFormId))
                    attrs.Add("data-ajax-dataformid", "#" + _ajaxOptions.DataFormId);
                AddDataAttributes(attrs);
                tb.MergeAttributes(attrs, true);
            }
            if (_pagerOptions.ShowPageIndexBox) {
                if (!_ajaxPagingEnabled) {
                    var attrs = new Dictionary<string, object>();
                    AddDataAttributes(attrs);
                    tb.MergeAttributes(attrs, true);
                }
                sb.Append(BuildGoToPageSection());
            } else
                sb.Length -= _pagerOptions.PagerItemsSeperator.Length;
            tb.InnerHtml = sb.ToString();
            /* 注册客户端脚本
            string pagerScript = string.Empty;
            const string ctxItemName = "_MvcPagerScriptRegistered";
            ViewContext viewCtx = _ajaxPagingEnabled ? _ajax.ViewContext : _html.ViewContext;
            if (viewCtx.HttpContext.Items[ctxItemName] == null)
            {
                var page = viewCtx.HttpContext.CurrentHandler as Page;
                var scriptUrl = (page ?? new Page()).ClientScript.GetWebResourceUrl(typeof (PagerHelper),"Webdiyer.WebControls.Mvc.MvcPager.min.js");
                pagerScript = "<script type=\"text/javascript\" src=\"" + scriptUrl + "\"></script>";
                viewCtx.HttpContext.Items[ctxItemName] = true;
            }*/
            return MvcHtmlString.Create(CopyrightText + /*pagerScript +*/ tb.ToString(TagRenderMode.Normal) + CopyrightText);
        }
        private void AddDataAttributes(IDictionary<string, object> attrs) {
            attrs.Add("data-urlformat", GenerateUrl(0));
            attrs.Add("data-mvcpager", "true");
            if (_pageIndex > 1)
                attrs.Add("data-firstpage", GenerateUrl(1));
            attrs.Add("data-pageparameter", _pagerOptions.PageIndexParameterName);
            attrs.Add("data-maxpages", _totalPageCount);
            if (_pagerOptions.ShowPageIndexBox && _pagerOptions.PageIndexBoxType == PageIndexBoxType.TextBox) {
                attrs.Add("data-outrangeerrmsg", _pagerOptions.PageIndexOutOfRangeErrorMessage);
                attrs.Add("data-invalidpageerrmsg", _pagerOptions.InvalidPageIndexErrorMessage);
            }
        }
        private string BuildGoToPageSection() {
            var piBuilder = new StringBuilder();
            if (_pagerOptions.PageIndexBoxType == PageIndexBoxType.DropDownList) {
                // start page index
                int startIndex = _pageIndex - (_pagerOptions.MaximumPageIndexItems / 2);
                if (startIndex + _pagerOptions.MaximumPageIndexItems > _totalPageCount)
                    startIndex = _totalPageCount + 1 - _pagerOptions.MaximumPageIndexItems;
                if (startIndex < 1)
                    startIndex = 1;
                // end page index
                int endIndex = startIndex + _pagerOptions.MaximumPageIndexItems - 1;
                if (endIndex > _totalPageCount)
                    endIndex = _totalPageCount;
                piBuilder.AppendFormat("<select data-pageindexbox=\"true\"{0}>", (_pagerOptions.ShowGoButton ? "" : " data-autosubmit=\"true\""));
                for (int i = startIndex; i <= endIndex; i++) {
                    piBuilder.AppendFormat("<option value=\"{0}\"", i);
                    if (i == _pageIndex)
                        piBuilder.Append(" selected=\"selected\"");
                    piBuilder.AppendFormat(">{0}</option>", i);
                }
                piBuilder.Append("</select>");
            } else
                piBuilder.AppendFormat(
                    "<input type=\"text\" value=\"{0}\" data-pageindexbox=\"true\"{1}/>", _pageIndex, (_pagerOptions.ShowGoButton ? "" : " data-autosubmit=\"true\""));
            string outHtml;
            if (!string.IsNullOrEmpty(_pagerOptions.PageIndexBoxWrapperFormatString)) {
                outHtml = string.Format(_pagerOptions.PageIndexBoxWrapperFormatString, piBuilder);
                piBuilder = new StringBuilder(outHtml);
            }
            if (_pagerOptions.ShowGoButton) {
                piBuilder.AppendFormat("<input type=\"button\" data-submitbutton=\"true\" value=\"{0}\"/>", _pagerOptions.GoButtonText);
            }
            if (!string.IsNullOrEmpty(_pagerOptions.GoToPageSectionWrapperFormatString) ||
                !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString)) {
                outHtml = string.Format(
                    _pagerOptions.GoToPageSectionWrapperFormatString ?? _pagerOptions.PagerItemWrapperFormatString,
                    piBuilder);
            } else
                outHtml = piBuilder.ToString();
            return outHtml;
        }
        private string GenerateAjaxAnchor(PagerItem item) {
            string url = GenerateUrl(item.PageIndex);
            if (string.IsNullOrWhiteSpace(url))
                return HttpUtility.HtmlEncode(item.Text);
            var tag = new TagBuilder("a") { InnerHtml = item.Text };
            tag.MergeAttribute("href", url);
            tag.MergeAttribute("data-pageindex", item.PageIndex.ToString(CultureInfo.InvariantCulture));
            return tag.ToString(TagRenderMode.Normal);
        }
        private MvcHtmlString GeneratePagerElement(PagerItem item) {
            //pager item link
            string url = GenerateUrl(item.PageIndex);
            if (item.Disabled) //first,last,next or previous page
                return CreateWrappedPagerElement(item, string.Format("<a disabled=\"disabled\">{0}</a>", item.Text));
            return CreateWrappedPagerElement(item,
                                             string.IsNullOrEmpty(url)
                                                 ? HttpUtility.HtmlEncode(item.Text)
                                                 : string.Format("<a href=\"{0}\">{1}</a>", url, item.Text));
        }
        private MvcHtmlString GenerateAjaxPagerElement(PagerItem item) {
            if (item.Disabled)
                return CreateWrappedPagerElement(item, string.Format("<a disabled=\"disabled\">{0}</a>", item.Text));
            return CreateWrappedPagerElement(item, GenerateAjaxAnchor(item));
        }
        private MvcHtmlString CreateWrappedPagerElement(PagerItem item, string el) {
            string navStr = el;
            switch (item.Type) {
                case PagerItemType.FirstPage:
                case PagerItemType.LastPage:
                case PagerItemType.NextPage:
                case PagerItemType.PrevPage:
                    if ((!string.IsNullOrEmpty(_pagerOptions.NavigationPagerItemWrapperFormatString) ||
                         !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString)))
                        navStr =
                            string.Format(
                                _pagerOptions.NavigationPagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    break;
                case PagerItemType.MorePage:
                    if ((!string.IsNullOrEmpty(_pagerOptions.MorePagerItemWrapperFormatString) ||
                         !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString)))
                        navStr =
                            string.Format(
                                _pagerOptions.MorePagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    break;
                case PagerItemType.NumericPage:
                    if (item.PageIndex == _pageIndex &&
                        (!string.IsNullOrEmpty(_pagerOptions.CurrentPagerItemWrapperFormatString) ||
                         !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString))) //current page
                        navStr =
                            string.Format(
                                _pagerOptions.CurrentPagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    else if (!string.IsNullOrEmpty(_pagerOptions.NumericPagerItemWrapperFormatString) ||
                             !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString))
                        navStr =
                            string.Format(
                                _pagerOptions.NumericPagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    break;
            }
            return MvcHtmlString.Create(navStr + _pagerOptions.PagerItemsSeperator);
        }

        private void AddQueryStringToRouteValues(RouteValueDictionary routeValues, ViewContext viewContext) {
            if (routeValues == null)
                routeValues = new RouteValueDictionary();
            var rq = viewContext.HttpContext.Request.QueryString;
            if (rq != null && rq.Count > 0) {
                var invalidParams = new[] { "x-requested-with", "xmlhttprequest", _pagerOptions.PageIndexParameterName.ToLower() };
                foreach (string key in rq.Keys) {
                    // 添加url参数到路由中
                    if (!string.IsNullOrEmpty(key) && Array.IndexOf(invalidParams, key.ToLower()) < 0) {
                        var kv = rq[key];
                        routeValues[key] = kv;
                    }
                }
            }
        }
    }
}
