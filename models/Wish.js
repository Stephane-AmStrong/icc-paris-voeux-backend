const mongoose = require("mongoose");

const wishSchema = new mongoose.Schema({
  spiritually: {
    type: String,
    required: false,
  },
  familiallyRelationally: {
    type: String,
    required: false,
  },
  financiallyMaterially: {
    type: String,
    required: false,
  },
  professionallyAcademically: {
    type: String,
    required: false,
  },
  other: {
    type: String,
    required: false,
  },
  email: {
    type: String,
    required: true,
  },
});

module.exports = mongoose.model("Wish", wishSchema);
