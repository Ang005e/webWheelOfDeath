'use strict';
import * as Util from './lib/Utilities.js';
import {CTimer} from './lib/CTimer.js';
import {CGameStatus, EnumGameStatus} from "./CGameStatus.js";
import {CBalloon} from './CBalloon.js';
import {CGameSelectModal, CWinnerModal} from "./CAppModals.js";

//-------------------------------------------------------------------------------
//---------------------------------CWheelGame------------------------------------
//-------------------------------------------------------------------------------
export class CWheelGame extends CTimer {
    #status = new CGameStatus();
    #gameBalloonCount = 0;
    #missCounter = 0;
    #softHitCounter = 0;
    #knifeGallery = document.getElementById('knife-gallery');
    #wheelFrame = document.querySelector('.wheel-inner-frame');
    #victim = document.getElementById('victim');
    #killer_knife = this.#victim.querySelector('.big-knife');
    #unfair = document.getElementById('below-the-belt');
    #scorePanel = document.getElementById('scorePanel');
    #countdownPanel = document.getElementById('time-panel-1');
    #countdownGauge = this.#countdownPanel.querySelector('.countdown-gauge');
    #maxCountdownSeconds = this.#countdownPanel.querySelector('.countdown-max-seconds');
    #countdownRemaining = this.#countdownPanel.querySelector('.countdown-remaining');
    #btnStartStop = document.getElementById('btnStartStop');
    #balloons = new Map();
    #animationClass;

    #gameSelectionModal;
    constructor(playerUsername, duration = 30000, minBalloons = 10, maxBalloons = 16,
        maxThrows = 0) {
        // show popups can be set to false, to prevent default popups being shown... but things will not work

        if (minBalloons < 1) {
            throw new Error('Must have at least one balloon.\r\n\r\nChange Settings and Try again.');
        } else if (minBalloons > maxBalloons) {
            throw new Error('minBalloons may not exceed maxBalloons.\r\n\r\nChange Settings and Try again.');
        } else if (maxBalloons > maxThrows) {
            throw new Error('maxBalloons may not exceed maxThrows.\r\n\r\nChange Settings and Try again.');
        }

        super(duration, 100);

        this.playerUsername = playerUsername

        this.minBalloons = minBalloons;
        this.maxBalloons = maxBalloons;
        this.maxThrows = maxThrows > 0 ? maxThrows : maxBalloons * 2;
        this.#countdownGauge.max = duration.toString();
        this.#countdownGauge.value = '0';
        this.#maxCountdownSeconds.innerText = this.#maxSeconds; // Must come AFTER this.duration is set.
        this.#countdownRemaining.innerText = this.#maxSeconds; // Must come AFTER this.duration
        this.#showPopups = showPopups;


        // Create the game difficulty selection modal
        this.#gameSelectionModal = new CGameSelectModal('#modal-game-selection-id', true, 'game-selection-modal-hide')
        this.#gameSelectionModal.callbackFunction = () => {
            this.#gameSelectionModal.hide();
            this.start()
        };

        this.#countdownGauge.max = this.duration.toString();
        this.#maxCountdownSeconds.innerText = this.#maxSeconds;

        this.#updateScore();        // Initialise score display at page load
        this.#updateCountdown();    // Initialise countdown display at page load

        // Wire up event listeners...
        document.querySelector('body').addEventListener('mousedown', event=> {
            event.stopPropagation();    // prevent event bubbling up to parent(s)

            if (this.isRunning) {
                this.#miss();
            }
        });


        this.#victim.addEventListener('mousedown', event=> {
            event.stopPropagation();    // prevent event bubbling up to parent(s)

