angular.module('UserPage', ['UserPage.controllers', 'UserPage.services', 'UserPage.directives'])
.config(function ($sceProvider) {
    $sceProvider.enabled(false);
})
.run(function () {

});