

    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });
    function onResizeBody() {
        var width = $(window).width();
        var h = $(window).height();
        if (width > 768)
            if ($("#wrapper").hasClass("toggled"))
                $("#wrapper").removeClass("toggled")
    };
