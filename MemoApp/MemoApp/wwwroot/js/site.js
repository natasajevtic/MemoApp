var culture = $("#selectedCulture").val();

function getErrorMessage(xhr) {
    if (culture == "en-US") {
        switch (xhr.status) {
            case 400:
                return "Bad request. Please check the requested URL.";
            case 403:
                return "Access denied. You do not have access to this resource.";
            case 404:
                return "This resource was not found.";
            case 500:
                return "Sorry, something went wrong. Please try later.";
            default:
                return "An unknown error occurred. Please try again.";
        }
    }
    else if (culture == "sr") {
        switch (xhr.status) {
            case 400:
                return "Loš zahtev. Molimo vas proverite zahtevani URL.";
            case 403:
                return "Pristup odbijen. Nemate dozvolu za pristup ovom resursu.";
            case 404:
                return "Traženi resurs ne može biti pronađen.";
            case 500:
                return "Izvinite, nešto nije u redu. Pokušajte kasnije.";
            default:
                return "Dogodila se nepoznata greška. Pokušajte kasnije.";
        }
    }
}

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
                message: getErrorMessage(xhr),
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
                    message: getErrorMessage(xhr),
                });
            }
        });
    }
    return false;
}
