/**
 * DevExtreme (esm/ui/scheduler/appointments/viewModelGenerator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import VerticalAppointmentsStrategy from "./rendering_strategies/strategy_vertical";
import WeekAppointmentRenderingStrategy from "./rendering_strategies/strategy_week";
import HorizontalAppointmentsStrategy from "./rendering_strategies/strategy_horizontal";
import HorizontalMonthLineAppointmentsStrategy from "./rendering_strategies/strategy_horizontal_month_line";
import HorizontalMonthAppointmentsStrategy from "./rendering_strategies/strategy_horizontal_month";
import AgendaAppointmentsStrategy from "./rendering_strategies/strategy_agenda";
import {
    getAppointmentKey
} from "../../../renovation/ui/scheduler/appointment/utils";
import {
    getOverflowIndicatorColor
} from "../../../renovation/ui/scheduler/appointment/overflow_indicator/utils";
var RENDERING_STRATEGIES = {
    horizontal: HorizontalAppointmentsStrategy,
    horizontalMonth: HorizontalMonthAppointmentsStrategy,
    horizontalMonthLine: HorizontalMonthLineAppointmentsStrategy,
    vertical: VerticalAppointmentsStrategy,
    week: WeekAppointmentRenderingStrategy,
    agenda: AgendaAppointmentsStrategy
};
export class AppointmentViewModelGenerator {
    initRenderingStrategy(options) {
        var RenderingStrategy = RENDERING_STRATEGIES[options.appointmentRenderingStrategyName];
        this.renderingStrategy = new RenderingStrategy(options)
    }
    generate(filteredItems, options) {
        var {
            isRenovatedAppointments: isRenovatedAppointments
        } = options;
        var appointments = filteredItems ? filteredItems.slice() : [];
        this.initRenderingStrategy(options);
        var renderingStrategy = this.getRenderingStrategy();
        var positionMap = renderingStrategy.createTaskPositionMap(appointments);
        var viewModel = this.postProcess(appointments, positionMap, isRenovatedAppointments);
        if (isRenovatedAppointments) {
            return this.makeRenovatedViewModels(viewModel)
        }
        return {
            positionMap: positionMap,
            viewModel: viewModel
        }
    }
    postProcess(filteredItems, positionMap, isRenovatedAppointments) {
        var renderingStrategy = this.getRenderingStrategy();
        return filteredItems.map((data, index) => {
            if (!renderingStrategy.keepAppointmentSettings()) {
                delete data.settings
            }
            var appointmentSettings = positionMap[index];
            appointmentSettings.forEach(item => {
                item.direction = "vertical" === renderingStrategy.getDirection() && !item.allDay ? "vertical" : "horizontal"
            });
            var item = {
                itemData: data,
                settings: appointmentSettings
            };
            if (!isRenovatedAppointments) {
                item.needRepaint = true;
                item.needRemove = false
            }
            return item
        })
    }
    makeRenovatedViewModels(viewModel) {
        var strategy = this.getRenderingStrategy();
        var regularViewModels = [];
        var allDayViewModels = [];
        var compactOptions = [];
        viewModel.forEach(_ref => {
            var {
                itemData: itemData,
                settings: settings
            } = _ref;
            settings.forEach(options => {
                var item = this.prepareViewModel(options, strategy, itemData);
                if (options.isCompact) {
                    compactOptions.push({
                        compactViewModel: options.virtual,
                        appointmentViewModel: item
                    })
                } else if (options.allDay) {
                    allDayViewModels.push(item)
                } else {
                    regularViewModels.push(item)
                }
            })
        });
        var compactViewModels = this.prepareCompactViewModels(compactOptions);
        var result = _extends({
            allDay: allDayViewModels,
            regular: regularViewModels
        }, compactViewModels);
        return result
    }
    prepareViewModel(options, strategy, itemData) {
        var geometry = strategy.getAppointmentGeometry(options);
        var viewModel = {
            key: getAppointmentKey(geometry),
            appointment: itemData,
            geometry: _extends({}, geometry, {
                leftVirtualWidth: options.leftVirtualWidth,
                topVirtualHeight: options.topVirtualHeight
            }),
            info: _extends({}, options.info, {
                allDay: options.allDay,
                direction: options.direction,
                appointmentReduced: options.appointmentReduced
            })
        };
        return viewModel
    }
    getCompactViewModelFrame(compactViewModel) {
        return {
            isAllDay: !!compactViewModel.isAllDay,
            isCompact: compactViewModel.isCompact,
            geometry: {
                left: compactViewModel.left,
                top: compactViewModel.top,
                width: compactViewModel.width,
                height: compactViewModel.height
            },
            items: {
                colors: [],
                data: [],
                settings: []
            }
        }
    }
    prepareCompactViewModels(compactOptions) {
        var regularCompact = {};
        var allDayCompact = {};
        compactOptions.forEach(_ref2 => {
            var {
                compactViewModel: compactViewModel,
                appointmentViewModel: appointmentViewModel
            } = _ref2;
            var {
                index: index,
                isAllDay: isAllDay
            } = compactViewModel;
            var viewModel = isAllDay ? allDayCompact : regularCompact;
            if (!viewModel[index]) {
                viewModel[index] = this.getCompactViewModelFrame(compactViewModel)
            }
            var {
                settings: settings,
                data: data,
                colors: colors
            } = viewModel[index].items;
            settings.push(appointmentViewModel);
            data.push(appointmentViewModel.appointment);
            colors.push(appointmentViewModel.info.resourceColor)
        });
        var toArray = items => Object.keys(items).map(key => _extends({
            key: key
        }, items[key]));
        var allDayViewModels = toArray(allDayCompact);
        var regularViewModels = toArray(regularCompact);
        [...allDayViewModels, ...regularViewModels].forEach(viewModel => {
            var {
                colors: colors
            } = viewModel.items;
            viewModel.color = getOverflowIndicatorColor(colors[0], colors)
        });
        return {
            allDayCompact: allDayViewModels,
            regularCompact: regularViewModels
        }
    }
    getRenderingStrategy() {
        return this.renderingStrategy
    }
}
