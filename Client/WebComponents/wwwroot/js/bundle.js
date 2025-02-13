/*
 * ATTENTION: The "eval" devtool has been used (maybe by default in mode: "development").
 * This devtool is neither made for production nor for readable output files.
 * It uses "eval()" calls to create a separate source file in the browser devtools.
 * If you are trying to read the output file, select a different devtool (https://webpack.js.org/configuration/devtool/)
 * or disable the default devtool with "devtool: false".
 * If you are looking for production-ready output files, see mode: "production" (https://webpack.js.org/configuration/mode/).
 */
var ___MyScripts;
/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./WebComponents/Assets/Scss/main.scss":
/*!*********************************************!*\
  !*** ./WebComponents/Assets/Scss/main.scss ***!
  \*********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

eval("__webpack_require__.r(__webpack_exports__);\n// extracted by mini-css-extract-plugin\n\n\n//# sourceURL=webpack://___MyScripts/./WebComponents/Assets/Scss/main.scss?");

/***/ }),

/***/ "./WebComponents/Assets/Scripts/Test.ts":
/*!**********************************************!*\
  !*** ./WebComponents/Assets/Scripts/Test.ts ***!
  \**********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _selectBox__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./selectBox */ \"./WebComponents/Assets/Scripts/selectBox.ts\");\n\nwindow._selectLib = _selectBox__WEBPACK_IMPORTED_MODULE_0__;\n\n\n//# sourceURL=webpack://___MyScripts/./WebComponents/Assets/Scripts/Test.ts?");

/***/ }),

