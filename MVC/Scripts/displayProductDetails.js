$("#popupProductDetail").dialog({
    autoOpen: false,
    resizable: false,
    modal: true,
    close: function (event, ui) {
        $("#productDetailTableBody").empty();
    }
});

$("#popupProductDetail").dialog("option", "width", 1000);
$("#popupProductDetail").dialog("option", "height", 500);

function displayDetail(clickedProduct) {

    var id = clickedProduct;
    $('#hiddenProductID').val(id);

    $.ajax({
        url: 'http://webapidigitalx.azurewebsites.net/api/CategoryAndProduct/GetProductDetail/' + id,
        type: 'GET',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        error: function (xhr, textStatus, error) {
            alert('Internal Error! Please try again.');
        },
        success: function (data) {
            if (data != null) {
                var img = '<img src= "data:image/jpeg; base64,' + data.Picture + '" />';
                $('#productDetailTableBody').append('<tr><td>' + img + '</td><td>' + data.ProductName + '</td><td>' + data.ProductDescription + '</td><td>' + data.Price + '</td><td>' + data.UnitsInStock + '</td></tr>');
                $('#popupProductDetail').dialog('open');
            }
            else {
                alert('Internal Error! Please try again.')
            }
        }
    });
};