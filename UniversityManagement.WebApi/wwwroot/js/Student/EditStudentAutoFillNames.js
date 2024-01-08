function getStudentBothName()
{
    var studentIdElement = $('#studentId');
    if (studentIdElement.length > 0)
    {
        var selectedStudentId = studentIdElement.val();

        if (selectedStudentId !== null && selectedStudentId !== undefined)
        {
            $.ajax({
                url: '/Student/GetStudentNameDetails',
                method: 'GET',
                data: { studentId: selectedStudentId },
                success: function (data)
                {
                    $('#newChangedFirstName').val(data.firstName);
                    $('#newChangedLastName').val(data.lastName);
                },
                error: function (error)
                {
                    console.error('Error fetching student details: ', error);
                }
            });
        } else
        {
            console.error('selectedStudentId is null or undefined.');
        }
    } else
    {
        console.error('#studentId element not found.');
    }
}

$(document).ready(function ()
{
    var firstStudentId = $('#studentId option:first').val();
    getStudentBothName(firstStudentId);

    $('#studentId').change(function ()
    {
        var selectedStudentId = $(this).val();
        getStudentBothName(selectedStudentId);
    });
});