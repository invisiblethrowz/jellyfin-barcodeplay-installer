document.getElementById('configForm').addEventListener('submit', async e => {
  e.preventDefault();
  const upcKey = document.getElementById('upcKey').value;
  const tmdbKey = document.getElementById('tmdbKey').value;
  await fetch('/Plugin/BarcodePlay/config', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ upcKey, tmdbKey })
  });
  alert("Saved!");
});
