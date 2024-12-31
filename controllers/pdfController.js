const puppeteer = require("puppeteer");
const path = require("path");

const getPDF = async (req, res) => {
  const encodedPage = req.query.page;

  if (!encodedPage) {
    return res.status(400).send("Le paramètre 'page' est manquant");
  }

  const pageUrl = decodeURIComponent(encodedPage);

  let browser;

  const tempFilePath = path.join(__dirname, "..", "pdfs", "tmp.pdf");

  try {
    // Lancer un navigateur headless
    browser = await puppeteer.launch({ headless: true });

    // Ouvrir une nouvelle page
    const page = await browser.newPage();

    await page.setViewport({ width: 1920, height: 1080 });

    // Naviguer vers l'URL cible
    await page.goto(pageUrl, { waitUntil: "networkidle2" });

    // Émuler les styles CSS pour écran
    // await page.emulateMediaType("screen");

    // Générer le PDF directement en mémoire
    const pdfBuffer = await page.pdf({
      path: tempFilePath,
      format: "A4",
      printBackground: true, // Inclure les arrière-plans
    });

    await browser.close();
    res.set({
      "Content-Type": "application/pdf",
      "Content-Length": pdfBuffer.length,
    });

    res.sendFile(tempFilePath);
  } catch (error) {
    console.error("Erreur lors de la génération du PDF :", error);
    res.status(500).send("Erreur lors de la génération du PDF.");
  } finally {
    if (browser) await browser.close();
  }
};

module.exports = {
  getPDF,
};
