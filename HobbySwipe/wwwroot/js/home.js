// Initializes various components after the DOM content has loaded.
document.addEventListener('DOMContentLoaded', function () {
    // Initialize Animate On Scroll Library
    AOS.init();

    // Set up the Typed effect for specific elements
    handleTypedElement();

    // Handles animations for a timeline component
    handleTimelineAnimations();

    // Manages behaviour of a carousel component
    handleCarouselBehaviour();

    // Setup the Glider.js for carousel components
    handleGlider();
});

// When the window has loaded, listen for scroll events for parallax effect
window.addEventListener('load', function () {
    window.addEventListener('scroll', parallaxScroll);
});

// Applies a parallax effect on scroll
function parallaxScroll() {
    var scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    document.querySelectorAll('.parallax').forEach(function (el) {
        var topDistance = el.offsetTop;
        if (topDistance < scrollTop) {
            var difference = scrollTop - topDistance;
            var half = (difference / 2) * 0.5;
            el.style.backgroundPosition = `center ${-(half)}px`;
        }
    });
}

// Handles the typing animation of specific elements
function handleTypedElement() {
    const typedElement = document.querySelector("#typed");
    if (typedElement) {
        new Typed("#typed", {
            stringsElement: "#typed-strings",
            typeSpeed: 70,
            backSpeed: 50,
            backDelay: 1000,
            startDelay: 100,
            shuffle: true,
            loop: true
        });
    }
}

// Manages the animations for the timeline
function handleTimelineAnimations() {
    const tl = gsap.timeline({ defaults: { opacity: 0, ease: "power1.out" } });

    // Check if it's the user's first visit
    if (!localStorage.getItem('visited')) {
        // If it is, perform animations for first-time visitors
        animateForFirstTimeVisitors(tl);

        // Mark as visited
        localStorage.setItem('visited', 'true');
    } else {
        // Hide the preloader for returning visitors
        document.querySelector('.preloader').style.display = "none";
    }

    // Perform animations for hero elements
    animateHeroElements(tl);
}

// Animation sequence for first time visitors
function animateForFirstTimeVisitors(tl) {
    tl.to(".lightCyan-slider", { x: "-10%", duration: 1 })
        .to(".persianGreen-slider", { x: "-20%", duration: 1.5 }, "-=1")
        .to(".white-slider", { x: "-30%", duration: 1.5 }, "-=1")
        .to(".hide", { x: "0%", duration: 2, opacity: 1, })
        .to(".preloader", { x: "200%", duration: 3, });
}

// Animation sequence for hero elements
function animateHeroElements(tl) {
    tl.from(".hero-header", { y: "-50", duration: 1 }, "<")
        .from(".hero-text", { y: "-50", duration: 1, delay: .7, autoAlpha: 0 }, "<")
        .from(".hero-button", { y: "-50", duration: 1, delay: .7, autoAlpha: 0 }, "<");
}

// Manages the carousel's active state and initialization
function handleCarouselBehaviour() {
    var $carousel = document.querySelector('#hero_carousel');
    var $carouselItems = Array.from($carousel.querySelectorAll('.carousel-item'));
    var randomIndex = Math.floor(Math.random() * $carouselItems.length);

    // Remove 'active' class from the first item (if it has one)
    $carouselItems.forEach(item => item.classList.remove('active'));

    // Set 'active' class to the randomly chosen item
    $carouselItems[randomIndex].classList.add('active');
}

// Sets up the Glider.js for carousel elements
function handleGlider() {
    const glideElement = document.querySelector('.glide');

    if (glideElement) {
        const glider = new Glide('.glide-featured', {
            autoplay: 10000,
            type: 'carousel',
            perView: 4,
            breakpoints: {
                1200: {
                    perView: 3
                },
                992: {
                    perView: 2
                },
                535: {
                    perView: 1
                }
            }
        });

        // Mount the glider
        glider.mount();
    }
}
