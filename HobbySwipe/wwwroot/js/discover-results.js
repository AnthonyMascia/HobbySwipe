'use strict';

// Select elements from the DOM
var resultsContainer = document.querySelector('.results');
var allCards = document.querySelectorAll('.results--card');

// Function to initialize the cards
function initCards() {
    // Select the new cards that are not removed
    var newCards = document.querySelectorAll('.results--card:not(.removed)');

    if (newCards.length === 0) {
        window.location.href = '/Results/Review';
    }
    else {
        // Apply a series of transformations to each card
        newCards.forEach(function (card, index) {
            var isLandscape = window.innerWidth / window.innerHeight > 1;
            var isMobileOrTablet = window.innerWidth <= 1024;
            var translateY = (isLandscape && isMobileOrTablet) ? 15 * index : 20 * index;

            card.style.zIndex = allCards.length - index;
            card.style.transform = 'scale(' + (20 - index) / 20 + ') translateY(-' + translateY + 'px)';
            card.style.opacity = (10 - index) / 10;
        });

        // Add the 'loaded' class to the results container
        resultsContainer.classList.add('loaded');
    }
}

// Function to create a button listener
function createButtonListener(action) {
    return function (event) {
        var actionId = 0;
        var cards = document.querySelectorAll('.results--card:not(.removed)');
        var moveOutWidth = document.body.clientWidth * 1.5;

        if (!cards.length) return false;

        var card = cards[0];

        card.classList.add('removed');

        // Apply the appropriate transformations based on the action
        if (action === 'like') {
            actionId = 1;
            card.style.transform = 'translate(' + moveOutWidth + 'px, -10000px) rotate(-30deg)';
        } else if (action === 'dislike') {
            actionId = 0;
            card.style.transform = 'translate(-' + moveOutWidth + 'px, -10000px) rotate(30deg)';
        } else if (action === 'favorite') {
            actionId = 2;
            card.style.transform = 'translate(0px, -' + moveOutWidth + 'px) rotate(30deg)';
        }

        var hobbyId = event.target.closest('.results--card').getAttribute('data-hobby-id');
        saveUserPreference(actionId, hobbyId);

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
    var dislike = el.querySelector('#dislike');
    var like = el.querySelector('#like');
    var favorite = el.querySelector('#favorite');
    var btnCtnr = el.querySelector('.results--buttons');
    var interactiveElements = [dislike, like, favorite, btnCtnr];

    // Create and attach the button listeners
    var dislikeListener = createButtonListener('dislike');
    var likeListener = createButtonListener('like');
    var favoriteListener = createButtonListener('favorite');

    dislike.addEventListener('click', dislikeListener);
    like.addEventListener('click', likeListener);
    favorite.addEventListener('click', favoriteListener);

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
            // Remove 'like' and 'dislike' classes, add 'favorite' class
            resultsContainer.classList.remove('results_like', 'results_dislike');
            resultsContainer.classList.add('results_favorite');
        }
        else {
            // Remove 'favorite' class, add 'like' or 'dislike' class based on direction
            resultsContainer.classList.remove('results_favorite');
            resultsContainer.classList.toggle('results_like', event.deltaX > 0);
            resultsContainer.classList.toggle('results_dislike', event.deltaX < 0);
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
        resultsContainer.classList.remove('results_like', 'results_dislike', 'results_favorite');

        var moveOutWidth = document.body.clientWidth;
        var keep = (Math.abs(event.deltaX) < 80 || Math.abs(event.velocityX) < 0.5) && (event.deltaY >= -80 || Math.abs(event.velocityY) < 0.5);

        event.target.classList.toggle('removed', !keep);

        if (keep) {
            event.target.style.transform = '';
        } else {
            // Get the action ID depending on swipe direction
            var actionId = 0;
            var hobbyId = event.target.closest('.results--card').getAttribute('data-hobby-id');

            if (event.deltaX > 0) {
                actionId = 1; // Like
            } else if (event.deltaX < 0) {
                actionId = 0; // Dislike
            } else if (event.deltaY < 0 && Math.abs(event.deltaY) > Math.abs(event.deltaX)) {
                actionId = 2; // Favorite
            }

            // Save the preference to the database
            saveUserPreference(actionId, hobbyId);

            // Remove the event listeners
            dislike.removeEventListener('click', dislikeListener);
            like.removeEventListener('click', likeListener);
            favorite.removeEventListener('click', favoriteListener);

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
    'ArrowLeft': 'dislike',
    'ArrowRight': 'like',
    'ArrowUp': 'favorite'
};

window.addEventListener('keydown', function (event) {
    var action = keyActions[event.key];
    var actionId = 0;
    var cardElement = document.querySelector('.results--card:not(.removed)');
    console.log(cardElement);
    var hobbyId = cardElement.getAttribute('data-hobby-id');

    if (action) {
        var cards = document.querySelectorAll('.results--card:not(.removed)');
        if (!cards.length) return false;
        var card = cards[0];
        var moveOutWidth = document.body.clientWidth * 1.5;

        card.classList.add('removed');

        if (action === 'like') {
            actionId = 1;
            card.style.transform = 'translate(' + moveOutWidth + 'px, -100px) rotate(-30deg)';
        } else if (action === 'dislike') {
            actionId = 0;
            card.style.transform = 'translate(-' + moveOutWidth + 'px, -100px) rotate(30deg)';
        } else if (action === 'favorite') {
            actionId = 2;
            card.style.transform = 'translate(0px, -' + moveOutWidth + 'px) rotate(30deg)';
        }
        saveUserPreference(actionId, hobbyId);

        initCards();
    }
});

function saveUserPreference(actionId, hobbyId) {
    console.log(actionId);
  
    $.ajax({
        url: '/Discover/ProcessResult',
        type: 'POST',
        data: {
            actionId: actionId,
            hobbyId: hobbyId
        },
        success: function (response) {
            if (!response.success) {
                console.error(response.errorMessage);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(textStatus, errorThrown);
        }
    });
}