const previousFill = 'previousFill';

define(function () {
    return {
        BackColorAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = 1;

            if (processAnimationValue(scope, animation, value, screenId)) {

                if (!animation[previousFill])
                    animation[previousFill] = control.attr('fill');

                //if (!animation[propNameExecuted]) {
                animation[propNameExecuted] = true;

                executeBackColor(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.BackColorAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        BackColorAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (animation[propAnimation] && animation[propAnimation].length > 0)
                $.each(animation[propAnimation], function (i, e) { e.reset(); });
            if (animation[previousFill])
                control.fill(animation[previousFill]);
        }
    };
});

function executeBackColor(scope, animation, control, value) {

    var blinkElement = function (animation, e, blinkColor, colorBlinkOpacity) {
        var blinkColor = animation.data.bInterpolate ? colorHelpers.interpolateColors(control.attr("strokecolor"), blinkColor, 1 - e.attr("colorinterpolationpercentiage"), false) : blinkColor;
        if (!animation[propAnimation])
            animation[propAnimation] = [];
        if (!animation.data.bInterpolate)
            try {
                animation[propAnimation].push(
                    e.animate(animation.AnimationTime)
                        .fill({ color: blinkColor, opacity: colorBlinkOpacity })
                        .loop(999999999999, true) //SVG.js unstoppable eternal loop bug
                );
            }
            catch { }
        else
            try {
                animation[propAnimation].push(
                    e.animate(animation.AnimationTime)
                        .css({ "stroke": blinkColor, "stroke-opacity": colorBlinkOpacity })
                        .loop(999999999999, true) //SVG.js unstoppable eternal loop bug
                );
            }
            catch { }
    };

    if (!control.attr("originalFill"))
        control.attr("originalFill", control.attr("fill"));

    if (animation.listColors.length === 0)
        return;

    if (animation[propAnimation] && animation[propAnimation].length > 0) {
        $.each(animation[propAnimation], function (i, e) {
            if (e.timeline()) {
                e.timeline().stop(true, true);
                e.timeline().finish();
            }
        });
        animation[propAnimation] = [];
    }

    if (!animation.data)
        colorAnimation.initAnimationData(animation, control);

    var indexcolor = -1;
    for (var i = 0; i < animation.listColors.length; ++i) {
        if (animation.listColors[i].valueColor > animation.targetValue)
            break;
        indexcolor = i;
    }
    if (indexcolor < 0) {
        if (!animation.data.children)
            colorAnimation.buildAnimationChildren(animation, control);
        for (var j = 0; j < animation.data.children.length; j++) {
            if (!animation.data.bInterpolate) {
                try {
                    var classes = $(animation.data.children[j].node).attr("_class");
                    if (classes)
                        $(animation.data.children[j].node).attr("class", classes);
                    animation.data.children[j].fill(animation.data.children[j].attr("originalFill"));
                }
                catch { }
            }
            else
                animation.data.children[j].css("stroke", "");
        }
        return;
    }

    var colorOpacity = animation.listColors[indexcolor].colorOpacity;

    var bBlinks = false;
    var blinkColor = animation.listColors[indexcolor].colorBlink;
    var colorBlinkOpacity = animation.listColors[indexcolor].colorBlinkOpacity;
    if (blinkColor === '#000000') {
        if (colorOpacity > 0) {
            $(animation.data.filledControls).each(function (i, e) {
                try {
                    if (!animation.data.bInterpolate)
                        e.fill({ color: animation.listColors[indexcolor].color, opacity: colorOpacity });
                    else {
                        var strokeColor = colorHelpers.interpolateColors(animation.listColors[indexcolor].color, control.attr("strokecolor"), e.attr("colorinterpolationpercentiage"));
                        e.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
                    }
                }
                catch { }
            });
        }
        else {
            $(animation.data.filledControls).each(function (i, e) {
                try {
                    if (!animation.data.bInterpolate)
                        e.fill(control.attr("originalFill"));
                    else
                        e.css("stroke", "");
                }
                catch { }
            });
        }
    }
    else {
        bBlinks = colorBlinkOpacity > 0;
        if (colorOpacity > 0) {
            $(animation.data.filledControls).each(function (i, e) {
                if (!animation.data.bInterpolate) {
                    try {
                        var classes = $(animation.data.children[j].node).attr("class");
                        if (classes)
                            $(animation.data.children[j].node).attr("_class", classes).removeClass();
                        animation.data.children[j].fill({ color: animation.listColors[indexcolor].color, opacity: colorOpacity });
                    }
                    catch { }
                }
                else {
                    var strokeColor = colorHelpers.interpolateColors(animation.listColors[indexcolor].color, control.attr("strokecolor"), e.attr("colorinterpolationpercentiage"), !bBlinks);
                    e.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
                }
            });
        }
        else {
            $(animation.data.filledControls).each(function (i, e) {
                if (!animation.data.bInterpolate)
                    e.fill(control.attr("originalFill"));
                else
                    e.css("stroke", "");
            });
        }

        if (bBlinks) {
            $(animation.data.filledControls).each(function (i, e) {
                blinkElement(animation, e, blinkColor, colorBlinkOpacity);
            });
        }
    }
    colorAnimation.buildAnimationChildren(animation, control);
    for (var j = 0; j < animation.data.children.length; j++) {
        if (!animation.data.children[j].attr("originalFill"))
            animation.data.children[j].attr("originalFill", animation.data.children[j].attr("fill"));
        if (colorOpacity > 0) {
            if (!animation.data.bInterpolate) {
                try {
                    var classes = $(animation.data.children[j].node).attr("class");
                    if (classes)
                        $(animation.data.children[j].node).attr("_class", classes).removeClass();
                    animation.data.children[j].fill({ color: animation.listColors[indexcolor].color, opacity: colorOpacity });
                }
                catch { }
            }
            else {
                var strokeColor = animation.data.bInterpolate ? colorHelpers.interpolateColors(control.attr("strokecolor"), animation.listColors[indexcolor].color, 1 - animation.data.children[j].attr("colorinterpolationpercentiage"), !bBlinks) : animation.listColors[indexcolor].color;
                animation.data.children[j].css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
            }
        }
        else {
            if (!animation.data.bInterpolate) {
                try {
                    var classes = $(animation.data.children[j].node).attr("class");
                    if (classes)
                        $(animation.data.children[j].node).attr("_class", classes).removeClass();
                    animation.data.children[j].fill(animation.data.children[j].attr("originalFill"));
                }
                catch { }
                $(animation.data.children[j].node).css("fill", animation.data.children[j].attr("originalFill"));
            }
            else
                animation.data.children[j].css("stroke", "");
        }
        if (bBlinks)
            blinkElement(animation, animation.data.children[j], blinkColor, colorBlinkOpacity);
    }
}