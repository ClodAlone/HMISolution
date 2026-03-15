define(function () {
    return {
        FillingAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = animation.Offset;

            if (processAnimationValue(scope, animation, value, screenId)) {
                var gradient;
                var isRadial = animation.GradientType == 1;
                if (!isRadial) {
                    gradient = SVG("#" + screenId).gradient('linear', function (add) {
                        add.stop({ offset: 0, color: animation.StartColor, opacity: animation.StartColorOpacity })
                        add.stop({ offset: 0 + Number.EPSILON, color: animation.EndColor, opacity: animation.EndColorOpacity })
                        add.stop({ offset: 0 + Number.EPSILON * 2, color: "transparent" })
                    });
                }
                else {
                    gradient = SVG("#" + screenId).gradient('radial', function (add) {
                        add.stop({ offset: 0, color: animation.StartColor, opacity: animation.StartColorOpacity })
                        add.stop({ offset: 0 + Number.EPSILON, color: animation.EndColor, opacity: animation.EndColorOpacity })
                        add.stop(0 + Number.EPSILON * 2, "transparent")
                    });
                    if (animation.RadialRadiusX == animation.RadialRadiusY)
                        gradient.radius(parseFloat(animation.RadialRadiusX));
                    else
                        $(gradient.node).attr("gradientTransform", "scale(" + animation.RadialRadiusX + ", " + animation.RadialRadiusY + ")");
                }
                switch (animation.FillingBehavior) {
                    case 0: //bottomup
                        if (!isRadial)
                            gradient.from(0.5, 1).to(0.5, 0);
                        else
                            $(gradient.node).attr({ "cx": 1, "cy": 1, "fx": 1, "fy": 0 });
                        break;
                    case 1: //upbottom
                        if (!isRadial)
                            gradient.from(0.5, 0).to(0.5, 1);
                        else
                            $(gradient.node).attr({ "cx": 1, "cy": 0, "fx": 1, "fy": 1 });
                        break;
                    case 2: //leftright
                        if (!isRadial)
                            gradient.from(0, 0.5).to(1, 0.5);
                        else
                            $(gradient.node).attr({ "cx": 0, "cy": 1, "fx": 1 });
                        break;
                    case 3: //rightleft
                        if (!isRadial)
                            gradient.from(1, 0.5).to(0, 0.5);
                        else
                            $(gradient.node).attr({ "cx": 1, "cy": 1, "fx": 0 });
                        break;
                    case 4: //center
                        gradient.from(0.5, 0.5).to(0.5, 0.5);
                        break;
                    case 5: //custom
                        if (isRadial)
                            $(gradient.node).attr({ "cx": 0, "cy": 0, "fx": 0, "fy": 0 });
                        break;
                }

                animation[propNameExecuted] = true;
                executeFilling(scope, animation, control, value, gradient);
            }
        },

        FillingAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            resetFilling(animation);
        }
    };
});

function resetFilling(animation) {
    if (animation["fillingEnd"])
        animation["fillingEnd"].reset();
    //if (animation["fillingTransparent"])
    //    animation["fillingTransparent"].reset();
    if (animation["endInterval"])
        clearInterval(animation["endInterval"]);
}

