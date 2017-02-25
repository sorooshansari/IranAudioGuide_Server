angular.module('AdminPage', ['AdminPage.controllers', 'AdminPage.services', 'AdminPage.directives', 'angular-ladda'])
.config(function ($sceProvider, laddaProvider) {
    $sceProvider.enabled(false);
    laddaProvider.setOption({ /* optional */
        style: 'expand-left',
        spinnerSize: 35,
        spinnerColor: '#ffffff'
    });
})
.run(function () {

});