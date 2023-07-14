// Ensures that the entire DOM is loaded and ready before the code inside is executed.
$(document).ready(function () {
    // Define string and HTML element selectors as variables
    const answerAction = 'Answer';
    const goBackAction = 'GoBack';
    const discoverController = 'Discover';
    const qstnCtnrSelector = '#qstn_ctnr';
    const questionIdSelector = '#Question_Id';
    const responseSelector = '#Answer_Response';
    const questionFormSelector = '#questionForm';

    // Creates a new Swiper instance with given configuration.
    const swiper = new Swiper('.swiper', {
        loop: true,  // Enables continuous loop mode.
        watchOverflow: false,  // Disables the observation of slides to fit into container.
        preventInteractionOnTransition: true,  // Prevents any interaction (like swipes) during transition.
        longSwipes: false,  // Disables long swipes.
        spaceBetween: 10000,  // Sets the distance between slides in pixels.
        navigation: {  // Enables navigation via next and previous buttons.
            nextEl: '.swiper-button-next',  // Sets the selector for the "next" button.
            prevEl: '.swiper-button-prev',  // Sets the selector for the "previous" button.
        },
        on: {
            // Event handlers for swiper events.
            realIndexChange: disableSwiperInteraction,  // Disables swiper interaction when the real index changes.
            touchEnd: enableSwiperInteraction,  // Enables swiper interaction when a touch swipe ends.
            slideNextTransitionEnd: (swiper) => {
                // Handles the event when a slide-next transition ends.
                processSwiperEvent(swiper, answerAction, discoverController);
                swiper.slideTo(0, 0, false);
            },
            slidePrevTransitionEnd: (swiper) => {
                // Handles the event when a slide-prev transition ends.
                processSwiperEvent(swiper, goBackAction, discoverController);
                swiper.slidePrev(0, false);
            },
        },
    });

    function disableSwiperInteraction(swiper) {
        // Disables touch interactions and unset grab cursor on the swiper instance.
        swiper.allowTouchMove = false;
        swiper.unsetGrabCursor();
    }

    function enableSwiperInteraction(swiper) {
        // Enables touch interactions on the swiper instance.
        swiper.allowTouchMove = true;
    }

    function processSwiperEvent(swiper, action, controller) {
        // Adds 'd-none' class to qstnCtnrSelector to hide it.
        $(qstnCtnrSelector).addClass('d-none');

        // Calls the function getAnswer to fetch the answer object.
        var answer = getAnswer();

        // Makes an AJAX request to the server and handles the response.
        ajaxRequest(action, controller, answer, function (data) {
            // On success, updates qstnCtnrSelector with new data and makes it visible.
            $(qstnCtnrSelector).html(data);
            $(qstnCtnrSelector).removeClass('d-none');
        });
    }

    // Binds a 'submit' event handler to the form with id questionFormSelector.
    $(document).on('submit', questionFormSelector, function (e) {
        // Prevents the form from submitting normally.
        e.preventDefault();

        // Fetches the answer object and makes an AJAX request.
        var answer = getAnswer();

        ajaxRequest(answerAction, discoverController, answer, function (data) {
            if (!data.success) {
                // If there's no success, notifies the user that they have completed the questionnaire.
                alert('You have completed the questionnaire!');
            } else {
                // Otherwise, updates qstnCtnrSelector with new data.
                $(qstnCtnrSelector).html(data);
            }
        });
    });

    // Binds a 'keydown' event handler to the entire document.
    $(document).keydown(function (e) {
        switch (e.which) {
            case 37: // left arrow key
                // Processes key event for left arrow key.
                processKeyEvent(swiper, goBackAction, discoverController, 0);
                break;
            case 39: // right arrow key
            case 13: // enter key
                // Processes key event for right arrow key and enter key.
                processKeyEvent(swiper, answerAction, discoverController, 0);
                break;
            default: return; // exit this handler for other keys
        }
        e.preventDefault(); // prevent the default action (scroll / move caret)
    });

    function getAnswer() {
        // Fetches the 'QuestionId' and 'Response' from respective fields and returns them as an object.
        return {
            "QuestionId": $(questionIdSelector).val(),
            "Response": $(responseSelector).val()
        };
    }

    function ajaxRequest(action, controller, data, successCallback) {
        // Makes an AJAX POST request to the specified URL with the provided data.
        $.ajax({
            url: '/' + controller + '/' + action,
            method: 'POST',
            data: data,
            success: successCallback,
            error: function (jqXHR, textStatus, errorThrown) {
                // Logs any errors that occur during the request.
                console.log(textStatus, errorThrown);
            }
        });
    }

    function processKeyEvent(swiper, action, controller, transitionTime) {
        // Hides qstnCtnrSelector.
        $(qstnCtnrSelector).addClass('d-none');

        // Fetches the answer object and makes an AJAX request.
        var answer = getAnswer();

        ajaxRequest(action, controller, answer, function (data) {
            if (data.success === false) {
                // If there's no success, notifies the user that they have completed the questionnaire.
                alert('You have completed the questionnaire!');
            } else {
                // Otherwise, updates qstnCtnrSelector with new data and makes it visible.
                $(qstnCtnrSelector).html(data);
                $(qstnCtnrSelector).removeClass('d-none');
            }
        });

        // Slides to the specified slide.
        swiper.slideTo(0, transitionTime, false);
    }
});
