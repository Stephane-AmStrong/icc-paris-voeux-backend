const express = require("express");
const router = express.Router();
const wishesController = require("../controllers/pdfController");

router.route("/print").get(wishesController.getPDF);

module.exports = router;
