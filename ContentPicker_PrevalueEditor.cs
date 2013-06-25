using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Xml;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using umbraco.interfaces;

using System.Web.Script.Serialization;

namespace ContentPicker_FE
{
    /// <summary>
    /// This class is used to setup the datatype settings. 
    /// On save it will store these values (using the datalayer) in the database
    /// </summary>
    public class ContentPicker_PrevalueEditor : System.Web.UI.UpdatePanel, IDataPrevalue
    {
        // referenced datatype
        private umbraco.cms.businesslogic.datatype.BaseDataType _datatype;

        private TextBox saveBox;

        private JavaScriptSerializer jsonSerializer;
        private ContentPicker_Options savedOptions;

        public ContentPicker_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType DataType)
        {
            _datatype = DataType;
            jsonSerializer = new JavaScriptSerializer();
            savedOptions = Configuration;
        }

        public Control Editor
        {
            get
            {
                return this;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            saveBox = new TextBox();
            saveBox.TextMode = TextBoxMode.MultiLine;
            saveBox.CssClass = "saveBox";
            ContentTemplateContainer.Controls.Add(saveBox);

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/ContentPicker/ContentPicker_Prevalue.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(ContentPicker_DataEditor), "ContentPickerCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/ContentPicker/ContentPicker_Prevalue.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(ContentPicker_DataEditor), "ContentPickerJS", js, false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            ContentPicker_Options renderingOptions;

            //test for postback, decide to use db or saveBox for rendering
            if (Page.IsPostBack)
            {
                //test for saveBox having a value, default if not
                if (saveBox.Text != "")
                {
                    renderingOptions = jsonSerializer.Deserialize<ContentPicker_Options>(saveBox.Text);
                }
                else
                {
                    renderingOptions = new ContentPicker_Options();
                }
            }
            else
            {
                renderingOptions = savedOptions;
            }

            //Log.Add(LogTypes.Debug, 0, "snid=>"+renderingOptions.startNodeID.ToString());
            //Log.Add(LogTypes.Debug, 0, "dtid=>" + renderingOptions.allowedDocTypeIDs);

            HtmlTable table = new HtmlTable();

            HtmlTableRow tr;
            HtmlTableCell td;

            //start node
            tr = new HtmlTableRow();
            table.Controls.Add(tr);

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            td.InnerText = "Start Node ID";

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            TextBox startNode = new TextBox();
            startNode.CssClass = "startNodeID";
            td.Controls.Add(startNode);
            startNode.Text = renderingOptions.startNodeID.ToString();

            //allow multiple
            tr = new HtmlTableRow();
            table.Controls.Add(tr);

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            td.InnerText = "Allow Multiple?";

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            CheckBox allowMultiple = new CheckBox();
            allowMultiple.CssClass = "allowMultiple";
            td.Controls.Add(allowMultiple);
            allowMultiple.Checked = renderingOptions.allowMultiple;

            //allowed doctypeids
            tr = new HtmlTableRow();
            table.Controls.Add(tr);

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            td.InnerText = "Allowed Doc Type ID's (CSV)";

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            TextBox allowedDocTypes = new TextBox();
            allowedDocTypes.CssClass = "allowedDocTypes";
            td.Controls.Add(allowedDocTypes);
            allowedDocTypes.Text = renderingOptions.allowedDocTypeIDs;

            //show all docTypes?
            tr = new HtmlTableRow();
            table.Controls.Add(tr);

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            td.InnerText = "Show All Doc Types?";

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            CheckBox showAllDocTypes = new CheckBox();
            showAllDocTypes.CssClass = "showAllDocTypes";
            td.Controls.Add(showAllDocTypes);
            showAllDocTypes.Checked = renderingOptions.showAllDocTypes;


            //js path
            tr = new HtmlTableRow();
            table.Controls.Add(tr);

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            td.InnerText = "JS Path";

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            TextBox jsPath = new TextBox();
            jsPath.CssClass = "jsPath";
            td.Controls.Add(jsPath);
            jsPath.Text = renderingOptions.jsPath;

            //css path
            tr = new HtmlTableRow();
            table.Controls.Add(tr);

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            td.InnerText = "CSS Path";

            td = new HtmlTableCell();
            tr.Controls.Add(td);
            TextBox cssPath = new TextBox();
            cssPath.CssClass = "cssPath";
            td.Controls.Add(cssPath);
            cssPath.Text = renderingOptions.cssPath;

            table.RenderControl(writer);            
        }

        public void Save()
        {
            _datatype.DBType = (umbraco.cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(umbraco.cms.businesslogic.datatype.DBTypes), DBTypes.Ntext.ToString(), true);

            SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @dtdefid", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId));
            SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId), SqlHelper.CreateParameter("@value", saveBox.Text));
        }

        public ContentPicker_Options Configuration
        {
            get
            {
                string dbValue = "";
                try
                {
                    object conf = SqlHelper.ExecuteScalar<object>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", SqlHelper.CreateParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));
                    dbValue = conf.ToString();
                }
                catch (Exception e)
                {
                }

                if (dbValue.ToString() != "")
                {
                    return jsonSerializer.Deserialize<ContentPicker_Options>(dbValue.ToString());
                }
                else
                {
                    return new ContentPicker_Options();
                }
            }
        }

        public static ISqlHelper SqlHelper
        {
            get
            {
                return Application.SqlHelper;
            }
        }
    }
}