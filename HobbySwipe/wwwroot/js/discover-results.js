'use strict';

var resultsContainer = document.querySelector('.results');
var allCards = document.querySelectorAll('.results--card');
var nope = document.getElementById('nope');
var love = document.getElementById('love');
var fav = document.getElementById('fav'); // Assuming you have a button for fav

function initCards(card, index) {
    var newCards = document.querySelectorAll('.results--card:not(.removed)');

    newCards.forEach(function (card, index) {
        card.style.zIndex = allCards.length - index;
        card.style.transform = 'scale(' + (20 - index) / 20 + ') translateY(-' + 30 * index + 'px)';
        card.style.opacity = (10 - index) / 10;
    });

    resultsContainer.classList.add('loaded');
}

initCards();

allCards.forEach(function (el) {
    var hammertime = new Hammer(el);

    hammertime.on('pan', function (event) {
        el.classList.add('moving');
    });

    hammertime.on('pan', function (event) {
        el.classList.add('moving');

        if (event.deltaX === 0 && event.deltaY >= 0) return;
        if (event.center.x === 0 && event.center.y === 0) return;

        // Swipe up
        if (event.deltaY < 0 && Math.abs(event.deltaY) > Math.abs(event.deltaX)) {
            resultsContainer.classList.remove('results_love', 'results_nope');
            resultsContainer.classList.add('results_fav');
        }
        else {
            resultsContainer.classList.remove('results_fav');
            resultsContainer.classList.toggle('results_love', event.deltaX > 0);
            resultsContainer.classList.toggle('results_nope', event.deltaX < 0);
        }

        var xMulti = event.deltaX * 0.03;
        var yMulti = event.deltaY / 80;
        var rotate = xMulti * yMulti;

        event.target.style.transform = 'translate(' + event.deltaX + 'px, ' + event.deltaY + 'px) rotate(' + rotate + 'deg)';
    });

    hammertime.on('panend', function (event) {
        el.classList.remove('moving');
        resultsContainer.classList.remove('results_love', 'results_nope', 'results_fav');

        var moveOutWidth = document.body.clientWidth;
        var keep = (Math.abs(event.deltaX) < 80 || Math.abs(event.velocityX) < 0.5) && (event.deltaY >= -80 || Math.abs(event.velocityY) < 0.5);

        event.target.classList.toggle('removed', !keep);

        if (keep) {
            event.target.style.transform = '';
        } else {
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

            event.target.style.transform = 'translate(' + toX + 'px, ' + toY + 'px) rotate(' + rotate + 'deg)';
            initCards();
        }
    });
});

function createButtonListener(action) {
    return function (event) {
        var cards = document.querySelectorAll('.results--card:not(.removed)');
        var moveOutWidth = document.body.clientWidth * 1.5;

        if (!cards.length) return false;

        var card = cards[0];

        card.classList.add('removed');

        if (action === 'love') {
            card.style.transform = 'translate(' + moveOutWidth + 'px, -100px) rotate(-30deg)';
        } else if (action === 'nope') {
            card.style.transform = 'translate(-' + moveOutWidth + 'px, -100px) rotate(30deg)';
        } else if (action === 'fav') {
            card.style.transform = 'translate(0px, -' + moveOutWidth + 'px) rotate(30deg)';
        }

        initCards();

        event.preventDefault();
    };
}

var nopeListener = createButtonListener('nope');
var loveListener = createButtonListener('love');
var favListener = createButtonListener('fav');

nope.addEventListener('click', nopeListener);
love.addEventListener('click', loveListener);
fav.addEventListener('click', favListener);
