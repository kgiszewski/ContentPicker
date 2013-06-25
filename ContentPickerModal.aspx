<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentPickerModal.aspx.cs" Inherits="ContentPicker_FE.ContentPickerPage" %>
<!DOCTYPE html>
<html>
    <head id='head' runat="server">
        <title>Select</title>

        <link type="text/css" href="/umbraco/plugins/ContentPicker/ContentPickerModal.css" rel="stylesheet"/>

        <script type="text/javascript" src="/umbraco_client/ui/jquery.js?cdv=1"></script>
        <script type="text/javascript" src="/umbraco/plugins/ContentPicker/ContentPickerModal.js"></script>

        <script src="/umbraco_client/Application/NamespaceManager.js"></script>
        <script src="/umbraco_client/Application/UmbracoApplicationActions.js"></script>
        <script src="/umbraco_client/Application/UmbracoClientManager.js"></script>
        <script src="/umbraco_client/Application/JQuery/jquery.metadata.min.js"></script>
        <script src="/umbraco_client/Application/UmbracoUtils.js"></script>

        <script src="/umbraco_client/Tree/UmbracoTree.js"></script>
        <script src="/umbraco_client/Tree/NodeDefinition.js"></script>
        <script src="/umbraco_client/Tree/jquery.tree.js"></script>

        
        <script src="/umbraco_client/Tree/jquery.tree.metadata.js"></script>

    </head>

    <body>

            <div id="treePickerDiv" runat="server">

                <div id="filterDiv" runat="server">
                    <div class='searchBoxDiv'>
                        <label>Filter</label>
                        <input class="searchBox" type="text" />
                    </div>
                    <div class='sortDiv'>
                        <img direction='asc' class='sortTitle' src='/umbraco/plugins/ContentPicker/images/sort-alphabet.png' title='Sort by Title'/>
                        <img direction='asc' class='sortDocType' src='/umbraco/plugins/ContentPicker/images/sort-alphabet.png' title='Sort by DocType'/>
                        <img direction='asc' class='sortPath' src='/umbraco/plugins/ContentPicker/images/sort-alphabet.png' title='Sort by Path'/>
                        <img direction='asc' class='sortUpdatedOn' src='/umbraco/plugins/ContentPicker/images/sort-alphabet.png' title='Sort by Updated Date'/>
                        <img direction='asc' class='sortCreatedOn' src='/umbraco/plugins/ContentPicker/images/sort-alphabet.png' title='Sort by Create Date'/>
                        <img direction='asc' class='sortCreatedBy' src='/umbraco/plugins/ContentPicker/images/sort-alphabet.png' title='Sort by Created By'/>
                        <img direction='asc' class='sortUpdatedBy' src='/umbraco/plugins/ContentPicker/images/sort-alphabet.png' title='Sort by Updated By'/>
                    </div>

                    <div class='countDiv'><span></span></div>
                </div>

                <div id="previewDiv" runat="server"/>

                <div id="selectedDiv" runat="server"/>
            </div>
    
            <div id="adminDiv">
                <input type='button' value='Select' onclick='selectValues();'/><em> or </em><input type='button' value='Cancel' onclick='closeModal();'/>
            </div>

    </body>
</html>