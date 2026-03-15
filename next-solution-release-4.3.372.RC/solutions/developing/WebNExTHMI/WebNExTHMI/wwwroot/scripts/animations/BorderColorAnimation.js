const previousStroke = 'previousStroke';

define(function () {
    return {
        BorderColorAnimationExecute: function (scope, animation, control, value, screenId) {

            animation.commonTarget = 1;

            if (processAnimationValue(scope, animation, value, screenId)) {

                if (!animation[previousStroke])
                    animation[previousStroke] = control.attr('stroke');

                //if (!animation[propNameExecuted]) {
                    animation[propNameExecuted] = true;

                    executeBorderColor(scope, animation, control, value);
                // }
            }
            //else if (animation[propNameExecuted]) {
            //    this.BorderColorAnimationStop(scope, animation, control);
            //    animation[propNameExecuted] = false;
            //}
        },

        BorderColorAnimationStop: function (scope, animation, control, screenId) {
            if (!animation[propNameExecuted])
                return;
            if (animation[propAnimation] && animation[propAnimation].length > 0)
                $.each(animation[propAnimation], function (i, e) { e.reset(); });
            if (animation[previousStroke])
                control.stroke(animation[previousStroke]);
        }
    };
});

function executeBorderColor(scope, animation, control, value) {
    if (!animation.data)
        colorAnimation.initAnimationData(animation, control);

    if (animation.listColors.length === 0)
        return;

    if (!control.attr("originalStroke"))
        control.attr("originalStroke", animation.data.controlNode.is("[stroke]") ? control.attr("stroke") : "transparent");

    if (animation[propAnimation] && animation[propAnimation].length > 0) {
        $.each(animation[propAnimation], function (i, e) {
            if (e.timeline()) {
                e.timeline().stop(true, true);
                e.timeline().finish();
            }
        });
        animation[propAnimation] = [];
    }

    var indexcolor = -1;
    for (var i = 0; i < animation.listColors.length; ++i) {
        if (animation.listColors[i].valueColor > animation.targetValue)
            break;
        indexcolor = i;
    }
    if (indexcolor < 0) {
        if (!animation.data.children)
            colorAnimation.buildAnimationChildren(animation, control);
        for (var j = 0; j < animation.data.children.length; j++)
            animation.data.children[j].css("stroke", animation.data.svgnode.attr("originalStroke"));
        return;
    }

    var colorOpacity = animation.listColors[indexcolor].colorOpacity;

    var colorBlinkOpacity = animation.listColors[indexcolor].colorBlinkOpacity;
    var bBlinks = colorBlinkOpacity > 0;
    if (colorOpacity > 0) {
        $(animation.data.filledControls).each(function (i, e) {
            var strokeColor = animation.data.bInterpolate ? colorHelpers.interpolateColors(control.attr("fillcolor"), animation.listColors[indexcolor].color, e.attr("colorinterpolationpercentiage"), !bBlinks) : animation.listColors[indexcolor].color;
            e.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
        });
    }
    else {
        $(animation.data.filledControls).each(function (i, e) {
            e.css("stroke", animation.data.svgnode.attr("originalStroke"));
        });
    }

    if (bBlinks) {
        $(animation.data.filledControls).each(function (i, e) {
            setBlinking(e);
        });
    }

    colorAnimation.buildAnimationChildren(animation, control);
    $.each(animation.data.children, function (index, item) {
        if (bBlinks)
            setBlinking(item);

        if (!item.attr("originalStroke"))
            item.attr("originalStroke", item.attr("stroke"));
        if (colorOpacity > 0) {
            var strokeColor = animation.data.bInterpolate ? colorHelpers.interpolateColors(control.attr("fillcolor"), animation.listColors[indexcolor].color, item.attr("colorinterpolationpercentiage"), !bBlinks) : animation.listColors[indexcolor].color;
            item.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
        }
        else
            item.css("stroke", animation.data.svgnode.attr("originalStroke"));
    });

    function setBlinking(item) {
        if (!animation[propAnimation])
            animation[propAnimation] = [];
        var blinkColor = animation.data.bInterpolate ? colorHelpers.interpolateColors(control.attr("fillcolor"), animation.listColors[indexcolor].colorBlink, item.attr("colorinterpolationpercentiage"), false) : animation.listColors[indexcolor].colorBlink;
        try {
            animation[propAnimation].push(
                item.animate(animation.listColors[indexcolor].blinkTime)
                    .css({ "stroke": blinkColor, "stroke-opacity": colorBlinkOpacity })
                    .loop(999999999999, true) //SVG.js unstoppable eternal loop bug
            );
        }
        catch { }
    }
}