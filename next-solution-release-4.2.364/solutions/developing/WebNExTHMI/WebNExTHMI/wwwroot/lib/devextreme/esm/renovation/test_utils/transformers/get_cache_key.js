/**
 * DevExtreme (esm/renovation/test_utils/transformers/get_cache_key.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var crypto = require("crypto");
module.exports = function(fileData, filePath, configStr, transformerFileSrc) {
    return crypto.createHash("md5").update(transformerFileSrc).update("\0", "utf8").update(fileData).update("\0", "utf8").update(filePath).update("\0", "utf8").update(configStr).digest("hex")
};
