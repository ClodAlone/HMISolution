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
    var controlNode = $(control.node);

    if (!control.attr("originalStroke"))
        control.attr("originalStroke", controlNode.is("[stroke]") ? control.attr("stroke") : "transparent");

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

    var svgnode = SVG(control.node);
    var useTagBG = controlNode.attr("usetagbgforstroke");
    var bInterpolate = controlNode.attr("backanimationproperty");
    if (useTagBG)
        buildAnimationChildren(animation, control, controlNode);

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
        for (var j = 0; j < animation.children.length; j++)
            animation.children[j].css("stroke", svgnode.attr("originalStroke"));
        return;
    }

    var colorOpacity = animation.listColors[indexcolor].colorOpacity;

    var colorBlinkOpacity = animation.listColors[indexcolor].colorBlinkOpacity;
    var bBlinks = colorBlinkOpacity > 0;
    if (colorOpacity > 0) {
        $(animation.filledControls).each(function (i, e) {
            var strokeColor = bInterpolate ? colorHelpers.interpolateColors(control.attr("fillcolor"), animation.listColors[indexcolor].color, e.attr("colorinterpolationpercentiage"), !bBlinks) : animation.listColors[indexcolor].color;
            e.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
        });
    }
    else {
        $(animation.filledControls).each(function (i, e) {
            e.css("stroke", svgnode.attr("originalStroke"));
        });
    }

    if (bBlinks) {
        $(animation.filledControls).each(function (i, e) {
            setBlinking(e);
        });
    }

    buildAnimationChildren(animation, control, controlNode);
    $.each(animation.children, function (index, item) {
        if (bBlinks)
            setBlinking(item);

        if (!item.attr("originalStroke"))
            item.attr("originalStroke", item.attr("stroke"));
        if (colorOpacity > 0) {
            var strokeColor = bInterpolate ? colorHelpers.interpolateColors(control.attr("fillcolor"), animation.listColors[indexcolor].color, svgnode.attr("colorinterpolationpercentiage"), !bBlinks) : animation.listColors[indexcolor].color;
            item.css({ "stroke": strokeColor, "stroke-opacity": colorOpacity });
        }
        else
            item.css("stroke", svgnode.attr("originalStroke"));
    });

    function setBlinking(item) {
        if (!animation[propAnimation])
            animation[propAnimation] = [];
        var blinkColor = bInterpolate ? colorHelpers.interpolateColors(control.attr("fillcolor"), animation.listColors[indexcolor].colorBlink, item.attr("colorinterpolationpercentiage"), false) : animation.listColors[indexcolor].colorBlink;
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