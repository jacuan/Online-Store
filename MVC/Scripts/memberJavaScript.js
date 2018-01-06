
$('#accessOrderInformation').click(function () {
    location.href = "../Order/GetOrderInformation"
});

$("#loginForm").dialog({
    autoOpen: false,
    resizable: false,
    modal: true,
    close: function (event, ui) {
        $('.error1').empty();
        $('.error2').empty();
        $('.error3').empty();
        $('#loginForm').find('input:text').val('');
    }
});

$("#loginForm").dialog("option", "width", 300);

$('#loginButton').click(function () {
    $('#loginForm').dialog('open');
});

$('#submitLogin').click(function () {

    if ($('#userName').val() == '' && $('#password').val() == '') {
        $('.error1').text('User Name is required.');
        $('.error2').text('Password is required.')
    }

    else if ($('#userName').val() == '') {
        $('.error1').text('User Name is required.');
    }

    else if ($('#password').val() == '') {
        $('.error2').text('Password is required.')
    }

    else {

        var customer = {
            UserName: $('#userName').val(),
            Password: $('#password').val()
        };

        $.ajax({
            url: '../../Membership/Login',
            type: "POST",
            data: JSON.stringify(customer),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, textStatus, error) {
                alert('Error!');
            },
            success: function (data) {
                if (data == 1) {
                    $('#loginForm').dialog('close');
                    location.reload();
                }
                else if (data == 0) {
                    $('.error3').text('INVALID User Name and/or Password.')
                }
            }
        });
    }
});

$('#submitLoginLoginPage').click(function () {

    if ($('#userNameLoginPage').val() == '' && $('#passwordLoginPage').val() == '') {
        $('#error1LoginPage').text('User Name is required.');
        $('#error2LoginPage').text('Password is required.')
    }

    else if ($('#userNameLoginPage').val() == '') {
        $('#error1LoginPage').text('User Name is required.');
    }

    else if ($('#passwordLoginPage').val() == '') {
        $('#error2LoginPage').text('Password is required.')
    }

    else {

        var customer = {
            UserName: $('#userNameLoginPage').val(),
            Password: $('#passwordLoginPage').val()
        };

        $.ajax({
            url: '../Membership/Login',
            type: "POST",
            data: JSON.stringify(customer),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, textStatus, error) {
                alert('Error!');
            },
            success: function (data) {
                if (data == 1) {
                    window.location.href = "../Membership/UserCenter";
                }
                else if (data == 0) {
                    $('#error3LoginPage').text('INVALID User Name and/or Password.')
                }
            }
        });
    }
});

$(document).ready(function () {

    if ($('#hiddenCustomerID').val == "")
    {
        $('#hiddenProductID').val("");
        $('#hiddenOrderDetailId').val("");
        $('#hiddenOrderId').val("");
        $('#hiddenCartItemsCoutn').val("");
    }

});