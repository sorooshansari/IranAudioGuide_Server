//Developed by pourmand
userApp.controller('userController', ['$scope', 'userServices', '$timeout', function ($scope, userServices, $timeout) {
    userServices.getUser().then(function (data) {
        console.log(data);
        $scope.user = data;
    });
    $scope.deactivateMobile = function () {
        userServices.deactivateMobile();
    };
    $scope.getPalaceForCity = function (city) {

        $scope.city = city;
        //userServices.getPalaceForCity(id).then(function (data) {

        //    $scope.palces = data;

        //});
    };

    $scope.isDisplayPakages = false;

    userServices.getPackages().then(function (data) {
        console.log(data);

        $scope.packages = data;

        $timeout(function () {
            $scope.isDisplayPakages = true;
            nav();
        }, 2000);
    })
    var nav = function () {
        $('.gw-nav > li > a').click(function () {
            var gw_nav = $('.gw-nav');
            gw_nav.find('li').removeClass('active');
            $('.gw-nav > li > ul > li').removeClass('active');

            var checkElement = $(this).parent();
            var ulDom = checkElement.find('.gw-submenu')[0];

            if (ulDom == undefined) {
                checkElement.addClass('active');
                $('.gw-nav').find('li').find('ul:visible').slideUp();
                return;
            }
            if (ulDom.style.display != 'block') {
                gw_nav.find('li').find('ul:visible').slideUp();
                gw_nav.find('li.init-arrow-up').removeClass('init-arrow-up').addClass('arrow-down');
                gw_nav.find('li.arrow-up').removeClass('arrow-up').addClass('arrow-down');
                checkElement.removeClass('init-arrow-down');
                checkElement.removeClass('arrow-down');
                checkElement.addClass('arrow-up');
                checkElement.addClass('active');
                checkElement.find('ul').slideDown(300);
            } else {
                checkElement.removeClass('init-arrow-up');
                checkElement.removeClass('arrow-up');
                checkElement.removeClass('active');
                checkElement.addClass('arrow-down');
                checkElement.find('ul').slideUp(300);

            }
        });
        $('.gw-nav > li > ul > li > a').click(function () {
            $(this).parent().parent().parent().removeClass('active');
            $('.gw-nav > li > ul > li').removeClass('active');
            $(this).parent().addClass('active')
        });
    };
}])
