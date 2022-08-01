function getbyID(orderItemId) {
    $.ajax({
        url: "/Orders/GetOrderItembyID?orderItemId=" + orderItemId + "",
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $('#Id').val(result.Id);
            $('#Name').val(result.Product.NameAR);
            $('#PCount').val(result.ProductCount);
            $('#Price').val(result.ProductPrice);
            $('#OrderId').val(result.OrderId);
            $('#myModal').modal('show');
            $('#btnUpdate').show();
         },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}



function Update() {
    var UpdateOrderItemModel = {
        Id: $('#Id').val(),
        ProductCount: $('#PCount').val(),
        OrderId: $('#OrderId').val()
    };
    $.ajax({
        url: "/Orders/UpdateOrderItem",
        data: JSON.stringify(UpdateOrderItemModel),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.IsError > 0) {
                alert("الكمية المطلوبة غير متوفرة الكمية المتاحة حاليا هي : " + result.IsError + "");
                                  $('#modal').modal('toggle');
                    $('#myModal').modal('hide');
                 
            }
            else {
                $('#myModal').modal('hide');
                $.ajax({
                    type: "GET",
                    url: "/Orders/EditWithAddItem/" + result.OrderId,
                    data: result.OrderId,
                    success: function (data) {
                        window.location.reload();
                    }
                });
            }
           
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
