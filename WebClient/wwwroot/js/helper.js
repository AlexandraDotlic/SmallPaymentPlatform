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
            $('#modalTitle').html("<p class=\"text-danger\">Fail</p>");
            $('#modalContent').html(errorMessage);
            $('#modalCenter').modal('show');
        });
    }
}