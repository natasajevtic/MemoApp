function showPopupForm(url, title) {
    $.ajax({
        type: "GET",
        url: url,
        success: function (response) {
            $('#modalAdd .modal-body').html(response);
            $('#modalAdd .modal-title').html(title);
            $('#modalAdd').modal('show');
        },
        error: function (xhr) {
            bootbox.alert({
                title: "Error: " + xhr.status,
                message: xhr.responseText,
            });
        }
    })
}

function submitForm(form) {
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        $.ajax({
            type: "POST",
            url: form.action,
            data: $(form).serialize(),
            success: function (data) {
                if (data.isValid) {
                    $('#modalAdd').modal('hide');
                    bootbox.alert(
                        {
                            title: "Notification",
                            message: data.message.value
                        });
                    datatable.ajax.reload();
                }
                else {
                    $('#modalAdd .modal-body').html(data);
                }
            },
            error: function (xhr) {
                $('#modalAdd').modal('hide');
                bootbox.alert({
                    title: "Error: " + xhr.status,
                    message: xhr.responseText,
                });
            }
        });
    }
    return false;
}
