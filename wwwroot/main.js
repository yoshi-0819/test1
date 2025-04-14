async function searchCountry() {
    const name = document.getElementById("countryInput").value.trim();
    const result = document.getElementById("result");
    result.innerHTML = "";

    if (!name) {
        result.textContent = "国名を入力してください";
        return;
    }

    try {
        const res = await fetch(`/api/country/info?name=${encodeURIComponent(name)}`);
        if (!res.ok) throw new Error("国が見つかりませんでした");

        const data = await res.json();

        result.innerHTML = `
      <h2>${data.name}</h2>
      <img src="${data.flag}" alt="国旗" width="150"><br>
      <strong>首都:</strong> ${data.capital}<br>
      <strong>人口:</strong> ${data.population.toLocaleString()}人<br>
      <strong>通貨:</strong> ${data.currency}<br>
      <strong>言語:</strong> ${data.language}<br>
    `;
    } catch (err) {
        result.textContent = "エラー: " + err.message;
    }
}
