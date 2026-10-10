// Mert Admin - panel davranışları: aktif menü, breadcrumb ve arama paleti
(function () {
    'use strict';

    var path = window.location.pathname.toLowerCase().replace(/\/+$/, '');
    var segments = path.split('/').filter(Boolean);

    // ---------- Aktif menü: sadece bulunduğun bölüm işaretlensin ----------
    function sectionKey(url) {
        var parts = url.toLowerCase().split('?')[0].split('/').filter(Boolean);
        // Area'lı adreslerde (Admin/Teacher/Student) area + controller, diğerlerinde controller
        if (['admin', 'teacher', 'student'].indexOf(parts[0]) >= 0) return parts.slice(0, 2).join('/');
        return parts.slice(0, 1).join('/');
    }

    var currentKey = sectionKey(path);
    var activeLink = null;
    var links = document.querySelectorAll('#sidebarnav .panel-menu-link');

    // Önce birebir adres eşleşmesi, sonra aynı bölüm (ör. /Admin/Course/UpdateCourse/5 -> Kurslar)
    links.forEach(function (a) {
        if (a.getAttribute('href').toLowerCase().replace(/\/+$/, '') === path) activeLink = a;
    });
    if (!activeLink) {
        links.forEach(function (a) {
            if (!activeLink && sectionKey(a.getAttribute('href')) === currentKey) activeLink = a;
        });
    }

    // Temanın kendi işaretlemesini temizleyip tek bir öğeyi aktif yap
    function applyActive() {
        document.querySelectorAll('#sidebarnav .sidebar-item').forEach(function (li) { li.classList.remove('selected'); });
        document.querySelectorAll('#sidebarnav .sidebar-link').forEach(function (a) { a.classList.remove('active'); });
        if (activeLink) {
            activeLink.classList.add('active');
            activeLink.parentElement.classList.add('selected');
        }
    }
    applyActive();
    window.addEventListener('load', applyActive);

    // ---------- Breadcrumb: Panel > Bölüm > Sayfa ----------
    var breadcrumb = document.getElementById('panelBreadcrumb');
    if (breadcrumb) {
        var sectionTitle = activeLink ? activeLink.textContent.trim() : '';
        var pageTitle = (window.panelPageTitle || '').trim();
        var isSectionHome = activeLink && activeLink.getAttribute('href').toLowerCase().replace(/\/+$/, '') === path;

        function addCrumb(text, href) {
            var li = document.createElement('li');
            li.className = 'breadcrumb-item' + (href ? '' : ' active');
            if (href) {
                var a = document.createElement('a');
                a.href = href;
                a.textContent = text;
                li.appendChild(a);
            } else {
                li.textContent = text;
                li.setAttribute('aria-current', 'page');
            }
            breadcrumb.appendChild(li);
        }

        if (sectionTitle && !isSectionHome) {
            addCrumb(sectionTitle, activeLink.getAttribute('href'));
            if (pageTitle && pageTitle.toLowerCase() !== sectionTitle.toLowerCase()) addCrumb(pageTitle);
        } else if (sectionTitle) {
            addCrumb(sectionTitle);
        } else if (pageTitle) {
            addCrumb(pageTitle);
        }
    }

    // ---------- Arama paleti ----------
    var input = document.getElementById('panelSearch');
    var results = document.getElementById('panelSearchResults');
    var items = window.panelSearchItems || [];
    var selectedIndex = -1;
    var current = [];

    if (!input || !results) return;

    function normalize(text) {
        return (text || '').toLocaleLowerCase('tr-TR')
            .replace(/ı/g, 'i').replace(/ğ/g, 'g').replace(/ü/g, 'u')
            .replace(/ş/g, 's').replace(/ö/g, 'o').replace(/ç/g, 'c');
    }

    function escapeHtml(text) {
        var div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function render(list) {
        current = list;
        selectedIndex = list.length ? 0 : -1;
        if (!list.length) {
            results.innerHTML = '<div class="panel-search-empty"><i class="mdi mdi-magnify-close"></i> Sonuç bulunamadı</div>';
            results.classList.add('open');
            return;
        }
        var html = '';
        var lastGroup = null;
        list.forEach(function (item, i) {
            if (item.group !== lastGroup) {
                html += '<div class="panel-search-group">' + escapeHtml(item.group) + '</div>';
                lastGroup = item.group;
            }
            html += '<a href="' + item.url + '" class="panel-search-item' + (i === 0 ? ' selected' : '') + '" data-index="' + i + '" role="option">' +
                '<i class="mdi ' + item.icon + '"></i><span>' + escapeHtml(item.title) + '</span><kbd>↵</kbd></a>';
        });
        results.innerHTML = html;
        results.classList.add('open');
    }

    function search() {
        var q = normalize(input.value.trim());
        if (!q) {
            render(items);
            return;
        }
        var words = q.split(/\s+/);
        var matched = items.filter(function (item) {
            var hay = normalize(item.title + ' ' + item.group);
            return words.every(function (w) { return hay.indexOf(w) >= 0; });
        });
        // Başlığı aranan kelimeyle başlayanlar önce gelsin
        matched.sort(function (a, b) {
            return (normalize(a.title).indexOf(q) === 0 ? 0 : 1) - (normalize(b.title).indexOf(q) === 0 ? 0 : 1);
        });
        render(matched);
    }

    function highlight() {
        results.querySelectorAll('.panel-search-item').forEach(function (el) {
            el.classList.toggle('selected', Number(el.dataset.index) === selectedIndex);
        });
        var sel = results.querySelector('.panel-search-item.selected');
        if (sel) sel.scrollIntoView({ block: 'nearest' });
    }

    function close() {
        results.classList.remove('open');
    }

    input.addEventListener('focus', search);
    input.addEventListener('input', search);
    input.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowDown' && current.length) {
            e.preventDefault();
            selectedIndex = (selectedIndex + 1) % current.length;
            highlight();
        } else if (e.key === 'ArrowUp' && current.length) {
            e.preventDefault();
            selectedIndex = (selectedIndex - 1 + current.length) % current.length;
            highlight();
        } else if (e.key === 'Enter') {
            e.preventDefault();
            if (selectedIndex >= 0 && current[selectedIndex]) window.location.href = current[selectedIndex].url;
        } else if (e.key === 'Escape') {
            close();
            input.blur();
        }
    });

    document.addEventListener('click', function (e) {
        if (!e.target.closest('.panel-search-box')) close();
    });

    // "/" ile aramaya odaklan
    document.addEventListener('keydown', function (e) {
        var tag = (document.activeElement && document.activeElement.tagName) || '';
        if (e.key === '/' && tag !== 'INPUT' && tag !== 'TEXTAREA' && tag !== 'SELECT') {
            e.preventDefault();
            input.focus();
        }
    });
})();

