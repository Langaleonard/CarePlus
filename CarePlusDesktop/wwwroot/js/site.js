document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.alert.success').forEach(el => setTimeout(() => {
    el.style.transition = 'opacity .3s ease';
    el.style.opacity = '0';
    setTimeout(() => el.remove(), 350);
  }, 3500));
});
