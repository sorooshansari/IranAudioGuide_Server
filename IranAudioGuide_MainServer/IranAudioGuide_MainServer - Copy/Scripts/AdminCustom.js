angular.module('AdminPage', ['AdminPage.controllers', 'AdminPage.services', 'AdminPage.directives'])
.config(function ($sceProvider) {
    $sceProvider.enabled(false);
})
.run(function () {

});