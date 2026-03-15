
const animationpath = './scripts/animations/';
const propNameExecuted = "executed";
const propAnimation = "animation";
const webKitDeregisterListener = "webKitDeregisterListener";

var moduleLoaded = {};


function initAnimations($scope) {

    $scope.preInitAnimation = function (animation, control) {
        try {
            var module = animationpath + animation.__type + '.js';

            if (moduleLoaded[module]) {
                if (!moduleLoaded[module][animation.__type + 'PreInit'])
                    return;
                return moduleLoaded[module][animation.__type + 'PreInit']($scope, animation, control);
            }

            require([module], function (moduleAnimation) {
                moduleLoaded[module] = moduleAnimation;
                if (!moduleAnimation[animation.__type + 'PreInit'])
                    return;
                moduleAnimation[animation.__type + 'PreInit']($scope, animation, control);
            });
        } catch (e) {
            console.log(e.toString());
        }
    }

    $scope.executeAnimation = function (animation, control, value, screenId) {

        try {
            var module = animationpath + animation.__type + '.js';

            if (moduleLoaded[module])
                return moduleLoaded[module][animation.__type + 'Execute']($scope, animation, control, value, screenId);

            require([module], function (moduleAnimation) {
                moduleLoaded[module] = moduleAnimation;
                moduleAnimation[animation.__type + 'Execute']($scope, animation, control, value, screenId);
            });
        } catch (e) {

            console.log(e.toString());
            // $scope.showMesageBox(i18next.t('AnimationNotAvailable') + ' : ' + animation.__type);
        }
    };

    $scope.stopAnimation = function (animation, control, screenId) {

        try {
            var module = animationpath + animation.__type + '.js';

            if (moduleLoaded[module])
                return moduleLoaded[module][animation.__type + 'Stop']($scope, animation, control, screenId);

            require([module], function (moduleAnimation) {
                moduleLoaded[module] = moduleAnimation;
                moduleAnimation[animation.__type + 'Stop']($scope, animation, control, screenId);
            });
        } catch (e) {

            console.log(e.toString());
            // $scope.showMesageBox(i18next.t('AnimationNotAvailable') + ' : ' + animation.__type);
        }
    };
}

function processAnimationValue(scope, animation, value, screenId) {

    if (!(screenId in scope.dataValues) || !(animation.SVGReferenceId in scope.dataValues[screenId]))
        return false;

    if (!scope.dataValues[screenId][animation.SVGReferenceId].isGood || isNaN(value))
        return false;
    if (animation.lastValue === value)
        return false;

    animation.lastValue = value;

    switch (animation.AnimationBehavior) {
        case /*'Trigger'*/0:
        {
            if (value !== 0) {
                animation.targetValue = animation.commonTarget;
                return true;
            }
            break;
        }
        case /*'Absolute'*/2:
        {
            animation.targetValue = value;
            if (animation.rangeLimit && animation.Range) {
                animation.targetValue = Math.max(animation.targetValue, animation.Range.Low);
                animation.targetValue = Math.min(animation.targetValue, animation.Range.High);
            }
            return true;
        }
        case /*'Proportional'*/1:
        {
            animation.targetValue = (value - animation.Range.Low) / (animation.Range.High - animation.Range.Low) * animation.commonTarget;
            if (animation.rangeLimit && animation.Range) {
                animation.targetValue = Math.max(animation.targetValue, animation.range.Low);
                animation.targetValue = Math.min(animation.targetValue, animation.range.High);
            }
            if (isNaN(animation.targetValue))
                animation.targetValue = 0;
            return true;
        }
    }

    return false;
}

var colorAnimation = {
    initAnimationData: function (animation, control) {
        animation.data = {
            controlNode: $(control.node),
            svgnode: SVG(control.node)
        };
        animation.data.useTagBG = animation.data.controlNode.attr("usetagbgforstroke");
        animation.data.bInterpolate = animation.data.controlNode.attr("backanimationproperty");
        animation.data.controlNodeChildren = animation.data.controlNode.children();
        if (animation.data.useTagBG)
            this.buildAnimationChildren(animation, control);

        var nodeIsG = control.node instanceof SVGGElement;
        animation.data.filledControls = animation.data.useTagBG ? animation.data.children : (nodeIsG ? [] : [control]);

        if (!animation.data.useTagBG && nodeIsG && animation.ApplyToAllChild) {
            animation.data.filledControls = [];
            $.each(animation.data.controlNodeChildren, function (i, e) {
                if ($(e).is("[iscomposed]"))
                    $(e).find("svg").children().each(function (i, el) {
                        animation.data.filledControls.push(SVG(el));
                    });
                else
                    animation.data.filledControls.push(SVG(e));
            });
        }
    },
    buildAnimationChildren: function (animation, control) {
        if (!animation.data.children) {
            var svgnode = SVG(control.node);
            var filledNodes = animation.ApplyToAllChild ? svgnode.find("*") : svgnode.find("[tag='BG']");
            animation.data.children = filledNodes;
        }
    }
}

var moveAnimation = {
    execute: function (animation, control, axis) {
        if (animation.Repeatable && animation[propAnimation]) {
            animation.data.lastTargetValue = axis == "x" ? control.transform().translateX : control.transform().translateY;
            animation[propAnimation].unschedule();
        }

        if (animation.data === undefined) {
            var node = $(control.node);
            animation.data = {
                node: node,
                lastTargetValue: 0
            };
        }

        if (!animation.data.node.hasClass("animated") && animation.targetValue === 0)
            return;
        animation.data.node.addClass("animated");

        var startingPos = axis == "x" ? control.transform().translateX : control.transform().translateY;

        var animator = control.animate(animation.AnimationTime, 0, 'now');

        let newTransform = {};
        newTransform[axis == "x" ? "translateX" : "translateY"] = animation.targetValue - animation.data.lastTargetValue;
        animation[propAnimation] = animator.transform(newTransform, true);

        animation.data.lastTargetValue = animation.targetValue;

        if (animation.Repeatable && animation.targetValue !== 0)
            animation[propAnimation].loop(true, false);

        if (animation.Autoreverse && animation.targetValue != 0)
            animation[propAnimation].after(function (sit) {
                let newTransform = {};
                newTransform[axis == "x" ? "translateX" : "translateY"] = startingPos - animation.data.lastTargetValue;
                animator.animate(animation.AnimationTime, 0, 'now').transform(newTransform, true);
                animation.data.lastTargetValue = startingPos;
            });
    }
};