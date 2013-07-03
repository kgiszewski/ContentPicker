$(function(){

  //page ini
  $(".contentPickerDiv").each(function(){
    var $thisDiv=$(this);
    buildXML($(this));
  });
  
  function makeSortable($pickerDiv){
    
    //make the selected items sortable
    $pickerDiv.find('.contentPickerSortTable tbody').sortable({
      handle: ".contentPickerSort",
      cursor: 'move',
      helper: fixHelper,
      update: function(){buildXML(this);},
      start : function(e, ui){ui.placeholder.html('<tr><td colspan="2">Insert Here</td></tr>')},
      placeholder: 'contentPickerPlaceholder'
    });
  }
  
  //handle a remove
  $(".contentPickerRemove").live('click', function(){
    var $button=$(this);
    var $table=$button.closest('table');
    
    $button.closest('tr').remove();
    buildXML($table);
  });
  
  //build the xml
  function buildXML(element){
    //console.log('building...');
    
    var $pickerDiv=$(element).closest('.contentPickerDiv');
    var $saveBox=$pickerDiv.find(".contentPickerSaveBox");
    var count=0;
    var xml="";
    
    makeSortable($pickerDiv);
    
    xml+="<contentPicker>";
    
    
    $pickerDiv.find(".contentPickerSortTable tr").each(function(){
      var $tr=$(this);
      
      xml+="<content>"+$tr.attr('nodeid')+"</content>";
      count++;
    });
    
    if(!count){
      $pickerDiv.find('.contentPickerTableWrapper').hide();
    }
    
    xml+="</contentPicker>";
    
    $saveBox.val(xml);
  }
   
  //handle the modal process
  $(".contentPickerSelect").live('click', function(event){
    var $link=$(this);

    UmbClientMgr.openModalWindow(
      '/umbraco/plugins/ContentPicker/ContentPickerModal.aspx', 
      'Select media item', 
      true, 
      1024, 
      768,
      0,
      0,
      '',
      function(returnValue){
        if(returnValue.outVal!=null){
          if(returnValue.outVal.length){
            var $sortTable=$link.closest('.contentPickerDiv').find(".contentPickerSortTable");
            $sortTable.parent().show();
            
            //console.log(returnValue.outVal);
            
            //grab cookie for settings
            var allowMultiple=eval($link.closest(".contentPickerDiv").attr('allowmultiple'));
            
            returnValue.outVal.each(function(){
              var $thisValue=$(this);
              var row="";
              
              row+="<tr nodeid='"+$thisValue.attr('nodeid')+"'>";
              
              row+="<td class='contentPickerTitle'>";
              row+=$($thisValue.find('h1')[1]).html();
              row+="</td>";
              
              row+="<td class='contentPickerButtons'>";
              row+=(allowMultiple)?"<img class='contentPickerSort' src='/umbraco/plugins/ContentPicker/images/sort.png'/>":"";
              row+="<img class='contentPickerRemove' src='/umbraco/plugins/ContentPicker/images/minus.png'/>";
              row+="</td>";
              
              row+="</tr>";
              
              if(!allowMultiple){
                $sortTable.find('tr').remove();
              }
              
              $sortTable.append(row);
            });
            buildXML($link);
          }
        }
      });
   });
   
  //helpers
  //supposed to help sortable widths 
  function fixHelper(e, ui){
    ui.children().each(function(){
      $(this).width($(this).width());
    });
    return ui;
  };
});

//cookie helper
function SetCookie(cookieName,cookieValue,nDays) {
  var today = new Date();
  var expire = new Date();
  if (nDays==null || nDays==0) nDays=1;
  expire.setTime(today.getTime() + 3600000*24*nDays);
  document.cookie = cookieName+"="+escape(cookieValue)+ ";expires="+expire.toGMTString()+ ";path=/umbraco";
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