// ---------- Modern sayfa düzeni: tabloları ve formları kartlara al ----------
(function () {
    'use strict';
    var content = document.querySelector('.page-wrapper > .container-fluid');
    if (!content) return;

    var ICONS = [
        [/^sil$/i, 'mdi-delete-outline'],
        [/^(güncelle|düzenle)$/i, 'mdi-pencil-outline'],
        [/^gösterme$/i, 'mdi-eye-off-outline'],
        [/^göster$/i, 'mdi-eye-outline'],
        [/video/i, 'mdi-play-circle-outline'],
        [/^rol(leri)? ata$/i, 'mdi-account-key'],
        [/^onayla$/i, 'mdi-check'],
        [/^iptal et$/i, 'mdi-close'],
        [/^(kaydet|rolleri ata|kursa kaydol)$/i, 'mdi-content-save-outline'],
        [/^(yeni|kursa yeni)/i, 'mdi-plus'],
        [/geri dön/i, 'mdi-arrow-left']
    ];

    // Butonlara metne göre ikon ekle
    content.querySelectorAll('.btn').forEach(function (btn) {
        if (btn.querySelector('i')) return;
        var text = btn.textContent.trim();
        for (var i = 0; i < ICONS.length; i++) {
            if (ICONS[i][0].test(text)) {
                var icon = document.createElement('i');
                icon.className = 'mdi ' + ICONS[i][1];
                btn.insertBefore(icon, btn.firstChild);
                break;
            }
        }
    });

    var pageTitle = (window.panelPageTitle || '').trim();

    // Doğrudan içerikteki tabloları kart içine al
    Array.prototype.slice.call(content.children).forEach(function (el) {
        if (el.tagName !== 'TABLE' || !el.classList.contains('table')) return;

        var card = document.createElement('div');
        card.className = 'panel-card';
        var header = document.createElement('div');
        header.className = 'panel-card-header';
        var rows = el.querySelectorAll('tr').length - 1;
        var h5 = document.createElement('h5');
        h5.textContent = pageTitle || 'Kayıtlar';
        var count = document.createElement('span');
        count.className = 'panel-count';
        count.textContent = Math.max(rows, 0) + ' kayıt';
        h5.appendChild(count);
        header.appendChild(h5);

        var actions = document.createElement('div');
        actions.className = 'panel-card-actions';

        if (rows > 3) {
            var filter = document.createElement('div');
            filter.className = 'panel-table-filter';
            filter.innerHTML = '<i class="mdi mdi-magnify"></i><input type="text" placeholder="Tabloda ara...">';
            actions.appendChild(filter);
            var input = filter.querySelector('input');
            input.addEventListener('input', function () {
                var q = input.value.toLocaleLowerCase('tr-TR');
                el.querySelectorAll('tr').forEach(function (tr, i) {
                    if (i === 0) return;
                    tr.style.display = tr.textContent.toLocaleLowerCase('tr-TR').indexOf(q) >= 0 ? '' : 'none';
                });
            });
        }

        // Tablonun altındaki "Yeni ... Ekle" butonunu kart başlığına taşı
        var sib = el.nextElementSibling;
        while (sib && (sib.tagName === 'BR' || (sib.tagName === 'A' && sib.classList.contains('btn')))) {
            var next = sib.nextElementSibling;
            if (sib.tagName === 'A') actions.appendChild(sib); else sib.remove();
            sib = next;
        }

        header.appendChild(actions);
        var wrap = document.createElement('div');
        wrap.className = 'table-responsive';
        el.parentNode.insertBefore(card, el);
        card.appendChild(header);
        card.appendChild(wrap);
        wrap.appendChild(el);

        if (rows <= 0) {
            var empty = document.createElement('div');
            empty.className = 'panel-empty';
            empty.innerHTML = '<i class="mdi mdi-inbox-outline"></i>Henüz kayıt yok.';
            card.appendChild(empty);
        }
    });

    // Formları kart içine al
    Array.prototype.slice.call(content.children).forEach(function (el) {
        if (el.tagName !== 'FORM') return;
        var card = document.createElement('div');
        card.className = 'panel-card panel-form-card';
        el.parentNode.insertBefore(card, el);
        card.appendChild(el);
        // Alanlar arası boşluğu <br> yerine CSS ile ver
        el.querySelectorAll('br').forEach(function (br) {
            var spacer = document.createElement('div');
            spacer.style.height = '16px';
            br.replaceWith(spacer);
        });
    });
})();
