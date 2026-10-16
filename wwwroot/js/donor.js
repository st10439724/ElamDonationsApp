// Switches the Donate pop-up between "goods" and "money" depending on the chosen type
(function () {
    var typeSelect = document.getElementById('ItemType');
    if (!typeSelect) return;

    var moneyValue = typeSelect.dataset.moneyValue;
    var amount = document.getElementById('Amount');
    var proof = document.getElementById('Proof');
    var city = document.getElementById('City');
    var moneyOnly = document.querySelectorAll('[data-money-only]');
    var goodsOnly = document.querySelectorAll('[data-goods-only]');

    function refresh() {
        var isMoney = typeSelect.value === moneyValue;
        moneyOnly.forEach(function (el) { el.classList.toggle('d-none', !isMoney); });
        goodsOnly.forEach(function (el) { el.classList.toggle('d-none', isMoney); });
        proof.required = isMoney;
        amount.required = isMoney;
        city.required = !isMoney;
    }

    typeSelect.addEventListener('change', refresh);
    refresh();
})();
