// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showAlert({message, label}) {
    $('#alertModalBody').text(message);
    $('#alertModalLabel').text(label);
    $('#alertModal').modal('show');
}