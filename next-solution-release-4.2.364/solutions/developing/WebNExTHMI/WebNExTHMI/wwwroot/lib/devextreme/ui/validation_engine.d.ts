/**
* DevExtreme (ui/validation_engine.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    ValidationResult,
} from './validation_group';

/**
                                                                    * An object that serves as a namespace for the methods required to perform validation.
                                                                    */
                                                                   export default class validationEngine {
    /**
     * Gets the default validation group.
     */
    static getGroupConfig(): any;
    /**
     * Gets a validation group with a specific key.
     */
    static getGroupConfig(group: string | any): any;
    /**
     * Registers all the Validator objects extending fields of the specified ViewModel.
     */
    static registerModelForValidation(model: any): void;
    /**
     * Resets the values and validation result of the editors that belong to the default validation group.
     */
    static resetGroup(): void;
    /**
     * Resets the values and validation result of the editors that belong to the specified validation group.
     */
    static resetGroup(group: string | any): void;
    /**
     * Unregisters all the Validator objects extending fields of the specified ViewModel.
     */
    static unregisterModelForValidation(model: any): void;
    /**
     * Validates editors from the default validation group.
     */
    static validateGroup(): ValidationResult;
    /**
     * Validates editors from a specific validation group.
     */
    static validateGroup(group: string | any): ValidationResult;
    /**
     * Validates a view model.
     */
    static validateModel(model: any): any;
}
