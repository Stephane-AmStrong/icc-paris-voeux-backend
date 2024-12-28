const express = require("express");
const router = express.Router();
const wishesController = require("../controllers/wishesController");

router
  .route("/")
  .get(wishesController.getAllWishes)
  .post(wishesController.createWish)
  .patch(wishesController.updateWish)
  .delete(wishesController.deleteWish);

module.exports = router;
