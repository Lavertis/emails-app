// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showAlert({message, label, isConfirm}) {
    $('#alertModalBody').text(message);
    $('#alertModalLabel').text(label);

    if (isConfirm) {
        $('#alertModalClose').hide();
        $('#alertModalConfirm, #alertModalCancel').show();

        return new Promise((resolve) => {
            const alertModal = new bootstrap.Modal(document.getElementById('alertModal'));
            document.getElementById('alertModalConfirm').onclick = function () {
                resolve(true);
                alertModal.hide();
            };
            document.getElementById('alertModalCancel').onclick = function () {
                resolve(false);
                alertModal.hide();
            };
            alertModal.show();
        });
    } else {
        $('#alertModalClose').show();
        $('#alertModalConfirm, #alertModalCancel').hide();
        $('#alertModal').modal('show');
    }
}
