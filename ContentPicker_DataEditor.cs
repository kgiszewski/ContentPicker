using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Web.Script.Serialization;

using umbraco.interfaces;
using umbraco.NodeFactory;
using umbraco.BusinessLogic;

using umbraco.controls.Tree;
using umbraco.editorControls.MultiNodeTreePicker;
using umbraco.cms.presentation.Trees;
using umbraco.cms.businesslogic.web;

namespace ContentPicker_FE
{
    /// <summary>
    /// This class is used for the actual datatype dataeditor, i.e. the control you will get in the content section of umbraco. 
    /// </summary>
    public class ContentPicker_DataEditor : System.Web.UI.UpdatePanel, umbraco.interfaces.IDataEditor
    {

        private umbraco.interfaces.IData savedData;
        private ContentPicker_Options savedOptions;
        private XmlDocument savedXML = new XmlDocument();
        private TextBox saveBox;
        private HtmlGenericControl wrapperDiv = new HtmlGenericControl("div");
        private JavaScriptSerializer jsonSerializer=new JavaScriptSerializer();
        public string currentData="";

        public ContentPicker_DataEditor(umbraco.interfaces.IData Data, ContentPicker_Options Configuration)
        {
            //load the prevalues
            savedOptions = Configuration;

            //ini the savedData object
            savedData = Data;
        }

        public virtual bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        public bool ShowLabel
        {
            get { return true; }
        }

        public Control Editor { get { return this; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.EnsureChildControls();

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/ContentPicker/ContentPicker.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(ContentPicker_DataEditor), "ContentPickerCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/ContentPicker/ContentPicker.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(ContentPicker_DataEditor), "ContentPickerJS", js, false);

            //add custom js/css if defined
            if (savedOptions.cssPath != "")
            {
                string customCSS = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", savedOptions.cssPath);
                ScriptManager.RegisterClientScriptBlock(Page, typeof(ContentPicker_DataEditor), "ContentPickerCSS" + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, customCSS, false);
            }

            if (savedOptions.jsPath != "")
            {
                string customJS = string.Format("<script src=\"{0}\" ></script>", savedOptions.jsPath);
                ScriptManager.RegisterClientScriptBlock(Page, typeof(ContentPicker_DataEditor), "ContentPickerJS" + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, customJS, false);
            }

            ContentTemplateContainer.Controls.Add(wrapperDiv);
            wrapperDiv.Attributes["class"] = "contentPickerDiv";
            wrapperDiv.Attributes["allowMultiple"] = savedOptions.allowMultiple.ToString().ToLower();

            saveBox = new TextBox();
            saveBox.TextMode = TextBoxMode.MultiLine;
            saveBox.CssClass = "contentPickerSaveBox";
            wrapperDiv.Controls.Add(saveBox);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            EnsureChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            buildControls();
        }

        protected void buildControls()
        {
            string data;

            //get the data based on action
            if (Page.IsPostBack)
            {
                data = saveBox.Text;
            }
            else
            {
                data = savedData.Value.ToString();
            }

            //hook for widget builder
            if (currentData != "")
            {
                data = currentData;
            }

            //load the data into an xml doc
            XmlDocument xd = new XmlDocument();

            try
            {
                xd.LoadXml(data);
            }
            catch (Exception e)
            {
                xd.LoadXml(ContentPickerDefaultData.defaultXML);
            }
            //XmlNodeList dataXML = xd.SelectNodes("contentPicker");

            LiteralControl selectLink = new LiteralControl("<div class='contentPickerButtonWrapper'><a class='contentPickerSelect' onclick=\"SetCookie('contentPicker', '"+HttpUtility.HtmlEncode(jsonSerializer.Serialize(savedOptions))+"', 1);\" href='#'>Select...</a></div>");

            wrapperDiv.Controls.Add(selectLink);

            HtmlGenericControl tableWrapper = new HtmlGenericControl("div");
            tableWrapper.Attributes["class"] = "contentPickerTableWrapper";
            wrapperDiv.Controls.Add(tableWrapper);

            HtmlTable sortTable = new HtmlTable();
            sortTable.Attributes["class"] = "contentPickerSortTable";
            tableWrapper.Controls.Add(sortTable);

            HtmlTableRow tr;
            HtmlTableCell td;

            //Log.Add(LogTypes.Debug, 0, "data xml=>" + xd.OuterXml);
            bool hasContent = false;

            foreach(XmlNode thisNode in xd.SelectNodes("contentPicker/content")){

                //Log.Add(LogTypes.Debug, 0, "nodeID=>"+thisNode.InnerText);

                //get the information from the document api
                Document thisDocument;
                try
                {
                    thisDocument = new Document(Convert.ToInt32(thisNode.InnerText));
                }
                catch (Exception e2)
                {
                    continue;
                }

                tr = new HtmlTableRow();
                sortTable.Controls.Add(tr);
                tr.Attributes["nodeid"] = thisNode.InnerText;

                td = new HtmlTableCell();
                tr.Controls.Add(td);
                td.InnerHtml = "<img class='thumbnail' src='/umbraco/images/thumbnails/"+thisDocument.ContentType.Thumbnail+"'/><span>"+thisDocument.Text+"</span>";

                td = new HtmlTableCell();
                tr.Controls.Add(td);
                td.Attributes["class"] = "contentPickerButtons";

                string buttons = "";
                buttons+=(savedOptions.allowMultiple)?"<img class='contentPickerSort' src='/umbraco/plugins/ContentPicker/images/sort.png'/>":"";
                buttons+="<img class='contentPickerRemove' src='/umbraco/plugins/ContentPicker/images/minus.png'/>";
                td.InnerHtml=buttons;

                hasContent = true;
            }

            if (hasContent)
            {
                tableWrapper.Attributes["style"] = "display: block;";
            }
        }

        public void Save()
        {
            savedData.Value = saveBox.Text;
        }

    }
}