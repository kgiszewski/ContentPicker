using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using umbraco.controls.Tree;
using umbraco.editorControls.MultiNodeTreePicker;
using umbraco.cms.presentation.Trees;
using umbraco.BusinessLogic;

using System.Web.Script.Serialization;

using ContentPicker_FE;

namespace ContentPicker_FE
{
    public partial class ContentPickerPage : System.Web.UI.Page
    {
        private JavaScriptSerializer jsonSerializer;

        protected void Page_Init (object sender, EventArgs e)
        {
           
            jsonSerializer=new JavaScriptSerializer();
            TreeDefinition contentTree = TreeDefinitionCollection.Instance.Single(x => x.Tree.Alias.ToUpper() == "CONTENT");

            TreeDefinition filteredContentTree = new TreeDefinition(typeof(ContentPickerTree),
                                                         new umbraco.BusinessLogic.ApplicationTree(true, false, 0,
                                                                   contentTree.Tree.ApplicationAlias,
                                                                   "ContentPickerTree",
                                                                   contentTree.Tree.Title,
                                                                   contentTree.Tree.IconClosed,
                                                                   contentTree.Tree.IconOpened,
                                                                   "umbraco.editorControls",
                                                                   "ContentPicker_FE.ContentPickerTree",
                                                                   contentTree.Tree.Action),
                                                         contentTree.App);

            TreeDefinitionCollection.Instance.Add(filteredContentTree);
            
            CustomTreeControl treeControl = new CustomTreeControl();
            treeControl.StartNodeID=1937;//does not work
            treeControl.IsDialog=true;
            treeControl.DialogMode=TreeDialogModes.id;
            treeControl.TreeType="ContentPickerTree";
            treeControl.ShowContextMenu=false;

            treePickerDiv.Controls.AddAt(0, treeControl);

            //passing in the saved options to the page for use by jQuery
            ContentPicker_Options savedOptions = jsonSerializer.Deserialize<ContentPicker_Options>(HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["contentPicker"].Value));

            //Log.Add(LogTypes.Debug, 0, "cp=>"+savedOptions.jsPath);

            //add custom js/css if defined
            if (savedOptions.cssPath != "")
            {
                string customCSS = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", savedOptions.cssPath);

                HtmlGenericControl link = new HtmlGenericControl();
                link.InnerHtml = customCSS;
                head.Controls.Add(link);
            }

            if (savedOptions.jsPath != "")
            {
                string customJS = string.Format("<script src=\"{0}\" ></script>", savedOptions.jsPath);
                HtmlGenericControl script= new HtmlGenericControl();
                script.InnerHtml = customJS;
                head.Controls.Add(script);
            }
            treePickerDiv.Attributes["allowMultiple"] = savedOptions.allowMultiple.ToString().ToLower();
        }
    }
}