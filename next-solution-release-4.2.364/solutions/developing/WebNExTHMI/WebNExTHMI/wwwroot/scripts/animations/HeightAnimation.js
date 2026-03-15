
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
    if (control.node instanceof SVGGElement)
        control = SVG($(control.node).find("svg")[0]);
    if (!control.attr("startingHeight"))
        control.attr("startingHeight", control.height());

    if (animation.targetValue === 0 && (animation.Repeatable || animation.Autoreverse)) {
        if (animation[propAnimation]) {
            animation[propAnimation].finish();
            animation[propAnimation].unschedule();
        }
        var startingHeight = control.attr("startingHeight");
        if (startingHeight)
            control.height(startingHeight);
        return;
    }

    if (animation.Repeatable && animation[propAnimation])
        animation[propAnimation].unschedule();

    var startingH = control.height();

    if (!control.hasClass("animated") && animation.targetValue === startingH)
        return;
    control.addClass("animated");

    var animator = control.animate(animation.AnimationTime);

    animation[propAnimation] = animator.height(Math.max(0.00000000000001, animation.targetValue)); //SVG.js 0-height bug

    if (animation.Repeatable && animation.targetValue !== 0)
        animation[propAnimation].loop(true, false);

    if (animation.Autoreverse)
        animation[propAnimation].after(function (sit) {
            animator.animate(animation.AnimationTime).height(startingH);
        });
}
