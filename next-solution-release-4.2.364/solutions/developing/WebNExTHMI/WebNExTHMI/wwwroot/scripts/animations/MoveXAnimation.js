
define(function () {
    return {
        MoveXAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = animation.X;

            if (processAnimationValue(scope, animation, value, screenId)) {

                // if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                executeMoveX(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.MoveXAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        MoveXAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (!animation[propAnimation])
                return;
            animation[propAnimation].reset();
        }
    };
});

function executeMoveX(scope, animation, control, value) {
    moveAnimation.execute(animation, control, "x");
}
