function closeModal(value){
  if(value==undefined){
    value=null;
  }
  parent.UmbClientMgr.closeModalWindow(value);
}

function selectValues(){
  closeModal($("#selectedDiv .selectedItem"));
}

$(function(){
  //attach an event to tree items
  $("li").live('mouseup', function(event){
    var $eventTarget=$(event.target).closest("li");
    //console.log($eventTarget);
    
    if($eventTarget[0]===this){
      //console.log($eventTarget.attr('id'));
      getContent($eventTarget.attr('id'));
    }
  });
});

function showLoader(){
  $("body").append("<div id='loaderDiv'><span><img src='/umbraco/plugins/ContentPicker/images/ajax-loader.gif'/></span></div>");
  $("#loaderDiv span").css('line-height', $("#loaderDiv").height()+"px"); 
}

function closeLoader(){
  $("#loaderDiv").remove();
}

//goes and gets the content
function getContent(rootNodeID){

  showLoader();

  $.ajax({
    type: "POST",
    async: false,
    url: "/umbraco/plugins/ContentPicker/ContentPicker_Service.asmx/GetContent",
    data: '{"rootNodeID":'+rootNodeID+'}',
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (returnValue){
      buildPreview(returnValue);
      closeLoader();
    }
  });
}

//builds the preview pane from JSON
function buildPreview(results){
  
  var content=eval(results.d.content);
  var $previewDiv=$("#previewDiv");
  
  var prevalues=eval("("+decodeURIComponent(getCookie('contentPicker'))+")");
  prevalues.allowedDocTypeIDs=prevalues.allowedDocTypeIDs.split(',');
  //console.log(prevalues);
  
  //clear it out
  $previewDiv.html(' ');
  
  var selectedIDs=getSelectedIDs();
  var counter=0;

  for(var i=0;i<content.length;i++){
    //console.log(content[i]);
    
    var previewDiv="";
    var publisher=(content[i].publisher!=null)?content[i].publisher:'Not Published';
    
    var highlight=(inArray(content[i].nodeID, selectedIDs))?'previewSelected':'';
    var disabled="";
    
    if(prevalues.allowedDocTypeIDs!=""){
      disabled=(!inArray(content[i].docTypeID, prevalues.allowedDocTypeIDs))?'disabled':'';
    }
    
    if(!prevalues.showAllDocTypes&&disabled!=""){
        continue;
    }
    counter++;
    
    previewDiv+="<div class='previewItem "+highlight+" "+disabled+"' nodeID='"+content[i].nodeID+"' docTypeID='"+content[i].docTypeID+"'>";
    //previewDiv+="<h1 class='title'><img class='thumbnail' src='"+content[i].image+"'/><span>"+content[i].name+"</span><div>"+content[i].regions+"</div></h1>";
    previewDiv+="<h1 class='title'><img class='thumbnail' src='"+content[i].image+"'/><span>"+content[i].name+"</span></h1>";
    previewDiv+="<h2 class='docType'><img class='icon' src='"+content[i].icon+"'/>"+content[i].docType+"</h2>";
    previewDiv+="<h3 class='updatedOn'>Updated: "+content[i].updateDate+"</h3>";
    previewDiv+="<h3 class='updatedBy'>Published By: "+publisher+"</h3>";
    previewDiv+="<h3 class='createdOn'>Created: "+content[i].createDate+"</h3>";
    previewDiv+="<h3 class='createdBy'>Created By: "+content[i].creator+"</h3>";
    previewDiv+="<h3 class='path'>Path: "+content[i].path+"</h3>";
    previewDiv+="</div>";
    
    $previewDiv.append(previewDiv);
  }
  
  $(".disabled").fadeTo('fast', '0.5');
  $(".searchBox").keyup();
  $(".sortTitle").click();
  
  $(".countDiv span").text((counter)+' Result(s)');
}

//add to selected
$(".previewItem").live("click", function(){

  var $thisItem=$(this);
  var $selectedDiv=$("#selectedDiv");
  
  if($thisItem.hasClass('disabled')){
    return;
  }
  
  var allowMultiple=eval($("#treePickerDiv").attr('allowMultiple'));
    
  //get list of what is selected
  var selectedIDs=getSelectedIDs();
   
  if(!inArray($thisItem.attr('nodeid'), selectedIDs)){
  
    if(selectedIDs.length>0 && !allowMultiple){
     return;
    }
    
    var $newItem=$thisItem.clone();
    $newItem.removeClass("previewItem");
    $newItem.addClass("selectedItem");
    
    //hide some elements
    $newItem.find("h2, h3").hide();
    
    //clone the old title and hide it
    $thisItem.find("h1").clone().hide().appendTo($newItem);
    
    //limit the title length
    var oldTitle=$($newItem.find("h1 span")[0]).text();
    var newTitle=oldTitle;
    
    if(oldTitle.length>15){
      newTitle=oldTitle.substring(0, 15)+'...';
    }
    
    $($newItem.find("h1 span")[0]).text(newTitle).prepend("<img class='removeItem' src='/umbraco/plugins/ContentPicker/images/minus.png'/>");
    $selectedDiv.append($newItem);
    
    $thisItem.addClass("previewSelected");
  } else {
    $thisItem.removeClass('previewSelected');
    $selectedDiv.find('[nodeid='+$thisItem.attr('nodeid')+']').remove();
  }
});

