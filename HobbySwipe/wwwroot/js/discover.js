// Ensures that the entire DOM is loaded and ready before the code inside is executed.
$(document).ready(function () {
    // Define string and HTML element selectors as variables
    const answerAction = 'Answer';
    const goBackAction = 'GoBack';
    const processAction = 'ProcessAnswers';
    const discoverController = 'Discover';
    const qstnCtnrSelector = '#qstn_ctnr';
    const transitionCtnrSelect = '#transition_ctnr';
    const questionIdSelector = '#Question_Id';
    const responseSelector = '#Answer_Response';
    const questionFormSelector = '#questionForm';
    let isTransitioning = false;

    // Initialize the question index to 0.
    let questionIndex = 0;

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
                // Increment the question index.
                questionIndex++;

                // Update the visibility of the previous button.
                updatePreviousButtonVisibility();

                // Handles the event when a slide-next transition ends.
                processSwiperEvent(swiper, answerAction, discoverController);
                swiper.slideTo(0, 0, false);
            },
            slidePrevTransitionEnd: (swiper) => {
                // Only go to the previous question if we're not at the first question.
                if (questionIndex > 0) {
                    // Decrement the question index.
                    questionIndex--;

                    // Update the visibility of the previous button.
                    updatePreviousButtonVisibility();

                    // Handles the event when a slide-prev transition ends.
                    processSwiperEvent(swiper, goBackAction, discoverController);
                    swiper.slidePrev(0, false);
                }                
            },
        },
    });

    // Function to update the visibility of the previous button.
    const updatePreviousButtonVisibility = function () {
        let prevButton = $('.swiper-button-prev');
        prevButton.css('display', questionIndex === 0 ? 'none' : 'block');
    };

    // Update the visibility of the previous button when the page loads.
    updatePreviousButtonVisibility();

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
            if (String(data).includes('questionForm')) {
                // On success, updates qstnCtnrSelector with new data and makes it visible.
                $(qstnCtnrSelector).html(data);
                $(qstnCtnrSelector).removeClass('d-none');
            }
            else {
                initTransition(data);
            }
        });
    }

    // Binds a 'submit' event handler to the form with id questionFormSelector.
    $(document).on('submit', questionFormSelector, function (e) {
        // Prevents the form from submitting normally.
        e.preventDefault();

        if (isTransitioning) {
            return;
        }

        // Fetches the answer object and makes an AJAX request.
        var answer = getAnswer();

        ajaxRequest(answerAction, discoverController, answer, function (data) {
            if (String(data).includes('questionForm')) {
                // On success, updates qstnCtnrSelector with new data and makes it visible.
                $(qstnCtnrSelector).html(data);
                $(qstnCtnrSelector).removeClass('d-none');
            }
            else {
                initTransition(data);
            }
        });
    });

    // Binds a 'keydown' event handler to the entire document.
    $(document).keydown(function (e) {
        if (isTransitioning) {
            return;
        }

        switch (e.which) {
            case 37: // left arrow key
                // Only go to the previous question if we're not at the first question.
                if (questionIndex > 0) {
                    // Decrement the question index.
                    questionIndex--;

                    // Update the visibility of the previous button.
                    updatePreviousButtonVisibility();

                    // Processes key event for left arrow key.
                    processKeyEvent(swiper, goBackAction, discoverController, 0);
                }
                break;
            case 39: // right arrow key
            case 13: // enter key
                // Increment the question index.
                questionIndex++;

                // Update the visibility of the previous button.
                updatePreviousButtonVisibility();

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

    function initTransition(data) {
        isTransitioning = true;

        $(qstnCtnrSelector).addClass('d-none');
        $(transitionCtnrSelect).removeClass('d-none');
        $(transitionCtnrSelect).html(data);

        const tl = gsap.timeline({ defaults: { opacity: 0, ease: "power1.out" }, repeat: -1, repeatDelay: 1.5 });

        tl.to(".lightCyan-slider", { x: "-10%", duration: 1 })
            .to(".persianGreen-slider", { x: "-20%", duration: 1.5 }, "-=1")
            .to(".white-slider", { x: "-30%", duration: 1.5 }, "-=1")
            .to(".hide", { x: "0%", duration: 2, opacity: 1, });
        //.to(".preloader", { x: "200%", duration: 3, });

        ajaxRequest(processAction, discoverController, null, function (data) {
            if (data.url) {
                setTimeout(function () {
                    window.location.href = data.url;
                }, 1);  // Wait for 5 seconds (5000 milliseconds)
            }
        });
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
            if (String(data).includes('questionForm')) {
                // On success, updates qstnCtnrSelector with new data and makes it visible.
                $(qstnCtnrSelector).html(data);
                $(qstnCtnrSelector).removeClass('d-none');

                // Slides to the specified slide.
                swiper.slideTo(0, transitionTime, false);
            }
            else {
                initTransition(data);
            }
        });
    }
});
