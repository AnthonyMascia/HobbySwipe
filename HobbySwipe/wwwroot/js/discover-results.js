'use strict';

// Select elements from the DOM
var resultsContainer = document.querySelector('.results');
var allCards = document.querySelectorAll('.results--card');

// Function to initialize the cards
function initCards() {
    // Select the new cards that are not removed
    var newCards = document.querySelectorAll('.results--card:not(.removed)');

    // Apply a series of transformations to each card
    newCards.forEach(function (card, index) {
        card.style.zIndex = allCards.length - index;
        card.style.transform = 'scale(' + (20 - index) / 20 + ') translateY(-' + 30 * index + 'px)';
        card.style.opacity = (10 - index) / 10;
    });

    // Add the 'loaded' class to the results container
    resultsContainer.classList.add('loaded');
}

// Function to create a button listener
function createButtonListener(action) {
    return function (event) {
        var cards = document.querySelectorAll('.results--card:not(.removed)');
        var moveOutWidth = document.body.clientWidth * 1.5;

        if (!cards.length) return false;

        var card = cards[0];

        card.classList.add('removed');

        // Apply the appropriate transformations based on the action
        if (action === 'love') {
            card.style.transform = 'translate(' + moveOutWidth + 'px, -100px) rotate(-30deg)';
        } else if (action === 'nope') {
            card.style.transform = 'translate(-' + moveOutWidth + 'px, -100px) rotate(30deg)';
        } else if (action === 'fav') {
            card.style.transform = 'translate(0px, -' + moveOutWidth + 'px) rotate(30deg)';
        }

        // Initialize the next set of cards
        initCards();

        event.preventDefault();
    };
}

// Initialize the cards
initCards();

// Attach Hammer.js listeners to each card
allCards.forEach(function (el) {
    // Prevent scrolling when interacting with the card
    el.addEventListener('touchmove', function (e) {
        e.preventDefault();
    }, { passive: false });

    // Get the buttons for this card
    var nope = el.querySelector('#nope');
    var love = el.querySelector('#love');
    var fav = el.querySelector('#fav');
    var btnCtnr = el.querySelector('.results--buttons');
    var interactiveElements = [nope, love, fav, btnCtnr];

    // Create and attach the button listeners
    var nopeListener = createButtonListener('nope');
    var loveListener = createButtonListener('love');
    var favListener = createButtonListener('fav');

    nope.addEventListener('click', nopeListener);
    love.addEventListener('click', loveListener);
    fav.addEventListener('click', favListener);

    var hammertime = new Hammer(el);
    hammertime.get('pan').set({ direction: Hammer.DIRECTION_ALL, threshold: 0, touchAction: 'none' });

    // Disable Hammer.js when touch or mouse starts on the interactive elements
    interactiveElements.forEach(function (elem) {
        elem.addEventListener('touchstart', function () {
            hammertime.get('pan').set({ enable: false });
        });
        elem.addEventListener('mousedown', function () {
            hammertime.get('pan').set({ enable: false });
        });
    });

    // Enable Hammer.js when touch or mouse ends on the interactive elements
    interactiveElements.forEach(function (elem) {
        elem.addEventListener('touchend', function () {
            hammertime.get('pan').set({ enable: true });
        });
    });

    document.addEventListener('mouseup', function () {
        hammertime.get('pan').set({ enable: true });
    });

    // On 'pan', determine swipe direction and apply transformations
    hammertime.on('pan', function (event) {
        el.classList.add('moving');

        // If no horizontal or vertical movement, return
        if (event.deltaX === 0 && event.deltaY >= 0) return;
        if (event.center.x === 0 && event.center.y === 0) return;

        // If swipe up
        if (event.deltaY < 0 && Math.abs(event.deltaY) > Math.abs(event.deltaX)) {
            // Remove 'love' and 'nope' classes, add 'fav' class
            resultsContainer.classList.remove('results_love', 'results_nope');
            resultsContainer.classList.add('results_fav');
        }
        else {
            // Remove 'fav' class, add 'love' or 'nope' class based on direction
            resultsContainer.classList.remove('results_fav');
            resultsContainer.classList.toggle('results_love', event.deltaX > 0);
            resultsContainer.classList.toggle('results_nope', event.deltaX < 0);
        }

        // Apply transformations to the card
        var xMulti = event.deltaX * 0.03;
        var yMulti = event.deltaY / 80;
        var rotate = xMulti * yMulti;

        event.target.style.transform = 'translate(' + event.deltaX + 'px, ' + event.deltaY + 'px) rotate(' + rotate + 'deg)';
    });

    // On 'panend', determine whether to keep the card or discard it
    hammertime.on('panend', function (event) {
        el.classList.remove('moving');
        resultsContainer.classList.remove('results_love', 'results_nope', 'results_fav');

        var moveOutWidth = document.body.clientWidth;
        var keep = (Math.abs(event.deltaX) < 80 || Math.abs(event.velocityX) < 0.5) && (event.deltaY >= -80 || Math.abs(event.velocityY) < 0.5);

        event.target.classList.toggle('removed', !keep);

        if (keep) {
            event.target.style.transform = '';
        } else {
            // Remove the event listeners
            nope.removeEventListener('click', nopeListener);
            love.removeEventListener('click', loveListener);
            fav.removeEventListener('click', favListener);

            // Determine the final location of the card
            var endX = Math.max(Math.abs(event.velocityX) * moveOutWidth, moveOutWidth);
            var toX = event.deltaX > 0 ? endX : -endX;
            var endY = Math.abs(event.velocityY) * moveOutWidth;
            var toY = event.deltaY > 0 ? endY : -endY;
            var xMulti = event.deltaX * 0.03;
            var yMulti = event.deltaY / 80;
            var rotate = xMulti * yMulti;

            // If swipe up, change the endY and toY
            if (event.deltaY < 0 && Math.abs(event.deltaY) > Math.abs(event.deltaX)) {
                endY = -endY;
                toY = endY;
            }

            // Apply the final transformations
            event.target.style.transform = 'translate(' + toX + 'px, ' + toY + 'px) rotate(' + rotate + 'deg)';
            initCards();
        }
    });
});

// Map keys to actions
var keyActions = {
    'ArrowLeft': 'nope',
    'ArrowRight': 'love',
    'ArrowUp': 'fav'
};

window.addEventListener('keydown', function (event) {
    var action = keyActions[event.key];
    if (action) {
        var cards = document.querySelectorAll('.results--card:not(.removed)');
        if (!cards.length) return false;
        var card = cards[0];
        var moveOutWidth = document.body.clientWidth * 1.5;

        card.classList.add('removed');

        if (action === 'love') {
            card.style.transform = 'translate(' + moveOutWidth + 'px, -100px) rotate(-30deg)';
        } else if (action === 'nope') {
            card.style.transform = 'translate(-' + moveOutWidth + 'px, -100px) rotate(30deg)';
        } else if (action === 'fav') {
            card.style.transform = 'translate(0px, -' + moveOutWidth + 'px) rotate(30deg)';
        }

        initCards();
    }
});