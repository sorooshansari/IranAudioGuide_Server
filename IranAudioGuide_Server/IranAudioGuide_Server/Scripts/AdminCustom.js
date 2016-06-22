angular.module('AdminPage', ['AdminPage.controllers', 'AdminPage.services', 'AdminPage.directives'])
.config(function ($sceProvider) {
    $sceProvider.enabled(false);
})
.run(function () {

});

//$(document).ready(function () {
//    //Define the player 
//    var player = document.getElementById('audio');
//    $('#play').click(function () { player.play() }); //play when play is clicked 
//    $('#pause').click(function () { player.pause() }); //pause when pause clicked
//    /*UPDATE PROGRESS BAR*/
//    function updateProgress() {

//        var progress = $("#progressIn");
//        var value = 0;

//        if (player.duration == 'Infinity') { //If duration = infinity set value to 100
//            value = 100;
//        } else if (player.currentTime > 0) { //else if it is > 0 calculate percentage to highlight
//            value = Math.floor((100 / player.duration) * player.currentTime);
//        }

//        progress.stop().animate({ 'width': value + '%' }, 500) //set the width of the progress bar

//        $('#time').html(formatTime(player.currentTime))
//    }
//    //set the new timestamp
//    player.addEventListener("timeupdate", updateProgress, false);// add event listener for audio time updates
//    //Format the audio tag's time stamp 
//    function formatTime(seconds) {
//        minutes = Math.floor(seconds / 60);
//        minutes = (minutes >= 10) ? minutes : "" + minutes;
//        seconds = Math.floor(seconds % 60);
//        seconds = (seconds >= 10) ? seconds : "0" + seconds;
//        return minutes + ":" + seconds;
//    }
//});



//$(document).ready(function () {
//$("#NewPlace_Image").on("change", function () {
//    var img = $("#NewPlace_Image").val().split("\\");
//    $("#imgUrl").val(img[img.length - 1]);
//});
//$('#myModal').on('show.bs.modal', function (event) {
//    var button = $(event.relatedTarget); // Button that triggered the modal
//    var AudioId = button.data('audioid'); // Extract info from data-audioId attributes
//    var modal = $(this);
//    //modal.find('.modal-title').text('New message to ' + recipient);
//});
//$(".LoadAudio").on("click", function (event) {
//    var tr = $(this);
//    var Id = tr.data('id');
//    //var playList = $(Id).html();
//    //$(".amazingaudioplayer").html('<ul class="amazingaudioplayer-audios" style="display:none;"></ul>')
//    //$(".amazingaudioplayer-audios").html(playList);
//    $.get("/Admin/Audios", { PlaceId: Id })
//            .done(function (data) {
//                console.log(data);
//                $(".Player").html(data);
//                $(".Player").removeClass("hidden");
//            });
//});
//$("#NewCityForm").submit(function (event) {
//    // Stop form from submitting normally
//    event.preventDefault();

//    // Get some values from elements on the page:
//    var $form = $(this),
//      CityName = $form.find("input[name='NewCity.CityName']").val(),
//      CityDesc = $form.find("textarea[name='NewCity.CityDesc']").val(),
//      url = $form.attr("action");
//    if (CityName.length > 0) {
//        $("#NewCityValidation").addClass("hidden");

//        // Send the data using post
//        var posting = $.post(url, { CityName: CityName, CityDesc: CityDesc });

//        // Put the results in a div
//        posting.done(function (data) {
//            if (data.success) {
//                location.reload();
//                location.replace(location.href.split('#')[0] + "#Cities");
//            }
//        });
//    }
//    else
//        $("#NewCityValidation").removeClass("hidden");
//});
//$(".rmvCty").on("click", function (event) {
//    var button = $(this); // Button that Clicked
//    var CityId = button.data('cityid'); // Extract info from data-CityId attribute
//    var url = $("#ctyTbl").data('action');
//    var posting = $.post(url, { Id: CityId });
//    posting.done(function (data) {
//        switch (data.status) {
//            case 0:
//                $("#rmvError").empty();
//                location.reload();
//                location.replace(location.href.split('#')[0] + "#Cities");
//                break;
//            case 1:
//                var CityName = button.data('cityname');
//                var btn = $("#ForignKeyErrorModal_Delete");
//                btn.addClass("hidden");
//                $("#ForignKeyErrorModal_body").empty().append('<div class="container text-info">This city (<span class="text-danger">' + CityName + '</span>) has one or more sub-places.<br/>To remove this city, first you have to delete it\'s sub-places.</div>');
//                $('#ForignKeyErrorModal').modal('show');
//                break;
//            case 2:
//                var btn = $("#ForignKeyErrorModal_Delete");
//                btn.addClass("hidden");
//                $("#ForignKeyErrorModal_body").empty().append('<div class="container text-info">Error in deletting city. Contact site developer to get more information.</div>');
//                $('#ForignKeyErrorModal').modal('show');
//                break;
//            default:

//        }
//        if (data.status) {
//            location.replace(location.href.split('#')[0] + "#ctyTbl");
//        }
//    });

//});
//$(".rmvPlc").on("click", function (event) {
//    var button = $(this); // Button that Clicked
//    var PlaceId = button.data('placeid'); // Extract info from data-CityId attribute
//    var url = $("#placeTbl").data('action');
//    console.log(PlaceId);
//    var posting = $.post(url, { Id: PlaceId });
//    posting.done(function (data) {
//        console.log(data);
//        console.log(data.status);
//        switch (data.status) {
//            case 0:
//                $("#rmvError").empty();
//                location.reload();
//                location.replace(location.href.split('#')[0] + "#PlaceList");
//                break;
//            case 1:
//                var PlaceName = button.data('placename');
//                var btn = $("#ForignKeyErrorModal_Delete");
//                btn.removeClass("hidden");
//                btn.addClass("removeClassWithSubs");
//                btn.data('id', PlaceId);
//                $("#ForignKeyErrorModal_body").empty().append('<div class="container text-info">This place (<span class="text-danger">' + PlaceName + '</span>) has one or more audios.<br/>Do you want to delete it with all of it\'s audios?</div>');
//                $('#ForignKeyErrorModal').modal('show');
//                break;
//            case 2:
//                var btn = $("#ForignKeyErrorModal_Delete");
//                btn.addClass("hidden");
//                $("#ForignKeyErrorModal_body").empty().append('<div class="container text-info">Error in deletting city. Contact site developer to get more information.</div>');
//                $('#ForignKeyErrorModal').modal('show');
//                break;
//            default:

//        }
//        if (data.status) {
//            location.replace(location.href.split('#')[0] + "#ctyTbl");
//        }
//    });
//});
//});