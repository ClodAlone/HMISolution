
define(function () {
    return {
        VisibilityAnimationPreInit: function (scope, animation, control) {
            control.addClass(animationHiddenClass).hide();
        },

        VisibilityAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = animation.CompareValue;

            if (processAnimationValue(scope, animation, value, screenId)) {

                // if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                executeVisibility(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.VisibilityAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        VisibilityAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (!animation[propAnimation])
                return;
            animation[propAnimation].reset();
        }
    };
});

function executeVisibility(scope, animation, control, value) {
    var result = false;
    switch (animation.CompareMode) {
        case 0 /*CompareModes.Equal*/: result = animation.targetValue === animation.CompareValue; break;
        case 1 /*CompareModes.Major*/: result = animation.targetValue > animation.CompareValue; break;
        case 2 /*CompareModes.Minor*/: result = animation.targetValue < animation.CompareValue; break;
        case 3 /*CompareModes.MajorEqual*/: result = animation.targetValue >= animation.CompareValue; break;
        case 4 /*CompareModes.MinorEqual*/: result = animation.targetValue <= animation.CompareValue; break;
        case 5 /*CompareModes.NotEqual*/: result = animation.targetValue !== animation.CompareValue; break;
    }

    fastdom.measure(function () {
        if (result) {
            fastdom.mutate(function () {
                control.removeClass(animationHiddenClass);
                checkControlSetVisible(control);
            });
        }
        else {
            fastdom.mutate(function () {
                control.addClass(animationHiddenClass).hide();
            });
        }
    });
}
