
const stores = {
    "hashScreen": {
        "name": "hashScreen",
        "key": "name"
    },
    "userObjectsData": {
        "name": "userObjectsData",
        "key": ["currentUser", "parentScreenName", "controlID", "propertyName"]
    }
};

var db;
function initIndexedDBSupported($scope, projectTitle) {

    if (!Modernizr.indexeddb)
        return false;

    var request = window.indexedDB.open(projectTitle, 2);

    // Seed the database with entries if it doesn't exist
    request.onupgradeneeded = function (e) {
        var db = e.target.result;

        if (e.oldVersion < 1) {
            var storeScreen = db.createObjectStore(stores.hashScreen.name, { keyPath: stores.hashScreen.key });
            db.createObjectStore(stores.userObjectsData.name, { keyPath: stores.userObjectsData.key });
        }

        // storeproject.createIndex("hash", "hash", { unique: true });
    };

    // Query the database and initialize the UI
    request.onsuccess = function (e) {
        db = e.target.result;
        $scope.$broadcast(onindexedDBReady, projectTitle);
    };

    request.onerror = function (ex) {

        $scope.$broadcast(onindexedDBReady, null);
        $scope.$broadcast(onError, { e: ex, value: 'indexedDB' });
        console.log('indexedDB: cannot create the indexdb for the project');
    };

    return true;
}

function getRecordKey(sourceObj, keysObj) {
    var keys = [];
    for (var i = 0; i < keysObj.length; i++) {
        if (!(keysObj[i] in sourceObj))
            return false;
        keys.push(sourceObj[keysObj[i]]);
    }
    return keys;
}

var controlPropertiesDB = function () {
    var valueFieldName = "propertyValue";
    return {
        add: function (scope, propertyRecord, propertyValue) {
            if (!db)
                return false;

            propertyRecord["currentUser"] = scope.currentUser ? scope.currentUser.name : "";
            var recordKey = getRecordKey(propertyRecord, stores.userObjectsData.key);
            if (!recordKey) {
                console.log("indexedDB: error adding control property: missing object store keys");
                return false;
            }

            var transaction = db.transaction(stores.userObjectsData.name, "readwrite");
            var store = transaction.objectStore(stores.userObjectsData.name);

            var request = store.delete(recordKey);

            var onDeletedOldRecord = function (event) {
                propertyRecord[valueFieldName] = propertyValue;
                request = store.add(propertyRecord);
                request.onerror = function (e) {
                    console.log('indexedDB: error adding control property: add failed');
                };
            };
            request.onsuccess = onDeletedOldRecord;
            request.onerror = onDeletedOldRecord;
        },
        delete: function (scope, propertyRecord) {
            if (!db)
                return false;

            propertyRecord["currentUser"] = scope.currentUser ? scope.currentUser.name : "";
            var recordKey = getRecordKey(propertyRecord, stores.userObjectsData.key);
            if (!recordKey) {
                console.log("indexedDB: error deleting control property: missing object store keys");
                return false;
            }

            var transaction = db.transaction(stores.userObjectsData.name, "readwrite");
            var store = transaction.objectStore(stores.userObjectsData.name);

            var request = store.delete(recordKey);
            request.onerror = function (e) {
                console.log('indexedDB: error deleting control property: delete failed');
            };
        },
        get: function (scope, propertyRecord) {
            return new Promise(function (resolve, reject) {
                if (!db)
                    reject(false);

                propertyRecord["currentUser"] = scope.currentUser ? scope.currentUser.name : "";
                var recordKey = getRecordKey(propertyRecord, stores.userObjectsData.key);
                if (!recordKey) {
                    reject(false);
                }

                var request = db.transaction(stores.userObjectsData.name).objectStore(stores.userObjectsData.name).get(recordKey);

                request.onsuccess = function (event) {
                    if (!event.target.result || !(valueFieldName in event.target.result)) {
                        if (propertyRecord["currentUser"]) {
                            propertyRecord["currentUser"] = "";
                            var recordKey = getRecordKey(propertyRecord, stores.userObjectsData.key);
                            if (!recordKey) {
                                reject(false);
                            }
                            var request2 = db.transaction(stores.userObjectsData.name).objectStore(stores.userObjectsData.name).get(recordKey);
                            request2.onsuccess = function (event) {
                                if (!event.target.result || !(valueFieldName in event.target.result))
                                    resolve(undefined);
                                else
                                    resolve(event.target.result[valueFieldName]);
                            };
                            request2.onerror = function (e) {
                                reject(false);
                            };
                        }
                        else
                            resolve(undefined);
                    }
                    else {
                        resolve(event.target.result[valueFieldName]);
                    }
                };
                request.onerror = function (e) {
                    reject(false);
                };
            });
        }
    }
}();

function addScreenHash(screenHash) {
    if (!db)
        return false;

    var transaction = db.transaction(stores.hashScreen.name, "readwrite");
    var store = transaction.objectStore(stores.hashScreen.name);

    var request = store.delete(screenHash.name);
    request.onsuccess = function (event) {
        request = store.add(screenHash);
        request.onerror = function (e) {
            console.log('indexedDB: error adding hash screen to indexdb');
        };
    };
    request.onerror = function (event) {
        request = store.add(screenHash);
        request.onerror = function (e) {
            console.log('indexedDB: error adding hash screen to indexdb');
        };
    };

    return true;
}

function getScreenHash($scope, name) {
    if (!db) {
        $scope.$broadcast(onscreenHashReady + name, null);
        return false;
    }

    var request = db.transaction(stores.hashScreen.name).objectStore(stores.hashScreen.name).get(name);
    request.onsuccess = function (event) {
        $scope.$broadcast(onscreenHashReady + name, event.target.result);
    };
    request.onerror = function (e) {
        console.log('indexedDB: error getting hash screen from indexdb');
        $scope.$broadcast(onscreenHashReady + name, null);
    };

    return true;
}
