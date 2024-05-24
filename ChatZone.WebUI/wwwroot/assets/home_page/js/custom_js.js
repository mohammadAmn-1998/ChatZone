$("#register-btn").click(function() {
    $("#login").slideUp();
    $("#register").fadeIn();
});
$("#login-btn").click(function() {
    $("#register").slideUp();
    $("#login").slideDown();
});

$(document).ready(function () {

    connection.onclose(start);

    loadUSerGroups();
    if (Notification.permission !== "granted") {
        Notification.requestPermission();

    }

    //var result = getCookie("systemAlert");
    //if (result) {

    //    result = JSON.parse(result);


    //}

});

var currentGroupId = 0;

function createNewGroup(event) {

    event.preventDefault();

    var imageFile = event.target[1].files[0];
    var groupTitle = event.target[0].value;
    var isPrivate = event.target[2].checked;


    if (!groupTitle || groupTitle.trim() === "") {
        alert("لطفا نام گروه را انتخاب کنید");
        return false;
    }


    if (imageFile) {
        const allowedExtensions = /(\.jpg|\.jpeg|\.png|\.bmp)$/i;
        if (!allowedExtensions.test(imageFile.name)) {
            alert('نوع فایل نامعتبر است. فقط JPG، JPEG، PNG و BMP مجاز هستند.');
            return false;
        }
    }

    var formData = new FormData();
    formData.append("title", groupTitle);
    formData.append("imageFile", imageFile);
    formData.append("isPrivate", isPrivate);
    $.ajax({
        url: "/Home/AddNewGroup",
        type: "Post",
        data: formData,
        encyType: "multipart/form-data",
        processData: false,
        contentType: false

    }).done(function (obj) {

        if (obj) {
            if (obj.message === "Error") {
                alert("مشکلی پیش آمده! چند لحظه دیگر دوباره تلاش کنید");
                return false;
            }
            if (obj.message === "Success") {
                $('.modal').toggle();

                return true;
            }
        }


        return false;

    });

    return false;
}

function loadUSerGroups() {

    $.ajax({
        url: "/Home/LoadUserGroupsList",
        type: "get"

    }).done(function (data) {

        $("#user_groups").html(data);
    });


}

function getUserProfile(id) {

    if (id !== 0) {

        $.ajax({
            url: "/Home/GetUserProfile?userId=" + id,
            type: "get"

        }).done(function (data) {

           

            $(".profile-modal").empty();

            $(".profile-modal").append(`
					
					<h3>${data.userName}</h3>
							<img src="/assets/home_page/img/${data.avatar}"alt="avatar_image"/>
					`);

        });

    }

}

function getChatGroupProfile(id) {

    if (id != 0) {

        $.ajax({
            url: "/Home/GetChatGroupProfile?groupId=" + id,
            type: "get"

        }).done(function (data) {

           

            $("#members_list").empty();

            data.users.forEach(user => {



                $("#members_list").append(`

												<li class="w-100 btn p-2" data-id="${user.id}">${user.userName}</li>

												`);
            });

        });

    }
}

function getGroupChats(event) {

    let token = event.srcElement.attributes[0].value;

    if (!token)
        return;

    $.ajax({
        url: "/Home/LoadGroupChats?token=" + token,
        type: "get"

    }).done(function (data) {

        $("#chat_content").html(data);

        currentGroupId = $("#group_id").val();
       
    });


}

function search() {

    var title = $("#search_text").val().trim();


    if (!title) {

        loadUSerGroups();
        return;

    }

    $.ajax({
        url: `/Home/SearchUserGroups?title=${title}`,
        type: "get"

    }).done(function (data) {

        if (data.length < 1) {

            $("#search_ul").empty();
            $("#search_ul").append(`
								<div class="text-center">
								<li>
								Not Found!
								</li>
								</div>
								`);
            return;
        }

        $("#search_ul").empty();
        data.forEach(function (obj) {
            var lastChat = obj.lastChat;
            if (obj.isUser) {

                $("#search_ul").append(`

																				<li data-token=${obj.token} onclick="getUserPrivateChats(event)">
												   ${obj.title}
													<img src="/assets/home_page/img/group_img/${obj.imageName}" alt="Avatar" />
										
										</li>

												`);




            }
            else {

                if (lastChat) {
                    $("#search_ul").append(`

																<li data-token="${obj.token}" onclick="getGroupChats(event)">
										   ${obj.title}
									<img src="/assets/home_page/img/group_img/${obj.imageName}" alt="Avatar" />
									<div class="text-start">
											<span>${obj.lastChat}
											<br/>${obj.lastChatDate}
										</span>
												</div>
								</li>

									`);
                } else {
                    $("#search_ul").append(`

																<li data-token=${obj.token} onclick="getGroupChats(event)">
												   ${obj.title}
													<img src="/assets/home_page/img/group_img/${obj.imageName}" alt="Avatar" />
										
										</li>

												`);


                }
            }


        });


    });


}

