(function () {
    function updateCartCount(count) {
        var badge = document.getElementById("cart-count-badge");
        if (badge) {
            badge.textContent = count;
        }
    }

    function showToast(message, type) {
        var container = document.getElementById("toast-container");
        if (!container) return;

        var wrapper = document.createElement("div");
        wrapper.className = "toast align-items-center text-bg-" + (type || "success") + " border-0 mb-2";
        wrapper.setAttribute("role", "alert");
        wrapper.setAttribute("aria-live", "assertive");
        wrapper.setAttribute("aria-atomic", "true");

        wrapper.innerHTML =
            '<div class="d-flex">' +
            '  <div class="toast-body">' + message + '</div>' +
            '  <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>' +
            '</div>';

        container.appendChild(wrapper);
        var toast = new bootstrap.Toast(wrapper, { delay: 3000 });
        toast.show();
    }

    window.BookShopCart = {
        addToCart: function (bookId, quantity) {
            $.post("/Cart/Add", { bookId: bookId, quantity: quantity || 1 })
                .done(function (res) {
                    if (res && res.success) {
                        updateCartCount(res.count);
                        showToast("Book added to cart.");
                    }
                })
                .fail(function () {
                    showToast("Unable to add to cart. Please login.", "danger");
                });
        },
        updateQuantity: function (cartId, quantity) {
            $.post("/Cart/UpdateQuantity", { cartId: cartId, quantity: quantity })
                .done(function (res) {
                    if (res && res.success) {
                        $("#cart-total").text(res.total.toFixed(2));
                        updateCartCount(res.count);
                    }
                });
        },
        removeItem: function (cartId) {
            $.post("/Cart/Remove", { cartId: cartId })
                .done(function (res) {
                    if (res && res.success) {
                        $("#cart-row-" + cartId).remove();
                        updateCartCount(res.count);
                    }
                });
        }
    };
})();

