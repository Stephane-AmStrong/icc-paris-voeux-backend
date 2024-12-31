// const allowedOrigins = [process.env.ALLOWED_ORIGIN.split(",")];
const allowedOrigins = process.env.ALLOWED_ORIGIN.split(",");

module.exports = allowedOrigins;
