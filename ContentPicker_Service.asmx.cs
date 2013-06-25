using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

using umbraco.DataLayer;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;
using umbraco.BusinessLogic;


namespace ContentPicker_FE
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://fele.com/")]
    [ScriptService]

    public class ContentPickerService : WebService
    {
        //properties
        private JavaScriptSerializer jsonSerializer=new JavaScriptSerializer();
        private Dictionary<string, string> returnValue = new Dictionary<string, string>();

        public ContentPickerService()
        {
            
        }

        //web methods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, string> GetContent(string rootNodeID)
        {
            Authorize();
            List<Content> contentList = new List<Content>();

            //cannot load the -1 doc
            if (rootNodeID == "-1")
            {
                returnValue.Add("content", jsonSerializer.Serialize(contentList));
            }
            else
            {
                Document rootDocument = new Document(Convert.ToInt32(rootNodeID));

                try
                {
                    addChildren(rootDocument, contentList, rootDocument.Level);
                }
                catch (Exception e)
                {
                    Log.Add(LogTypes.Debug, 0, "content picker error=>"+e.Message);
                }
                returnValue.Add("content", jsonSerializer.Serialize(contentList));
            }

            return returnValue;
        }

        void addChildren(Document document, List<Content> contentList, int startLevel)
        {
            //base case
            //Log.Add(LogTypes.Debug, 0, "#children=>"+document.Children.Length.ToString());

            if (document.Children.Length == 0)
            {
                //Log.Add(LogTypes.Debug, 0, "path=>"+path);
                addContent(document, contentList, buildPath(document, ""));
            }
            else
            {
                //Log.Add(LogTypes.Debug, 0, "kids=>" + document.Text);
                //limit how deep we go
                if (document.Level-startLevel<2)
                {
                    addContent(document, contentList, buildPath(document, ""));
                    foreach (Document child in document.Children)
                    {
                        addChildren(child, contentList, startLevel);
                    }
                }
            }
        }

        string buildPath(Document document, string path)
        {
            if (document.Level == 1)
            {
                return document.Text + "/" + path;
            }
            else
            {
                 Document parent = new Document(document.ParentId);
                 return buildPath(parent, document.Text + "/" + path);
            }
        }

        public virtual void addContent(Document document, List<Content> contentList, string path)
        {

            Content content = new Content();
            content.nodeID = document.Id;
            content.name = document.Text;
            content.updateDate = document.UpdateDate.ToString();
            content.createDate = document.CreateDateTime.ToString();
            content.icon = "/umbraco/images/umbraco/" + document.ContentTypeIcon;
            content.creator = document.Creator.Name;
            content.docType = document.ContentType.Alias;
            content.image = "/umbraco/images/thumbnails/" + document.ContentType.Thumbnail;
            content.docTypeID = document.ContentType.Id;
            content.path = path;

            Node thisNode = new Node(document.Id);
            content.publisher = thisNode.WriterName;

            contentList.Add(content);
        }

        internal static void Authorize()
        {
            if (!umbraco.BasePages.BasePage.ValidateUserContextID(umbraco.BasePages.BasePage.umbracoUserContextID))
            {
                throw new Exception("Client authorization failed. User is not logged in");
            }

        }
    }


    //helper classes
    public class Content
    {
        public int nodeID;
        public string name;
        public string updateDate;
        public string createDate;
        public string icon;
        public string image;
        public string creator;
        public string docType;
        public string publisher;
        public int docTypeID;
        public string path;

        public Content(){

        }
    }
}
