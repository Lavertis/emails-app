function deletePerson({actionUrl, redirectUrl}) {
    if (confirm('Are you sure you want to delete this person?')) {
        $.ajax({
            url: actionUrl,
            type: 'DELETE',
            contentType: 'application/json',
            success: function () {
                window.location.href = redirectUrl;
            },
            error: function () {
                alert('An error occurred while deleting the person.');
            }
        });
    }
}

function deleteEmail({actionUrl, emailCount, emailAddress}) {
    if (emailCount <= 1) {
        alert('Last email cannot be deleted.');
        return;
    }

    if (confirm(`Are you sure you want to delete the email address "${emailAddress}"?`)) {
        $.ajax({
            url: actionUrl,
            type: 'DELETE',
            contentType: 'application/json',
            success: function () {
                location.reload();
            },
            error: function () {
                alert('An error occurred while deleting the email.');
            }
        });
    }
}


function onSubmit(event) {
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
            alert(error.responseText);
        }
    });
}

$('#addEmailForm').submit(onSubmit);


$('#editButton').click(function () {
    $('#display').hide();
    $('#edit').show();
});

function cancelEdit() {
    $('#display').show();
    $('#edit').hide();
    $('#edit form')[0].reset();
}