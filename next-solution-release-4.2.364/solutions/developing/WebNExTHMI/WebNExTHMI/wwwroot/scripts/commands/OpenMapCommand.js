
define(function () {
    return {
        OpenMapCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        OpenMapCommandExecute: function (scope, command, control, currentPopupId) {

            var options = {
                modal: scope.IsPopupModal(command),
                x: command.x, y: command.y, zoomTo: command.zoomTo, enableZoomingScrolling: command.enableZoomingScrolling};
            if (command.executionMode !== 0) {

                var thisPage = $(control.node).parent();
                var thisParent = thisPage.parent();

                options.width = "100%";
                options.height = "100%";
                options.notScrollable = true;
                scope.activatePopup('mapView', JSON.stringify(options));
            }
            else {

                scope.activateView('mapView', JSON.stringify(options));
            }
        },

        OpenMapCommandCanExecute: function (scope, command, control, screenId) {
                return true;
        }
    };
});
