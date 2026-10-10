// Kurs filtreleme: sadece "Filtrele"ye basınca uygulanır ve sayfa yenilenmeden sonuçlar güncellenir
(function () {
    'use strict';
    var form = document.getElementById('courseFilter');
    var results = document.getElementById('courseResults');
    var button = document.getElementById('courseFilterBtn');
    if (!form || !results || !button) return;

    var fields = Array.prototype.slice.call(form.querySelectorAll('input[name], select[name]'));
    var applied = snapshot();

    function snapshot() {
        var state = {};
        fields.forEach(function (f) { state[f.name] = f.value; });
        return state;
    }

    // Uygulanmamış değişiklik varsa butonu vurgula
    function markPending() {
        var pending = false;
        fields.forEach(function (f) {
            var changed = f.value !== applied[f.name];
            f.classList.toggle('changed', changed);
            if (changed) pending = true;
        });
        button.classList.toggle('pending', pending);
        button.querySelector('span').textContent = pending ? 'Filtreyi Uygula' : 'Filtrele';
    }

    function setFormFromUrl(url) {
        var params = new URL(url, window.location.origin).searchParams;
        fields.forEach(function (f) { f.value = params.get(f.name) || ''; });
        applied = snapshot();
        markPending();
    }

    function load(url, push) {
        results.classList.add('loading');
        fetch(url, { headers: { 'X-Requested-With': 'fetch' } })
            .then(function (r) { return r.text(); })
            .then(function (html) {
                var doc = new DOMParser().parseFromString(html, 'text/html');
                var fresh = doc.getElementById('courseResults');
                var heading = doc.querySelector('.hero-cap h2');
                if (fresh) results.innerHTML = fresh.innerHTML;
                var currentHeading = document.querySelector('.hero-cap h2');
                if (heading && currentHeading) currentHeading.textContent = heading.textContent;
                if (push) history.pushState({ courseFilter: true }, '', url);
                applied = snapshot();
                markPending();
                form.scrollIntoView({ behavior: 'smooth', block: 'start' });
            })
            .catch(function () { window.location.href = url; })
            .finally(function () { results.classList.remove('loading'); });
    }

    function buildUrl(page) {
        var params = new URLSearchParams();
        fields.forEach(function (f) { if (f.value) params.set(f.name, f.value); });
        if (page && page > 1) params.set('page', page);
        var query = params.toString();
        return form.getAttribute('action') + (query ? '?' + query : '');
    }

    fields.forEach(function (f) {
        f.addEventListener('change', markPending);
        f.addEventListener('input', markPending);
    });

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        load(buildUrl(1), true);
    });

    // Sayfalama ve "filtreleri temizle" linkleri de yenilemeden çalışsın
    results.addEventListener('click', function (e) {
        var link = e.target.closest('a');
        if (!link) return;
        var href = link.getAttribute('href');
        if (!href || href === '#' || href.indexOf('/Course/Index') !== 0) return;
        // Eğitmen sayfasından çıkılıyorsa (eğitmen kartı değişmeli) normal sayfa geçişi yap
        if (document.querySelector('.oe-teacher-hero') && href.indexOf('teacher=') < 0) return;
        e.preventDefault();
        setFormFromUrl(href);
        load(href, true);
    });

    window.addEventListener('popstate', function () {
        setFormFromUrl(window.location.href);
        load(window.location.pathname + window.location.search, false);
    });
})();
