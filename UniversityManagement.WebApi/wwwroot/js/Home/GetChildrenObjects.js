$(document).ready(function () {
    $('.course-link').click(function () {

        var courseId = $(this).data('course-id');
        var groupsContainer = $('#groups-' + courseId);

        $('.groups-list').not(groupsContainer).hide();

        $.ajax({
            url: '/Home/GetGroupsByCourseId?courseId=' + courseId,
            method: 'GET',
            success: function (data) {
                groupsContainer.html(data);
                groupsContainer.show();
            },
            error: function (error) {
                console.error('Error in AJAX request:', error);
                showPopupErrorMessage('An error occurred: Course didn\'t have any associated groups.');
            }
        });
    });
    
    function showPopupErrorMessage(message) {
        alert(message);
    }
});

$(document).on('click', '.group-link', function (e) {
    e.preventDefault();

    var groupId = $(this).data('group-id');
    var studentsContainer = $('#students-' + groupId);

    $('.students-list').not(studentsContainer).hide();

    $.ajax({
        url: '/Home/GetStudentsByGroupId?groupId=' + groupId,
        method: 'GET',
        success: function (data) {
            studentsContainer.html(data);
            studentsContainer.show();
        },
        error: function (error) {
            console.error('Error in AJAX request:', error);
            showPopupErrorMessage('An error occurred: Group didn\'t have any associated students.');
        }
    });
    
    function showPopupErrorMessage(message) {
        alert(message);
    }
});