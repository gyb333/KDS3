using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Wilmar.Foundation.Projects;
using Wilmar.Model.Core.Definitions;
using Wilmar.Model.Core.Definitions.Screens;
using Wilmar.Model.Core.Definitions.Screens.Controls;
using Wilmar.Service.Common.Generate;

namespace Wilmar.Build.Core.Dojo.Default.Builders
{
    /// <summary>
    /// 网格控件生成器
    /// </summary>
    internal class GridPanelBuild : ContainerBuildBase
    {
        public GridPanelBuild(bool isPreview, ControlHost controlHost, ScreenDefinition screenDef, CompileBase compile, ProjectDocument doc, Dictionary<int, Tuple<int, string>> permissionData, HtmlTextWriter htmlWriter)
            : base(isPreview, controlHost, screenDef, compile, doc, permissionData, htmlWriter)
        {

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        protected override void SetAttributes()
        {
            GridPanel control = this.ControlHost.Content as GridPanel;
            this.HtmlWriter.AddAttribute("dojoType", "Controls/GridPanel");
            if (!IsPreview && !string.IsNullOrEmpty(this.ControlHost.Name))
            {
                this.HtmlWriter.AddAttribute("id", this.ControlHost.Name);
                this.HtmlWriter.AddAttribute("name", this.ControlHost.Name);
            }

            StringBuilder sbProps = new StringBuilder();
            StringBuilder returnContent = new StringBuilder();
            string props = control.BuildControlProps(this.ScreenDefinition, this.IsPreview, this.PermissionData, returnContent);
            if (!string.IsNullOrEmpty(props)) sbProps.AppendFormat("{0},", props);
            if (sbProps.ToString().Length > 0)
            {
                this.HtmlWriter.AddAttribute("data-dojo-props", sbProps.ToString().Substring(0, sbProps.ToString().Length - 1), false);
            }

            base.SetAttributes();
        }
        /// <summary>
        /// 设置子元素
        /// </summary>
        protected override void SetChildElements()
        {
            GridPanel control = (GridPanel)this.ControlHost.Content;

            //行
            List<RowDefinitin> rows = control.RowDefinitins;
            if (rows.Count == 0)
            {
                rows.Add(new RowDefinitin
                {
                    UnitType = EGridUnitType.Star,
                    Height = 1
                });
            }
            //列
            List<ColumnDefinitin> columns = control.ColumnDefinitins;
            if (columns.Count == 0)
            {
                columns.Add(new ColumnDefinitin
                {
                    UnitType = EGridUnitType.Star,
                    Width = 1
                });
            }

            int rowCount = rows.Count();
            int columnCount = columns.Count();

            #region

            Dictionary<int, Tuple<string, int, int, int>> dictRows = new Dictionary<int, Tuple<string, int, int, int>>();
            Dictionary<int, Tuple<string, int, int, int>> dictColumns = new Dictionary<int, Tuple<string, int, int, int>>();
            for (int i = 0; i < rows.Count; i++)
            {
                int height = -1, minHeight = -1, maxHeight = -1;
                if (rows[i].Height != null) height = rows[i].Height.Value;
                if (rows[i].MinHeight != null) minHeight = rows[i].MinHeight.Value;
                if (rows[i].MaxHeight != null) maxHeight = rows[i].MaxHeight.Value;

                string unitType = rows[i].UnitType.ToString().ToLower();
                Tuple<string, int, int, int> tuple = new Tuple<string, int, int, int>(unitType, height, minHeight, maxHeight);
                dictRows.Add(i, tuple);
            }
            for (int i = 0; i < columns.Count; i++)
            {
                int width = -1, minWidth = -1, maxWidth = -1;
                if (columns[i].Width != null) width = columns[i].Width.Value;
                if (columns[i].MinWidth != null) minWidth = columns[i].MinWidth.Value;
                if (columns[i].MaxWidth != null) maxWidth = columns[i].MaxWidth.Value;

                string unitType = columns[i].UnitType.ToString().ToLower();
                Tuple<string, int, int, int> tuple = new Tuple<string, int, int, int>(unitType, width, minWidth, maxWidth);
                dictColumns.Add(i, tuple);
            }

            #endregion

            #region

            bool[,] attachFlags = new bool[rowCount, columnCount];
            ControlHost[,] controlFlags = new ControlHost[rowCount, columnCount];
            foreach (var child in this.ControlHost.Children)
            {
                GridAttach attachObject = child.AttachObject as GridAttach;
                int attachRow = attachObject.Row;
                int attachColumn = attachObject.Column;
                int attachColumnSpan = attachObject.ColumnSpan;

                int flagRow = (attachRow - 1) >= rowCount ? (rowCount - 1) : (attachRow - 1);
                int flagColumn = (attachColumn - 1) >= columnCount ? (columnCount - 1) : (attachColumn - 1);

                controlFlags[flagRow, flagColumn] = child;
                if (attachColumnSpan > 1)
                {
                    for (int i = 1; i < attachColumnSpan; i++)
                    {
                        int tempRow = (attachRow - 1) >= rowCount ? (rowCount - 1) : (attachRow - 1);
                        int tempCol = (attachColumn + i - 1) >= columnCount ? (columnCount - 1) : (attachColumn + i - 1);
                        attachFlags[tempRow, tempCol] = true;
                    }
                }
            }

            #endregion

            #region

            var rowSumWeight = (from t in dictRows
                                where t.Value.Item1.ToLower() == "star" && t.Value.Item2 >= 0
                                select t.Value.Item2).Sum();
            var columnSumWeight = (from t in dictColumns
                                   where t.Value.Item1.ToLower() == "star" && t.Value.Item2 >= 0
                                   select t.Value.Item2).Sum();

            #endregion

            #region

            for (int i = 0; i < rowCount; i++)
            {
                string rowStyle = string.Empty, rowHeight = string.Empty;

                #region
                rowStyle = dictRows[i].Item1;
                rowHeight = dictRows[i].Item2.ToString();
                string minHeight = dictRows[i].Item3 >= 0 ? dictRows[i].Item3.ToString() : "";
                string maxHeight = dictRows[i].Item4 >= 0 ? dictRows[i].Item4.ToString() : "";
                string styleStrHeight = string.Empty;
                if (rowStyle.ToLower() == "star")
                {
                    styleStrHeight = string.Format("flex:{0};", rowHeight);
                    rowHeight = (Convert.ToDouble(rowHeight) / rowSumWeight).ToString("P");
                }
                else if (rowStyle.ToLower() == "pixel")
                {
                    rowHeight = rowHeight + "px";
                    styleStrHeight = string.Format("height:{0};", rowHeight);
                }
                if (!string.IsNullOrEmpty(minHeight))
                {
                    styleStrHeight += string.Format("min-height:{0}px;", minHeight);
                }
                if (!string.IsNullOrEmpty(maxHeight))
                {
                    styleStrHeight += string.Format("max-height:{0}px;", minHeight);
                }
                this.HtmlWriter.AddAttribute("class", "tablerow");
                this.HtmlWriter.AddAttribute("style", "display:flex;" + styleStrHeight);
                #endregion

                this.HtmlWriter.RenderBeginTag("div");

                for (int j = 0; j < columnCount; j++)
                {
                    if (attachFlags[i, j] != true)
                    {
                        string columnStyle = string.Empty, columnWidth = string.Empty;

                        #region
                        columnStyle = dictColumns[j].Item1;
                        columnWidth = dictColumns[j].Item2.ToString();
                        string minWidth = dictColumns[j].Item3 >= 0 ? dictColumns[j].Item3.ToString() : "";
                        string maxWidth = dictColumns[j].Item4 >= 0 ? dictColumns[j].Item4.ToString() : "";
                        string styleStrWidth = string.Empty;
                        if (columnStyle.ToLower() == "star")
                        {
                            styleStrWidth = string.Format("flex:{0};", columnWidth);
                        }
                        else if (columnStyle.ToLower() == "pixel")
                        {
                            columnWidth = columnWidth + "px";
                            styleStrWidth = string.Format("width:{0};", columnWidth);
                        }
                        if (controlFlags[i, j] != null)
                        {
                            GridAttach gridAttach = controlFlags[i, j].AttachObject as GridAttach;
                            int columnSpan = gridAttach.ColumnSpan;
                            if (columnSpan > 1)
                            {
                                styleStrWidth = string.Format("flex:{0};", columnSpan);
                            }
                        }

                        if (!string.IsNullOrEmpty(minWidth))
                        {
                            styleStrWidth += string.Format("min-width:{0}px;", minWidth);
                        }
                        if (!string.IsNullOrEmpty(maxWidth))
                        {
                            styleStrWidth += string.Format("max-width:{0}px;", maxWidth);
                        }
                        styleStrWidth += "position:relative;";
                        this.HtmlWriter.AddAttribute("class", "tablecell");
                        this.HtmlWriter.AddAttribute("style", styleStrWidth);

                        #endregion

                        #region
                        if (controlFlags[i, j] != null)
                        {
                           // var str = "<<";
                            //GridAttach gridAttach = controlFlags[i, j].AttachObject as GridAttach;
                            //int columnSpan = gridAttach.ColumnSpan;
                            //if (columnSpan > 1)
                            //{
                            //    this.HtmlWriter.AddAttribute("colspan", columnSpan.ToString());
                            //    styleStrWidth = string.Format("flex:{0};", columnSpan);
                            //}
                         
                            this.HtmlWriter.RenderBeginTag("div");

                            if (controlFlags[i, j] != null)
                            {
                                var builder = controlFlags[i, j].GetBuilder(this.IsPreview, this.ScreenDefinition, this.Compile, this.ProjectDocument, this.PermissionData, this.HtmlWriter);
                                builder.Parent = this;
                                builder.Build();
                            }

                            this.HtmlWriter.RenderEndTag();
                        }
                        else
                        {
                            this.HtmlWriter.RenderBeginTag("div");
                            this.HtmlWriter.RenderEndTag();
                        }
                        #endregion
                    }
                }
                this.HtmlWriter.RenderEndTag();
            }

            #endregion
        }
    }
}
