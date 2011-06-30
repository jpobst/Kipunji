/*
 * Autogrow textarea for Jeditable
 *
 * Copyright (c) 2008 Mika Tuupola
 *
 * Licensed under the MIT license:
 *   http://www.opensource.org/licenses/mit-license.php
 * 
 * Depends on Autogrow jQuery plugin by Chrys Bader:
 *   http://www.aclevercookie.com/facebook-like-auto-growing-textarea/
 *
 * Project home:
 *   http://www.appelsiini.net/projects/jeditable
 *
 * Revision: $Id$
 *
 */
 
jQuery.fn.extend({
  insertAtCaret: function(myValue){
    return this.each(function(i) {
      if (document.selection) {
        this.focus();
        sel = document.selection.createRange();
        sel.text = myValue;
        this.focus();
      } else if (this.selectionStart || this.selectionStart == '0') {
        var startPos = this.selectionStart;
        var endPos = this.selectionEnd;
        var scrollTop = this.scrollTop;
        this.value = this.value.substring(0, startPos)+myValue+this.value.substring(endPos,this.value.length);
        this.focus();
        this.selectionStart = startPos + myValue.length;
        this.selectionEnd = startPos + myValue.length;
        this.scrollTop = scrollTop;
      } else {
        this.value += myValue;
        this.focus();
      }
   })
}
});

function add_menu_item (menu, textarea, icon, text, insert_text)
{
	var li = $('<li></li>');
	var link = li.append ("<a href='#' style='background-image: url(" + icon + ");'>" + text + "</a>");
	$(link).click(function (event) {
        textarea.insertAtCaret (insert_text); 
    });
        
	menu.append (li);
}

$.editable.addInputType('autogrow', {
    element : function(settings, original) {
        var textarea = $('<textarea />');
        if (settings.rows) {
            textarea.attr('rows', settings.rows);
        } else {
            textarea.height(settings.height);
        }
        if (settings.cols) {
            textarea.attr('cols', settings.cols);
        } else {
            textarea.width(settings.width);
        }
        
        var menubar = $("<div class='menubar'>");
        var menu = $("<ul class='menubar'>");

		add_menu_item (menu, textarea, '/Images/Editing/example.png', 'Example', '\n<example>\n\t<code lang="C#">\n\t</code>\n</example>\n');
		add_menu_item (menu, textarea, '/Images/Editing/list.png', 'List', '\n<list type="bullet">\n\t<item>\n\t\t<term>First Item</term>\n\t</item>\n</list>\n');
		add_menu_item (menu, textarea, '/Images/Editing/table.png', 'Table', '\n<list type="table">\n\t<listheader>\n\t\t<term>Column</term>\n\t\t<description>Description</description>\n\t</listheader>\n\t<item>\n\t\t<term>Term</term>\n\t\t<description>Description</description>\n\t</item>\n</list>\n');
		add_menu_item (menu, textarea, '/Images/Editing/see.png', 'See', '<see cref="T:System.Object"/>');
		add_menu_item (menu, textarea, '/Images/Editing/para.png', 'Paragraph', '\n<para>\n</para>\n');
		add_menu_item (menu, textarea, '/Images/Editing/note.png', 'Note', '\n<block subset="none" type="note">\n\t<para>\n\t</para>\n</block>\n');
		
		menubar.append (menu);
		
        $(this).append (menubar);
        $(this).append (textarea);
               
        return(textarea);
    },
    plugin : function(settings, original) {
        $('textarea', this).autogrow(settings.autogrow);
    }
});
