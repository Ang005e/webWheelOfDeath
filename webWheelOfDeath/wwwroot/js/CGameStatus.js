import * as Util from "./lib/Utilities.js";

export const EnumGameStatus = Object.freeze({
    Stopped: 0,
    Running: 1,
    Won: 2,
    Killed: 3,
    Timed_Out: 4,
    Exceeded_Throws: 5
});

//-------------------------------------------------------------------------------
//---------------------------------CGameStatus-----------------------------------
//-------------------------------------------------------------------------------
export class CGameStatus {
    #gameStatus = EnumGameStatus.Stopped;
    #statusPanel = document.getElementById('statusPanel');

    constructor() { }

    get gameStatus() {
        return this.#gameStatus;
    }

    set gameStatus(newStatus) {
        //console.log(`Previous Status: ${this.#gameStatus} New Status: ${newStatus}`)
        if (newStatus !== this.#gameStatus) {
            // Out with the old (Must do this BEFORE resetting #gameStatus!)...
            const priorCssClass = this.cssClass(this.#gameStatus);
            this.#statusPanel.classList.remove(priorCssClass);

            // In with the new...
            const newCssClass = this.cssClass(newStatus);
            this.#statusPanel.classList.add(newCssClass);

            this.#gameStatus = newStatus;
        }
    }

    // For the supplied 'status' parameter, returns the class name corresponding to
    // selectors defined in stylesheet statusPanel.css
    cssClass(status) {
        switch (status) {
            case EnumGameStatus.Stopped:
                return 'stopped';
            case EnumGameStatus.Running:
                return 'running';
            case EnumGameStatus.Won:
                return 'won';
            case EnumGameStatus.Killed:
            case EnumGameStatus.Timed_Out:
            case EnumGameStatus.Exceeded_Throws:
                return 'lost';
            default:
                return null;
        }
    }

    display(elapsedTime) {
        //Util.hide(this.#statusPanel);

        let statusHtml = `<strong>${this.toString()}</strong>`;

        if (this.gameStatus !== EnumGameStatus.Running) {
            statusHtml += `<br/><br/>Elapsed Time: ${Util.toSeconds(elapsedTime, 1)} sec`;
        }

        this.#statusPanel.innerHTML = statusHtml;
        Util.show(this.#statusPanel);
    }

    toString() {
        return Util.enumFriendlyNameFromValue(EnumGameStatus, this.gameStatus);
    }
}
