using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace XNY.Helper.Extensions
{
    /// <summary>
    /// 將數據導出到Excel
    /// </summary>
    /// 创建时间：2018-4-19
    public class ExportToExcel
    {
        private bool _ApplyStyle;
        private string _CharSet;
        private string _ColumsToExclude;
        private string _ContentEncoding;
        private bool _EnableHyperLinks;
        private int _EndRow;
        private string _FileName;
        private int _StartRow;

        public ExportToExcel()
        {
            this.InternalConstructor();
        }

        public ExportToExcel(string filename)
            : this()
        {
            this.FileName = filename;
        }

        /// <summary>
        /// 將DataTable數據導出到Excel
        /// </summary>
        /// <param name="dt">DataTable</param>
        public void Export(DataTable dt)
        {
            TableCell tc;
            if (dt == null)
            {
                throw new Exception("DataTable Obejec Is Null");
            }
            HttpResponse response = this.SetContent();
            Table table = new Table();
            string[] ExcludeColumns = this.ColumsToExclude.Split(new char[] { ',' });
            for (int i = 0; i < ExcludeColumns.Length; i++)
            {
                if (ExcludeColumns[i] != "")
                {
                    dt.Columns.RemoveAt(int.Parse(ExcludeColumns[i]));
                }
            }
            TableRow tr = new TableRow();
            foreach (DataColumn dc in dt.Columns)
            {
                tc = new TableCell();
                tc.Text = dc.ColumnName;
                tr.Cells.Add(tc);
            }
            table.Rows.Add(tr);
            int k = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if ((k >= this.StartRow) && (k <= this.EndRow))
                {
                    TableRow trr = new TableRow();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        tc = new TableCell();
                        tc.Text = dr[dc].ToString();
                        //增加網格線
                        //if (k == this.EndRow)
                        //{

                        //}
                        //else
                        //{
                        //    tc.Attributes.Add("style", "");
                        //}

                        trr.Cells.Add(tc);
                    }
                    table.Rows.Add(trr);
                }
                k++;
            }
            this.SendExcelToResponse(response, table);
        }

        /// <summary>
        /// 將DataGrid數據導出到Excel
        /// </summary>
        /// <param name="dg">DataGrid</param>
        public void Export(DataGrid dg)
        {
            if (dg == null)
            {
                throw new Exception("DataGrid Obejec Is Null");
            }
            if (HttpContext.Current == null)
            {
                throw new Exception("Not In A HttpContent");
            }
            HttpResponse response = this.SetContent();
            this.PrepareDataGridForExport(dg);
            string[] ExcludeColumns = this.ColumsToExclude.Split(new char[] { ',' });
            for (int i = 0; i < ExcludeColumns.Length; i++)
            {
                if (ExcludeColumns[i] != "")
                {
                    int column = int.Parse(ExcludeColumns[i]);
                    if (dg.ShowHeader)
                    {
                        (dg.Controls[0].Controls[0] as TableRow).Cells[column].Visible = false;
                    }
                    if (dg.ShowFooter)
                    {
                        (dg.Controls[0].Controls[dg.Controls[0].Controls.Count - 1] as TableRow).Cells[column].Visible = false;
                    }
                    foreach (DataGridItem dgi in dg.Items)
                    {
                        dgi.Cells[column].Visible = false;
                    }
                }
            }
            int k = 0;
            foreach (DataGridItem dgi in dg.Items)
            {
                if ((k < this.StartRow) || (k > this.EndRow))
                {
                    dgi.Visible = false;
                }
                k++;
            }
            this.SendExcelToResponse(response, dg);
        }

        /// <summary>
        /// 將GridView數據導出到Excel
        /// </summary>
        /// <param name="gv"></param>
        public void Export(GridView gv)
        {
            if (gv == null)
            {
                throw new Exception("GridView Obejec Is Null");
            }
            if (HttpContext.Current == null)
            {
                throw new Exception("Not In A HttpContent");
            }
            HttpResponse response = this.SetContent();
            this.PrepareDataGridForExport(gv);
            string[] ExcludeColumns = this.ColumsToExclude.Split(new char[] { ',' });
            for (int i = 0; i < ExcludeColumns.Length; i++)
            {
                if (ExcludeColumns[i] != "")
                {
                    int column = int.Parse(ExcludeColumns[i]);
                    if (gv.ShowHeader)
                    {
                        gv.HeaderRow.Cells[column].Visible = false;
                    }
                    if (gv.ShowFooter)
                    {
                        gv.FooterRow.Cells[column].Visible = false;
                    }
                    foreach (GridViewRow gvr in gv.Rows)
                    {
                        gvr.Cells[column].Visible = false;
                    }
                }
            }
            int k = 0;
            foreach (GridViewRow gvr in gv.Rows)
            {
                if ((k < this.StartRow) || (k > this.EndRow))
                {
                    gvr.Visible = false;
                }
                k++;
            }
            this.SendExcelToResponse(response, gv);
        }

        /// <summary>
        /// 將DataSet[TableIndex]數據導出到Excel
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="TableIndex"></param>
        public void Export(DataSet ds, int TableIndex)
        {
            if (ds == null)
            {
                throw new Exception("DataSet Obejec Is Null");
            }
            if ((TableIndex < 0) || (TableIndex > (ds.Tables.Count - 1)))
            {
                throw new Exception("Table Index IN DataSet Is Out Of Range");
            }
            this.Export(ds.Tables[TableIndex]);
        }

        /// <summary>
        /// 將DataTable+enColumnName+cnColumnName數據導出到Excel
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="enColumnName">需要導出的表頭數據(新增)</param>
        /// <param name="cnColumnName">需要導出的數據(新增)</param>
        public void ExportDt(DataTable dt, string[] enColumnName, string[] cnColumnName)
        {
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                if (enColumnName.Length != cnColumnName.Length)
                {
                    throw new Exception("The Number Of Columns does not Match the HeadText");
                }
                GridView gv = new GridView();
                gv.DataSource = dt.DefaultView;
                gv.AutoGenerateColumns = false;
                gv.AllowPaging = false;
                gv.HeaderStyle.BackColor = Color.LightGray;
                gv.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                gv.HeaderStyle.Font.Bold = true;
                for (int i = 0; i < enColumnName.Length; i++)
                {
                    BoundField dgc = new BoundField();
                    dgc.DataField = enColumnName[i];
                    dgc.HeaderText = cnColumnName[i];
                    gv.Columns.Add(dgc);
                }
                gv.RowDataBound += delegate (object obj, GridViewRowEventArgs e)
                {
                    e.Row.Cells[0].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
                    if (e.Row.Cells.Count > 7)
                    {
                        e.Row.Cells[7].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
                    }
                };
                gv.DataBind();
                this.ExportGv(gv);
            }
        }

        public void ExportGv(GridView gv)
        {
            HttpResponse response = this.SetContent();
            this.SendExcelToResponse(response, gv);
        }

        public void Exportrpt(Repeater rpt)
        {
            HttpResponse response = this.SetContent();
            this.SendExcelToResponse(response, rpt);
        }

        private void InternalConstructor()
        {
            this.FileName = "YourData.xls";
            this.CharSet = "UTF-8";
            this.ContentEncoding = "UTF-8";
            this.ColumsToExclude = "";
            this.EnableHyperLinks = false;
            this.StartRow = 0;
            this.EndRow = 0x7fffffff;//32位最大带符号整数,2147483647
        }

        protected void PrepareDataGridForExport(Control controls)
        {
            int count = controls.Controls.Count;
            for (int i = 0; i < count; i++)
            {
                Control control = controls.Controls[i];
                if (control is LinkButton)
                {
                    controls.Controls.Remove(control);
                    controls.Controls.AddAt(i, new LiteralControl(((LinkButton)control).Text));
                }
                else if (control is DropDownList)
                {
                    controls.Controls.Remove(control);
                    controls.Controls.AddAt(i, new LiteralControl(((DropDownList)control).SelectedItem.Text));
                }
                else if (control is HyperLink)
                {
                    if (!this.EnableHyperLinks)
                    {
                        controls.Controls.Remove(control);
                        controls.Controls.AddAt(i, new LiteralControl(((HyperLink)control).Text));
                    }
                }
                else if (control is HtmlAnchor)
                {
                    if (!this.EnableHyperLinks)
                    {
                        controls.Controls.Remove(control);
                        controls.Controls.AddAt(i, new LiteralControl(((HtmlAnchor)control).InnerText));
                    }
                }
                else if (control is CheckBox)
                {
                    controls.Controls.Remove(control);
                    controls.Controls.AddAt(i, new LiteralControl(Convert.ToString(((CheckBox)control).Checked)));
                }
                else if (control is ImageButton)
                {
                    controls.Controls.Remove(control);
                    controls.Controls.AddAt(i, new LiteralControl(((ImageButton)control).AlternateText));
                }
                else if (control is HtmlInputCheckBox)
                {
                    controls.Controls.Remove(control);
                    controls.Controls.AddAt(i, new LiteralControl(Convert.ToString(((HtmlInputCheckBox)control).Checked)));
                }
                else if (control.HasControls())
                {
                    this.PrepareDataGridForExport(control);
                }
            }
        }

        protected void SendExcelToResponse(HttpResponse response, Control C)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<head><style>table td,th{vnd.ms-excel.numberformat:@;border-collapse:collapse;border: 1px solid black;} </style></head>");//导出到execl里的格式化为字符类型，添加網格線          

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            C.RenderControl(htw);
            response.Write("<meta http-equiv=Content-Type content=\"text/html; charset=UTF-8\">");//解决导出乱码
            sb.Append(sw.ToString());

            // response.Write(sw);
            sw.Close();
            response.Write(sb.ToString());
            response.End();
        }




        protected HttpResponse SetContent()
        {
            HttpResponse response = null;
            if (HttpContext.Current == null)
            {
                throw new Exception("Not In A HttpContent");
            }
            response = HttpContext.Current.Response;
            response.Clear();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", this.FileName));
            response.AddHeader("Content-Type", "text/html;charset=UTF-8");
            response.ContentType = "application/vnd-excel";
            try
            {
                response.ContentEncoding = Encoding.GetEncoding(this.ContentEncoding);
            }
            catch (Exception)
            {
                throw new Exception("Invaild Encoding Name " + this.ContentEncoding);
            }
            try
            {
                response.Charset = this.CharSet;
            }
            catch (Exception)
            {
                throw new Exception("Invaild CharSet " + this.CharSet);
            }
            return response;
        }

        public bool ApplyStyle
        {
            get
            {
                return this._ApplyStyle;
            }
            set
            {
                try
                {
                    this._ApplyStyle = value;
                }
                catch (Exception)
                {
                    this._ApplyStyle = false;
                }
            }
        }

        public string CharSet
        {
            get
            {
                return this._CharSet;
            }
            set
            {
                this._CharSet = value;
            }
        }

        /// <summary>
        /// 不顯示的欄位
        /// </summary>
        public string ColumsToExclude
        {
            get
            {
                return this._ColumsToExclude;
            }
            set
            {
                this._ColumsToExclude = value;
            }
        }

        public string ContentEncoding
        {
            get
            {
                return this._ContentEncoding;
            }
            set
            {
                this._ContentEncoding = "utf-8";
            }
        }

        /// <summary>
        /// 超級鏈接是否能跳轉
        /// True為能
        /// </summary>
        public bool EnableHyperLinks
        {
            get
            {
                return this._EnableHyperLinks;
            }
            set
            {
                try
                {
                    this._EnableHyperLinks = value;
                }
                catch (Exception)
                {
                    this._EnableHyperLinks = false;
                }
            }
        }

        /// <summary>
        ///  導出結束的行號，默認為2147483647
        /// </summary>
        public int EndRow
        {
            get
            {
                return this._EndRow;
            }
            set
            {
                this._EndRow = value;
            }
        }

        /// <summary>
        /// 導出後的文件名
        /// </summary>
        public string FileName
        {
            get
            {
                return this._FileName;
            }
            set
            {
                this._FileName = value;
            }
        }

        /// <summary>
        /// 開始導出的行號，默認為0
        /// </summary>
        public int StartRow
        {
            get
            {
                return this._StartRow;
            }
            set
            {
                this._StartRow = value;
            }
        }
    }
}
