// =========================================================
// TeamNest - shared helpers used on every page
// =========================================================

window.TeamNest = (() => {
  'use strict';

  // The anti-forgery token rendered by @Html.AntiForgeryToken() in _Layout.cshtml
  const antiForgeryToken = () =>
    document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';

  /**
   * AJAX POST that sends the anti-forgery token in a header and returns parsed JSON.
   * Throws an Error with a readable message when the server says no.
   */
  async function post(url, body = null) {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'RequestVerificationToken': antiForgeryToken(),
        'X-Requested-With': 'XMLHttpRequest'
      },
      body
    });

    const data = await response.json().catch(() => ({}));
    if (!response.ok) {
      throw new Error(data.message || 'The request failed. Refresh the page and try again.');
    }
    return data;
  }

  /** Shows a small message in the bottom-right corner. type: 'success' | 'error' */
  function toast(message, type = 'success') {
    const area = document.getElementById('toastArea');
    if (!area) return;

    const element = document.createElement('div');
    element.className = `toast tn-toast ${type === 'error' ? 'tn-toast-error' : ''}`;
    element.setAttribute('role', type === 'error' ? 'alert' : 'status');
    element.setAttribute('aria-live', 'polite');
    element.innerHTML = `
      <div class="d-flex">
        <div class="toast-body"><i class="bi ${type === 'error' ? 'bi-exclamation-circle' : 'bi-check-circle'} me-2"></i></div>
        <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
      </div>`;
    // textContent (not innerHTML) so server messages can't inject HTML
    element.querySelector('.toast-body').append(document.createTextNode(message));

    area.append(element);
    const instance = bootstrap.Toast.getOrCreateInstance(element, { delay: 4000 });
    element.addEventListener('hidden.bs.toast', () => element.remove());
    instance.show();
  }

  // Show toasts that came from TempData after a redirect
  document.querySelectorAll('#toastArea .toast').forEach((element) => {
    bootstrap.Toast.getOrCreateInstance(element).show();
  });

  // Copy buttons:  <button data-copy-target="#someElement">
  document.addEventListener('click', async (event) => {
    const button = event.target.closest('[data-copy-target]');
    if (!button) return;

    const source = document.querySelector(button.dataset.copyTarget);
    if (!source) return;

    try {
      await navigator.clipboard.writeText(source.textContent.trim());
      toast('Invite code copied.');
    } catch {
      toast('Copy failed. Select the code and copy it manually.', 'error');
    }
  });

  // Clear a field's error as soon as the user fixes it, instead of waiting until they leave the field.
  // Without this, an error disappearing on blur can shift the Submit button away from the cursor mid-click.
  ['input', 'change'].forEach((type) => {
    document.addEventListener(type, (event) => {
      const field = event.target;
      if (window.jQuery && $.fn.valid && field.classList?.contains('input-validation-error') && field.form) {
        $(field).valid();
      }
    });
  });

  return { post, toast, antiForgeryToken };
})();
