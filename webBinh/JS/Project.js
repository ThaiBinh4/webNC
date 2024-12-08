document.querySelectorAll('.add-card').forEach(button => {
    button.addEventListener('click', () => {
      const card = document.createElement('div');
      card.className = 'card';
      card.textContent = 'Thẻ mới';
      button.parentElement.insertBefore(card, button);
    });
  });