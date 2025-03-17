'use strict';

export function playSoundFile(file) {
    new Audio(file).play().then();
}

export function show(element) {
    element.classList.remove('hidden');
}

export function hide(element) {
    element.classList.add('hidden');
}

export function enumNameFromValue(enumObj, searchValue) {
    for (const [key, value] of Object.entries(enumObj)) {
        if (value === searchValue) {
            return key;
        }
    }
    return null;

    // Alternatively...
    // let enumName = null;
    //
    // for (const property in enumObj) {
    //
    //     if (enumObj.hasOwnProperty(property)) {
    //         if (enumObj[property] === searchValue) {
    //             enumName = property;
    //             break;
    //         }
    //     }
    // }
    //
    // return enumName;
}

export function enumFriendlyNameFromValue(enumObj, searchValue) {
    return enumNameFromValue(enumObj, searchValue).replace('_', ' ');
}

export function toSecondCeiling(millisecond) {
    return Math.abs( Math.ceil(millisecond/1000) );
}

export function toSecondFloor(millisecond) {
    return Math.abs( Math.floor(millisecond/1000) );
}

export function toSeconds(millisecond, decimals = 0) {
    return (millisecond/1000).toFixed(decimals);
}

// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/random
export function getRandomIntBetween(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1) + min);
}