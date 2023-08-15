$(document).ready(function () {
    var interval = setInterval(function () {
        var momentNow = moment();
        $('#date-part').html(momentNow.format('DD MMMM YYYY') + ' '
                            + momentNow.format('dddd')
                             .substring(0, 3).toUpperCase());
        $('#time-part').html(momentNow.format('hh:mm:ss A'));
    }, 100);

    $('#stop-interval').on('click', function () {
        clearInterval(interval);
    });
});