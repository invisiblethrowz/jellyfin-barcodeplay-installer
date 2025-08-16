(function(){
  const $ = (s)=>document.querySelector(s);
  const upc = $('#upc'), tmdb = $('#tmdb'), session = $('#session'), exact = $('#exact');
  const barcode = $('#barcode'), send = $('#send'), status = $('#status'), log = $('#log'), last = $('#last');
  const keysDetails = $('#keys');

  async function getConfig(){
    const res = await fetch('/web/Configuration/Barcode Play');
    const cfg = await res.json();
    upc.value = cfg.UPCItemDbApiKey || 'demo-upcitemdb-key';
    tmdb.value = cfg.TMDbApiKey || 'demo-tmdb-key';
    session.value = cfg.DefaultSessionId || '';
    exact.checked = !!cfg.PreferExactTitleMatch;
    if(!cfg.HasSeenConfigOnce){ keysDetails.open = true; }
  }

  async function saveConfig(){
    const cfg = {
      UPCItemDbApiKey: upc.value.trim(),
      TMDbApiKey: tmdb.value.trim(),
      DefaultSessionId: session.value.trim() || null,
      PreferExactTitleMatch: !!exact.checked,
      HasSeenConfigOnce: true
    };
    await fetch('/web/Configuration/Barcode Play', {
      method: 'POST',
      headers: {'Content-Type':'application/json'},
      body: JSON.stringify(cfg)
    });
  }

  async function testScan(code){
    status.textContent = 'Submitting…';
    const res = await fetch('/BarcodePlay/scan', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ code, sessionId: session.value || null })
    });
    const data = await res.json();
    const tr = document.createElement('tr');
    tr.innerHTML = `<td>${new Date().toLocaleString()}</td><td><code>${code}</code></td>
      <td>${res.ok ? '<span class="ok">OK</span>' : '<span class="err">ERR</span>'}</td><td>${data.message||''}</td>`;
    log.prepend(tr);
    if(res.ok){
      status.textContent = `Playing ${data.itemName||data.itemId}`;
      last.innerHTML = `<strong>${data.title||'?'}</strong> ${data.year? '('+data.year+')':''} – playing ${data.itemName||data.itemId}`;
    } else {
      status.textContent = data.message||'Error';
    }
  }

  send.addEventListener('click', async ()=>{
    if(barcode.value.trim()){ await saveConfig(); await testScan(barcode.value.trim()); barcode.value=''; barcode.focus(); }
  });
  barcode.addEventListener('keydown', async (e)=>{
    if(e.key==='Enter' && barcode.value.trim()){ await saveConfig(); await testScan(barcode.value.trim()); barcode.value=''; }
  });

  getConfig().catch(console.error);
  barcode.focus();
})();