$("#onlineSupportWindow").dialog({
    autoOpen: false,
    resizable: false,
    modal: true
});

$('img#floating').click(function () {
    $('#onlineSupportWindow').dialog('open');
});