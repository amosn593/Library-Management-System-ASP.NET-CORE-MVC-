$(document).ready(function () {
    $('#myNav').find('[href="'+window.location.pathname+'"]').parent().addClass('active');
})