$(document).ready(function () {
    AOS.init();
    handleTypedElement();
    handleTimelineAnimations();
    handleCarouselBehaviour();
    handleGlider();
});

function handleTypedElement() {
    const typedElement = document.querySelector("#typed");
    if (typedElement) {
        new Typed("#typed", {
            stringsElement: "#typed-strings",
            typeSpeed: 70,
            backSpeed: 50,
            backDelay: 600,
            startDelay: 100,
            shuffle: true,
            loop: true
        });
    }
}

function handleTimelineAnimations() {
    const tl = gsap.timeline({ defaults: { opacity: 0, ease: "power1.out" } });

    if (!localStorage.getItem('visited')) {
        animateForFirstTimeVisitors(tl);
        localStorage.setItem('visited', 'true');
    } else {
        document.querySelector('.preloader').style.display = "none";
    }

    animateHeroElements(tl);
}

function animateForFirstTimeVisitors(tl) {
    tl.to(".lightCyan-slider", { x: "-10%", duration: 1 })
        .to(".persianGreen-slider", { x: "-20%", duration: 1.5 }, "-=1")
        .to(".white-slider", { x: "-30%", duration: 1.5 }, "-=1")
        .to(".hide", { x: "0%", duration: 2, opacity: 1, })
        .to(".preloader", { x: "200%", duration: 3, });
}

function animateHeroElements(tl) {
    tl.from(".hero-header", { y: "-50", duration: 1 }, "<")
        .from(".hero-text", { y: "-50", duration: 1, delay: .7, autoAlpha: 0 }, "<")
        .from(".hero-button", { y: "-50", duration: 1, delay: .7, autoAlpha: 0 }, "<");
}

function handleCarouselBehaviour() {
    var $carousel = $('#hero_carousel');
    var $carouselItems = $carousel.find('.carousel-item');
    var randomIndex = Math.floor(Math.random() * $carouselItems.length);

    // Remove 'active' class from the first item (if it has one)
    $carouselItems.removeClass('active');

    // Set 'active' class to the randomly chosen item
    $carouselItems.eq(randomIndex).addClass('active');

    // Start the carousel
    $carousel.carousel();
}

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

        glider.mount();
    };
}
