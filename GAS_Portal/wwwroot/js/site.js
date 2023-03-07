// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#example').DataTable();
});

//Botão editar 
const editBtn = document.getElementById('editBtn');
const inputs = document.querySelectorAll('.settings-form input');

editBtn.addEventListener('click', function () {
    inputs.forEach(input => {
        input.removeAttribute('disabled');
    });

//Voltar Atrás
$(document).ready(function () {
    $('.submenu-toggle').click(function () {
        var submenu = $(this).next('.submenu');
        submenu.slideToggle();
        window.history.back();
        return false;
    });
});