//add remove item events
$(".selectedItem").live('click', function(){
  //remove from selectedDiv
  var $selectedItem=$(this).closest('.selectedItem');
  
  //unhighlight
  $("#previewDiv [nodeid="+$selectedItem.attr('nodeid')+"]").removeClass("previewSelected");
  
  $selectedItem.remove();
});

//filter box event
$(function (){
  $(".searchBox").keyup(function(){
    var keywords=$(this).val();
    keywords=keywords.toLowerCase();
    //console.log(keywords);
    
    $previewItems=$(".previewItem");
    $previewItems.each(function(){
      var $thisItem=$(this);
      
      var searchHaystack=$thisItem.text();
      searchHaystack=searchHaystack.toLowerCase();
      
      if(searchHaystack.indexOf(keywords)==-1){
        $thisItem.hide();
      } else {
        $thisItem.show();
      }
    });
  });
});

//load the root node automatically - only works in FF
/*
$(window).load(function(){
  $("[rel=rootNode]").mouseup();
  $($("[rel=rootNode]").find('a')[0]).click();
});
*/

//sorts
$(function(){
  $(".sortTitle, .sortDocType, .sortPath, .sortUpdatedOn, .sortCreatedOn, .sortCreatedBy, .sortUpdatedBy").click(function(){
    var $button=$(this);
    var className=$button.attr('class');
    var direction=$button.attr('direction');
    var $previewItems=$(".previewItem:not(.disabled)");
    
    if($previewItems.length){
      $previewItems.sort($button.attr('direction')=='asc'?eval(className+'_asc'):eval(className+'_desc')).prependTo($previewItems.parent());
      $button.attr('direction', (direction=='asc')?'desc':'asc');
    }
  });
});

function sortTitle_asc(a, b){
  return (($(b).find('.title span').text()) <= ($(a).find('.title span').text()))?1:-1;    
}
function sortTitle_desc(a, b){
  return (($(b).find('.title span').text()) >= ($(a).find('.title span').text()))?1:-1;    
}

function sortDocType_asc(a, b){
  return (($(b).find('.docType').text()) < ($(a).find('.docType').text()))?1:-1;    
}
function sortDocType_desc(a, b){
  return (($(b).find('.docType').text()) > ($(a).find('.docType').text()))?1:-1;    
}

function sortPath_asc(a, b){
  return (($(b).find('.path').text()) < ($(a).find('.path').text()))?1:-1;    
}
function sortPath_desc(a, b){
  return (($(b).find('.path').text()) > ($(a).find('.path').text()))?1:-1;    
}

function sortUpdatedOn_asc(a, b){
  return (($(b).find('.updatedOn').text()) < ($(a).find('.updatedOn').text()))?1:-1;    
}
function sortUpdatedOn_desc(a, b){
  return (($(b).find('.updatedOn').text()) > ($(a).find('.updatedOn').text()))?1:-1;    
}

function sortCreatedOn_asc(a, b){
  return (($(b).find('.createdOn').text()) < ($(a).find('.createdOn').text()))?1:-1;    
}
function sortCreatedOn_desc(a, b){
  return (($(b).find('.createdOn').text()) > ($(a).find('.createdOn').text()))?1:-1;    
}

function sortCreatedBy_asc(a, b){
  return (($(b).find('.createdBy').text()) < ($(a).find('.createdBy').text()))?1:-1;    
}
function sortCreatedBy_desc(a, b){
  return (($(b).find('.createdBy').text()) > ($(a).find('.createdBy').text()))?1:-1;    
}

function sortUpdatedBy_asc(a, b){
  return (($(b).find('.updatedBy').text()) < ($(a).find('.updatedBy').text()))?1:-1;    
}
function sortUpdatedBy_desc(a, b){
  return (($(b).find('.updatedBy').text()) > ($(a).find('.updatedBy').text()))?1:-1;    
}


//helpers
function getSelectedIDs(){

  var $selectedDiv=$('#selectedDiv');
  var selectedIDs=[];
  
  $selectedDiv.find('.selectedItem').each(function(){
    var $thisItem=$(this);
    selectedIDs.push($thisItem.attr('nodeid'));
  });
  return selectedIDs;
}

function inArray(needle, haystack){
  var length = haystack.length;
  for(var i = 0; i < length; i++){
    if(haystack[i] == needle) return true;
  }
  return false;
}

function getCookie(c_name){
  var i,x,y,ARRcookies=document.cookie.split(";");

  for (i=0;i<ARRcookies.length;i++){
    x=ARRcookies[i].substr(0,ARRcookies[i].indexOf("="));
    y=ARRcookies[i].substr(ARRcookies[i].indexOf("=")+1);
    x=x.replace(/^\s+|\s+$/g,"");
    if (x==c_name)
    {
      return unescape(y);
    }
  }
}