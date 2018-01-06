
var globaleVariable;

var globalCloseVariable;

var loadedvariable = false;

function updateQuantity(clickedProduct) {

    $('#hiddenOrderDetailId').val(clickedProduct);

    globaleVariable = clickedProduct;

    inputvalue = $('#' + globaleVariable).closest('tr').find('.inputQuantity').val();

    if (inputvalue == 0 || inputvalue == "") {
        alert("Minimum purchase quantity is 1.")
    }
    else {
        var quantity = {
            AdjustedQuantity: inputvalue,
        };

        var id = $('#hiddenOrderDetailId').val();

        $.ajax({
            url: '../../Order/AdjustQuantity/' + id,
            type: "POST",
            data: JSON.stringify(quantity),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            error: function (xhr, textStatus, error) {
                alert('Error!');
            },
            success: function (response) {
                if (response == 1) {
                    var cid = $('#hiddenCustomerID').val();
                    $.ajax({
                        url: 'http://digitalx.azurewebsites.net/Membership/CountShoppingCartItems/' + cid,
                        type: "GET",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        cache: false,
                        error: function (xhr, textStatus, error) {
                            alert('Server Error!');
                        },
                        success: function (data) {
                            $('#cartItemsCount').text(data + "  items");
                        }
                    });

                    var message = 'Quantity has been updated successfully!';
                    $('div#confirmationMessage').text(message);
                    $('div#confirmationMessage').css('background-color', 'green');
                    messageStyle();

                    calculateTotalPrice();

                }
                else if (response == 0) {
                    $('#loginForm').dialog('open');
                }
                else if (response == -1) {
                    alert("Server error! Please try again later.");
                }
            }
        });
    }
};

function deleteItem(clickedProduct) {

    var id = clickedProduct;

    $.ajax({
        url: '../../Order/DeleteItem/' + id,
        type: "GET",
        error: function (xhr, textStatus, error) {
            alert('Error!');
        },
        success: function (response) {
            if (response == 1) {

                var cid = $('#hiddenCustomerID').val();

                $.ajax({
                    url: 'http://digitalx.azurewebsites.net/Membership/CountShoppingCartItems/' + cid,
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false,
                    error: function (xhr, textStatus, error) {
                        alert('Server Error!');
                    },
                    success: function (data) {
                        $('#cartItemsCount').text(data + "  items");
                    }
                });

                $('#' + clickedProduct).parent().parent().remove();

                calculateTotalPrice();
            }
            else if (response == 0) {
                $('#loginForm').dialog('open');
            }
            else if (response == -1) {
                var message = 'Server Error! Please try again later.'
                $('div#confirmationMessage').text(message);
                $('div#confirmationMessage').css('background-color', 'red');
                messageStyle();
            }
        }
    });
};

function calculateTotalPrice () {

    var totalPrice = 0;

    $("tr.sum").each(function () {

        var quantity = $(this).find('.inputQuantity').val();
        var price = $(this).find("td").eq(1);

        var sum = parseFloat(price.text()) * parseFloat(quantity);

        totalPrice += sum;
    });

    $('#displayTotalPrice').text('$' + totalPrice.toFixed(2));
};

function messageStyle() {
    $('div#confirmationMessage').toggle('6000', 'swing');
    $('div#confirmationMessage').delay(1000).toggle('5000');
};

$(function () {
    $("#checkoutPageMainDiv").accordion({
        heightStyle: "content",
  });
});

function pay() {

    var radioValue = $("input[name='address']:checked").val();

    var id = $('#hiddenOrderId').text();

    if ($('.inputQuantity').val() == 0 || $('.inputQuantity').val() == "") {
        alert("Some items' quantity is either zero or empty. The minimum purchase quantity is 1. Please verify again.");
    }
    else if ($('#hiddenCartItemsCoutn').text() == 0 || $('#hiddenCartItemsCoutn').text() == "") {
        alert("Your shopping cart is empty.")
    }
    else if ((!radioValue)) {
        alert("Please choose an address.");
    }
    else {

        $.ajax({
            url: '../Order/DeductStockConditionaly/' + id,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data == 1) {
                    $('#hiddenOrderId').text("");
                    location.href = '../Order/ThankYouPage';
                }
                else if (data == 0)
                {
                    location.href = '../Home/ErrorPage';
                }
                else
                {
                    for (var i = 0; i < data.length; i++) {
                        $('#backorderTableBody').append('<tr><td>' + data[i].ProductName + '</td><td>' +
                            data[i].AvailableStock + "</td><td class='.overOrdered'>" + data[i].OrderedQuantity + '</td></tr>');
                    }

                    $("#backorderTable td:nth-child(3)").css({ "color": "red", "font-weight": "bold" });
                    
                    $("#hiddenBackorderTable").dialog({
                        autoOpen: false,
                        resizable: true,
                        modal: true,
                        width: 'auto',
                        close: function (event, ui) {
                            $("#backorderTableBody").empty()
                        }
                    });

                    $('#hiddenBackorderTable').dialog('open');

                    $('#backorderButton').click(function () {

                        $('#hiddenBackorderTable').dialog('close');

                        $.ajax({
                            url: '../Order/DeductStockWithBackorder/' + id,
                            type: 'GET',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            cache: false,
                            success: function (data) {
                                if (data == 1) {
                                    $('#hiddenOrderId').text("");
                                    location.href = '../Order/ThankYouPage';
                                }
                                else if (data == 0) {
                                    location.href = '../Home/ErrorPage';
                                }
                            },
                            error: function (xhr, textStatus, error) {
                                alert("Error occured! Please contact us on 0800 777 777 to confirm your payment.")
                            }
                        })
                    });   
                }
            },
            error: function (xhr, textStatus, error) {
                alert("Error occured. Please contact us on 0800 777 777 to confirm your payment.")
            }
        });
    }
};

function AddressCreatedOK(data) {

    globalCloseVariable.dialog('close');

    $('#addressTableBody').append('<tr><td>' + "<input class='addressRadioButton' type='radio' name='address' value='" + data.AddressId + "'>" +
                   '</td><td>' + data.Street + '</td><td>' + data.Suburb + '</td><td>' + data.City + '</td><td>' +
                   data.Country + '</td><td>' + data.PostalCode + '</td></tr>');

};

function AddressCreatedFailed() {
    location.href = "../Home/ErrorPage"
};

function loadAddressList() {

    var id = $('#hiddenCustomerID').val();

        $.ajax({
            url: '../Membership/GetAddressList/' + id,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    $('#addressTableBody').append('<tr><td>' + "<input class='addressRadioButton' type='radio' name='address' value='" + data[i].AddressId + "'>" +
                        '</td><td>' + data[i].Street + '</td><td>' + data[i].Suburb + '</td><td>' + data[i].City + '</td><td>' +
                        data[i].Country + '</td><td>' + data[i].PostalCode + '</td></tr>');
                }
            },
            error: function (xhr, textStatus, error) {
                $('#addressTableMessage').text('Error occued. Please try again later.');
            }
        });
};

window.onload = loadAddressList;