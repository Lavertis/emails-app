async function deletePerson({actionUrl, redirectUrl}) {
    const isConfirmed = await showAlert({
        label: 'Confirmation',
        message: 'Are you sure you want to delete this person?',
        isConfirm: true
    });
    if (!isConfirmed) return;

    $.ajax({
        url: actionUrl,
        type: 'DELETE',
        contentType: 'application/json',
        success: function () {
            window.location.href = redirectUrl;
        },
        error: function () {
            showAlert({label: 'Error', message: 'An error occurred while deleting the person.'});
        }
    });
}

async function deleteEmail({actionUrl, emailAddress}) {
    const isConfirmed = await showAlert({
        label: 'Confirmation',
        message: `Are you sure you want to delete the email address "${emailAddress}"?`,
        isConfirm: true
    });
    if (!isConfirmed) return;

    $.ajax({
        url: actionUrl,
        type: 'DELETE',
        contentType: 'application/json',
        success: function () {
            location.reload();
        },
        error: function () {
            showAlert({label: 'Error', message: 'An error occurred while deleting the email.'});
        }
    });
}

$('#addEmailForm').submit(function (event) {
    event.preventDefault();

    const actionUrl = $(event.target).attr('action');
    const formData = Object.fromEntries(new FormData(event.target));

    $.ajax({
        url: actionUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        async: false,
        success: () => location.reload(),
        error: function (error) {
            showAlert({label: 'Error', message: error.responseText});
        }
    });
});

$('#editButton').click(function () {
    $('#display').hide();
    $('#edit').show();
});
