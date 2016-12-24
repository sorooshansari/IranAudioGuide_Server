angular.module('HomePage', ['HomePage.controllers',"scrollSpyModule", "countUpModule"])
.config(function ($sceProvider) {
    $sceProvider.enabled(false);
})
.run(function () {

});