function executeFilling(scope, animation, control, value, gradient) {
    resetFilling(animation);

    var startStop = gradient.get(0);
    var endStop = gradient.get(1);
    var transparentStop = gradient.get(2);

    control.attr({ fill: gradient });
    $(control.node).find("[tag='BG']").each(function (i, e) {
        $(e).attr({ fill: gradient });
    });

    if (animation.AnimationBehavior == 1) { //Proportional
        var percentValue = animation.targetValue;
        let endStopColor = mixColors(hexToRgb($(startStop.node).attr("stop-color")), hexToRgb($(endStop.node).attr("stop-color")), (1 - percentValue));
        endStop.update({ color: endStopColor });
        var lastOffset = 0;
        if (animation["lastOffset"])
            lastOffset = animation["lastOffset"];

        //bool bIsBackward;
        //var startAtTime = animation.AnimationTime - ((percentValue - lastOffset) * animation.AnimationTime);
        //if (startAtTime > animation.AnimationTime) bIsBackward = true;

        //endStop.update({ offset: percentValue });

        if (!animation.Repeatable) {
            endStop.update({ offset: percentValue });
            animation["fillingEnd"] = endStop.animate(animation.AnimationTime).ease('-').update({ offset: lastOffset }).reverse();
        }
        else {
            animation["endInterval"] = setInterval(animationLoop, animation.AnimationTime * 2);
            animationLoop();
        }
        animation["lastOffset"] = percentValue;
    }
    else if (animation.AnimationBehavior == 2) { //Absolute
        var lastOffset = 0;
        if (animation["lastOffset"])
            lastOffset = animation["lastOffset"];
        if (value == 0) {
            animation["fillingEnd"] = endStop.animate(animation.AnimationTime).ease('-').update({ offset: 1 }).reverse();
            //animation["fillingTransparent"] = transparentStop.animate(animation.AnimationTime).ease('-').update({ offset: 1 }).reverse();
        }
        else {
            let endStopColor = mixColors(hexToRgb($(startStop.node).attr("stop-color")), hexToRgb($(endStop.node).attr("stop-color")), (value >= 1 ? (1 - 1 / value) : 1 - value));

            var isRadial = animation.GradientType == 1;
            if (animation.FillingBehavior == 4 && !isRadial) {
                if (value > 1)
                    gradient.from(0.5, 0).to(0.5, 1);
                else
                    gradient.from(0.5, 0.5).to(0.5, 0.5);
            }

            if (value == 1) {
                endStop.update({ color: endStopColor });
                animation["fillingEnd"] = endStop.animate(animation.AnimationTime).ease('-').update({ offset: 1 });
                //animation["fillingTransparent"] = transparentStop.animate(animation.AnimationTime).ease('-').update({ offset: 1 });
            }
            else {
                if (isRadial && value > 1)
                    transparentStop.update({ color: endStopColor });
                else
                    transparentStop.update({ color: "transparent" });
                transparentStop.update({ offset: value < 1 ? value : 1 });
                endStop.update({ offset: value < 1 ? value : 1 });
                animation["fillingEnd"] = endStop.animate(animation.AnimationTime).ease('-').update({ color: endStopColor });
            }
            if (animation.Repeatable) {
                endStop.animate(animation.AnimationTime).ease('-').update({ offset: lastOffset }).after(
                    function () {
                        animation["endInterval"] = setInterval(animationLoop, animation.AnimationTime * 2);
                        animationLoop();
                    }
                )
            }
        }
        animation["lastOffset"] = value;
    }

    function animationLoop() {
        animation["fillingEnd"] = endStop.animate(animation.AnimationTime).ease('-').update({ offset: animation.targetValue })
            .after(function () {
                animation["fillingEnd"] = endStop.animate(animation.AnimationTime).ease('-').update({ offset: lastOffset });
            });
    }
}

function mixColors(color1, color2, weight) {
    var w1 = weight;
    var w2 = 1 - w1;
    var rgb = [Math.round(color1.r * w1 + color2.r * w2), Math.round(color1.g * w1 + color2.g * w2), Math.round(color1.b * w1 + color2.b * w2)];
    return rgbToHex(rgb[0], rgb[1], rgb[2]);
}

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

var rgbToHex = (function () {
    var componentToHex = function (c) {
        var hex = c.toString(16);
        return hex.length == 1 ? "0" + hex : hex;
    };
    return function (r, g, b) {
        return "#" + componentToHex(Math.min(255, Math.max(0, r))) + componentToHex(Math.min(255, Math.max(0, g))) + componentToHex(Math.min(255, Math.max(0, b)));
    };
})();