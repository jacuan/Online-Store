$('#addToCart').click(function () {

    if ($('#orderQuantity').val() <= 0 || $('#orderQuantity').val() == "") {
        alert("Minimum purchase quantity is 1.")
    }
    else if ($('#hiddenCustomerID').val() == "")
    {
        $('#popupProductDetail').dialog('close');
        $('#loginForm').dialog('open');
    }
    else
    {
        var order = {
            ProductId: $('#hiddenProductID').val(),
            Quantity: $('#orderQuantity').val()
        };

        var id = $('#hiddenCustomerID').val();

        $.ajax({
            url: '../../Product/PurchaseProduct/' + id,
            type: "POST",
            data: JSON.stringify(order),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            error: function (xhr, textStatus, error) {
                alert('Error!');
            },
            success: function (data) {
                if (data == 1) {
                    $('#popupProductDetail').dialog('close');
                    $('#orderQuantity').val(1);

                    $.ajax({
                        url: 'http://digitalx.azurewebsites.net/Membership/CountShoppingCartItems/' + id,
                        type: "GET",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        cache: false,
                        error: function (xhr, textStatus, error) {
                            alert('Error');
                        },
                        success: function (data) {
                            $('#cartItemsCount').text(data + "  items");
                        }
                    });
                }
                else if (data == 0) {
                    $('#popupProductDetail').dialog('close');
                    $('#loginForm').dialog('open');
                }
                else if (data == -1) {
                    alert("Server error! Please try again later.");
                }
            }
        });
    }
});