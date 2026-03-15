
define(function () {
    return {
        HeightAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = animation.Height;

            if (processAnimationValue(scope, animation, value, screenId)) {

                // if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                executeHeight(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.HeightAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        HeightAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (!animation[propAnimation])
                return;
            animation[propAnimation].reset();
        }
    };
});

function executeHeight(scope, animation, control, value) {
    var bFirstTime;
    if (!animation.data) {
        bFirstTime = true;
        animation.data = {
            control: control.node instanceof SVGGElement ? SVG($(control.node).find("svg")[0]) : control
        };
        animation.data.startingHeight = animation.data.control.height();
    }

    if (animation.targetValue === 0 && (animation.Repeatable || animation.Autoreverse)) {
        if (animation[propAnimation]) {
            animation[propAnimation].finish();
            animation[propAnimation].unschedule();
        }
        if (animation.data.startingHeight)
            animation.data.control.height(animation.data.startingHeight);
        return;
    }

    if (animation.Repeatable && animation[propAnimation])
        animation[propAnimation].unschedule();

    var startingH = bFirstTime ? animation.data.startingHeight : animation.data.control.height();

    if (!animation.data.control.hasClass("animated") && animation.targetValue === startingH)
        return;
    animation.data.control.addClass("animated");

    var animator = animation.data.control.animate(animation.AnimationTime);

    animation[propAnimation] = animator.height(Math.max(0.00000000000001, animation.targetValue)); //SVG.js 0-height bug

    if (animation.Repeatable && animation.targetValue !== 0)
        animation[propAnimation].loop(true, false);

    if (animation.Autoreverse)
        animation[propAnimation].after(function (sit) {
            animator.animate(animation.AnimationTime).height(startingH);
        });
}
