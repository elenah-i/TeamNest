// =========================================================
// Register page: switch between "Create a new one" and "Join with a code"
// =========================================================
(() => {
  'use strict';

  const choiceCreate = document.getElementById('choiceCreate');
  const choiceJoin = document.getElementById('choiceJoin');
  const orgGroup = document.getElementById('orgNameGroup');
  const inviteGroup = document.getElementById('inviteGroup');
  const inviteInput = document.getElementById('InviteCode');

  if (!choiceCreate || !choiceJoin) return;

  function update() {
    const creating = choiceCreate.checked;
    orgGroup.classList.toggle('d-none', !creating);
    inviteGroup.classList.toggle('d-none', creating);
    // jQuery Validate skips hidden fields, so the hidden one is never checked
  }

  choiceCreate.addEventListener('change', update);
  choiceJoin.addEventListener('change', update);

  // Show codes in capitals as the user types
  inviteInput?.addEventListener('input', () => {
    const position = inviteInput.selectionStart;
    inviteInput.value = inviteInput.value.toUpperCase();
    inviteInput.setSelectionRange(position, position);
  });

  update();
})();