function searchUsers() {


    var searchText = $("#search_userName_text").val().trim();

    if (!searchText)
        return;

    $.ajax({
        url: `/Home/SearchUsers?userName=${searchText}&currentGroupId=${currentGroupId}`,
        type: "get"


    }).done(function (data) {

        $("#users_list").empty();
        data.forEach(user => {
            if (user.isMember) {
                $("#users_list").append(`

					<li data-id="${user.id}">

							<div class="p-2 w-100">${user.userName}<span>(عضو)</span>
					</div>
							

					</li>

					`);
            } else {

                $("#users_list").append(`

											<li class="btn btn-info  w-100" data-id="${user.id}" onclick="addMember(event)">

									${user.userName} (افزودن به گروه)  
						
									

							</li>

							`);

            }
        });

    });

}

function addMember(event) {
    var userId = event.target.getAttribute('data-id');

    if (!userId)
        return;

    $.ajax({
        url: `/Home/AddMemberToGroup?groupId=${currentGroupId}&userId=${userId}`,
        type: "get"

    }).done(function (result) {
        if (result) {
            if (result.status.trim() == "success") {
               

                $("#users_list li").each(function () {

                    if ($(this).attr("data-id") == result.userId) {
                        $(this).removeAttr("onclick");
                        $(this).removeClass("btn");
                        $(this).removeClass("btn-info");
                        $(this).text('');
                        $(this).append(`<div class="p-2 w-100">${result.userName}<span>(عضو)</span>
							</div>`);

                        
                    }


                });


            }


        }
    });

}

function joinGroup(groupId) {


    if (!groupId)
        return;

    $.ajax({
        url: "/Home/JoinGroup?groupId=" + groupId,
        type: "get"

    }).done(function (data) {

        $("#chat_content").empty();
        $("#chat_content").html(data);


    });


}

function beginChatWithUser(userId) {


    if (!userId) {
        alert("مشکلی وجود دارد دوباره امتحان کنید");
        return;
    }

    $.ajax({
        url: "/Home/AddNewUserPrivateChatGroup?userId=" + userId,
        type: "get"

    }).done(function (data) {


        $("#chat_content").html(data);

    });

}

function getUserPrivateChats(event) {

    let userId = event.srcElement.attributes[0].value;
    $.ajax({
        url: "/Home/GetUserChats?userId=" + userId,
        type: "get"

    }).done(function (data) {

        $("#chat_content").html(data);


    });


}

function sendFileToGroup(event) {

    event.preventDefault();

    

    var file = event.srcElement[0].files[0];
    var groupId = currentGroupId;

   

   
    if (!file) {

        ErrorAlert("فایلی انتخاب کنید و سپس ارسال را بزنید");
        return;

    }

    const allowedExtensions = /(\.jpg|\.jpeg|\.png|\.bmp|\.mkv|\.mp4)$/i;
    if (!allowedExtensions.test(file.name)) {
        alert('نوع فایل نامعتبر است. فقط JPG، JPEG، PNG و BMP ،mp4 ،mkv مجاز هستند.');
        return;
    }

    var formData = new FormData();
    
   

    formData.append("file", file);
    formData.append("groupId", groupId);
    var caption = $("#message_text").val();

    if (caption) {
        formData.append("caption", caption);
    }
   

    
    $.ajax({

        url: "/Home/SendFileToGroup",
        type: "Post",
        data: formData,
        encyType: "multipart/form-data",
        timeout: 20000, // Set a timeout of 20 seconds (adjust as needed)
        processData: false,
        contentType: false

    }).done(function (result) {


        if (result === "success") {

            $("#file_form_modal").fadeOut();

        }
        

    });
    
}

function handleAddMemberModalButton(parameters) {

    $("#profile_modal").fadeOut();

}

function reloadWindow() {

    location.reload();

}