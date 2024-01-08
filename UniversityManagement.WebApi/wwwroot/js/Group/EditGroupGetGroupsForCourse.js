function getGroups()
{
    var courseId = $("#courseId").val();
    $.ajax(
        {
            url: '/Group/GetGroupsByCourseId?courseId=' + courseId,
            method: 'GET',
            success: function (data)
            {
                var groupDropdown = $("#groupId");
                groupDropdown.empty();
                $.each(data, function (index, item)
                {
                    groupDropdown.append($('<option>',
                        {
                            value: item.id,
                            text: item.name
                        }));
                });
            },
            error: function (error)
            {
                console.error('Error fetching groups', error);

                var groupDropdown = $("#groupId");
                groupDropdown.empty();
                groupDropdown.append($('<option>',
                    {
                        text: 'Course didnt have any groups'
                    }));
            }
        });
}

$(document).ready(function ()
{
    var firstCourseId = $("#courseId option:first").val();
    $("#courseId").val(firstCourseId);
    getGroups();
});