async function deletePerson({actionUrl, redirectUrl}) {
    if (confirm('Are you sure you want to delete this person?')) {
        fetch(actionUrl, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (response.ok) {
                    location.href = redirectUrl;
                } else {
                    alert('An error occurred while deleting the person.');
                }
            })
            .catch(error => {
                console.error('An error occurred while deleting the person.', error);
                alert('An error occurred while deleting the person.');
            });
    }
}

async function deleteEmail({actionUrl, emailCount, emailAddress}) {
    if (emailCount <= 1) {
        alert('Last email cannot be deleted.');
        return;
    }

    if (confirm(`Are you sure you want to delete the email address "${emailAddress}"?`)) {
        fetch(actionUrl, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (response.ok) {
                    location.reload();
                } else {
                    alert('An error occurred while deleting the email.');
                }
            })
            .catch(error => {
                console.error('An error occurred while deleting the email.', error);
                alert('An error occurred while deleting the email.');
            });
    }
}

async function addEmail({actionUrl}) {
    const form = document.querySelector('#addEmailForm');
    const formData = new FormData(form);

    fetch(actionUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(Object.fromEntries(formData))
    })
        .then(response => {
            if (response.ok) {
                location.reload();
            } else {
                alert('An error occurred while adding the email.');
            }
        })
        .catch(error => {
            console.error('An error occurred while adding the email.', error);
            alert('An error occurred while adding the email.');
        });
}

document.getElementById('editButton').addEventListener('click', function () {
    document.getElementById('display').style.display = 'none';
    document.getElementById('edit').style.display = 'block';
});

function cancelEdit() {
    document.getElementById('display').style.display = 'block';
    document.getElementById('edit').style.display = 'none';
    document.querySelector('#edit form').reset();
}