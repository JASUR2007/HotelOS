/**
 * Downloads Google Fonts (Barlow, Barlow Condensed, Gilda Display) as woff2
 * into public/fonts for local self-hosting. Run: node scripts/download-fonts.cjs
 */

const https = require('https');
const fs = require('fs');
const path = require('path');

const FONTS_DIR = path.join(__dirname, '..', 'public', 'fonts');

const FONT_FILES = [
  { url: 'https://fonts.gstatic.com/s/barlow/v13/7cHpv4kjgoGqM7E_DMs5.woff2', file: 'barlow-400.woff2' },
  { url: 'https://fonts.gstatic.com/s/barlow/v13/7cHqv4kjgoGqM7E3_-gs51os.woff2', file: 'barlow-500.woff2' },
  { url: 'https://fonts.gstatic.com/s/barlow/v13/7cHqv4kjgoGqM7E30-8s51os.woff2', file: 'barlow-600.woff2' },
  { url: 'https://fonts.gstatic.com/s/barlowcondensed/v13/HTx3L3I-JCGChYJ8VI-L6OO_au7B6xHT2g.woff2', file: 'barlow-condensed-400.woff2' },
  { url: 'https://fonts.gstatic.com/s/barlowcondensed/v13/HTxwL3I-JCGChYJ8VI-L6OO_au7B4873z3bWuQ.woff2', file: 'barlow-condensed-600.woff2' },
  { url: 'https://fonts.gstatic.com/s/barlowcondensed/v13/HTxwL3I-JCGChYJ8VI-L6OO_au7B46r2z3bWuQ.woff2', file: 'barlow-condensed-700.woff2' },
  { url: 'https://fonts.gstatic.com/s/gildadisplay/v20/t5tmIRoYMoaYG0WEOh7HwMeR3T7Prw.woff2', file: 'gilda-display-400.woff2' },
];

function download(url) {
  return new Promise((resolve, reject) => {
    https.get(url, (res) => {
      if (res.statusCode !== 200) {
        reject(new Error(`${url} => ${res.statusCode}`));
        return;
      }
      const chunks = [];
      res.on('data', (chunk) => chunks.push(chunk));
      res.on('end', () => resolve(Buffer.concat(chunks)));
      res.on('error', reject);
    }).on('error', reject);
  });
}

async function main() {
  if (!fs.existsSync(FONTS_DIR)) {
    fs.mkdirSync(FONTS_DIR, { recursive: true });
  }
  for (const { url, file } of FONT_FILES) {
    const outPath = path.join(FONTS_DIR, file);
    process.stdout.write(`Downloading ${file}... `);
    try {
      const buf = await download(url);
      fs.writeFileSync(outPath, buf);
      console.log('ok');
    } catch (e) {
      console.log('FAIL', e.message);
    }
  }
  console.log('Done. Fonts saved to public/fonts/');
}

main();
