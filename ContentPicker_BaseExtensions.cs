using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.cms.presentation.Trees;
using System.Web;
using umbraco.cms.businesslogic;
using umbraco.BusinessLogic;
using umbraco.presentation;

namespace ContentPicker_FE
{
    /// <summary>
    /// BaseTree extensions for MultiNodeTreePicker.
    /// </summary>
    public static class BaseTreeExtensions
    {

        internal const int NoAccessId = -123456789;
        internal const int NoChildNodesId = -897987654;

        /// <summary>
        /// Determines if it needs to render a null tree based on the start node id and returns true if it is the case. 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="startNodeId"></param>
        /// <param name="rootNode"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        internal static bool SetNullTreeRootNode(this BaseTree tree, int startNodeId, ref XmlTreeNode rootNode, string app)
        {
            if (startNodeId == NoAccessId)
            {
                rootNode = new NullTree(app).RootNode;
                rootNode.Text = "You do not have permission to view this tree";
                rootNode.HasChildren = false;
                rootNode.Source = string.Empty;
                return true;
            }

            if (startNodeId == NoChildNodesId)
            {
                rootNode = new NullTree(app).RootNode;
                rootNode.Text = "[No start node found]";
                rootNode.HasChildren = false;
                rootNode.Source = string.Empty;
                return true;
            }

            rootNode.NodeType = "multinodetreepicker";
            return false;
        }

       

        /// <summary>
        /// Returns the data type id for the current base tree
        /// </summary>
        /// <remarks>
        /// The data type definition id is persisted between request as a query string.
        /// This is used to retreive values from the cookie which are easier persisted values
        /// than trying to append everything to custom query strings.
        /// </remarks>
        /// <param name="tree"></param>
        /// <returns></returns>
        internal static int GetDataTypeId(this BaseTree tree)
        {
            var id = -1;
            int.TryParse(tree.NodeKey, out id);
            return id;
        }

        /// <summary>
        /// Helper method to return the persisted cookied value for the tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree"></param>
        /// <param name="output"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        internal static T GetPersistedCookieValue<T>(this BaseTree tree, Func<HttpCookie, T> output, T defaultVal)
        {
            var cookie = HttpContext.Current.Request.Cookies["MultiNodeTreePicker"];
            if (cookie != null && cookie.Values.Count > 0)
            {
                return output(cookie);
            }
            return defaultVal;
        }

        /// <summary>
        /// This will return the normal service url based on id but will also ensure that the data type definition id is passed through as the nodeKey param
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="id">The id.</param>
        /// <param name="dataTypeDefId">The data type def id.</param>
        /// <returns></returns>
        /// <remarks>
        /// We only need to set the custom source to pass in our extra NodeKey data.
        /// By default the system will use one or the other: Id or NodeKey, in this case
        /// we are sort of 'tricking' the system and we require both.
        /// Umbraco allows you to theoretically pass in any source as long as it meets the standard
        /// which means you can pass around any arbitrary data to your trees in the form of a query string,
        /// though it's just a bit convoluted to do so.
        /// </remarks>
        internal static string GetTreeServiceUrlWithParams(this BaseTree tree, int id, int dataTypeDefId)
        {
            var url = tree.GetTreeServiceUrl(id);
            //append the node key
            return url + "&nodeKey=" + dataTypeDefId;
        }
    }
}
