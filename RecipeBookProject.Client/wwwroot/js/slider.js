window.slider = {
    scrollRowByPage(containerId, dir) {
        const el = document.getElementById(containerId);
        if (!el) return;

        // Scroll eden doğru eleman mı?
        if (el.scrollWidth <= el.clientWidth) {
            console.warn('Kayacak içerik yok veya yanlış eleman hedeflendi.');
            return;
        }

        const step = Math.round(el.getBoundingClientRect().width);
        const target = el.scrollLeft + dir * step;

        el.scrollTo({ left: target, behavior: 'smooth' });
    }
};
