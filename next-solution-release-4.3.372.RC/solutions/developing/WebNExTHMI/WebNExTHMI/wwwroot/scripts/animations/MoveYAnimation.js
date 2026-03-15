
define(function () {
    return {
        MoveYAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = animation.Y;

            if (processAnimationValue(scope, animation, value, screenId)) {

                // if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                executeMoveY(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.MoveYAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        MoveYAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (!animation[propAnimation])
                return;
            animation[propAnimation].reset();
        }
    };
});

function executeMoveY(scope, animation, control, value) {
    moveAnimation.execute(animation, control, "y");
}
