window.slider = {
  scrollRowByPage: function (containerId, dir) {
    const el = document.getElementById(containerId);
    if (!el) return;
    const step = el.clientWidth; // Görünür sayfa kadar kay
    el.scrollBy({ left: dir * step, behavior: 'smooth' });
  }
};