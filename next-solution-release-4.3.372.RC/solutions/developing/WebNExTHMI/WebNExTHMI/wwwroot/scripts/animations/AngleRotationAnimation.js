const previousAngle = 'previousAngle';

define(function () {
    return {
        AngleRotationAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = animation.TargetAngle;
            if (!animation[previousAngle])
                animation[previousAngle] = 0;

            if (processAnimationValue(scope, animation, value, screenId)) {

                // if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                executeAngleRotation(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.AngleRotationAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        AngleRotationAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (!animation[propAnimation])
                return;
            animation[propAnimation].reset();
        }
    };
});

function executeAngleRotation(scope, animation, control, value) {

    if (animation.targetValue === 0 && animation[propAnimation] && (animation.Repeatable || animation.Autoreverse)) {
        animation[propAnimation].unschedule();
        control.transform({ rotation: 0 });
        animation[previousAngle] = 0;
        return;
    }

    if ((animation.Repeatable || animation.Autoreverse) && animation[propAnimation]) {
        animation[propAnimation].finish();
        animation[propAnimation].unschedule();
    }

    var animator = control.animate(animation.AnimationTime).ease("-");

    var diff = animation.targetValue - animation[previousAngle];

    animation[propAnimation] = animator.rotate(diff);
    animation[previousAngle] = animation.targetValue;
    
    if (animation.Repeatable)
        animation[propAnimation].loop(true, false);
    if (animation.Autoreverse) {
        animation[propAnimation].after(function (sit) {
            var reverseRunner = control.animate(animation.AnimationTime).ease("-");
            animation[propAnimation] = reverseRunner.rotate(-diff);
        });
    }
}
