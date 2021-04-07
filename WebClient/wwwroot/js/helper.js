function ShowMessage(successStatus, successMessage, errorMessage) {
    if (successStatus == "True") {
        $(document).ready(function () {
            $('#modalTitle').html("<p class=\"text-success\">Success</p>");
            $('#modalContent').html(successMessage);
            $('#modalCenter').modal('show');
        });
    }
    if (successStatus == "False") {
        $(document).ready(function () {
            $('#modalTitle').html("<p class=\"text-danger\">Error</p>");
            $('#modalContent').html(errorMessage);
            $('#modalCenter').modal('show');
        });
    }
}

function CalculateFee() {
    if (!isEmpty(document.getElementById('SourceJMBG').value) && !isEmpty(document.getElementById('SourcePASS').value) && !isEmpty(document.getElementById('Amount').value)) {
        $.ajax({
            type: "POST",
            url: '/Wallet/CalculateFee',
            contentType: "application/json",
            data: JSON.stringify({
                jmbg: document.getElementById('SourceJMBG').value,
                pass: document.getElementById('SourcePASS').value,
                amount: parseFloat(document.getElementById('Amount').value)
            }),
            dataType: "json",
            success: function (data) { document.getElementById('Fee').value = data },
            error: function (data) {
                $('#modalTitle').html("<p class=\"text-danger\">Fee calculation failed.</p>");
                $('#modalContent').html(data.responseJSON.errorMessage);
                $('#modalCenter').modal('show');
            }
        });
    }
}

$(document).on('keydown', '#Amount', function (e) {
    var input = $(this);
    var oldVal = input.val();
    var regex = new RegExp(input.attr('pattern'), 'g');

    setTimeout(function () {
        var newVal = input.val();
        if (!regex.test(newVal)) {
            input.val(oldVal);
        }
    }, 0);
});

function isEmpty(str) {
    return (!str || str.length === 0);
}