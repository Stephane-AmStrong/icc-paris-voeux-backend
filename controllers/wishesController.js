const Wish = require("../models/Wish");

// @desc Get all wishes
// @route GET /wishes
// @access Private
const getAllWishes = async (req, res) => {
  // Get all wishes from MongoDB
  const wishes = await Wish.find().lean();

  res.json(wishes);
};

// @desc Create new wish
// @route POST /wishes
// @access Private
const createWish = async (req, res) => {
  const {
    spiritually,
    familiallyRelationally,
    financiallyMaterially,
    professionallyAcademically,
    email,
    other,
  } = req.body;

  const validationError = validateData({
    spiritually,
    familiallyRelationally,
    financiallyMaterially,
    professionallyAcademically,
    other,
    email,
  });

  if (validationError) {
    return res
      .status(validationError.status)
      .json({ message: validationError.message });
  }

  // Check for duplicate title
  const duplicate = await Wish.findOne({ email }).lean().exec();

  if (duplicate) {
    return res.status(409).json({
      message: `Le mail "${email}" existe déjà veuillez utiliser un autre`,
    });
  }

  // Create and store the new user
  const wish = await Wish.create({
    spiritually,
    familiallyRelationally,
    financiallyMaterially,
    professionallyAcademically,
    email,
    other,
  });

  if (wish) {
    // Created
    return res.status(201).json(wish);
  } else {
    return res.status(400).json({ message: "Invalid wish data received" });
  }
};

// @desc Update a wish
// @route PATCH /wishes
// @access Private
const updateWish = async (req, res) => {
  const {
    id,
    spiritually,
    familiallyRelationally,
    financiallyMaterially,
    professionallyAcademically,
    email,
    other,
  } = req.body;

  const validationError = validateData({
    spiritually,
    familiallyRelationally,
    financiallyMaterially,
    professionallyAcademically,
    other,
    email,
  });

  if (validationError) {
    return res
      .status(validationError.status)
      .json({ message: validationError.message });
  }

  // Confirm wish exists to update
  const wish = await Wish.findById(id).exec();

  if (!wish) {
    return res.status(400).json({ message: "Wish not found" });
  }

  // Check for duplicate title
  const duplicate = await Wish.findOne({ title })
    .collation({ locale: "en", strength: 2 })
    .lean()
    .exec();

  // Allow renaming of the original wish
  if (duplicate && duplicate?._id.toString() !== id) {
    return res.status(409).json({ message: "Duplicate wish title" });
  }

  wish.user = user;
  wish.title = title;
  wish.text = text;
  wish.completed = completed;

  const updatedWish = await wish.save();

  res.json(`'${updatedWish.title}' updated`);
};

// @desc Delete a wish
// @route DELETE /wishes
// @access Private
const deleteWish = async (req, res) => {
  const { id } = req.body;

  const validationError = validateData({
    spiritually,
    familiallyRelationally,
    financiallyMaterially,
    professionallyAcademically,
    other,
    email,
  });

  if (validationError) {
    return res
      .status(validationError.status)
      .json({ message: validationError.message });
  }

  // Confirm wish exists to delete
  const wish = await Wish.findById(id).exec();

  if (!wish) {
    return res.status(400).json({ message: "Wish not found" });
  }

  const result = await wish.deleteOne();

  const reply = `Wish '${result.title}' with ID ${result._id} deleted`;

  res.json(reply);
};

function validateData({
  spiritually,
  familiallyRelationally,
  financiallyMaterially,
  professionallyAcademically,
  other,
  email,
}) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/; // Basic regex for email validation

  // Vérifier que l'un des voeux est présent
  if (
    !spiritually &&
    !familiallyRelationally &&
    !financiallyMaterially &&
    !professionallyAcademically &&
    !other
  ) {
    return { status: 400, message: "Veuillez formuler au moins un voeux" };
  }

  // Vérifier que l'email est présent
  if (!email) {
    return { status: 400, message: "Le mail est requis" };
  }

  // Vérifier la validité de l'email avec un regex
  if (!emailRegex.test(email)) {
    return { status: 400, message: "Seul les mail valide sont acceptés" };
  }

  return null; // Aucune erreur, validation réussie
}

module.exports = {
  getAllWishes,
  createWish,
  updateWish,
  deleteWish,
};