/***/ "./WebComponents/Assets/Scripts/selectBox.ts":
/*!***************************************************!*\
  !*** ./WebComponents/Assets/Scripts/selectBox.ts ***!
  \***************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   addScrollDropdownEventListeners: () => (/* binding */ addScrollDropdownEventListeners),\n/* harmony export */   positionDropdown: () => (/* binding */ positionDropdown),\n/* harmony export */   resetScrollDropdownEventListeners: () => (/* binding */ resetScrollDropdownEventListeners)\n/* harmony export */ });\n// Export functions so they can be called from C# via JS interop\nvar __awaiter = (undefined && undefined.__awaiter) || function (thisArg, _arguments, P, generator) {\n    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }\n    return new (P || (P = Promise))(function (resolve, reject) {\n        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }\n        function rejected(value) { try { step(generator[\"throw\"](value)); } catch (e) { reject(e); } }\n        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }\n        step((generator = generator.apply(thisArg, _arguments || [])).next());\n    });\n};\nvar __generator = (undefined && undefined.__generator) || function (thisArg, body) {\n    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === \"function\" ? Iterator : Object).prototype);\n    return g.next = verb(0), g[\"throw\"] = verb(1), g[\"return\"] = verb(2), typeof Symbol === \"function\" && (g[Symbol.iterator] = function() { return this; }), g;\n    function verb(n) { return function (v) { return step([n, v]); }; }\n    function step(op) {\n        if (f) throw new TypeError(\"Generator is already executing.\");\n        while (g && (g = 0, op[0] && (_ = 0)), _) try {\n            if (f = 1, y && (t = op[0] & 2 ? y[\"return\"] : op[0] ? y[\"throw\"] || ((t = y[\"return\"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;\n            if (y = 0, t) op = [op[0] & 2, t.value];\n            switch (op[0]) {\n                case 0: case 1: t = op; break;\n                case 4: _.label++; return { value: op[1], done: false };\n                case 5: _.label++; y = op[1]; op = [0]; continue;\n                case 7: op = _.ops.pop(); _.trys.pop(); continue;\n                default:\n                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }\n                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }\n                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }\n                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }\n                    if (t[2]) _.ops.pop();\n                    _.trys.pop(); continue;\n            }\n            op = body.call(thisArg, _);\n        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }\n        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };\n    }\n};\n/**\n * Positions the dropdown relative to the select box.\n * @param selectBoxId - The ID of the select box container.\n * @param dropdownId - The ID of the dropdown container.\n * @param dropdownItemsId - The ID of the container holding the dropdown items.\n */\nfunction positionDropdown(selectBoxId, dropdownId, dropdownItemsId) {\n    var selectBox = document.getElementById(selectBoxId);\n    var dropdown = document.getElementById(dropdownId);\n    var dropdownItems = document.getElementById(dropdownItemsId);\n    console.log(dropdownItems);\n    if (!selectBox || !dropdown || !dropdownItems)\n        return;\n    dropdown.style.position = 'absolute';\n    dropdown.style.zIndex = '10000';\n    var rect = selectBox.getBoundingClientRect();\n    var spaceBelow = window.innerHeight - rect.bottom;\n    var spaceAbove = rect.top;\n    // If a maxHeight is set via inline styles, try to parse it; otherwise, default to 250.\n    var dropdownMaxHeight;\n    if (dropdownItems.style.maxHeight) {\n        dropdownMaxHeight = parseInt(dropdownItems.style.maxHeight, 10);\n        if (isNaN(dropdownMaxHeight)) {\n            dropdownMaxHeight = 250;\n        }\n    }\n    else {\n        dropdownMaxHeight = 250;\n    }\n    if (spaceBelow < dropdownMaxHeight && spaceAbove > spaceBelow) {\n        dropdown.style.top = \"\".concat(rect.top - dropdownMaxHeight, \"px\");\n    }\n    else {\n        dropdown.style.top = \"\".concat(rect.bottom, \"px\");\n    }\n    dropdown.style.left = \"\".concat(rect.left, \"px\");\n    dropdown.style.width = \"\".concat(rect.width, \"px\");\n}\n/**\n * Global map for storing scroll event handlers.\n * The key is the element ID, and the value is the event listener function.\n */\nvar eventHandlersMap = new Map();\n/**\n * Adds a scroll event listener to the element with the specified ID.\n * The function uses dotNetObjectReference to call the C# method 'LoadData'.\n * @param dropdownItemsId - The ID of the element where the scroll event is listened to.\n * @param dotNetObjectReference - The object passed from C# to call JSInvokable methods.\n */\nfunction addScrollDropdownEventListeners(dropdownItemsId, dotNetObjectReference) {\n    var _this = this;\n    var dropdownItems = document.getElementById(dropdownItemsId);\n    if (!dropdownItems || eventHandlersMap.has(dropdownItemsId)) {\n        return;\n    }\n    var isLoading = false;\n    // Define the scroll event handler\n    var callback = function () { return __awaiter(_this, void 0, void 0, function () {\n        return __generator(this, function (_a) {\n            switch (_a.label) {\n                case 0:\n                    if (!(!isLoading &&\n                        dropdownItems.scrollTop + dropdownItems.clientHeight >= dropdownItems.scrollHeight - 100)) return [3 /*break*/, 2];\n                    isLoading = true;\n                    return [4 /*yield*/, dotNetObjectReference.invokeMethodAsync('LoadData', true)];\n                case 1:\n                    _a.sent();\n                    isLoading = false;\n                    _a.label = 2;\n                case 2: return [2 /*return*/];\n            }\n        });\n    }); };\n    // Store the callback in the map for later removal\n    eventHandlersMap.set(dropdownItemsId, callback);\n    dropdownItems.addEventListener('scroll', callback);\n}\n/**\n * Removes the scroll event listener from the element with the specified ID,\n * if such a handler was previously added.\n * @param dropdownItemsId - The ID of the element from which to remove the event handler.\n */\nfunction resetScrollDropdownEventListeners(dropdownItemsId) {\n    if (eventHandlersMap.has(dropdownItemsId)) {\n        var callback = eventHandlersMap.get(dropdownItemsId);\n        eventHandlersMap.delete(dropdownItemsId);\n        var dropdownItems = document.getElementById(dropdownItemsId);\n        if (dropdownItems && callback) {\n            dropdownItems.removeEventListener('scroll', callback);\n        }\n    }\n}\n\n\n//# sourceURL=webpack://___MyScripts/./WebComponents/Assets/Scripts/selectBox.ts?");

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module can't be inlined because the eval devtool is used.
/******/ 	__webpack_require__("./WebComponents/Assets/Scripts/Test.ts");
/******/ 	var __webpack_exports__ = __webpack_require__("./WebComponents/Assets/Scss/main.scss");
/******/ 	___MyScripts = __webpack_exports__;
/******/ 	
/******/ })()
;