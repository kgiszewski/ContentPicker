using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using umbraco.cms.businesslogic.datatype;

namespace ContentPicker_FE
{

    public class ContentPicker : umbraco.cms.businesslogic.datatype.BaseDataType, umbraco.interfaces.IDataType
    {
        private umbraco.interfaces.IDataEditor _Editor;
        private umbraco.interfaces.IData _baseData;
        private ContentPicker_PrevalueEditor _prevalueeditor;

        // Instance of the Datatype
        public override umbraco.interfaces.IDataEditor DataEditor
        {
            get
            {
                if (_Editor == null)
                    _Editor = new ContentPicker_DataEditor(Data, ((ContentPicker_PrevalueEditor)PrevalueEditor).Configuration);
                return _Editor;
            }
        }

        //this is what the cache will use when getting the data
        public override umbraco.interfaces.IData Data
        {
            get
            {
                if (_baseData == null)
                    _baseData = new ContentPickerDefaultData(this);
                return _baseData;
            }
        }

        /// <summary>
        /// Gets the datatype unique id.
        /// </summary>
        /// <value>The id.</value>
        public override Guid Id
        {
            get
            {
                return new Guid("4964e90b-7408-472e-81b4-d559be024b2d");
            }
        }

        /// <summary>
        /// Gets the datatype unique id.
        /// </summary>
        /// <value>The id.</value>
        public override string DataTypeName
        {
            get
            {
                return "Content Picker FE";
            }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override umbraco.interfaces.IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_prevalueeditor == null)
                {
                    _prevalueeditor = new ContentPicker_PrevalueEditor(this);
                }
                return _prevalueeditor;
            }
        }
    }
}