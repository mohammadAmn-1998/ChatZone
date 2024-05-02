


function sendMessage(event) {

    event.preventDefault();

    var messageText = $("#message_text").val().trim();
    var groupId = $("#group_id").val();
    console.log(messageText);
    console.log(groupId);

    if (!messageText || !groupId)
        return;
    connection.invoke("AddNewMessage", messageText, groupId);

    
}
