using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco;
using umbraco.cms.presentation.Trees;
using umbraco.cms.businesslogic.web;
using System.Web;
using umbraco.BusinessLogic.Utils;
using System.Xml.Linq;
using System.Xml.XPath;
using umbraco.NodeFactory;
using System.Xml;
using umbraco.interfaces;
using umbraco.BusinessLogic;
using System.Web.Script.Serialization;

namespace ContentPicker_FE
{
    public class ContentPickerTree : BaseContentTree
    {
        private JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        private ContentPicker_Options savedOptions;

        public ContentPickerTree(string app):base(app)
        {
            try
            {
                savedOptions = jsonSerializer.Deserialize<ContentPicker_Options>(HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["contentPicker"].Value));
            }
            catch (Exception e)
            {
                savedOptions = new ContentPicker_Options();
            }
        }
        #region Overridden methods
        
        public override int StartNodeID
        {
            get
            {
                return savedOptions.startNodeID;
            }
        }

        /// <summary>
        /// Creates the root node.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            if (!this.SetNullTreeRootNode(StartNodeID, ref rootNode, app))
            {
                rootNode.Action = "javascript:openContent(-1);";
                rootNode.Source = this.GetTreeServiceUrlWithParams(StartNodeID, this.GetDataTypeId());
                if (StartNodeID > 0)
                {
                    var startNode = new Document(StartNodeID);
                    rootNode.Text = startNode.Text;
                    rootNode.Icon = startNode.ContentTypeIcon;
                }
            }
        }
        #endregion
    }
}