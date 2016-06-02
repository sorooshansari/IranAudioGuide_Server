$(document).ready(function () {
    $("#img").on("change", function () {
        var img = $("#img").val().split("\\");
        $("#imgUrl").val(img[img.length - 1]);
    });
    $('#myModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var AudioId = button.data('audioid'); // Extract info from data-audioId attributes
        var modal = $(this);
        //modal.find('.modal-title').text('New message to ' + recipient);
    });
    $(".LoadAudio").on("click", function (event) {
        var tr = $(this);
        var Id = tr.data('id');
        //var playList = $(Id).html();
        //$(".amazingaudioplayer").html('<ul class="amazingaudioplayer-audios" style="display:none;"></ul>')
        //$(".amazingaudioplayer-audios").html(playList);
        $.get("/Admin/Audios", { PlaceId: Id })
                .done(function (data) {
                    console.log(data);
                    $(".Player").html(data);
                    $(".Player").removeClass("hidden");
                });
    });
    $("#NewCityForm").submit(function (event) {
        // Stop form from submitting normally
        event.preventDefault();

        // Get some values from elements on the page:
        var $form = $(this),
          CityName = $form.find("input[name='NewCity.CityName']").val(),
          CityDesc = $form.find("textarea[name='NewCity.CityDesc']").val(),
          url = $form.attr("action");
        if (CityName.length > 0) {
            $("#NewCityValidation").addClass("hidden");

            // Send the data using post
            var posting = $.post(url, { CityName: CityName, CityDesc: CityDesc });

            // Put the results in a div
            posting.done(function (data) {
                if (data.success) {
                    location.replace(location.href.split('#')[0] + "#Cities");
                }
            });
        }
        else
            $("#NewCityValidation").removeClass("hidden");
    });
    $(".rmvCty").on("click", function (event) {
        var button = $(this); // Button that Clicked
        var CityId = button.data('cityid'); // Extract info from data-CityId attribute
        var url = $("#ctyTbl").data('action');
        var posting = $.post(url, { Id: CityId });
        posting.done(function (data) {
            console.log(data);
            console.log(data.status);
            switch (data.status) {
                case 0:
                    $("#rmvError").empty();
                    location.reload();
                    break;
                case 1:
                    var CityName = button.data('cityname');
                    var btn = $("#ForignKeyErrorModal_Delete");
                    btn.addClass("removeClassWithSubs");
                    btn.data('id', CityId);
                    $("#ForignKeyErrorModal_body").empty().append('<div class="container text-info">This city (<span class="text-danger">' + CityName + '</span>) has one or more sub-places.<br/>Do you want to delete it with all of it\'s sub-places?</div>');
                    $('#ForignKeyErrorModal').modal('show');
                    break;
                case 2:
                    $("#rmvError").empty().append('<div class="12u$ align-left text-danger">Error in deletting city. Contact site developer to get more information.</div>');
                    break;
                default:

            }
            if (data.status) {
                location.replace(location.href.split('#')[0] + "#ctyTbl");
            }
        });

    });
    $(".rmvPlc").on("click", function (event) {
        var button = $(this); // Button that Clicked
        var CityId = button.data('placeid'); // Extract info from data-CityId attribute
        var url = $("#placeTbl").data('action');
        var posting = $.post(url, { Id: CityId });
        posting.done(function (data) {
            console.log(data);
            console.log(data.status);
            switch (data.status) {
                case 0:
                    $("#rmvError").empty();
                    location.reload();
                    break;
                case 1:
                    var CityName = button.data('placename');
                    $("#rmvError").empty().append('<div class="12u$ align-left text-danger">Error in deletting city. This city (' + CityName + ') has one or more place(s).</div>');
                    break;
                case 2:
                    $("#rmvError").empty().append('<div class="12u$ align-left text-danger">Error in deletting city. Contact site developer to get more information.</div>');
                    break;
                default:

            }
            if (data.status) {
                location.replace(location.href.split('#')[0] + "#ctyTbl");
            }
        });
    });
    $(".removeClassWithSubs").on("click", function (event) {
        alert('hi');
    })
});