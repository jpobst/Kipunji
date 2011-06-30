
var current_edit_item;

function htmlEncode(value){ 
  return $('<div/>').text(value).html(); 
} 

function htmlDecode(value){ 
  return $('<div/>').html(value).text(); 
}

function clear_errors ()
{
	$(".edit-error").remove ();
}

function set_error (form, data) {
	
	clear_errors ();
	
	var str = "<span class='edit-error'>Error: ";
	
	// The line info is usually in the exception message.
	/*
	if (data.error_line)
		str += " on line: " + data.error_line;
	if (data.error_pos)
		str += " position: " + data.error_pos;
	*/
	str += " " + data.error + "</span>";
	
	$('.current_edit').append (str);
}

$(document).ready(function() {

	$(".summary,.remarks,.description-params,.summary-return,.remarks-member,.summary-member,.remarks-type,.summary-type").editable(function (value, settings, completed) {
						
			if (this.revert == value) {
				this.reset ();
				return;
			}
			
			var editable = this;
			var success = false;
			var form = $(this);
			
			var url = "";
			var index = "";
			var type = $(this).attr ("class");
					
			if (type == "description-params") {
				url = current_url;
				index = 0;
				$(".description-params").each (function (ind, value) {
				     if (value === editable) {
				     	index =  ind;
				     }
				 
				});
			} else if (type == "summary-return" || type == "remarks-member" || type == "summary-type" || type == "remarks-type" || type == "summary-member") {
				url = current_url;
			} else {
				url = $(this).parent().parent("tr").find ("a:first").attr ("href");
			}

			clear_errors ();
			
			$('.editor_submit:eq(1)').after (settings.indicator);
			$('.editor_submit').hide ();
			
			$.ajax ({
   				url: edit_url,
   				global: false,
   				type: "POST",
   				data: {
   					"identifier": url,
   					"text": value,
   					"type": type,
   					"index": index
   				},
   				complete: function (r, status) {
   					success = (status == "success");
   					if (!success) {
   						
   						$('.editor_submit').show ();
   						$('.busy_indicator').remove ();
   						set_error (form, jQuery.parseJSON (r.responseText));
   						
   						return;
   					}
   					
   					var text = r.responseText;
   					if (text == null || text == '')
   						text = settings.tooltip;
   					
   					console.log ('the text is: "' + text + '"');
   					form.html (text);
   					completed ();
   				}
 			});
	   	},
	   	{
	   		indicator : edit_busy_image,
	        type      : "autogrow",
	        submit    : 'Save',
	        cancel    : 'Cancel',
	        tooltip   : "Click to edit...",
	        onblur    : "nothing",
	        cssclass  : "current_edit",
	        rows      : 3,
	        autogrow  : {
			       width: '500px'
        	},
        	onedit    :  function (value, settings) {
        		if (current_edit_item)
        			current_edit_item.reset ();
        		current_edit_item = this;
        	},

        	onreset   :  function(settings, original) { 
        		current_edit_item = null;
        		clear_errors ();
        	},
        	data	  : function (value, settings) {
			
				var editable = this;
				var success = false;		
				
				var url = "";
				var index = "";
				var type = $(this).attr ("class");
						
				if (type == "description-params") {
					url = current_url;
					index = 0;
					$(".description-params").each (function (ind, value) {
					     if (value === editable) {
					     	index =  ind;
					     }
					});
				} else if (type == "summary-return" || type == "remarks-member" || type == "summary-type" || type == "remarks-type" || type == "summary-member") {
					url = current_url;
				} else {
					url = $(this).parent().parent("tr").find ("a:first").attr ("href");
				}
							
				$.ajax ({
	   				url: edit_data_url,
	   				global: false,
	   				type: "POST",
	   				async: false,
	   				data: {
	   					"identifier": url,
	   					"text": value,
	   					"type": type,
	   					"index": index
	   				},
	   				complete: function (r, status) {
	   					success = (status == "success");
	   					if (!success)
	   						console.log ("Get Data error: " + status);
	   					else {
	   						value = r.responseText;
	   					}
	   				}
	 			});
	 			 			
	    		return value;
	    }
    });
});
 	
