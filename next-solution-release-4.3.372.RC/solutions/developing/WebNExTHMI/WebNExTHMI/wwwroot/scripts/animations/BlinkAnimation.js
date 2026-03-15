const previousOpacity = 'previousOpacity';

define(function () {
    return {
        BlinkAnimationExecute: function (scope, animation, control, value, screenId) {

            let opacityAnimation = animation.OpacityFrom !== animation.OpacityTo;
            let scaleAnimation = animation.ScaleTo !== 1;

            if (scaleAnimation || opacityAnimation) {

                animation.commonTarget = animation.OpacityTo;

                if (processAnimationValue(scope, animation, value, screenId)) {

                    if (opacityAnimation) {
                        if (!animation[previousOpacity])
                            animation[previousOpacity] = control.attr('opacity');
                        control.opacity(animation.OpacityFrom);
                    }

                    // if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                    executeBlinkOpacityScale(scope, animation, control, value, scaleAnimation, opacityAnimation);
                    // }
                }
                else if (opacityAnimation && animation[propNameExecuted]) {
                    this.BlinkAnimationStop(scope, animation, control, screenId);
                    animation[propNameExecuted] = false;
                }
            }
        },

        BlinkAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            control.timeline().stop();
            if (animation[propAnimation])
                animation[propAnimation].reset();
            if (animation[previousOpacity])
                control.opacity(animation[previousOpacity]);
        }
    };
});

function executeBlinkOpacityScale(scope, animation, control, value, scaleAnimation, opacityAnimation) {
    animation[propAnimation] = control.animate(animation.AnimationTime);

    if (opacityAnimation) {
        if (animation[propAnimation])
            animation[propAnimation].reset();

        control.attr('opacity', animation.OpacityFrom);
        animation[propAnimation].opacity(animation.targetValue);
    }
    if (scaleAnimation) {
        var oX, oY;
        if (animation.RenderTransformOrigin) {
            oX = animation.RenderTransformOrigin._x;
            oY = animation.RenderTransformOrigin._y;
        }

        if (animation.ScaleTo === 0.0)
            animation.ScaleTo = 0.000000001; // avoid svg.js default faulty animation 

        animation[propAnimation].transform({ scale: animation.ScaleTo, originX: oX, originY: oY });
    }
    animation[propAnimation].loop(true, true);
}
