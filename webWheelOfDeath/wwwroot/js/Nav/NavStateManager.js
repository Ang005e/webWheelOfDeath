

import { partialLoader } from '../Utils/partialLoader.js';
import $ from 'jquery';

// navigationManager.js
export class NavigationManager {
    constructor() {
        this.currentView = null;
        this.isLoggedIn = false;
    }

    async navigate(viewName, data = null) {
        const routes = {
            'login': { url: '/GameAjax/Login', target: '#page-content' },
            'gameSelection': { url: '/GameAjax/LoadGameSelection', target: '#page-content' },
            'game': { url: '/GameAjax/Game', target: '#page-content' },
            'hallOfFame': { url: '/GameAjax/HallOfFameHigh', target: '#page-content' }
        };

        const route = routes[viewName];
        if (!route) throw new Error(`Unknown view: ${viewName}`);

        // Hide any open modals
        $('.modal-canvas').addClass('hidden');

        // Load the view
        await partialLoader(data, route.url, route.target, viewName, true);
        this.currentView = viewName;
    }

    setLoginState(isLoggedIn) {
        this.isLoggedIn = isLoggedIn;
        // Update UI elements based on login state
        $('#btnStartStop').text(isLoggedIn ? 'Start' : 'Login');
    }
}

// Single instance
export const navManager = new NavigationManager();











