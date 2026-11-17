// =========================================================
// Projects page: AJAX search, create (modal) and delete
// =========================================================
(() => {
  'use strict';

  const filters = document.getElementById('projectFilters');
  const list = document.getElementById('projectList');
  const searchInput = document.getElementById('projectSearch');
  const statusSelect = document.getElementById('statusFilter');
  const modalElement = document.getElementById('projectModal');
  const formContainer = document.getElementById('projectFormContainer');

  // ---------- 1. Live search ----------
  // Calls ProjectsController.Search, which returns the _ProjectList partial as HTML
  async function refreshList() {
    const params = new URLSearchParams();
    if (searchInput.value.trim()) params.set('term', searchInput.value.trim());
    if (statusSelect.value) params.set('status', statusSelect.value);

    const response = await fetch(`${filters.dataset.searchUrl}?${params}`, {
      headers: { 'X-Requested-With': 'XMLHttpRequest' }
    });

    if (response.ok) {
      list.innerHTML = await response.text();
    } else {
      TeamNest.toast('Couldn\'t load projects. Refresh the page and try again.', 'error');
    }
  }

  // Debounce: wait until the user stops typing for 300 ms before searching
  let searchTimer;
  searchInput.addEventListener('input', () => {
    clearTimeout(searchTimer);
    searchTimer = setTimeout(refreshList, 300);
  });
  statusSelect.addEventListener('change', refreshList);

  // ---------- 2. Create project in the modal ----------
  if (modalElement && formContainer) {
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    // Keep a copy of the empty form so we can put it back after saving or closing
    const emptyFormHtml = formContainer.innerHTML;

    function showForm(html) {
      formContainer.innerHTML = html;
      // Newly inserted forms must be registered with jQuery Validate
      if (window.jQuery) $.validator.unobtrusive.parse(formContainer.querySelector('form'));
    }

    // Dashboard links to /Projects#new to open the modal straight away
    if (window.location.hash === '#new') {
      modal.show();
      history.replaceState(null, '', window.location.pathname + window.location.search);
    }

    // Event delegation: the form can be replaced by the server, so we listen on its container
    formContainer.addEventListener('submit', async (event) => {
      event.preventDefault();
      const form = event.target;

      // Client-side validation first (jQuery Validate + unobtrusive)
      if (window.jQuery && !$(form).valid()) return;

      const submitButton = form.querySelector('[type="submit"]');
      submitButton.disabled = true;

      try {
        // FormData includes the hidden anti-forgery field the form tag helper added
        const response = await fetch(form.action, {
          method: 'POST',
          body: new FormData(form),
          headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        if (response.ok) {
          const data = await response.json();
          modal.hide(); // the 'hidden' handler below puts the empty form back
          TeamNest.toast(data.message);
          await refreshList();
        } else if (response.status === 400) {
          // Server-side validation failed: show the form it sent back, with errors
          showForm(await response.text());
        } else {
          TeamNest.toast('Couldn\'t create the project. Try again.', 'error');
        }
      } finally {
        submitButton.disabled = false;
      }
    });

    // Start fresh each time the modal closes
    modalElement.addEventListener('hidden.bs.modal', () => showForm(emptyFormHtml));

    // Put the cursor in the first field when the modal opens
    modalElement.addEventListener('shown.bs.modal', () => {
      formContainer.querySelector('input:not([type=hidden])')?.focus();
    });
  }


  // ---------- 3. Delete project ----------
  list.addEventListener('click', async (event) => {
    const button = event.target.closest('[data-delete-url]');
    if (!button) return;

    if (!confirm(`Delete "${button.dataset.name}"? You can't undo this from the app.`)) return;

    button.disabled = true;
    try {
      const data = await TeamNest.post(button.dataset.deleteUrl);
      const row = document.getElementById(button.dataset.rowId);
      row?.classList.add('row-removing');
      setTimeout(refreshList, 250);
      TeamNest.toast(data.message);
    } catch (error) {
      button.disabled = false;
      TeamNest.toast(error.message, 'error');
    }
  });
})();
