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
})