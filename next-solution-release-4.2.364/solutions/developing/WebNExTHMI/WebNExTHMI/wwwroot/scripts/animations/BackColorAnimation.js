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

    var controlNode = $(control.node);
    var useTagBG = controlNode.attr("usetagbgforstroke");
    var bInterpolate = controlNode.attr("backanimationproperty");
    if (useTagBG)
        buildAnimationChildren(animation, control, controlNode);

    var blinkElement = function (animation, e, blinkColor, colorBlinkOpacity) {
        var blinkColor = bInterpolate ? colorHelpers.interpolateColors(control.attr("strokecolor"), animation.listColors[indexcolor].colorBlink, 1 - e.attr("colorinterpolationpercentiage"), false) : animation.listColors[indexcolor].colorBlink;
        if (!animation[propAnimation])
            animation[propAnimation] = [];
        if (!bInterpolate)
            try {
                animation[propAnimation].push(
                    e.animate(animation.AnimationTime)
                        .fill({ color: animation.listColors[indexcolor].colorBlink, opacity: colorBlinkOpacity })
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

    if (!animation.filledControls) {
        animation.filledControls = useTagBG ? animation.children : (controlNode.is("g") ? [] : [control]);

        if (!useTagBG && control.node instanceof SVGGElement && animation.ApplyToAllChild) {
            animation.filledControls = [];
            $.each(controlNode.children(), function (i, e) {
                if ($(e).is("[iscomposed]"))
                    $(e).find("svg").children().each(function (i, el) {
                        animation.filledControls.push(SVG(el));
                    });
                else
                    animation.filledControls.push(SVG(e));
            });
        }
    }

    var indexcolor = -1;
    for (var i = 0; i < animation.listColors.length; ++i) {
        if (animation.listColors[i].valueColor > animation.targetValue)
            break;
        indexcolor = i;
    }
    if (indexcolor < 0) {
        if (!animation.children)
            buildAnimationChildren(animation, control, controlNode);
        for (var j = 0; j < animation.children.length; j++) {
            if (!bInterpolate) {
                try {
                    var classes = $(animation.children[j].node).attr("_class");
                    if (classes)
                        $(animation.children[j].node).attr("class", classes);
                    animation.children[j].fill(animation.children[j].attr("originalFill"));
                }
                catch { }
            }
            else
                animation.children[j].css("stroke", "");
        }
        return;
    }

    var colorOpacity = animation.listColors[indexcolor].colorOpacity;

    var bBlinks = false;
    var blinkColor = animation.listColors[indexcolor].colorBlink;
    var colorBlinkOpacity = animation.listColors[indexcolor].colorBlinkOpacity;
    if (animation.listColors[indexcolor].colorBlink === '#000000') {
        if (colorOpacity > 0) {
            $(animation.filledControls).each(function (i, e) {
                if (!bInterpolate)
                    e.fill({ color: animation.listColors[indexcolor].color, opacity: colorOpacity });
                else {
                    var strokeColor = colorHelpers.interpolateColors(animation.listColors[indexcolor].color, control.attr("strokecolor"), e.attr("colorinterpolationpercentiage"));
                    e.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
                }
            });
        }
        else {
            $(animation.filledControls).each(function (i, e) {
                try {
                    if (!bInterpolate)
                        e.fill(control.attr("originalFill"));
                    else
                        e.css("stroke", "");
                }
                catch { }
            });
        }
    }
    else {
        var colorBlinkOpacity = animation.listColors[indexcolor].colorBlinkOpacity;
        bBlinks = colorBlinkOpacity > 0;
        if (colorOpacity > 0) {
            $(animation.filledControls).each(function (i, e) {
                if (!bInterpolate)
                    e.fill({ color: animation.listColors[indexcolor].color, opacity: animation.listColors[indexcolor].colorOpacity });
                else {
                    var strokeColor = colorHelpers.interpolateColors(animation.listColors[indexcolor].color, control.attr("strokecolor"), e.attr("colorinterpolationpercentiage"), !bBlinks);
                    e.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
                }
            });
        }
        else {
            $(animation.filledControls).each(function (i, e) {
                try {
                    if (!bInterpolate)
                        e.fill(control.attr("originalFill"));
                    else
                        e.css("stroke", "");
                }
                catch { }
            });
        }

        if (bBlinks) {
            $(animation.filledControls).each(function (i, e) {
                blinkElement(animation, e, blinkColor, colorBlinkOpacity);
            });
        }
    }
    buildAnimationChildren(animation, control, controlNode);
    for (var j = 0; j < animation.children.length; j++) {
        if (!animation.children[j].attr("originalFill"))
            animation.children[j].attr("originalFill", animation.children[j].attr("fill"));
        if (colorOpacity > 0) {
            if (!bInterpolate) {
                try {
                    var classes = $(animation.children[j].node).attr("class");
                    if (classes)
                        $(animation.children[j].node).attr("_class", classes).removeClass();
                    animation.children[j].fill({ color: animation.listColors[indexcolor].color, opacity: colorOpacity });
                }
                catch { }
            }
            else {
                var strokeColor = bInterpolate ? colorHelpers.interpolateColors(control.attr("strokecolor"), animation.listColors[indexcolor].color, 1 - animation.children[j].attr("colorinterpolationpercentiage"), !bBlinks) : animation.listColors[indexcolor].color;
                animation.children[j].css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
            }
        }
        else {
            if (!bInterpolate) {
                try {
                    var classes = $(animation.children[j].node).attr("class");
                    if (classes)
                        $(animation.children[j].node).attr("_class", classes).removeClass();
                    animation.children[j].fill(animation.children[j].attr("originalFill"));
                }
                catch { }
                $(animation.children[j].node).css("fill", animation.children[j].attr("originalFill"));
            }
            else
                animation.children[j].css("stroke", "");
        }
        if (bBlinks)
            blinkElement(animation, animation.children[j], blinkColor, colorBlinkOpacity);
    }
}