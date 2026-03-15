
define(function () {
    return {
        ScaleAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = animation.Scale;

            if (processAnimationValue(scope, animation, value, screenId)) {

                // if (!animation[propNameExecuted]) {
                animation[propNameExecuted] = true;

                executeScale(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.ScaleAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        ScaleAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (!animation[propAnimation])
                return;
            animation[propAnimation].reset();
            if (animation[webKitDeregisterListener])
                animation[webKitDeregisterListener]();
        }
    };
});

function executeScale(scope, animation, control, value) {
    if (!animation.data) {
        animation.data = {
            node: $(control.node)
        };
    }

    if (!animation.data.node.hasClass("animated") && animation.targetValue === 1)
        return;
    animation.data.node.addClass("animated");
    
    //var bWebKitRenderCorrection = animation.data.node.hasClass("webKitRenderCorrection");
    var bRepeats = animation.Repeatable && animation.targetValue !== 0;
    var oX, oY;
    if (animation.RenderTransformOrigin) {
        oX = animation.RenderTransformOrigin._x;
        oY = animation.RenderTransformOrigin._y;
    }

    if (animation[propAnimation]) {
        animation[propAnimation].reset();
        animation[propAnimation].unschedule();
        if (animation[webKitDeregisterListener])
            animation[webKitDeregisterListener]();
    }

    if (animation.targetValue === 0 && (animation.Repeatable || animation.Autoreverse)) {
        if (animation[propAnimation]) {
            animation[propAnimation].finish();
            animation[propAnimation].unschedule();
            if (animation[webKitDeregisterListener])
                animation[webKitDeregisterListener]();
        }
        switch (animation.ScaleType) {
            case 0:
                control.transform("scale", 1);
                break;
            case 1:
                control.transform("scaleX", 1);
                break;
            case 2:
                control.transform("scaleY", 1);
                break;
        }
        return;
    }

    if (animation.targetValue === 0.0)
        animation.targetValue = 0.000000001; // avoid svg.js default faulty animation 

    var animator = control.animate(animation.AnimationTime);

    var startingS;
    var startingX = control.transform().originX;
    var startingY = control.transform().originY;
    
    switch (animation.ScaleType) {

        case 0 /*Both*/:
            startingS = control.transform().scale;
            animation[propAnimation] = animator.transform({ scale: animation.targetValue, originX: oX, originY: oY });
            break;

        case 1 /*X*/:
            startingS = control.transform().scaleX;
            animation[propAnimation] = animator.transform({ scaleX: animation.targetValue, originX: oX, originY: oY });
            break;

        case 2 /*Y*/:
            startingS = control.transform().scaleY;
            animation[propAnimation] = animator.transform({ scaleY: animation.targetValue, originX: oX, originY: oY });
            break;
    }

    if (bRepeats)
        animation[propAnimation].loop(true, false);

    if (animation.Autoreverse)
        animation[propAnimation].after(function (sit) {
            switch (animation.ScaleType) {
                case 0 /*Both*/:
                    animator.animate(animation.AnimationTime).transform({ scale: startingS, originX: startingX, originY: startingY });
                    break;

                case 1 /*X*/:
                    animator.animate(animation.AnimationTime).transform({ scaleX: startingS, originX: startingX, originY: startingY });
                    break;

                case 2 /*Y*/:
                    animator.animate(animation.AnimationTime).transform({ scaleY: startingS, originX: startingX, originY: startingY });
                    break;
            }
        });
}
