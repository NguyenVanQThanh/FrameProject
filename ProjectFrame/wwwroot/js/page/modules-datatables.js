"use strict";

$("[data-checkboxes]").each(function() {
  var me = $(this),
    group = me.data('checkboxes'),
    role = me.data('checkbox-role');

  me.change(function() {
    var all = $('[data-checkboxes="' + group + '"]:not([data-checkbox-role="dad"])'),
      checked = $('[data-checkboxes="' + group + '"]:not([data-checkbox-role="dad"]):checked'),
      dad = $('[data-checkboxes="' + group + '"][data-checkbox-role="dad"]'),
      total = all.length,
      checked_length = checked.length;

    if(role == 'dad') {
      if(me.is(':checked')) {
        all.prop('checked', true);
      }else{
        all.prop('checked', false);
      }
    }else{
      if(checked_length >= total) {
        dad.prop('checked', true);
      }else{
        dad.prop('checked', false);
      }
    }
  });
});

$("#table-1").dataTable({
  "columnDefs": [
    { "sortable": false, "targets": [3,5,11] }
  ]
});

//-----------

// document.addEventListener('DOMContentLoaded', function () {
//   var multilineTexts = document.querySelectorAll('.multiline-text');

//   multilineTexts.forEach(function (textElement) {
//     if (textElement.scrollHeight > textElement.clientHeight) {
//       var showMoreLink = document.createElement('span');
//       showMoreLink.className = 'show-more';
//       showMoreLink.innerHTML = '... Xem thêm';
//       showMoreLink.onclick = function () {
//         textElement.style.maxHeight = 'none';
//         showMoreLink.style.display = 'none';
//       };

//       textElement.appendChild(showMoreLink);
//     }
//   });
// });


// ------------------------