$(function(){

  buildJSON();
  
  $('input').keyup(function(){
    buildJSON();
  });
  
  $('input').click(function(){
    buildJSON();
  });
  
  function buildJSON(){
    
    var $saveBox=$(".saveBox");
    
    var json="";
    
    json+="{";
    
    json+='"startNodeID":'+$('.startNodeID').val()+",";
    json+='"allowedDocTypeIDs":"'+$('.allowedDocTypes').val()+'",';
    json+='"showAllDocTypes":'+$('.showAllDocTypes input').is(":checked")+',';
    json+='"allowMultiple":'+$('.allowMultiple input').is(":checked")+',';
    json+='"jsPath":"'+$('.jsPath').val()+'",';
    json+='"cssPath":"'+$('.cssPath').val()+'"';
    
    json+="}";
    
    $saveBox.val(json);
    
  }
});