            if (this.isRunning) {
                this.#kill();
            }
        });

        this.#unfair.addEventListener('mousedown', event=> {
            event.stopPropagation();    // prevent event bubbling up to parent(s)

            if (this.isRunning) {
                this.#maim();
            }
        });

        this.#btnStartStop.addEventListener('mousedown', event=> {
            event.stopPropagation();    // prevent event bubbling up to parent(s)

            if (this.isRunning) {
                this.#gameOver(EnumGameStatus.Stopped);
            } else {
                this.start();
            }
        });

        // ########### NAVIGATION LISTENERS ############

        // When login is completed, open the game selection modal
        // [redundant]
        //document.addEventListener('login-complete', event => {
        //    event.stopPropagation();
        //    this.#gameSelectionModal.display();
        //})
        
    }

    get #maxSeconds() {
        return Util.toSeconds(this.duration);
    }

    start() {
        this.#countdownRemaining.innerText = this.#maxSeconds; // Must come AFTER this.duration is set
        Util.hide(this.#killer_knife);
        this.#randomBalloons();
        return super.start();
    }

    started() {
        this.#status.gameStatus = EnumGameStatus.Running;
        this.#missCounter = 0;
        this.#softHitCounter = 0;
        this.#btnStartStop.innerText = 'Stop';

        this.#resetGallery();
        this.#updateScore();
        this.#status.display(this.elapsedTime);
        this.#updateCountdown();
        this.#startAnimation();
    }

    timerTicked() {
        this.#updateCountdown();
    }

    cancelled() {
        this.#cleanupGame();
    }

    timerCompleted() {
        this.#gameOver(EnumGameStatus.Timed_Out);
        this.#cleanupGame();

        // https://pixabay.com/sound-effects/search/timeout/
        Util.playSoundFile('/audio/timeout.mp3');
    }

    #cleanupGame() {
        this.#updateCountdown();
        this.#btnStartStop.innerText = 'Start';
        this.#stopAnimation();
        this.#updateScore();
        this.#status.display(this.elapsedTime);

        document.dispatchEvent(new CustomEvent("game-over", {
            bubbles: true,
            detail:
                {
                    gameStatus: this.#status.gameStatus,
                    elapsed: this.elapsedTime
                }
        }));
    }

    #animate(className) {
        this.#animationClass = className;
        this.#wheelFrame.classList.add(className);
    }

    #startAnimation() {
        // See animations.css
        this.#animate(`wheelSpin${Util.getRandomIntBetween(1, 6)}`);
    }

    #stopAnimation() {
        this.#wheelFrame.classList.remove(this.#animationClass);
        for (const balloon of this.#balloons.values()) {
            balloon.stopAnimation();
        }
    }

    #throwsUsedUp() {
        if (this.throwsMade === this.maxThrows) {
            this.#gameOver(EnumGameStatus.Exceeded_Throws);
            return true;
        }
        return false;
    }

    get throwsMade() {
        return this.#missCounter + this.poppedCount;
    }

    #miss() {
        Util.playSoundFile('audio/Arrow_on_wood.mp3');
        this.#useKnife();
        this.#missCounter++;
        this.#updateScore();
        this.#throwsUsedUp();
    }

    #kill() {
        Util.playSoundFile('/audio/Sad_Death_Cropped.mp3');
        Util.show(this.#killer_knife);
        this.#useKnife();
        this.#missCounter++;
        this.#gameOver(EnumGameStatus.Killed);
    }

    #maim() {
        let soundFile;

        this.#softHitCounter++;

        switch (this.#softHitCounter) {
            case 1:
                soundFile = '/audio/tender_hit_1.mp3';
                break;
            case 2:
                soundFile = '/audio/tender_hit_2.mp3';
                break;
            case 3:
                soundFile = '/audio/tender_hit_3.mp3';
                break;
            default:
                this.#kill();
                return;
        }

        // Replace with shriek sound...
        Util.playSoundFile(soundFile);

        this.#useKnife();
        this.#missCounter++;
        this.#updateScore();
        this.#throwsUsedUp();
    }

    #hit(balloon) {
        if (balloon.popped) {
            // If balloon (now knife) has previously been popped...
            this.#miss();
        } else {
            balloon.pop();
            Util.playSoundFile('audio/balloon-burst.mp3');

            this.#useKnife();
            this.#updateScore();

            if (this.#allBalloonsPopped()) {
                // Check this first in case the last balloon was popped
                // on the final throw.
            } else if (this.#throwsUsedUp()) {
                // If no win, then check for all throws being used up.
            } else {
                //
            }
        }
    }

    get poppedCount() {
        let count = 0;
        for(const balloon of this.#balloons.values()) {
            if (balloon.popped) {
                count++;
            }
        }
        return count;
    }

    #allBalloonsPopped() {
        for(const balloon of this.#balloons.values()) {
            if (!balloon.popped) {
                return false;
            }
        }

        this.#gameOver(EnumGameStatus.Won);
        return true;
    }


    #updateScore() {
        this.#scorePanel.innerText =
            `Max Throws: ${this.maxThrows} | `+
            `Hits: ${this.poppedCount} | ` +
            `Misses: ${this.#missCounter}`;
    }

    #updateCountdown() {
        this.#countdownGauge.value = this.elapsedTime;
        this.#countdownRemaining.innerText = Util.toSecondCeiling(this.remainingTime);
    }


    #randomBalloons() {
        // Remove the DOM elements for each balloon img...
        for (const balloon of this.#balloons.values()) {
            balloon.removeNode();
        }

        // Then remove the CButton object instances from the Map...
        this.#balloons.clear();

        this.#gameBalloonCount = Util.getRandomIntBetween(this.minBalloons, this.maxBalloons);

        for (let i = 1; i <= this.#gameBalloonCount; i++) {
            const node = document.createElement("img");
            this.#wheelFrame.appendChild(node);

            const balloon = new CBalloon(node);
            node.addEventListener('mousedown', event=> {
                event.stopPropagation();    // prevent event bubbling up to parent(s)
                if (this.isRunning) {
                    this.#hit(balloon);
                }
            });

            // Note: the balloon DOM node element acts as the
            // key to this dictionary map. That way, events
            // on the element can allow retrieval of the associated
            // CBalloon object in this Map item...
            this.#balloons.set(node, balloon);
        }

    }

    #gameOver(status) {
        this.#status.gameStatus = status;
        this.cancel();

        // Check for a win...
        if (status === EnumGameStatus.Won) {
            // Check for a new record...
            const FASTEST_PLAYER_KEY = 'fastest_player';
            const FASTEST_TIME_KEY = 'fastest_time';
            const fastestPlayerOnRecord = localStorage.getItem(FASTEST_PLAYER_KEY);
            const fastestTimeOnRecord = localStorage.getItem(FASTEST_TIME_KEY);

            let message;

            if (fastestTimeOnRecord === null || this.elapsedTime < parseInt(fastestTimeOnRecord)) {

                localStorage.setItem(FASTEST_PLAYER_KEY, this.playerUsername);
                localStorage.setItem(FASTEST_TIME_KEY, this.elapsedTime.toString());

                if (fastestTimeOnRecord === null) {
                    message = `Your time of ${Util.toSeconds(this.elapsedTime, 1)} seconds is the new record!`;
                } else {
                    message = `Your time of ${Util.toSeconds(this.elapsedTime, 1)} seconds replaces the previous fastest time of ${Util.toSeconds(fastestTimeOnRecord, 1)} seconds as the new record!`;
                }
            } else {
                message = `Try harder next time to beat the all-time record of ${Util.toSeconds(fastestTimeOnRecord, 1)} seconds, held by ${fastestPlayerOnRecord}!`;
            }

           new CWinnerModal('#modal-winner-id', true)
            .display(Util.toSeconds(this.elapsedTime, 1), this.#gameBalloonCount, this.#missCounter, message);
        }
    }


    #resetGallery() {
        this.#knifeGallery.innerHTML = '';

        for (let i = 1; i <= (this.maxThrows - this.throwsMade) ; i++) {
            const node = document.createElement('img');

            const srcAttrib = document.createAttribute(`src`);
            srcAttrib.value = `/pics/Knife-Bottom.png`;
            node.setAttributeNode(srcAttrib);

            const altAttrib = document.createAttribute(`alt`);
            altAttrib.value = `Knife`;
            node.setAttributeNode(altAttrib);

            this.#knifeGallery.appendChild(node);

        }

        // Alternatively...
        // let html = '';
        // for (let i = 1; i <= (this.maxThrows - this.throwsMade) ; i++) {
        //     html += '<img src="/pics/Knife-Bottom.png" alt="Knife" />'
        // }
        // this.#knifeGallery.innerHTML = html;
    }

    #useKnife() {
        try {
            // Remove the last knife image (if any) from the gallery...
            const node = this.#knifeGallery.lastChild;
            if (node !== null) {
                node.remove();
            }
            // Alternatively...
            //if (this.#knifeGallery.hasChildNodes()) {
            //    this.#knifeGallery.lastChild.remove();
            //}
        } finally { }

    }
}
