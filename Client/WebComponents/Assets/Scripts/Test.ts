import * as SelectLib from './selectBox';

(window as any)._selectLib = SelectLib;

(window as any).focusElement = function (elementId: string): void {
    document.getElementById(elementId)?.focus();
}