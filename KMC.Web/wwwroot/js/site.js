// Reveal-on-scroll for elements with class "reveal"
(function () {
    var revealEls = document.querySelectorAll('.reveal');
    if (!revealEls.length) return;

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('in-view');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.15 });

    revealEls.forEach(function (el) { observer.observe(el); });
})();

// Floating navbar becomes more opaque after scrolling past the hero
(function () {
    var nav = document.querySelector('.site-nav-float');
    if (!nav) return;

    function updateNav() {
        if (window.scrollY > 40) {
            nav.classList.add('nav-scrolled');
        } else {
            nav.classList.remove('nav-scrolled');
        }
    }
    updateNav();
    window.addEventListener('scroll', updateNav, { passive: true });
})();

// Animated number counters for the stats section
(function () {
    var counters = document.querySelectorAll('[data-counter]');
    if (!counters.length) return;

    function animateCounter(el) {
        var target = parseInt(el.getAttribute('data-counter'), 10) || 0;
        var duration = 1200;
        var start = null;

        function step(timestamp) {
            if (!start) start = timestamp;
            var progress = Math.min((timestamp - start) / duration, 1);
            var eased = 1 - Math.pow(1 - progress, 3);
            el.textContent = Math.floor(eased * target).toLocaleString() + (el.getAttribute('data-suffix') || '');
            if (progress < 1) requestAnimationFrame(step);
        }
        requestAnimationFrame(step);
    }

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.4 });

    counters.forEach(function (el) { observer.observe(el); });
})();

// Very subtle hero parallax on scroll
(function () {
    var heroBg = document.querySelector('.hero-bg-image');
    if (!heroBg) return;

    window.addEventListener('scroll', function () {
        var offset = window.scrollY * 0.15;
        heroBg.style.transform = 'scale(1.08) translateY(' + offset + 'px)';
    }, { passive: true });
})();
