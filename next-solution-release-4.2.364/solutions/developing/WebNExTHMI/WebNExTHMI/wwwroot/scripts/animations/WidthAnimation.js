
define(function () {
    return {
        WidthAnimationExecute: function (scope, animation, control, value, screenId) {
            
            animation.commonTarget = animation.Width;

            if (processAnimationValue(scope, animation, value, screenId)) {

                // if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                executeWidth(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.WidthAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        WidthAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (!animation[propAnimation])
                return;
            animation[propAnimation].reset();
        }
    };
});

function executeWidth(scope, animation, control, value) {
    if (control.node instanceof SVGGElement)
        control = SVG($(control.node).find("svg")[0]);

    if (!control.attr("startingWidth"))
        control.attr("startingWidth", control.width());

    if (animation.targetValue === 0 && (animation.Repeatable || animation.Autoreverse)) {
        if (animation[propAnimation]) {
            animation[propAnimation].finish();
            animation[propAnimation].unschedule();
        }
        var startingWidth = control.attr("startingWidth");
        if (startingWidth)
            control.width(startingWidth);
        return;
    }

    if (animation.Repeatable && animation[propAnimation])
        animation[propAnimation].unschedule();

    var startingW = control.width();

    if (!control.hasClass("animated") && animation.targetValue === startingW)
        return;
    control.addClass("animated");

    var animator = control.animate(animation.AnimationTime);

    animation[propAnimation] = animator.width(Math.max(0.00000000000001, animation.targetValue)); //SVG.js 0-width bug

    if (animation.Repeatable && animation.targetValue !== 0)
        animation[propAnimation].loop(true, false);

    if (animation.Autoreverse)
        animation[propAnimation].after(function (sit) {
            animator.animate(animation.AnimationTime).width(startingW);
        });
}
