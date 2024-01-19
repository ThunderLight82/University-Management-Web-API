$(document).ready(function () {
    $('.addStudentToGroup').on('click', function () {

        var selectedStudentId = $(this).data('studentid');
        var selectedGroupId = $('#groupSelector_' + selectedStudentId).val();
        
       /* var buttonAction = $(this).hasClass('addStudentToGroup') ? 'addStudentToGroup' : 'removeStudentFromGroup';*/

        $.ajax({
            url: '/Student/AddStudentToGroup/',
            method: 'POST',
            data: {
                studentDto: { Id: selectedStudentId, GroupId: selectedGroupId }
            },
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
    
    $('.removeStudentFromGroup').on('click', function () {
        var selectedStudentId = $(this).data('studentid');

        $.ajax({
            url: '/Student/RemoveStudentFromGroup/',
            method: 'POST',
            data: {
                studentDto: { Id: selectedStudentId }
            },
            success: function () 
            {
                location.reload(true);
            },
            error: function (data) 
            {
                console.error('Error during student operation: ', data.message);
            }
        });
        
    });
});