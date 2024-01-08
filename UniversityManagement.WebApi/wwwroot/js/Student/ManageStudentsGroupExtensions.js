$(document).ready(function () {
    $('.addStudentToGroup, .removeStudentFromGroup').on('click', function () {

        var selectedStudentId = $(this).data('studentid');
        var selectedGroupId = $('#groupSelector_' + selectedStudentId).val();
        var buttonAction = $(this).hasClass('addStudentToGroup') ? 'addStudentToGroup' : 'removeStudentFromGroup';

        $.ajax({
            url: '/Student/ManageStudentsGroup/',
            method: 'POST',
            data: { studentId: selectedStudentId, groupId: selectedGroupId, buttonAction: buttonAction},
            success: function ()
            {
                location.reload(true);
            },
            error: function ()
            {
                console.error('Error during student operation: ', data.message);
            }
        });

    });
});