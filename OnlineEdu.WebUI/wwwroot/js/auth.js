// Giriş / kayıt sayfaları: şifre göster-gizle, canlı şifre kuralları ve eşleşme kontrolü
(function () {
    'use strict';

    // Şifreyi göster / gizle
    document.querySelectorAll('.oe-toggle-pw').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = btn.parentElement.querySelector('input');
            var show = input.type === 'password';
            input.type = show ? 'text' : 'password';
            btn.querySelector('i').className = show ? 'far fa-eye-slash' : 'far fa-eye';
            btn.setAttribute('aria-label', show ? 'Şifreyi gizle' : 'Şifreyi göster');
        });
    });

    // Şifre kuralları (API'deki Identity kurallarıyla aynı)
    var RULES = {
        length: function (v) { return v.length >= 6; },
        upper: function (v) { return /[A-ZÇĞİÖŞÜ]/.test(v); },
        lower: function (v) { return /[a-zçğıöşü]/.test(v); },
        digit: function (v) { return /\d/.test(v); },
        special: function (v) { return /[^A-Za-z0-9ÇĞİÖŞÜçğıöşü\s]/.test(v); }
    };

    document.querySelectorAll('[data-password-rules]').forEach(function (input) {
        var list = document.getElementById(input.getAttribute('data-password-rules'));
        if (!list) return;
        function update() {
            var v = input.value;
            list.classList.toggle('active', v.length > 0);
            list.querySelectorAll('li').forEach(function (li) {
                li.classList.toggle('ok', RULES[li.dataset.rule](v));
            });
        }
        input.addEventListener('input', update);
        update();
    });

    // Şifre tekrarı eşleşiyor mu?
    document.querySelectorAll('[data-match]').forEach(function (input) {
        var other = document.getElementById(input.getAttribute('data-match'));
        var hint = document.getElementById('matchHint');
        if (!other || !hint) return;
        function update() {
            if (!input.value) { hint.textContent = ''; hint.className = 'oe-match-hint'; return; }
            var ok = input.value === other.value;
            hint.textContent = ok ? '✓ Şifreler eşleşiyor' : 'Şifreler henüz eşleşmiyor';
            hint.className = 'oe-match-hint ' + (ok ? 'ok' : 'bad');
        }
        input.addEventListener('input', update);
        other.addEventListener('input', update);
    });

    // Hatalı alanı işaretle ve yazmaya başlayınca hatayı kaldır
    document.querySelectorAll('.oe-field').forEach(function (field) {
        var error = field.querySelector('.oe-field-error');
        var input = field.querySelector('input');
        if (!error || !input) return;
        if (error.textContent.trim()) field.classList.add('has-error');
        input.addEventListener('input', function () {
            field.classList.remove('has-error');
            error.textContent = '';
        });
    });
})();
