using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContentPicker_FE
{
    public class ContentPicker_Options
    {

        public int startNodeID = -1;
        public string allowedDocTypeIDs = "";
        public bool allowMultiple = true;
        public bool showAllDocTypes = true;
        public string jsPath = "";
        public string cssPath = "";
        
        public ContentPicker_Options() { }
    }

}