'use strict';
import * as Util from './lib/Utilities.js';

export class CBalloon {
    #node;
    #popped;
    #animationClass;


    constructor(node) {
        this.#popped = false;
        this.#node = node;

        node.classList.add('balloon');

        const srcAttrib = document.createAttribute("src");
        srcAttrib.value = `/pics/balloon${Util.getRandomIntBetween(1, 7)}.png`;
        node.setAttributeNode(srcAttrib);

        const altAttrib = document.createAttribute("alt");
        altAttrib.value = `Balloon`;
        node.setAttributeNode(altAttrib);

        // Allowable % range for top: 20-30 or 40-55
        let top;
        let isTopInRange = false;
        do {
            top = Util.getRandomIntBetween(20, 55);

            if(top < 30 || top > 40) {
                isTopInRange = true;
            }
        } while(!isTopInRange);

        node.style.top = `${top}%`;

        // Allowable % range for left: 25-45 or 55-65
        let left;
        let isLeftInRange = false;
        do {
            left = Util.getRandomIntBetween(25, 65);

            if(left < 45 || left > 55) {
                isLeftInRange = true;
            }
        } while(!isLeftInRange);

        node.style.left = `${left}%`;

        this.#startAnimation();
    }

    #animate(className) {
        this.#animationClass = className;
        this.#node.classList.add(className);
    }

    #startAnimation() {
        // See animations.css
        this.#animate(`balloonSpin${Util.getRandomIntBetween(1, 6)}`);
    }

    get node() {
        return this.#node;
    }

    get popped() {
        return this.#popped;
    }

    removeNode() {
        this.node.remove();
    }

    stopAnimation() {
        this.node.classList.remove(this.#animationClass);
    }

    pop() {
        this.stopAnimation();

        let knifeFile;

        switch (Util.getRandomIntBetween(1, 2)) {
            case 1:
                knifeFile = '/pics/Knife-Right.png';
                break;
            default:
                knifeFile = '/pics/Knife-Left.png';
        }

        this.node.setAttribute('src', knifeFile)
        this.#popped = true;
    }
}
